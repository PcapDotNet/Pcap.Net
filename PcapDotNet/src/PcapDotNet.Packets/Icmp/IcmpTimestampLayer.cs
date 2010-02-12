using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTimestampLayer : IcmpIdentifiedLayer
    {
        public IpV4TimeOfDay OriginateTimestamp { get; set; }
        public IpV4TimeOfDay ReceiveTimestamp { get; set; }
        public IpV4TimeOfDay TransmitTimestamp { get; set; }
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Timestamp; }
        }

        protected override int PayloadLength
        {
            get
            {
                return IcmpTimestampDatagram.PayloadLength;
            }
        }

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