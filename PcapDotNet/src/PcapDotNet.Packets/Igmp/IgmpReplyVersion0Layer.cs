using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// </summary>
    public sealed class IgmpReplyVersion0Layer : IgmpVersion0Layer
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
                    case IgmpMessageType.CreateGroupReplyVersion0:
                    case IgmpMessageType.JoinGroupReplyVersion0:
                    case IgmpMessageType.LeaveGroupReplyVersion0:
                    case IgmpMessageType.ConfirmGroupReplyVersion0:
                        _type = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value", value, string.Format("Do not use {0} for {1}", GetType(), value));
                }
            }
        }

        public IgmpVersion0ReplyCode Code { get; set; }

        public byte RetryInThisManySeconds { get; set; }

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
            get { return Code == IgmpVersion0ReplyCode.RequestPendingRetryInThisManySeconds ? RetryInThisManySeconds : (byte)Code; }
        }


        protected override IpV4Address GroupAddressValue
        {
            get { return GroupAddress; }
        }

        private IgmpMessageType _type = IgmpMessageType.CreateGroupReplyVersion0;
    }
}