namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 1112.
    /// </summary>
    public class IgmpReportVersion1Layer : IgmpVersion1Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion1; }
        }
    }
}