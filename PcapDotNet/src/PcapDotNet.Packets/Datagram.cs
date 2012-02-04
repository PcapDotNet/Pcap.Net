using System;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents a packet datagram.
    /// A datagram is part of the packet bytes that can be treated as a specific protocol data (usually header + payload).
    /// Never copies the given buffer.
    /// </summary>
    public class Datagram : DataSegment
    {
        /// <summary>
        /// Take all the bytes as a datagram.
        /// </summary>
        /// <param name="buffer">The buffer to take as a datagram.</param>
        public Datagram(byte[] buffer)
            : base(buffer)
        {
        }

        /// <summary>
        /// Take only part of the bytes as a datagram.
        /// </summary>
        /// <param name="buffer">The bytes to take the datagram from.</param>
        /// <param name="offset">The offset in the buffer to start taking the bytes from.</param>
        /// <param name="length">The number of bytes to take.</param>
        public Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// An empty datagram.
        /// Useful for empty payloads.
        /// </summary>
        public new static Datagram Empty
        {
            get { return _empty; }
        }

        /// <summary>
        /// A datagram is checked for validity according to the protocol.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                    _isValid = CalculateIsValid();
                return _isValid.Value;
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public virtual ILayer ExtractLayer()
        {
            return new PayloadLayer
                       {
                           Data = this
                       };
        }

        /// <summary>
        /// The default validity check always returns true.
        /// </summary>
        protected virtual bool CalculateIsValid()
        {
            return true;
        }

        private static readonly Datagram _empty = new Datagram(new byte[0]);
        private bool? _isValid;
    }
}