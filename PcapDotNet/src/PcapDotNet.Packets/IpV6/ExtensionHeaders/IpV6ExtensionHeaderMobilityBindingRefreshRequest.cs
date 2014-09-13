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
    /// | 64  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingRefreshRequest : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int Options = 2;
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        public IpV6ExtensionHeaderMobilityBindingRefreshRequest(IpV4Protocol nextHeader, ushort checksum, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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

        internal override int GetMessageDataHashCode()
        {
            return 0;
        }
    }
}