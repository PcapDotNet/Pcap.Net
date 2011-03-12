using System;
using System.Collections.Generic;

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
    public abstract class DnsResourceRecord
    {
        private static class OffsetAfterDomainName
        {
            public const int Type = 0;
            public const int DnsClass = 2;
        }

        private const int MinimumLengthAfterDomainName = 4;

        public DnsResourceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass) 
        {
            DomainName = domainName;
            Type = type;
            DnsClass = dnsClass;
        }

        /// <summary>
        /// An owner name, i.e., the name of the node to which this resource record pertains.
        /// </summary>
        public DnsDomainName DomainName { get; private set; }

        /// <summary>
        /// Two octets containing one of the RR TYPE codes.
        /// </summary>
        public DnsType Type { get; private set;}

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

        internal static bool TryParseBase(DnsDatagram dns, int offsetInDns, 
            out DnsDomainName domainName, out DnsType type, out DnsClass dnsClass, out int numBytesRead)
        {
            type = DnsType.All;
            dnsClass = DnsClass.Any;
            domainName = DnsDomainName.Parse(dns, offsetInDns, out numBytesRead);
            if (domainName == null)
                return false;

            if (offsetInDns + numBytesRead + MinimumLengthAfterDomainName >= dns.Length)
                return false;

            type = (DnsType)dns.ReadUShort(OffsetAfterDomainName.Type, Endianity.Big);
            dnsClass = (DnsClass)dns.ReadUShort(OffsetAfterDomainName.DnsClass, Endianity.Big);
            numBytesRead += MinimumLengthAfterDomainName;
            return true;
        }

        internal int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return DomainName.GetLength(compressionData, offsetInDns) + MinimumLengthAfterDomainName + GetLengthAfterBase(compressionData, offsetInDns);
        }

        internal abstract int GetLengthAfterBase(DnsDomainNameCompressionData compressionData, int offsetInDns);
    }

    public class DnsQueryResouceRecord : DnsResourceRecord
    {
        public DnsQueryResouceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass) 
            : base(domainName, type, dnsClass)
        {
        }

        public override int Ttl
        {
            get { throw new InvalidOperationException("No TTL in queries"); }
            protected set { throw new InvalidOperationException("No TTL in queries"); }
        }

        public override DnsResourceData Data
        {
            get { throw new InvalidOperationException("No Resource Data in queries"); }
            protected set { throw new InvalidOperationException("No Resource Data in queries"); }
        }

        internal static DnsQueryResouceRecord Parse(DnsDatagram dns, int offsetInDns, out int numBytesRead)
        {
            DnsDomainName domainName;
            DnsType type;
            DnsClass dnsClass;
            if (!TryParseBase(dns, offsetInDns, out domainName, out type, out dnsClass, out numBytesRead))
                return null;

            return new DnsQueryResouceRecord(domainName, type, dnsClass);
        }

        internal override int GetLengthAfterBase(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return 0;
        }
    }

    public class DnsDataResourceRecord : DnsResourceRecord
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

        internal static DnsDataResourceRecord Parse(DnsDatagram dns, int offsetInDns, out int numBytesRead)
        {
            DnsDomainName domainName;
            DnsType type;
            DnsClass dnsClass;
            if (!TryParseBase(dns, offsetInDns, out domainName, out type, out dnsClass, out numBytesRead))
                return null;

            offsetInDns += numBytesRead;

            if (offsetInDns + numBytesRead + MinimumLengthAfterBase >= dns.Length)
                return null;

            int ttl = dns.ReadInt(offsetInDns + numBytesRead + OffsetAfterBase.Ttl, Endianity.Big);
            ushort dataLength = dns.ReadUShort(offsetInDns + numBytesRead + OffsetAfterBase.DataLength, Endianity.Big);
            numBytesRead += MinimumLengthAfterBase;
            DnsResourceData data = DnsResourceData.Read(dns, offsetInDns + numBytesRead, dataLength);
            if (data == null)
                return null;
            numBytesRead += dataLength;

            return new DnsDataResourceRecord(domainName, type, dnsClass, ttl, data);
        }

        internal override int GetLengthAfterBase(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return MinimumLengthAfterBase + Data.GetLength(compressionData, offsetInDns);
        }
    }

    public abstract class DnsResourceData
    {
        internal static DnsResourceData Read(DnsDatagram dns, int offsetInDns, int dataLength)
        {
            return new DnsResourceDataUnknown(dns.SubSegment(offsetInDns, dataLength));
        }

        internal abstract int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns);
    }

    public class DnsResourceDataUnknown : DnsResourceData
    {
        public DnsResourceDataUnknown(DataSegment data)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Data.Length;
        }
    }
}