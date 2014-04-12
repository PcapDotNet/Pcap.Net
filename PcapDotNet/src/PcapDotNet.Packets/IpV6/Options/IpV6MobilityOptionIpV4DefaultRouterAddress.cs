using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | IPv4 home address          |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4DefaultRouterAddress)]
    public class IpV6MobilityOptionIpV4DefaultRouterAddress : IpV6MobilityOptionIpV4Address
    {
        public IpV6MobilityOptionIpV4DefaultRouterAddress(IpV4Address defaultRouterAddress)
            : base(IpV6MobilityOptionType.IpV4DefaultRouterAddress, defaultRouterAddress)
        {
        }

        /// <summary>
        /// The mobile node's default router address.
        /// </summary>
        public IpV4Address DefaultRouterAddress { get { return Address; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV4Address defaultRouterAddress;
            if (!Read(data, out defaultRouterAddress))
                return null;

            return new IpV6MobilityOptionIpV4DefaultRouterAddress(defaultRouterAddress);
        }

        private IpV6MobilityOptionIpV4DefaultRouterAddress()
            : this(IpV4Address.Zero)
        {
        }
    }
}