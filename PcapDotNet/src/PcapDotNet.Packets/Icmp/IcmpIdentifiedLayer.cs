namespace PcapDotNet.Packets.Icmp
{
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