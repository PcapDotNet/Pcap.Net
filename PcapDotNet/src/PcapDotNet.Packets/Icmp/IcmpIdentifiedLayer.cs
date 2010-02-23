namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Represents an ICMP layer with an Identifier and a Sequence Number.
    /// </summary>
    public abstract class IcmpIdentifiedLayer : IcmpLayer
    {
        public ushort Identifier { get; set; }

        public ushort SequenceNumber { get; set; }

        protected override sealed uint Value
        {
            get
            {
                return (uint)((Identifier << 16) | SequenceNumber);
            }
        }
    }
}