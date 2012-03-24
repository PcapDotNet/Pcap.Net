using System;
using System.Collections.Generic;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// All RRs have the same top level format shown below:
    /// <pre>
    /// +------+-------------------------------------------------+
    /// | byte | 0-1                                             |
    /// +------+-------------------------------------------------+
    /// | 0    | Name                                            |
    /// | ...  |                                                 |
    /// +------+-------------------------------------------------+
    /// |      | Type                                            |
    /// +------+-------------------------------------------------+
    /// |      | Class                                           |
    /// +------+-------------------------------------------------+
    /// |      | TTL (not available in queries)                  |
    /// |      |                                                 |
    /// +------+-------------------------------------------------+
    /// |      | Resource Data Length (not available in queries) |
    /// +------+-------------------------------------------------+
    /// |      | Resource Data (not available in queries)        |
    /// | ...  |                                                 |
    /// +------+-------------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class DnsResourceRecord
    {
        private static class OffsetAfterDomainName
        {
            public const int Type = 0;
            public const int DnsClass = 2;
        }

        private const int MinimumLengthAfterDomainName = 4;

        /// <summary>
        /// An owner name, i.e., the name of the node to which this resource record pertains.
        /// </summary>
        public DnsDomainName DomainName { get; private set; }

        /// <summary>
        /// Two octets containing one of the RR TYPE codes.
        /// </summary>
        public DnsType DnsType { get; private set; }

        /// <summary>
        /// Two octets containing one of the RR CLASS codes.
        /// </summary>
        public DnsClass DnsClass { get; private set; }

        /// <summary>
        /// A 32 bit signed integer that specifies the time interval that the resource record may be cached before the source of the information should again be consulted.  
        /// Zero values are interpreted to mean that the RR can only be used for the transaction in progress, and should not be cached.  
        /// For example, SOA records are always distributed with a zero TTL to prohibit caching.  
        /// Zero values can also be used for extremely volatile data.
        /// Not available in queries.
        /// </summary>
        public abstract int Ttl { get; protected set; }

        /// <summary>
        /// A variable length string of octets that describes the resource.  
        /// The format of this information varies according to the TYPE and CLASS of the resource record.
        /// For example, the if the TYPE is A and the CLASS is IN, the RDATA field is a 4 octet ARPA Internet address.
        /// Not available in queries.
        /// </summary>
        public abstract DnsResourceData Data { get; protected set; }

        /// <summary>
        /// A string representing the resource record by concatenating its different parts.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", DomainName, DnsType, DnsClass);
        }

        internal DnsResourceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass)
        {
            DomainName = domainName;
            DnsType = type;
            DnsClass = dnsClass;
        }

        internal bool EqualsBase(DnsResourceRecord other)
        {
            return other != null &&
                   DomainName.Equals(other.DomainName) &&
                   DnsType.Equals(other.DnsType) &&
                   DnsClass.Equals(other.DnsClass);
        }

        internal int GetHashCodeBase()
        {
            return Sequence.GetHashCode(DomainName, BitSequence.Merge((ushort)DnsType, (ushort)DnsClass));
        }

        internal static bool TryParseBase(DnsDatagram dns, int offsetInDns,
                                          out DnsDomainName domainName, out DnsType type, out DnsClass dnsClass, out int numBytesRead)
        {
            type = DnsType.Any;
            dnsClass = DnsClass.Any;
            if (!DnsDomainName.TryParse(dns, offsetInDns, dns.Length - offsetInDns, out domainName, out numBytesRead))
                return false;

            if (offsetInDns + numBytesRead + MinimumLengthAfterDomainName > dns.Length)
                return false;

            type = (DnsType)dns.ReadUShort(offsetInDns + numBytesRead + OffsetAfterDomainName.Type, Endianity.Big);
            dnsClass = (DnsClass)dns.ReadUShort(offsetInDns + numBytesRead + OffsetAfterDomainName.DnsClass, Endianity.Big);
            numBytesRead += MinimumLengthAfterDomainName;
            return true;
        }

        internal int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return DomainName.GetLength(compressionData, offsetInDns) + MinimumLengthAfterDomainName + GetLengthAfterBase(compressionData, offsetInDns);
        }

        internal abstract int GetLengthAfterBase(DnsDomainNameCompressionData compressionData, int offsetInDns);

        internal virtual int Write(byte[] buffer, int dnsOffset, DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = 0;
            length += DomainName.Write(buffer, dnsOffset, compressionData, offsetInDns + length);
            buffer.Write(dnsOffset + offsetInDns + length, (ushort)DnsType, Endianity.Big);
            length += sizeof(ushort);
            buffer.Write(dnsOffset + offsetInDns + length, (ushort)DnsClass, Endianity.Big);
            length += sizeof(ushort);
            return length;
        }
    }
}