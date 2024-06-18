﻿

using Google.Protobuf.WellKnownTypes;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace PayStationSW
{

    public partial class CommandDataResponseWeb2Park
    {

        public readonly byte[] Ack_Polling = [0x01, 0x30, 0x30, 0x31, 0x31, 0x30, 0x30, 0x30, 0x30, 0x31, 0x31, 0x04];
        public readonly byte[] Ack_Command = [0x01, 0x31, 0x30, 0x31, 0x31, 0x30, 0x31, 0x30, 0x30, 0x30, 0x30, 0x04];


    }
}