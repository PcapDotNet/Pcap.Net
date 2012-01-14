using System;

namespace PcapDotNet.Packets.Dns
{
    public class DnsGatewayDomainName : DnsGateway, IEquatable<DnsGatewayDomainName>
    {
        public DnsGatewayDomainName(DnsDomainName value)
        {
            Value = value;
        }

        public DnsDomainName Value { get; private set; }

        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.DomainName; }
        }

        public override int Length
        {
            get { return Value.NonCompressedLength; }
        }

        public bool Equals(DnsGatewayDomainName other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayDomainName);
        }

        internal override void Write(byte[] buffer, int offset)
        {
            Value.WriteUncompressed(buffer, offset);
        }
    }
}