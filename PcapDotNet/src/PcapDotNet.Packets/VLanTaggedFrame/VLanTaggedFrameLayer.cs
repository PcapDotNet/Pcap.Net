using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.VLanTaggedFrame
{
    public sealed class VLanTaggedFrameLayer : Layer, IEquatable<VLanTaggedFrameLayer>, IEthernetNextLayer
    {
        public VLanTaggedFrameLayer()
        {
        }

        public ClassOfService PriorityCodePoint { get; set; }

        public bool CanonicalFormatIndicator { get; set; }

        public ushort VLanIdentifier { get; set; }

        public ushort TagControlInformation
        {
            get { return VLanTaggedFrameDatagram.CalculateTagControlInformation(PriorityCodePoint, CanonicalFormatIndicator, VLanIdentifier); }
        }

        public EthernetType EtherType { get; set; }

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

        public bool Equals(VLanTaggedFrameLayer other)
        {
            return other != null &&
                   PriorityCodePoint == other.PriorityCodePoint && CanonicalFormatIndicator == other.CanonicalFormatIndicator &&
                   VLanIdentifier == other.VLanIdentifier && EtherType == other.EtherType;
        }

        public override bool Equals(Layer other)
        {
            return Equals(other as VLanTaggedFrameLayer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   BitSequence.Merge(TagControlInformation, (ushort)EtherType).GetHashCode();
        }

        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.VLanTaggedFrame; }
        }

        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return null; }
        }
    }
}