namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// A layer under an IPv4 layer.
    /// Must provide the IPv4 Protocol.
    /// </summary>
    public interface IIpV4NextLayer : ILayer
    {
        IpV4Protocol PreviousLayerProtocol { get; }
    }
}