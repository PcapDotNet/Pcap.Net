namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// The different ICMP message types and codes.
    /// Each of the values is a combination of the message type and a code values that is legal with this message type.
    /// </summary>
    public enum IcmpMessageTypeAndCode : ushort
    {
        /// <summary>
        /// RFC 792.
        /// If, according to the information in the gateway's routing tables, 
        /// the network specified in the internet destination field of a datagram is unreachable, 
        /// e.g., the distance to the network is infinity, 
        /// the gateway may send a destination unreachable message to the internet source host of the datagram.
        /// </summary>
        DestinationUnreachableNetUnreachable = (IcmpMessageType.DestinationUnreachable << 8) | IcmpCodeDestinationUnreachable.NetUnreachable,

        /// <summary>
        /// RFC 792.
        /// In some networks, the gateway may be able to determine if the internet destination host is unreachable.  
        /// Gateways in these networks may send destination unreachable messages to the source host when the destination host is unreachable.
        /// </summary>
        DestinationUnreachableHostUnreachable = (IcmpMessageType.DestinationUnreachable << 8) | IcmpCodeDestinationUnreachable.HostUnreachable,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated protocol module is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        DestinationUnreachableProtocolUnreachable = (IcmpMessageType.DestinationUnreachable << 8) | IcmpCodeDestinationUnreachable.ProtocolUnreachable,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated process port is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        DestinationUnreachablePortUnreachable = (IcmpMessageType.DestinationUnreachable << 8) | IcmpCodeDestinationUnreachable.PortUnreachable,

        /// <summary>
        /// RFC 792.
        /// A datagram must be fragmented to be forwarded by a gateway yet the Don't Fragment flag is on.  
        /// In this case the gateway must discard the datagram and may return a destination unreachable message.
        /// </summary>
        DestinationUnreachableFragmentationNeededAndDoNotFragmentSet = (IcmpMessageType.DestinationUnreachable << 8) | IcmpCodeDestinationUnreachable.FragmentationNeededAndDoNotFragmentSet,

        /// <summary>
        /// RFC 792.
        /// </summary>
        DestinationUnreachableSourceRouteFailed = (IcmpMessageType.DestinationUnreachable << 8) | IcmpCodeDestinationUnreachable.SourceRouteFailed,

        /// <summary>
        /// RFC 792.
        /// If the gateway processing a datagram finds the time to live field is zero it must discard the datagram.  
        /// The gateway may also notify the source host via the time exceeded message.
        /// </summary>
        TimeExceededTimeToLive = (IcmpMessageType.TimeExceeded << 8) | IcmpCodeTimeExceeded.TimeToLive,

        /// <summary>
        /// RFC 792.
        /// If a host reassembling a fragmented datagram cannot complete the reassembly due to missing fragments within its time limit it discards the datagram, 
        /// and it may send a time exceeded message.
        /// If fragment zero is not available then no time exceeded need be sent at all.
        /// </summary>
        TimeExceededFragmentReassembly = (IcmpMessageType.TimeExceeded << 8) | IcmpCodeTimeExceeded.FragmentReassembly,

        /// <summary>
        /// RFC 792.
        /// If the gateway or host processing a datagram finds a problem with the header parameters such that it cannot complete processing the datagram it must discard the datagram.  
        /// One potential source of such a problem is with incorrect arguments in an option.  
        /// The gateway or host may also notify the source host via the parameter problem message. 
        /// This message is only sent if the error caused the datagram to be discarded.
        /// </summary>
        ParameterProblemPointerIndicatesTheError = (IcmpMessageType.ParameterProblem << 8),

        /// <summary>
        /// RFC 792.
        /// 
        /// <para>
        /// A gateway may discard internet datagrams if it does not have the buffer space needed to queue the datagrams for output to the next network on the route to the destination network.  
        /// If a gateway discards a datagram, it may send a source quench message to the internet source host of the datagram.  
        /// A destination host may also send a source quench message if datagrams arrive too fast to be processed.  
        /// The source quench message is a request to the host to cut back the rate at which it is sending traffic to the internet destination.  
        /// The gateway may send a source quench message for every message that it discards.  
        /// On receipt of a source quench message, the source host should cut back the rate at which it is sending traffic to the specified destination 
        /// until it no longer receives source quench messages from the gateway.  
        /// The source host can then gradually increase the rate at which it sends traffic to the destination until it again receives source quench messages.
        /// </para>
        /// 
        /// <para>
        /// The gateway or host may send the source quench message when it approaches its capacity limit rather than waiting until the capacity is exceeded.  
        /// This means that the data datagram which triggered the source quench message may be delivered.
        /// </para>
        /// </summary>
        SourceQuench = (IcmpMessageType.SourceQuench << 8),

        /// <summary>
        /// RFC 792.
        /// </summary>
        RedirectDatagramsForTheNetwork = (IcmpMessageType.Redirect << 8) | IcmpCodeRedirect.ForTheNetwork,
    
        /// <summary>
        /// RFC 792.
        /// </summary>
        RedirectDatagramsForTheHost = (IcmpMessageType.Redirect << 8) | IcmpCodeRedirect.ForTheHost,

        /// <summary>
        /// RFC 792.
        /// </summary>
        RedirectDatagramsForTheTypeOfServiceAndNetwork = (IcmpMessageType.Redirect << 8) | IcmpCodeRedirect.ForTheTypeOfServiceAndNetwork,

        /// <summary>
        /// RFC 792.
        /// </summary>
        RedirectDatagramsForTheTypeOfServiceAndHost = (IcmpMessageType.Redirect << 8) | IcmpCodeRedirect.ForTheTypeOfServiceAndHost,

        /// <summary>
        /// RFC 792.
        /// <para>
        /// The data received in the echo message must be returned in the echo reply message.
        /// </para>
        /// 
        /// <para>
        /// The identifier and sequence number may be used by the echo sender to aid in matching the replies with the echo requests.  
        /// For example, the identifier might be used like a port in TCP or UDP to identify a session, and the sequence number might be incremented on each echo request sent.  
        /// The echoer returns these same values in the echo reply.
        /// </para>
        /// 
        /// <para>
        /// Code 0 may be received from a gateway or a host.
        /// </para>        
        /// </summary>
        Echo = (IcmpMessageType.Echo << 8),

        /// <summary>
        /// RFC 792.
        /// <para>
        /// The data received in the echo message must be returned in the echo reply message.
        /// </para>
        /// 
        /// <para>
        /// The identifier and sequence number may be used by the echo sender to aid in matching the replies with the echo requests.  
        /// For example, the identifier might be used like a port in TCP or UDP to identify a session, and the sequence number might be incremented on each echo request sent.  
        /// The echoer returns these same values in the echo reply.
        /// </para>
        /// 
        /// <para>
        /// Code 0 may be received from a gateway or a host.
        /// </para>        
        /// </summary>
        EchoReply = (IcmpMessageType.EchoReply << 8),

        /// <summary>
        /// RFC 792
        /// <para>
        /// The data received (a timestamp) in the message is returned in the reply together with an additional timestamp.  
        /// The timestamp is 32 bits of milliseconds since midnight UT.  
        /// </para>
        /// 
        /// <para>
        /// If the time is not available in miliseconds or cannot be provided with respect to midnight UT 
        /// then any time can be inserted in a timestamp provided the high order bit of the timestamp is also set to indicate this non-standard value.
        /// </para>
        /// 
        /// <para>
        /// The identifier and sequence number may be used by the echo sender to aid in matching the replies with the requests.  
        /// For example, the identifier might be used like a port in TCP or UDP to identify a session, and the sequence number might be incremented on each request sent.  
        /// The destination returns these same values in the reply.
        /// </para>
        /// 
        /// <para>
        /// Code 0 may be received from a gateway or a host.
        /// </para>
        /// </summary>
        Timestamp = (IcmpMessageType.Timestamp << 8),

        /// <summary>
        /// RFC 792
        /// <para>
        /// The data received (a timestamp) in the message is returned in the reply together with an additional timestamp.  
        /// The timestamp is 32 bits of milliseconds since midnight UT.  
        /// </para>
        /// 
        /// <para>
        /// If the time is not available in miliseconds or cannot be provided with respect to midnight UT 
        /// then any time can be inserted in a timestamp provided the high order bit of the timestamp is also set to indicate this non-standard value.
        /// </para>
        /// 
        /// <para>
        /// The identifier and sequence number may be used by the echo sender to aid in matching the replies with the requests.  
        /// For example, the identifier might be used like a port in TCP or UDP to identify a session, and the sequence number might be incremented on each request sent.  
        /// The destination returns these same values in the reply.
        /// </para>
        /// 
        /// <para>
        /// Code 0 may be received from a gateway or a host.
        /// </para>
        /// </summary>
        TimestampReply = (IcmpMessageType.TimestampReply << 8),

        /// <summary>
        /// RFC 792.
        /// <para>
        /// This message may be sent with the source network in the IP header source and destination address fields zero (which means "this" network).  
        /// The replying IP module should send the reply with the addresses fully specified.  
        /// This message is a way for a host to find out the number of the network it is on.
        /// </para>
        /// 
        /// <para>
        /// The identifier and sequence number may be used by the echo sender to aid in matching the replies with the requests.  
        /// For example, the identifier might be used like a port in TCP or UDP to identify a session, and the sequence number might be incremented on each request sent.  
        /// The destination returns these same values in the reply.
        /// </para>
        /// 
        /// <para>
        /// Code 0 may be received from a gateway or a host.
        /// </para>
        /// </summary>
        InformationRequest = (IcmpMessageType.InformationRequest << 8),

        /// <summary>
        /// RFC 792.
        /// <para>
        /// This message may be sent with the source network in the IP header source and destination address fields zero (which means "this" network).  
        /// The replying IP module should send the reply with the addresses fully specified.  
        /// This message is a way for a host to find out the number of the network it is on.
        /// </para>
        /// 
        /// <para>
        /// The identifier and sequence number may be used by the echo sender to aid in matching the replies with the requests.  
        /// For example, the identifier might be used like a port in TCP or UDP to identify a session, and the sequence number might be incremented on each request sent.  
        /// The destination returns these same values in the reply.
        /// </para>
        /// 
        /// <para>
        /// Code 0 may be received from a gateway or a host.
        /// </para>
        /// </summary>
        InformationReply = (IcmpMessageType.InformationReply << 8),

        /// <summary>
        /// RFC 1256.
        /// </summary>
        RouterAdvertisement = (IcmpMessageType.RouterAdvertisement << 8),

        /// <summary>
        /// RFC 1256.
        /// </summary>
        RouterSolicitation = (IcmpMessageType.RouterSolicitation << 8),

        /// <summary>
        /// RFC 950.
        /// 
        /// <para>
        /// A gateway receiving an address mask request should return it with the address mask field set to the 32-bit mask of the bits identifying the subnet and network, 
        /// for the subnet on which the request was received.
        /// </para>
        /// 
        /// <para>
        /// If the requesting host does not know its own IP address, it may leave the source field zero; the reply should then be broadcast.  
        /// However, this approach should be avoided if at all possible, since it increases the superfluous broadcast load on the network.  
        /// Even when the replies are broadcast, since there is only one possible address mask for a subnet, there is no need to match requests with replies.  
        /// The "Identifier" and "Sequence Number" fields can be ignored.
        /// </para>
        /// </summary>
        AddressMaskRequest = (IcmpMessageType.AddressMaskRequest << 8),

        /// <summary>
        /// RFC 950.
        /// 
        /// <para>
        /// A gateway receiving an address mask request should return it with the address mask field set to the 32-bit mask of the bits identifying the subnet and network, 
        /// for the subnet on which the request was received.
        /// </para>
        /// 
        /// <para>
        /// If the requesting host does not know its own IP address, it may leave the source field zero; the reply should then be broadcast.  
        /// However, this approach should be avoided if at all possible, since it increases the superfluous broadcast load on the network.  
        /// Even when the replies are broadcast, since there is only one possible address mask for a subnet, there is no need to match requests with replies.  
        /// The "Identifier" and "Sequence Number" fields can be ignored.
        /// </para>
        /// </summary>
        AddressMaskReply = (IcmpMessageType.AddressMaskReply << 8),

        /// <summary>
        /// RFC 1393.
        /// </summary>
        TraceRouteOutboundPacketSuccessfullyForwarded = (IcmpMessageType.TraceRoute << 8) | IcmpCodeTraceRoute.OutboundPacketSuccessfullyForwarded,

        /// <summary>
        /// RFC 1393.
        /// </summary>
        TraceRouteNoRouteForOutboundPacketDiscarded = (IcmpMessageType.TraceRoute << 8) | IcmpCodeTraceRoute.NoRouteForOutboundPacketDiscarded,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedUnknownOrUnspecifiedError = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.UnknownOrUnspecifiedError,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedDoNotConvertOptionPresent = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.DoNotConvertOptionPresent,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedUnknownMandatoryOptionPresent = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.UnknownMandatoryOptionPresent,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedKnownUnsupportedOptionPresent = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.KnownUnsupportedOptionPresent,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedUnsupportedTransportProtocol = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.UnsupportedTransportProtocol,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedOverallLengthExceeded = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.OverallLengthExceeded,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedIpHeaderLengthExceeded = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.IpHeaderLengthExceeded,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedTransportProtocolIsBiggerThan255 = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.TransportProtocolIsBiggerThan255,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedPortConversionOutOfRange = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.PortConversionOutOfRange,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedTransportHeaderLengthExceeded = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.TransportHeaderLengthExceeded,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailed32BitRolloverMissingAndAckSet = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.Code32BitRolloverMissingAndAckSet,

        /// <summary>
        /// RFC 1475.
        /// The introduction of network layer conversion requires a new message type, to report conversion errors.  
        /// Note that an invalid datagram should result in the sending of some other ICMP message (e.g., parameter problem) or the silent discarding of the datagram.  
        /// This message is only sent when a valid datagram cannot be converted.
        /// </summary>
        ConversionFailedUnknownMandatoryTransportOptionPresent = (IcmpMessageType.ConversionFailed << 8) | IcmpCodeConversionFailed.UnknownMandatoryTransportOptionPresent,

        /// <summary>
        /// RFC 1788.
        /// </summary>
        DomainNameRequest = (IcmpMessageType.DomainNameRequest << 8),

        /// <summary>
        /// RFC 1788.
        /// Parsing of this datagram isn't supported because its parsing is not clear from the RFC.
        /// </summary>
        DomainNameReply = (IcmpMessageType.DomainNameReply << 8),

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram includes a Security Parameters Index (SPI) that is invalid or has expired.
        /// </summary>
        SecurityFailuresBadSecurityParametersIndex = (IcmpMessageType.SecurityFailures << 8) | IcmpCodeSecurityFailure.BadSecurityParametersIndex,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed the authenticity or integrity check for a given SPI.
        /// 
        /// <para>
        /// Note that the SPI may indicate an outer Encapsulating Security Protocol when a separate Authentication Header SPI is hidden inside.
        /// </para>
        /// </summary>
        SecurityFailuresAuthenticationFailed = (IcmpMessageType.SecurityFailures << 8) | IcmpCodeSecurityFailure.AuthenticationFailed,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed a decompression check for a given SPI.
        /// </summary>
        SecurityFailuresDecompressionFailed = (IcmpMessageType.SecurityFailures << 8) | IcmpCodeSecurityFailure.DecompressionFailed,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed a decryption check for a given SPI.
        /// </summary>
        SecurityFailuresDecryptionFailed = (IcmpMessageType.SecurityFailures << 8) | IcmpCodeSecurityFailure.DecryptionFailed,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram will not be accepted without additional authentication.
        /// 
        /// <para>
        /// In this case, either no SPI is present, or an unsuitable SPI is present.  
        /// For example, an encryption SPI without integrity arrives from a secure operating system with mutually suspicious users.
        /// </para>
        /// </summary>
        SecurityFailuresNeedAuthentication = (IcmpMessageType.SecurityFailures << 8) | IcmpCodeSecurityFailure.NeedAuthentication,
        
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
        SecurityFailuresNeedAuthorization = (IcmpMessageType.SecurityFailures << 8) | IcmpCodeSecurityFailure.NeedAuthorization,
    }
}