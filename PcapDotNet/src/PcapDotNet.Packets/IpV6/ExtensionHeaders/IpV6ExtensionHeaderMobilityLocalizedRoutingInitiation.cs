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
        /// <summary>
        /// Creates an instance from next header, checksum, sequence number, lifetime and options.
        /// </summary>
        /// <param name="nextHeader">
        /// Identifies the type of header immediately following this extension header.
        /// </param>
        /// <param name="checksum">
        /// Contains the checksum of the Mobility Header.
        /// The checksum is calculated from the octet string consisting of a "pseudo-header"
        /// followed by the entire Mobility Header starting with the Payload Proto field.
        /// The checksum is the 16-bit one's complement of the one's complement sum of this string.
        /// </param>
        /// <param name="sequenceNumber">
        /// A monotonically increasing integer. Set by a sending node in a request message, and used to match a reply to the request.
        /// </param>
        /// <param name="lifetime">
        /// The requested time in seconds for which the sender wishes to have local forwarding.
        /// A value of 0xffff (all ones) indicates an infinite lifetime.
        /// When set to 0, indicates a request to stop localized routing.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation(IpV4Protocol? nextHeader, ushort checksum, ushort sequenceNumber,
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