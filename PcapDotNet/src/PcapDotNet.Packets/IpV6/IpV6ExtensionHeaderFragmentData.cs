using System;
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
    public class IpV6ExtensionHeaderFragmentData : IpV6ExtensionHeader
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

        public const int DataLength = DataOffset.Identification + sizeof(uint);

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

        internal static IpV6ExtensionHeaderFragmentData ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            if (data.Length != DataLength)
                return null;

            ushort fragmentOffset = (ushort)((data.ReadUShort(DataOffset.FragmentOffset, Endianity.Big) & DataMask.FragmentOffset) >> DataShift.FragmentOffset);
            bool moreFragments = data.ReadBool(DataOffset.MoreFragments, DataMask.MoreFragments);
            uint identification = data.ReadUInt(DataOffset.Identification, Endianity.Big);

            return new IpV6ExtensionHeaderFragmentData(nextHeader, fragmentOffset, moreFragments, identification);
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
        FastBindingAcknowledgment = 9,

        /// <summary>
        /// RFC 5568.
        /// Deprecated.
        /// </summary>
        FastNeighborAdvertisement = 10,

        /// <summary>
        /// RFC 5096.
        /// </summary>
        ExperimentalMobilityHeader = 11,

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
        LocalizedRoutingAcknowledgment = 18,
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
    public abstract class IpV6ExtensionHeaderMobility : IpV6ExtensionHeader
    {
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
                case IpV6MobilityHeaderType.FastBindingUpdate: // 8
                case IpV6MobilityHeaderType.FastBindingAcknowledgment: // 9
                case IpV6MobilityHeaderType.FastNeighborAdvertisement: // 10
                case IpV6MobilityHeaderType.ExperimentalMobilityHeader: // 11
                case IpV6MobilityHeaderType.HomeAgentSwitchMessage: // 12
                case IpV6MobilityHeaderType.HeartbeatMessage: // 13
                case IpV6MobilityHeaderType.HandoverInitiateMessage: // 14
                case IpV6MobilityHeaderType.HandoverAcknowledgeMessage: // 15
                case IpV6MobilityHeaderType.BindingRevocationMessage: // 16
                case IpV6MobilityHeaderType.LocalizedRoutingInitiation: // 17
                case IpV6MobilityHeaderType.LocalizedRoutingAcknowledgment: // 18
                default:
                    return null;
            }
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
    public class IpV6ExtensionHeaderMobilityHomeTestInit : IpV6ExtensionHeaderMobility
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

        internal static IpV6ExtensionHeaderMobilityHomeTestInit ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ulong homeInitCookie = messageData.ReadULong(MessageDataOffset.HomeInitCookie, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityHomeTestInit(nextHeader, checksum, homeInitCookie, options);
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
    public class IpV6ExtensionHeaderMobilityCareOfTestInit : IpV6ExtensionHeaderMobility
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

        internal static IpV6ExtensionHeaderMobilityCareOfTestInit ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ulong careOfInitCookie = messageData.ReadULong(MessageDataOffset.CareOfInitCookie, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityCareOfTestInit(nextHeader, checksum, careOfInitCookie, options);
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
    public class IpV6ExtensionHeaderMobilityHomeTest : IpV6ExtensionHeaderMobility
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
    public class IpV6ExtensionHeaderMobilityCareOfTest : IpV6ExtensionHeaderMobility
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
    public class IpV6ExtensionHeaderMobilityBindingUpdate : IpV6ExtensionHeaderMobility
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

        public IpV6ExtensionHeaderMobilityBindingUpdate(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge, bool homeRegistration,
                                                        bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability, ushort lifetime,
                                                        IpV6MobilityOptions options)
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
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingUpdate; }
        }

        /// <summary>
        /// Used by the receiving node to sequence Binding Updates and by the sending node to match a returned Binding Acknowledgement with this Binding Update.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Set by the sending mobile node to request a Binding Acknowledgement be returned upon receipt of the Binding Update.
        /// </summary>
        public bool Acknowledge { get; private set; }

        /// <summary>
        /// Set by the sending mobile node to request that the receiving node should act as this node's home agent.
        /// The destination of the packet carrying this message must be that of a router sharing the same subnet prefix as the home address 
        /// of the mobile node in the binding.
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
        /// One time unit is 4 seconds.
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal static IpV6ExtensionHeaderMobilityBindingUpdate ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ushort sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            bool acknowledge = messageData.ReadBool(MessageDataOffset.Acknowledge, MessageDataMask.Acknowledge);
            bool homeRegistration = messageData.ReadBool(MessageDataOffset.HomeRegistration, MessageDataMask.HomeRegistration);
            bool linkLocalAddressCompatibility = messageData.ReadBool(MessageDataOffset.LinkLocalAddressCompatibility,
                                                                      MessageDataMask.LinkLocalAddressCompatibility);
            bool keyManagementMobilityCapability = messageData.ReadBool(MessageDataOffset.KeyManagementMobilityCapability,
                                                                        MessageDataMask.KeyManagementMobilityCapability);
            ushort lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityBindingUpdate(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration,
                                                                linkLocalAddressCompatibility, keyManagementMobilityCapability, lifetime, options);
        }
    }

    public enum IpV6BindingAcknowledgementStatus : byte
    {
        /// <summary>
        /// RFCs 5213, 6275.
        /// </summary>
        BindingUpdateAcceptedProxyBindingUpdateAccepted = 0,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        AcceptedButPrefixDiscoveryNecessary = 1,

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
        /// RFC 6275.
        /// </summary>
        ReasonUnspecified = 128,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        InsufficientResources = 130,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeRegistrationNotSupported = 131,

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
    public class IpV6ExtensionHeaderMobilityBindingAcknowledgement : IpV6ExtensionHeaderMobility
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

        public IpV6ExtensionHeaderMobilityBindingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
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
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingAcknowledgement; }
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
        /// The granted lifetime, in time units of 4 seconds, for which this node should retain the entry for this mobile node in its Binding Cache.
        /// </para>
        /// <para>
        /// The value of this field is undefined if the Status field indicates that the Binding Update was rejected.
        /// </para>
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal static IpV6ExtensionHeaderMobilityBindingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6BindingAcknowledgementStatus status = (IpV6BindingAcknowledgementStatus)messageData[MessageDataOffset.Status];
            bool keyManagementMobilityCapability = messageData.ReadBool(MessageDataOffset.KeyManagementMobilityCapability,
                                                                        MessageDataMask.KeyManagementMobilityCapability);

            ushort sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            ushort lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
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
    public class IpV6ExtensionHeaderMobilityBindingError : IpV6ExtensionHeaderMobility
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

        internal static IpV6ExtensionHeaderMobilityBindingError ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6BindingErrorStatus status = (IpV6BindingErrorStatus)messageData[MessageDataOffset.Status];
            IpV6Address homeAddress = messageData.ReadIpV6Address(MessageDataOffset.HomeAddress, Endianity.Big);
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return new IpV6ExtensionHeaderMobilityBindingError(nextHeader, checksum, status, homeAddress, options);
        }
    }
}