using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+-------------------------+-----------------+-------+----+
    /// | Bit | 0-7         | 8-15                    | 16-28           | 29-30 | 31 |
    /// +-----+-------------+-------------------------+-----------------+-------+----+
    /// | 0   | Next Header | Header Extension Length | Fragment Offset | Res   | M  |
    /// +-----+-------------+-------------------------+-----------------+-------+----+
    /// | 32  | Identification                                                       |
    /// +-----+----------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderFragmentData : IpV6ExtensionHeaderStandard
    {
        private static class DataOffset
        {
            public const int FragmentOffset = 0;
            public const int MoreFragments = FragmentOffset + sizeof(byte);
            public const int Identification = MoreFragments + sizeof(byte);
        }

        private static class DataMask
        {
            public const ushort FragmentOffset = 0xFFF8;
            public const byte MoreFragments = 0x01;
        }

        private static class DataShift
        {
            public const int FragmentOffset = 3;
        }

        public const int ExtensionHeaderDataLength = DataOffset.Identification + sizeof(uint);

        public const ushort MaxFragmentOffset = 0x1FFF;

        public IpV6ExtensionHeaderFragmentData(IpV4Protocol nextHeader, ushort fragmentOffset, bool moreFragments, uint identification)
            : base(nextHeader)
        {
            if (fragmentOffset > MaxFragmentOffset)
                throw new ArgumentOutOfRangeException("fragmentOffset", fragmentOffset, "Max value is " + MaxFragmentOffset);
            FragmentOffset = fragmentOffset;
            MoreFragments = moreFragments;
            Identification = identification;
        }

        /// <summary>
        /// The offset, in 8-octet units, of the data following this header, relative to the start of the Fragmentable Part of the original packet.
        /// </summary>
        public ushort FragmentOffset { get; private set; }

        /// <summary>
        /// True - more fragments.
        /// False - last fragment.
        /// </summary>
        public bool MoreFragments { get; private set; }

        /// <summary>
        /// For every packet that is to be fragmented, the source node generates an Identification value. 
        /// The Identification must be different than that of any other fragmented packet sent recently with the same Source Address and Destination Address.
        /// If a Routing header is present, the Destination Address of concern is that of the final destination.
        /// </summary>
        public uint Identification { get; private set; }

        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.FragmentHeaderForIpV6; }
        }

        public override bool IsValid
        {
            get { return true; }
        }

        internal override int DataLength
        {
            get { return ExtensionHeaderDataLength; }
        }

        internal static IpV6ExtensionHeaderFragmentData ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            if (data.Length != ExtensionHeaderDataLength)
                return null;

            ushort fragmentOffset = (ushort)((data.ReadUShort(DataOffset.FragmentOffset, Endianity.Big) & DataMask.FragmentOffset) >> DataShift.FragmentOffset);
            bool moreFragments = data.ReadBool(DataOffset.MoreFragments, DataMask.MoreFragments);
            uint identification = data.ReadUInt(DataOffset.Identification, Endianity.Big);

            return new IpV6ExtensionHeaderFragmentData(nextHeader, fragmentOffset, moreFragments, identification);
        }

        internal override bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderFragmentData);
        }

        internal override void WriteData(byte[] buffer, int offset)
        {
            ushort fragmentOffsetAndMoreFragments = (ushort)(FragmentOffset << DataShift.FragmentOffset);
            if (MoreFragments)
                fragmentOffsetAndMoreFragments |= DataMask.MoreFragments;

            buffer.Write(offset + DataOffset.FragmentOffset, fragmentOffsetAndMoreFragments, Endianity.Big);
            buffer.Write(offset + DataOffset.Identification, Identification, Endianity.Big);
        }

        private bool EqualsData(IpV6ExtensionHeaderFragmentData other)
        {
            return other != null &&
                   FragmentOffset == other.FragmentOffset && MoreFragments == other.MoreFragments && Identification == other.Identification;
        }
    }

    public enum IpV6MobilityHeaderType : byte
    {
        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingRefreshRequest = 0,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeTestInit = 1,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        CareOfTestInit = 2,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeTest = 3,

        /// <summary>
        /// RFC6275
        /// </summary>
        CareOfTest = 4,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingUpdate = 5,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingAcknowledgement = 6,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingError = 7,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        FastBindingUpdate = 8,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        FastBindingAcknowledgement = 9,

        /// <summary>
        /// RFCs 4068, 5568.
        /// Deprecated.
        /// </summary>
        FastNeighborAdvertisement = 10,

        /// <summary>
        /// RFC 5096.
        /// </summary>
        Experimental = 11,

        /// <summary>
        /// RFC 5142.
        /// </summary>
        HomeAgentSwitchMessage = 12,

        /// <summary>
        /// RFC 5847.
        /// </summary>
        HeartbeatMessage = 13,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        HandoverInitiateMessage = 14,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        HandoverAcknowledgeMessage = 15,

        /// <summary>
        /// RFC 5846.
        /// </summary>
        BindingRevocationMessage = 16,

        /// <summary>
        /// RFC-ietf-netext-pmip-lr-10.
        /// </summary>
        LocalizedRoutingInitiation = 17,

        /// <summary>
        /// RFC-ietf-netext-pmip-lr-10.
        /// </summary>
        LocalizedRoutingAcknowledgement = 18,
    }

    /// <summary>
    /// RFC 3775, 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Message Data                          |
    /// | ... | ends with Mobility Options            |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobility : IpV6ExtensionHeaderStandard
    {
        public override sealed IpV4Protocol Protocol
        {
            get { return IpV4Protocol.MobilityHeader; }
        }

        public override sealed bool IsValid
        {
            get { return MobilityOptions.IsValid && Length % 8 == 0; }
        }

        private static class DataOffset
        {
            public const int MobilityHeaderType = 0;
            public const int Checksum = MobilityHeaderType + sizeof(byte) + sizeof(byte);
            public const int MessageData = Checksum + sizeof(ushort);
        }

        public const int MinimumDataLength = DataOffset.MessageData;

        public IpV6ExtensionHeaderMobility(IpV4Protocol nextHeader, ushort checksum, IpV6MobilityOptions mobilityOptions)
            : base(nextHeader)
        {
            Checksum = checksum;
            MobilityOptions = mobilityOptions;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public abstract IpV6MobilityHeaderType MobilityHeaderType { get; }

        /// <summary>
        /// <para>
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </para>
        /// <para>
        /// The pseudo-header contains IPv6 header fields.
        /// The Next Header value used in the pseudo-header is 135.
        /// The addresses used in the pseudo-header are the addresses that appear in the Source and Destination Address fields in the IPv6 packet 
        /// carrying the Mobility Header.
        /// </para>
        /// <para>
        /// Note that the procedures of calculating upper-layer checksums while away from home apply even for the Mobility Header.
        /// If a mobility message has a Home Address destination option, then the checksum calculation uses the home address in this option as the value of the IPv6 Source Address field.
        /// </para>
        /// <para>
        /// The Mobility Header is considered as the upper-layer protocol for the purposes of calculating the pseudo-header.
        /// The Upper-Layer Packet Length field in the pseudo-header MUST be set to the total length of the Mobility Header.
        /// </para>
        /// <para>
        /// For computing the checksum, the checksum field is set to zero.
        /// </para>
        /// </summary>
        public ushort Checksum { get; private set; }

        /// <summary>
        /// Zero or more TLV-encoded mobility options.
        /// </summary>
        public IpV6MobilityOptions MobilityOptions { get; private set; }

        internal static IpV6ExtensionHeaderMobility ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            if (data.Length < MinimumDataLength)
                return null;

            IpV6MobilityHeaderType mobilityHeaderType = (IpV6MobilityHeaderType)data[DataOffset.MobilityHeaderType];
            ushort checksum = data.ReadUShort(DataOffset.Checksum, Endianity.Big);
            DataSegment messageData = data.Subsegment(DataOffset.MessageData, data.Length - DataOffset.MessageData);

            switch (mobilityHeaderType)
            {
                case IpV6MobilityHeaderType.BindingRefreshRequest: // 0
                    return IpV6ExtensionHeaderMobilityBindingRefreshRequest.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HomeTestInit: // 1
                    return IpV6ExtensionHeaderMobilityHomeTestInit.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.CareOfTestInit: // 2
                    return IpV6ExtensionHeaderMobilityCareOfTestInit.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HomeTest: // 3
                    return IpV6ExtensionHeaderMobilityHomeTest.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.CareOfTest: // 4
                    return IpV6ExtensionHeaderMobilityCareOfTest.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingUpdate: // 5
                    return IpV6ExtensionHeaderMobilityBindingUpdate.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingAcknowledgement: // 6
                    return IpV6ExtensionHeaderMobilityBindingAcknowledgement.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingError: // 7
                    return IpV6ExtensionHeaderMobilityBindingError.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.FastBindingUpdate: // 8
                    return IpV6ExtensionHeaderMobilityFastBindingUpdate.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.FastBindingAcknowledgement: // 9
                    return IpV6ExtensionHeaderMobilityFastBindingAcknowledgement.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.FastNeighborAdvertisement: // 10
                    return IpV6ExtensionHeaderMobilityFastNeighborAdvertisement.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.Experimental: // 11
                    return IpV6ExtensionHeaderMobilityExperimental.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HomeAgentSwitchMessage: // 12
                    return IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HeartbeatMessage: // 13
                    return IpV6ExtensionHeaderMobilityHeartbeatMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HandoverInitiateMessage: // 14
                    return IpV6ExtensionHeaderMobilityHandoverInitiateMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HandoverAcknowledgeMessage: // 15
                    return IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingRevocationMessage: // 16
                    return IpV6ExtensionHeaderMobilityBindingRevocationMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.LocalizedRoutingInitiation: // 17
                    return IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement: // 18
                    return IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement.ParseMessageData(nextHeader, checksum, messageData);

                default:
                    return null;
            }
        }

        internal override sealed void WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + DataOffset.MobilityHeaderType, (byte)MobilityHeaderType);
            buffer.Write(offset + DataOffset.Checksum, Checksum, Endianity.Big);
            WriteMessageData(buffer, offset + DataOffset.MessageData);
        }

        internal abstract void WriteMessageData(byte[] buffer, int offset);

        internal override sealed int DataLength
        {
            get { return MinimumDataLength + MessageDataLength; }
        }

        internal abstract int MessageDataLength { get; }

        internal override sealed bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderMobility);
        }

        internal abstract bool EqualsMessageData(IpV6ExtensionHeaderMobility other);

        private bool EqualsData(IpV6ExtensionHeaderMobility other)
        {
            return other != null &&
                   MobilityHeaderType == other.MobilityHeaderType && Checksum == other.Checksum && MobilityOptions.Equals(other.MobilityOptions) &&
                   EqualsMessageData(other);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 64  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderMobilityBindingRefreshRequest : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int Options = 2;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityBindingRefreshRequest(IpV4Protocol nextHeader, ushort checksum, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
        }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingRefreshRequest; }
        }

        internal static IpV6ExtensionHeaderMobilityBindingRefreshRequest ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityBindingRefreshRequest(nextHeader, checksum, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return other != null;
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 64  | Home Init Cookie                      |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 128 | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHomeTestInit : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int HomeInitCookie = 2;
            public const int Options = HomeInitCookie + sizeof(ulong);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityHomeTestInit(IpV4Protocol nextHeader, ushort checksum, ulong homeInitCookie, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            HomeInitCookie = homeInitCookie;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HomeTestInit; }
        }

        /// <summary>
        /// Contains a random value, the home init cookie.
        /// </summary>
        public ulong HomeInitCookie { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHomeTestInit);
        }

        internal static IpV6ExtensionHeaderMobilityHomeTestInit ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ulong homeInitCookie = messageData.ReadULong(MessageDataOffset.HomeInitCookie, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityHomeTestInit(nextHeader, checksum, homeInitCookie, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.HomeInitCookie, HomeInitCookie, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHomeTestInit other)
        {
            return other != null &&
                   HomeInitCookie == other.HomeInitCookie;
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 64  | Care-of Init Cookie                   |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 128 | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityCareOfTestInit : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int CareOfInitCookie = 2;
            public const int Options = CareOfInitCookie + sizeof(ulong);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityCareOfTestInit(IpV4Protocol nextHeader, ushort checksum, ulong careOfInitCookie, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            CareOfInitCookie = careOfInitCookie;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.CareOfTestInit; }
        }

        /// <summary>
        /// Contains a random value, the care-of init cookie.
        /// </summary>
        public ulong CareOfInitCookie { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityCareOfTestInit);
        }

        internal static IpV6ExtensionHeaderMobilityCareOfTestInit ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ulong careOfInitCookie = messageData.ReadULong(MessageDataOffset.CareOfInitCookie, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityCareOfTestInit(nextHeader, checksum, careOfInitCookie, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.CareOfInitCookie, CareOfInitCookie, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityCareOfTestInit other)
        {
            return other != null &&
                   CareOfInitCookie == other.CareOfInitCookie;
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Home Nonce Index                      |
    /// +-----+---------------------------------------+
    /// | 64  | Home Init Cookie                      |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 128 | Home Keygen Token                     |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 192 | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHomeTest : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int HomeNonceIndex = 0;
            public const int HomeInitCookie = HomeNonceIndex + sizeof(ushort);
            public const int HomeKeygenToken = HomeInitCookie + sizeof(ulong);
            public const int Options = HomeKeygenToken + sizeof(ulong);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityHomeTest(IpV4Protocol nextHeader, ushort checksum, ushort homeNonceIndex, ulong homeInitCookie, ulong homeKeygenToken,
                                                   IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            HomeNonceIndex = homeNonceIndex;
            HomeInitCookie = homeInitCookie;
            HomeKeygenToken = homeKeygenToken;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HomeTest; }
        }

        /// <summary>
        /// Will be echoed back by the mobile node to the correspondent node in a subsequent Binding Update.
        /// </summary>
        public ushort HomeNonceIndex { get; private set; }

        /// <summary>
        /// Contains the home init cookie.
        /// </summary>
        public ulong HomeInitCookie { get; private set; }

        /// <summary>
        /// Contains the 64-bit home keygen token used in the return routability procedure.
        /// </summary>
        public ulong HomeKeygenToken { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHomeTest);
        }

        internal static IpV6ExtensionHeaderMobilityHomeTest ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ushort homeNonceIndex = messageData.ReadUShort(MessageDataOffset.HomeNonceIndex, Endianity.Big);
            ushort homeInitCookie = messageData.ReadUShort(MessageDataOffset.HomeInitCookie, Endianity.Big);
            ushort homeKeygenToken = messageData.ReadUShort(MessageDataOffset.HomeKeygenToken, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityHomeTest(nextHeader, checksum, homeNonceIndex, homeInitCookie, homeKeygenToken, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.HomeNonceIndex, HomeNonceIndex, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.HomeInitCookie, HomeInitCookie, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.HomeKeygenToken, HomeKeygenToken, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHomeTest other)
        {
            return other != null &&
                   HomeNonceIndex == other.HomeNonceIndex && HomeInitCookie == other.HomeInitCookie && HomeKeygenToken == other.HomeKeygenToken;
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Care-of Nonce Index                   |
    /// +-----+---------------------------------------+
    /// | 64  | Care-of Init Cookie                   |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 128 | Care-of Keygen Token                  |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 192 | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityCareOfTest : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int CareOfNonceIndex = 0;
            public const int CareOfInitCookie = CareOfNonceIndex + sizeof(ushort);
            public const int CareOfKeygenToken = CareOfInitCookie + sizeof(ulong);
            public const int Options = CareOfKeygenToken + sizeof(ulong);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityCareOfTest(IpV4Protocol nextHeader, ushort checksum, ushort careOfNonceIndex, ulong careOfInitCookie,
                                                     ulong careOfKeygenToken, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            CareOfNonceIndex = careOfNonceIndex;
            CareOfInitCookie = careOfInitCookie;
            CareOfKeygenToken = careOfKeygenToken;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.CareOfTest; }
        }

        /// <summary>
        /// Will be echoed back by the mobile node to the correspondent node in a subsequent Binding Update.
        /// </summary>
        public ushort CareOfNonceIndex { get; private set; }

        /// <summary>
        /// Contains the care-of init cookie.
        /// </summary>
        public ulong CareOfInitCookie { get; private set; }

        /// <summary>
        /// Contains the 64-bit care-of keygen token used in the return routability procedure.
        /// </summary>
        public ulong CareOfKeygenToken { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityCareOfTest);
        }

        internal static IpV6ExtensionHeaderMobilityCareOfTest ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ushort careOfNonceIndex = messageData.ReadUShort(MessageDataOffset.CareOfNonceIndex, Endianity.Big);
            ushort careOfInitCookie = messageData.ReadUShort(MessageDataOffset.CareOfInitCookie, Endianity.Big);
            ushort careOfKeygenToken = messageData.ReadUShort(MessageDataOffset.CareOfKeygenToken, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityCareOfTest(nextHeader, checksum, careOfNonceIndex, careOfInitCookie, careOfKeygenToken, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.CareOfNonceIndex, CareOfNonceIndex, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.CareOfInitCookie, CareOfInitCookie, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.CareOfKeygenToken, CareOfKeygenToken, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityCareOfTest other)
        {
            return other != null &&
                   CareOfNonceIndex == other.CareOfNonceIndex && CareOfInitCookie == other.CareOfInitCookie && CareOfKeygenToken == other.CareOfKeygenToken;
        }
    }

    /// <summary>
    /// RFCs 5568, 6275.
    /// <pre>
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4-7 | 8-15                    |
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | 0   | Next Header         | Header Extension Length |
    /// +-----+---------------------+-------------------------+
    /// | 16  | MH Type             | Reserved                |
    /// +-----+---------------------+-------------------------+
    /// | 32  | Checksum                                      |
    /// +-----+-----------------------------------------------+
    /// | 48  | Sequence #                                    |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 64  | A | H | L | K | Reserved                      |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 80  | Lifetime                                      |
    /// +-----+-----------------------------------------------+
    /// | 96  | Mobility Options                              |
    /// | ... |                                               |
    /// +-----+-----------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityBindingUpdateBase : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int SequenceNumber = 0;
            public const int Acknowledge = SequenceNumber + sizeof(ushort);
            public const int HomeRegistration = Acknowledge;
            public const int LinkLocalAddressCompatibility = HomeRegistration;
            public const int KeyManagementMobilityCapability = LinkLocalAddressCompatibility;
            public const int Lifetime = KeyManagementMobilityCapability + sizeof(byte) + sizeof(byte);
            public const int Options = Lifetime + sizeof(ushort);
        }

        private static class MessageDataMask
        {
            public const byte Acknowledge = 0x80;
            public const byte HomeRegistration = 0x40;
            public const byte LinkLocalAddressCompatibility = 0x20;
            public const byte KeyManagementMobilityCapability = 0x10;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityBindingUpdateBase(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge,
                                                            bool homeRegistration, bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability,
                                                            ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            SequenceNumber = sequenceNumber;
            Acknowledge = acknowledge;
            HomeRegistration = homeRegistration;
            LinkLocalAddressCompatibility = linkLocalAddressCompatibility;
            KeyManagementMobilityCapability = keyManagementMobilityCapability;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Used by the receiving node to sequence Binding Updates and by the sending node to match a returned Binding Acknowledgement with this Binding Update.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Set by the sending mobile node to request a Binding Acknowledgement be returned upon receipt of the Binding Update.
        /// For Fast Binding Update this must be set to one to request that PAR send a Fast Binding Acknowledgement message.
        /// </summary>
        public bool Acknowledge { get; private set; }

        /// <summary>
        /// Set by the sending mobile node to request that the receiving node should act as this node's home agent.
        /// The destination of the packet carrying this message must be that of a router sharing the same subnet prefix as the home address 
        /// of the mobile node in the binding.
        /// For Fast Binding Update this must be set to one.
        /// </summary>
        public bool HomeRegistration { get; private set; }

        /// <summary>
        /// Set when the home address reported by the mobile node has the same interface identifier as the mobile node's link-local address.
        /// </summary>
        public bool LinkLocalAddressCompatibility { get; private set; }

        /// <summary>
        /// <para>
        /// If this is cleared, the protocol used for establishing the IPsec security associations between the mobile node and the home agent 
        /// does not survive movements.
        /// It may then have to be rerun. (Note that the IPsec security associations themselves are expected to survive movements.)
        /// If manual IPsec configuration is used, the bit must be cleared.
        /// </para>
        /// <para>
        /// This bit is valid only in Binding Updates sent to the home agent, and mustbe cleared in other Binding Updates.
        /// Correspondent nodes must ignore this bit.
        /// </para>
        /// </summary>
        public bool KeyManagementMobilityCapability { get; private set; }

        /// <summary>
        /// The number of time units remaining before the binding must be considered expired.
        /// A value of zero indicates that the Binding Cache entry for the mobile node must be deleted.
        /// One time unit is 4 seconds for Binding Update and 1 second for Fast Binding Update.
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingUpdateBase);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingUpdateBase other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && Acknowledge == other.Acknowledge && HomeRegistration == other.HomeRegistration &&
                   LinkLocalAddressCompatibility == other.LinkLocalAddressCompatibility &&
                   KeyManagementMobilityCapability == other.KeyManagementMobilityCapability && Lifetime == other.Lifetime;
        }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal static bool ParseMessageDataToFields(DataSegment messageData, out ushort sequenceNumber,
                                                      out bool acknowledge, out bool homeRegistration, out bool linkLocalAddressCompatibility,
                                                      out bool keyManagementMobilityCapability, out ushort lifetime, out IpV6MobilityOptions options)
        {
            if (messageData.Length < MinimumMessageDataLength)
            {
                sequenceNumber = 0;
                acknowledge = false;
                homeRegistration = false;
                linkLocalAddressCompatibility = false;
                keyManagementMobilityCapability = false;
                lifetime = 0;
                options = null;
                return false;
            }

            sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            acknowledge = messageData.ReadBool(MessageDataOffset.Acknowledge, MessageDataMask.Acknowledge);
            homeRegistration = messageData.ReadBool(MessageDataOffset.HomeRegistration, MessageDataMask.HomeRegistration);
            linkLocalAddressCompatibility = messageData.ReadBool(MessageDataOffset.LinkLocalAddressCompatibility, MessageDataMask.LinkLocalAddressCompatibility);
            keyManagementMobilityCapability = messageData.ReadBool(MessageDataOffset.KeyManagementMobilityCapability,
                                                                   MessageDataMask.KeyManagementMobilityCapability);
            lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return true;
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);

            byte flags = 0;
            if (Acknowledge)
                flags |= MessageDataMask.Acknowledge;
            if (HomeRegistration)
                flags |= MessageDataMask.HomeRegistration;
            if (LinkLocalAddressCompatibility)
                flags |= MessageDataMask.LinkLocalAddressCompatibility;
            if (KeyManagementMobilityCapability)
                flags |= MessageDataMask.KeyManagementMobilityCapability;
            buffer.Write(offset + MessageDataOffset.Acknowledge, flags);

            buffer.Write(offset + MessageDataOffset.Lifetime, Lifetime, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4-7 | 8-15                    |
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | 0   | Next Header         | Header Extension Length |
    /// +-----+---------------------+-------------------------+
    /// | 16  | MH Type             | Reserved                |
    /// +-----+---------------------+-------------------------+
    /// | 32  | Checksum                                      |
    /// +-----+-----------------------------------------------+
    /// | 48  | Sequence #                                    |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 64  | A | H | L | K | Reserved                      |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 80  | Lifetime                                      |
    /// +-----+-----------------------------------------------+
    /// | 96  | Mobility Options                              |
    /// | ... |                                               |
    /// +-----+-----------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingUpdate : IpV6ExtensionHeaderMobilityBindingUpdateBase
    {
        public IpV6ExtensionHeaderMobilityBindingUpdate(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge, bool homeRegistration,
                                                        bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability, ushort lifetime,
                                                        IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration, linkLocalAddressCompatibility, keyManagementMobilityCapability,
                   lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingUpdate; }
        }

        internal static IpV6ExtensionHeaderMobilityBindingUpdate ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            bool acknowledge;
            bool homeRegistration;
            bool linkLocalAddressCompatibility;
            bool keyManagementMobilityCapability;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out acknowledge, out homeRegistration, out linkLocalAddressCompatibility, 
                                          out keyManagementMobilityCapability, out lifetime, out options))
            {
                return null;
            }

            return new IpV6ExtensionHeaderMobilityBindingUpdate(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration,
                                                                linkLocalAddressCompatibility, keyManagementMobilityCapability, lifetime, options);
        }
    }

    public enum IpV6BindingAcknowledgementStatus : byte
    {
        /// <summary>
        /// RFCs 5213, 5568, 6275.
        /// </summary>
        BindingUpdateAccepted = 0,

        /// <summary>
        /// RFCs 5568, 6275.
        /// Binding Acknowledgement: Prefix discovery necessary.
        /// Fast Binding Acknowledgement: NCoA is invalid. Use NCoA supplied in "alternate" CoA.
        /// </summary>
        AcceptedBut = 1,

        /// <summary>
        /// RFC 5845.
        /// </summary>
        GreKeyOptionNotRequired = 2,

        /// <summary>
        /// RFC 5845.
        /// </summary>
        GreTunnelingButTlvHeaderNotSupported = 3,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaNotComplete = 4,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaReturnHomeWoNdp = 5,

        /// <summary>
        /// RFC 6058.
        /// </summary>
        PbuAcceptedTbIgnoredSettingsMistmatch = 6,

        /// <summary>
        /// RFCs 5568, 6275.
        /// </summary>
        ReasonUnspecified = 128,

        /// <summary>
        /// RFCs 5568, 6275.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// RFCs 5568, 6275.
        /// </summary>
        InsufficientResources = 130,

        /// <summary>
        /// RFCs 5568, 6275.
        /// Binding Acknowledgement: Home registration not supported.
        /// Fast Binding Acknowledgement: Incorrect interface identifier length.
        /// </summary>
        HomeRegistrationNotSupportedOrIncorrectInterfaceIdentifierLength = 131,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        NotHomeSubnet = 132,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        NotHomeAgentForThisMobileNode = 133,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        DuplicateAddressDetectionFailed = 134,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        SequenceNumberOutOfWindow = 135,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        ExpiredHomeNonceIndex = 136,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        ExpiredCareOfNonceIndex = 137,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        ExpiredNonces = 138,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        RegistrationTypeChangeDisallowed = 139,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        MobileRouterOperationNotPermitted = 140,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        InvalidPrefix = 141,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        NotAuthorizedForPrefix = 142,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        ForwardingSetupFailed = 143,

        /// <summary>
        /// RFC 4285.
        /// </summary>
        MIpV6IdMismatch = 144,

        /// <summary>
        /// RFC 4285.
        /// </summary>
        MIpV6MesgIdReqd = 145,

        /// <summary>
        /// RFC 4285.
        /// </summary>
        MIpV6AuthFail = 146,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        PermanentHomeKeygenTokenUnavailable = 147,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        CgaAndSignatureVerificationFailed = 148,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        PermanentHomeKeygenTokenExists = 149,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        NonNullHomeNonceIndexExpected = 150,

        /// <summary>
        /// RFC 5149.
        /// </summary>
        ServiceAuthorizationFailed = 151,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        ProxyRegNotEnabled = 152,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        NotLmaForThisMobileNode = 153,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MagNotAuthorizedForProxyReg = 154,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        NotAuthorizedForHomeNetworkPrefix = 155,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        TimestampMismatch = 156,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        TimestampLowerThanPrevAccepted = 157,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingHomeNetworkPrefixOption = 158,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        BcePbuPrefixSetDoNotMatch = 159,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingMnIdentifierOption = 160,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingHandoffIndicatorOption = 161,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingAccessTechTypeOption = 162,

        /// <summary>
        /// RFC 5845.
        /// </summary>
        GreKeyOptionRequired = 163,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaMalformed = 164,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaNonMcoaBindingExists = 165,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaProhibited = 166,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaUnknownCoa = 167,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaBulkRegistarationProhiited = 168,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaSimultaneousHomeAndForeignProhibited = 169,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        NotAuthorizedForIpV4MobilityService = 170,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        NotAuthorizedForIpV4HomeAddress = 171,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        NotAuthorizedForIpV6MobilityService = 172,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        MultipleIpV4HomeAddressAssignmentNotSupported = 173,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        InvalidCareOfAddress = 174,

        /// <summary>
        /// RFC 6602.
        /// </summary>
        InvalidMobileNodeGroupIdentifier = 175,

        /// <summary>
        /// RFC 6618.
        /// </summary>
        ReinitSaWithHac = 176,
    }

    /// <summary>
    /// RFCs 5568, 6275.
    /// <pre>
    /// +-----+-------------+---+---------------------+
    /// | Bit | 0-7         | 8 | 9-15                |
    /// +-----+-------------+---+---------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+---+---------------------+
    /// | 48  | Status      | K | Reserved            |
    /// +-----+-------------+---+---------------------+
    /// | 64  | Sequence #                            |
    /// +-----+---------------------------------------+
    /// | 80  | Lifetime                              |
    /// +-----+---------------------------------------+
    /// | 96  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityBindingAcknowledgementBase : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int Status = 0;
            public const int KeyManagementMobilityCapability = Status + sizeof(byte);
            public const int SequenceNumber = KeyManagementMobilityCapability + sizeof(byte);
            public const int Lifetime = SequenceNumber + sizeof(ushort);
            public const int Options = Lifetime + sizeof(ushort);
        }

        private static class MessageDataMask
        {
            public const byte KeyManagementMobilityCapability = 0x80;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityBindingAcknowledgementBase(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
                                                                     bool keyManagementMobilityCapability, ushort sequenceNumber, ushort lifetime,
                                                                     IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            Status = status;
            KeyManagementMobilityCapability = keyManagementMobilityCapability;
            SequenceNumber = sequenceNumber;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Indicating the disposition of the Binding Update.
        /// Values of the Status field less than 128 indicate that the Binding Update was accepted by the receiving node.
        /// Values greater than or equal to 128 indicate that the Binding Update was rejected by the receiving node.
        /// </summary>
        public IpV6BindingAcknowledgementStatus Status { get; private set; }

        /// <summary>
        /// <para>
        /// If this is cleared, the protocol used by the home agent for establishing the IPsec security associations between the mobile node and the home agent
        /// does not survive movements.
        /// It may then have to be rerun.
        /// (Note that the IPsec security associations themselves are expected to survive movements.)
        /// </para>
        /// <para>
        /// Correspondent nodes must set the K bit to 0.
        /// </para>
        /// </summary>
        public bool KeyManagementMobilityCapability { get; private set; }

        /// <summary>
        /// Copied from the Sequence Number field in the Binding Update.
        /// It is used by the mobile node in matching this Binding Acknowledgement with an outstanding Binding Update.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// <para>
        /// The granted lifetime, in time units of 4 seconds for Binding Acknowledgement and 1 second for Fast Binding Acknowledgement, 
        /// for which this node should retain the entry for this mobile node in its Binding Cache.
        /// </para>
        /// <para>
        /// The value of this field is undefined if the Status field indicates that the Binding Update was rejected.
        /// </para>
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingAcknowledgementBase);
        }

        internal static bool ParseMessageDataFields(DataSegment messageData, out IpV6BindingAcknowledgementStatus status,
                                                    out bool keyManagementMobilityCapability, out ushort sequenceNumber, out ushort lifetime,
                                                    out IpV6MobilityOptions options)
        {
            if (messageData.Length < MinimumMessageDataLength)
            {
                status = IpV6BindingAcknowledgementStatus.BindingUpdateAccepted;
                keyManagementMobilityCapability = false;
                sequenceNumber = 0;
                lifetime = 0;
                options = null;
                return false;
            }

            status = (IpV6BindingAcknowledgementStatus)messageData[MessageDataOffset.Status];
            keyManagementMobilityCapability = messageData.ReadBool(MessageDataOffset.KeyManagementMobilityCapability,
                                                                   MessageDataMask.KeyManagementMobilityCapability);

            sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return true;
        }

        internal override sealed void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.Status, (byte)Status);
            if (KeyManagementMobilityCapability)
                buffer.Write(offset + MessageDataOffset.KeyManagementMobilityCapability, MessageDataMask.KeyManagementMobilityCapability);
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.Lifetime, Lifetime, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingAcknowledgementBase other)
        {
            return other != null &&
                   Status == other.Status && KeyManagementMobilityCapability == other.KeyManagementMobilityCapability && SequenceNumber == other.SequenceNumber &&
                   Lifetime == other.Lifetime;
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+---+---------------------+
    /// | Bit | 0-7         | 8 | 9-15                |
    /// +-----+-------------+---+---------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+---+---------------------+
    /// | 48  | Status      | K | Reserved            |
    /// +-----+-------------+---+---------------------+
    /// | 64  | Sequence #                            |
    /// +-----+---------------------------------------+
    /// | 80  | Lifetime                              |
    /// +-----+---------------------------------------+
    /// | 96  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingAcknowledgement : IpV6ExtensionHeaderMobilityBindingAcknowledgementBase
    {
        public IpV6ExtensionHeaderMobilityBindingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
                                                                 bool keyManagementMobilityCapability, ushort sequenceNumber, ushort lifetime,
                                                                 IpV6MobilityOptions options)
            : base(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingAcknowledgement; }
        }

        internal static IpV6ExtensionHeaderMobilityBindingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            IpV6BindingAcknowledgementStatus status;
            bool keyManagementMobilityCapability;
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataFields(messageData, out status, out keyManagementMobilityCapability, out sequenceNumber, out lifetime, out options))
                return null;

            return new IpV6ExtensionHeaderMobilityBindingAcknowledgement(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber, lifetime,
                                                                         options);
        }
    }

    public enum IpV6BindingErrorStatus : byte
    {
        /// <summary>
        /// RFC 6275.
        /// </summary>
        UnknownBindingForHomeAddressDestinationOption = 1,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        UnrecognizedMhTypeValue = 2,
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+-------------------------+
    /// | 48  | Status      | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 64  | Home Address                          |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// | 192 | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingError : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int Status = 0;
            public const int HomeAddress = Status + sizeof(byte) + sizeof(byte);
            public const int Options = HomeAddress + IpV6Address.SizeOf;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityBindingError(IpV4Protocol nextHeader, ushort checksum, IpV6BindingErrorStatus status, IpV6Address homeAddress,
                                                       IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            Status = status;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingError; }
        }

        /// <summary>
        /// Indicating the reason for this message.
        /// </summary>
        public IpV6BindingErrorStatus Status { get; private set; }

        /// <summary>
        /// The home address that was contained in the Home Address destination option.
        /// The mobile node uses this information to determine which binding does not exist, in cases where the mobile node has several home addresses.
        /// </summary>
        public IpV6Address HomeAddress { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingError);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingError other)
        {
            return other != null &&
                   Status == other.Status && HomeAddress == other.HomeAddress;
        }

        internal static IpV6ExtensionHeaderMobilityBindingError ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6BindingErrorStatus status = (IpV6BindingErrorStatus)messageData[MessageDataOffset.Status];
            IpV6Address homeAddress = messageData.ReadIpV6Address(MessageDataOffset.HomeAddress, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityBindingError(nextHeader, checksum, status, homeAddress, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.Status, (byte)Status);
            buffer.Write(offset + MessageDataOffset.HomeAddress, HomeAddress, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }
    }

    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4-7 | 8-15                    |
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | 0   | Next Header         | Header Extension Length |
    /// +-----+---------------------+-------------------------+
    /// | 16  | MH Type             | Reserved                |
    /// +-----+---------------------+-------------------------+
    /// | 32  | Checksum                                      |
    /// +-----+---------------------+-------------------------+
    /// | 48  | Sequence #                                    |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 64  | A | H | L | K | Reserved                      |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 80  | Lifetime                                      |
    /// +-----+-----------------------------------------------+
    /// | 96  | Mobility Options                              |
    /// | ... |                                               |
    /// +-----+-----------------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderMobilityFastBindingUpdate : IpV6ExtensionHeaderMobilityBindingUpdateBase
    {
        public IpV6ExtensionHeaderMobilityFastBindingUpdate(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge,
                                                            bool homeRegistration, bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability,
                                                            ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration, linkLocalAddressCompatibility, keyManagementMobilityCapability, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.FastBindingUpdate; }
        }

        internal static IpV6ExtensionHeaderMobilityFastBindingUpdate ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            bool acknowledge;
            bool homeRegistration;
            bool linkLocalAddressCompatibility;
            bool keyManagementMobilityCapability;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out acknowledge, out homeRegistration, out linkLocalAddressCompatibility,
                                          out keyManagementMobilityCapability, out lifetime, out options))
            {
                return null;
            }

            return new IpV6ExtensionHeaderMobilityFastBindingUpdate(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration,
                                                                    linkLocalAddressCompatibility, keyManagementMobilityCapability, lifetime, options);
        }
    }

    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+-------------+---+---------------------+
    /// | Bit | 0-7         | 8 | 9-15                |
    /// +-----+-------------+---+---------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+---+---------------------+
    /// | 48  | Status      | K | Reserved            |
    /// +-----+-------------+---+---------------------+
    /// | 64  | Sequence #                            |
    /// +-----+---------------------------------------+
    /// | 80  | Lifetime                              |
    /// +-----+---------------------------------------+
    /// | 96  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderMobilityFastBindingAcknowledgement : IpV6ExtensionHeaderMobilityBindingAcknowledgementBase
    {
        public IpV6ExtensionHeaderMobilityFastBindingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
                                                                     bool keyManagementMobilityCapability, ushort sequenceNumber, ushort lifetime,
                                                                     IpV6MobilityOptions options)
            : base(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.FastBindingAcknowledgement; }
        }

        internal static IpV6ExtensionHeaderMobilityFastBindingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            IpV6BindingAcknowledgementStatus status;
            bool keyManagementMobilityCapability;
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataFields(messageData, out status, out keyManagementMobilityCapability, out sequenceNumber, out lifetime, out options))
                return null;

            return new IpV6ExtensionHeaderMobilityFastBindingAcknowledgement(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber,
                                                                             lifetime, options);
        }
    }

    /// <summary>
    /// RFCs 4068, 5568.
    /// Deprecated.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+-------------------------+
    /// | 48  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityFastNeighborAdvertisement : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int Options = 0;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityFastNeighborAdvertisement(IpV4Protocol nextHeader, ushort checksum, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.FastNeighborAdvertisement; }
        }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return other != null;
        }

        internal static IpV6ExtensionHeaderMobilityFastNeighborAdvertisement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityFastNeighborAdvertisement(nextHeader, checksum, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }
    }

    /// <summary>
    /// RFCs 5096.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+-------------------------+
    /// | 48  | Message Data                          |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityExperimental : IpV6ExtensionHeaderMobility
    {
        public IpV6ExtensionHeaderMobilityExperimental(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
            : base(nextHeader, checksum, IpV6MobilityOptions.None)
        {
            MessageData = messageData;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.Experimental; }
        }

        /// <summary>
        /// Carries the data specific to the experimental protocol extension.
        /// </summary>
        public DataSegment MessageData { get; private set; }

        internal override int MessageDataLength
        {
            get { return MessageData.Length; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityExperimental);
        }

        internal static IpV6ExtensionHeaderMobilityExperimental ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            return new IpV6ExtensionHeaderMobilityExperimental(nextHeader, checksum, messageData);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            MessageData.Write(buffer, offset);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityExperimental other)
        {
            return other != null &&
                   MessageData.Equals(other.MessageData);
        }
    }

    /// <summary>
    /// RFC 5142.
    /// <pre>
    /// +-----+----------------+-------------------------+
    /// | Bit | 0-7            | 8-15                    |
    /// +-----+----------------+-------------------------+
    /// | 0   | Next Header    | Header Extension Length |
    /// +-----+----------------+-------------------------+
    /// | 16  | MH Type        | Reserved                |
    /// +-----+----------------+-------------------------+
    /// | 32  | Checksum                                 |
    /// +-----+----------------+-------------------------+
    /// | 48  | # of Addresses | Reserved                |
    /// +-----+----------------+-------------------------+
    /// | 64  | Home Agent Addresses                     |
    /// | ... |                                          |
    /// +-----+------------------------------------------+
    /// |     | Mobility Options                         |
    /// | ... |                                          |
    /// +-----+------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int NumberOfAddresses = 0;
            public const int HomeAgentAddresses = NumberOfAddresses + sizeof(byte) + sizeof(byte);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.HomeAgentAddresses;

        public IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(IpV4Protocol nextHeader, ushort checksum, ReadOnlyCollection<IpV6Address> homeAgentAddresses,
                                                                 IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            HomeAgentAddresses = homeAgentAddresses;
        }

        public IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(IpV4Protocol nextHeader, ushort checksum, IList<IpV6Address> homeAgentAddresses, IpV6MobilityOptions options)
            : this(nextHeader, checksum, homeAgentAddresses.AsReadOnly(), options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HomeAgentSwitchMessage; }
        }

        /// <summary>
        /// A list of alternate home agent addresses for the mobile node.
        /// </summary>
        public ReadOnlyCollection<IpV6Address> HomeAgentAddresses { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + HomeAgentAddresses.Count * IpV6Address.SizeOf + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage);
        }

        internal static IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            byte numberOfAddresses = messageData[MessageDataOffset.NumberOfAddresses];
            int homeAgentAddressesSize = numberOfAddresses * IpV6Address.SizeOf;
            if (messageData.Length < MinimumMessageDataLength + homeAgentAddressesSize)
                return null;

            IpV6Address[] homeAgentAddresses = new IpV6Address[numberOfAddresses];
            for (int i = 0; i != numberOfAddresses; ++i)
                homeAgentAddresses[i] = messageData.ReadIpV6Address(MessageDataOffset.HomeAgentAddresses + i * IpV6Address.SizeOf, Endianity.Big);

            int optionsOffset = MessageDataOffset.HomeAgentAddresses + homeAgentAddressesSize;
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(optionsOffset, messageData.Length - optionsOffset));
            return new IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(nextHeader, checksum, homeAgentAddresses, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.NumberOfAddresses, (byte)HomeAgentAddresses.Count);
            for (int i = 0; i != HomeAgentAddresses.Count; ++i)
                buffer.Write(offset + MessageDataOffset.HomeAgentAddresses + i * IpV6Address.SizeOf, HomeAgentAddresses[i], Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.HomeAgentAddresses + HomeAgentAddresses.Count * IpV6Address.SizeOf);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage other)
        {
            return other != null &&
                   HomeAgentAddresses.SequenceEqual(other.HomeAgentAddresses);
        }
    }

    /// <summary>
    /// RFC 5847.
    /// <pre>
    /// +-----+----------------+------+----+-------------+
    /// | Bit | 0-7            | 8-13 | 14 | 15          |
    /// +-----+----------------+------+----+-------------+
    /// | 0   | Next Header    | Header Extension Length |
    /// +-----+----------------+-------------------------+
    /// | 16  | MH Type        | Reserved                |
    /// +-----+----------------+-------------------------+
    /// | 32  | Checksum                                 |
    /// +-----+-----------------------+----+-------------+
    /// | 48  | Reserved              | U  | R           |
    /// +-----+-----------------------+----+-------------+
    /// | 64  | Sequence Number                          |
    /// |     |                                          |
    /// +-----+------------------------------------------+
    /// | 96  | Mobility Options                         |
    /// | ... |                                          |
    /// +-----+------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHeartbeatMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int IsUnsolicitedHeartbeatResponse = sizeof(byte);
            public const int IsResponse = IsUnsolicitedHeartbeatResponse;
            public const int SequenceNumber = IsResponse + sizeof(byte);
            public const int MobilityOptions = SequenceNumber + sizeof(uint);
        }

        private static class MessageDataMask
        {
            public const byte IsUnsolicitedHeartbeatResponse = 0x02;
            public const byte IsResponse = 0x01;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.MobilityOptions;

        public IpV6ExtensionHeaderMobilityHeartbeatMessage(IpV4Protocol nextHeader, ushort checksum, bool isUnsolicitedHeartbeatResponse, bool isResponse,
                                                           uint sequenceNumber, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            IsUnsolicitedHeartbeatResponse = isUnsolicitedHeartbeatResponse;
            IsResponse = isResponse;
            SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HeartbeatMessage; }
        }

        /// <summary>
        /// Set to true in Unsolicited Heartbeat Response.
        /// </summary>
        public bool IsUnsolicitedHeartbeatResponse { get; private set; }

        /// <summary>
        /// Indicates whether the message is a request or a response. 
        /// When it's set to false, it indicates that the Heartbeat message is a request.
        /// When it's set to true, it indicates that the Heartbeat message is a response.
        /// </summary>
        public bool IsResponse { get; private set; }

        /// <summary>
        /// Sequence number used for matching the request to the reply.
        /// </summary>
        public uint SequenceNumber { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHeartbeatMessage);
        }

        internal static IpV6ExtensionHeaderMobilityHeartbeatMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            bool isUnsolicitedHeartbeatResponse = messageData.ReadBool(MessageDataOffset.IsUnsolicitedHeartbeatResponse, MessageDataMask.IsUnsolicitedHeartbeatResponse);
            bool isResponse = messageData.ReadBool(MessageDataOffset.IsResponse, MessageDataMask.IsResponse);
            uint sequenceNumber = messageData.ReadUInt(MessageDataOffset.SequenceNumber, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.MobilityOptions, messageData.Length - MessageDataOffset.MobilityOptions));
            return new IpV6ExtensionHeaderMobilityHeartbeatMessage(nextHeader, checksum, isUnsolicitedHeartbeatResponse, isResponse, sequenceNumber, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            byte isUnsolicitedHeartbeatResponseAndIsResponse = 0;
            if (IsUnsolicitedHeartbeatResponse)
                isUnsolicitedHeartbeatResponseAndIsResponse |= MessageDataMask.IsUnsolicitedHeartbeatResponse;
            if (IsResponse)
                isUnsolicitedHeartbeatResponseAndIsResponse |= MessageDataMask.IsResponse;
            buffer.Write(offset + MessageDataOffset.IsUnsolicitedHeartbeatResponse, isUnsolicitedHeartbeatResponseAndIsResponse);

            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.MobilityOptions);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHeartbeatMessage other)
        {
            return other != null &&
                   IsUnsolicitedHeartbeatResponse == other.IsUnsolicitedHeartbeatResponse && IsResponse == other.IsResponse &&
                   SequenceNumber == other.SequenceNumber;
        }
    }
    
    public enum IpV6HandoverInitiateMessageCode : byte
    {
        SourceIpAddressIsPreviousCareOfAddress = 0x00,
        SourceIpAddressIsNotPreviousCareOfAddress = 0x01,
    }
    
    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+---+---+----------+-------------------------+
    /// | Bit | 0 | 1 | 2-7      | 8-15                    |
    /// +-----+---+---+----------+-------------------------+
    /// | 0   | Next Header      | Header Extension Length |
    /// +-----+------------------+-------------------------+
    /// | 16  | MH Type          | Reserved                |
    /// +-----+------------------+-------------------------+
    /// | 32  | Checksum                                   |
    /// +-----+--------------------------------------------+
    /// | 48  | Sequence #                                 |
    /// +-----+---+---+----------+-------------------------+
    /// | 64  | S | U | Reserved | Code                    |
    /// +-----+---+---+----------+-------------------------+
    /// | 96  | Mobility Options                           |
    /// | ... |                                            |
    /// +-----+--------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHandoverInitiateMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int SequenceNumber = 0;
            public const int AssignedAddressConfiguration = SequenceNumber + sizeof(ushort);
            public const int Buffer = AssignedAddressConfiguration;
            public const int Code = Buffer + sizeof(byte);
            public const int Options = Code + sizeof(byte);
        }

        private static class MessageDataMask
        {
            public const byte AssignedAddressConfiguration = 0x80;
            public const byte Buffer = 0x40;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityHandoverInitiateMessage(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool assignedAddressConfiguration,
                                                                  bool buffer, IpV6HandoverInitiateMessageCode code,
                                                                  IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            SequenceNumber = sequenceNumber;
            AssignedAddressConfiguration = assignedAddressConfiguration;
            Buffer = buffer;
            Code = code;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HandoverInitiateMessage; }
        }

        /// <summary>
        /// Must be set by the sender so replies can be matched to this message.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Assigned address configuration flag.  
        /// When set to true, this message requests a new CoA to be returned by the destination.
        /// May be set when Code = 0. Must be false when Code = 1.
        /// </summary>
        public bool AssignedAddressConfiguration { get; private set; }

        /// <summary>
        /// When set, the destination should buffer any packets toward the node indicated in the options of this message.
        /// Used when Code = 0, should be set to false when Code = 1.
        /// </summary>
        public bool Buffer { get; private set; }

        /// <summary>
        /// Describes whether the source ip address is a previous care of address.
        /// </summary>
        public IpV6HandoverInitiateMessageCode Code { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHandoverInitiateMessage);
        }

        internal static IpV6ExtensionHeaderMobilityHandoverInitiateMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ushort sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            bool assignedAddressConfiguration = messageData.ReadBool(MessageDataOffset.AssignedAddressConfiguration,
                                                                     MessageDataMask.AssignedAddressConfiguration);
            bool buffer = messageData.ReadBool(MessageDataOffset.Buffer, MessageDataMask.Buffer);
            IpV6HandoverInitiateMessageCode code = (IpV6HandoverInitiateMessageCode)messageData[MessageDataOffset.Code];
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityHandoverInitiateMessage(nextHeader, checksum, sequenceNumber, assignedAddressConfiguration, buffer, code, options);
        }
        
        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);

            byte assignedAddressConfigurationAndBuffer = 0;
            if (AssignedAddressConfiguration)
                assignedAddressConfigurationAndBuffer |= MessageDataMask.AssignedAddressConfiguration;
            if (Buffer)
                assignedAddressConfigurationAndBuffer |= MessageDataMask.Buffer;
            buffer.Write(offset + MessageDataOffset.AssignedAddressConfiguration, assignedAddressConfigurationAndBuffer);

            buffer.Write(offset + MessageDataOffset.Code, (byte)Code);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHandoverInitiateMessage other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && AssignedAddressConfiguration == other.AssignedAddressConfiguration && Buffer == other.Buffer &&
                   Code == other.Code;
        }
    }

    public enum IpV6MobilityHandoverAcknowledgeCode : byte
    {
        /// <summary>
        /// Handover Accepted, NCoA valid.
        /// </summary>
        HandoverAcceptedMobilityNodeCareOfAddressValid = 0,
        
        /// <summary>
        /// Handover Accepted, NCoA not valid or in use.
        /// </summary>
        HandoverAcceptedMobilityNodeCareOfAddressNotValidOrInUse = 1,

        /// <summary>
        /// Handover Accepted, NCoA assigned (used in Assigned Addressing).
        /// </summary>
        HandoverAcceptedMobilityNodeCareOfAddressAssigned = 2,

        /// <summary>
        /// Handover Accepted, use PCoA.
        /// </summary>
        HandoverAcceptedUsePreviousCareOfAddress = 3,

        /// <summary>
        /// Message sent unsolicited, usually to trigger an HI message.
        /// </summary>
        MessageSentUnsolicited = 4,

        /// <summary>
        /// Handover Not Accepted, reason unspecified.
        /// </summary>
        HandoverNotAccepted = 128,

        /// <summary>
        /// Administratively prohibited.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// Insufficient resources.
        /// </summary>
        InsufficientResources = 130,
    }

    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Sequence #                            |
    /// +-----+-------------+-------------------------+
    /// | 64  | Reserved    | Code                    |
    /// +-----+-------------+-------------------------+
    /// | 80  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int SequenceNumber = 0;
            public const int Code = SequenceNumber + sizeof(ushort) + sizeof(byte);
            public const int Options = Code + sizeof(byte);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber,
                                                                     IpV6MobilityHandoverAcknowledgeCode code, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            SequenceNumber = sequenceNumber;
            Code = code;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HandoverAcknowledgeMessage; }
        }

        /// <summary>
        /// Copied from the corresponding field in the Handover Initiate message to which this message is a response,
        /// to enable the receiver to match this Handover Acknowledge message with an outstanding Handover Initiate message.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Describes whether the handover was accepted or not and more details.
        /// </summary>
        public IpV6MobilityHandoverAcknowledgeCode Code { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && Code == other.Code;
        }

        internal static IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ushort sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            IpV6MobilityHandoverAcknowledgeCode code = (IpV6MobilityHandoverAcknowledgeCode)messageData[MessageDataOffset.Code];
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage(nextHeader, checksum, sequenceNumber, code, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.Code, (byte)Code);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }
    }

    public enum IpV6MobilityBindingRevocationType : byte
    {
        /// <summary>
        /// Binding Revocation Indication.
        /// </summary>
        BindingRevocationIndication = 1,

        /// <summary>
        /// Binding Revocation Acknowledgement.
        /// </summary>
        BindingRevocationAcknowledgement = 2,
    }

    /// <summary>
    /// RFC 5846.
    /// <pre>
    /// +-----+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3-7 | 8-15                    |
    /// +-----+---+---+---+-----+-------------------------+
    /// | 0   | Next Header     | Header Extension Length |
    /// +-----+-----------------+-------------------------+
    /// | 16  | MH Type         | Reserved                |
    /// +-----+-----------------+-------------------------+
    /// | 32  | Checksum                                  |
    /// +-----+-----------------+-------------------------+
    /// | 48  | B.R. Type       | R. Trigger or Status    |
    /// +-----+-----------------+-------------------------+
    /// | 64  | Sequence #                                |
    /// +-----+---+---+---+-------------------------------+
    /// | 80  | P | V | G | Reserved                      |
    /// +-----+---+---+---+-------------------------------+
    /// | 96  | Mobility Options                          |
    /// | ... |                                           |
    /// +-----+-------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityBindingRevocationMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int BindingRevocationType = 0;
            public const int RevocationTriggerOrStatus = BindingRevocationType + sizeof(byte);
            public const int SequenceNumber = RevocationTriggerOrStatus + sizeof(byte);
            public const int ProxyBinding = SequenceNumber + sizeof(ushort);
            public const int IpV4HomeAddressBindingOnly = ProxyBinding;
            public const int Global = IpV4HomeAddressBindingOnly;
            public const int Options = Global + sizeof(byte) + sizeof(byte);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        private static class MessageDataMask
        {
            public const byte ProxyBinding = 0x80;
            public const byte IpV4HomeAddressBindingOnly = 0x40;
            public const byte Global = 0x20;
        }

        public IpV6ExtensionHeaderMobilityBindingRevocationMessage(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool proxyBinding,
                                                                   bool ipV4HomeAddressBindingOnly, bool global, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            SequenceNumber = sequenceNumber;
            ProxyBinding = proxyBinding;
            IpV4HomeAddressBindingOnly = ipV4HomeAddressBindingOnly;
            Global = global;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override sealed IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingRevocationMessage; }
        }

        /// <summary>
        /// Defines the type of the Binding Revocation Message.
        /// </summary>
        public abstract IpV6MobilityBindingRevocationType BindingRevocationType { get; }

        /// <summary>
        /// In indication, used by the initiator to match a returned Binding Revocation Acknowledgement with this Binding Revocation Indication.
        /// This sequence number could be a random number.
        /// At any time, implementations must ensure there is no collision between the sequence numbers of all outstanding Binding Revocation Indication 
        /// Messages.
        /// In acknowledgement, copied from the Sequence Number field in the Binding Revocation Indication.
        /// It is used by the initiator, e.g., HA, LMA, MAG, in matching this Binding Revocation Acknowledgement 
        /// with the outstanding Binding Revocation Indication.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// In indication, set by the initiator to indicate that the revoked binding(s) is a PMIPv6 binding.
        /// In acknowledgement, set if set in the corresponding Binding Revocation Indication message.
        /// </summary>
        public bool ProxyBinding { get; private set; }

        /// <summary>
        /// In indication, Set by the initiator, home agent, or local mobility anchor to indicate to the receiving mobility entity the termination
        /// of the IPv4 Home Address binding only as in Home Agent Operation and Local Mobility Anchor Operation.
        /// In acknowledgement, set if the it is set in the corresponding Binding Revocation Indication message.
        /// </summary>
        public bool IpV4HomeAddressBindingOnly { get; private set; }

        /// <summary>
        /// In indication, Set by the initiator, LMA or MAG, to indicate the termination of all Per-Peer mobility Bindings or Multiple Bindings that share 
        /// a common identifier(s) and are served by the initiator and responder as in Local Mobility Anchor Operation and Mobile Access Gateway Operation.
        /// In acknowledgement, set if it is set in the corresponding Binding Revocation Indication message.
        /// </summary>
        public bool Global { get; private set; }

        internal abstract byte RevocationTriggerOrStatus { get; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingRevocationMessage);
        }

        internal static IpV6ExtensionHeaderMobilityBindingRevocationMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6MobilityBindingRevocationType bindingRevocationType = (IpV6MobilityBindingRevocationType)messageData[MessageDataOffset.BindingRevocationType];
            byte revocationTriggerOrStatus = messageData[MessageDataOffset.RevocationTriggerOrStatus];
            ushort sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            bool proxyBinding = messageData.ReadBool(MessageDataOffset.ProxyBinding, MessageDataMask.ProxyBinding);
            bool ipV4HomeAddressBindingOnly = messageData.ReadBool(MessageDataOffset.IpV4HomeAddressBindingOnly, MessageDataMask.IpV4HomeAddressBindingOnly);
            bool global = messageData.ReadBool(MessageDataOffset.Global, MessageDataMask.Global);
            IpV6MobilityOptions options =
                new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            switch (bindingRevocationType)
            {
                case IpV6MobilityBindingRevocationType.BindingRevocationIndication:
                    return new IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage(nextHeader, checksum, (Ipv6MobilityBindingRevocationTrigger)revocationTriggerOrStatus, sequenceNumber,
                                                                                             proxyBinding, ipV4HomeAddressBindingOnly, global, options);

                case IpV6MobilityBindingRevocationType.BindingRevocationAcknowledgement:
                    return new IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage(nextHeader, checksum,
                                                                                                  (Ipv6MobilityBindingRevocationStatus)revocationTriggerOrStatus,
                                                                                                  sequenceNumber, proxyBinding, ipV4HomeAddressBindingOnly,
                                                                                                  global, options);

                default:
                    return null;
            }
        }

        internal override sealed void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.BindingRevocationType, (byte)BindingRevocationType);
            buffer.Write(offset + MessageDataOffset.RevocationTriggerOrStatus, RevocationTriggerOrStatus);
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            
            byte flags = 0;
            if (ProxyBinding)
                flags |= MessageDataMask.ProxyBinding;
            if (IpV4HomeAddressBindingOnly)
                flags |= MessageDataMask.IpV4HomeAddressBindingOnly;
            if (Global)
                flags |= MessageDataMask.Global;
            buffer.Write(offset + MessageDataOffset.ProxyBinding, flags);

            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingRevocationMessage other)
        {
            return other != null &&
                   BindingRevocationType == other.BindingRevocationType && RevocationTriggerOrStatus == other.RevocationTriggerOrStatus &&
                   SequenceNumber == other.SequenceNumber && ProxyBinding == other.ProxyBinding &&
                   IpV4HomeAddressBindingOnly == other.IpV4HomeAddressBindingOnly && Global == other.Global;
        }
    }

    /// <summary>
    /// The Per-MN Revocation Trigger values are less than 128.
    /// The Per-MN Revocation Trigger is used when the BRI message intends to revoke one or more bindings for the same mobile node.
    /// The Global Revocation Trigger values are greater than 128 and less than 250 and used in the BRI message 
    /// when the Global (G) bit is set for global revocation.
    /// The values 250-255 are reserved for testing purposes only.
    /// </summary>
    public enum Ipv6MobilityBindingRevocationTrigger : byte
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Administrative Reason.
        /// </summary>
        AdministrativeReason = 1,

        /// <summary>
        /// Inter-MAG Handover - same Access Type.
        /// </summary>
        InterMagHandoverSameAccessType = 2,

        /// <summary>
        /// Inter-MAG Handover - different Access Type.
        /// </summary>
        InterMagHandoverDifferentAccessType = 3,

        /// <summary>
        /// Inter-MAG Handover - Unknown.
        /// </summary>
        InterMagHandoverUnknown = 4,

        /// <summary>
        /// User-Initiated Session(s) Termination
        /// </summary>
        UserInitiatedSessionsTermination = 5,

        /// <summary>
        /// Access Network Session(s) Termination
        /// </summary>
        AccessNetworkSessionsTermination = 6,
        
        /// <summary>
        /// Possible Out-of-Sync BCE State.
        /// </summary>
        PossibleOutOfSyncBceState = 7,

        /// <summary>
        /// Global Revocation Trigger Value.
        /// Per-Peer Policy.
        /// </summary>
        GlobalPerPeerPolicy = 128,

        /// <summary>
        /// Global Revocation Trigger Value.
        /// Revoking Mobility Node Local Policy.
        /// </summary>
        GlobalRevokingMobilityNodeLocalPolicy = 129,
    }

    /// <summary>
    /// RFC 5846.
    /// <pre>
    /// +-----+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3-7 | 8-15                    |
    /// +-----+---+---+---+-----+-------------------------+
    /// | 0   | Next Header     | Header Extension Length |
    /// +-----+-----------------+-------------------------+
    /// | 16  | MH Type         | Reserved                |
    /// +-----+-----------------+-------------------------+
    /// | 32  | Checksum                                  |
    /// +-----+-----------------+-------------------------+
    /// | 48  | B.R. Type       | R. Trigger              |
    /// +-----+-----------------+-------------------------+
    /// | 64  | Sequence #                                |
    /// +-----+---+---+---+-------------------------------+
    /// | 80  | P | V | G | Reserved                      |
    /// +-----+---+---+---+-------------------------------+
    /// | 96  | Mobility Options                          |
    /// | ... |                                           |
    /// +-----+-------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage : IpV6ExtensionHeaderMobilityBindingRevocationMessage
    {
        public IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage(IpV4Protocol nextHeader, ushort checksum,
                                                                             Ipv6MobilityBindingRevocationTrigger revocationTrigger, ushort sequenceNumber,
                                                                             bool proxyBinding, bool ipV4HomeAddressBindingOnly, bool global,
                                                                             IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, proxyBinding, ipV4HomeAddressBindingOnly, global, options)
        {
            RevocationTrigger = revocationTrigger;
        }

        /// <summary>
        /// Defines the type of the Binding Revocation Message.
        /// </summary>
        public override sealed IpV6MobilityBindingRevocationType BindingRevocationType
        {
            get { return IpV6MobilityBindingRevocationType.BindingRevocationIndication; }
        }

        /// <summary>
        /// Indicating the event that triggered the initiator to send the BRI message.
        /// </summary>
        public Ipv6MobilityBindingRevocationTrigger RevocationTrigger { get; private set; }

        internal override byte RevocationTriggerOrStatus
        {
            get { return (byte)RevocationTrigger; }
        }
    }

    /// <summary>
    /// Values of the Status field less than 128 indicate that the Binding Revocation Indication was processed successfully by the responder.
    /// Values greater than or equal to 128 indicate that the Binding Revocation Indication was rejected by the responder. 
    /// </summary>
    public enum Ipv6MobilityBindingRevocationStatus : byte
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Partial success.
        /// </summary>
        PartialSuccess = 1,

        /// <summary>
        /// Binding Does NOT Exist.
        /// </summary>
        BindingDoesNotExist = 128,

        /// <summary>
        /// IPv4 Home Address Option Required.
        /// </summary>
        IpV4HomeAddressOptionRequired = 129,

        /// <summary>
        /// Global Revocation NOT Authorized.
        /// </summary>
        GlobalRevocationNotAuthorized = 130,

        /// <summary>
        /// Revoked Mobile Nodes Identity Required.
        /// </summary>
        RevokedMobileNodesIdentityRequired = 131,

        /// <summary>
        /// Revocation Failed - MN is Attached.
        /// </summary>
        RevocationFailedMobilityNodeIsAttached = 132,

        /// <summary>
        /// Revocation Trigger NOT Supported.
        /// </summary>
        RevocationTriggerNotSupported = 133,

        /// <summary>
        /// Revocation Function NOT Supported.
        /// </summary>
        RevocationFunctionNotSupported = 134,

        /// <summary>
        /// Proxy Binding Revocation NOT Supported.
        /// </summary>
        ProxyBindingRevocationNotSupported = 135,
    }

    /// <summary>
    /// RFC 5846.
    /// <pre>
    /// +-----+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3-7 | 8-15                    |
    /// +-----+---+---+---+-----+-------------------------+
    /// | 0   | Next Header     | Header Extension Length |
    /// +-----+-----------------+-------------------------+
    /// | 16  | MH Type         | Reserved                |
    /// +-----+-----------------+-------------------------+
    /// | 32  | Checksum                                  |
    /// +-----+-----------------+-------------------------+
    /// | 48  | B.R. Type       | Status                  |
    /// +-----+-----------------+-------------------------+
    /// | 64  | Sequence #                                |
    /// +-----+---+---+---+-------------------------------+
    /// | 80  | P | V | G | Reserved                      |
    /// +-----+---+---+---+-------------------------------+
    /// | 96  | Mobility options                          |
    /// | ... |                                           |
    /// +-----+-------------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage : IpV6ExtensionHeaderMobilityBindingRevocationMessage
    {
        public IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage(IpV4Protocol nextHeader, ushort checksum,
                                                                                  Ipv6MobilityBindingRevocationStatus status, ushort sequenceNumber,
                                                                                  bool proxyBinding, bool ipV4HomeAddressBindingOnly, bool global,
                                                                                  IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, proxyBinding, ipV4HomeAddressBindingOnly, global, options)
        {
            Status = status;
        }

        /// <summary>
        /// Defines the type of the Binding Revocation Message.
        /// </summary>
        public override sealed IpV6MobilityBindingRevocationType BindingRevocationType
        {
            get { return IpV6MobilityBindingRevocationType.BindingRevocationAcknowledgement; }
        }

        /// <summary>
        /// Indicating the result of processing the Binding Revocation Indication message by the responder.
        /// </summary>
        public Ipv6MobilityBindingRevocationStatus Status { get; private set; }

        internal override byte RevocationTriggerOrStatus
        {
            get { return (byte)Status; }
        }
    }

    /// <summary>
    /// RFC-ietf-netext-pmip-lr-10.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Sequence #                            |
    /// +-----+---------------------------------------+
    /// | 64  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 80  | Lifetime                              |
    /// +-----+---------------------------------------+
    /// | 96  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityLocalizedRouting : IpV6ExtensionHeaderMobility
    {
        /// <summary>
        /// Indicates an infinite lifetime.
        /// </summary>
        public const ushort LifetimeInfinite = 0xFFFF;

        private static class MessageDataOffset
        {
            public const int SequenceNumber = 0;
            public const int Lifetime = SequenceNumber + sizeof(ushort) + sizeof(ushort);
            public const int Options = Lifetime + sizeof(ushort);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityLocalizedRouting(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, ushort lifetime,
                                                           IpV6MobilityOptions options)
            : base(nextHeader, checksum, options)
        {
            SequenceNumber = sequenceNumber;
            Lifetime = lifetime;
        }

        /// <summary>
        /// In initiation, a monotonically increasing integer. Set by a sending node in a request message, and used to match a reply to the request.
        /// In acknowledgement, copied from the sequence number field of the LRI message being responded to.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// In initiation, the requested time in seconds for which the sender wishes to have local forwarding.
        /// A value of 0xffff (all ones) indicates an infinite lifetime.
        /// When set to 0, indicates a request to stop localized routing.
        /// In acknowledgement, the time in seconds for which the local forwarding is supported.
        /// Typically copied from the corresponding field in the LRI message.
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override sealed bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityLocalizedRouting);
        }

        internal abstract bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRouting other);

        internal static bool ParseMessageDataToFields(DataSegment messageData, out ushort sequenceNumber, out ushort lifetime, out IpV6MobilityOptions options)
        {
            if (messageData.Length < MinimumMessageDataLength)
            {
                sequenceNumber = 0;
                lifetime = 0;
                options = null;
                return false;
            }

            sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return true;
        }

        internal override sealed void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.Lifetime, Lifetime, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityLocalizedRouting other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && Lifetime == other.Lifetime && EqualsMessageDataLocalizedRoutingExtraFields(other);
        }
    }

    /// <summary>
    /// RFC-ietf-netext-pmip-lr-10.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+---------------------------------------+
    /// | 48  | Sequence #                            |
    /// +-----+---------------------------------------+
    /// | 64  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 80  | Lifetime                              |
    /// +-----+---------------------------------------+
    /// | 96  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation : IpV6ExtensionHeaderMobilityLocalizedRouting
    {
        public IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber,
                                                                     ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.LocalizedRoutingInitiation; }
        }

        internal static IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out lifetime, out options))
                return null;

            return new IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation(nextHeader, checksum, sequenceNumber, lifetime, options);
        }

        internal override bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRouting other)
        {
            return true;
        }
    }

    /// <summary>
    /// RFC-ietf-netext-pmip-lr-10.
    /// <pre>
    /// +-----+---+----------+-------------------------+
    /// | Bit | 0 | 3-7      | 8-15                    |
    /// +-----+---+----------+-------------------------+
    /// | 0   | Next Header  | Header Extension Length |
    /// +-----+--------------+-------------------------+
    /// | 16  | MH Type      | Reserved                |
    /// +-----+--------------+-------------------------+
    /// | 32  | Checksum                               |
    /// +-----+----------------------------------------+
    /// | 48  | Sequence #                             |
    /// +-----+---+----------+-------------------------+
    /// | 64  | U | Reserved | Status                  |
    /// +-----+---+----------+-------------------------+
    /// | 80  | Lifetime                               |
    /// +-----+----------------------------------------+
    /// | 96  | Mobility Options                       |
    /// | ... |                                        |
    /// +-----+----------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement : IpV6ExtensionHeaderMobilityLocalizedRouting
    {
        private static class MessageDataOffset
        {
            public const int Unsolicited = sizeof(ushort);
        }

        private static class MessageDataMask
        {
            public const byte Unsolicited = 0x80;
        }

        public IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool unsolicited,
                                                                          ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, lifetime, options)
        {
            Unsolicited = unsolicited;
        }

        /// <summary>
        /// When true, the LRA message is sent unsolicited.
        /// The Lifetime field indicates a new requested value.
        /// The MAG must wait for the regular LRI message to confirm that the request is acceptable to the LMA.
        /// </summary>
        public bool Unsolicited { get; private set; }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement; }
        }

        internal static IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out lifetime, out options))
                return null;

            bool unsolicited = messageData.ReadBool(MessageDataOffset.Unsolicited, MessageDataMask.Unsolicited);

            return new IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement(nextHeader, checksum, sequenceNumber, unsolicited, lifetime, options);
        }

        internal override bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRouting other)
        {
            return EqualsMessageDataLocalizedRoutingExtraFields(other as IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement);
        }

        private bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement other)
        {
            return other != null &&
                   Unsolicited == other.Unsolicited;
        }
    }
}