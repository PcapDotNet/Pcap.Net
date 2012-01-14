using System;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets.Dns
{
    public class DnsGatewayIpV6 : DnsGateway, IEquatable<DnsGatewayIpV6>
    {
        public DnsGatewayIpV6(IpV6Address value)
        {
            Value = value;
        }

        public IpV6Address Value { get; private set; }

        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.IpV6; }
        }

        public override int Length
        {
            get { return IpV6Address.SizeOf; }
        }

        public bool Equals(DnsGatewayIpV6 other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayIpV6);
        }

        internal override void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, Value, Endianity.Big);
        }
    }
}