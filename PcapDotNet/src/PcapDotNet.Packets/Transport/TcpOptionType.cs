namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// An enum for all the different tcp option types.
    /// </summary>
    public enum TcpOptionType : byte
    {
        /// <summary>
        /// End of Option List (RFC793)
        /// </summary>
        EndOfOptionList = 0,

        /// <summary>
        /// No-Operation (RFC793)
        /// </summary>
        NoOperation = 1,

        /// <summary>
        /// Maximum Segment Size (RFC793)
        /// </summary>
        MaximumSegmentSize = 2,
        
        /// <summary>
        /// WSOPT - Window Scale (RFC1323)
        /// </summary>
        WindowScale = 3,

        /// <summary>
        /// Denote Packet Mood (RFC5841)
        /// </summary>
        Mood = 25,

        /// <summary>
        /// SACK Permitted (RFC2018)
        /// </summary>
        SelectiveAcknowledgmentPermitted = 4,

        /// <summary>
        /// SACK (RFC2018)
        /// </summary>
        SelectiveAcknowledgment = 5,

        /// <summary>
        /// Echo (obsoleted by option 8) (RFC1072)
        /// </summary>
        Echo = 6,
        
        /// <summary>
        /// Echo Reply (obsoleted by option 8) (RFC1072)
        /// </summary>
        EchoReply = 7,

        /// <summary>
        /// TSOPT - Time Stamp Option (RFC1323)
        /// </summary>
        Timestamp = 8,

        /// <summary>
        /// Partial Order Connection Permitted (RFC1693)
        /// </summary>
        PartialOrderConnectionPermitted = 9,

        /// <summary>
        /// Partial Order Service Profile (RFC1693)
        /// </summary>
        PartialOrderServiceProfile = 10,

        /// <summary>
        /// CC (RFC1644)
        /// </summary>
        ConnectionCount = 11,

        /// <summary>
        /// CC.NEW (RFC1644)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        ConnectionCountNew = 12,

        /// <summary>
        /// CC.ECHO (RFC1644)
        /// </summary>
        ConnectionCountEcho = 13,

        /// <summary>
        /// TCP Alternate Checksum Request (RFC1146)
        /// </summary>
        AlternateChecksumRequest = 14,

        /// <summary>
        /// TCP Alternate Checksum Data (RFC1146)
        /// </summary>
        AlternateChecksumData = 15,

        /// <summary>
        /// MD5 Signature Option (RFC2385)
        /// </summary>
        Md5Signature = 19,

        /// <summary>
        /// Quick-Start Response (RFC4782)
        /// </summary>
        QuickStartResponse = 27,

        /// <summary>
        /// User Timeout Option (RFC5482)
        /// </summary>
        UserTimeout = 28,
    }
}