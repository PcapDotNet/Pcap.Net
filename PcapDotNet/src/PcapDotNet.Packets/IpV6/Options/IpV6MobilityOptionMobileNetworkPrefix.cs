namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 3963.
    /// <pre>
    /// +-----+--------------+---------------+
    /// | Bit | 0-7          | 8-15          |
    /// +-----+--------------+---------------+
    /// | 0   | Option Type  | Opt Data Len  |
    /// +-----+--------------+---------------+
    /// | 16  | Reserved     | Prefix Length |
    /// +-----+--------------+---------------+
    /// | 32  | Mobile Network Prefix        |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNetworkPrefix)]
    public sealed class IpV6MobilityOptionMobileNetworkPrefix : IpV6MobilityOptionNetworkPrefix
    {
        /// <summary>
        /// Create an instance from prefix length and mobile network prefix.
        /// </summary>
        /// <param name="prefixLength">Indicates the prefix length of the IPv6 prefix contained in the option.</param>
        /// <param name="mobileNetworkPrefix">Contains the Mobile Network Prefix.</param>
        public IpV6MobilityOptionMobileNetworkPrefix(byte prefixLength, IpV6Address mobileNetworkPrefix)
            : base(IpV6MobilityOptionType.MobileNetworkPrefix, prefixLength, mobileNetworkPrefix)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte prefixLength;
            IpV6Address mobileNetworkPrefix;
            if (!Read(data, out prefixLength, out mobileNetworkPrefix))
                return null;

            return new IpV6MobilityOptionMobileNetworkPrefix(prefixLength, mobileNetworkPrefix);
        }

        private IpV6MobilityOptionMobileNetworkPrefix()
            : this(0, IpV6Address.Zero)
        {
        }
    }
}