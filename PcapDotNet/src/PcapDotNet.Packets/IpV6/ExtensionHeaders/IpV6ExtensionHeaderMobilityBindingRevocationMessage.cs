using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5846.
    /// <pre>
    /// +-----+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3-7 | 8-15                    |
    /// +-----+---+---+---+-----+-------------------------+
    /// | 0   | Next Header     | Header Extension Length |
    /// +-----+-----------------+-------------------------+
    /// | 16  | MH Type         | Reserved                |
    /// +-----+-----------------+-------------------------+
    /// | 32  | Checksum                                  |
    /// +-----+-----------------+-------------------------+
    /// | 48  | B.R. Type       | R. Trigger or Status    |
    /// +-----+-----------------+-------------------------+
    /// | 64  | Sequence #                                |
    /// +-----+---+---+---+-------------------------------+
    /// | 80  | P | V | G | Reserved                      |
    /// +-----+---+---+---+-------------------------------+
    /// | 96  | Mobility Options                          |
    /// | ... |                                           |
    /// +-----+-------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobilityBindingRevocationMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int BindingRevocationType = 0;
            public const int RevocationTriggerOrStatus = BindingRevocationType + sizeof(byte);
            public const int SequenceNumber = RevocationTriggerOrStatus + sizeof(byte);
            public const int ProxyBinding = SequenceNumber + sizeof(ushort);
            public const int IpV4HomeAddressBindingOnly = ProxyBinding;
            public const int Global = IpV4HomeAddressBindingOnly;
            public const int Options = Global + sizeof(byte) + sizeof(byte);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.Options;

        private static class MessageDataMask
        {
            public const byte ProxyBinding = 0x80;
            public const byte IpV4HomeAddressBindingOnly = 0x40;
            public const byte Global = 0x20;
        }

        public IpV6ExtensionHeaderMobilityBindingRevocationMessage(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool proxyBinding,
                                                                   bool ipV4HomeAddressBindingOnly, bool global, IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.Options)
        {
            SequenceNumber = sequenceNumber;
            ProxyBinding = proxyBinding;
            IpV4HomeAddressBindingOnly = ipV4HomeAddressBindingOnly;
            Global = global;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public sealed override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingRevocationMessage; }
        }

        /// <summary>
        /// Defines the type of the Binding Revocation Message.
        /// </summary>
        public abstract IpV6MobilityBindingRevocationType BindingRevocationType { get; }

        /// <summary>
        /// In indication, used by the initiator to match a returned Binding Revocation Acknowledgement with this Binding Revocation Indication.
        /// This sequence number could be a random number.
        /// At any time, implementations must ensure there is no collision between the sequence numbers of all outstanding Binding Revocation Indication 
        /// Messages.
        /// In acknowledgement, copied from the Sequence Number field in the Binding Revocation Indication.
        /// It is used by the initiator, e.g., HA, LMA, MAG, in matching this Binding Revocation Acknowledgement 
        /// with the outstanding Binding Revocation Indication.
        /// </summary>
        public ushort SequenceNumber { get; private set; }

        /// <summary>
        /// In indication, set by the initiator to indicate that the revoked binding(s) is a PMIPv6 binding.
        /// In acknowledgement, set if set in the corresponding Binding Revocation Indication message.
        /// </summary>
        public bool ProxyBinding { get; private set; }

        /// <summary>
        /// In indication, Set by the initiator, home agent, or local mobility anchor to indicate to the receiving mobility entity the termination
        /// of the IPv4 Home Address binding only as in Home Agent Operation and Local Mobility Anchor Operation.
        /// In acknowledgement, set if the it is set in the corresponding Binding Revocation Indication message.
        /// </summary>
        public bool IpV4HomeAddressBindingOnly { get; private set; }

        /// <summary>
        /// In indication, Set by the initiator, LMA or MAG, to indicate the termination of all Per-Peer mobility Bindings or Multiple Bindings that share 
        /// a common identifier(s) and are served by the initiator and responder as in Local Mobility Anchor Operation and Mobile Access Gateway Operation.
        /// In acknowledgement, set if it is set in the corresponding Binding Revocation Indication message.
        /// </summary>
        public bool Global { get; private set; }

        internal abstract byte RevocationTriggerOrStatus { get; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityBindingRevocationMessage);
        }

        internal static IpV6ExtensionHeaderMobilityBindingRevocationMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            IpV6MobilityBindingRevocationType bindingRevocationType = (IpV6MobilityBindingRevocationType)messageData[MessageDataOffset.BindingRevocationType];
            byte revocationTriggerOrStatus = messageData[MessageDataOffset.RevocationTriggerOrStatus];
            ushort sequenceNumber = messageData.ReadUShort(MessageDataOffset.SequenceNumber, Endianity.Big);
            bool proxyBinding = messageData.ReadBool(MessageDataOffset.ProxyBinding, MessageDataMask.ProxyBinding);
            bool ipV4HomeAddressBindingOnly = messageData.ReadBool(MessageDataOffset.IpV4HomeAddressBindingOnly, MessageDataMask.IpV4HomeAddressBindingOnly);
            bool global = messageData.ReadBool(MessageDataOffset.Global, MessageDataMask.Global);
            IpV6MobilityOptions options =
                new IpV6MobilityOptions(messageData.Subsegment(MessageDataOffset.Options, messageData.Length - MessageDataOffset.Options));
            switch (bindingRevocationType)
            {
                case IpV6MobilityBindingRevocationType.BindingRevocationIndication:
                    return new IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage(nextHeader, checksum, (Ipv6MobilityBindingRevocationTrigger)revocationTriggerOrStatus, sequenceNumber,
                                                                                             proxyBinding, ipV4HomeAddressBindingOnly, global, options);

                case IpV6MobilityBindingRevocationType.BindingRevocationAcknowledgement:
                    return new IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage(nextHeader, checksum,
                                                                                                  (Ipv6MobilityBindingRevocationStatus)revocationTriggerOrStatus,
                                                                                                  sequenceNumber, proxyBinding, ipV4HomeAddressBindingOnly,
                                                                                                  global, options);

                default:
                    return null;
            }
        }

        internal sealed override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.BindingRevocationType, (byte)BindingRevocationType);
            buffer.Write(offset + MessageDataOffset.RevocationTriggerOrStatus, RevocationTriggerOrStatus);
            buffer.Write(offset + MessageDataOffset.SequenceNumber, SequenceNumber, Endianity.Big);
            
            byte flags = 0;
            if (ProxyBinding)
                flags |= MessageDataMask.ProxyBinding;
            if (IpV4HomeAddressBindingOnly)
                flags |= MessageDataMask.IpV4HomeAddressBindingOnly;
            if (Global)
                flags |= MessageDataMask.Global;
            buffer.Write(offset + MessageDataOffset.ProxyBinding, flags);

            MobilityOptions.Write(buffer, offset + MessageDataOffset.Options);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityBindingRevocationMessage other)
        {
            return other != null &&
                   BindingRevocationType == other.BindingRevocationType && RevocationTriggerOrStatus == other.RevocationTriggerOrStatus &&
                   SequenceNumber == other.SequenceNumber && ProxyBinding == other.ProxyBinding &&
                   IpV4HomeAddressBindingOnly == other.IpV4HomeAddressBindingOnly && Global == other.Global;
        }
    }
}