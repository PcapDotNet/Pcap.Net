namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// The ICMP code values for Code Conversion Failed ICMP type.
    /// </summary>
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
        DoNotConvertOptionPresent = 0x01,

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
}