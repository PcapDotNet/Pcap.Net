namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// +-----+-----+
    /// | Bit | 0-7 |
    /// +-----+-----+
    /// | 0   | 0   |
    /// +-----+-----+
    /// </summary>
    public class IpV6OptionPad1 : IpV6OptionSimple
    {
        public const int OptionLength = sizeof(byte);

        public IpV6OptionPad1()
            : base(IpV6OptionType.Pad1)
        {
        }
    }
}