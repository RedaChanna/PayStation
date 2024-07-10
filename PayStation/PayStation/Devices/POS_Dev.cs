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
using System.Collections.Generic;
using System.Runtime.InteropServices;




namespace PayStationSW.Devices
{
    public class POSDevice : Device
    {
        private readonly    InterfacePOSProtocol _protocol;
        private readonly    ApplicationDbContext _context;
        //0x02 teminal ID from Hex to byte and 0x30 
        public byte[] ID_Terminal { get; set; } = [0x31, 0x31, 0x30, 0x37, 0x37, 0x31, 0x32, 0x38];

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
            CommandParameter _commandParameter = new CommandParameter();
            byte[] ConcatArray = [];
            var result = "";
            if (_protocol is ProtocolIngenico ingenicoProtocol)
            {
                _commandParameter = _protocol.ActivationCommand(ID_Terminal);
                _commandParameter = await this.Command(_commandParameter);
                if (_commandParameter.validatedCommand)
                {
                    Config.IsSetUp = _commandParameter.validatedCommand;
                   
                    Console.WriteLine("Recived the ACK esit POS Activation");
                }
                else
                {
                    Console.WriteLine("Recived the NCK");
                    result = "Could not activate POS";
                    return result;  
                }

            }
           /* if (Config.IsSetUp)
            {
                Console.WriteLine(result);
                _commandParameter = new CommandParameter();

                _commandParameter = _protocol.ACKCommand();
                _commandParameter = await this.Command(_commandParameter);
            }*/

            return result;


        }

        public async Task<string> SetImportoPos()
        {
            CommandParameter _commandParameter = new CommandParameter();
            byte[] ConcatArray = [];


            if (_protocol is ProtocolIngenico ingenicoProtocol)
            {
                _commandParameter = _protocol.ActivationCommand(ID_Terminal);
                _commandParameter = await this.Command(_commandParameter);
                if (_commandParameter.validatedCommand)
                {
                    Console.WriteLine("Recived the ACK command now should wait Activation comand from POS");
                }
                
            }else

            {
                
            }





            /*
            CommandParameter _commandParameter = new CommandParameter();

            if (_protocol is ProtocolIngenico ingenicoProtocol)
            {



                if (Config.IsConnected && Config.IsSetUp)
                {
                    _commandParameter = new CommandParameter();
                    _commandParameter = _protocol.SendAmount("11077128",10);
                    _commandParameter = await this.Command(_commandParameter);
                    return "Pos importo is setted.";
                }
            }
            */
            return "result";

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
        public async Task<string> InhibitionCommand()
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
        public async Task<string> DisinhibitionCommand()
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