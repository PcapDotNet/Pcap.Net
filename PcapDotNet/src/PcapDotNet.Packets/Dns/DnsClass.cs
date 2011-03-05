namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// CLASS fields appear in resource records.
    /// </summary>
    public enum DnsClass : ushort
    {
        /// <summary>
        /// The Internet.
        /// </summary>
        In = 1,

        /// <summary>
        /// The CSNET class (Obsolete - used only for examples in some obsolete RFCs).
        /// </summary>
        Cs = 2,
       
        /// <summary>
        /// The CHAOS class.
        /// </summary>
        Ch = 3,

        /// <summary>
        /// Hesiod [Dyer 87].
        /// </summary>
        Hs = 4,

        /// <summary>
        /// *.
        /// Any class.
        /// Query Class.
        /// </summary>
        Any = 255,

    }
}