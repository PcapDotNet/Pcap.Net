namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public enum IcmpCodeDestinationUnrechable : byte
    {
        /// <summary>
        /// If, according to the information in the gateway's routing tables, 
        /// the network specified in the internet destination field of a datagram is unreachable, 
        /// e.g., the distance to the network is infinity, 
        /// the gateway may send a destination unreachable message to the internet source host of the datagram.
        /// </summary>
        NetUnreachable = 0x00,

        /// <summary>
        /// RFC 792.
        /// In some networks, the gateway may be able to determine if the internet destination host is unreachable.  
        /// Gateways in these networks may send destination unreachable messages to the source host when the destination host is unreachable.
        /// </summary>
        HostUnreachable = 0x01,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated protocol module is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        ProtocolUnreachable = 0x02,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated process port is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        PortUnreachable = 0x03,

        /// <summary>
        /// RFC 792.
        /// A datagram must be fragmented to be forwarded by a gateway yet the Don't Fragment flag is on.  
        /// In this case the gateway must discard the datagram and may return a destination unreachable message.
        /// </summary>
        FragmentationNeededAndDontFragmentSet = 0x04,

        /// <summary>
        /// RFC 792.
        /// </summary>
        SourceRouteFailed = 0x05,
    }

    public enum IcmpCodeTimeExceeded : byte
    {
        /// <summary>
        /// RFC 792.
        /// If the gateway processing a datagram finds the time to live field is zero it must discard the datagram.  
        /// The gateway may also notify the source host via the time exceeded message.
        /// </summary>
        TimeToLive = 0x00,

        /// <summary>
        /// RFC 792.
        /// If a host reassembling a fragmented datagram cannot complete the reassembly due to missing fragments within its time limit it discards the datagram, 
        /// and it may send a time exceeded message.
        /// If fragment zero is not available then no time exceeded need be sent at all.
        /// </summary>
        FragmentReassembly = 0x01,
    }

    public enum IcmpCodeRedirect : byte
    {
        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheNetwork = 0x00,

        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheHost = 0x01,

        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheTypeOfServiceAndNetwork = 0x02,

        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheTypeOfServiceAndHost = 0x03,
    }

    public enum IcmpCodeTraceroute : byte
    {
        /// <summary>
        /// RFC 1393.
        /// </summary>
        OutboundPacketSuccessfullyForwarded = 0x00,

        /// <summary>
        /// RFC 1393.
        /// </summary>
        NoRouteForOutboundPacketDiscarded = 0x01,
    }

    public enum IcmpCodeConversionFailed : byte
    {
        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        UnknownOrUnspecifiedError = 0x00,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        DontConvertOptionPresent = 0x01,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        UnknownMandatoryOptionPresent = 0x02,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        KnownUnsupportedOptionPresent = 0x03,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        UnsupportedTransportProtocol = 0x04,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        OverallLengthExceeded = 0x05,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        IpHeaderLengthExceeded = 0x06,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        TransportProtocolIsBiggerThan255 = 0x07,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        PortConversionOutOfRange = 0x08,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        TransportHeaderLengthExceeded = 0x09,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        Code32BitRolloverMissingAndAckSet = 0x0A,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        UnknownMandatoryTransportOptionPresent = 0x0B,
    }

    public enum IcmpCodeSecurityFailures : byte
    {
        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram includes a Security Parameters Index (SPI) that is invalid or has expired.
        /// </summary>
        BadSpi = 0x00,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed the authenticity or integrity check for a given SPI.
        /// 
        /// <para>
        /// Note that the SPI may indicate an outer Encapsulating Security Protocol when a separate Authentication Header SPI is hidden inside.
        /// </para>
        /// </summary>
        AuthenticationFailed = 0x01,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed a decompression check for a given SPI.
        /// </summary>
        DecompressionFailed = 0x02,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed a decryption check for a given SPI.
        /// </summary>
        DecryptionFailed = 0x03,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram will not be accepted without additional authentication.
        /// 
        /// <para>
        /// In this case, either no SPI is present, or an unsuitable SPI is present.  
        /// For example, an encryption SPI without integrity arrives from a secure operating system with mutually suspicious users.
        /// </para>
        /// </summary>
        NeedAuthentication = 0x04,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram will not be accepted because it has insufficient authorization.
        /// 
        /// <para>
        /// In this case, an authentication SPI is present that is inappropriate for the target transport or application.  
        /// The principle party denoted by the SPI does not have proper authorization for the facilities used by the datagram.  
        /// For example, the party is authorized for Telnet access, but not for FTP access.
        /// </para>
        /// </summary>
        NeedAuthorization = 0x05,
    }
}
