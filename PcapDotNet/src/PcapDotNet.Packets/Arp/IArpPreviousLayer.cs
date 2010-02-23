namespace PcapDotNet.Packets.Arp
{
    /// <summary>
    /// A layer that contains an ARP layer.
    /// Must provide the ARP hardware type.
    /// </summary>
    public interface IArpPreviousLayer : ILayer
    {
        ArpHardwareType PreviousLayerHardwareType { get; }   
    }
}