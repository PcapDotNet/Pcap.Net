using System;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets.Dns
{
    public abstract class DnsGateway : IEquatable<DnsGateway>
    {
        public static DnsGatewayNone None { get { return _none; } }

        public abstract DnsGatewayType Type { get; }

        public abstract int Length { get; }

        public abstract bool Equals(DnsGateway other);

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsGateway);
        }

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