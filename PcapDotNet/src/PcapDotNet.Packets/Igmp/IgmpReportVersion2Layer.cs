namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 2236.
    /// </summary>
    public class IgmpReportVersion2Layer : IgmpVersion2Layer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion2; }
        }
    }
}