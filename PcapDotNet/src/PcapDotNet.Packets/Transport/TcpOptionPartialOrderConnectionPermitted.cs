using System;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP POC-permitted Option (RFC 1693)
    /// <pre>
    /// +-----------+-------------+
    /// |  Kind=9   |  Length=2   |
    /// +-----------+-------------+
    /// </pre>
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.PartialOrderConnectionPermitted)]
    public sealed class TcpOptionPartialOrderConnectionPermitted : TcpOptionComplex, IOptionComplexFactory
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 2;

        /// <summary>
        /// The number of bytes this option value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Creates a partial order connection permitted option.
        /// </summary>
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

            return _instance;
        }

        internal override bool EqualsData(TcpOption other)
        {
            return (other is TcpOptionPartialOrderConnectionPermitted);
        }

        internal override int GetDataHashCode()
        {
            return 0;
        }

        private static readonly TcpOptionPartialOrderConnectionPermitted _instance = new TcpOptionPartialOrderConnectionPermitted();
    }
}