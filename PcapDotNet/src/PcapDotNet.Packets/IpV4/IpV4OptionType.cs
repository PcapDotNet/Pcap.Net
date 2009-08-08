namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The option-type octet is viewed as having 3 fields:
    /// 1 bit   copied flag,
    /// 2 bits  option class,
    /// 5 bits  option number.
    /// 
    /// The copied flag indicates that this option is copied into all fragments on fragmentation.
    /// 0 = not copied
    /// 1 = copied
    /// 
    /// The option classes are:
    /// 0 = control
    /// 1 = reserved for future use
    /// 2 = debugging and measurement
    /// 3 = reserved for future use
    /// </summary>
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
        /// DoD Basic Security:  
        /// Used to carry the classification level and protection authority flags.
        /// </summary>
        Security = 130,

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
        InternetTimestamp = 68
    }
}