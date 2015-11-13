using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// </summary>
    public sealed class IgmpRequestVersion0Layer : IgmpVersion0Layer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return Type; }
        }

        public IgmpMessageType Type
        {
            get { return _type; }
            set
            {
                switch (value)
                {
                    case IgmpMessageType.JoinGroupRequestVersion0:
                    case IgmpMessageType.LeaveGroupRequestVersion0:
                        _type = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value", value, string.Format("Do not use {0} for {1}", GetType(), value));
                }
            }
        }

        public uint Identifier { get; set; }

        public override uint IdentifierValue
        {
            get { return Identifier; }
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

        private IgmpMessageType _type = IgmpMessageType.JoinGroupRequestVersion0;
    }
}