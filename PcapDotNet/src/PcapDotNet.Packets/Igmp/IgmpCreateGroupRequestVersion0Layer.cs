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
        public override IgmpMessageType MessageTypeValue
        {
            get { return IgmpMessageType.CreateGroupRequestVersion0; }
        }

        /// <summary>
        /// Indicates if the new host group is to be private or public.
        /// </summary>
        public IgmpVersion0CreateGroupRequestCode CreateGroupRequestCode { get; set; }

        /// <summary>
        /// Contains a value to distinguish the request from other requests by the same host.
        /// </summary>
        public uint Identifier { get; set; }

        /// <summary>
        /// Contains a value to distinguish the request from other requests by the same host.
        /// </summary>
        public override uint IdentifierValue
        {
            get { return Identifier; }
        }

        /// <summary>
        /// Zero.
        /// </summary>
        public override ulong AccessKeyValue
        {
            get { return 0; }
        }

        internal override byte CodeValue
        {
            get { return (byte)CreateGroupRequestCode; }
        }


        internal override IpV4Address GroupAddressValue
        {
            get { return IpV4Address.Zero; }
        }
    }
}