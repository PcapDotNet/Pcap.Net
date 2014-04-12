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
    public class IpV6ExtensionHeaderMobilityFastBindingUpdate : IpV6ExtensionHeaderMobilityBindingUpdateBase
    {
        public IpV6ExtensionHeaderMobilityFastBindingUpdate(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge,
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