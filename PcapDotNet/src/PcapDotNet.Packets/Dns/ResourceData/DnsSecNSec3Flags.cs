using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 5155.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), Flags]
    public enum DnsSecNSec3Flags : byte
    {
        None = 0x00,

        /// <summary>
        /// RFC 5155.
        /// Indicates whether this NSEC3 RR may cover unsigned delegations.
        /// If set, the NSEC3 record covers zero or more unsigned delegations.
        /// If clear, the NSEC3 record covers zero unsigned delegations.
        /// </summary>
        OptOut = 0x01,
    }
}