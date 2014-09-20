using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFCs 5568, 6275.
    /// <pre>
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4-7 | 8-15                    |
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | 0   | Next Header         | Header Extension Length |
    /// +-----+---------------------+-------------------------+
    /// | 16  | MH Type             | Reserved                |
    /// +-----+---------------------+-------------------------+
    /// | 32  | Checksum                                      |
    /// +-----+-----------------------------------------------+
    /// | 48  | Sequence #                                    |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 64  | A | H | L | K | Reserved                      |
    /// +-----+---+---+---+---+-------------------------------+
    /// | 80  | Lifetime                                      |
    /// +-----+-----------------------------------------------+
    /// | 96  | Mobility Options                              |
    /// | ... |                                               |
    /// +-----+-----------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityBindingUpdateBase : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int SequenceNumber = 0;
            public const int Acknowledge = SequenceNumber + sizeof(ushort);
            public const int HomeRegistration = Acknowledge;
            public const int LinkLocalAddressCompatibility = HomeRegistration;
            public const int KeyManagementMobilityCapability = LinkLocalAddressCompatibility;
            public const int Lifetime = KeyManagementMobilityCapability + sizeof(byte) + sizeof(byte);
            public const int Options = Lifetime + sizeof(ushort);
        }

        private static class MessageDataMask
        {
            public const byte Acknowledge = 0x80;
            public const byte HomeRegistration = 0x40;
            public const byte LinkLocalAddressCompatibility = 0x20;
            public const byte KeyManagementMobilityCapability = 0x10;
        }

        /// <summary>
        /// The minimum number of bytes the message data takes.
        /// </summary>
        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        /// <summary>
        /// Used by the receiving node to sequence Binding Updates and by the sending node to match a returned Binding Acknowledgement with this Binding Update.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// Set by the sending mobile node to request a Binding Acknowledgement be returned upon receipt of the Binding Update.
        /// For Fast Binding Update this must be set to one to request that PAR send a Fast Binding Acknowledgement message.
        /// </summary>
        public bool Acknowledge { get; private set; }

        /// <summary>
        /// Set by the sending mobile node to request that the receiving node should act as this node's home agent.
        /// The destination of the packet carrying this message must be that of a router sharing the same subnet prefix as the home address 
        /// of the mobile node in the binding.
        /// For Fast Binding Update this must be set to one.
        /// </summary>
        public bool HomeRegistration { get; private set; }

        /// <summary>
        /// Set when the home address reported by the mobile node has the same interface identifier as the mobile node's link-local address.
        /// </summary>
        public bool LinkLocalAddressCompatibility { get; private set; }

        /// <summary>
        /// <para>
        /// If this is cleared, the protocol used for establishing the IPsec security associations between the mobile node and the home agent 
        /// does not survive movements.
        /// It may then have to be rerun. (Note that the IPsec security associations themselves are expected to survive movements.)
        /// If manual IPsec configuration is used, the bit must be cleared.
        /// </para>
        /// <para>
        /// This bit is valid only in Binding Updates sent to the home agent, and mustbe cleared in other Binding Updates.
        /// Correspondent nodes must ignore this bit.
        /// </para>
        /// </summary>
        public bool KeyManagementMobilityCapability { get; private set; }

        /// <summary>
        /// The number of time units remaining before the binding must be considered expired.
        /// A value of zero indicates that the Binding Cache entry for the mobile node must be deleted.
        /// One time unit is 4 seconds for Binding Update and 1 second for Fast Binding Update.
        /// </summary>
        public ushort Lifetime { get; private set; }

        internal IpV6ExtensionHeaderMobilityBindingUpdateBase(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge,
                                                              bool homeRegistration, bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability,
                                                              ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
        {
            SequenceNumber = sequenceNumber;
            Acknowledge = acknowledge;
            HomeRegistration = homeRegistration;
            LinkLocalAddressCompatibility = linkLocalAddressCompatibility;
            KeyManagementMobilityCapability = keyManagementMobilityCapability;
            Lifetime = lifetime;
        }

        internal sealed override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingUpdateBase);
        }

        internal sealed override int GetMessageDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(SequenceNumber, Lifetime),
                                        BitSequence.Merge(Acknowledge, HomeRegistration, LinkLocalAddressCompatibility, KeyManagementMobilityCapability));
        }

        internal sealed override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal static bool ParseMessageDataToFields(DataSegment messageData, out ushort sequenceNumber,
                                                      out bool acknowledge, out bool homeRegistration, out bool linkLocalAddressCompatibility,
                                                      out bool keyManagementMobilityCapability, out ushort lifetime, out IpV6MobilityOptions options)
        {
            if (messageData.Length < MinimumMessageDataLength)
            {
                sequenceNumber = 0;
                acknowledge = false;
                homeRegistration = false;
                linkLocalAddressCompatibility = false;
                keyManagementMobilityCapability = false;
                lifetime = 0;
                options = null;
                return false;
            }

            sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            acknowledge = messageData.ReadBool(MessageDataOffset.Acknowledge, MessageDataMask.Acknowledge);
            homeRegistration = messageData.ReadBool(MessageDataOffset.HomeRegistration, MessageDataMask.HomeRegistration);
            linkLocalAddressCompatibility = messageData.ReadBool(MessageDataOffset.LinkLocalAddressCompatibility, MessageDataMask.LinkLocalAddressCompatibility);
            keyManagementMobilityCapability = messageData.ReadBool(MessageDataOffset.KeyManagementMobilityCapability,
                                                                   MessageDataMask.KeyManagementMobilityCapability);
            lifetime = messageData.ReadUShort(MessageDataOffset.Lifetime, Endianity.Big);
            options = new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            return true;
        }

        internal sealed override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);

            byte flags = 0;
            if (Acknowledge)
                flags |= MessageDataMask.Acknowledge;
            if (HomeRegistration)
                flags |= MessageDataMask.HomeRegistration;
            if (LinkLocalAddressCompatibility)
                flags |= MessageDataMask.LinkLocalAddressCompatibility;
            if (KeyManagementMobilityCapability)
                flags |= MessageDataMask.KeyManagementMobilityCapability;
            buffer.Write(offset + MessageDataOffset.Acknowledge, flags);

            buffer.Write(offset + MessageDataOffset.Lifetime, Lifetime, Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingUpdateBase other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && Acknowledge == other.Acknowledge && HomeRegistration == other.HomeRegistration &&
                   LinkLocalAddressCompatibility == other.LinkLocalAddressCompatibility &&
                   KeyManagementMobilityCapability == other.KeyManagementMobilityCapability && Lifetime == other.Lifetime;
        }

    }
}