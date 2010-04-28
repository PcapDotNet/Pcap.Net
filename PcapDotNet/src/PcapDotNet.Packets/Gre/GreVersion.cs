namespace PcapDotNet.Packets.Gre
{
    public enum GreVersion : byte
    {
        /// <summary>
        /// RFC 2784
        /// </summary>
        Gre = 0x00,

        /// <summary>
        /// RFC 2637
        /// </summary>
        EnhancedGre = 0x01
    }
}