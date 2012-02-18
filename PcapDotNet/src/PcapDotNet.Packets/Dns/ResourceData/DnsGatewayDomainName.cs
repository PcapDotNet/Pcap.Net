using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A gateway that is represented using a domain name.
    /// </summary>
    public class DnsGatewayDomainName : DnsGateway, IEquatable<DnsGatewayDomainName>
    {
        /// <summary>
        /// Creates the gateway using the given domain name.
        /// </summary>
        public DnsGatewayDomainName(DnsDomainName value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns the domain name value.
        /// </summary>
        public DnsDomainName Value { get; private set; }

        /// <summary>
        /// The gateway represnetation type.
        /// </summary>
        public override DnsGatewayType GatewayType
        {
            get { return DnsGatewayType.DomainName; }
        }

        /// <summary>
        /// The number of bytes the gateway represnetation takes.
        /// </summary>
        public override int Length
        {
            get { return Value.NonCompressedLength; }
        }

        /// <summary>
        /// Two DnsGatewayDomainName are equal if their domain name values are equal.
        /// </summary>
        public bool Equals(DnsGatewayDomainName other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        /// <summary>
        /// Two gateway representations are equal if they are of the same type and the value is the same.
        /// </summary>
        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayDomainName);
        }

        internal override int DataGetHashCode()
        {
            return Value.GetHashCode();
        }

        internal override void Write(byte[] buffer, int offset)
        {
            Value.WriteUncompressed(buffer, offset);
        }
    }
}