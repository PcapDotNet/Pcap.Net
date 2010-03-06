using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// RFC 793.
    /// Represents the TCP layer.
    /// <seealso cref="TcpDatagram"/>
    /// </summary>
    public class TcpLayer : TransportLayer
    {
        /// <summary>
        /// The sequence number of the first data octet in this segment (except when SYN is present). 
        /// If SYN is present the sequence number is the initial sequence number (ISN) and the first data octet is ISN+1.
        /// </summary>
        public uint SequenceNumber { get; set; }

        /// <summary>
        /// If the ACK control bit is set this field contains the value of the next sequence number 
        /// the sender of the segment is expecting to receive.  
        /// Once a connection is established this is always sent.
        /// </summary>
        public uint AcknowledgmentNumber { get; set; }

        /// <summary>
        /// A collection of bits for the TCP control.
        /// </summary>
        public TcpControlBits ControlBits { get; set; }

        /// <summary>
        /// The number of data octets beginning with the one indicated in the acknowledgment field which the sender of this segment is willing to accept.
        /// </summary>
        public ushort Window { get; set; }

        /// <summary>
        /// This field communicates the current value of the urgent pointer as a positive offset from the sequence number in this segment.  
        /// The urgent pointer points to the sequence number of the octet following the urgent data.  
        /// This field is only be interpreted in segments with the URG control bit set.
        /// </summary>
        public ushort UrgentPointer { get; set; }

        /// <summary>
        /// The TCP options contained in this TCP Datagram.
        /// </summary>
        public TcpOptions Options { get; set; }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        public override IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Tcp; }
        }

        /// <summary>
        /// The offset in the layer where the checksum should be written.
        /// </summary>
        public override int ChecksumOffset
        {
            get { return TcpDatagram.Offset.Checksum; }
        }

        /// <summary>
        /// Whether the checksum is optional in the layer.
        /// </summary>
        public override bool IsChecksumOptional
        {
            get { return false; }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return TcpDatagram.HeaderMinimumLength + Options.BytesLength; }
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
            TcpDatagram.WriteHeader(buffer, offset,
                                    SourcePort, DestinationPort,
                                    SequenceNumber, AcknowledgmentNumber,
                                    ControlBits, Window, UrgentPointer,
                                    Options);
        }

        /// <summary>
        /// True iff the SequenceNumber, AcknowledgmentNumber, ControlBits, Window, UrgentPointer and Options fields are equal.
        /// </summary>
        protected override sealed bool EqualFields(TransportLayer other)
        {
            return EqualFields(other as TcpLayer);
        }

        /// <summary>
        /// True iff the SequenceNumber, AcknowledgmentNumber, ControlBits, Window, UrgentPointer and Options fields are equal.
        /// </summary>
        private bool EqualFields(TcpLayer other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && AcknowledgmentNumber == other.AcknowledgmentNumber &&
                   ControlBits == other.ControlBits &&
                   Window == other.Window && UrgentPointer == other.UrgentPointer &&
                   Options.Equals(other.Options);
        }
    }
}