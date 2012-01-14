using System;

namespace PcapDotNet.Packets.Dns
{
    [Flags]
    public enum DnsOptFlags : ushort
    {
        DnsSecOk = 0x8000,
    }
}