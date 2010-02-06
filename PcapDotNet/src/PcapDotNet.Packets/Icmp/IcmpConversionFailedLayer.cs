namespace PcapDotNet.Packets.Icmp
{
    public class IcmpConversionFailedLayer : IcmpLayer
    {
        public IcmpCodeConversionFailed Code { get; set; }
        public uint Pointer { get; set; }
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ConversionFailed; }
        }

        public override byte CodeValue
        {
            get
            {
                return (byte)Code;
            }
        }

        protected override uint Value
        {
            get
            {
                return Pointer;
            }
        }
    }
}