using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2671.
    /// <pre>
    /// +------+----------------+----------+
    /// | bit  | 0-7            | 8-15     |
    /// +------+----------------+----------+
    /// | 0    | Name                      |
    /// | ...  |                           |
    /// +------+---------------------------+
    /// |      | Type = OPT                |
    /// +------+---------------------------+
    /// |      | Sender's UDP payload size |
    /// +------+----------------+----------+
    /// |      | EXTENDED-RCODE | VERSION  |
    /// +------+----------------+----------+
    /// |      | Flags                     |
    /// +------+---------------------------+
    /// |      | Resource Data Length      |
    /// +------+---------------------------+
    /// |      | Resource Data             |
    /// | ...  |                           |
    /// +------+---------------------------+
    /// </pre>
    /// </summary>
    public sealed class DnsOptResourceRecord : DnsDataResourceRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags")]
        public DnsOptResourceRecord(DnsDomainName domainName, ushort sendersUdpPayloadSize, byte extendedReturnCode, DnsOptVersion version, DnsOptFlags flags, DnsResourceDataOptions data)
            : this(domainName, (DnsClass)sendersUdpPayloadSize, (int)BitSequence.Merge(extendedReturnCode, (byte)version, (ushort)flags), data)
        {
        }

        /// <summary>
        /// The number of octets of the largest UDP payload that can be reassembled and delivered in the sender's network stack.
        /// Note that path MTU, with or without fragmentation, may be smaller than this.
        /// </summary>
        public ushort SendersUdpPayloadSize { get { return (ushort)DnsClass; } }

        /// <summary>
        /// Forms upper 8 bits of extended 12-bit RCODE.
        /// Note that EXTENDED-RCODE value "0" indicates that an unextended RCODE is in use (values "0" through "15").
        /// </summary>
        public byte ExtendedReturnCode { get { return (byte)(Ttl >> 24); } }

        /// <summary>
        /// Indicates the implementation level of whoever sets it.
        /// Full conformance with this specification is indicated by version "0".
        /// Requestors are encouraged to set this to the lowest implemented level capable of expressing a transaction, 
        /// to minimize the responder and network load of discovering the greatest common implementation level between requestor and responder.
        /// A requestor's version numbering strategy should ideally be a run time configuration option.
        /// </summary>
        public DnsOptVersion Version { get { return (DnsOptVersion)(Ttl >> 16); }}

        /// <summary>
        /// OPT flags.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public DnsOptFlags Flags { get { return (DnsOptFlags)Ttl; } }

        /// <summary>
        /// A string representing the resource record by concatenating its different parts.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}", DomainName, DnsType, SendersUdpPayloadSize, ExtendedReturnCode, Version, Flags, Data);
        }

        internal DnsOptResourceRecord(DnsDomainName domainName, DnsClass dnsClass, int ttl, DnsResourceData data)
            : base(domainName, DnsType.Opt, dnsClass, ttl, data)
        {
        }
    }
}