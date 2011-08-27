namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 1112.
    /// </summary>
    public sealed class IgmpReportVersion1Layer : IgmpVersion1Layer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion1; }
        }
    }
}