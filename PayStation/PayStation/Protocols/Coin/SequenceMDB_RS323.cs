using Mysqlx.Session;

namespace PayStationSW
{

    public partial class ProtocolMDB_RS323
    {

        public CommandParameter ResetCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = resetRequest;
            _commandParameter.validateAnyResponse = false;
            _commandParameter.expectedMultipleResponse = true;
            _commandParameter.nmbrResponseExpected = 2;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }

        public CommandParameter SetUpCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = setUpRequest;
            _commandParameter.expectedMinLength = true;
            _commandParameter.expectedMinLengthList = [24];
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }

        public CommandParameter SetUpExpansionCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = setUpExpansionRequest;
            _commandParameter.expectedMinLength = true;
            _commandParameter.expectedMinLengthList = [36];
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true; ;
            return _commandParameter;
        }

        public CommandParameter SetUpFeautureCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = setUpFeatureRequest;
            _commandParameter.expectedResponse = true;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true ;
            return _commandParameter;
        }


        public CommandParameter InhibitionCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = inhibitionRequest;
            _commandParameter.expectedResponse = true;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }

        public CommandParameter DisinhibitionCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = disinhibitionRequest;
            _commandParameter.expectedResponse = true;
            _commandParameter.messageEndingBytes = [0x0D, 0x0A];
            _commandParameter.messageResponseHaveEndingByte = true;
            return _commandParameter;
        }



        public CommandParameter StatusCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = coinIntroducedRequest;
            _commandParameter.validateAnyResponse = true;
            return _commandParameter;
        }

    }
}