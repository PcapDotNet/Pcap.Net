namespace PcapDotNet.Packets
{
    public abstract class SimpleLayer : Layer
    {
        public override sealed void Write(byte[] buffer, int offset, int payloadLength, ILayer nextLayer, ILayer nextLayer1)
        {
            Write(buffer, offset);
        }

        protected abstract void Write(byte[] buffer, int offset);
    }
}