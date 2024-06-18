

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

        public readonly byte[] Ack_Command = [0x01, 0x30, 0x30, 0x31, 0x31, 0x30, 0x30, 0x30, 0x30, 0x31, 0x31, 0x04];
        public readonly byte[] Ack_Polling = [0x01, 0x31, 0x30, 0x31, 0x31, 0x30, 0x31, 0x30, 0x30, 0x30, 0x30, 0x04];


        public async Task<bool> Listener()
        {
            return true;
        }


    }

}