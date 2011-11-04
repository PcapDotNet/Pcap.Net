using System;

namespace PcapDotNet.Packets.Dns
{
    public class DnsDataResourceRecord : DnsResourceRecord, IEquatable<DnsDataResourceRecord>
    {
        private static class OffsetAfterBase
        {
            public const int Ttl = 0;
            public const int DataLength = 4;
            public const int Data = 6;
        }

        private const int MinimumLengthAfterBase = 6;

        public DnsDataResourceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass, int ttl, DnsResourceData data) 
            : base(domainName, type, dnsClass)
        {
            Ttl = ttl;
            Data = data;
        }

        /// <summary>
        /// A 32 bit signed integer that specifies the time interval that the resource record may be cached before the source of the information should again be consulted.  
        /// Zero values are interpreted to mean that the RR can only be used for the transaction in progress, and should not be cached.  
        /// For example, SOA records are always distributed with a zero TTL to prohibit caching.  
        /// Zero values can also be used for extremely volatile data.
        /// </summary>
        public override int Ttl { get; protected set; }

        /// <summary>
        /// A variable length string of octets that describes the resource.  
        /// The format of this information varies according to the TYPE and CLASS of the resource record.
        /// For example, the if the TYPE is A and the CLASS is IN, the RDATA field is a 4 octet ARPA Internet address.
        /// </summary>
        public override DnsResourceData Data { get; protected set; }

        public override string ToString()
        {
            return base.ToString() + " " + Ttl + " " + Data;
        }

        public bool Equals(DnsDataResourceRecord other)
        {
            return EqualsBase(other) &&
                   Ttl.Equals(other.Ttl) &&
                   Data.Equals(other.Data);
        }

        public override bool  Equals(object obj)
        {
 	        return Equals(obj as DnsDataResourceRecord);
        }

        internal static DnsDataResourceRecord Parse(DnsDatagram dns, int offsetInDns, out int numBytesRead)
        {
            DnsDomainName domainName;
            DnsType type;
            DnsClass dnsClass;
            if (!TryParseBase(dns, offsetInDns, out domainName, out type, out dnsClass, out numBytesRead))
                return null;

            if (offsetInDns + numBytesRead + MinimumLengthAfterBase > dns.Length)
                return null;

            int ttl = dns.ReadInt(offsetInDns + numBytesRead + OffsetAfterBase.Ttl, Endianity.Big);
            ushort dataLength = dns.ReadUShort(offsetInDns + numBytesRead + OffsetAfterBase.DataLength, Endianity.Big);
            numBytesRead += MinimumLengthAfterBase;
            if (offsetInDns + numBytesRead + dataLength > dns.Length)
                return null;
            DnsResourceData data = DnsResourceData.Read(dns, type, dnsClass, offsetInDns + numBytesRead, dataLength);
            if (data == null)
                return null;
            numBytesRead += dataLength;

            if (type == DnsType.Opt)
                return new DnsOptResourceRecord(domainName, dnsClass, ttl, data);
            return new DnsDataResourceRecord(domainName, type, dnsClass, ttl, data);
        }

        internal override int GetLengthAfterBase(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return MinimumLengthAfterBase + Data.GetLength(compressionData, offsetInDns);
        }

        internal override int Write(byte[] buffer, int dnsOffset, DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = base.Write(buffer, dnsOffset, compressionData, offsetInDns);
            buffer.Write(dnsOffset + offsetInDns + length, Ttl, Endianity.Big);
            length += sizeof(uint);
            length += Data.Write(buffer, dnsOffset, offsetInDns + length, compressionData);
            return length;
        }
    }

    public enum DnsOptVersion : byte
    {
        Version0 = 0,
    }

    [Flags]
    public enum DnsOptFlags : ushort
    {
        DnsSecOk = 0x8000,
    }

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
        public DnsOptResourceRecord(DnsDomainName domainName, ushort sendersUdpPayloadSize, byte extendedRcode, DnsOptVersion version, DnsOptFlags flags, DnsResourceDataOptions data)
            : this(domainName, (DnsClass)sendersUdpPayloadSize, (extendedRcode << 24) | ((byte)version << 16) | (ushort)flags, data)
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
        public byte ExtendedRcode { get { return (byte)(Ttl >> 24); } }

        /// <summary>
        /// Indicates the implementation level of whoever sets it.
        /// Full conformance with this specification is indicated by version "0".
        /// Requestors are encouraged to set this to the lowest implemented level capable of expressing a transaction, 
        /// to minimize the responder and network load of discovering the greatest common implementation level between requestor and responder.
        /// A requestor's version numbering strategy should ideally be a run time configuration option.
        /// </summary>
        public DnsOptVersion Version { get { return (DnsOptVersion)(Ttl >> 16); }}

        public DnsOptFlags Flags { get { return (DnsOptFlags)Ttl; } }

        internal DnsOptResourceRecord(DnsDomainName domainName, DnsClass dnsClass, int ttl, DnsResourceData data)
            : base(domainName, DnsType.Opt, dnsClass, ttl, data)
        {
        }
    }
}