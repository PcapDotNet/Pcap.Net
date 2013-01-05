namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// +-----+-----+
    /// | Bit | 0-7 |
    /// +-----+-----+
    /// | 0   | 0   |
    /// +-----+-----+
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.Pad1)]
    public class IpV6OptionPad1 : IpV6OptionSimple
    {
        public const int OptionLength = sizeof(byte);

        public IpV6OptionPad1()
            : base(IpV6OptionType.Pad1)
        {
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            return new IpV6OptionPad1();
        }
    }

    /// <summary>
    /// RFC 6275.
    /// +-----+-----+
    /// | Bit | 0-7 |
    /// +-----+-----+
    /// | 0   | 0   |
    /// +-----+-----+
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Pad1)]
    public class IpV6MobilityOptionPad1 : IpV6MobilityOption
    {
        public const int OptionLength = sizeof(byte);

        public IpV6MobilityOptionPad1()
            : base(IpV6MobilityOptionType.Pad1)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionPad1();
        }
    }
}