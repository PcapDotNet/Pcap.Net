using System;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Represents a gateway to which an IPsec tunnel may be created in order to reach the entity named by an IPsec resource record.
    /// </summary>
    public abstract class DnsGateway : IEquatable<DnsGateway>
    {
        /// <summary>
        /// An instance that represents that no gateway exists.
        /// </summary>
        public static DnsGatewayNone None { get { return _none; } }
        
        /// <summary>
        /// The gateway represnetation type.
        /// </summary>
        public abstract DnsGatewayType GatewayType { get; }

        /// <summary>
        /// The number of bytes the gateway represnetation takes.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Two gateway representations are equal if they are of the same type and the value is the same.
        /// </summary>
        public abstract bool Equals(DnsGateway other);

        /// <summary>
        /// Two gateway representations are equal if they are of the same type and the value is the same.
        /// </summary>
        public override sealed bool Equals(object obj)
        {
            return Equals(obj as DnsGateway);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>A hash code for the current gateway represnetation.</returns>
        public override sealed int GetHashCode()
        {
            return GatewayType.GetHashCode() ^ DataGetHashCode();
        }

        internal abstract int DataGetHashCode();

        internal abstract void Write(byte[] buffer, int offset);

        internal static DnsGateway CreateInstance(DnsGatewayType gatewayType, DnsDatagram dns, int offsetInDns, int length)
        {
            switch (gatewayType)
            {
                case DnsGatewayType.None:
                    return None;

                case DnsGatewayType.IpV4:
                    if (length < IpV4Address.SizeOf)
                        return null;
                    return new DnsGatewayIpV4(dns.ReadIpV4Address(offsetInDns, Endianity.Big));

                case DnsGatewayType.IpV6:
                    if (length < IpV6Address.SizeOf)
                        return null;
                    return new DnsGatewayIpV6(dns.ReadIpV6Address(offsetInDns, Endianity.Big));

                case DnsGatewayType.DomainName:
                    DnsDomainName domainName;
                    int numBytesRead;
                    if (!DnsDomainName.TryParse(dns, offsetInDns, length, out domainName, out numBytesRead))
                        return null;
                    return new DnsGatewayDomainName(domainName);

                default:
                    return null;
            }
        }

        private static DnsGatewayNone _none = new DnsGatewayNone();
    }
}