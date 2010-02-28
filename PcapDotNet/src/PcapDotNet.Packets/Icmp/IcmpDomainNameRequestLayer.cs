namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1788.
    /// </summary>
    public class IcmpDomainNameRequestLayer : IcmpIdentifiedLayer
    {
        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DomainNameRequest; }
        }
    }
}