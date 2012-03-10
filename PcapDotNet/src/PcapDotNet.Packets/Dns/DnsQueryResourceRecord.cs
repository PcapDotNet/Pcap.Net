using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
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
    /// </pre>
    /// </summary>
    public sealed class DnsQueryResourceRecord : DnsResourceRecord, IEquatable<DnsQueryResourceRecord>
    {
        /// <summary>
        /// Creates a DNS query record from a domain name, type and class.
        /// </summary>
        /// <param name="domainName">An owner name, i.e., the name of the node to which this resource record pertains.</param>
        /// <param name="type">Two octets containing one of the RR TYPE codes.</param>
        /// <param name="dnsClass">Two octets containing one of the RR CLASS codes.</param>
        public DnsQueryResourceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass) 
            : base(domainName, type, dnsClass)
        {
        }

        /// <summary>
        /// There's no TTL in a query record.
        /// </summary>
        public override int Ttl
        {
            get { throw new InvalidOperationException("No TTL in queries"); }
            protected set { throw new InvalidOperationException("No TTL in queries"); }
        }

        /// <summary>
        /// There's no data in a query record.
        /// </summary>
        public override DnsResourceData Data
        {
            get { throw new InvalidOperationException("No Resource Data in queries"); }
            protected set { throw new InvalidOperationException("No Resource Data in queries"); }
        }

        /// <summary>
        /// Two query records are equal if they have the same domain name, type and class.
        /// </summary>
        public bool Equals(DnsQueryResourceRecord other)
        {
            return EqualsBase(other);
        }

        /// <summary>
        /// Two query records are equal if they have the same domain name, type and class.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsQueryResourceRecord);
        }

        /// <summary>
        /// A hash code based on the query record's domain name, type and class.
        /// </summary>
        public override int GetHashCode()
        {
            return GetHashCodeBase();
        }

        internal static DnsQueryResourceRecord Parse(DnsDatagram dns, int offsetInDns, out int numBytesRead)
        {
            DnsDomainName domainName;
            DnsType type;
            DnsClass dnsClass;
            if (!TryParseBase(dns, offsetInDns, out domainName, out type, out dnsClass, out numBytesRead))
                return null;

            return new DnsQueryResourceRecord(domainName, type, dnsClass);
        }

        internal override int GetLengthAfterBase(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return 0;
        }
    }
}