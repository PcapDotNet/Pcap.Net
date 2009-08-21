using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP POC-permitted Option (RFC 1693)
    /// +-----------+-------------+
    /// |  Kind=9   |  Length=2   |
    /// +-----------+-------------+
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.PartialOrderConnectionPermitted)]
    public class TcpOptionPartialOrderConnectionPermitted : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionPartialOrderConnectionPermitted>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 2;

        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public TcpOptionPartialOrderConnectionPermitted()
            : base(TcpOptionType.PartialOrderConnectionPermitted)
        {
        }

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

        public bool Equals(TcpOptionPartialOrderConnectionPermitted other)
        {
            return other != null;
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionPartialOrderConnectionPermitted);
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

            return _instance;
        }

        private static readonly TcpOptionPartialOrderConnectionPermitted _instance = new TcpOptionPartialOrderConnectionPermitted();
    }
}