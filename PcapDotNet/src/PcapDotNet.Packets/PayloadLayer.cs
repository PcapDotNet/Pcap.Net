namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents a layer that adds a simple payload.
    /// Actually can be any buffer of bytes.
    /// </summary>
    public class PayloadLayer : SimpleLayer
    {
        public Datagram Data { get; set; }

        public PayloadLayer()
        {
            Data = Datagram.Empty;
        }

        public override int Length
        {
            get { return Data.Length; }
        }

        public bool Equals(PayloadLayer other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as PayloadLayer);
        }

        protected override void Write(byte[] buffer, int offset)
        {
            Data.Write(buffer, offset);
        }
    }
}