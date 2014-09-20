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

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        /// <summary>
        /// Creates an instance from next header, checksum, care of init cookie and options.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
        /// <param name="checksum">
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </param>
        /// <param name="careOfInitCookie">Contains a random value, the care-of init cookie.</param>
        /// <param name="options">Zero or more TLV-encoded mobility options.</param>
        public IpV6ExtensionHeaderMobilityCareOfTestInit(IpV4Protocol nextHeader, ushort checksum, ulong careOfInitCookie, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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

        internal override int GetMessageDataHashCode()
        {
            return CareOfInitCookie.GetHashCode();
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
}