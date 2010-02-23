namespace PcapDotNet.Packets
{
    /// <summary>
    /// A simple layer is a layer that doesn't care what is the length of its payload, what layer comes after it and what layer comes before it.
    /// </summary>
    public abstract class SimpleLayer : Layer
    {
        public override sealed void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            Write(buffer, offset);
        }

        protected abstract void Write(byte[] buffer, int offset);
    }
}