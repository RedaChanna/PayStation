

using Google.Protobuf;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Session;
using PayStationSW.Devices;
using PayStationSW.Protocols.Web2Park;
using System.Collections;
using System.Linq;
using System.Reflection.Metadata;

namespace PayStationSW
{
    public partial class ProtocolWeb2Park : InterfaceWeb2Park
    {
        private readonly Device _device;

        public ProtocolWeb2Park(Device device)
        {
            _device = device;
        }

        //Response
        private readonly byte[] ACK = [0x30, 0x30, 0x20, 0x0D, 0x0A];



        public async Task<bool> Listener()
        {
            return true;
        }


    }

}