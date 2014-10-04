using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC-ietf-netext-pmip-lr-10.
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
    /// +-----+---------------------------------------+
    /// | 64  | Reserved                              |
    /// +-----+---------------------------------------+
    /// | 80  | Lifetime                              |
    /// +-----+---------------------------------------+
    /// | 96  | Mobility Options                      |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityLocalizedRouting : IpV6ExtensionHeaderMobility
    {
        /// <summary>
        /// Indicates an infinite lifetime.
        /// </summary>
        public const ushort LifetimeInfinite = 0xFFFF;

        private static class MessageDataOffset
        {
            public const int SequenceNumber = 0;
            public const int Lifetime = SequenceNumber + sizeof(ushort) + sizeof(ushort);
            public const int Options = Lifetime + sizeof(ushort);
        }

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        /// <summary>
        /// In initiation, a monotonically increasing integer. Set by a sending node in a request message, and used to match a reply to the request.
        /// In acknowledgement, copied from the sequence number field of the LRI message being responded to.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// In initiation, the requested time in seconds for which the sender wishes to have local forwarding.
        /// A value of 0xffff (all ones) indicates an infinite lifetime.
        /// When set to 0, indicates a request to stop localized routing.
        /// In acknowledgement, the time in seconds for which the local forwarding is supported.
        /// Typically copied from the corresponding field in the LRI message.
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal IpV6ExtensionHeaderMobilityLocalizedRouting(IpV4Protocol? nextHeader, ushort checksum, ushort sequenceNumber, ushort lifetime,
                                                           IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
        {
            SequenceNumber = sequenceNumber;
            Lifetime = lifetime;
        }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal sealed override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityLocalizedRouting);
        }

        internal abstract bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRouting other);

        internal sealed override int GetMessageDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(SequenceNumber, Lifetime), GetMessageDataLocalizedRoutingExtraFieldsHashCode());
        }

        internal abstract int GetMessageDataLocalizedRoutingExtraFieldsHashCode();

        internal static bool ParseMessageDataToFields(DataSegment messageData, out ushort sequenceNumber, out ushort lifetime, out IpV6MobilityOptions options)
        {
            if (messageData.Length < MinimumMessageDataLength)
            {
                sequenceNumber = 0;
                lifetime = 0;
                options = null;
                return false;
            }

            sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return true;
        }

        internal sealed override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            WriteMessageDataBetweenSequenceNumberAndLifetime(buffer, offset);
            buffer.Write(offset + MessageDataOffset.Lifetime, Lifetime, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        internal abstract void WriteMessageDataBetweenSequenceNumberAndLifetime(byte[] buffer, int offset);

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityLocalizedRouting other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && Lifetime == other.Lifetime && EqualsMessageDataLocalizedRoutingExtraFields(other);
        }
    }
}