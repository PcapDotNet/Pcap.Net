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
        public override IgmpMessageType MessageTypeValue
        {
            get { return IgmpMessageType.ConfirmGroupRequestVersion0; }
        }

        /// <summary>
        /// Zero.
        /// </summary>
        public override uint IdentifierValue
        {
            get { return 0; }
        }

        /// <summary>
        /// Contains a host group address.
        /// </summary>
        public IpV4Address GroupAddress { get; set; }

        /// <summary>
        /// Contains the access key assigned to the host group identified in the Group Address field (zero for public groups).
        /// </summary>
        public ulong AccessKey { get; set; }

        /// <summary>
        /// Contains the access key assigned to the host group identified in the Group Address field (zero for public groups).
        /// </summary>
        public override ulong AccessKeyValue
        {
            get { return AccessKey; }
        }

        internal override byte CodeValue
        {
            get { return 0; }
        }

        internal override IpV4Address GroupAddressValue
        {
            get { return GroupAddress; }
        }
    }
}