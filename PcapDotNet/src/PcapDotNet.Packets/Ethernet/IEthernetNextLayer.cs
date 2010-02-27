namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// A layer under an Ethernet layer.
    /// Must provide the Ethernet Type and the default destination MAC address (if any).
    /// </summary>
    public interface IEthernetNextLayer : ILayer
    {
        /// <summary>
        /// The Ethernet Type the Ethernet layer should write when this layer is the Ethernet payload.
        /// </summary>
        EthernetType PreviousLayerEtherType { get; }

        /// <summary>
        /// The default MAC Address value when this layer is the Ethernet payload.
        /// null means there is no default value.
        /// </summary>
        MacAddress? PreviousLayerDefaultDestination { get; }
    }
}