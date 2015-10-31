using System;
using System.Runtime.InteropServices;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 1112.
    /// </summary>
    public sealed class IgmpQueryVersion1Layer : IgmpVersion1Layer
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

    /// <summary>
    /// RFC 988.
    /// </summary>
    public abstract class IgmpVersion0Layer : IgmpLayer
    {
        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override sealed int Length
        {
            get { return IgmpDatagram.Version0HeaderLength; }
        }

        public abstract uint IdentifierValue { get; }

        public abstract ulong AccessKeyValue { get; }

        public override int GetHashCode()
        {
            return new[]
                   {
                       base.GetHashCode(),
                       CodeValue.GetHashCode(),
                       IdentifierValue.GetHashCode(),
                       GroupAddressValue.GetHashCode(),
                       AccessKeyValue.GetHashCode()
                   }.Xor();
        }

        protected override sealed bool EqualsVersionSpecific(IgmpLayer other)
        {
            return EqualsVersionSpecific(other as IgmpVersion0Layer);
        }

        protected abstract byte CodeValue { get; }
        protected abstract IpV4Address GroupAddressValue { get; }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteVersion0Header(buffer, offset, MessageType, CodeValue, IdentifierValue, GroupAddressValue, AccessKeyValue);
        }

        private bool EqualsVersionSpecific(IgmpVersion0Layer other)
        {
            return other != null &&
                   CodeValue == other.CodeValue &&
                   IdentifierValue == other.IdentifierValue &&
                   GroupAddressValue.Equals(other.GroupAddressValue) &&
                   AccessKeyValue == other.AccessKeyValue;
        }
    }

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