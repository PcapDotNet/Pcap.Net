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
    public class IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage : IpV6ExtensionHeaderMobilityBindingRevocationMessage
    {
        public IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage(IpV4Protocol nextHeader, ushort checksum,
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
        public sealed override IpV6MobilityBindingRevocationType BindingRevocationType
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