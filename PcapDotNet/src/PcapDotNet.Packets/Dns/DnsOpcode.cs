namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 1035, 1996, 2136, 3425.
    /// Specifies kind of query in this message.  
    /// This value is set by the originator of a query and copied into the response.
    /// </summary>
    public enum DnsOpcode : byte
    {
        /// <summary>
        /// RFC 1035.
        /// A standard query (QUERY).
        /// </summary>
        Query = 0, 

        /// <summary>
        /// RFC 3425.
        /// An inverse query (IQUERY).
        /// </summary>
        IQuery = 1,

        /// <summary>
        /// RFC 1035.
        /// A server status request (STATUS).
        /// </summary>
        Status = 2,

        /// <summary>
        /// RFC 1996.
        /// </summary>
        Notify = 4,

        /// <summary>
        /// RFC 2136.
        /// </summary>
        Update = 5,
    }
}