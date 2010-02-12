using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpAddressMaskRequestLayer : IcmpIdentifiedLayer
    {
        public IpV4Address AddressMask { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.AddressMaskRequest; }
        }

        protected override int PayloadLength
        {
            get
            {
                return IcmpAddressMaskRequestDatagram.PayloadLength;
            }
        }

        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpAddressMaskRequestDatagram.WriteHeaderAdditional(buffer, offset, AddressMask);
        }

        public bool Equals(IcmpAddressMaskRequestLayer other)
        {
            return other != null &&
                   AddressMask == other.AddressMask;
        }

        public override sealed bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpAddressMaskRequestLayer);
        }
    }
}