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

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        /// <summary>
        /// Creates an instance from next header, checksum, status, home address and options.
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
        /// <param name="status">Indicating the reason for this message.</param>
        /// <param name="homeAddress">
        /// The home address that was contained in the Home Address destination option.
        /// The mobile node uses this information to determine which binding does not exist, in cases where the mobile node has several home addresses.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityBindingError(IpV4Protocol? nextHeader, ushort checksum, IpV6BindingErrorStatus status, IpV6Address homeAddress,
                                                       IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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

        internal override int GetMessageDataHashCode()
        {
            return Sequence.GetHashCode(Status, HomeAddress);
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

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingError other)
        {
            return other != null &&
                   Status == other.Status && HomeAddress == other.HomeAddress;
        }
    }
}