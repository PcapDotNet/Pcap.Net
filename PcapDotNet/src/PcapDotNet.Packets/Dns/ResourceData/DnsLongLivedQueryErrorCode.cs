namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// The DNS LLQ Error code values.
    /// </summary>
    public enum DnsLongLivedQueryErrorCode : ushort
    {
        /// <summary>
        /// The LLQ Setup Request was successful.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// The server cannot grant the LLQ request because it is overloaded,
        /// or the request exceeds the server's rate limit (see Section 8 "Security Considerations").
        /// Upon returning this error, the server MUST include in the LEASE-LIFE field a time interval, in seconds,
        /// after which the client may re-try the LLQ Setup.
        /// </summary>
        ServerFull = 1,

        /// <summary>
        /// The data for this name and type is not expected to change frequently, and the server therefore does not support the requested LLQ.
        /// The client must not poll for this name and type, nor should it re-try the LLQ Setup, and should instead honor the normal resource record TTLs returned.
        /// To reduce server load, an administrator MAY return this error for all records with types other than PTR and TXT as a matter of course.
        /// </summary>
        Static = 2,

        /// <summary>
        /// The LLQ was improperly formatted.
        /// Note that if the rest of the DNS message is properly formatted, the DNS header error code must not include a format error code,
        ///  as this would cause confusion between a server that does not understand the LLQ format, and a client that sends malformed LLQs.
        /// </summary>
        FormatError = 3,

        /// <summary>
        /// The client attempts to refresh an expired or non-existent LLQ (as determined by the LLQ-ID in the request).
        /// </summary>
        NoSuchLlq =  4,

        /// <summary>
        /// The protocol version specified in the client's request is not supported by the server.
        /// </summary>
        BadVersion = 5,

        /// <summary>
        /// The LLQ was not granted for an unknown reason.
        /// </summary>
        UnknownError = 6,
    }
}