using System;
using System.Globalization;
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
        public override IgmpMessageType MessageTypeValue
        {
            get { return MessageType; }
        }

        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// Either JoinGroupRequestVersion0 or LeaveGroupRequestVersion0.
        /// </summary>
        public IgmpMessageType MessageType
        {
            get { return _messageType; }
            set
            {
                switch (value)
                {
                    case IgmpMessageType.JoinGroupRequestVersion0:
                    case IgmpMessageType.LeaveGroupRequestVersion0:
                        _messageType = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value", value, string.Format(CultureInfo.InvariantCulture, "Do not use {0} for {1}", GetType(), value));
                }
            }
        }

        /// <summary>
        /// In a Confirm Group Request message, the identifier field should contain zero.
        /// In all other Request messages, the identifier field contains a value to distinguish the request from other requests by the same host.
        /// </summary>
        public uint Identifier { get; set; }

        /// <summary>
        /// In a Confirm Group Request message, the identifier field contains zero.
        /// In all other Request messages, the identifier field contains a value to distinguish the request from other requests by the same host.
        /// </summary>
        public override uint IdentifierValue
        {
            get { return Identifier; }
        }

        /// <summary>
        /// In a Create Group Request message, the group address field contains zero.
        /// In all other Request messages, the group address field contains a host group address.
        /// </summary>
        public IpV4Address GroupAddress { get; set; }

        /// <summary>
        /// In a Create Group Request message, the access key field contains zero.
        /// In all other Request messages, the access key field contains the access key assigned to the host group identified in the Group Address field
        /// (zero for public groups).
        /// </summary>
        public ulong AccessKey { get; set; }

        /// <summary>
        /// In a Create Group Request message, the access key field contains zero.
        /// In all other Request messages, the access key field contains the access key assigned to the host group identified in the Group Address field
        /// (zero for public groups).
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

        private IgmpMessageType _messageType = IgmpMessageType.JoinGroupRequestVersion0;
    }
}