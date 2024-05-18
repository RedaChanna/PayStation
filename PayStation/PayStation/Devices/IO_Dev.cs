using PayStationName.Protocols.Coin;
using PayStationName.Protocols.POS;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayStationName.Devices
{
    public class IO_Dev : Device
    {
        private readonly InterfacePOSProtocol _protocol;

        public IO_Dev()
        {
            DeviceType = DeviceEnum.IO;

            _protocol = new ProtocolIngenico(this);
        }
        public override void ApplyConfig()
        {

        }

        public override void IdentifyDevice()
        {
        }
    }
}