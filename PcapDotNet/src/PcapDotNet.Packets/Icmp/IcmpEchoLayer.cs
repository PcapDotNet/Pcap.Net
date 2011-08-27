namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public sealed class IcmpEchoLayer : IcmpIdentifiedLayer
    {
        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Echo; }
        }
    }
}