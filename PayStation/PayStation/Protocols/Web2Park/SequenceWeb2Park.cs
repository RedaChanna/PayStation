using Mysqlx.Session;

namespace PayStationSW
{

    public partial class ProtocolWeb2Park
    {

        public CommandParameter AckCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = Ack_Command;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.expectedResponse = true;
            _commandParameter.messageEndingBytes = [0x04];
            _commandParameter.messageResponseHaveEndingByte = true;
            _commandParameter.messageStartingBytes = [0x01];
            _commandParameter.messageResponseHaveStartingByte = true;
            return _commandParameter;
        }

        public CommandParameter AckPolling()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = Ack_Polling;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.expectedResponse = true;
            _commandParameter.messageEndingBytes = [0x04];
            _commandParameter.messageResponseHaveEndingByte = true;
            _commandParameter.messageStartingBytes = [0x01];
            _commandParameter.messageResponseHaveStartingByte = true;
            return _commandParameter;
        }

        public CommandParameter SendTestCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = Ack_Command;
            _commandParameter.expectedResponse = false;
            return _commandParameter;
        }
        public CommandParameter ListenerCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.listenerMode = true;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.messageEndingBytes = [0x04];
            _commandParameter.messageResponseHaveEndingByte = true;
            _commandParameter.messageStartingBytes = [0x01];
            _commandParameter.messageResponseHaveStartingByte = true;
            _commandParameter.maxRetryAttempts = 100;
            return _commandParameter;
        }


    }
}