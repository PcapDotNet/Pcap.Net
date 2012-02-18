using System;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Represents an IPv6 gateway to which an IPsec tunnel may be created in order to reach the entity named by an IPsec resource record.
    /// </summary>
    public class DnsGatewayIpV6 : DnsGateway, IEquatable<DnsGatewayIpV6>
    {
        /// <summary>
        /// Creates a gateway using the given IPv6 address.
        /// </summary>
        public DnsGatewayIpV6(IpV6Address value)
        {
            Value = value;
        }

        /// <summary>
        /// The IPv6 address value of the gateway.
        /// </summary>
        public IpV6Address Value { get; private set; }

        /// <summary>
        /// The gateway represnetation type.
        /// </summary>
        public override DnsGatewayType GatewayType
        {
            get { return DnsGatewayType.IpV6; }
        }

        /// <summary>
        /// The number of bytes the gateway represnetation takes.
        /// </summary>
        public override int Length
        {
            get { return IpV6Address.SizeOf; }
        }

        /// <summary>
        /// Two DnsGatewayIpV6 are equal if their IPv6 addresses are equal.
        /// </summary>
        public bool Equals(DnsGatewayIpV6 other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        /// <summary>
        /// Two gateway representations are equal if they are of the same type and the value is the same.
        /// </summary>
        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayIpV6);
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