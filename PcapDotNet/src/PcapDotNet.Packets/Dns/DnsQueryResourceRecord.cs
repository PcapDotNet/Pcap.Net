using System;

namespace PcapDotNet.Packets.Dns
{
    public sealed class DnsQueryResourceRecord : DnsResourceRecord, IEquatable<DnsQueryResourceRecord>
    {
        public DnsQueryResourceRecord(DnsDomainName domainName, DnsType type, DnsClass dnsClass) 
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

        public bool Equals(DnsQueryResourceRecord other)
        {
            return EqualsBase(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsQueryResourceRecord);
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