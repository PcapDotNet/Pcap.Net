namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// A layer under an Ethernet layer.
    /// Must provide the Ethernet Type and the default destination MAC address (if any).
    /// </summary>
    public interface IEthernetNextLayer : ILayer
    {
        EthernetType PreviousLayerEtherType { get; }
        MacAddress? PreviousLayerDefaultDestination { get; }
    }
}