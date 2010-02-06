namespace PcapDotNet.Packets
{
    public interface ILayer
    {
        int Length { get; }
        void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer);
        void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer);
        DataLinkKind? DataLink { get; }
    }
}