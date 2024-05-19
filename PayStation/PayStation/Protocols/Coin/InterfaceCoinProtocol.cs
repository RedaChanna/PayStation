using PayStationSW.Devices;
using static PayStationSW.ProtocolMDB_RS323;

namespace PayStationSW.Protocols.Coin
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
