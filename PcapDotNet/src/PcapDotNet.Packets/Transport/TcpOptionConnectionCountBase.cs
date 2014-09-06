using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// The base class for connection count TCP options.
    /// </summary>
    public abstract class TcpOptionConnectionCountBase : TcpOptionComplex
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 6;

        /// <summary>
        /// The number of bytes this option value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// The connection count value.
        /// </summary>
        public uint ConnectionCount { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public sealed override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public sealed override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        internal sealed override bool EqualsData(TcpOption other)
        {
            return EqualsData(other as TcpOptionConnectionCountBase);
        }

        internal sealed override int GetDataHashCode()
        {
            return ConnectionCount.GetHashCode();
        }

        internal sealed override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, ConnectionCount, Endianity.Big);
        }
    
        /// <summary>
        /// Creates a connection count option according to the given option type and given connection count value.
        /// </summary>
        protected TcpOptionConnectionCountBase(TcpOptionType optionType, uint connectionCount)
            : base(optionType)
        {
            ConnectionCount = connectionCount;
        }

        /// <summary>
        /// Reads the connection count value from the buffer.
        /// </summary>
        /// <param name="connectionCount">The result connection count.</param>
        /// <param name="buffer">The buffer to read the connection count from.</param>
        /// <param name="offset">The offset to start reading the connection byte from.</param>
        /// <param name="valueLength">The number of bytes available for read in this buffer.</param>
        /// <returns>True iff the connection count could be read (there were enough bytes to read).</returns>
        protected static bool TryRead(out uint connectionCount, byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionValueLength)
            {
                connectionCount = 0;
                return false;
            }

            connectionCount = buffer.ReadUInt(ref offset, Endianity.Big);
            return true;
        }

        private bool EqualsData(TcpOptionConnectionCountBase other)
        {
            return other != null &&
                   OptionType == other.OptionType && ConnectionCount == other.ConnectionCount;
        }
    }
}