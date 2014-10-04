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
    /// | 48  | B.R. Type       | Status                  |
    /// +-----+-----------------+-------------------------+
    /// | 64  | Sequence #                                |
    /// +-----+---+---+---+-------------------------------+
    /// | 80  | P | V | G | Reserved                      |
    /// +-----+---+---+---+-------------------------------+
    /// | 96  | Mobility options                          |
    /// | ... |                                           |
    /// +-----+-------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage : IpV6ExtensionHeaderMobilityBindingRevocationMessage
    {
        /// <summary>
        /// Creates an instance from next header, checksum, status, sequence number, proxy binding, IPv4 home address binding only, global and options.
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
        /// Indicating the result of processing the Binding Revocation Indication message by the responder.
        /// </param>
        /// <param name="sequenceNumber">
        /// Copied from the Sequence Number field in the Binding Revocation Indication.
        /// It is used by the initiator, e.g., HA, LMA, MAG, in matching this Binding Revocation Acknowledgement 
        /// with the outstanding Binding Revocation Indication.
        /// </param>
        /// <param name="proxyBinding">
        /// Set if set in the corresponding Binding Revocation Indication message.
        /// </param>
        /// <param name="ipV4HomeAddressBindingOnly">
        /// Set if the it is set in the corresponding Binding Revocation Indication message.
        /// </param>
        /// <param name="global">
        /// Set if it is set in the corresponding Binding Revocation Indication message.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage(IpV4Protocol? nextHeader, ushort checksum,
                                                                                  Ipv6MobilityBindingRevocationStatus status, ushort sequenceNumber,
                                                                                  bool proxyBinding, bool ipV4HomeAddressBindingOnly, bool global,
                                                                                  IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, proxyBinding, ipV4HomeAddressBindingOnly, global, options)
        {
            Status = status;
        }

        /// <summary>
        /// Defines the type of the Binding Revocation Message.
        /// </summary>
        public override IpV6MobilityBindingRevocationType BindingRevocationType
        {
            get { return IpV6MobilityBindingRevocationType.BindingRevocationAcknowledgement; }
        }

        /// <summary>
        /// Indicating the result of processing the Binding Revocation Indication message by the responder.
        /// </summary>
        public Ipv6MobilityBindingRevocationStatus Status { get; private set; }

        internal override byte RevocationTriggerOrStatus
        {
            get { return (byte)Status; }
        }
    }
}