using PayStationName.Devices;
using PayStationName.Protocols.Cash;
using PayStationName.Protocols.Coin;
using PayStationName.Protocols.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayStationName
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

