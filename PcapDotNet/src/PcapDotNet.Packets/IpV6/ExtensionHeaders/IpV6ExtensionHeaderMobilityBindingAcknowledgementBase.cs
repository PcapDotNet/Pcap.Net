using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFCs 5568, 6275.
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
    public abstract class IpV6ExtensionHeaderMobilityBindingAcknowledgementBase : IpV6ExtensionHeaderMobility
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

        public IpV6ExtensionHeaderMobilityBindingAcknowledgementBase(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
                                                                     bool keyManagementMobilityCapability, ushort sequenceNumber, ushort lifetime,
                                                                     IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
        {
            Status = status;
            KeyManagementMobilityCapability = keyManagementMobilityCapability;
            SequenceNumber = sequenceNumber;
            Lifetime = lifetime;
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
        /// The granted lifetime, in time units of 4 seconds for Binding Acknowledgement and 1 second for Fast Binding Acknowledgement, 
        /// for which this node should retain the entry for this mobile node in its Binding Cache.
        /// </para>
        /// <para>
        /// The value of this field is undefined if the Status field indicates that the Binding Update was rejected.
        /// </para>
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingAcknowledgementBase);
        }

        internal static bool ParseMessageDataFields(DataSegment messageData, out IpV6BindingAcknowledgementStatus status,
                                                    out bool keyManagementMobilityCapability, out ushort sequenceNumber, out ushort lifetime,
                                                    out IpV6MobilityOptions options)
        {
            if (messageData.Length < MinimumMessageDataLength)
            {
                status = IpV6BindingAcknowledgementStatus.BindingUpdateAccepted;
                keyManagementMobilityCapability = false;
                sequenceNumber = 0;
                lifetime = 0;
                options = null;
                return false;
            }

            status = (IpV6BindingAcknowledgementStatus)messageData[MessageDataOffset.Status];
            keyManagementMobilityCapability = messageData.ReadBool(MessageDataOffset.KeyManagementMobilityCapability,
                                                                   MessageDataMask.KeyManagementMobilityCapability);

            sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return true;
        }

        internal sealed override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.Status, (byte)Status);
            if (KeyManagementMobilityCapability)
                buffer.Write(offset + MessageDataOffset.KeyManagementMobilityCapability, MessageDataMask.KeyManagementMobilityCapability);
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            buffer.Write(offset + MessageDataOffset.Lifetime, Lifetime, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingAcknowledgementBase other)
        {
            return other != null &&
                   Status == other.Status && KeyManagementMobilityCapability == other.KeyManagementMobilityCapability && SequenceNumber == other.SequenceNumber &&
                   Lifetime == other.Lifetime;
        }
    }
}