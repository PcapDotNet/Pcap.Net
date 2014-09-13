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
    public sealed class IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation : IpV6ExtensionHeaderMobilityLocalizedRouting
    {
        public IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber,
                                                                     ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.LocalizedRoutingInitiation; }
        }

        internal static IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out lifetime, out options))
                return null;

            return new IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation(nextHeader, checksum, sequenceNumber, lifetime, options);
        }

        internal override bool EqualsMessageDataLocalizedRoutingExtraFields(IpV6ExtensionHeaderMobilityLocalizedRouting other)
        {
            return true;
        }

        internal override int GetMessageDataLocalizedRoutingExtraFieldsHashCode()
        {
            return 0;
        }

        internal override void WriteMessageDataBetweenSequenceNumberAndLifetime(byte[] buffer, int offset)
        {
        }
    }
}