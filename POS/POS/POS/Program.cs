using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS
{
   
    public class PosReader
    {
       
        private readonly SerialPort serialPort;

        // Constants for success acknowledgement and message 
        private const string ACK_SUCCESS = "06037A";
        private const string MESSAGE_START = "02";
        private const string MESSAGE_END = "03";

       
        private string completeReceivedData = "";


        public PosReader()
        {

            serialPort = new SerialPort("COM1", 115200, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += SerialPortDataReceived;
            serialPort.Open();
        }

        // Method to send a message asynchronously to the POS device
        private async Task SendMessageAsync(string message)
        {
            // Convert message from hexadecimal string to byte array
            byte[] hexBytes = Enumerable.Range(0, message.Length)
               .Where(x => x % 2 == 0)
               .Select(x => Convert.ToByte(message.Substring(x, 2), 16))
               .ToArray();

            // Convert byte array to hexadecimal string for logging
            string hexString = BitConverter.ToString(hexBytes).Replace("-", " ");
            Console.WriteLine("\n" + $"Sending message to POS : {hexString}");

            
            await serialPort.BaseStream.WriteAsync(hexBytes, 0, hexBytes.Length);
        }

        // Event handler for data received from the serial port
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Cast sender to SerialPort
                SerialPort sp = (SerialPort)sender;
                byte[] buffer = new byte[sp.BytesToRead];
                sp.Read(buffer, 0, buffer.Length);

                // Convert received bytes to hexadecimal string
                string hexData = BitConverter.ToString(buffer).Replace("-", "");

                // Append the received data to the completeReceivedData variable
                completeReceivedData += hexData;

                Console.WriteLine("\n" + "Data Received from POS:");
                Console.WriteLine(hexData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SerialPortDataReceived: {ex.Message}");
            }
        }

        // Method to wait for a response from the POS device
        private async Task<string> WaitForResponse()
        {
            try
            {
                TimeSpan timeout = TimeSpan.FromSeconds(100); // 100 seconds timeout
                Stopwatch stopwatch = Stopwatch.StartNew();

                while (stopwatch.Elapsed < timeout)
                {
                    // Check if the desired response is received
                    if (!string.IsNullOrEmpty(completeReceivedData))
                    {
                        // Check if the received data is an ACK
                        if (completeReceivedData.Trim().Equals(ACK_SUCCESS))
                        {
                            completeReceivedData = string.Empty; // Clear the received data for the next message
                            Console.WriteLine("\nReceived ACK:");
                            Console.WriteLine(ACK_SUCCESS);
                        }
                        else
                        {
                            string response = completeReceivedData;
                            int dataLength = response.Length; // store length of the received data
                            completeReceivedData = string.Empty;
                            Console.WriteLine($"\nReceived response with {dataLength} characters:");
                            Console.WriteLine(response); 
                            return response; // Return other response
                        }
                    }

                    // Introduce a delay to avoid tight loop
                    await Task.Delay(100);
                }

                // Handle timeout
                Console.WriteLine("Timeout waiting for response.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WaitForResponse: {ex.Message}");
                return null;
            }
        }

        // Method to send ACK (acknowledgment) to the POS device asynchronously
        private async Task SendACKToPOSAsync()
        {
            string ackMessage = ACK_SUCCESS;
            await SendMessageAsync(ackMessage);
        }

        // Method to calculate the LCR for a given message
        private string CalculateLCR(string message)
        {
            //Remove any non-hexadecimal characters from the message
            string hexMessage = new string(message.Where(c => Uri.IsHexDigit(c)).ToArray());

            //Ensure the length is even
            if (hexMessage.Length % 2 != 0)
            {
                hexMessage = "0" + hexMessage; // Pad with '0' if necessary
            }

            //Convert the hex string to byte array
            byte[] byteArray = Enumerable.Range(0, hexMessage.Length / 2)
                .Select(x => Convert.ToByte(hexMessage.Substring(x * 2, 2), 16))
                .ToArray();

            //Calculate the LCR
            int lcr = 0x7F;
            foreach (byte b in byteArray)
            {
                lcr ^= b;
            }

            return lcr.ToString("X2");
        }

        // Method to send an activation command to the POS device
        public async Task SendActivationCommand(string terminalID)
        {
            try
            {
                // Construct activation message
                string hexTerminalID = BitConverter.ToString(Encoding.ASCII.GetBytes(terminalID)).Replace("-", "");
                string activationMessage = $"{MESSAGE_START}{hexTerminalID}3061303030303030303030303030303003";
                string lcrActivation = CalculateLCR(activationMessage);

                // Send activation message
                await SendMessageAsync(activationMessage + lcrActivation);

                // Wait for the response
                string receivedData = await WaitForResponse();

                // Process the response
                if (!string.IsNullOrEmpty(receivedData))
                {
                    if (receivedData.Length == 32 && receivedData.Substring(20, 2) == "61" && receivedData.Substring(22, 2) == "34" && receivedData.Substring(26, 2) == "31")
                    {
                        Console.WriteLine("Command A: Activation was successful.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid response message received.");
                    }
                }
                else
                {
                    Console.WriteLine("No response received within the timeout period.");
                }

                // Send an ACK after receiving the response
                await SendACKToPOSAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendActivationCommand: {ex.Message}");
            }
        }

        // Method to send a transaction completion command to the POS device
        public async Task CompleteTransactionAsync(string terminalID, int amountInCents)
        {
            try
            {
                await SendAmountAndHandleCardReadingAsync(terminalID, amountInCents);

                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
                Task<string> responseTask = WaitForResponse();
                Task completedTask = await Task.WhenAny(timeoutTask, responseTask);

                if (completedTask == timeoutTask)
                {
                    Console.WriteLine("TIME OUT.");
                }
                else
                {
                    // Response received
                    string receivedData = await responseTask;

                    // Handle transaction outcome message
                    HandleOutcomeMessage(receivedData);
                }

                await SendACKToPOSAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CompleteTransactionAsync: {ex.Message}");
            }
        }

        // Method to send amount and handle card reading asynchronously
        private async Task SendAmountAndHandleCardReadingAsync(string terminalID, int amountInCents)
        {
            try
            {
                string hexTerminalID = BitConverter.ToString(Encoding.ASCII.GetBytes(terminalID)).Replace("-", "");

                // Convert amount to string with 8 digits
                string amountString = amountInCents.ToString("D8");

                // Convert amount string to hexadecimal representation
                string hexAmount = BitConverter.ToString(Encoding.ASCII.GetBytes(amountString)).Replace("-", "");

                // Construct the amount message
                string posAmountMessage = $"{MESSAGE_START}{hexTerminalID}305030303030{hexAmount}3031302A30303030303030303030303003";
                string lcrPosAmount = CalculateLCR(posAmountMessage);

                // Send the amount message
                await SendMessageAsync(posAmountMessage + lcrPosAmount);

                // Wait for the response
                string receivedData = await WaitForResponse();

                // Check if the received data contains the expected card reading message
                string expectedCardReadingMessage = $"{MESSAGE_START}{hexTerminalID}3049{MESSAGE_END}";
                string lcrExpectedCardReadingMessage = CalculateLCR(expectedCardReadingMessage);

                if (receivedData.Contains(expectedCardReadingMessage + lcrExpectedCardReadingMessage))
                {
                    // Send acknowledgment to POS
                    await SendACKToPOSAsync();

                    // Construct the reply message I
                    string replyMessageI = $"{MESSAGE_START}{hexTerminalID}3049{Generate30Bytes()}{MESSAGE_END}";
                    string lcrReplyMessageI = CalculateLCR(replyMessageI);

                    // Send the reply message I to POS
                    await SendMessageAsync($"{replyMessageI}{lcrReplyMessageI}");
                    await SendACKToPOSAsync();

                    // Wait for transaction response from POS
                    string transactionResponse = await WaitForResponse();

                    // Check if the transaction response is valid
                    if (!string.IsNullOrEmpty(transactionResponse))
                    {
                        // Check the transaction outcome
                        HandleOutcomeMessage(transactionResponse);
                    }
                    else
                    {
                        Console.WriteLine("No transaction response received within the timeout period.");
                    }
                }
                else
                {
                    // No card reading message received, continue with handling outcome messages
                    HandleOutcomeMessage(receivedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendAmountAndHandleCardReadingAsync: {ex.Message}");
            }
        }

        // Method to handle outcome messages received from the POS device
        private void HandleOutcomeMessage(string receivedData)
        {
            try
            {
                if (!string.IsNullOrEmpty(receivedData))
                {
                    string term_id = null;
                    string rfubase = null;
                    string op_code = null;
                    string transaction_result = null;
                    string acquirer_id = null;
                    string transaction_type = null;
                    string ticket_number_echo = null;
                    string card_type = null;
                    string stan = null;
                    string approved_amount = null;
                    string transaction_data_and_time = null;
                    string approval_type = null;
                    string acquirer_name = null;
                    string pan = null;
                    string warning_card_in = null;
                    string rfu = null;
                    string receipt_rows = null;
                    string message_for_pos = null;
                    string approval_code = null;
                    string merchant_identifier = null;
                    string issuer_code = null;
                    string action_code = null;

                    if (receivedData.Length == 572 && receivedData.Substring(20, 2) == "45" && (receivedData.Substring(552, 6) == "303030"))
                    {
                        // Extract the ASCII characters from the received data
                        byte[] asciiBytes = new byte[receivedData.Length / 2];
                        for (int i = 0; i < receivedData.Length; i += 2)
                        {
                            string hexByte = receivedData.Substring(i, 2);
                            asciiBytes[i / 2] = Convert.ToByte(hexByte, 16);
                        }

                        // Convert ASCII bytes to string
                        string asciiMessage = Encoding.ASCII.GetString(asciiBytes);

                        // Extract parameters
                        term_id = asciiMessage.Substring(1, 8);
                        rfubase = asciiMessage.Substring(9, 1);
                        op_code = asciiMessage.Substring(10, 1);
                        transaction_result = asciiMessage.Substring(11, 2);
                        acquirer_id = asciiMessage.Substring(13, 11);
                        transaction_type = asciiMessage.Substring(24, 3);
                        ticket_number_echo = asciiMessage.Substring(27, 6);
                        card_type = asciiMessage.Substring(33, 1);
                        stan = asciiMessage.Substring(34, 6);
                        approved_amount = asciiMessage.Substring(40, 8);
                        transaction_data_and_time = asciiMessage.Substring(48, 12);
                        approval_type = asciiMessage.Substring(60, 1);
                        acquirer_name = asciiMessage.Substring(61, 16);
                        pan = asciiMessage.Substring(77, 19);
                        warning_card_in = asciiMessage.Substring(96, 1);
                        rfu = asciiMessage.Substring(113, 1);
                        receipt_rows = asciiMessage.Substring(114, 120);
                        message_for_pos = asciiMessage.Substring(234, 16);
                        approval_code = asciiMessage.Substring(250, 6);
                        merchant_identifier = asciiMessage.Substring(256, 15);
                        issuer_code = asciiMessage.Substring(271, 5);
                        action_code = asciiMessage.Substring(276, 3);

                        string dbString = $"term_id: {term_id}|rfubase: {rfubase}|op_code: {op_code}|transaction_result:{transaction_result}|" +
                            $"acquirer_id:{acquirer_id}|transaction_type: {transaction_type}|ticket_number_echo: {ticket_number_echo}|card_type: {card_type}|" +
                            $"stan: {stan}|approved_amount: {approved_amount}|transaction_data_and_time: {transaction_data_and_time}|approval_type: {approval_type}|" +
                            $"acquirer_name: {acquirer_name}|pan: {pan}|warning_card_in: {warning_card_in}|rfu: {rfu}|receipt_rows: {receipt_rows}|" +
                            $"message_for_pos: {message_for_pos}|approval_code: {approval_code}|merchant_identifier: {merchant_identifier}|issuer_code: {issuer_code}|action_code: {action_code}";

                        Console.WriteLine("\nParsed string: " + dbString);
                       
                    }
                    else if (receivedData.Length == 104 && receivedData.Substring(20, 2) == "46")
                    {
                        // Extract the ASCII characters from the received data
                        byte[] asciiBytes = new byte[receivedData.Length / 2];
                        for (int i = 0; i < receivedData.Length; i += 2)
                        {
                            string hexByte = receivedData.Substring(i, 2);
                            asciiBytes[i / 2] = Convert.ToByte(hexByte, 16);
                        }

                        // Convert ASCII bytes to string
                        string asciiMessage = Encoding.ASCII.GetString(asciiBytes);

                        // Extract error code substring
                        string errorCode = asciiMessage.Substring(11, 2);
                        string errorMessage = "";

                        switch (errorCode)
                        {
                            case "01":
                                errorMessage = "Transaction cancelled by cardholder";
                                break;
                            case "02":
                                errorMessage = "Internal error";
                                break;
                            case "03":
                                errorMessage = "Lev 2 Emv error";
                                break;
                            case "04":
                                errorMessage = "Transaction declined by Gt";
                                break;
                            case "05":
                                errorMessage = "Operation not allowed Try to Send a Daily Close";
                                break;
                            case "06":
                                errorMessage = "Emv application on card not managed by the terminal";
                                break;
                            case "07":
                                errorMessage = "Future not configured by Gt";
                                break;
                            case "08":
                                errorMessage = "Driver error during magnetic stripe read";
                                break;
                            case "09":
                                errorMessage = "Magnetic card not read";
                                break;
                            case "10":
                                errorMessage = "Wrong Data mapping magnetic card";
                                break;
                            case "11":
                                errorMessage = "Magnetic card with data on either track number 2 or 3, but bancomat or credit card not enabled by Gt";
                                break;
                            case "12":
                                errorMessage = "Magnetic card with data on track number 3, but bancomat or credit card not enabled by Gt";
                                break;
                            case "13":
                                errorMessage = "Card expired";
                                break;
                            case "14":
                                errorMessage = "Application not managed";
                                break;
                            case "15":
                                errorMessage = "Chip Application locked";
                                break;
                            case "16":
                                errorMessage = "Chip Application locked";
                                break;
                            case "17":
                                errorMessage = "Acquirer data missed or wrong";
                                break;
                            case "18":
                                errorMessage = "Transaction log Preauth full, closing daily session is needed";
                                break;
                            case "20":
                                errorMessage = "Terminal not configured";
                                break;
                            case "21":
                                errorMessage = "Terminal not enabled by Ecr";
                                break;
                            case "22":
                                errorMessage = "Tamper";
                                break;
                            case "31":
                                errorMessage = "Connection not possible Check line on POS";
                                break;
                            case "32":
                                errorMessage = "A problem occurs during the data Exchange with the Gt";
                                break;
                            case "33":
                                errorMessage = "Pin Attemps exhausted";
                                break;
                            case "34":
                                errorMessage = "Service operation not performed";
                                break;
                            case "35":
                                errorMessage = "Amount Revert note equal to transaction amount";
                                break;
                            case "36":
                                errorMessage = "Transaction amount is zero";
                                break;
                            case "37":
                                errorMessage = "Messages AAAA or BBBB received";
                                break;
                            case "38":
                                errorMessage = "Transaction declined by card";
                                break;
                            case "39":
                                errorMessage = "Transaction declined with explicit revert";
                                break;
                            case "40":
                                errorMessage = "Time out card withdrawal";
                                break;
                            case "41":
                                errorMessage = "Driver error magnetic card";
                                break;
                            case "42":
                                errorMessage = "Card read but tracks 2 and 3 empty";
                                break;
                            case "43":
                                errorMessage = "Log not found";
                                break;
                            case "44":
                                errorMessage = "Log not sent";
                                break;
                            case "45":
                                errorMessage = "Terminal Id wrong";
                                break;
                            case "47":
                                errorMessage = "Reader not recognized";
                                break;
                            case "48":
                                errorMessage = "Transaction cancelled due timeout";
                                break;
                            case "49":
                                errorMessage = "Track 2 recognized, but credit card not enabled by Gt";
                                break;
                            case "50":
                                errorMessage = "Track 3 recognized, but pago bancomat not enabled by Gt";
                                break;
                            case "51":
                                errorMessage = "Track 3 recognized but not compliant";
                                break;
                            case "52":
                                errorMessage = "Wrong TAG from the host";
                                break;
                            case "53":
                                errorMessage = "Transaction declined with implicit revert";
                                break;
                            case "54":
                                errorMessage = "Error NO CARD";
                                break;
                            case "55":
                                errorMessage = "Error CARD IN";
                                break;
                            case "60":
                                errorMessage = "Card Error";
                                break;
                            case "61":
                                errorMessage = "Card Removed";
                                break;
                            case "62":
                                errorMessage = "Invalid Card";
                                break;
                            case "63":
                                errorMessage = "Implicit Revert";
                                break;
                            case "64":
                                errorMessage = "Invalid Card";
                                break;
                            case "65":
                                errorMessage = "Terminal Id wrong";
                                break;
                            case "66":
                                errorMessage = "Command Unknown";
                                break;
                            case "67":
                                errorMessage = "Protocol Error";
                                break;
                            case "70":
                                errorMessage = "OP – Log Full";
                                break;
                            case "71":
                                errorMessage = "OP – AntiPassBack";
                                break;
                            case "72":
                                errorMessage = "OP – Card in Black-List";
                                break;
                            case "73":
                                errorMessage = "OP – Card Rejected Offline";
                                break;
                            case "74":
                                errorMessage = "OP – Transaction Id Error";
                                break;
                            case "75":
                                errorMessage = "OP – Card Expired";
                                break;
                            case "88":
                                errorMessage = "Preaut log full";
                                break;
                            case "90":
                                errorMessage = "Device is not active";
                                break;
                            case "91":
                                errorMessage = "Wrong Operation Type";
                                break;
                            case "99":
                                errorMessage = "Generic Error";
                                break;
                            default:
                                errorMessage = "Unknown error code";
                                break;
                        }

                        Console.WriteLine("\n" + "Message F: " + errorMessage);
                    }
                    else
                    {
                        Console.WriteLine("Invalid outcome message received.");
                    }
                }
                else
                {
                    Console.WriteLine("No response received within the timeout period.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleOutcomeMessage: {ex.Message}");
            }
        }


        // Method to generate 30 bytes of data
        private string Generate30Bytes()
        {
            return "303030303030303030303030303030303030303030303030303030303030";
        }

        // Method to send a command to disable POS device operations
        public async Task SendDisableOperationCommand(string terminalID)
        {
            try
            {
                string hexTerminalID = BitConverter.ToString(Encoding.ASCII.GetBytes(terminalID)).Replace("-", "");

                // Construct disable operation message
                string disableOperationMessage = $"02{hexTerminalID}305803";
                string lcrDisableOperation = CalculateLCR(disableOperationMessage);

                // Send disableOperationMessage
                await SendMessageAsync(disableOperationMessage + lcrDisableOperation);

                // Send ACK immediately after sending disableOperationMessage
                await SendACKToPOSAsync();

                // Wait for the reply
                string reply = await WaitForResponse();

                // Check if a response was received
                if (!string.IsNullOrEmpty(reply))
                {
                    if (reply.Length == 104 && reply.Substring(20, 2) == "46")
                    {
                        Console.WriteLine("Message F: Disable successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid outcome message received.");
                    }
                }
                else
                {
                    Console.WriteLine("No response received within the timeout period.");
                }

                // Send the ACK 
                await SendMessageAsync(ACK_SUCCESS);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendDisableOperationCommand: {ex.Message}");
            }
        }


    }
   
    class Program
    {
        static async Task Main()
        {
            var posReader = new PosReader();

            //await posReader.SendActivationCommand("11077128");
            await posReader.CompleteTransactionAsync("11077128", 10);
            //await posReader.SendDisableOperationCommand("11077128");
            // Attendere l'input dell'utente prima di terminare
            Console.WriteLine("Premi un tasto per chiudere la console...");
            Console.ReadKey();

        }
    }
}
