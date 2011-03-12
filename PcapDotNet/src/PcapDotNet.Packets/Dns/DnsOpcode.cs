namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Specifies kind of query in this message.  
    /// This value is set by the originator of a query and copied into the response.
    /// </summary>
    public enum DnsOpcode : byte
    {
        /// <summary>
        /// A standard query (QUERY).
        /// </summary>
        Query = 0, 

        /// <summary>
        /// An inverse query (IQUERY).
        /// </summary>
        IQuery = 1,

        /// <summary>
        /// A server status request (STATUS).
        /// </summary>
        Status = 2,
    }
}