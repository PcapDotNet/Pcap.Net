using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4-7 | 8-15                    |
    /// +-----+---+---+---+---+-----+-------------------------+
    /// | 0   | Next Header         | Header Extension Length |
    /// +-----+---------------------+-------------------------+
    /// | 16  | MH Type             | Reserved                |
    /// +-----+---------------------+-------------------------+
    /// | 32  | Checksum                                      |
    /// +-----+---------------------+-------------------------+
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
    public sealed class IpV6ExtensionHeaderMobilityFastBindingUpdate : IpV6ExtensionHeaderMobilityBindingUpdateBase
    {
        /// <summary>
        /// Creates an instance from next header, checksum, sequence number, acknowledge, home registration, link local address compatilibity, 
        /// key management mobility capability, lifetime and options.
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
        /// Used by the receiving node to sequence Binding Updates and by the sending node to match a returned Binding Acknowledgement with this Binding Update.
        /// </param>
        /// <param name="acknowledge">
        /// Set by the sending mobile node to request a Binding Acknowledgement be returned upon receipt of the Binding Update.
        /// For Fast Binding Update this must be set to one to request that PAR send a Fast Binding Acknowledgement message.
        /// </param>
        /// <param name="homeRegistration">
        /// Set by the sending mobile node to request that the receiving node should act as this node's home agent.
        /// The destination of the packet carrying this message must be that of a router sharing the same subnet prefix as the home address 
        /// of the mobile node in the binding.
        /// For Fast Binding Update this must be set to one.
        /// </param>
        /// <param name="linkLocalAddressCompatibility">
        /// Set when the home address reported by the mobile node has the same interface identifier as the mobile node's link-local address.
        /// </param>
        /// <param name="keyManagementMobilityCapability">
        /// If this is cleared, the protocol used for establishing the IPsec security associations between the mobile node and the home agent 
        /// does not survive movements.
        /// It may then have to be rerun. (Note that the IPsec security associations themselves are expected to survive movements.)
        /// If manual IPsec configuration is used, the bit must be cleared.
        /// </param>
        /// <param name="lifetime">
        /// The number of time units remaining before the binding must be considered expired.
        /// A value of zero indicates that the Binding Cache entry for the mobile node must be deleted.
        /// One time unit is 4 seconds for Binding Update and 1 second for Fast Binding Update.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityFastBindingUpdate(IpV4Protocol? nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge,
                                                            bool homeRegistration, bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability,
                                                            ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration, linkLocalAddressCompatibility, keyManagementMobilityCapability, lifetime, options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.FastBindingUpdate; }
        }

        internal static IpV6ExtensionHeaderMobilityFastBindingUpdate ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            ushort sequenceNumber;
            bool acknowledge;
            bool homeRegistration;
            bool linkLocalAddressCompatibility;
            bool keyManagementMobilityCapability;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataToFields(messageData, out sequenceNumber, out acknowledge, out homeRegistration, out linkLocalAddressCompatibility,
                                          out keyManagementMobilityCapability, out lifetime, out options))
            {
                return null;
            }

            return new IpV6ExtensionHeaderMobilityFastBindingUpdate(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration,
                                                                    linkLocalAddressCompatibility, keyManagementMobilityCapability, lifetime, options);
        }
    }
}