using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
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
    /// +-----+---------------------------------------+
    /// | 48  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 64  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityFastNeighborAdvertisement : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int Options = sizeof(ushort);
        }

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        /// <summary>
        /// Creates an instance from next header, checksum and options.
        /// </summary>
        /// <param name="nextHeader">
        /// Identifies the type of header immediately following this extension header.
        /// </param>
        /// <param name="checksum">
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityFastNeighborAdvertisement(IpV4Protocol nextHeader, ushort checksum, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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

        internal override int GetMessageDataHashCode()
        {
            return 0;
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
}