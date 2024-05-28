using Mysqlx.Session;

namespace PayStationSW
{


    public partial class ProtocolMDB_RS323
    {

        public async Task<bool> PowerUPSequence()
        {
            /*
            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)PowerUpResponseAcceptor.PowerUpWithBillInAcceptor);
            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)PowerUpResponseAcceptor.PowerUpWithBillInStacker);


            await command((byte)OperationCommandAcceptor.Reset, (byte)OperationResponseAcceptor.ACK);

            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)StatusResponseAcceptor.Initialize);
            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)StatusResponseAcceptor.StatusRequest);
            */
            return false;
        }


        public CommandParameter ResetCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = resetRequest;
            _commandParameter.validateAnyResponse = true;
            return _commandParameter;
        }

        public CommandParameter EnableCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = enableRequest;
            _commandParameter.validateAnyResponse = true;
            return _commandParameter;
        }

        public CommandParameter DisableCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = disableRequest;
            _commandParameter.validateAnyResponse = true;
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