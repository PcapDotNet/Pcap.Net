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
        /// <summary>
        /// The number of bytes the option takes.
        /// </summary>
        public const int OptionLength = sizeof(byte);

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public IpV6OptionPad1()
            : base(IpV6OptionType.Pad1)
        {
        }
    }
}