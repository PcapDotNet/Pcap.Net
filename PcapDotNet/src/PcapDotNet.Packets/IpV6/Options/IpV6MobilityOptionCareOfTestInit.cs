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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CareOfTestInit)]
    public sealed class IpV6MobilityOptionCareOfTestInit : IpV6MobilityOptionEmpty
    {
        public IpV6MobilityOptionCareOfTestInit()
            : base(IpV6MobilityOptionType.CareOfTestInit)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionCareOfTestInit();
        }
    }
}