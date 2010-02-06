namespace PcapDotNet.Packets
{
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
    }
}