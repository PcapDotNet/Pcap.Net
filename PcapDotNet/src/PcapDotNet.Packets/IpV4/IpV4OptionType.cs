namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The option-type octet is viewed as having 3 fields:
    /// <list type="table">
    ///   <listheader>
    ///       <term>Bits</term>
    ///       <description>Description</description>
    ///   </listheader>
    ///   <item><term>1 bit</term>
    ///     <description>
    ///       Copied flag.
    ///       <para>
    ///         The copied flag indicates that this option is copied into all fragments on fragmentation.
    ///         <list type="bullet">
    ///           <item>0 = not copied.</item>
    ///           <item>1 = copied.</item>
    ///         </list>
    ///       </para>
    ///     </description>
    ///   </item>
    ///   <item><term>2 bits</term>
    ///     <description>
    ///       Option class.
    ///       <para>
    ///         The option classes are:
    ///         <list type="bullet">
    ///           <item>0 = control.</item>
    ///           <item>1 = reserved for future use.</item>
    ///           <item>2 = debugging and measurement.</item>
    ///           <item>3 = reserved for future use.</item>
    ///         </list>
    ///       </para>
    ///     </description>
    ///   </item>
    ///   <item><term>5 bits</term><description>Option number.</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>DoD Extended Security Option (133), RFC 1108, is not supported because it only defines abstract option and no concrete option RFC is available.</item>
    ///   <item>EIP: The Extended Internet Protocol, RFC 1385, is not supported because according iana.org its option type is 145 but according to the RFC its option type is 138 (0x8A).</item>
    /// </list>
    /// </remarks>
    public enum IpV4OptionType : byte
    {
        /// <summary>
        /// End of Option list.  
        /// This option occupies only 1 octet; it has no length octet.
        /// </summary>
        EndOfOptionList = 0,

        /// <summary>
        /// No Operation.  
        /// This option occupies only 1 octet; it has no length octet.
        /// </summary>
        NoOperation = 1,

        /// <summary>
        /// Quick Start (QS). RFC 4782.  
        /// </summary>
        QuickStart = 25,

        /// <summary>
        /// Traceroute Using an IP Option.
        /// RFC 1393.
        /// </summary>
        TraceRoute = 82,

        /// <summary>
        /// DoD Basic Security:  
        /// Used to carry the classification level and protection authority flags.
        /// </summary>
        BasicSecurity = 130,

        /// <summary>
        /// Loose Source Routing.
        /// Used to route the internet datagram based on information supplied by the source.
        /// </summary>
        LooseSourceRouting = 131,

        /// <summary>
        /// Strict Source Routing.  
        /// Used to route the internet datagram based on information supplied by the source.
        /// </summary>
        StrictSourceRouting = 137,

        /// <summary>
        /// Record Route.  
        /// Used to trace the route an internet datagram takes.
        /// </summary>
        RecordRoute = 7,

        /// <summary>
        /// Stream ID.  
        /// Used to carry the stream identifier.
        /// </summary>
        StreamIdentifier = 136,

        /// <summary>
        /// Internet Timestamp.
        /// </summary>
        InternetTimestamp = 68,

        /// <summary>
        /// Router Alert Option (RFC 2113).
        /// </summary>
        RouterAlert = 148
    }
}