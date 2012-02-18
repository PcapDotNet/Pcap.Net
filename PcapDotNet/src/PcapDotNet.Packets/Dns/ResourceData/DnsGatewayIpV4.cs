using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Represents an IPv4 gateway to which an IPsec tunnel may be created in order to reach the entity named by an IPsec resource record.
    /// </summary>
    public class DnsGatewayIpV4 : DnsGateway, IEquatable<DnsGatewayIpV4>
    {
        /// <summary>
        /// Creates a gateway using the given IPv4 address.
        /// </summary>
        public DnsGatewayIpV4(IpV4Address value)
        {
            Value = value;
        }

        /// <summary>
        /// The IPv4 address value of the gateway.
        /// </summary>
        public IpV4Address Value { get; private set; }

        /// <summary>
        /// The gateway represnetation type.
        /// </summary>
        public override DnsGatewayType GatewayType
        {
            get { return DnsGatewayType.IpV4; }
        }

        /// <summary>
        /// The number of bytes the gateway represnetation takes.
        /// </summary>
        public override int Length
        {
            get { return IpV4Address.SizeOf; }
        }

        /// <summary>
        /// Two DnsGatewayIpV4 are equal if their IPv4 addresses are equal.
        /// </summary>
        public bool Equals(DnsGatewayIpV4 other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        /// <summary>
        /// Two gateway representations are equal if they are of the same type and the value is the same.
        /// </summary>
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