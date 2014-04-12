namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Link-local Address          |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LinkLocalAddress)]
    public sealed class IpV6MobilityOptionLinkLocalAddress : IpV6MobilityOptionIpV6Address
    {
        public IpV6MobilityOptionLinkLocalAddress(IpV6Address linkLocalAddress)
            : base(IpV6MobilityOptionType.LinkLocalAddress, linkLocalAddress)
        {
        }

        /// <summary>
        /// Contains the link-local address.
        /// </summary>
        public IpV6Address LinkLocalAddress
        {
            get { return Address; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV6Address linkLocalAddress;
            if (!Read(data, out linkLocalAddress))
                return null;

            return new IpV6MobilityOptionLinkLocalAddress(linkLocalAddress);
        }

        private IpV6MobilityOptionLinkLocalAddress()
            : this(IpV6Address.Zero)
        {
        }
    }
}