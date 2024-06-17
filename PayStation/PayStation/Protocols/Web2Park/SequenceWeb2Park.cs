using Mysqlx.Session;

namespace PayStationSW
{

    public partial class ProtocolWeb2Park
    {

        public CommandParameter AckCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = ACK;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.expectedMultipleResponse = true;
            _commandParameter.nmbrResponseExpected = 2;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }

        public CommandParameter SendTestCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = ACK;
            _commandParameter.expectedResponse = false;
            return _commandParameter;
        }
        public CommandParameter ListenerCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.listenerMode = true;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }


    }
}