namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// </summary>
    public sealed class IpV6FlowIdentificationSubOptionPad1 : IpV6FlowIdentificationSubOptionSimple
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = sizeof(byte);

        /// <summary>
        /// Constructs the option.
        /// </summary>
        public IpV6FlowIdentificationSubOptionPad1()
            : base(IpV6FlowIdentificationSubOptionType.Pad1)
        {
        }
    }
}