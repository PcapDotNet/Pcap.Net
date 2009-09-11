namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// The type of the checksum to be used.
    /// </summary>
    public enum TcpOptionAlternateChecksumType : byte
    {
        /// <summary>
        /// TCP checksum.
        /// </summary>
        TcpChecksum = 0,

        /// <summary>
        /// 8-bit Fletcher's algorithm.
        /// </summary>
        FletchersAlgorithm8Bit = 1,

        /// <summary>
        /// 16-bit Fletcher's algorithm.
        /// </summary>
        FletchersAlgorithm16Bit = 2,
    }
}