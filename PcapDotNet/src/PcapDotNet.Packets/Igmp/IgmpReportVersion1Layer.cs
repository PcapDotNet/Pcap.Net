namespace PcapDotNet.Packets.Igmp
{
    public class IgmpReportVersion1Layer : IgmpVersion1Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion1; }
        }
    }
}