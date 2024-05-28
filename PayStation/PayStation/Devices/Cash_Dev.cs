using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PayStationSW.DataBase;
using PayStationSW.Protocols.Cash;
using PayStationSW.Protocols.Coin;
using PayStationSW.RESTAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static PayStationSW.ProtocolVega100;
using Microsoft.EntityFrameworkCore;


namespace PayStationSW.Devices
{
    public class CashDevice : Device
    {

        private readonly ApplicationDbContext _context;

        public bool IsDevicePresent { get; set; }
        public RCModule RCModule { get; private set; }
        public TwinModule TwinModule { get; private set; }
        public bool IsRCModuleType1 { get; private set; }
        public bool IsRCModuleType2 { get; private set; }
        private bool PayedOut;
        private bool PoweredUp;

        private Timer _statusPollingTimer;
        private bool _isPollingActive;

        private readonly InterfaceCashProtocol _protocol;
        public CashDevice(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.Cash;

            _context = context;
            _protocol = new ProtocolVega100(this);
            this.ErrorReceived += Device_ErrorReceived;
            RCModule = new RCModule(_context);
            IsRCModuleType1 = true; // Set the default type to 1 (€5 bills)
            IsRCModuleType2 = false;

            // Set RC module parameters (type 1 by default, no bills initially)
            RCModule.IsDevicePresent = false;
            RCModule.BillDenominations = new List<decimal>();
            RCModule.BillCount5 = 0;
            RCModule.BillCount10 = 0;

        }
        public void StartPolling()
        {
            if (!_isPollingActive)
            {
                // Imposta il timer per avviare il polling subito e ripetere ogni 300 ms
                _statusPollingTimer = new Timer(PollStatus, null, 0, 300);
                _isPollingActive = true;
            }
        }

        public void StopPolling()
        {
            if (_isPollingActive)
            {
                _isPollingActive = false;
            }
        }

        private void PollStatus(object state)
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter = _protocol.StatusCommand();
            Task<CommandParameter> statusTask = this.Command(_commandParameter);
            statusTask.ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    // Gestisci eccezioni
                    Console.WriteLine("Error polling status: " + task.Exception.InnerException?.Message);
                }
                else
                {
                    // Processa la risposta
                    Console.WriteLine("Status: " + task.Result.validatedCommand);
                }
            });
        }

        public override void ApplyConfig()
        {

        }

        public static async Task<CashDevice> CreateAsync(ApplicationDbContext context)
        {

            var device = new CashDevice(context);
            device.Config.IsSetUp = await device.PreSetting();
            return device;
        }


        public override void IdentifyDevice()
        {
        }

        public async Task<string> Enable()
        {
            if (_protocol is ProtocolVega100 vegaProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.EnableCommand();
                _commandParameter = await this.Command(_commandParameter);

                Config.IsEnabled = _commandParameter.validatedCommand;
            }
            if (Config.IsEnabled)
            {
                return "Cash device is enable.";
            }
            else
            {
                return "Cash device is disable";
            }

        }
        public async Task<string> Disable()
        {
            if (_protocol is ProtocolVega100 vegaProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.DisableCommand();
                _commandParameter = await this.Command(_commandParameter);

                Config.IsEnabled = _commandParameter.validatedCommand;
            }
            Config.IsEnabled = !Config.IsEnabled;
            if (Config.IsEnabled)
            {
                return "Cash device is enable";
            }
            else
            {
                return "Cash device is disable.";
            }
        }
        private async Task<bool> PreSetting()
        {
            try
            {
                string codeDevice = "2";
                var existingDevice = await _context.DevicesDB
                    .Where(x => x.DeviceType == codeDevice).FirstOrDefaultAsync();
                // Controllo corretto del valore null
                if (existingDevice != null)
                {
                    if (existingDevice.Enabled == "1")
                    {
                        this.Config.IsEnabled = true;
                    }
                    else
                    {
                        this.Config.IsEnabled = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Considera di loggare l'eccezione ex per un debugging più efficace
                return false;
            }
        }



        public async Task<string> Reset()
        {
            if (_protocol is ProtocolVega100 vegaProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.ResetCommand();
                _commandParameter = await this.Command(_commandParameter);

                Config.IsReset = _commandParameter.validatedCommand;
            }
            if (Config.IsReset)
            {
                return "Cash device reset correctly.";
            }
            else
            {
                return "Cash device is NOT reset correctly";
            }
        }

        public async Task<string> Payout()
        {
            if (_protocol is ProtocolVega100 vegaProtocol)
            {
                //PayedOut = await _protocol.PayOutSequence();
                PayedOut = false;
                
            }
            if (PayedOut)
            {
                return "Pay out end correctly.";
            }
            else
            {
                return "Pay out NOT end correctly";
            }
        }
        public async Task<string> PowerUP()
        {
            if (_protocol is ProtocolVega100 vegaProtocol)
            {
                // PoweredUp = await _protocol.PowerUPSequence();
                PoweredUp = false;
            }
            if (PoweredUp)
            {
                return "Power up end correctly.";
            }
            else
            {
                return "Power up NOT ended correctly";
            }
        }

    }

    public class RCModule : Device
    {
        private readonly ApplicationDbContext _context;

        public RCModule(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.RCModule;
            _context = context;
        }


        // Implementa il metodo ApplyConfig
        public override void ApplyConfig()
        {

        }
        public static async Task<RCModule> CreateAsync(ApplicationDbContext context)
        {
            var device = new RCModule(context);
            return device;
        }

        public bool IsDevicePresent { get; set; }
        public List<decimal> BillDenominations { get; set; }
        public int BillCount5 { get; set; }
        public int BillCount10 { get; set; }

        public int GetMaxBillsOfType1()
        {
            // Your logic to determine the maximum bills for type 1
            return 0;
        }

        public int GetMaxBillsOfType2()
        {
            // Your logic to determine the maximum bills for type 2
            return 0;
        }
    }



    public class TwinModule : Device
    {
        private readonly ApplicationDbContext _context;

        public TwinModule(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.TwinModule;
            _context = context;
        }
        public override void ApplyConfig()
        {

        }

        public static async Task<TwinModule> CreateAsync(ApplicationDbContext context)
        {
            var device = new TwinModule(context);
            return device;
        }
        public bool IsDevicePresent { get; set; }
        public List<decimal> BillDenominations { get; set; }
        public int BillCount5 { get; set; }
        public int BillCount10 { get; set; }

        public int GetMaxBillsOfType1()
        {
            // Your logic to determine the maximum bills for type 1
            return 0;
        }

        public int GetMaxBillsOfType2()
        {
            // Your logic to determine the maximum bills for type 2
            return 0;
        }
    }

}