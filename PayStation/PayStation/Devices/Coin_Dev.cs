using Microsoft.AspNetCore.SignalR.Protocol;
using PayStationName.DataBase;
using PayStationName.Protocols.Coin;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace PayStationName.Devices
{

    public class CoinDevice : Device
    {
        private readonly InterfaceCoinProtocol _protocol;
        private readonly ApplicationDbContext _context;


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
        public static async Task<CoinDevice> CreateAsync(ApplicationDbContext context)
        {
            var device = new CoinDevice(context);
            device.Config.IsSetUp = await device.PreSetting();
            return device;
        }

        private async Task<bool> PreSetting()
        {
            try
            {
                string codeDevice = "1";
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




        public async Task<string> SetUp()
        {
            Config.IsSetUp = await _protocol.setUp();
            if (Config.IsSetUp)
            {
                return "Coin device set up correctly.";
            }
            else
            {
                return "Coin device is NOT set up correctly";
            }
        }
        public async Task<string> SetUpFeauture()
        {
            Config.IsSetUpFeauture = await _protocol.setUpFeauture();
            if (Config.IsSetUpFeauture)
            {
                return "Coin device set up feauture correctly.";
            }
            else
            {
                return "Coin device is NOT set up feature correctly";
            }
        }
        public async Task<string> ExpansionSetUp()
        {
            Config.IsExpansionSetUp = await _protocol.expansionSetUp();
            if (Config.IsExpansionSetUp)
            {
                return "Coin device expansion set up correctly.";
            }
            else
            {
                return "Coin device expansion is NOT set up correctly";
            }
        }





        public async Task<string> Reset()
        {
            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.ResetCommand();
                Config.IsReset = await this.Command(_commandParameter);
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
        public async Task<string> EnableCommand()
        {
            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.EnableCommand();
                Config.IsEnabled = await this.Command(_commandParameter);
            }
            if (Config.IsEnabled)
            {
                CoinIntroducedLstn(En_CoinIntroducedLstn);
                return "Coin device is enable.";
            }
            else
            {
                return "Coin device still disable";
            }
        }
        public async Task<string> DisableCommand()
        {
            if (_protocol is ProtocolMDB_RS323 gryphonProtocol)
            {
                CommandParameter _commandParameter = new CommandParameter();
                _commandParameter = _protocol.DisableCommand();
                Config.IsEnabled = await this.Command(_commandParameter);
            }
            Config.IsEnabled = !Config.IsEnabled;
            if (Config.IsEnabled)
            {
                CoinIntroducedLstn(En_CoinIntroducedLstn);
                return "Coin device still enable";
            }
            else
            {
                return "Coin device is disable.";
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
        public async void CoinIntroducedLstn(bool enableListener)
        {
            bool ValideResponse = true;
            int countPolling = 0;
            while (Config.IsEnabled && enableListener && ValideResponse) {
                countPolling++;
                Console.WriteLine($"\n\n\n polling {countPolling}");
                ValideResponse = await _protocol.coinIntroducedLstn();
            }
        }
    }
}




/*



public void RequestCoinsRead()
{
    Console.WriteLine("Reading introduced coins...");

    //Point 1: Request coins read
    byte[] requestCoinsRead = { 0x0B };
    string coinsReadResponse = SendMessageWithRetry(requestCoinsRead);

    // Process coinsReadResponse
    if (string.IsNullOrEmpty(coinsReadResponse))
    {
        Console.WriteLine("No coins read.");
    }
    else if (coinsReadResponse.Length == 7 && coinsReadResponse == "30-38-20-32-31-0D-0A")
    {
        Console.WriteLine("Rejected: UNKNOWN COIN");
        // Go back to point 1
        RequestCoinsRead();
    }
    else if (coinsReadResponse.Length == 10 && coinsReadResponse.StartsWith("30-38-20"))
    {
        // Extract bytes for further processing
        byte[] responseBytes = coinsReadResponse.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();

        // Check fixed bytes (0x30, 0x38, 0x20)
        if (responseBytes[0] == 0x30 && responseBytes[1] == 0x38 && responseBytes[2] == 0x20)
        {

            byte coinLocation = responseBytes[3];
            byte coinType = responseBytes[4];

            switch (coinLocation)
            {
                case 0x34:
                    Console.WriteLine($"Accepted: {GetCoinType(coinType)} coin went into the excess coin drawer");
                    break;
                case 0x35:
                    Console.WriteLine($"Accepted: {GetCoinType(coinType)} coin in the gryphon tube");
                    break;
                case 0x37:
                    Console.WriteLine($"Discarded: {GetCoinType(coinType)} coin");
                    break;
                default:
                    Console.WriteLine("Unknown coin location");
                    break;
            }
        }
        else
        {
            // Unexpected response, retry by going to point 1
            Console.WriteLine("Retrying...");
            RequestCoinsRead();
        }
    }
    else
    {
        Console.WriteLine("Retrying...");
        // Go back to point 1
        RequestCoinsRead();
    }
}
private string GetCoinType(byte coinType)
{
    switch (coinType)
    {
        case 0x30:
            return "10 cent";
        case 0x31:
            return "20 cent";
        case 0x32:
            return "50 cent";
        case 0x33:
            return "1 euro";
        case 0x34:
            return "2 euro";
        default:
            return "Unknown coin type";
    }
}
private void AmountDeliveryFinish()
{
    Console.WriteLine("Checking if change delivery has finished...");

    // Wait for 500 milliseconds to check if there is no more response
    Thread.Sleep(500);

    // Check if there is no more response (08 02 {0D}{0A})
    string finishResponse = ReceiveMessage();
    string expectedFinishResponse = "08 02 {0D}{0A}";

    if (finishResponse == expectedFinishResponse)
    {
        Console.WriteLine("Change delivery finished. Proceeding to the next step.");
        // Call the next method
        RequestHowManyCoinsDispensed();
    }
    else
    {
        Console.WriteLine("Change delivery still in progress. Waiting...");
        // should i retry or take appropriate action ?
    }
}
private void RequestHowManyCoinsDispensed()
{
    Console.WriteLine("Requesting coins dispensed for change...");

    // Request coins dispensed for change
    byte[] requestCoinsDispensed = { 0x0F, 0x03 };
    string coinsDispensedAck = "00 00 {0D}{0A}";

    string coinsDispensedResponse = SendMessageWithRetry(requestCoinsDispensed);

    if (coinsDispensedResponse == coinsDispensedAck)
    {
        Console.WriteLine("Waiting for Gryphon to provide coins dispensed...");

        // Wait for coins dispensed response
        Thread.Sleep(300);

        // Retry to get coins dispensed response
        coinsDispensedResponse = ReceiveMessage();

        if (coinsDispensedResponse == coinsDispensedAck)
        {
            Console.WriteLine("No coins dispensed for change.");
        }
        else
        {

            ProcessCoinsDispensed(coinsDispensedResponse);
        }
    }
    else
    {
        Console.WriteLine("Request for coins dispensed not acknowledged. Gryphon may be busy.");
    }
}
private void ProcessCoinsDispensed(string response)
{
    // Validate the length of the response
    if (response.Length == 53)
    {
        // Extract relevant part of the response containing tube status
        string tubeStatusData = response.Substring(16, 36);

        // Extract coin counts from the tube status data
        int count10Cent = Convert.ToInt32(tubeStatusData.Substring(0, 2), 16);
        int count20Cent = Convert.ToInt32(tubeStatusData.Substring(3, 2), 16);
        int count50Cent = Convert.ToInt32(tubeStatusData.Substring(6, 2), 16);
        int count1Euro = Convert.ToInt32(tubeStatusData.Substring(9, 2), 16);
        int count2Euro = Convert.ToInt32(tubeStatusData.Substring(12, 2), 16);

        // Calculate the total amount in each tube
        double amount10Cent = count10Cent * 0.10;
        double amount20Cent = count20Cent * 0.20;
        double amount50Cent = count50Cent * 0.50;
        double amount1Euro = count1Euro * 1.00;
        double amount2Euro = count2Euro * 2.00;

        // Display the exact amount present in each tube
        Console.WriteLine("Coins dispensed for change:");
        Console.WriteLine($"10 Cent: {amount10Cent} euros");
        Console.WriteLine($"20 Cent: {amount20Cent} euros");
        Console.WriteLine($"50 Cent: {amount50Cent} euros");
        Console.WriteLine($"1 Euro: {amount1Euro} euros");
        Console.WriteLine($"2 Euro: {amount2Euro} euros");


    }
    else
    {
        Console.WriteLine("Invalid response length for coins dispensed.");
    }
}
private string ReceiveMessage()
{
    Thread.Sleep(RetryDelayMilliseconds); // Wait for Gryphon response
    int bytesToRead = serialPort.BytesToRead;

    if (bytesToRead == 0)
    {
        Console.WriteLine("No response received.");
        return string.Empty;
    }

    byte[] responseBytes = new byte[bytesToRead];
    serialPort.Read(responseBytes, 0, bytesToRead);
    string responseAscii = Encoding.ASCII.GetString(responseBytes);

    Console.WriteLine($"Received response: {responseAscii}");
    return responseAscii;
}
public void RequestCoinsInTubes()
{
    Console.WriteLine("Requesting coins present in the tubes...");

    // Request coins present in the tubes
    byte[] requestCoinsInTubes = { 0x0A };
    string coinsInTubesResponse = SendMessageWithRetry(requestCoinsInTubes);

    // Process coinsInTubesResponse
    if (string.IsNullOrEmpty(coinsInTubesResponse))
    {
        Console.WriteLine("No coins present in the tubes.");
    }
    else
    {
        DisplayCoinTubesStatus(coinsInTubesResponse);
        Console.WriteLine("Coins are present in the tubes.");
    }
}
private void DisplayCoinTubesStatus(string response)
{
    // Convert the response to bytes
    byte[] responseBytes = Encoding.ASCII.GetBytes(response);

    // Check if the response is of the expected length
    if (responseBytes.Length == 59)
    {
        // Extract relevant part of the response containing tube status
        byte[] tubeStatusData = new byte[15];
        Array.Copy(responseBytes, 5, tubeStatusData, 0, 15);

        // Interpret the tube status data as ASCII values
        string tenCentCoins = Convert.ToChar(tubeStatusData[2]).ToString();
        string twentyCentCoins = Convert.ToChar(tubeStatusData[5]).ToString();
        string fiftyCentCoins = Convert.ToChar(tubeStatusData[8]).ToString();
        string oneEuroCoins = Convert.ToChar(tubeStatusData[11]).ToString();
        string twoEuroCoins = Convert.ToChar(tubeStatusData[14]).ToString();

        Console.WriteLine($"Number of coins inside the gryphon:");
        Console.WriteLine($"10 Cent Coins: {tenCentCoins}");
        Console.WriteLine($"20 Cent Coins: {twentyCentCoins}");
        Console.WriteLine($"50 Cent Coins: {fiftyCentCoins}");
        Console.WriteLine($"1 Euro Coins: {oneEuroCoins}");
        Console.WriteLine($"2 Euro Coins: {twoEuroCoins}");
    }
    else
    {
        Console.WriteLine("Invalid response length for coins in tubes.");
    }
}
*/