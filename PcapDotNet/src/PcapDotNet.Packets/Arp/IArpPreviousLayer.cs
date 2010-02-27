namespace PcapDotNet.Packets.Arp
{
    /// <summary>
    /// A layer that contains an ARP layer.
    /// Must provide the ARP hardware type.
    /// </summary>
    public interface IArpPreviousLayer : ILayer
    {
        /// <summary>
        /// The ARP Hardware Type of the layer before the ARP layer.
        /// </summary>
        ArpHardwareType PreviousLayerHardwareType { get; }
    }
}