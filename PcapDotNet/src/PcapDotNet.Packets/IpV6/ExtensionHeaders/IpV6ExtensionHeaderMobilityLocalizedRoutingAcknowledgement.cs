using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC-ietf-netext-pmip-lr-10.
    /// <pre>
    /// +-----+---+----------+-------------------------+
    /// | Bit | 0 | 3-7      | 8-15                    |
    /// +-----+---+----------+-------------------------+
    /// | 0   | Next Header  | Header Extension Length |
    /// +-----+--------------+-------------------------+
    /// | 16  | MH Type      | Reserved                |
    /// +-----+--------------+-------------------------+
    /// | 32  | Checksum                               |
    /// +-----+----------------------------------------+
    /// | 48  | Sequence #                             |
    /// +-----+---+----------+-------------------------+
    /// | 64  | U | Reserved | Status                  |
    /// +-----+---+----------+-------------------------+
    /// | 80  | Lifetime                               |
    /// +-----+----------------------------------------+
    /// | 96  | Mobility Options                       |
    /// | ... |                                        |
    /// +-----+----------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement : IpV6ExtensionHeaderMobilityLocalizedRouting
    {
        private static class MessageDataOffset
        {
            public const int Unsolicited = sizeof(ushort);
            public const int Status = Unsolicited + sizeof(byte);
        }

        private static class MessageDataMask
        {
            public const byte Unsolicited = 0x80;
        }

        /// <summary>
        /// Creates an instance from next header, checksum, sequence number, unsolicited, status, lifetime and options.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
        /// <param name="checksum">
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </param>
        /// <param name="sequenceNumber">Copied from the sequence number field of the LRI message being responded to.</param>
        /// <param name="unsolicited">
        /// When true, the LRA message is sent unsolicited.
        /// The Lifetime field indicates a new requested value.
        /// The MAG must wait for the regular LRI message to confirm that the request is acceptable to the LMA.
        /// </param>
        /// <param name="status">The acknowledgement status.</param>
        /// <param name="lifetime">
        /// The time in seconds for which the local forwarding is supported.
        /// Typically copied from the corresponding field in the LRI message.
        /// </param>
        /// <param name="options">Zero or more TLV-encoded mobility options.</param>
        public IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement(IpV4Protocol? nextHeader, ushort checksum, ushort sequenceNumber, bool unsolicited,
                                                                          IpV6MobilityLocalizedRoutingAcknowledgementStatus status, ushort lifetime,
                                                                          IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, lifetime, options)
        {
            Status = status;
            Unsolicited = unsolicited;
        }

        /// <summary>
        /// When true, the LRA message is sent unsolicited.
        /// The Lifetime field indicates a new requested value.
        /// The MAG must wait for the regular LRI message to confirm that the request is acceptable to the LMA.
        /// </summary>
        public bool Unsolicited { get; private set; }

        /// <summary>
        /// The acknowledgement status.
        /// </summary>
        public IpV6MobilityLocalizedRoutingAcknowledgementStatus Status { get; private set; }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement; }
        }

        internal static IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out lifetime, out options))
                return null;

            bool unsolicited = messageData.ReadBool(MessageDataOffset.Unsolicited, MessageDataMask.Unsolicited);
            IpV6MobilityLocalizedRoutingAcknowledgementStatus status = (IpV6MobilityLocalizedRoutingAcknowledgementStatus)messageData[MessageDataOffset.Status];

            return new IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement(nextHeader, checksum, sequenceNumber, unsolicited, status, lifetime, options);
        }

        internal override bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRouting other)
        {
            return EqualsMessageDataLocalizedRoutingExtraFields(other as IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement);
        }

        internal override int GetMessageDataLocalizedRoutingExtraFieldsHashCode()
        {
            return BitSequence.Merge((byte)Status, Unsolicited.ToByte()).GetHashCode();
        }

        internal override void WriteMessageDataBetweenSequenceNumberAndLifetime(byte[] buffer, int offset)
        {
            if (Unsolicited)
                buffer.Write(offset + MessageDataOffset.Unsolicited, MessageDataMask.Unsolicited);
            buffer.Write(offset + MessageDataOffset.Status, (byte)Status);
        }

        private bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement other)
        {
            return other != null &&
                   Unsolicited == other.Unsolicited && Status == other.Status;
        }
    }
}