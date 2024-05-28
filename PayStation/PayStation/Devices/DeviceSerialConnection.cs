using PayStationSW.DataBase;
using System.IO.Ports;

namespace PayStationSW.Devices
{
    public abstract partial class Device
    {
        #region public var
        public SerialPort serialPort;
        public event EventHandler? DataReceived;
        public event EventHandler? ErrorReceived;
        #endregion
        #region privat var
        private CommandParameter _commandParameters = new CommandParameter();
        private System.Timers.Timer? retryTimer;
        private int currentRetryCount = 0; // Current retry attempt count
        private bool isWaitingForResponse = false; // Flag to check if we are waiting for a response
        private byte[]? lastSentMessageByte;
        private string? lastSentMessageString;
        private int countCompletBufferRecived = 0;
        private List<byte[]> messageFragments = new List<byte[]>();
        private List<byte> messageBuffer = new List<byte>();
        private object bufferLock = new object(); // For thread-safe access to the buffer
        #endregion
        #region Serial Port Settings
        //Configure serial Port parameters
        public virtual void ConfigureSerialPort(SerialPort serialPort)
        {
            // Set default serial port settings here
            serialPort.PortName = Config.serialConnectionParameter.LastPortName;
            serialPort.BaudRate = Config.serialConnectionParameter.BaudRate;
            serialPort.DataBits = Config.serialConnectionParameter.DataBits;

            Parity ParityValue;
            switch (Config.serialConnectionParameter.Parity)
            {
                case 0:
                    ParityValue = Parity.None;
                    break;
                case 1:
                    ParityValue = Parity.Odd;
                    break;
                case 2:
                    ParityValue = Parity.Even;
                    break;
                case 3:
                    ParityValue = Parity.Mark;
                    break;
                case 4:
                    ParityValue = Parity.Space;
                    break;
                default:
                    ParityValue = Parity.None;
                    break;
            }
            serialPort.Parity = ParityValue;

            StopBits StopBitsValue;
            switch (Config.serialConnectionParameter.StopBits)
            {
                case 0:
                    StopBitsValue = StopBits.None;
                    break;
                case 1:
                    StopBitsValue = StopBits.One;
                    break;
                case 2:
                    StopBitsValue = StopBits.Two;
                    break;
                case 3:
                    StopBitsValue = StopBits.OnePointFive;
                    break;
                default:
                    StopBitsValue = StopBits.None;
                    break;
            }
            serialPort.StopBits = StopBitsValue;

            Handshake HandshakeValue;
            switch (Config.serialConnectionParameter.Handshake)
            {
                case 0:
                    HandshakeValue = Handshake.None;
                    break;
                case 1:
                    HandshakeValue = Handshake.XOnXOff;
                    break;
                case 2:
                    HandshakeValue = Handshake.RequestToSend;
                    break;
                case 3:
                    HandshakeValue = Handshake.RequestToSendXOnXOff;
                    break;
                default:
                    HandshakeValue = Handshake.None;
                    break;
            }
            serialPort.Handshake = HandshakeValue;
        }
        // Disconect Device
        public void Disconnect()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Serial connection closed.");
            }
            else
            {
                Console.WriteLine("Serial connection is not open.");
            }
            Config.IsConnected = false; // Renamed property
        }
        // Connect Device
        public void Connect()
        {
            try
            {
                ConfigureSerialPort(serialPort);
                serialPort.DataReceived += DataReceivedHandler;
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    Config.IsConnected = true;
                    Console.WriteLine($"Connected to {serialPort.PortName}.");
                }
            }
            catch (Exception ex)
            {
                Config.IsConnected = false;
                Console.WriteLine($"Failed to connect to {serialPort.PortName}: {ex.Message}");
            }
        }
        public virtual void IdentifyDevice() { }
        #endregion
        #region Send Message
        // SendMessage for byte[]
        public void SendMessage(byte[] message)
        {
            if (serialPort.IsOpen)
            {
                lastSentMessageByte = message; // Save the current message as the last sent message
                serialPort.Write(message, 0, message.Length); // Send the byte array
                Console.WriteLine("Byte message sent: " + BitConverter.ToString(message));
                if (_commandParameters.expectedResponse)
                {
                    isWaitingForResponse = true; // Start waiting for response
                    retryTimer.Start(); // Start the response wait timer
                }
                else
                {
                    Console.WriteLine("No response expected.");
                    OnDataReceived();
                    isWaitingForResponse = false;
                    retryTimer.Stop();
                    currentRetryCount = 0;
                    _commandParameters.responseByte = null;
                }
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }
        // SendMessage for string
        public void SendMessage(string message)
        {
            if (serialPort.IsOpen)
            {
                lastSentMessageString = message; // Save the string message for eventual retry
                serialPort.WriteLine(message); // Send the string message
                Console.WriteLine("String message sent: " + message);

                if (_commandParameters.expectedResponse)
                {
                    isWaitingForResponse = true; // Start waiting for response
                    retryTimer.Start(); // Start the response wait timer
                }
                else
                {
                    Console.WriteLine("No response expected.");
                    OnDataReceived();
                    isWaitingForResponse = false;
                    retryTimer.Stop();
                    currentRetryCount = 0;
                    _commandParameters.responseStrings = null;
                }
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }
        // SendMessage with retray for byte[]
        public void SendMessageWithRetry(byte[] message)
        {
            currentRetryCount = 0;
            SetRetryAndTimerResponse();
            SendMessage(message);
        }
        // SendMessage with retray for string
        public void SendMessageWithRetry(string message)
        {
            currentRetryCount = 0;
            SetRetryAndTimerResponse();
            SendMessage(message);
        }
        public void SetRetryAndTimerResponse()
        {
            // Initialize the timer but do not start it
            retryTimer = new System.Timers.Timer(_commandParameters.retryDelayMilliseconds); // Set timeout to 1 second
            retryTimer.AutoReset = false; // Ensure the timer runs only once per message
            retryTimer.Elapsed += OnResponseTimeout; // Specify the method to call when the timer elapses
        }
        #endregion
        #region Recive Message
        private void OnResponseTimeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            isWaitingForResponse = false; // Stop waiting for response

            if ((currentRetryCount < _commandParameters.maxRetryAttempts) && (_commandParameters.sendWithRetry))
            {
                currentRetryCount++;
                Console.WriteLine($"No response received, retrying... Attempt {currentRetryCount} of {_commandParameters.maxRetryAttempts}");
                if (_commandParameters.sendStringOrHEX)
                {
                    SendMessage(lastSentMessageString); // Retrying with the last string message
                }
                else
                {
                    SendMessage(lastSentMessageByte); // Retrying with the last byte array message
                }
            }
            else
            {
                Console.WriteLine("Device doesn't answer after maximum retry attempts.");
                // Handle the case where the device is not responding after retries (e.g., alert the user, log the error, etc.)
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (!_commandParameters.isTimedMode || isWaitingForResponse)
            {
                try
                {
                    if (_commandParameters.expectedStringOrHEX)
                    {
                        // Read the incoming data as a string
                        string receivedString;
                        while ((receivedString = serialPort.ReadLine()) != null) { }
                        if (CheckAndProcessMessageBufferString(receivedString))
                        {
                            Console.WriteLine("Recived valid format of string.");
                            OnDataReceived(); // Notify subscribers that no response expected
                            isWaitingForResponse = false;
                            retryTimer.Stop();
                            currentRetryCount = 0;
                            _commandParameters.responseByte = null;
                        }

                    }
                    else
                    {
                        // Read the incoming data as a byte array
                        int bytesToRead = serialPort.BytesToRead;
                        byte[] receivedBytes = new byte[bytesToRead];
                        int bytesRead = serialPort.Read(receivedBytes, 0, bytesToRead);
                        Console.WriteLine("Received byte data: " + BitConverter.ToString(receivedBytes, 0, bytesRead));

                        // Thread-safe addition of received bytes to the buffer
                        lock (bufferLock)
                        {
                            messageBuffer.AddRange(receivedBytes.Take(bytesRead)); // Add only the bytes that were read
                        }

                        // Check for complete message and process
                        if (CheckAndProcessMessageBufferByte())
                        {

                            ++countCompletBufferRecived;

                            if (countCompletBufferRecived == _commandParameters.nmbrResponseExpected)
                            {
                                List<Byte[]> combinedMessages = new List<Byte[]>();
                                combinedMessages = messageFragments.ToList();
                                _commandParameters.responseByte = combinedMessages;
                                OnDataReceived(); // Notify subscribers that a new complete message is ready
                                // Reset the state for processing the next set of messages
                                messageFragments.Clear();
                                countCompletBufferRecived = 0;
                                isWaitingForResponse = false;
                                retryTimer.Stop();
                                currentRetryCount = 0;
                                _commandParameters.responseByte = null; // Clear the response if not needed after the event
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        private bool CheckAndProcessMessageBufferString(string message)
        {

            // Assuming _commandParameters is an instance variable of your class
            if (message == null)
            {
                // Return false or handle the error if the message is null
                return false;
            }

            // Check if both flags are off
            if (!_commandParameters.messageResponseHaveStartingWord && !_commandParameters.messageResponseHaveEndingWord)
            {
                // If both flags are off, return the message as is
                _commandParameters.responseStrings.Add(message);
                return true;
            }

            // Check if the starting word flag is on
            if (_commandParameters.messageResponseHaveStartingWord)
            {
                int startingIndex = 0;

                // Iterate over the message until all occurrences of the starting word are processed
                while ((startingIndex = message.IndexOf(_commandParameters.messageStartingWord, startingIndex)) != -1)
                {
                    // Find the index of the ending word after the starting word
                    int endingIndex = message.IndexOf(_commandParameters.messageEndingWord, startingIndex);

                    if (endingIndex != -1)
                    {
                        // Extract the substring between the starting and ending words
                        string extractedString = message.Substring(startingIndex, endingIndex + _commandParameters.messageEndingWord.Length - startingIndex);
                        _commandParameters.responseStrings.Add(extractedString); // Add the extracted substring to the list

                        // Move the starting index beyond the current ending word
                        startingIndex = endingIndex + _commandParameters.messageEndingWord.Length;
                    }
                    else
                    {
                        // If the ending word is not found, break the loop
                        break;
                    }
                }
            }

            // Check if the ending word flag is on
            if (_commandParameters.messageResponseHaveEndingWord && !_commandParameters.messageResponseHaveStartingWord)
            {
                // Split the message based on the ending word if the starting word flag is off
                string[] splitStrings = message.Split(new string[] { _commandParameters.messageEndingWord }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string splitString in splitStrings)
                {
                    _commandParameters.responseStrings.Add(splitString);
                }
            }
            // Check if the starting word flag is on
            if (!_commandParameters.messageResponseHaveEndingWord && _commandParameters.messageResponseHaveStartingWord)
            {
                // Split the message based on the ending word if the starting word flag is off
                string[] splitStrings = message.Split(new string[] { _commandParameters.messageStartingWord }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string splitString in splitStrings)
                {
                    _commandParameters.responseStrings.Add(splitString);
                }
            }

            // Return true indicating success
            return _commandParameters.responseStrings.Count > 0;
        }
        private bool CheckAndProcessMessageBufferByte()
        {
            lock (bufferLock)
            {
                bool messageProcessed = false;
                int startIndex = 0;
                int endIndex = 0;

                // Process the buffer while it contains enough data for at least the minimum message length
                while (messageBuffer.Count >= _commandParameters.minBufferLength)
                {
                    if (_commandParameters.messageResponseHaveStartingByte)
                    {
                        startIndex = FindStartIndexOfCompleteMessage(messageBuffer);
                    }

                    if (_commandParameters.messageResponseHaveEndingByte)
                    {
                        endIndex = FindEndIndexOfCompleteMessage(messageBuffer);
                    }
                    if (endIndex != -1 && startIndex != -1 && endIndex > 0) // A complete message is found based on ending bytes or other criteria
                    {
                        byte[] completeMessageFragment = messageBuffer.Take(endIndex + 1).ToArray();
                        if (startIndex >= 0 && endIndex < messageBuffer.Count)
                        {
                            messageBuffer.RemoveRange(startIndex, endIndex - startIndex + 1);
                        }
                        messageFragments.Add(completeMessageFragment);
                        messageProcessed = true;
                    }
                    else if (messageBuffer.Count >= _commandParameters.minBufferLength) // Not found or not specifid ending, and not found or specified start, but minimum length is met
                    {
                        byte[] messageFragment = messageBuffer.ToArray(); // Consider the entire buffer as a message fragment
                        messageBuffer.Clear(); // Clear the buffer since we've taken all its content
                        messageFragments.Add(messageFragment);
                        messageProcessed = true;
                        break; // Exit the loop since we've processed the whole buffer
                    }
                    else
                    {
                        // Not enough data for a complete message, exit the loop
                        break;
                    }
                }
                return messageProcessed;
            }
        }
        private int FindStartIndexOfCompleteMessage(List<byte> buffer)
        {
            if (_commandParameters.messageResponseHaveStartingByte)
            {
                if (_commandParameters.messageStartingBytes != null && _commandParameters.messageStartingBytes.Length > 0)
                {
                    // Look for ending bytes sequence in the buffer
                    for (int i = 0; i <= buffer.Count - _commandParameters.messageStartingBytes.Length; i++)
                    {
                        bool sequenceFound = true;
                        for (int j = 0; j < _commandParameters.messageStartingBytes.Length; j++)
                        {
                            if (buffer[i + j] != _commandParameters.messageStartingBytes[j])
                            {
                                sequenceFound = false;
                                break; // Exit the inner loop as the sequence does not match
                            }
                        }
                        if (sequenceFound)
                        {
                            return i + _commandParameters.messageStartingBytes.Length - 1; // Return the end index of the sequence
                        }
                    }
                }
            }
            return -1; // No complete message found
        }
        private int FindEndIndexOfCompleteMessage(List<byte> buffer)
        {
            if (_commandParameters.messageResponseHaveEndingByte)
            {
                if (_commandParameters.messageEndingBytes != null && _commandParameters.messageEndingBytes.Length > 0)
                {
                    // Look for ending bytes sequence in the buffer
                    for (int i = 0; i <= buffer.Count - _commandParameters.messageEndingBytes.Length; i++)
                    {
                        bool sequenceFound = true;
                        for (int j = 0; j < _commandParameters.messageEndingBytes.Length; j++)
                        {
                            if (buffer[i + j] != _commandParameters.messageEndingBytes[j])
                            {
                                sequenceFound = false;
                                break; // Exit the inner loop as the sequence does not match
                            }
                        }
                        if (sequenceFound)
                        {
                            return i + _commandParameters.messageEndingBytes.Length - 1; // Return the end index of the sequence
                        }
                    }
                }
            }
            return -1; // No complete message found
        }
        #endregion
        #region Datarecived handeling
        public virtual void OnDataReceived()
        {
            DataReceived?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region Error recived
        public void Device_ErrorReceived(object? sender, EventArgs e)
        {
            var eventArgs = (SerialErrorReceivedEventArgs)e;
            Console.WriteLine($"Error received from device: {eventArgs.EventType}");
            // Handle the error, e.g., retry, log, notify user, etc.
        }
        #endregion
        #region Serial port error
        // Event handler that gets called when an error is received on the serial port
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            // Process the error
            Console.WriteLine("Serial port error: " + e.EventType.ToString());

            // Raise the ErrorReceived event
            OnErrorReceived(e);
        }
        // Method to raise the ErrorReceived event
        protected virtual void OnErrorReceived(SerialErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(this, e);
        }
        #endregion
        #region Command
        public async Task<bool> Command(CommandParameter commandParameters)
        {
            _commandParameters = commandParameters;
            TimeSpan timeout = TimeSpan.FromSeconds(_commandParameters.timeOutResponseMilliseconds);
            var tcs = new TaskCompletionSource<bool>();

            EventHandler dataReceivedHandler = null;
            dataReceivedHandler = (sender, args) =>
            {
                bool validate = true; // Assume validation passes initially

                if (_commandParameters.expectedStringOrHEX)
                {
                    validate = ValidateResponse(_commandParameters.responseStrings, _commandParameters.expectedResponsesListString, _commandParameters.expectedSpecificLengthList, _commandParameters.expectedMinLengthList, _commandParameters.expectedSpecificResponse, _commandParameters.expectedSpecificLength, _commandParameters.expectedMinLength, _commandParameters.validateAnyResponse, _commandParameters.expectedMultipleResponse);
                }
                else
                {
                    validate = ValidateResponse(_commandParameters.responseByte, _commandParameters.expectedResponsesListByte, _commandParameters.expectedSpecificLengthList, _commandParameters.expectedMinLengthList, _commandParameters.expectedSpecificResponse, _commandParameters.expectedSpecificLength, _commandParameters.expectedMinLength, _commandParameters.validateAnyResponse, _commandParameters.expectedMultipleResponse);
                }
                tcs.TrySetResult(validate);
                DataReceived -= dataReceivedHandler; // Unsubscribe after receiving response
            };

            DataReceived += dataReceivedHandler; // Subscribe to the DataReceived event
            if (_commandParameters.sendStringOrHEX)
            {
                SendMessageWithRetry(_commandParameters.messageToSendString ?? "");
            }
            else
            {
                SendMessageWithRetry(_commandParameters.messageToSendBytes ?? []);
            }
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout)); // Wait for completion or timeout
            if ((completedTask == tcs.Task && await tcs.Task) || !(_commandParameters.expectedResponse))
            {
                return true; // Task completed successfully
            }
            else
            {
                Console.WriteLine("Something went wrong. Device did not respond.");
                return false; // Task failed or timed out
            }
        }
        #endregion
        #region Validation Response
        private bool ValidateResponse<T>(List<T> response, List<T> expectedResponses, List<int> expectedLengths, List<int> minLengths, bool expectedSpecificResponse, bool expectedSpecificLength, bool expectedMinLength, bool validateAnyResponse, bool expectedMultipleResponse)
        {

            if (response == null || response.Count == 0)
            {
                return false; // No response received
            }

            if (validateAnyResponse)
            {
                return true; //Recived at list one response, and reciving a response validate the command
            }
            if (expectedMultipleResponse && response.Count <= 1)
            {
                return false;
            }
            if (expectedSpecificResponse && expectedResponses != null && expectedResponses.Count > 0)
            {
                if (!ValidateExpectedResponses(response, expectedResponses))
                {
                    return false; // Response did not match expected values
                }
            }

            if (expectedSpecificLength && expectedLengths != null && expectedLengths.Count > 0)
            {
                if (!ValidateExpectedLengths(response, expectedLengths))
                {
                    return false; // Response lengths did not match expected lengths
                }
            }

            if (expectedMinLength && minLengths != null && minLengths.Count > 0)
            {
                if (!ValidateMinLengths(response, minLengths))
                {
                    return false; // Response lengths are less than expected minimum lengths
                }
            }

            return true; // All validations enabled: passed
        }

        private bool ValidateExpectedResponses<T>(List<T> response, List<T> expectedResponses)
        {
            for (int i = 0; i < Math.Min(response.Count, expectedResponses.Count); i++)
            {

                if (response[i] != null && !response[i].Equals(expectedResponses[i]))
                {
                    return false; // Response did not match expected value
                }
            }
            return true;
        }

        private bool ValidateExpectedLengths<T>(List<T> response, List<int> expectedLengths)
        {
            for (int i = 0; i < Math.Min(response.Count, expectedLengths.Count); i++)
            {
                if (response[i] is string str && str.Length != expectedLengths[i])
                {
                    return false; // Response length did not match expected length
                }
                else if (response[i] is byte[] bytes && bytes.Length != expectedLengths[i])
                {
                    return false; // Response length did not match expected length
                }
            }
            return true;
        }

        private bool ValidateMinLengths<T>(List<T> response, List<int> minLengths)
        {
            for (int i = 0; i < Math.Min(response.Count, minLengths.Count); i++)
            {
                if (response[i] is string str && str.Length < minLengths[i])
                {
                    return false; // Response length is less than expected minimum length
                }
                else if (response[i] is byte[] bytes && bytes.Length < minLengths[i])
                {
                    return false; // Response length is less than expected minimum length
                }
            }
            return true;
        }
        #endregion
    }
}