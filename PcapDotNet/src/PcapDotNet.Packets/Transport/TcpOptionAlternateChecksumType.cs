namespace PcapDotNet.Packets.Transport
{
    public enum TcpOptionAlternateChecksumType : byte
    {
        TcpChecksum = 0,
        FletchersAlgorithm8Bit = 1,
        FletchersAlgorithm16Bit = 2,
    }
}