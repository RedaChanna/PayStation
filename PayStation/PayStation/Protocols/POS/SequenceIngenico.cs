using Microsoft.AspNetCore.SignalR.Protocol;
using Mysqlx.Session;
using PayStationSW.Protocols.POS;
using PayStationSW.Devices;

namespace PayStationSW
{

    public partial class ProtocolIngenico
    {

        public CommandParameter ACKCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = ACK_command;
            _commandParameter.expectedResponse = false;
            return _commandParameter;
        }
        public CommandParameter ResetCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = [0x12];
            _commandParameter.validateAnyResponse = false;
            _commandParameter.expectedMultipleResponse = true;
            _commandParameter.nmbrResponseExpected = 2;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }

        public CommandParameter ActivationCommand(byte[] Terminal_ID)
        {

            List<byte[]> MessageArrays;
            byte[] fill_array_1 = MessageConstructIgenico.RepeatBytes([0x30], 1);
            byte[] fill_array_2 = MessageConstructIgenico.RepeatBytes([0x30], 16);
            MessageArrays = new List<byte[]>()
            {
                STX,
                Terminal_ID,
                fill_array_1,
                activation_request,
                fill_array_2,
                ETX,
            };

            byte[] MessageConcatenated = MessageConstructIgenico.ConstructMessage(MessageArrays);
            byte[] Message_to_send = MessageConstructIgenico.AppendLCR(MessageConcatenated);
            CommandParameter _commandParameter = new CommandParameter();

            _commandParameter.messageToSendBytes = Message_to_send;
            _commandParameter.expectedResponse = true;
            _commandParameter.expectedMultipleResponse = true;
            _commandParameter.nmbrResponseExpected = 2;
            _commandParameter.multipleMessageDifferentValidation = true;

            _commandParameter.ListMessageEndingBytes = new List<byte[]> { new byte[] { 0x7A }, new byte[] { 0x03 } };
            _commandParameter.messageResponseHaveEndingByte = true;
            _commandParameter.ListMessageStartingBytes = new List<byte[]> { new byte[] { 0x06 }, new byte[] { 0x02 } };
            _commandParameter.messageResponseHaveStartingByte = true;
            _commandParameter.retryDelayMilliseconds = 10000;

            return _commandParameter;
  
        }

        public CommandParameter ListenerCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.listenerMode = true;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            _commandParameter.messageStartingBytes = [0x30];
            _commandParameter.messageResponseHaveStartingByte = true;
            _commandParameter.maxRetryAttempts = 100;
            return _commandParameter;
        }

    }
}