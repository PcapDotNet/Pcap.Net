namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// A layer under an IP layer.
    /// Must provide the IPv4 Protocol.
    /// </summary>
    public interface IIpNextLayer : ILayer
    {
        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        IpV4Protocol PreviousLayerProtocol { get; }
    }
}