namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792 and RFC 1191.
    /// </summary>
    public class IcmpDestinationUnreachableLayer : IcmpLayer
    {
        public IcmpCodeDestinationUnreachable Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DestinationUnreachable; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }

        public ushort NextHopMaximumTransmissionUnit { get; set; }

        public bool Equals(IcmpDestinationUnreachableLayer other)
        {
            return other != null &&
                   NextHopMaximumTransmissionUnit == other.NextHopMaximumTransmissionUnit;
        }

        public override sealed bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpDestinationUnreachableLayer);
        }

        protected override uint Value
        {
            get { return NextHopMaximumTransmissionUnit; }
        }
    }
}