using System;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.Arp
{
    /// <summary>
    /// Represents an ARP protocol layer.
    /// </summary>
    public sealed class ArpLayer : Layer, IEthernetNextLayer
    {
        /// <summary>
        /// Each protocol is assigned a number used in this field.
        /// </summary>
        public EthernetType ProtocolType { get; set; }

        /// <summary>
        /// Specifies the operation the sender is performing.
        /// </summary>
        public ArpOperation Operation { get; set; }

        /// <summary>
        /// Hardware address of the sender.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ReadOnlyCollection<byte> SenderHardwareAddress { get; set; }

        /// <summary>
        /// Protocol address of the sender.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ReadOnlyCollection<byte> SenderProtocolAddress { get; set; }

        /// <summary>
        /// Hardware address of the intended receiver. 
        /// This field is ignored in requests.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ReadOnlyCollection<byte> TargetHardwareAddress { get; set; }

        /// <summary>
        /// Protocol address of the intended receiver.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ReadOnlyCollection<byte> TargetProtocolAddress { get; set; }

        /// <summary>
        /// The Ethernet Type the Ethernet layer should write when this layer is the Ethernet payload.
        /// </summary>
        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.Arp; }
        }

        /// <summary>
        /// The default MAC Address value when this layer is the Ethernet payload.
        /// null means there is no default value.
        /// </summary>
        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return EthernetDatagram.BroadcastAddress; }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return ArpDatagram.GetHeaderLength(SenderHardwareAddress.Count, SenderProtocolAddress.Count); }
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
            if (previousLayer == null)
                throw new ArgumentNullException("previousLayer");

            IArpPreviousLayer arpPreviousLayer = previousLayer as IArpPreviousLayer;
            if (arpPreviousLayer == null)
                throw new ArgumentException("The layer before the ARP layer must be an ARP previous layer and can't be " + previousLayer.GetType());

            if (SenderHardwareAddress.Count != TargetHardwareAddress.Count)
            {
                throw new ArgumentException("Sender hardware address length is " + SenderHardwareAddress.Count + " bytes " +
                                            "while target hardware address length is " + TargetHardwareAddress.Count + " bytes");
            }
            if (SenderProtocolAddress.Count != TargetProtocolAddress.Count)
            {
                throw new ArgumentException("Sender protocol address length is " + SenderProtocolAddress.Count + " bytes " +
                                            "while target protocol address length is " + TargetProtocolAddress.Count + " bytes");
            }

            ArpDatagram.WriteHeader(buffer, offset,
                                    arpPreviousLayer.PreviousLayerHardwareType, ProtocolType, Operation,
                                    SenderHardwareAddress, SenderProtocolAddress, TargetHardwareAddress, TargetProtocolAddress);
        }

        /// <summary>
        /// True iff the two ARP layers have equal protocol type, operation and addresses.
        /// </summary>
        /// <param name="other">The ARP layer to compare the layer to.</param>
        /// <returns>True iff the two layers are equal.</returns>
        public bool Equals(ArpLayer other)
        {
            return other != null &&
                   ProtocolType == other.ProtocolType && Operation == other.Operation &&
                   SenderHardwareAddress.SequenceEqual(other.SenderHardwareAddress) &&
                   SenderProtocolAddress.SequenceEqual(other.SenderProtocolAddress) &&
                   TargetHardwareAddress.SequenceEqual(other.TargetHardwareAddress) &&
                   TargetProtocolAddress.SequenceEqual(other.TargetProtocolAddress);
        }

        /// <summary>
        /// True iff the two ARP layers have equal protocol type, operation and addresses.
        /// </summary>
        /// <param name="other">The ARP layer to compare the layer to.</param>
        /// <returns>True iff the two layers are equal.</returns>
        public override bool Equals(Layer other)
        {
            return Equals(other as ArpLayer);
        }

        /// <summary>
        /// Returns a hash code for the layer.
        /// The hash code is a XOR of a combination of the protocol type and operation and the hash codes of the layer length and data link.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   (((ushort)ProtocolType << 16) + (ushort)Operation);
        }
    }
}