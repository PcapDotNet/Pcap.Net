namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// The version of the IGMP query message.
    /// </summary>
    public enum IgmpQueryVersion
    {
        /// <summary>
        /// No query version - not a query.
        /// </summary>
        None,
        
        /// <summary>
        /// Version 1 query - RFC1112.
        /// </summary>
        Version1,

        /// <summary>
        /// Version 2 query - RFC2236.
        /// </summary>
        Version2,

        /// <summary>
        /// Version 3 query - RFC3376.
        /// </summary>
        Version3,

        /// <summary>
        /// The version of the query could not be interpreted.
        /// </summary>
        Unknown
    }
}