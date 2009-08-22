using System;

namespace PcapDotNet.Packets.Transport
{
    public abstract class TcpOptionConnectionCountBase : TcpOptionComplex, IEquatable<TcpOptionConnectionCountBase>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 6;

        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public uint ConnectionCount { get; private set; }

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

        public bool Equals(TcpOptionConnectionCountBase other)
        {
            if (other == null)
                return false;

            return OptionType == other.OptionType &&
                   ConnectionCount == other.ConnectionCount;
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionConnectionCountBase);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, ConnectionCount, Endianity.Big);
        }
    
        protected TcpOptionConnectionCountBase(TcpOptionType optionType, uint connectionCount)
            : base(optionType)
        {
            ConnectionCount = connectionCount;
        }

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
    }
}