namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// A simple TCP option - holds only the type.
    /// </summary>
    public sealed class TcpOptionSimple : TcpOption
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

        internal TcpOptionSimple(TcpOptionType optionType)
            : base(optionType)
        {
        }
    }
}