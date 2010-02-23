namespace PcapDotNet.Packets
{
    /// <summary>
    /// The base class of a layer used to build a Packet.
    /// Each layer represents the part of the packet relevant to a specific protocol.
    /// A sequence of layers can represent a packet.
    /// A packet can be according to a sequence of layers.
    /// <seealso cref="PacketBuilder"/>
    /// </summary>
    public abstract class Layer : ILayer
    {
        public abstract int Length { get; }
        public abstract void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer);
        public virtual void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
        }

        public virtual DataLinkKind? DataLink
        {
            get { return null; }
        }

        public virtual bool Equals(Layer other)
        {
            return other != null &&
                   Length == other.Length && DataLink == other.DataLink;
        }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as Layer);
        }

        public override int GetHashCode()
        {
            return Length.GetHashCode() ^ DataLink.GetHashCode();
        }
    }
}