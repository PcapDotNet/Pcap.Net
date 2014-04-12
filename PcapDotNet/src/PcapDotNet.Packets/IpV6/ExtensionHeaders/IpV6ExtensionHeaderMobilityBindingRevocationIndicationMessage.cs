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
        public IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage(IpV4Protocol nextHeader, ushort checksum,
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
        public override sealed IpV6MobilityBindingRevocationType BindingRevocationType
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