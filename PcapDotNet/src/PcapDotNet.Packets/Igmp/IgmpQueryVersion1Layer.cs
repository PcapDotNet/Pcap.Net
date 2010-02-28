namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 1112.
    /// </summary>
    public class IgmpQueryVersion1Layer : IgmpVersion1Layer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        /// <summary>
        /// The IGMP version of a Membership Query message.
        /// If the type is not a query, None will be returned.
        /// </summary>
        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version1;
            }
        }
    }
}