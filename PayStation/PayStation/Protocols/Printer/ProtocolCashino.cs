using PayStationSW.Devices;
using PayStationSW.Protocols.Cash;
using PayStationSW.Protocols.Coin;
using PayStationSW.Protocols.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayStationSW
{

    public partial class ProtocolCashino : InterfacePrinterProtocol
    {
        private readonly Device _device;
        private int timeOutTask = 10;

        public ProtocolCashino(Device dvice)
        {
            _device = dvice;
        }

    }
}

