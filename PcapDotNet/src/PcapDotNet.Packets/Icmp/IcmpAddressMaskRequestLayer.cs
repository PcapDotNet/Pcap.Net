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

        public override int Length
        {
            get
            {
                return base.Length + IcmpAddressMaskDatagram.HeaderAdditionalLength;
            }
        }

        protected override void WriteHeaderAdditional(byte[] buffer, int offset)
        {
            IcmpAddressMaskDatagram.WriteHeaderAdditional(buffer, offset, AddressMask);
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