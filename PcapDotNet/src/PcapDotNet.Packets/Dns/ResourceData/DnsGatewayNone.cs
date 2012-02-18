using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A gateway representation that represents that no gateway is present.
    /// </summary>
    public class DnsGatewayNone : DnsGateway, IEquatable<DnsGatewayNone>
    {
        /// <summary>
        /// The gateway represnetation type.
        /// </summary>
        public override DnsGatewayType GatewayType
        {
            get { return DnsGatewayType.None; }
        }

        /// <summary>
        /// The number of bytes the gateway represnetation takes.
        /// </summary>
        public override int Length
        {
            get { return 0; }
        }

        /// <summary>
        /// Two DnsGatewayNone are always equal.
        /// </summary>
        public bool Equals(DnsGatewayNone other)
        {
            return other != null;
        }

        /// <summary>
        /// Two gateway representations are equal if they are of the same type and the value is the same.
        /// </summary>
        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayNone);
        }

        internal override int DataGetHashCode()
        {
            return 0;
        }

        internal DnsGatewayNone()
        {
        }

        internal override void Write(byte[] buffer, int offset)
        {
        }
    }
}