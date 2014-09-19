using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | IPv4 Care-of address       |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4CareOfAddress)]
    public sealed class IpV6MobilityOptionIpV4CareOfAddress : IpV6MobilityOptionIpV4Address
    {
        /// <summary>
        /// Creates an instance from a care-of address.
        /// </summary>
        /// <param name="careOfAddress">
        /// Contains the mobile node's IPv4 care-of address.
        /// The IPv4 care-of address is used when the mobile node is located in an IPv4-only network.
        /// </param>
        public IpV6MobilityOptionIpV4CareOfAddress(IpV4Address careOfAddress)
            : base(IpV6MobilityOptionType.IpV4CareOfAddress, careOfAddress)
        {
        }

        /// <summary>
        /// Contains the mobile node's IPv4 care-of address.
        /// The IPv4 care-of address is used when the mobile node is located in an IPv4-only network.
        /// </summary>
        public IpV4Address CareOfAddress { get { return Address; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV4Address careOfAddress;
            if (!Read(data, out careOfAddress))
                return null;

            return new IpV6MobilityOptionIpV4CareOfAddress(careOfAddress);
        }

        private IpV6MobilityOptionIpV4CareOfAddress()
            : this(IpV4Address.Zero)
        {
        }
    }
}