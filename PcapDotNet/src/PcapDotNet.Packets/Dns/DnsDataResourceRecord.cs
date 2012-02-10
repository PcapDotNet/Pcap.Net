using System;
using PcapDotNet.Base;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public bool Equals(DnsDataResourceRecord other)
        {
            return EqualsBase(other) &&
                   Ttl.Equals(other.Ttl) &&
                   Data.Equals(other.Data);
        }

        public override bool Equals(object obj)
        {
 	        return Equals(obj as DnsDataResourceRecord);
        }

        public override int GetHashCode()
        {
            return GetHashCodeBase() ^ Sequence.GetHashCode(Ttl, Data);
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
}