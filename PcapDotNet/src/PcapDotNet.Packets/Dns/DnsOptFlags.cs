using System;

namespace PcapDotNet.Packets.Dns
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), Flags]
    public enum DnsOptFlags : ushort
    {
        DnsSecOk = 0x8000,
    }
}