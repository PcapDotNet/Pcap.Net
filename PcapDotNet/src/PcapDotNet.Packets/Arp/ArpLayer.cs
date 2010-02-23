using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.Arp
{
    public class ArpLayer : Layer, IEthernetNextLayer
    {
        public EthernetType ProtocolType { get; set; }

        public ArpOperation Operation { get; set; }

        public IList<byte> SenderHardwareAddress { get; set; }

        public IList<byte> SenderProtocolAddress { get; set; }

        public IList<byte> TargetHardwareAddress { get; set; }

        public IList<byte> TargetProtocolAddress { get; set; }

        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.Arp; }
        }

        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return EthernetDatagram.BroadcastAddress; }
        }

        public override int Length
        {
            get { return ArpDatagram.GetHeaderLength(SenderHardwareAddress.Count, SenderProtocolAddress.Count); }
        }

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

        public bool Equals(ArpLayer other)
        {
            return other != null &&
                   ProtocolType == other.ProtocolType && Operation == other.Operation &&
                   SenderHardwareAddress.SequenceEqual(other.SenderHardwareAddress) &&
                   SenderProtocolAddress.SequenceEqual(other.SenderProtocolAddress) &&
                   TargetHardwareAddress.SequenceEqual(other.TargetHardwareAddress) &&
                   TargetProtocolAddress.SequenceEqual(other.TargetProtocolAddress);
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as ArpLayer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   (((ushort)ProtocolType << 16) + (ushort)Operation);

        }
    }
}