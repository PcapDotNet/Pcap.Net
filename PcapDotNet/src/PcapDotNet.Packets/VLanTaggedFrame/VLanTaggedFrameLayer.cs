using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.VLanTaggedFrame
{
    /// <summary>
    /// Represents an VLAN Tagged Frame layer.
    /// <seealso cref="VLanTaggedFrameDatagram"/>
    /// </summary>
    public sealed class VLanTaggedFrameLayer : EthernetBaseLayer, IEquatable<VLanTaggedFrameLayer>, IEthernetNextLayer
    {
        /// <summary>
        /// Creates an instance with zero values.
        /// </summary>
        public VLanTaggedFrameLayer()
        {
            PriorityCodePoint = ClassOfService.BestEffort;
            CanonicalFormatIndicator = false;
            VLanIdentifier = 0;
        }

        /// <summary>
        /// Indicates the frame priority level.
        /// Values are from 0 (best effort) to 7 (highest); 1 represents the lowest priority.
        /// These values can be used to prioritize different classes of traffic (voice, video, data, etc.).
        /// </summary>
        public ClassOfService PriorityCodePoint { get; set; }

        /// <summary>
        /// If reset, all MAC Address information that may be present in the MSDU is in Canonical format and the tag comprises solely the TPID and TCI fields,
        /// i.e., the tag does not contain an Embedded Routing Information Field (E-RIF).
        /// </summary>
        public bool CanonicalFormatIndicator { get; set; }

        /// <summary>
        /// A VLAN-aware Bridge may not support the full range of VID values but shall support the use of all VID values in the range 0 through a maximum N,
        /// less than or equal to 4094 and specified for that implementation.
        /// </summary>
        public ushort VLanIdentifier { get; set; }

        /// <summary>
        /// A combination of pcp (PriorityCodePoint), cfi (CanonicalFormatIndicator) and vid (VLanIdentifier).
        /// </summary>
        public ushort TagControlInformation
        {
            get { return VLanTaggedFrameDatagram.CalculateTagControlInformation(PriorityCodePoint, CanonicalFormatIndicator, VLanIdentifier); }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return VLanTaggedFrameDatagram.HeaderLengthValue; }
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
            VLanTaggedFrameDatagram.WriteHeader(buffer, offset, PriorityCodePoint, CanonicalFormatIndicator, VLanIdentifier, etherType);
        }

        /// <summary>
        /// Two VLAN Tagged Frame layers are equal iff their PriorityCodePoint, CanonicalFormatIndicator, VLanIdentifier and EtherType are equal.
        /// </summary>
        public bool Equals(VLanTaggedFrameLayer other)
        {
            return other != null &&
                   PriorityCodePoint == other.PriorityCodePoint && CanonicalFormatIndicator == other.CanonicalFormatIndicator &&
                   VLanIdentifier == other.VLanIdentifier && EtherType == other.EtherType;
        }

        /// <summary>
        /// Two VLAN Tagged Frame layers are equal iff their PriorityCodePoint, CanonicalFormatIndicator, VLanIdentifier and EtherType are equal.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as VLanTaggedFrameLayer);
        }

        /// <summary>
        /// Returns a hash code for the layer.
        /// The hash code is a XOR of the hash codes of the layer length, data link, TagControlInformation and the ethernet type.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   BitSequence.Merge(TagControlInformation, (ushort)EtherType).GetHashCode();
        }

        /// <summary>
        /// The Ethernet Type the Ethernet layer should write when this layer is the Ethernet payload.
        /// </summary>
        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.VLanTaggedFrame; }
        }

        /// <summary>
        /// The default MAC Address value when this layer is the Ethernet payload.
        /// null means there is no default value.
        /// </summary>
        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return null; }
        }
    }
}