using System;
using PcapDotNet.Packets.Arp;

namespace PcapDotNet.Packets.Ethernet
{
    public class EthernetLayer : Layer, IArpPreviousLayer
    {
        public MacAddress Source { get; set; }
        public MacAddress Destination { get; set; }
        public EthernetType EtherType { get; set; }

        public EthernetLayer()
        {
            Source = MacAddress.Zero;
            Destination = MacAddress.Zero;
            EtherType = EthernetType.None;
        }

        public override int Length
        {
            get { return EthernetDatagram.HeaderLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            EthernetType etherType = EtherType;
            MacAddress destination = Destination;

            IEthernetNextLayer ethernetNextLayer = nextLayer as IEthernetNextLayer;
            if (etherType == EthernetType.None)
            {
                if (nextLayer == null)
                    throw new ArgumentException("Can't determine ether type automatically from next layer because there is not next layer");
                if (ethernetNextLayer == null)
                    throw new ArgumentException("Can't determine ether type automatically from next layer (" + nextLayer.GetType() + ")");
                etherType = ethernetNextLayer.PreviousLayerEtherType;
            }
            if (destination == MacAddress.Zero)
            {
                if (ethernetNextLayer != null && ethernetNextLayer.PreviousLayerDefaultDestination != null)
                    destination = ethernetNextLayer.PreviousLayerDefaultDestination.Value;
            }

            EthernetDatagram.WriteHeader(buffer, 0, Source, destination, etherType);
        }

        public override DataLinkKind? DataLink
        {
            get { return DataLinkKind.Ethernet; }
        }

        public ArpHardwareType PreviousLayerHardwareType
        {
            get { return ArpHardwareType.Ethernet; }
        }

        public bool Equals(EthernetLayer other)
        {
            return other != null &&
                   Source == other.Source && Destination == other.Destination && EtherType == other.EtherType;
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as EthernetLayer);
        }

        public override string ToString()
        {
            return Source + " -> " + Destination + " (" + EtherType + ")";
        }
    }
}