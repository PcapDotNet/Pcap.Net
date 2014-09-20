using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFCs 3963, 4140, 5213, 5380, 5555, 5845, 6275, 6602.
    /// <pre>
    /// +-----+---+---+---+---+---+---+---+---+---+---+-----------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10-15           |
    /// +-----+---+---+---+---+---+---+---+---+---+---+-----------------+
    /// | 0   | Next Header                   | Header Extension Length |
    /// +-----+-------------------------------+-------------------------+
    /// | 16  | MH Type                       | Reserved                |
    /// +-----+-------------------------------+-------------------------+
    /// | 32  | Checksum                                                |
    /// +-----+---------------------------------------------------------+
    /// | 48  | Sequence #                                              |
    /// +-----+---+---+---+---+---+---+---+---+---+---+-----------------+
    /// | 64  | A | H | L | K | M | R | P | F | T | B | Reserved        |
    /// +-----+---+---+---+---+---+---+---+---+---+---+-----------------+
    /// | 80  | Lifetime                                                |
    /// +-----+---------------------------------------------------------+
    /// | 96  | Mobility Options                                        |
    /// | ... |                                                         |
    /// +-----+---------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingUpdate : IpV6ExtensionHeaderMobilityBindingUpdateBase
    {
        private static class MessageDataOffset
        {
            public const int MapRegistration = sizeof(ushort);
            public const int MobileRouter = MapRegistration;
            public const int ProxyRegistrationFlag = MobileRouter;
            public const int ForcingUdpEncapsulation = ProxyRegistrationFlag;
            public const int TlvHeaderFormat = ForcingUdpEncapsulation + sizeof(byte);
            public const int BulkBindingUpdate = TlvHeaderFormat;
        }

        private static class MessageDataMask
        {
            public const byte MapRegistration = 0x08;
            public const byte MobileRouter = 0x04;
            public const byte ProxyRegistrationFlag = 0x02;
            public const byte ForcingUdpEncapsulation = 0x01;
            public const byte TlvHeaderFormat = 0x80;
            public const byte BulkBindingUpdate = 0x40;
        }

        /// <summary>
        /// Creates an instance from next header, checksum, sequence number, acknowledge, home registration, link local address compatibility,
        /// key management mobiltiy capability, map registration, mobile router, proxy registration flag, forcing UDP encapsulation, TLV header format,
        /// bulk binding update, lifetime and options.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
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
        /// <param name="mapRegistration">
        /// Indicates MAP registration.
        /// When a mobile node registers with the MAP, the MapRegistration and Acknowledge must be set to distinguish this registration
        /// from a Binding Update being sent to the Home Agent or a correspondent node.
        /// </param>
        /// <param name="mobileRouter">
        /// Indicates to the Home Agent that the Binding Update is from a Mobile Router.
        /// If false, the Home Agent assumes that the Mobile Router is behaving as a Mobile Node,
        /// and it must not forward packets destined for the Mobile Network to the Mobile Router.
        /// </param>
        /// <param name="proxyRegistrationFlag">
        /// Indicates to the local mobility anchor that the Binding Update message is a proxy registration.
        /// Must be true for proxy registrations and must be false direct registrations sent by a mobile node.
        /// </param>
        /// <param name="forcingUdpEncapsulation">
        /// Indicates a request for forcing UDP encapsulation regardless of whether a NAT is present on the path between the mobile node and the home agent.
        /// May be set by the mobile node if it is required to use UDP encapsulation regardless of the presence of a NAT.
        /// </param>
        /// <param name="tlvHeaderFormat">
        /// Indicates that the mobile access gateway requests the use of the TLV header for encapsulating IPv6 or IPv4 packets in IPv4.
        /// </param>
        /// <param name="bulkBindingUpdate">
        /// If true, it informs the local mobility anchor to enable bulk binding update support for the mobility session associated with this message.
        /// If false, the local mobility anchor must exclude the mobility session associated with this message from any bulk-binding-related operations
        /// and any binding update, or binding revocation operations with bulk-specific scope will not be relevant to that mobility session.
        /// This flag is relevant only for Proxy Mobile IPv6 and therefore must be set to false when the ProxyRegistrationFlag is false.
        /// </param>
        /// <param name="lifetime">
        /// The number of time units remaining before the binding must be considered expired.
        /// A value of zero indicates that the Binding Cache entry for the mobile node must be deleted.
        /// One time unit is 4 seconds for Binding Update and 1 second for Fast Binding Update.
        /// </param>
        /// <param name="options">Zero or more TLV-encoded mobility options.</param>
        public IpV6ExtensionHeaderMobilityBindingUpdate(IpV4Protocol nextHeader, ushort checksum, ushort sequenceNumber, bool acknowledge, bool homeRegistration,
                                                        bool linkLocalAddressCompatibility, bool keyManagementMobilityCapability, bool mapRegistration,
                                                        bool mobileRouter, bool proxyRegistrationFlag, bool forcingUdpEncapsulation, bool tlvHeaderFormat,
                                                        bool bulkBindingUpdate, ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration, linkLocalAddressCompatibility, keyManagementMobilityCapability,
                   lifetime, options)
        {
            MapRegistration = mapRegistration;
            MobileRouter = mobileRouter;
            ProxyRegistrationFlag = proxyRegistrationFlag;
            ForcingUdpEncapsulation = forcingUdpEncapsulation;
            TlvHeaderFormat = tlvHeaderFormat;
            BulkBindingUpdate = bulkBindingUpdate;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingUpdate; }
        }

        /// <summary>
        /// Indicates MAP registration.
        /// When a mobile node registers with the MAP, the MapRegistration and Acknowledge must be set to distinguish this registration
        /// from a Binding Update being sent to the Home Agent or a correspondent node.
        /// </summary>
        public bool MapRegistration { get; private set; }

        /// <summary>
        /// Indicates to the Home Agent that the Binding Update is from a Mobile Router.
        /// If false, the Home Agent assumes that the Mobile Router is behaving as a Mobile Node,
        /// and it must not forward packets destined for the Mobile Network to the Mobile Router.
        /// </summary>
        public bool MobileRouter { get; private set; }

        /// <summary>
        /// Indicates to the local mobility anchor that the Binding Update message is a proxy registration.
        /// Must be true for proxy registrations and must be false direct registrations sent by a mobile node.
        /// </summary>
        public bool ProxyRegistrationFlag { get; private set; }

        /// <summary>
        /// Indicates a request for forcing UDP encapsulation regardless of whether a NAT is present on the path between the mobile node and the home agent.
        /// May be set by the mobile node if it is required to use UDP encapsulation regardless of the presence of a NAT.
        /// </summary>
        public bool ForcingUdpEncapsulation { get; private set; }

        /// <summary>
        /// Indicates that the mobile access gateway requests the use of the TLV header for encapsulating IPv6 or IPv4 packets in IPv4.
        /// </summary>
        public bool TlvHeaderFormat { get; private set; }

        /// <summary>
        /// If true, it informs the local mobility anchor to enable bulk binding update support for the mobility session associated with this message.
        /// If false, the local mobility anchor must exclude the mobility session associated with this message from any bulk-binding-related operations
        /// and any binding update, or binding revocation operations with bulk-specific scope will not be relevant to that mobility session.
        /// This flag is relevant only for Proxy Mobile IPv6 and therefore must be set to false when the ProxyRegistrationFlag is false.
        /// </summary>
        public bool BulkBindingUpdate { get; private set; }

        internal static IpV6ExtensionHeaderMobilityBindingUpdate ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
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

            bool mapRegistration = messageData.ReadBool(MessageDataOffset.MapRegistration, MessageDataMask.MapRegistration);
            bool mobileRouter = messageData.ReadBool(MessageDataOffset.MobileRouter, MessageDataMask.MobileRouter);
            bool proxyRegistrationFlag = messageData.ReadBool(MessageDataOffset.ProxyRegistrationFlag, MessageDataMask.ProxyRegistrationFlag);
            bool forcingUdpEncapsulation = messageData.ReadBool(MessageDataOffset.ForcingUdpEncapsulation, MessageDataMask.ForcingUdpEncapsulation);
            bool tlvHeaderFormat = messageData.ReadBool(MessageDataOffset.TlvHeaderFormat, MessageDataMask.TlvHeaderFormat);
            bool bulkBindingUpdate = messageData.ReadBool(MessageDataOffset.BulkBindingUpdate, MessageDataMask.BulkBindingUpdate);

            return new IpV6ExtensionHeaderMobilityBindingUpdate(nextHeader, checksum, sequenceNumber, acknowledge, homeRegistration,
                                                                linkLocalAddressCompatibility, keyManagementMobilityCapability, mapRegistration, mobileRouter,
                                                                proxyRegistrationFlag, forcingUdpEncapsulation, tlvHeaderFormat, bulkBindingUpdate, lifetime,
                                                                options);
        }
    }
}