using PayStationName.Devices;
using static PayStationName.ProtocolVega100;

namespace PayStationName.Protocols.Cash
{
    public interface InterfaceCashProtocol
    {

       // CommandParameter TestCommand();
        CommandParameter ResetCommand();
        CommandParameter EnableCommand();
        CommandParameter DisableCommand();


        // Task<bool> PowerUPSequence();
        //Task<bool> PayOutSequence();


        StatusResponseAcceptor statusAcceptorResponse { get; set; } 
        PowerUpResponseAcceptor powerUpAcceptorResponse { get; set; } 
        ErrorResponseAcceptor errorAcceptorResponse { get; set; } 
        OperationResponseAcceptor opertationAcceptorResponse { get; set; }
        SettingCommandResponseAcceptor settingAcceptorResponse { get; set; } 
        SettingStatusRequestCommandResponseAcceptor settingStatusAcceptorResponse { get; set; }
        StatusResponseRC statusRCResponse { get; set; }
        ErrorResponseRC errorRCResponse { get; set; }
        OperationResponseRC opertationRCResponse { get; set; }
        SettingCommandResponseRC settingRCResponse { get; set; }
        SettingStatusRequestCommandResponseRC settingStatusRCResponse { get; set; }

    }
}