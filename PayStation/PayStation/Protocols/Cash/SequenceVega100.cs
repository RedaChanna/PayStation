using Mysqlx.Session;

namespace PayStationSW
{

    public partial class ProtocolVega100
    {

        private void SequenceFeedback(byte[] response)
        {

        }

        /*
        private void PollingFeedback(byte[] response)
        {
            byte cmdResponse = response[2];

            if (Enum.IsDefined(typeof(StatusCashDevice), cmdResponse))
            {
                statusPolling = (StatusCashDevice)cmdResponse;
                commandsPolling = CommandsCashDevice.Unknow;
                responsePolling = ResponseCashDevice.Unknow;
            }
            else if (Enum.IsDefined(typeof(ResponseCashDevice), cmdResponse))
            {
                statusPolling = StatusCashDevice.Unknow;
                commandsPolling = CommandsCashDevice.Unknow;
                responsePolling = (ResponseCashDevice)cmdResponse;
            }
            else if (Enum.IsDefined(typeof(CommandsCashDevice), cmdResponse))
            {
                statusPolling = StatusCashDevice.Unknow;
                commandsPolling = (CommandsCashDevice)cmdResponse;
                responsePolling = ResponseCashDevice.Unknow;
            }
            else
            {
                statusPolling = StatusCashDevice.Unknow;
                commandsPolling = CommandsCashDevice.Unknow;
                responsePolling = ResponseCashDevice.Unknow;
            }
            if (statusPolling == StatusCashDevice.Escrow && Enum.IsDefined(typeof(EscrowData), response[3]) && (response.Length >= 4))
            {
                string billIntroduced = EscrowDataString(response[3]);
                Console.WriteLine(billIntroduced);
            }
        }
        */


        /* PowerUPSequence
         * 
         * StatusRquest ->
         *      <-Power Up With bill in acceptor || <-Power Up With bill in stacker
         * 
         * Reset->
         *      <- ACK
         * 
         * StatusRquest ->
         *      <-Initialize
         * 
         * StatusRquest ->
         *      <-Enable
        */
        /*
        public async Task<bool> PowerUPSequence()
        {

            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)PowerUpResponseAcceptor.PowerUpWithBillInAcceptor);
            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)PowerUpResponseAcceptor.PowerUpWithBillInStacker);


            await command((byte)OperationCommandAcceptor.Reset, (byte)OperationResponseAcceptor.ACK);

            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)StatusResponseAcceptor.Initialize);
            await command((byte)StatusCommandAcceptor.StatusRequest, (byte)StatusResponseAcceptor.StatusRequest);

            return false;
        }

        */

        public CommandParameter ResetCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = [0xFC, 0x05, 0x40, 0x2B, 0x15];
            _commandParameter.validateAnyResponse = true;
            return _commandParameter;
        }

        public CommandParameter EnableCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = [0xFC, 0x06, 0xC3, 0x00, 0x04, 0xD6];
            _commandParameter.validateAnyResponse = true;
            return _commandParameter;
        }

        public CommandParameter DisableCommand()
        {
            CommandParameter _commandParameter = new CommandParameter();
            _commandParameter.messageToSendBytes = [0xFC, 0x06, 0xC3, 0x01, 0x8D, 0xC7];
            _commandParameter.validateAnyResponse = true;
            return _commandParameter;
        }

        /*


        public async Task<bool> PayOutSequence()
        {
            //Controller inhibit set
            //Accptor inhibit set
            //Controller PayoutRequest
            //Accptor ACK
            //Controller Status Reuqest
            //Accptor Paying
            //Controller Status Reuqest
            //Accptor PayStay            
            //Controller Status Reuqest
            //Accptor PayValid
            //Controller ACK
            //Controller Inhibit

            return false;

        }
        public async void Polling(bool enablePolling)
        {
            /*
            bool ValideResponse = true;
            while (IsEnabled && enablePolling && ValideResponse)
            {
                switch (_protocol.statusPolling)
                {
                    case StatusCashDevice.Unknow:
                        if (_protocol.responsePolling == ResponseCashDevice.ACK)
                        {
                            ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        }
                        else
                        {
                            ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        }
                        break;
                    case StatusCashDevice.Accepting:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.Escrow);
                        break;
                    case StatusCashDevice.StatusRequest:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.Escrow:
                        if (_protocol.responsePolling == ResponseCashDevice.ACK)
                        {
                            ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        }
                        else
                        {
                            ValideResponse = await _protocol.command((byte)CommandsCashDevice.Stack1, (byte)ResponseCashDevice.ACK);
                        }
                        break;
                    case StatusCashDevice.Stacking:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.VendValid:
                        ValideResponse = await _protocol.command((byte)ResponseCashDevice.ACK, null);
                        Console.WriteLine(ValideResponse);
                        if (ValideResponse)
                        {
                            _protocol.statusPolling = StatusCashDevice.StatusRequest;
                            _protocol.commandsPolling = CommandsCashDevice.Unknow;
                            _protocol.responsePolling = ResponseCashDevice.Unknow;
                        }
                        break;
                    case StatusCashDevice.Stacked:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.Rejecting:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.Returning:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.Holding:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.DisableInhibit:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    case StatusCashDevice.Initialize:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                    default:
                        ValideResponse = await _protocol.command((byte)StatusCashDevice.StatusRequest, (byte)StatusCashDevice.StatusRequest);
                        break;
                }
            }
        }
    */
    }
}




/*


PowerUPSequence {

StatusRquest ->
<-Power Up With bill in acceptor || <-Power Up With bill in stacker

Reset->
<- ACK

StatusRquest ->
<-Initialize

StatusRquest ->
<-Enable

}

RecivingBillStack1Sequence {

StatusRquest ->
<-Enable

StatusRquest ->             when a bill is intoduced
<-Accepting


StatusRquest ->             when a bill is riconized and data are sended to controller, if no status request or command recived after 3s/10s the acceptor return the bill
<-Escrow
Stack1->
<-ACK



StatusReuqest->
<- Stacking

StatusReuqest->
<- Vend Valid
ACK ->

StatusRquest ->
<-Stacked

StatusRquest ->
<-Enable

}




RecivingBillStack2Sequence {

StatusRquest ->
<-Enable

StatusRquest ->             when a bill is intoduced
<-Accepting


StatusRquest ->             when a bill is riconized and data are sended to controller, if no status request or command recived after 3s/10s the acceptor return the bill
<-Escrow
Stack2->
<-ACK



StatusReuqest->
<- Stacking

StatusReuqest->             VendValid will retrasmted for every status request until the ACKis not sent from CONTROLLER to ACCPETOR
<- Vend Valid
ACK ->

StatusRquest ->
<-Stacked

StatusRquest ->
<-Enable

}

//Return of bill from Process of conveyning for accomodation
RejectingBillAccomodationSequence {

StatusRquest ->
<-Enable

StatusRquest ->             when a bill is intoduced
<-Accepting


StatusRquest ->             when a bill is riconized and data are sended to controller, if no status request or command recived after 3s/10s the acceptor return the bill
<-Escrow
Stack1->
<-ACK



StatusReuqest->
<- Stacking

StatusReuqest->
<- Rejecting
ACK ->


StatusRquest ->
<-Enable


}





Return of Bill by command

ReturnBillByCommand {

StatusRquest ->
<-Enable

StatusRquest ->             when a bill is intoduced
<-Accepting


StatusRquest ->             when a bill is riconized and data are sended to controller, if no status request or command recived after 3s/10s the acceptor return the bill
<-Escrow
Return->
<-ACK



StatusReuqest->
<- Returning


StatusRquest ->
<-Enable


}




Inhibiting accpetor from reciving bills

InhibitingAcceptor {

StatusRquest ->
<-Enable

Inhibit (cmd/Status)->          
<-Echo of Inhibit (cmd/Status)


StatusRquest ->
<-Disable


}



StackerFullSequence {

StatusRquest ->
<-Enable

StatusRquest ->             when a bill is intoduced
<-Accepting

StatusReuqest->
<- Vend Valid
ACK ->

StatusRquest ->
<-Stacked

StatusRquest ->
<-Stacker Full

StatusRquest ->
<-initialize

StatusRquest ->
<-Disable

}




//Jam in return of bill
JamReturnOfBill {

StatusRquest ->
<-Enable

StatusRquest ->             when a bill is intoduced
<-Accepting

StatusReuqest->
<- Rejecting

StatusRquest ->
<-JamInAccpetor

StatusRquest ->
<-Disable

}






RC




PayOut 1bills{

StatusRequest->
<-Disable

StatusRequestRC->
<-Normal

PayOutRC(F0+4A)->
<-ACK

StatusRequest->
<-Paying

StatusRequest->
<-PayStay

StatusRequest->
<-PayValid
ACK->



StatusRequest->
<-Disable


}


PayOut Multiplebills{

StatusRequest->
<-Disable

StatusRequestRC->
<-Normal

PayOutRC(F0+4A) + data->
<-ACK

StatusRequest->
<-Paying

StatusRequest->
<-PayStay

StatusRequest->
<-PayValid
ACK->


StatusRequest->
<-Paying

StatusRequest->
<-PayStay

StatusRequest->
<-PayValid
ACK->

StatusRequest->
<-Disable


}



PayOutRCEmpty{

StatusRequest->
<-Disable

StatusRequestRC->
<-Empty

PayOutRC(F0+4A) + data->
<-InvalidCommand


StatusRequest->
<-Disable

}



//Whenduring despinsing there are not enough bills
PayOutMultipleRCEmpty{

StatusRequest->
<-Disable

StatusRequestRC->
<-Normal

PayOutRC(F0+4A) + data->
<-ACK

StatusRequest->
<-Paying

StatusRequest->
<-PayStay

StatusRequest->
<-PayValid
ACK->


StatusRequest->
<-Paying

StatusRequestRC->
<-Empty


StatusRequest->
<-RecyclerError


StatusRequest->
<-initialize (1B)

StatusRequest->
<-Disable

}




DA GESTIRE POWER INTERRUPT/HW RESET, ma l'investimento per l'implementazione per questo software non è giustificato da una reale casistica

arrivato alla 4.10 dispensing sequence a bill rejected
 
 * */
