namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public class IcmpParameterProblemLayer : IcmpLayer
    {
        public byte Pointer { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ParameterProblem; }
        }

        protected override uint Value
        {
            get
            {
                return (uint)(Pointer << 24);
            }
        }
    }
}