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
        int Length { get; }
        void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer);
        void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer);
        DataLinkKind? DataLink { get; }
    }
}