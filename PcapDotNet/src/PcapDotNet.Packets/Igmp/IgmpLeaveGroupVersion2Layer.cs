namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 2236.
    /// </summary>
    public class IgmpLeaveGroupVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.LeaveGroupVersion2; }
        }
    }
}