using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    public abstract class IgmpSimpleLayer : IgmpLayer, IIgmpLayerWithGroupAddress
    {
        public IpV4Address GroupAddress { get; set; }
        public override int Length
        {
            get { return IgmpDatagram.HeaderLength; }
        }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteHeader(buffer, offset,
                                     MessageType, MaxResponseTimeValue, GroupAddress);
        }

        public bool Equals(IgmpSimpleLayer other)
        {
            return other != null &&
                   GroupAddress == other.GroupAddress;
        }

        public override sealed bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpSimpleLayer);
        }
    }
}