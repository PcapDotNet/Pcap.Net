namespace PcapDotNet.Packets.Igmp
{
    public class IgmpReportVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion2; }
        }
    }
}