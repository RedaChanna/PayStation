using PayStationSW.DataBase;
using PayStationSW.Devices;
using PayStationSW.Protocols.Printer;
using PayStationSW;
using System.Diagnostics;
using Org.BouncyCastle.Asn1.Sec;

namespace PayStationSW.Devices
{
    public class Web2Park : Device
    {

        private readonly ApplicationDbContext _context;

        public CommandParameter _listenerCommand;
        private CancellationTokenSource _cancellationTokenSource;

        public Web2Park(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.Web2Park;
            _context = context;
            _listenerCommand = new CommandParameter();
            _listenerCommand.expectedStringOrHEX = false;
            _listenerCommand.messageResponseHaveStartingByte = true;
            _listenerCommand.messageResponseHaveEndingByte = true;
            _listenerCommand.messageStartingBytes = [0x01];
            _listenerCommand.messageEndingBytes = [0x04];
            _listenerCommand.expectedResponse = true;
            StartListener();

        }
        public override void ApplyConfig()
        {

        }
        public static async Task<Web2Park> CreateAsync(ApplicationDbContext context)
        {
            var device = new Web2Park(context);
            return device;
        }





        public async void StartListener()
        {
            _listenerCommand = new CommandParameter();
            _listenerCommand.expectedStringOrHEX = false;
            _listenerCommand.messageResponseHaveStartingByte = true;
            _listenerCommand.messageResponseHaveEndingByte = true;
            _listenerCommand.messageStartingBytes = [0x01];
            _listenerCommand.messageEndingBytes = [0x04];
            _listenerCommand.expectedResponse = true;
            _listenerCommand = await Command(_listenerCommand);
            bool ReciveSomething = _listenerCommand.validatedCommand;


            if (ReciveSomething)
            {

                foreach (byte[] b in _listenerCommand.responseByte)
                {
                    string hex1 = BitConverter.ToString(b.ToArray()).Replace("-", " ");
                    Console.WriteLine($"\n\nil messaggio ricevuto è: {hex1}");
                }

            }
           
        }
    }
}