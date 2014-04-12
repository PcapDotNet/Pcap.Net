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
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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
            ulong homeInitCookie = messageData.ReadULong(MessageDataOffset.HomeInitCookie, Endianity.Big);
            ulong homeKeygenToken = messageData.ReadULong(MessageDataOffset.HomeKeygenToken, Endianity.Big);
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
}