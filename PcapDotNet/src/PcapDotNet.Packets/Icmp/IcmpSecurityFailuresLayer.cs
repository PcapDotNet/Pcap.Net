namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 2521.
    /// </summary>
    public class IcmpSecurityFailuresLayer : IcmpLayer
    {
        public IcmpCodeSecurityFailure Code{get; set ;}
        public ushort Pointer{get; set ;}
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.SecurityFailures; }
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