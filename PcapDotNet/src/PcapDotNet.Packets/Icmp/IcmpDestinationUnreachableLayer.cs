namespace PcapDotNet.Packets.Icmp
{
    public class IcmpDestinationUnreachableLayer : IcmpLayer
    {
        public IcmpCodeDestinationUnrechable Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DestinationUnreachable; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }

        public ushort NextHopMtu { get; set; }

        public bool Equals(IcmpDestinationUnreachableLayer other)
        {
            return other != null &&
                   NextHopMtu == other.NextHopMtu;
        }

        public override sealed bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpDestinationUnreachableLayer);
        }

        protected override uint Value
        {
            get { return NextHopMtu; }
        }
    }
}