using PayStationSW.Protocols.Web2Park;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PayStationSW.DataBase;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Utilities;

namespace PayStationSW.Devices
{

    public class Web2ParkDevice : Device
    {
        private readonly InterfaceWeb2Park _protocol;
        private readonly ApplicationDbContext _context;
        private Timer _statusPollingTimer;
        private bool _isPollingActive;
        private bool _isComunicating = false;

        public Web2ParkDevice(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.Web2Park;

            _context = context;
            this.ErrorReceived += Device_ErrorReceived;
            _protocol = new ProtocolWeb2Park(this);
        }
        public override void ApplyConfig()
        {

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
                _statusPollingTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Ferma il timer
                _statusPollingTimer?.Dispose(); // Rilascia le risorse del timer
                _isPollingActive = false;
            }
        }
        private volatile bool waitingResponse = false;  // Now a class-level variable
        private volatile bool _receved_command = true;
        private async void PollStatus(object state)
        {
            try
            {
                CommandParameter commandParameter;
                if (_isComunicating && !waitingResponse)
                {
                    waitingResponse = true;
                    if (_receved_command)
                    {
                        Console.WriteLine("Ack Comand Send");
                        commandParameter = _protocol.AckCommand();
                        var statusTask = await this.Command(commandParameter);
                    }
                    else
                    {
                        Console.WriteLine("Ack Polling send");
                        commandParameter = _protocol.AckPolling();
                        var statusTask = await this.Command(commandParameter);
                    }

                    if (commandParameter.validatedCommand)
                    {
                        await HandleMessage(commandParameter.responseByte[0]);
                    }

                    waitingResponse = false; // Reset here after handling message
                }
                else if (!_isComunicating && !waitingResponse)
                {
                    commandParameter = _protocol.ListenerCommand();
                    var statusTask = await this.Command(commandParameter);
                    if (statusTask.validatedCommand)
                    {
                        _isComunicating = true;
                        Console.WriteLine("Message received and validated");
                    }
                    else
                    {
                        Console.WriteLine("No Message from Web2Park received");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in PollStatus: " + ex.Message);
            }
        }

        public async Task HandleMessage(byte[] responseByte)
        {

            // Continua solo se responseByte non è null e ha almeno 7 byte
            if (responseByte == null || responseByte.Length < 7)
            {
                Console.WriteLine("Received byte array is null or too short to process.");
                return;
            }

            // Estrai il sesto e il settimo byte per la verifica
            byte sixthByte = responseByte[5];
            byte seventhByte = responseByte[6];

            // Determina l'azione in base ai valori specifici dei byte
            if (sixthByte == 0x31 && seventhByte == 0x34)
            {
                _receved_command = false;
                Console.WriteLine("Cassa ask status");
            }
            else if (sixthByte == 0x39 && seventhByte == 0x34)
            {
                _receved_command = false;
                Console.WriteLine("Cassa ask status");
            }
            else if (sixthByte == 0x30 && seventhByte == 0x38)
            {
                _receved_command = true;
                Console.WriteLine("Set Importo");
            }
            else if (sixthByte == 0x31 && seventhByte == 0x32)
            {
                _receved_command = true;
                Console.WriteLine("Annulla Operazione");
            }
            else if (sixthByte == 0x32 && seventhByte == 0x32)
            {
                _receved_command = true;
                Console.WriteLine("Richiesta Stato Livelli Monete");
            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }




        public async Task<string> SendTestMessage()
        {
            bool IsPrinted = false;
            if (_protocol is ProtocolWeb2Park web2ParkProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.SendTestCommand();
                _commandParameter = await this.Command(_commandParameter);
            }
            if (IsPrinted)
            {
                return "Sended message test to web 2 park correctly.";
            }
            else
            {
                return "Coulden't send message test to web 2 park correctly";
            }
        }




        public static async Task<Web2ParkDevice> CreateAsync(ApplicationDbContext context)
        {
            var device = new Web2ParkDevice(context);
            //device.Config.IsSetUp = await device.PreSetting();
            return device;
        }
    }
}