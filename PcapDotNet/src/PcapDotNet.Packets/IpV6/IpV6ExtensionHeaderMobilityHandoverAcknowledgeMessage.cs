using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
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
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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
}