using PayStationSW.Devices;
using static PayStationSW.ProtocolCashino;

namespace PayStationSW.Protocols.Printer
{
    public interface InterfacePrinterProtocol
    {

        CommandParameter PrintTestPage();
    }
}
