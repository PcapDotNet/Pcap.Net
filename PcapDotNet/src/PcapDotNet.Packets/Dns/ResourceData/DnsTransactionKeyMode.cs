﻿namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2930.
    /// </summary>
    public enum DnsTransactionKeyMode : ushort
    {
        None = 0,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        ServerAssignment = 1,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        DiffieHellmanExchange = 2,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        GssApiNegotiation = 3,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        ResolverAssignment = 4,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        KeyDeletion = 5,
    }
}