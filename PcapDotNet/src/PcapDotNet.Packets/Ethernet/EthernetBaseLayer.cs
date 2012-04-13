using System;
using PcapDotNet.Packets.Arp;

namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// A base class for Ethernet like layers.
    /// Contains an Ethernet type that can be calculated according to the previous layer 
    /// and defines that if the next layer is an ARP layer, the hardware type should be Ethernet.
    /// </summary>
    public abstract class EthernetBaseLayer : Layer, IArpPreviousLayer
    {
        /// <summary>
        /// Ethernet type (next protocol).
        /// </summary>
        public EthernetType EtherType { get; set; }

        /// <summary>
        /// The ARP Hardware Type of the layer before the ARP layer.
        /// </summary>
        public ArpHardwareType PreviousLayerHardwareType
        {
            get { return ArpHardwareType.Ethernet; }
        }

        /// <summary>
        /// Creates an instance with zero values.
        /// </summary>
        protected EthernetBaseLayer()
        {
            EtherType = EthernetType.None;
        }

        internal static EthernetType GetEthernetType(EthernetType ethernetType, ILayer nextLayer)
        {
            if (ethernetType != EthernetType.None)
                return ethernetType;

            if (nextLayer == null)
                throw new ArgumentException("Can't determine ether type automatically from next layer because there is not next layer");
            IEthernetNextLayer ethernetNextLayer = nextLayer as IEthernetNextLayer;
            if (ethernetNextLayer == null)
                throw new ArgumentException("Can't determine ether type automatically from next layer (" + nextLayer.GetType() + ")");

            return ethernetNextLayer.PreviousLayerEtherType;
        }
    }
}