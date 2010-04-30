namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// The GRE Version Number.
    /// </summary>
    public enum GreVersion : byte
    {
        /// <summary>
        /// RFC 1701, RFC 2784
        /// </summary>
        Gre = 0x00,

        /// <summary>
        /// RFC 2637
        /// </summary>
        EnhancedGre = 0x01
    }
}