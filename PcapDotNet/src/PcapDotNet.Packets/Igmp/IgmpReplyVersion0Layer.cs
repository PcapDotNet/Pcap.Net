using System;
using System.Globalization;
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
        public override IgmpMessageType MessageTypeValue
        {
            get { return MessageType; }
        }

        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// Either CreateGroupReplyVersion0, JoinGroupReplyVersion0, LeaveGroupReplyVersion0 or ConfirmGroupReplyVersion0.
        /// </summary>
        public IgmpMessageType MessageType
        {
            get { return _messageType; }
            set
            {
                switch (value)
                {
                    case IgmpMessageType.CreateGroupReplyVersion0:
                    case IgmpMessageType.JoinGroupReplyVersion0:
                    case IgmpMessageType.LeaveGroupReplyVersion0:
                    case IgmpMessageType.ConfirmGroupReplyVersion0:
                        _messageType = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value", value, string.Format(CultureInfo.InvariantCulture, "Do not use {0} for {1}", GetType(), value));
                }
            }
        }

        /// <summary>
        /// Specifies the outcome of the request.
        /// </summary>
        public IgmpVersion0ReplyCode Code { get; set; }

        /// <summary>
        /// When Code = RequestPendingRetryInThisManySeconds, specifies the amount of seconds to retry in.
        /// </summary>
        public byte RetryInThisManySeconds { get; set; }

        /// <summary>
        /// Contains the same value as in the corresponding Request message.
        /// </summary>
        public uint Identifier { get; set; }

        /// <summary>
        /// Contains the same value as in the corresponding Request message.
        /// </summary>
        public override uint IdentifierValue
        {
            get { return Identifier; }
        }

        /// <summary>
        /// In a Create Group Reply message, the group address field contains either a newly allocated host group address (if the request is granted) 
        /// or zero (if denied).
        /// In all other Reply messages, the group address field contains the same host group address as in the corresponding Request message.
        /// </summary>
        public IpV4Address GroupAddress { get; set; }

        /// <summary>
        /// In a Create Group Reply message, the access key field contains either a non-zero 64-bit number (if the request for a private group is granted)
        /// or zero.
        /// In all other Reply messages, the access key field contains the same access key as in the corresponding Request.
        /// </summary>
        public ulong AccessKey { get; set; }

        /// <summary>
        /// In a Create Group Reply message, the access key field contains either a non-zero 64-bit number (if the request for a private group is granted)
        /// or zero.
        /// In all other Reply messages, the access key field contains the same access key as in the corresponding Request.
        /// </summary>
        public override ulong AccessKeyValue
        {
            get { return AccessKey; }
        }

        internal override byte CodeValue
        {
            get { return Code == IgmpVersion0ReplyCode.RequestPendingRetryInThisManySeconds ? RetryInThisManySeconds : (byte)Code; }
        }


        internal override IpV4Address GroupAddressValue
        {
            get { return GroupAddress; }
        }

        private IgmpMessageType _messageType = IgmpMessageType.CreateGroupReplyVersion0;
    }
}