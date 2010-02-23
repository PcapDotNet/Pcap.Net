namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 2236.
    /// </summary>
    public class IgmpQueryVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version2;
            }
        }
    }
}