namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Represents an ICMP layer with an unknown message type.
    /// </summary>
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

        public override byte CodeValue
        {
            get
            {
                return LayerCode;
            }
        }

        protected override int PayloadLength
        {
            get
            {
                return Payload.Length;
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