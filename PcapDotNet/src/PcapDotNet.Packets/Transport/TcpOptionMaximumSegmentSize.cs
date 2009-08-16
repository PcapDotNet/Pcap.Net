using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Maximum Segment Size (RFC 793)
    /// +--------+--------+---------+--------+
    /// |00000010|00000100|   max seg size   |
    /// +--------+--------+---------+--------+
    ///  Kind=2   Length=4
    /// 
    /// If this option is present, then it communicates the maximum receive segment size at the TCP which sends this segment.
    /// This field must only be sent in the initial connection request (i.e., in segments with the SYN control bit set).  
    /// If this option is not used, any segment size is allowed.
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.MaximumSegmentSize)]
    public class TcpOptionMaximumSegmentSize : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionMaximumSegmentSize>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 4;

        public TcpOptionMaximumSegmentSize(ushort maximumSegmentSize)
            : base(TcpOptionType.MaximumSegmentSize)
        {
            MaximumSegmentSize = maximumSegmentSize;
        }

        public TcpOptionMaximumSegmentSize()
            : this(0)
        {
        }

        public ushort MaximumSegmentSize { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        public bool Equals(TcpOptionMaximumSegmentSize other)
        {
            if (other == null)
                return false;
            return MaximumSegmentSize == other.MaximumSegmentSize;
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionMaximumSegmentSize);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   MaximumSegmentSize.GetHashCode();
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        public Option CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionHeaderLength)
                return null;

            ushort maximumSegmentSize = buffer.ReadUShort(ref offset, Endianity.Big);
            return new TcpOptionMaximumSegmentSize(maximumSegmentSize);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, MaximumSegmentSize, Endianity.Big);
        }
    }
}