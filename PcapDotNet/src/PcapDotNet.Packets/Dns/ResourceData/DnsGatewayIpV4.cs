using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dns
{
    public class DnsGatewayIpV4 : DnsGateway, IEquatable<DnsGatewayIpV4>
    {
        public DnsGatewayIpV4(IpV4Address value)
        {
            Value = value;
        }

        public IpV4Address Value { get; private set; }

        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.IpV4; }
        }

        public override int Length
        {
            get { return IpV4Address.SizeOf; }
        }

        public bool Equals(DnsGatewayIpV4 other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayIpV4);
        }

        internal override int DataGetHashCode()
        {
            return Value.GetHashCode();
        }

        internal override void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, Value, Endianity.Big);
        }
    }
}