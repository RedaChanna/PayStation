using PayStationSW.DataBase;
using PayStationSW.Protocols.POS;
using PayStationSW.Protocols.Printer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PayStationSW.Devices
{
    public class PrinterDevice : Device
    {
        private readonly InterfacePrinterProtocol _protocol;
        private readonly ApplicationDbContext _context;

        public PrinterDevice(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.Printer;
            _context = context;
            _protocol = new ProtocolCashino(this);
        }
        public override void ApplyConfig()
        {

        }

        public static async Task<PrinterDevice> CreateAsync(ApplicationDbContext context)
        {
            var device = new PrinterDevice(context);
            device.Config.IsSetUp = await device.PreSetting(); 
            return device;
        }
        private async Task<bool> PreSetting()
        {
            try
            {
                string codeDevice = "4";
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
        public async Task<string> Enable()
        {
            return "Coin device still enable";
        }
        public async Task<string> Disable()
        {
            return "Coin device still disable";

        }
        public async Task<string> PrintTestPage()
        {
            bool IsPrinted = false;
            if (_protocol is ProtocolCashino cashinoProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.PrintTestPage();
                _commandParameter = await this.Command(_commandParameter);
                IsPrinted = _commandParameter.validatedCommand;
            }
            if (IsPrinted)
            {
                return "Printer device printed test page correctly.";
            }
            else
            {
                return "Coin device dosen't print test page correctly";
            }
        }
    }
}
