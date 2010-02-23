namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public class IcmpTimeExceededLayer : IcmpLayer
    {
        public IcmpCodeTimeExceeded Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.TimeExceeded; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }
    }
}