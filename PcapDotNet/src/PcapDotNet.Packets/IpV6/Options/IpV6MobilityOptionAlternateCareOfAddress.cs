namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Alternate Care-of Address   |
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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AlternateCareOfAddress)]
    public sealed class IpV6MobilityOptionAlternateCareOfAddress : IpV6MobilityOptionIpV6Address
    {
        /// <summary>
        /// Creates an instance from an alternative care of address.
        /// </summary>
        /// <param name="alternateCareOfAddress">
        /// Contains an address to use as the care-of address for the binding, rather than using the Source Address of the packet as the care-of address.
        /// </param>
        public IpV6MobilityOptionAlternateCareOfAddress(IpV6Address alternateCareOfAddress)
            : base(IpV6MobilityOptionType.AlternateCareOfAddress, alternateCareOfAddress)
        {
        }

        /// <summary>
        /// Contains an address to use as the care-of address for the binding, rather than using the Source Address of the packet as the care-of address.
        /// </summary>
        public IpV6Address AlternateCareOfAddress
        {
            get { return Address; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV6Address alternateCareOfAddress;
            if (!Read(data, out alternateCareOfAddress))
                return null;

            return new IpV6MobilityOptionAlternateCareOfAddress(alternateCareOfAddress);
        }

        private IpV6MobilityOptionAlternateCareOfAddress()
            : this(IpV6Address.Zero)
        {
        }
    }
}