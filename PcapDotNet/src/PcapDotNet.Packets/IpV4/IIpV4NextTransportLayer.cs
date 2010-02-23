namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// A Transport layer under an IPv4 layer.
    /// Must supply information about the Transport layer checksum.
    /// </summary>
    public interface IIpV4NextTransportLayer : IIpV4NextLayer
    {
        ushort? Checksum { get; set; }
        bool CalculateChecksum { get; }
        int NextLayerChecksumOffset { get; }
        bool NextLayerIsChecksumOptional { get; }
        bool NextLayerCalculateChecksum { get; }
    }
}