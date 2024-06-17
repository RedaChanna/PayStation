using PayStationSW.Devices;
using static PayStationSW.ProtocolMDB_RS323;

namespace PayStationSW.Protocols.Coin
{
    public interface InterfaceCoinProtocol
    {


        CommandParameter ResetCommand();
        CommandParameter SetUpCommand();
        CommandParameter SetUpExpansionCommand();

        CommandParameter SetUpFeautureCommand();


        CommandParameter InhibitionCommand();
        CommandParameter DisinhibitionCommand();
        CommandParameter StatusCommand();


        Task<bool> setUp();
        Task<bool> setUpFeauture();
        Task<bool> setUpExpansion();
        Task<bool> coinIntroducedLstn();
    }
}
