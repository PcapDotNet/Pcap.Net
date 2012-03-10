using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 2671, 3225.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), Flags]
    public enum DnsOptFlags : ushort
    {
        /// <summary>
        /// RFC 3225.
        /// Setting the this bit to one in a query indicates to the server that the resolver is able to accept DNSSEC security RRs.
        /// Cleareing (setting to zero) indicates that the resolver is unprepared to handle DNSSEC security RRs 
        /// and those RRs must not be returned in the response (unless DNSSEC security RRs are explicitly queried for).
        /// The bit of the query MUST be copied in the response.
        /// </summary>
        DnsSecOk = 0x8000,
    }
}