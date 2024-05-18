using PayStationName.Devices;
using static PayStationName.ProtocolMDB_RS323;

namespace PayStationName.Protocols.Coin
{
    public interface InterfaceCoinProtocol
    {


        CommandParameter ResetCommand();
        CommandParameter EnableCommand();
        CommandParameter DisableCommand();


        Task<bool> setUp();
        Task<bool> setUpFeauture();
        Task<bool> expansionSetUp();
        Task<bool> coinIntroducedLstn();
    }
}
