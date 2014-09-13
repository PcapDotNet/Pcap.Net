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

        public IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool unsolicited, IpV6MobilityLocalizedRoutingAcknowledgementStatus status,
                                                                          ushort lifetime, IpV6MobilityOptions options)
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