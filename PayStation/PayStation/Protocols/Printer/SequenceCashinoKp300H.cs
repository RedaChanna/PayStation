using Mysqlx.Session;

namespace PayStationName
{ 
    public partial class ProtocolCashino
    {



        public CommandParameter PrintTestPage()
        {
            byte[] printTestPage = [0x10, 0x40, 0x12, 0x54];

            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = printTestPage;
            _commandParameter.expectedResponse = false;
            return _commandParameter;

        }
    }
}