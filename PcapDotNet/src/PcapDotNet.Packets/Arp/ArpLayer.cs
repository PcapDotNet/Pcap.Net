using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.Arp
{
    /// <summary>
    /// Represents an ARP protocol layer.
    /// </summary>
    public class ArpLayer : Layer, IEthernetNextLayer
    {
        /// <summary>
        /// Create an ARP layer by giving the different addresses.
        /// </summary>
        /// <param name="senderHardwareAddress">Hardware address of the sender.</param>
        /// <param name="senderProtocolAddress">Protocol address of the sender.</param>
        /// <param name="targetHardwareAddress">Hardware address of the intended receiver. This field is ignored in requests.</param>
        /// <param name="targetProtocolAddress">Protocol address of the intended receiver.</param>
        public ArpLayer(IList<byte> senderHardwareAddress, IList<byte> senderProtocolAddress, IList<byte> targetHardwareAddress, IList<byte> targetProtocolAddress)
        {
            _senderHardwareAddress = senderHardwareAddress;
            _senderProtocolAddress = senderProtocolAddress;
            _targetHardwareAddress = targetHardwareAddress;
            _targetProtocolAddress = targetProtocolAddress;
        }

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
        public IList<byte> SenderHardwareAddress { get { return _senderHardwareAddress; } }

        /// <summary>
        /// Protocol address of the sender.
        /// </summary>
        public IList<byte> SenderProtocolAddress { get { return _senderProtocolAddress; } }

        /// <summary>
        /// Hardware address of the intended receiver. 
        /// This field is ignored in requests.
        /// </summary>
        public IList<byte> TargetHardwareAddress { get { return _targetHardwareAddress; } }

        /// <summary>
        /// Protocol address of the intended receiver.
        /// </summary>
        public IList<byte> TargetProtocolAddress { get { return _targetProtocolAddress; } }

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
                throw new ArgumentException("Must have a previous layer");

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
        public override sealed bool Equals(Layer other)
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

        private readonly IList<byte> _senderHardwareAddress;
        private readonly IList<byte> _senderProtocolAddress;
        private readonly IList<byte> _targetHardwareAddress;
        private readonly IList<byte> _targetProtocolAddress;
    }
}