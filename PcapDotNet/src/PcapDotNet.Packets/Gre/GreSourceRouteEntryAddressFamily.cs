namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// A value representing the syntax and semantics of the Routing Information field.  
    /// </summary>
    public enum GreSourceRouteEntryAddressFamily : ushort
    {
        /// <summary>
        /// No address family
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// The Routing Information field will consist of a list of IP addresses and indicates an IP source route.
        /// </summary>
        IpSourceRoute = 0x0800,

        /// <summary>
        /// the Routing Information field will consist of a list of Autonomous System numbers and indicates an AS source route.
        /// </summary>
        AsSourceRoute = 0xfffe,
    }
}