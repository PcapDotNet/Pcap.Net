namespace PcapDotNet.Packets.Dns
{
    public enum DnsResponseCode : byte
    {
        /// <summary>
        /// No error condition
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Format error - The name server was unable to interpret the query.
        /// </summary>
        FormatError = 1,

        /// <summary>
        /// Server failure - The name server was unable to process this query due to a problem with the name server.
        /// </summary>
        ServerFailure = 2,
                
        /// <summary>
        /// Name Error - Meaningful only for responses from an authoritative name server, 
        /// this code signifies that the domain name referenced in the query does not exist.
        /// </summary>
        NameError = 3,
                
        /// <summary>
        /// Not Implemented - The name server does not support the requested kind of query.
        /// </summary>
        NotImplemented = 4,

        /// <summary>
        /// Refused - The name server refuses to perform the specified operation for policy reasons.  
        /// For example, a name server may not wish to provide the information to the particular requester, 
        /// or a name server may not wish to perform a particular operation (e.g., zone transfer) for particular data.
        /// </summary>
        Refused = 5,
    }
}