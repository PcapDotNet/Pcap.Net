using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// Represents an ICMP Timestamp layer.
    /// <seealso cref="IcmpTimestampDatagram"/>
    /// </summary>
    public class IcmpTimestampLayer : IcmpIdentifiedLayer
    {
        /// <summary>
        /// The time the sender last touched the message before sending it.
        /// </summary>
        public IpV4TimeOfDay OriginateTimestamp { get; set; }

        /// <summary>
        /// The time the echoer first touched it on receipt.
        /// </summary>
        public IpV4TimeOfDay ReceiveTimestamp { get; set; }

        /// <summary>
        /// The time the echoer last touched the message on sending it.
        /// </summary>
        public IpV4TimeOfDay TransmitTimestamp { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Timestamp; }
        }

        /// <summary>
        /// The number of bytes the ICMP payload takes.
        /// </summary>
        protected override int PayloadLength
        {
            get { return IcmpTimestampDatagram.PayloadLength; }
        }

        /// <summary>
        /// Writes the ICMP payload to the buffer.
        /// Doesn't include payload in the next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the ICMP payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing the payload at.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpTimestampDatagram.WriteHeaderAdditional(buffer, offset,
                                                        OriginateTimestamp, ReceiveTimestamp, TransmitTimestamp);
        }

        public bool Equals(IcmpTimestampLayer other)
        {
            return other != null &&
                   OriginateTimestamp == other.OriginateTimestamp &&
                   ReceiveTimestamp == other.ReceiveTimestamp &&
                   TransmitTimestamp == other.TransmitTimestamp;
        }

        public sealed override bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpTimestampLayer);
        }
    }
}