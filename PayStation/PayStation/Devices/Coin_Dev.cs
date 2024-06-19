using Microsoft.AspNetCore.SignalR.Protocol;
using PayStationSW.DataBase;
using PayStationSW.Protocols.Coin;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections;


namespace PayStationSW.Devices
{

    public class CoinDevice : Device
    {
        private readonly InterfaceCoinProtocol _protocol;
        private readonly ApplicationDbContext _context;
        private Timer _statusPollingTimer;
        private bool _isPollingActive;

        public bool En_CoinIntroducedLstn { get; set; }

        public CoinDevice(ApplicationDbContext context)
        {
            DeviceType = DeviceEnum.Coin;

            _context = context;
            this.ErrorReceived += Device_ErrorReceived;
            _protocol = new ProtocolMDB_RS323(this);
            En_CoinIntroducedLstn = true;
        }
        public override void ApplyConfig()
        {

        }
        #region Polling
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
        private volatile bool waitingResponse = false; 
        private async void PollStatus(object state)
        {
            try
            {
                CommandParameter commandParameter;
                if (!waitingResponse)
                {
                    waitingResponse = true;

                    commandParameter = new CommandParameter();
                    commandParameter = _protocol.ListenerCommand();
                    commandParameter = await this.Command(commandParameter);
                    if (commandParameter.validatedCommand)
                    {
                        await HandlePollingMessages(commandParameter.responseByte[0]);
                        waitingResponse = false;
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
        public async Task HandlePollingMessages(byte[] responseByte)
        {

            if (responseByte == null) {
                Console.WriteLine("Received byte array is null");
                return;
            }
   

            // Estrai il sesto e il settimo byte per la verifica
            byte sixthByte = responseByte[4];

            // Determina l'azione in base ai valori specifici dei byte
            if (sixthByte == 0x30)
            {
                Console.WriteLine("Introdotti 10 Centesimi");
            }
            else if (sixthByte == 0x31)
            {
                Console.WriteLine("Introdotti 20 Centesimi");
            }
            else if (sixthByte == 0x32)
            {
                Console.WriteLine("Introdotti 50 Centesimi");
            }
            else if (sixthByte == 0x33)
            {
                Console.WriteLine("Introdotti 1 Euro");

            }
            else if (sixthByte == 0x34)
            {
                Console.WriteLine("Introdotti 2 Euro");

            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }
        #endregion
        public static async Task<CoinDevice> CreateAsync(ApplicationDbContext context)
        {
            var device = new CoinDevice(context);
            //device.Config.IsSetUp = await device.PreSetting();
            return device;
        }
        public async Task<string> PreSettingSequence()
        {
            try
            {
                var retrayCount = 0;
                
                while (!Config.IsPreSetted  || retrayCount < 3)
                {
                    retrayCount += 1;
                    await Reset();
                    if (Config.IsReset)
                    {
                        await SetUp();
                        if (Config.IsSetUp)
                        {
                            await SetUpExpansion();
                            if (Config.IsSetUpExpansion)
                            {
                                await SetUpFeauture();
                                if (Config.IsSetUpFeauture)
                                {
                                    Config.IsPreSetted = true;
                                }
                            }
                        }
                    }
                }
                if (Config.IsPreSetted)
                {
                    return "Coin device pre-setted correctly.";
                }
                else
                {
                    return ($"Coin device is not pre-reset correctly: " +
                        $"Reset result:{Config.IsReset}, " +
                        $"Set up result:{Config.IsSetUp}, " +
                        $"Set up expansion result:{Config.IsSetUpExpansion}, " +
                        $"Set up feauture result:{Config.IsSetUpFeauture}.");
                }
            }
            catch (Exception ex)
            {
                return ($"Error occurred: {ex.Message}");
            }
        }
        public async Task<string> Reset()
        {
            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.ResetCommand();
                _commandParameter = await this.Command(_commandParameter);
                Config.IsReset = _commandParameter.validatedCommand;
            }
            if (Config.IsReset)
            {
                return "Coin device reset correctly.";
            }
            else
            {
                return "Coin device is NOT reset correctly";
            }
        }
        public async Task<string> SetUp()
        {
            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.SetUpCommand();
                _commandParameter = await this.Command(_commandParameter);
                Config.IsSetUp = _commandParameter.validatedCommand;
            }
            if (Config.IsSetUp)
            {
                return "Coin device is set up correctly.";
            }
            else
            {
                return "Coin device is NOT set up correctly";
            }
        }
        public async Task<string> SetUpExpansion()
        {
            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.SetUpExpansionCommand();
                _commandParameter = await this.Command(_commandParameter);

                Config.IsSetUpExpansion = _commandParameter.validatedCommand;
            }
            if (Config.IsSetUpExpansion)
            {
                return "Coin device expansion set up correctly.";
            }
            else
            {
                return "Coin device expansion is NOT set up correctly";
            }
        }
        public async Task<string> SetUpFeauture()
        {

            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.SetUpFeautureCommand();
                _commandParameter = await this.Command(_commandParameter);
                Config.IsSetUpFeauture = _commandParameter.validatedCommand;
            }
            if (Config.IsSetUpFeauture)
            {
                return "Coin device set up feauture correctly.";
            }
            else
            {
                return "Coin device is NOT set up feature correctly";
            }
        }
        public async Task<string> InhibitionCommand()
        {
            try
            {
                if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
                {
                    if (!Config.IsConnected)
                    {
                        return "Coin device is not connected.";
                    }
                    if (!Config.IsPreSetted)
                    {
                        return "Coin device is not pre-setted.";
                    }
                    CommandParameter _commandParameter = new CommandParameter();
                    _commandParameter = _protocol.InhibitionCommand();
                    _commandParameter = await this.Command(_commandParameter);
                    Config.IsInhibited = _commandParameter.validatedCommand;
                }
                if (Config.IsInhibited)
                {
                    return "Coin device is Inhibited.";
                }
                else
                {
                    return "Coin device still Disinhibited";
                }

            }
            catch (Exception ex)
            {
                return ($"Error occurred: {ex.Message}");
            }
            
        }
        public async Task<string> DisinhibitionCommand()
        {
            try
            {
                if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
                {
                    if (!Config.IsConnected)
                    {
                        return "Coin device is not connected.";
                    }
                    if (!Config.IsPreSetted)
                    {
                        return "Coin device is not pre-setted.";
                    }
                    CommandParameter _commandParameter = new CommandParameter();
                    _commandParameter = _protocol.DisinhibitionCommand();
                    _commandParameter = await this.Command(_commandParameter);
                    Config.IsInhibited = !_commandParameter.validatedCommand;
                }
                if (!Config.IsInhibited)
                {
                    return "Coin device is Disinhibited.";
                }
                else
                {
                    return "Coin device still Inhibited";
                }

            }
            catch (Exception ex)
            {
                return ($"Error occurred: {ex.Message}");
            }
        }
        public void SetAmountToDeliver(int changeAmount)
        {
            /* Console.WriteLine($"Requesting change dispense: {changeAmount} cents...");

             // Check if the changeAmount exceeds the maximum allowed value
             if (changeAmount > 2550)
             {
                 Console.WriteLine("Change amount exceeds the maximum limit of €25.50. Sending two subsequent messages.");

                 // First message to wait for the disbursement of the remainder
                 byte[] waitMessage = { 0x0F, 0x02, 0xFF };
                 string waitAck = "00 00 {0D}{0A}";

                 string waitResponse = SendMessageWithRetry(waitMessage);

                 if (waitResponse == waitAck)
                 {
                     Console.WriteLine("Waiting for the disbursement of the remainder acknowledged.");

                     // Second message to return the remainder
                     byte[] returnMessage = { 0x0F, 0x02, (byte)((changeAmount - 2550) / 10) };
                     string returnAck = "00 00 {0D}{0A}";

                     string returnResponse = SendMessageWithRetry(returnMessage);

                     if (returnResponse == returnAck)
                     {
                         Console.WriteLine("Remainder return acknowledged.");
                     }
                     else
                     {
                         Console.WriteLine("Remainder return not acknowledged. Gryphon may be busy.");
                     }
                 }
                 else
                 {
                     Console.WriteLine("Waiting for the disbursement of the remainder not acknowledged. Gryphon may be busy.");
                 }
             }
             else
             {
                 // Single message for change within the allowed limit
                 byte[] remainderToDeliverRequest = { 0x0F, 0x02, (byte)(changeAmount / 10) };
                 string remainderToDeliverAck = "00 00 {0D}{0A}";

                 string remainderToDeliverResponse = SendMessageWithRetry(remainderToDeliverRequest);

                 if (remainderToDeliverResponse == remainderToDeliverAck)
                 {
                     Console.WriteLine("Remainder to deliver acknowledged.");

                     // Wait for change dispense
                     Thread.Sleep(300);

                     // Request coins dispensed for change
                     RequestHowManyCoinsDispensed();
                 }
                 else
                 {
                     Console.WriteLine("Remainder to deliver not acknowledged. Gryphon may be busy.");
                 }
             }
             // Check if the change has been delivered successfully
             AmountDeliveryFinish();

            */
        }
    }
}