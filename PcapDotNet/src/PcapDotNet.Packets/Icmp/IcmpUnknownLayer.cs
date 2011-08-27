namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Represents an ICMP layer with an unknown message type.
    /// <seealso cref="IcmpUnknownDatagram"/>
    /// </summary>
    public sealed class IcmpUnknownLayer : IcmpLayer
    {
        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public byte LayerMessageType { get; set; }

        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public byte LayerCode { get; set; }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        public uint LayerVariable { get; set; }

        /// <summary>
        /// The payload of the ICMP.
        /// All the data without the ICMP header.
        /// </summary>
        public Datagram Payload { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return (IcmpMessageType)LayerMessageType; }
        }

        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public override byte CodeValue
        {
            get { return LayerCode; }
        }

        /// <summary>
        /// The number of bytes the ICMP payload takes.
        /// </summary>
        protected override int PayloadLength
        {
            get { return Payload.Length; }
        }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override uint Variable
        {
            get { return LayerVariable; }
        }

        /// <summary>
        /// Writes the ICMP payload to the buffer.
        /// Doesn't include payload in the next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the ICMP payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing the payload at.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            Payload.Write(buffer, offset);
        }
    }
}