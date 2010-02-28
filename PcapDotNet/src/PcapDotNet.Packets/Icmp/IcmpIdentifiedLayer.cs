namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Represents an ICMP layer with an Identifier and a Sequence Number.
    /// </summary>
    public abstract class IcmpIdentifiedLayer : IcmpLayer
    {
        /// <summary>
        /// An identifier to aid in matching requests and replies, may be zero.
        /// </summary>
        public ushort Identifier { get; set; }

        /// <summary>
        /// A sequence number to aid in matching requests and replies, may be zero.
        /// </summary>
        public ushort SequenceNumber { get; set; }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override sealed uint Variable
        {
            get { return (uint)((Identifier << 16) | SequenceNumber); }
        }
    }
}