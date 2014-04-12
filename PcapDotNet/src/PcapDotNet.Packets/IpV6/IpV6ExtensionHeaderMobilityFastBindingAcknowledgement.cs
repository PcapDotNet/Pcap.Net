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
        public IpV6ExtensionHeaderMobilityFastBindingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
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