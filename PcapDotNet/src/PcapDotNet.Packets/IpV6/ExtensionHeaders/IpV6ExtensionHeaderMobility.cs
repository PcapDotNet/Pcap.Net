using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 3775, 6275.
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
    /// | 48  | Message Data                          |
    /// | ... | ends with Mobility Options            |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderMobility : IpV6ExtensionHeaderStandard
    {
        /// <summary>
        /// Identifies the type of this extension header.
        /// </summary>
        public sealed override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.MobilityHeader; }
        }

        /// <summary>
        /// True iff the extension header parsing didn't encounter an issue.
        /// </summary>
        public sealed override bool IsValid
        {
            get { return MobilityOptions.IsValid; }
        }

        private static class DataOffset
        {
            public const int MobilityHeaderType = 0;
            public const int Checksum = MobilityHeaderType + sizeof(byte) + sizeof(byte);
            public const int MessageData = Checksum + sizeof(ushort);
        }

        /// <summary>
        /// The minimum number of bytes this extension header takes.
        /// </summary>
        public const int MinimumDataLength = DataOffset.MessageData;

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public abstract IpV6MobilityHeaderType MobilityHeaderType { get; }

        /// <summary>
        /// <para>
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </para>
        /// <para>
        /// The pseudo-header contains IPv6 header fields.
        /// The Next Header value used in the pseudo-header is 135.
        /// The addresses used in the pseudo-header are the addresses that appear in the Source and Destination Address fields in the IPv6 packet 
        /// carrying the Mobility Header.
        /// </para>
        /// <para>
        /// Note that the procedures of calculating upper-layer checksums while away from home apply even for the Mobility Header.
        /// If a mobility message has a Home Address destination option, then the checksum calculation uses the home address in this option as the value of the IPv6 Source Address field.
        /// </para>
        /// <para>
        /// The Mobility Header is considered as the upper-layer protocol for the purposes of calculating the pseudo-header.
        /// The Upper-Layer Packet Length field in the pseudo-header MUST be set to the total length of the Mobility Header.
        /// </para>
        /// <para>
        /// For computing the checksum, the checksum field is set to zero.
        /// </para>
        /// </summary>
        public ushort Checksum { get; private set; }

        /// <summary>
        /// Zero or more TLV-encoded mobility options.
        /// </summary>
        public IpV6MobilityOptions MobilityOptions { get; private set; }

        internal IpV6ExtensionHeaderMobility(IpV4Protocol? nextHeader, ushort checksum, IpV6MobilityOptions mobilityOptions, int? messageDataMobilityOptionsOffset)
            : base(nextHeader)
        {
            if (messageDataMobilityOptionsOffset.HasValue)
            {
                int mobilityOptionsExtraBytes = (8 - (messageDataMobilityOptionsOffset.Value + 6) % 8) % 8;
                if (mobilityOptions.BytesLength % 8 != mobilityOptionsExtraBytes)
                    mobilityOptions = mobilityOptions.Pad((8 + mobilityOptionsExtraBytes - (mobilityOptions.BytesLength % 8)) % 8);
            }
            Checksum = checksum;
            MobilityOptions = mobilityOptions;
        }

        internal static IpV6ExtensionHeaderMobility ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            if (data.Length < MinimumDataLength)
                return null;

            IpV6MobilityHeaderType mobilityHeaderType = (IpV6MobilityHeaderType)data[DataOffset.MobilityHeaderType];
            ushort checksum = data.ReadUShort(DataOffset.Checksum, Endianity.Big);
            DataSegment messageData = data.Subsegment(DataOffset.MessageData, data.Length - DataOffset.MessageData);

            switch (mobilityHeaderType)
            {
                case IpV6MobilityHeaderType.BindingRefreshRequest: // 0
                    return IpV6ExtensionHeaderMobilityBindingRefreshRequest.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HomeTestInit: // 1
                    return IpV6ExtensionHeaderMobilityHomeTestInit.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.CareOfTestInit: // 2
                    return IpV6ExtensionHeaderMobilityCareOfTestInit.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HomeTest: // 3
                    return IpV6ExtensionHeaderMobilityHomeTest.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.CareOfTest: // 4
                    return IpV6ExtensionHeaderMobilityCareOfTest.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingUpdate: // 5
                    return IpV6ExtensionHeaderMobilityBindingUpdate.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingAcknowledgement: // 6
                    return IpV6ExtensionHeaderMobilityBindingAcknowledgement.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingError: // 7
                    return IpV6ExtensionHeaderMobilityBindingError.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.FastBindingUpdate: // 8
                    return IpV6ExtensionHeaderMobilityFastBindingUpdate.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.FastBindingAcknowledgement: // 9
                    return IpV6ExtensionHeaderMobilityFastBindingAcknowledgement.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.FastNeighborAdvertisement: // 10
                    return IpV6ExtensionHeaderMobilityFastNeighborAdvertisement.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.Experimental: // 11
                    return IpV6ExtensionHeaderMobilityExperimental.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HomeAgentSwitchMessage: // 12
                    return IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HeartbeatMessage: // 13
                    return IpV6ExtensionHeaderMobilityHeartbeatMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HandoverInitiateMessage: // 14
                    return IpV6ExtensionHeaderMobilityHandoverInitiateMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.HandoverAcknowledgeMessage: // 15
                    return IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.BindingRevocationMessage: // 16
                    return IpV6ExtensionHeaderMobilityBindingRevocationMessage.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.LocalizedRoutingInitiation: // 17
                    return IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation.ParseMessageData(nextHeader, checksum, messageData);

                case IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement: // 18
                    return IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement.ParseMessageData(nextHeader, checksum, messageData);

                default:
                    return null;
            }
        }

        internal sealed override void WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + DataOffset.MobilityHeaderType, (byte)MobilityHeaderType);
            buffer.Write(offset + DataOffset.Checksum, Checksum, Endianity.Big);
            WriteMessageData(buffer, offset + DataOffset.MessageData);
        }

        internal abstract void WriteMessageData(byte[] buffer, int offset);

        internal sealed override int DataLength
        {
            get { return MinimumDataLength + MessageDataLength; }
        }

        internal abstract int MessageDataLength { get; }

        internal sealed override bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderMobility);
        }

        internal sealed override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(Checksum, (byte)MobilityHeaderType), MobilityOptions, GetMessageDataHashCode());
        }

        internal abstract bool EqualsMessageData(IpV6ExtensionHeaderMobility other);

        internal abstract int GetMessageDataHashCode();

        private bool EqualsData(IpV6ExtensionHeaderMobility other)
        {
            return other != null &&
                   MobilityHeaderType == other.MobilityHeaderType && Checksum == other.Checksum && MobilityOptions.Equals(other.MobilityOptions) &&
                   EqualsMessageData(other);
        }
    }
}