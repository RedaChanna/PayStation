using PayStationSW.Protocols.Web2Park;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PayStationSW.DataBase;

namespace PayStationSW.Devices
{

    public class Web2ParkDevice : Device
    {
        private readonly InterfaceWeb2Park _protocol;
        private readonly ApplicationDbContext _context;
        private Timer _statusPollingTimer;
        private bool _isPollingActive;


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

        private void PollStatus(object state)
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter = _protocol.ListenerCommand();

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
                    foreach (byte[] byteArray in task.Result.responseByte)
                    {
                        foreach (byte b in byteArray)
                        {
                            Console.Write($"Response: 0x{b:X2} ");
                        }
                        Console.WriteLine(); // New line for each byte array
                    }
                }
            });


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