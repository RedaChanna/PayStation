using PayStationName.Devices;
using static PayStationName.ProtocolCashino;

namespace PayStationName.Protocols.Printer
{
    public interface InterfacePrinterProtocol
    {

        CommandParameter PrintTestPage();
    }
}
