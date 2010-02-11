namespace PcapDotNet.Packets.Icmp
{
    public class IcmpUnknownLayer : IcmpLayer
    {
        public byte LayerMessageType { get; set; }
        public byte LayerCode { get; set; }
        public uint LayerValue { get; set; }
        public Datagram Payload { get; set; }

        public override IcmpMessageType MessageType
        {
            get
            {
                return (IcmpMessageType)LayerMessageType;
            }
        }

        protected override uint Value
        {
            get { return LayerValue; }
        }

        protected override void WritePayload(byte[] buffer, int offset)
        {
            Payload.Write(buffer, offset);
        }
    }
}