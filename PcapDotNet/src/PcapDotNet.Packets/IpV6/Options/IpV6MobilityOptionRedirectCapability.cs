namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.RedirectCapability)]
    public sealed class IpV6MobilityOptionRedirectCapability : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ushort);

        public IpV6MobilityOptionRedirectCapability()
            : base(IpV6MobilityOptionType.RedirectCapability)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionRedirectCapability();
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return true;
        }

        internal override int GetDataHashCode()
        {
            return 0;
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += DataLength;
        }
    }
}