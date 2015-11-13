using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// </summary>
    public sealed class IgmpCreateGroupRequestVersion0Layer : IgmpVersion0Layer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.CreateGroupRequestVersion0; }
        }

        public bool IsPrivate { get; set; }
        public uint Identifier { get; set; }

        public override uint IdentifierValue
        {
            get { return Identifier; }
        }

        public override ulong AccessKeyValue
        {
            get { return 0; }
        }

        protected override byte CodeValue
        {
            get { return IsPrivate.ToByte(); }
        }


        protected override IpV4Address GroupAddressValue
        {
            get { return IpV4Address.Zero; }
        }
    }
}