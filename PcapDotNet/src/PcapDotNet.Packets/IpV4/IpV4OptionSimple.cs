namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// A simple IPv4 option - holds only the type.
    /// </summary>
    public sealed class IpV4OptionSimple : IpV4Option
    {
        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public const int OptionLength = 1;

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }

        internal IpV4OptionSimple(IpV4OptionType optionType)
            : base(optionType)
        {
        }
    }
}