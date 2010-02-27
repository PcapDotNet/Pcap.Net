namespace PcapDotNet.Packets
{
    /// <summary>
    /// The interface of a layer used to build a Packet.
    /// Each layer represents the part of the packet relevant to a specific protocol.
    /// A sequence of layers can represent a packet.
    /// A packet can be according to a sequence of layers.
    /// <seealso cref="PacketBuilder"/>
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Writes the layer to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        /// <param name="payloadLength">The length of the layer's payload (the number of bytes after the layer in the packet).</param>
        /// <param name="previousLayer">The layer that comes before this layer. null if this is the first layer.</param>
        /// <param name="nextLayer">The layer that comes after this layer. null if this is the last layer.</param>
        void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer);

        /// <summary>
        /// Finalizes the layer data in the buffer.
        /// Used for fields that must be calculated according to the layer's payload (like checksum).
        /// </summary>
        /// <param name="buffer">The buffer to finalize the layer in.</param>
        /// <param name="offset">The offset in the buffer the layer starts.</param>
        /// <param name="payloadLength">The length of the layer's payload (the number of bytes after the layer in the packet).</param>
        /// <param name="nextLayer">The layer that comes after this layer. null if this is the last layer.</param>
        void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer);

        /// <summary>
        /// The kind of the data link of the layer.
        /// Can be null if this is not the first layer in the packet.
        /// </summary>
        DataLinkKind? DataLink { get; }
    }
}