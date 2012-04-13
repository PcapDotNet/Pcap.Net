using PcapDotNet.Base;

namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// Represents an Ethernet layer.
    /// <seealso cref="EthernetDatagram"/>
    /// </summary>
    public sealed class EthernetLayer : EthernetBaseLayer
    {
        /// <summary>
        /// Creates an instance with zero values.
        /// </summary>
        public EthernetLayer()
        {
            Source = MacAddress.Zero;
            Destination = MacAddress.Zero;
        }

        /// <summary>
        /// Ethernet source address.
        /// </summary>
        public MacAddress Source { get; set; }

        /// <summary>
        /// Ethernet destination address.
        /// </summary>
        public MacAddress Destination { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return EthernetDatagram.HeaderLengthValue; }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        /// <param name="payloadLength">The length of the layer's payload (the number of bytes after the layer in the packet).</param>
        /// <param name="previousLayer">The layer that comes before this layer. null if this is the first layer.</param>
        /// <param name="nextLayer">The layer that comes after this layer. null if this is the last layer.</param>
        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            EthernetType etherType = GetEthernetType(EtherType, nextLayer);
            
            MacAddress destination = Destination;
            IEthernetNextLayer ethernetNextLayer = nextLayer as IEthernetNextLayer;
            if (destination == MacAddress.Zero)
            {
                if (ethernetNextLayer != null && ethernetNextLayer.PreviousLayerDefaultDestination != null)
                    destination = ethernetNextLayer.PreviousLayerDefaultDestination.Value;
            }

            EthernetDatagram.WriteHeader(buffer, offset, Source, destination, etherType);
        }

        /// <summary>
        /// The kind of the data link of the layer.
        /// Can be null if this is not the first layer in the packet.
        /// </summary>
        public override DataLinkKind? DataLink
        {
            get { return DataLinkKind.Ethernet; }
        }

        /// <summary>
        /// Two Ethernet layers are equal if they have the same source, destination and ethernet type.
        /// </summary>
        public bool Equals(EthernetLayer other)
        {
            return other != null &&
                   Source == other.Source && Destination == other.Destination && EtherType == other.EtherType;
        }

        /// <summary>
        /// Two Ethernet layers are equal if they have the same source, destination and ethernet type.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as EthernetLayer);
        }

        /// <summary>
        /// Returns a hash code for the layer.
        /// The hash code is a XOR of the hash codes of the layer length, data link, source and destination addresses and the ethernet type.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Sequence.GetHashCode(Source, Destination, EtherType);
        }

        /// <summary>
        /// Contains the source, destination and ether type.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Source + " -> " + Destination + " (" + EtherType + ")";
        }
    }
}