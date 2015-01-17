using System;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Maximum Segment Size (RFC 793)
    /// <pre>
    /// +-----+------+--------+
    /// | Bit | 0-7  | 8-15   |
    /// +-----+------+--------+
    /// | 0   | Kind | Length |
    /// +-----+------+--------+
    /// | 16  | max seg size  |
    /// +-----+---------------+
    /// </pre>
    /// 
    /// <para>
    /// If this option is present, then it communicates the maximum receive segment size at the TCP which sends this segment.
    /// This field must only be sent in the initial connection request (i.e., in segments with the SYN control bit set).  
    /// If this option is not used, any segment size is allowed.
    /// </para>
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.MaximumSegmentSize)]
    public sealed class TcpOptionMaximumSegmentSize : TcpOptionComplex, IOptionComplexFactory
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 4;

        /// <summary>
        /// The number of bytes this option value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Creates the option using the given maximum segment size.
        /// </summary>
        public TcpOptionMaximumSegmentSize(ushort maximumSegmentSize)
            : base(TcpOptionType.MaximumSegmentSize)
        {
            MaximumSegmentSize = maximumSegmentSize;
        }

        /// <summary>
        /// The default maximum segment size is 0.
        /// </summary>
        public TcpOptionMaximumSegmentSize()
            : this(0)
        {
        }

        /// <summary>
        /// The maximum segment size value.
        /// </summary>
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

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        Option IOptionComplexFactory.CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionValueLength)
                return null;

            ushort maximumSegmentSize = buffer.ReadUShort(ref offset, Endianity.Big);
            return new TcpOptionMaximumSegmentSize(maximumSegmentSize);
        }

        internal override bool EqualsData(TcpOption other)
        {
            return EqualsData(other as TcpOptionMaximumSegmentSize);
        }

        internal override int GetDataHashCode()
        {
            return MaximumSegmentSize.GetHashCode();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, MaximumSegmentSize, Endianity.Big);
        }

        private bool EqualsData(TcpOptionMaximumSegmentSize other)
        {
            return other != null &&
                   MaximumSegmentSize == other.MaximumSegmentSize;
        }
    }
}