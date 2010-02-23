namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public class IcmpInformationRequestLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.InformationRequest; }
        }
    }
}