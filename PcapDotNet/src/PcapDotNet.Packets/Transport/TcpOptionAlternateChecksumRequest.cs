using System;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Alternate Checksum Request Option (RFC 1146).
    /// <pre>
    /// +-----+----------+
    /// | Bit | 0-7      |
    /// +-----+----------+
    /// | 0   | Kind     |
    /// +-----+----------+
    /// | 8   | Length   |
    /// +-----+----------+
    /// | 16  | Checksum |
    /// +-----+----------+
    /// </pre>
    /// 
    /// <para>
    /// Here chksum is a number identifying the type of checksum to be used.
    /// </para>
    /// 
    /// <para>
    /// The currently defined values of chksum are:
    /// <list>
    ///   <item>0  - TCP checksum.</item>
    ///   <item>1  - 8-bit  Fletcher's algorithm.</item>
    ///   <item>2  - 16-bit Fletcher's algorithm.</item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    /// Note that the 8-bit Fletcher algorithm gives a 16-bit checksum and the 16-bit algorithm gives a 32-bit checksum.
    /// </para>
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.AlternateChecksumRequest)]
    public sealed class TcpOptionAlternateChecksumRequest : TcpOptionComplex, IOptionComplexFactory
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 3;

        /// <summary>
        /// The number of bytes this option value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Creates the option using the given checksum type.
        /// </summary>
        public TcpOptionAlternateChecksumRequest(TcpOptionAlternateChecksumType checksumType)
            : base(TcpOptionType.AlternateChecksumRequest)
        {
            ChecksumType = checksumType;
        }

        /// <summary>
        /// The default checksum type is the TCP checksum.
        /// </summary>
        public TcpOptionAlternateChecksumRequest()
            : this(TcpOptionAlternateChecksumType.TcpChecksum)
        {
        }

        /// <summary>
        /// The type of the checksum.
        /// </summary>
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

            byte checksumType = buffer.ReadByte(ref offset);
            return new TcpOptionAlternateChecksumRequest((TcpOptionAlternateChecksumType)checksumType);
        }

        internal override bool EqualsData(TcpOption other)
        {
            return EqualsData(other as TcpOptionAlternateChecksumRequest);
        }

        internal override int GetDataHashCode()
        {
            return ChecksumType.GetHashCode();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, (byte)ChecksumType);
        }

        private bool EqualsData(TcpOptionAlternateChecksumRequest other)
        {
            return other != null &&
                   ChecksumType == other.ChecksumType;
        }
    }
}