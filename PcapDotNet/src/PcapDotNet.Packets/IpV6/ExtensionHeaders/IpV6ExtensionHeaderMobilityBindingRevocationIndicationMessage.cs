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
    /// | 48  | B.R. Type       | R. Trigger              |
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
    public sealed class IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage : IpV6ExtensionHeaderMobilityBindingRevocationMessage
    {
        /// <summary>
        /// Creates an instance from next header, checksum, revocation trigger, sequence number, proxy binding, IPv4 home address binding only, global 
        /// and options.
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
        /// <param name="revocationTrigger">
        /// Indicating the event that triggered the initiator to send the BRI message.
        /// </param>
        /// <param name="sequenceNumber">
        /// Used by the initiator to match a returned Binding Revocation Acknowledgement with this Binding Revocation Indication.
        /// This sequence number could be a random number.
        /// At any time, implementations must ensure there is no collision between the sequence numbers of all outstanding Binding Revocation Indication 
        /// Messages.
        /// </param>
        /// <param name="proxyBinding">
        /// Set by the initiator to indicate that the revoked binding(s) is a PMIPv6 binding.
        /// </param>
        /// <param name="ipV4HomeAddressBindingOnly">
        /// Set by the initiator, home agent, or local mobility anchor to indicate to the receiving mobility entity the termination
        /// of the IPv4 Home Address binding only as in Home Agent Operation and Local Mobility Anchor Operation.
        /// </param>
        /// <param name="global">
        /// Set by the initiator, LMA or MAG, to indicate the termination of all Per-Peer mobility Bindings or Multiple Bindings that share 
        /// a common identifier(s) and are served by the initiator and responder as in Local Mobility Anchor Operation and Mobile Access Gateway Operation.
        /// </param>
        /// <param name="options">
        /// Zero or more TLV-encoded mobility options.
        /// </param>
        public IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage(IpV4Protocol? nextHeader, ushort checksum,
                                                                             Ipv6MobilityBindingRevocationTrigger revocationTrigger, ushort sequenceNumber,
                                                                             bool proxyBinding, bool ipV4HomeAddressBindingOnly, bool global,
                                                                             IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, proxyBinding, ipV4HomeAddressBindingOnly, global, options)
        {
            RevocationTrigger = revocationTrigger;
        }

        /// <summary>
        /// Defines the type of the Binding Revocation Message.
        /// </summary>
        public override IpV6MobilityBindingRevocationType BindingRevocationType
        {
            get { return IpV6MobilityBindingRevocationType.BindingRevocationIndication; }
        }

        /// <summary>
        /// Indicating the event that triggered the initiator to send the BRI message.
        /// </summary>
        public Ipv6MobilityBindingRevocationTrigger RevocationTrigger { get; private set; }

        internal override byte RevocationTriggerOrStatus
        {
            get { return (byte)RevocationTrigger; }
        }
    }
}