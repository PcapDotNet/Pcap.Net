namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035, 2136.
    /// Other sources: Dyer 1987, Moon 1981.
    /// CLASS fields appear in resource records.
    /// </summary>
    public enum DnsClass : ushort
    {
        None = 0,

        /// <summary>
        /// RFC 1035.
        /// IN - Internet.
        /// </summary>
        Internet = 1,

        /// <summary>
        /// Moon 1981 1035.
        /// The CHAOS class.
        /// </summary>
        Chaos = 3,

        /// <summary>
        /// Dyer 87.
        /// HS - Hesiod.
        /// </summary>
        Hesiod = 4,

        /// <summary>
        /// RFC 2136.
        /// None.
        /// Query class.
        /// </summary>
        NoneExistent = 254, 

        /// <summary>
        /// RFC 1035.
        /// *.
        /// Any class.
        /// Query class.
        /// </summary>
        Any = 255,
    }
}