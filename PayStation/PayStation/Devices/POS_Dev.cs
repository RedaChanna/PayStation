 using PayStationSW.DataBase;
using PayStationSW.Protocols.Coin;
using PayStationSW.Protocols.POS;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;



namespace PayStationSW.Devices
{
    public class POSDevice : Device
    {
        private readonly InterfacePOSProtocol _protocol;
        private readonly ApplicationDbContext _context;

        public POSDevice(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.Pos;

            _context = context;
            _protocol = new ProtocolIngenico(this);
        }
        public override void ApplyConfig()
        {

        }
        public static async Task<POSDevice> CreateAsync(ApplicationDbContext context)
        {
            var device = new POSDevice(context);
            device.Config.IsSetUp = await device.PreSetting();
            return device;
        }

        private async Task<bool> PreSetting()
        {
            try
            {
                string codeDevice = "3";
                // Usa FirstOrDefaultAsync invece di First
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




        public async Task<string> ActivatePOS()
        {
            bool IsPosActivated = false;
            if (_protocol is ProtocolCashino cashinoProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.ActivationCommand();

                _commandParameter = await this.Command(_commandParameter);
                Console.WriteLine(_commandParameter.responseByte);
                IsPosActivated = _commandParameter.validatedCommand;
            }
            if (IsPosActivated)
            {
                return "Pos is activated.";
            }
            else
            {
                return "Pos in not activated.";
            }
        }




        public async void ResetDevice()
        {
            Config.IsReset = await _protocol.resetDevice();
            if (Config.IsReset)
            {
                Console.WriteLine("Reset acknowledged.");
            }
            else
            {
                Console.WriteLine("Reset not acknowledged. Gryphon disabled.");
            }
        }
        public async Task<string> Enable()
        {
            Config.IsEnabled = await _protocol.enableDevice();

            if (Config.IsEnabled)
            {
                return "Coin Device enabled.";
            }
            else
            {
                return  "Coin Device not enabled.";
            }
        }
        public async Task<string> Disable()
        {
            Config.IsEnabled = await _protocol.disableDevice();

            if (Config.IsEnabled)
            {
                return "Coin Device enabled.";
            }
            else
            {
                return "Coin Device not enabled.";
            }
        }
        public override void IdentifyDevice()
        {
        }
    }
}

/*
public class PosManagment : PosDevice
{
    public bool IsDevicePresent { get; set; }
    public PosManagment()
    {
        IsDevicePresent = true; // Assuming the POS device is always present
    }
    // Example properties
    public string TransactionId { get; private set; }
    // Example methods
    public void ProcessTransaction(decimal amount)
    {
        if (IsEnabled)
        {
            // Your logic to process the transaction
            TransactionId = GenerateTransactionId();
        }
        else
        {
            Console.WriteLine("POS device is not enabled. Cannot process transactions.");
        }
    }
    private string GenerateTransactionId()
    {
        // Your logic to generate a unique transaction ID
        return Guid.NewGuid().ToString();
    }
}
*/