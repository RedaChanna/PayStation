using Microsoft.OpenApi.Extensions;
using PayStationSW.Devices;
using PayStationSW.Protocols.Cash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PayStationSW
{

    public partial class ProtocolVega100 : InterfaceCashProtocol
    {
        private readonly Device _device;
        private int timeOutTask = 10;

        public ProtocolVega100(Device dvice)
        {
            _device = dvice;
        }

        public StatusResponseAcceptor statusAcceptorResponse { get; set; } = StatusResponseAcceptor.Unknow;
        public PowerUpResponseAcceptor powerUpAcceptorResponse { get; set; } = PowerUpResponseAcceptor.Unknow;
        public ErrorResponseAcceptor errorAcceptorResponse { get; set; } = ErrorResponseAcceptor.Unknow;
        public OperationResponseAcceptor opertationAcceptorResponse { get; set; } = OperationResponseAcceptor.Unknow;
        public SettingCommandResponseAcceptor settingAcceptorResponse { get; set; } = SettingCommandResponseAcceptor.Unknow;
        public SettingStatusRequestCommandResponseAcceptor settingStatusAcceptorResponse { get; set; } = SettingStatusRequestCommandResponseAcceptor.Unknow;

        public StatusResponseRC statusRCResponse { get; set; } = StatusResponseRC.Unknow;
        public ErrorResponseRC errorRCResponse { get; set; } = ErrorResponseRC.Unknow;
        public OperationResponseRC opertationRCResponse { get; set; } = OperationResponseRC.Unknow;
        public SettingCommandResponseRC settingRCResponse { get; set; } = SettingCommandResponseRC.Unknow;
        public SettingStatusRequestCommandResponseRC settingStatusRCResponse { get; set; } = SettingStatusRequestCommandResponseRC.Unknow;


        /*

        public async Task<bool> command(byte actionCode, byte? responseExpected, byte[]? dataAction = null, int? minimLength = null)
        {
            
            byte[] constructedMessage = ConstructMessageWithCrc(actionCode, dataAction);
            //this will be the expected response need improvment with lenght chek
            // byte[] constructedResponse = ConstructMessage(responseExpected);
            byte[]? constructedResponse = null;
            if (responseExpected.HasValue)
            {
                constructedResponse = ConstructMessageWithCrc(responseExpected.Value);
                _device.expectedResponse = true;
            }
            else
            {
                _device.expectedResponse = false;
            }
            var timeout = TimeSpan.FromSeconds(timeOutTask);
            _device.expectingStringResponse = false;
            _device.nbrMessageExpected = 1;
            //fixedLenghtEndingBytes true if the message has a fixe lenght expected
            //false if the message expecte has ending bytes
            _device.minimuLenghtEndingBytes = true;

            //The response of Cash device can have different lenght, depends if they have data incorporated or not
            //if not, default lenght is 5 byte otherwise is important to know the minimum lenght expected to validate the response
            //and send it in advence as minimLength as argument
            if (minimLength.HasValue && minimLength > 5)
            {
                _device.minimumLength = minimLength.Value;
            }
            else
            {
                _device.minimumLength = 5;
            }

            var tcs = new TaskCompletionSource<bool>();

            EventHandler dataReceivedHandler = (sender, args) =>
            {
                //If reset command is recived correctly, resetResponse will contain 2 array of byte
                //The first array is that ACK from coin device, the second one is the message  
                if (_device.expectedResponse)
                {
                    byte[][] response = _device.response;

                    if (response.Count() == _device.nbrMessageExpected && response[0].Length >= _device.minimumLength && response[0].Length >= 3)
                    {
                        byte[] responseHex = _device.response[0];
                        SequenceFeedback(responseHex);
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        Console.WriteLine("dispositivo disabilitato");
                        tcs.TrySetResult(false);
                    }

                }
                else
                {
                    tcs.TrySetResult(true);
                }
            };

            _device.DataReceived += dataReceivedHandler; // Subscribe to the DataReceived event

            // Send the reset message to the device
            // Assume SendMessage is a method to send data in your device class
            _device.SendMessage(constructedMessage);

            // Wait for either the response to be received or for the timeout
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout));

            // Unsubscribe from the DataReceived event to avoid memory leaks
            _device.DataReceived -= dataReceivedHandler;

            // return the result of task if is done ore not, and false if task time excede

            if (completedTask == tcs.Task && await tcs.Task)
            {
                //handel response : ACK mean no coin introduce, rejectedCoin o handal value of coin

                return true;
            }
            else
            {
                Console.WriteLine("something went wrong, device dosen't answare");
                return false;
            }
            
            return true;
        }
        */
        private byte[] ConstructMessageWithCrc(byte command, byte[]? data = null)
        {
            // Calculate the length of the full message: SYNC + LNG + COMMAND + DATA (if any) + CRC
            int dataLength = data != null ? data.Length : 0;
            int fullLength = 1 + 1 + 1 + dataLength + 2; // SYNC + LNG + COMMAND + DATA + CRC

            // Create the message array with the calculated length
            byte[] message = new byte[fullLength];

            // Fill in the message
            message[0] = SyncByte; // SYNC byte
            message[1] = (byte)fullLength; // LNG byte, length of the full message
            message[2] = command; // COMMAND byte

            // If there's data, copy it into the message
            if (data != null && dataLength > 0)
            {
                Array.Copy(data, 0, message, 3, dataLength);
            }

            // Calculate the CRC for the message excluding the CRC itself
            byte[] messageWithCrc = CRC16Kermit.CalculateAndAppendCRC(message[..(fullLength - 2)]);
            return messageWithCrc;
        }
    }
}