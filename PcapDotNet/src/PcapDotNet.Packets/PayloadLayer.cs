using System;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents a layer that adds a simple payload.
    /// Actually can be any buffer of bytes.
    /// </summary>
    public sealed class PayloadLayer : SimpleLayer, IEquatable<PayloadLayer>
    {
        /// <summary>
        /// Creates an empty payload.
        /// </summary>
        public PayloadLayer()
        {
            Data = Datagram.Empty;
        }

        /// <summary>
        /// The Payload data.
        /// </summary>
        public Datagram Data { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return Data.Length; }
        }

        /// <summary>
        /// Two payload layers are equal if they have same data.
        /// </summary>
        public bool Equals(PayloadLayer other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }

        /// <summary>
        /// Two payload layers are equal if they have same data.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as PayloadLayer);
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// This method ignores the payload length, and the previous and next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected override void Write(byte[] buffer, int offset)
        {
            Data.Write(buffer, offset);
        }
    }
}