using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// </summary>
    public sealed class IgmpConfirmGroupRequestVersion0Layer : IgmpVersion0Layer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.ConfirmGroupRequestVersion0; }
        }

        public override uint IdentifierValue
        {
            get { return 0; }
        }

        public IpV4Address GroupAddress { get; set; }

        public ulong AccessKey { get; set; }

        public override ulong AccessKeyValue
        {
            get { return AccessKey; }
        }

        protected override byte CodeValue
        {
            get { return 0; }
        }

        protected override IpV4Address GroupAddressValue
        {
            get { return GroupAddress; }
        }
    }
}