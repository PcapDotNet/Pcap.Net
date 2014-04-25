using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 3963, 5213, 5845, 6275.
    /// <pre>
    /// +-----+-------------+---+---+----+----+---------+
    /// | Bit | 0-7         | 8 | 9 | 10 | 11 | 12-15   |
    /// +-----+-------------+---+---+----+----+---------+
    /// | 0   | Next Header | Header Extension Length   |
    /// +-----+-------------+---------------------------+
    /// | 16  | MH Type     | Reserved                  |
    /// +-----+-------------+---------------------------+
    /// | 32  | Checksum                                |
    /// +-----+-------------+---+---+---+----+----------+
    /// | 48  | Status      | K | R | P | T  | Reserved |
    /// +-----+-------------+---+---+---+----+----------+
    /// | 64  | Sequence #                              |
    /// +-----+-----------------------------------------+
    /// | 80  | Lifetime                                |
    /// +-----+-----------------------------------------+
    /// | 96  | Mobility Options                        |
    /// | ... |                                         |
    /// +-----+-----------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityBindingAcknowledgement : IpV6ExtensionHeaderMobilityBindingAcknowledgementBase
    {
        private static class MessageDataOffset
        {
            public const int MobileRouter = sizeof(byte);
            public const int ProxyRegistration = MobileRouter;
            public const int TlvHeaderFormat = ProxyRegistration;
        }

        private static class MessageDataMask
        {
            public const byte MobileRouter = 0x40;
            public const byte ProxyRegistration = 0x20;
            public const byte TlvHeaderFormat = 0x10;
        }

        public IpV6ExtensionHeaderMobilityBindingAcknowledgement(IpV4Protocol nextHeader, ushort checksum, IpV6BindingAcknowledgementStatus status,
                                                                 bool keyManagementMobilityCapability, bool mobileRouter, bool proxyRegistration,
                                                                 bool tlvHeaderFormat, ushort sequenceNumber, ushort lifetime, IpV6MobilityOptions options)
            : base(nextHeader, checksum, status, keyManagementMobilityCapability, sequenceNumber, lifetime, options)
        {
            MobileRouter = mobileRouter;
            ProxyRegistration = proxyRegistration;
            TlvHeaderFormat = tlvHeaderFormat;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.BindingAcknowledgement; }
        }

        /// <summary>
        /// Indicates that the Home Agent that processed the Binding Update supports Mobile Routers.
        /// True only if the corresponding Binding Update had the Mobile Router set to true.
        /// </summary>
        public bool MobileRouter { get; private set; }

        /// <summary>
        /// Indicates that the local mobility anchor that processed the corresponding Proxy Binding Update message supports proxy registrations.
        /// True only if the corresponding Proxy Binding Update had the Proxy Registration set to true.
        /// </summary>
        public bool ProxyRegistration { get; private set; }

        /// <summary>
        /// Indicates that the sender of the Proxy Binding Acknowledgement, the LMA, supports tunneling IPv6-or-IPv4 in IPv4 using TLV-header format.
        /// </summary>
        public bool TlvHeaderFormat { get; private set; }

        internal static IpV6ExtensionHeaderMobilityBindingAcknowledgement ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            IpV6BindingAcknowledgementStatus status;
            bool keyManagementMobilityCapability;
            ushort sequenceNumber;
            ushort lifetime;
            IpV6MobilityOptions options;
            if (!ParseMessageDataFields(messageData, out status, out keyManagementMobilityCapability, out sequenceNumber, out lifetime, out options))
                return null;

            bool mobileRouter = messageData.ReadBool(MessageDataOffset.MobileRouter, MessageDataMask.MobileRouter);
            bool proxyRegistration = messageData.ReadBool(MessageDataOffset.ProxyRegistration, MessageDataMask.ProxyRegistration);
            bool tlvHeaderFormat = messageData.ReadBool(MessageDataOffset.TlvHeaderFormat, MessageDataMask.TlvHeaderFormat);

            return new IpV6ExtensionHeaderMobilityBindingAcknowledgement(nextHeader, checksum, status, keyManagementMobilityCapability, mobileRouter,
                                                                         proxyRegistration, tlvHeaderFormat, sequenceNumber, lifetime, options);
        }
    }
}