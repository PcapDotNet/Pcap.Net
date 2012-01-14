namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035, 2136.
    /// Other sources: Dyer 1987, Moon 1981.
    /// CLASS fields appear in resource records.
    /// </summary>
    public enum DnsClass : ushort
    {
        /// <summary>
        /// RFC 1035.
        /// Internet.
        /// </summary>
        In = 1,

        /// <summary>
        /// Moon 1981 1035.
        /// The CHAOS class.
        /// </summary>
        Ch = 3,

        /// <summary>
        /// Dyer 87.
        /// Hesiod.
        /// </summary>
        Hs = 4,

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