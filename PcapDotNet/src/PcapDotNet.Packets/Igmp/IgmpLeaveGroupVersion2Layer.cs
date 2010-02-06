namespace PcapDotNet.Packets.Igmp
{
    public class IgmpLeaveGroupVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.LeaveGroupVersion2; }
        }
    }
}