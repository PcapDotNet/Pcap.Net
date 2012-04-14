using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// RFC 768.
    /// Represents the UDP layer.
    /// <seealso cref="UdpDatagram"/>
    /// </summary>
    public sealed class UdpLayer : TransportLayer
    {
        /// <summary>
        /// Whether the checksum should be calculated.
        /// Can be false in UDP because the checksum is optional. false means that the checksum will be left zero.
        /// </summary>
        public override bool CalculateChecksum
        {
            get { return CalculateChecksumValue; }
        }

        /// <summary>
        /// Whether the checksum should be calculated.
        /// Can be false in UDP because the checksum is optional. false means that the checksum will be left zero.
        /// </summary>
        public bool CalculateChecksumValue { get; set; }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        public override IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Udp; }
        }

        /// <summary>
        /// The offset in the layer where the checksum should be written.
        /// </summary>
        public override int ChecksumOffset
        {
            get { return UdpDatagram.Offset.Checksum; }
        }

        /// <summary>
        /// Whether the checksum is optional in the layer.
        /// </summary>
        public override bool IsChecksumOptional
        {
            get { return true; }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return UdpDatagram.HeaderLength; }
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
            UdpDatagram.WriteHeader(buffer, offset, SourcePort, DestinationPort, payloadLength);
        }
    }
}