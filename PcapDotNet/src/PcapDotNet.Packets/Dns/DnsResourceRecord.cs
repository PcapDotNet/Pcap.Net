using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// All RRs have the same top level format shown below:
    /// <pre>
    /// +------+------------------------+
    /// | byte | 0-1                    |
    /// +------+------------------------+
    /// | 0    | Name                   |
    /// | ...  |                        |
    /// +------+------------------------+
    /// |      | Type                   |
    /// +------+------------------------+
    /// |      | Class                  |
    /// +------+------------------------+
    /// |      | TTL                    |
    /// |      |                        |
    /// +------+------------------------+
    /// |      | Resource Data Length   |
    /// +------+------------------------+
    /// |      | Resource Data          |
    /// | ...  |                        |
    /// +------+------------------------+
    /// </pre>
    public class DnsResourceRecord
    {
        public DnsResourceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass, int ttl, DataSegment data)
        {
            DomainName = domainName;
            Type = type;
            DnsClass = dnsClass;
            Ttl = ttl;
            Data = data;
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
        /// </summary>
        public int Ttl { get; private set; }

        public ushort DataLength { get { return (ushort)Data.Length; } }

        /// <summary>
        /// A variable length string of octets that describes the resource.  
        /// The format of this information varies according to the TYPE and CLASS of the resource record.
        /// </summary>
        public DataSegment Data  { get; private set; }
    }
}