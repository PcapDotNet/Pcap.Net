using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
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

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        /// <summary>
        /// Creates an instance from next header, checksum, sequence number, assigned address configuration, buffer, code and options.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
        /// <param name="checksum">
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </param>
        /// <param name="sequenceNumber">Must be set by the sender so replies can be matched to this message.</param>
        /// <param name="assignedAddressConfiguration">
        /// Assigned address configuration flag.  
        /// When set to true, this message requests a new CoA to be returned by the destination.
        /// May be set when Code = 0. Must be false when Code = 1.
        /// </param>
        /// <param name="buffer">
        /// When set, the destination should buffer any packets toward the node indicated in the options of this message.
        /// Used when Code = 0, should be set to false when Code = 1.
        /// </param>
        /// <param name="code">Describes whether the source ip address is a previous care of address.</param>
        /// <param name="options">Zero or more TLV-encoded mobility options.</param>
        public IpV6ExtensionHeaderMobilityHandoverInitiateMessage(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool assignedAddressConfiguration,
                                                                  bool buffer, IpV6HandoverInitiateMessageCode code, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
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

        internal override int GetMessageDataHashCode()
        {
            return BitSequence.Merge(SequenceNumber, (byte)Code, BitSequence.Merge(AssignedAddressConfiguration, Buffer)).GetHashCode();
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
}