using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Represents IPv6 layer.
    /// <seealso cref="IpV6Datagram"/>
    /// </summary>
    public sealed class IpV6Layer : Layer, IEthernetNextLayer, IIpV4NextLayer
    {
        /// <summary>
        /// Creates an IPv6 layer with all zero values.
        /// </summary>
        public IpV6Layer()
        {
            ExtensionHeaders = IpV6ExtensionHeaders.Empty;
        }

        /// <summary>
        /// Available for use by originating nodes and/or forwarding routers to identify and distinguish between different classes or priorities of 
        /// IPv6 packets.
        /// </summary>
        public byte TrafficClass { get; set; }

        /// <summary>
        /// May be used by a source to label sequences of packets for which it requests special handling by the IPv6 routers, 
        /// such as non-default quality of service or "real-time" service.
        /// Hosts or routers that do not support the functions of the Flow Label field are required to set the field to zero when originating a packet,
        /// pass the field on unchanged when forwarding a packet, and ignore the field when receiving a packet.
        /// </summary>
        public int FlowLabel { get; set; }

        /// <summary>
        /// Identifies the type of header immediately following the IPv6 header.
        /// Uses the same values as the IPv4 Protocol field.
        /// null means that this value should be calculated according to the next layer.
        /// </summary>
        public IpV4Protocol? NextHeader { get; set; }

        /// <summary>
        /// Decremented by 1 by each node that forwards the packet.
        /// The packet is discarded if Hop Limit is decremented to zero.
        /// </summary>
        public byte HopLimit { get; set; }

        /// <summary>
        /// Address of the originator of the packet.
        /// </summary>
        public IpV6Address Source { get; set; }

        /// <summary>
        /// Address of the intended recipient of the packet (possibly not the ultimate recipient, if a Routing header is present).
        /// </summary>
        public IpV6Address CurrentDestination { get; set; }

        /// <summary>
        /// The IPv6 extension headers.
        /// </summary>
        public IpV6ExtensionHeaders ExtensionHeaders { get; set; }

        /// <summary>
        /// The Ethernet Type the Ethernet layer should write when this layer is the Ethernet payload.
        /// </summary>
        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.IpV6; }
        }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer (in this case: IP).
        /// </summary>
        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.IpV6; }
        }

        /// <summary>
        /// The default MAC Address value when this layer is the Ethernet payload.
        /// null means there is no default value.
        /// </summary>
        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return null; }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return IpV6Datagram.HeaderLength + ExtensionHeaders.Sum(header => header.Length); }
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
            IpV4Protocol nextHeader;
            if (NextHeader == null)
            {
                if (ExtensionHeaders.FirstHeader != null)
                {
                    nextHeader = ExtensionHeaders.FirstHeader.Value;
                }
                else
                {
                    if (nextLayer == null)
                        throw new ArgumentException("Can't determine protocol automatically from next layer because there is no next layer");
                    IIpV4NextLayer ipV4NextLayer = nextLayer as IIpV4NextLayer;
                    if (ipV4NextLayer == null)
                        throw new ArgumentException("Can't determine protocol automatically from next layer (" + nextLayer.GetType() + ")");
                    nextHeader = ipV4NextLayer.PreviousLayerProtocol;
                }
            }
            else
            {
                nextHeader = NextHeader.Value;
            }

            if (nextLayer != null && ExtensionHeaders.LastHeader == IpV4Protocol.EncapsulatingSecurityPayload)
                throw new ArgumentException("Cannot have a layer after IpV6Layer with EncapsulatingSecurityPayload extension header.", "nextLayer");

            IpV6Datagram.WriteHeader(buffer, offset,
                                     TrafficClass, FlowLabel, (ushort)(payloadLength + ExtensionHeaders.Sum(header => header.Length)), nextHeader, HopLimit, Source, CurrentDestination, ExtensionHeaders);
        }

        /// <summary>
        /// True iff the two IPv4 layers have the same TypeOfService, Identification, Fragmentation, Ttl, Protocol, HeaderChecksum, Source, Destination and Options.
        /// </summary>
        public bool Equals(IpV6Layer other)
        {
            return other != null &&
                   TrafficClass == other.TrafficClass && FlowLabel == other.FlowLabel &&
                   NextHeader == other.NextHeader && HopLimit == other.HopLimit &&
                   Source == other.Source && CurrentDestination == other.CurrentDestination &&
                   ExtensionHeaders.SequenceEqual(other.ExtensionHeaders);
        }

        /// <summary>
        /// True iff the two IPv4 layers have the same TypeOfService, Identification, Fragmentation, Ttl, Protocol, HeaderChecksum, Source, Destination and Options.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as IpV6Layer);
        }

        /// <summary>
        /// Returns a hash code for the layer.
        /// The hash code is a XOR of the TrafficClass and HopLimit combined and the hash codes of the FlowLabel, Source, CurrentDestination, NextHeader, HeaderChecksum, ExtensionHeaders.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Sequence.GetHashCode(BitSequence.Merge(TrafficClass, HopLimit),
                                        FlowLabel, Source, CurrentDestination, NextHeader) ^ ExtensionHeaders.SequenceGetHashCode();
        }

        /// <summary>
        /// Contains the Source, Destination and Protocol.
        /// </summary>
        public override string ToString()
        {
            return Source + " -> " + CurrentDestination + " (" + NextHeader + ")";
        }
    }
}