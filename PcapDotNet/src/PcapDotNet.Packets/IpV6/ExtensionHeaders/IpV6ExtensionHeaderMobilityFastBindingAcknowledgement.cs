using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5568.
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
    public class IpV6ExtensionHeaderMobilityFastBindingAcknowledgement : IpV6ExtensionHeaderMobilityBindingAcknowledgementBase
    {
        /// <summary>
        /// Creates an instance from next header, checksum, status, key management mobility capability, sequence number, lifetime and options.
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
        /// <param name="status">
        /// Indicating the disposition of the Binding Update.
        /// Values of the Status field less than 128 indicate that the Binding Update was accepted by the receiving node.
        /// Values greater than or equal to 128 indicate that the Binding Update was rejected by the receiving node.
        /// </param>
        /// <param name="keyManagementMobilityCapability">
        /// If this is cleared, the protocol used by the home agent for establishing the IPsec security associations between the mobile node and the home agent
        /// does not survive movements.
        /// It may then have to be rerun.
        /// (Note that the IPsec security associations themselves are expected to survive movements.)
        /// </param>
        /// <param name="sequenceNumber">
        /// Copied from the Sequence Number field in the Binding Update.
        /// It is used by the mobile node in matching this Binding Acknowledgement with an outstanding Binding Update.
        /// </param>
        /// <param name="lifetime">
        /// The granted lifetime, in time units of 4 seconds for Binding Acknowledgement and 1 second for Fast Binding Acknowledgement, 
        /// for which this node should retain the entry for this mobile node in its Binding Cache.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityFastBindingAcknowledgement(IpV4Protocol? nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
                                                                     bool keyManagementMobilityCapability, ushort sequenceNumber, ushort lifetime,
                                                                     IpV6MobilityOptions options)
            : base(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.FastBindingAcknowledgement; }
        }

        internal static IpV6ExtensionHeaderMobilityFastBindingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            IpV6BindingAcknowledgementStatus status;
            bool keyManagementMobilityCapability;
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataFields(messageData, out status, out keyManagementMobilityCapability, out sequenceNumber, out lifetime, out options))
                return null;

            return new IpV6ExtensionHeaderMobilityFastBindingAcknowledgement(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber,
                                                                             lifetime, options);
        }
    }
}