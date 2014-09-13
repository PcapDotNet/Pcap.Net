using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
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
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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

        internal override int GetMessageDataHashCode()
        {
            return Sequence.GetHashCode(CareOfNonceIndex, CareOfInitCookie, CareOfKeygenToken);
        }

        internal static IpV6ExtensionHeaderMobilityCareOfTest ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            ushort careOfNonceIndex = messageData.ReadUShort(MessageDataOffset.CareOfNonceIndex, Endianity.Big);
            ulong careOfInitCookie = messageData.ReadULong(MessageDataOffset.CareOfInitCookie, Endianity.Big);
            ulong careOfKeygenToken = messageData.ReadULong(MessageDataOffset.CareOfKeygenToken, Endianity.Big);
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
}