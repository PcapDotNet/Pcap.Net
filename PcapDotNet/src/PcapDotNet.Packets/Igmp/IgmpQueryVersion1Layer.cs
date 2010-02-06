namespace PcapDotNet.Packets.Igmp
{
    public class IgmpQueryVersion1Layer : IgmpVersion1Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version1;
            }
        }
    }
}