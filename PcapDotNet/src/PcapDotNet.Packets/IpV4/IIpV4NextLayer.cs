namespace PcapDotNet.Packets.IpV4
{
    public interface IIpV4NextLayer : ILayer
    {
        IpV4Protocol PreviousLayerProtocol { get; }
    }
}