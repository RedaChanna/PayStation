

using PayStationSW.Devices;
using static PayStationSW.ProtocolMDB_RS323;

namespace PayStationSW.Protocols.Web2Park
{
    public interface InterfaceWeb2Park
    {


        CommandParameter AckCommand();
        CommandParameter AckPolling();
        CommandParameter ListenerCommand();
        CommandParameter SendTestCommand();
        
        Task<bool> Listener();

    }
}
