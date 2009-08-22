using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Alternate Checksum Request Option (RFC 1146).
    /// +----------+----------+----------+
    /// |  Kind=14 | Length=3 |  chksum  |
    /// +----------+----------+----------+
    /// 
    /// Here chksum is a number identifying the type of checksum to be used.
    /// 
    /// The currently defined values of chksum are:
    /// 0  -- TCP checksum.
    /// 1  -- 8-bit  Fletcher's algorithm.
    /// 2  -- 16-bit Fletcher's algorithm.
    /// 
    /// Note that the 8-bit Fletcher algorithm gives a 16-bit checksum and the 16-bit algorithm gives a 32-bit checksum.
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.AlternateChecksumRequest)]
    public class TcpOptionAlternateChecksumRequest : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionAlternateChecksumRequest>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 3;

        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public TcpOptionAlternateChecksumRequest(TcpOptionAlternateChecksumType checksumType)
            : base(TcpOptionType.AlternateChecksumRequest)
        {
            ChecksumType = checksumType;
        }

        public TcpOptionAlternateChecksumRequest()
            : this(0)
        {
        }

        public TcpOptionAlternateChecksumType ChecksumType { get; private set; }

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

        public bool Equals(TcpOptionAlternateChecksumRequest other)
        {
            if (other == null)
                return false;
            return ChecksumType == other.ChecksumType;
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionAlternateChecksumRequest);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   ChecksumType.GetHashCode();
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
            if (valueLength != OptionValueLength)
                return null;

            byte checksumType = buffer.ReadByte(ref offset);
            return new TcpOptionAlternateChecksumRequest((TcpOptionAlternateChecksumType)checksumType);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, (byte)ChecksumType);
        }
    }
}