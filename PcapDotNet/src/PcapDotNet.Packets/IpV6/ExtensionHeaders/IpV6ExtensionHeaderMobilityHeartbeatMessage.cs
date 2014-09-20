using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
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

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.MobilityOptions;

        /// <summary>
        /// Creates an instance from next header, checksum, is unsolicited heartbeat response, is response, sequence number and options.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
        /// <param name="checksum">
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </param>
        /// <param name="isUnsolicitedHeartbeatResponse">Set to true in Unsolicited Heartbeat Response.</param>
        /// <param name="isResponse">
        /// Indicates whether the message is a request or a response. 
        /// When it's set to false, it indicates that the Heartbeat message is a request.
        /// When it's set to true, it indicates that the Heartbeat message is a response.
        /// </param>
        /// <param name="sequenceNumber">Sequence number used for matching the request to the reply.</param>
        /// <param name="options">Zero or more TLV-encoded mobility options.</param>
        public IpV6ExtensionHeaderMobilityHeartbeatMessage(IpV4Protocol nextHeader, ushort checksum, bool isUnsolicitedHeartbeatResponse, bool isResponse,
                                                           uint sequenceNumber, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.MobilityOptions)
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

        internal override int GetMessageDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(IsUnsolicitedHeartbeatResponse, IsResponse), SequenceNumber);
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
}