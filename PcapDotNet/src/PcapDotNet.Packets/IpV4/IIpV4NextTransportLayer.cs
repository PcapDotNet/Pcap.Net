namespace PcapDotNet.Packets.IpV4
{
    public interface IIpV4NextTransportLayer : IIpV4NextLayer
    {
        ushort? Checksum { get; set; }
        bool CalculateChecksum { get; }
        int NextLayerChecksumOffset { get; }
        bool NextLayerIsChecksumOptional { get; }
        bool NextLayerCalculateChecksum { get; }
    }
}