namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CryptographicallyGeneratedAddressParametersRequest)]
    public sealed class IpV6MobilityOptionCryptographicallyGeneratedAddressParametersRequest : IpV6MobilityOptionEmpty
    {
        /// <summary>
        /// Creates an instance.
        /// </summary>
        public IpV6MobilityOptionCryptographicallyGeneratedAddressParametersRequest()
            : base(IpV6MobilityOptionType.CryptographicallyGeneratedAddressParametersRequest)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionCryptographicallyGeneratedAddressParametersRequest();
        }
    }
}