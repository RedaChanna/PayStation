using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayStationSW.Devices;

namespace PayStationSW
{
    public class AutomaticConnection_Mng
    {
        private List<Device> devices;
        private List<string> comPorts;
        private PayStation _payStation;

        // Set the default timeout and retry values
        private const int DefaultTimeoutMilliseconds = 1000; // 1 second
        private const int DefaultMaxRetryAttempts = 3;

        // With system function get the the number of serial port present in controller or PC
        // Get the list of devices instancieted in paystation 
        public AutomaticConnection_Mng(PayStation payStation)
        {
            _payStation = payStation;
            devices = _payStation.GetDevices();
            comPorts = SerialPort.GetPortNames().ToList();
        }

        public Dictionary<string, string> CheckDeviceConnections(int timeoutMs = DefaultTimeoutMilliseconds, int maxRetryAttempts = DefaultMaxRetryAttempts)
        {
            bool NextPort = true;
            Dictionary<string, string> connectedDevices = new Dictionary<string, string>();
            List<string> remainingComPorts = new List<string>(comPorts); // Make a copy of comPorts for tracking remaining ports

            foreach (Device device in devices)
            {
                List<string> portsToRetry = new List<string>(remainingComPorts); // Copy the remaining ports to retry
                List<string> portsToRemove = new List<string>();
                if (!device.Config.IsConnected)
                {
                    // Try to connect the device to the remaining ports list
                    foreach (string comPort in portsToRetry)
                    {
                        int retryCount = 0;
                        string deviceType = device.GetType().Name;
                        Console.WriteLine($"Device type: {deviceType}");
                        while (retryCount < maxRetryAttempts)
                        {
                            using (SerialPort serialPort = new SerialPort(comPort))
                            {
                                try
                                {
                                    device.ConfigureSerialPort(serialPort); // Set device-specific parameters
                                    serialPort.Open();
                                    serialPort.ReadTimeout = timeoutMs;
                                    if (deviceType == "PrinterDevice")
                                    {

                                        serialPort.Write("Stampante connessa Stampante connessa");

                                        Console.WriteLine($"Printer needs a manual feedback for automatic com port scanning, printer is connected Y/N?: {deviceType}");
                                        while ((NextPort) && (!device.Config.IsConnected))
                                        {
                                            string? PrinterConectedAnsware = Console.ReadLine();
                                            if (!string.IsNullOrEmpty(PrinterConectedAnsware))
                                            {
                                                if ((PrinterConectedAnsware == "Y") || (PrinterConectedAnsware == "y"))
                                                {
                                                    NextPort = false;
                                                    connectedDevices.Add(comPort, deviceType);
                                                    serialPort.Close();
                                                    break; // Device connected, move to the next device
                                                }
                                                else if ((PrinterConectedAnsware == "N") || (PrinterConectedAnsware == "n"))
                                                {
                                                    NextPort = false;
                                                }
                                            }
                                        }
                                        Console.WriteLine($"Device is connected: {device.Config.IsConnected}");
                                        if (device.Config.IsConnected)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            NextPort = true;
                                        }

                                    }
                                    else
                                    {

                                        // Hexadecimal data as a string
                                        string hexData = "1A2B3C4D"; // Change to your desired hexadecimal data

                                        // Convert the hexadecimal string to bytes
                                        byte[] byteArray = HexStringToByteArray(hexData);

                                        // Write the bytes to the serial port
                                        serialPort.Write(byteArray, 0, byteArray.Length);


                                        string deviceInfo = serialPort.ReadLine();
                                        string deviceName = deviceType;

                                        if (!string.IsNullOrEmpty(deviceName))
                                        {
                                            connectedDevices.Add(comPort, deviceName);
                                            serialPort.Close();
                                            break; // Device connected, move to the next device
                                        }

                                        serialPort.Close();
                                    }
                                    if (connectedDevices.ContainsKey(comPort))
                                    {
                                        // Device connected successfully, no need to retry this port for other devices
                                        portsToRemove.Add(comPort);
                                    }

                                }
                                catch (TimeoutException)
                                {

                                    // Handle timeout exception
                                    //Console.WriteLine($"Timeout on {comPort} (Device: {device.GetType().Name}), retry {retryCount + 1}/{maxRetryAttempts}");
                                }
                                catch (Exception ex)
                                {

                                    // Handle other exceptions
                                    //Console.WriteLine($"Error on {comPort} (Device: {device.GetType().Name}): {ex.Message}");
                                }
                            }

                            retryCount++;
                        }

                    }
                    // Remove the successfully connected ports from the remaining ports list
                    foreach (string portToRemove in portsToRemove)
                    {
                        remainingComPorts.Remove(portToRemove);
                    }
                }
            }
            return connectedDevices;
        }


        // Convert a hexadecimal string to a byte array
        static byte[] HexStringToByteArray(string hex)
        {
            hex = hex.Replace(" ", ""); // Remove spaces if present
            int length = hex.Length;
            byte[] byteArray = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return byteArray;
        }
    }
}