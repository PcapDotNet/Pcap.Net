using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Represents an ICMP layer.
    /// <seealso cref="IcmpDatagram"/>
    /// </summary>
    public abstract class IcmpLayer : SimpleLayer, IIpV4NextLayer
    {
        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public abstract IcmpMessageType MessageType { get; }

        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public virtual byte CodeValue
        {
            get { return 0; }

        }

        /// <summary>
        /// A combination of the ICMP Message Type and Code.
        /// </summary>
        public IcmpMessageTypeAndCode MessageTypeAndCode
        {
            get { return (IcmpMessageTypeAndCode)(((ushort)MessageType << 8) | CodeValue); }
        }

        /// <summary>
        /// The checksum is the 16-bit ones's complement of the one's complement sum of the ICMP message starting with the ICMP Type.
        /// For computing the checksum, the checksum field should be zero.
        /// This checksum may be replaced in the future.
        /// null means that this checksum should be calculated to be correct.
        /// </summary>
        public ushort? Checksum { get; set; }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected virtual uint Variable 
        { 
            get { return 0; }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override sealed int Length
        {
            get { return IcmpDatagram.HeaderLength + PayloadLength; } 
        }

        /// <summary>
        /// The number of bytes the ICMP payload takes.
        /// </summary>
        protected virtual int PayloadLength
        {
            get { return 0; }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// This method ignores the payload length, and the previous and next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected override sealed void Write(byte[] buffer, int offset)
        {
            IcmpDatagram.WriteHeader(buffer, offset, MessageType, CodeValue, Variable);
            WritePayload(buffer, offset + IcmpDatagram.HeaderLength);
        }

        /// <summary>
        /// Writes the ICMP payload to the buffer.
        /// Doesn't include payload in the next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the ICMP payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing the payload at.</param>
        protected virtual void WritePayload(byte[] buffer, int offset)
        {
        }

        public override sealed void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
            IcmpDatagram.WriteChecksum(buffer, offset, Length + payloadLength, Checksum);
        }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetControlMessageProtocol; }
        }

        public virtual bool Equals(IcmpLayer other)
        {
            return other != null &&
                   MessageType == other.MessageType && CodeValue == other.CodeValue &&
                   Checksum == other.Checksum &&
                   Variable == other.Variable;
        }

        public sealed override bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as IcmpLayer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   MessageTypeAndCode.GetHashCode() ^ Checksum.GetHashCode() ^ Variable.GetHashCode();
        }

        public override string ToString()
        {
            return MessageType + "." + CodeValue + "(" + Variable + ")";
        }
    }
}