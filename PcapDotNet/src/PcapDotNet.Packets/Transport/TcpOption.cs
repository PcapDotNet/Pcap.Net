using System;

namespace PcapDotNet.Packets.Transport
{
    public abstract class TcpOption : Option, IEquatable<TcpOption>
    {
        public static TcpOption End
        {
            get { return _end; }
        }

        public static TcpOption Nop
        {
            get { return _nop; }
        }

        public TcpOption(TcpOptionType optionType)
        {
            OptionType = optionType;
        }

        public TcpOptionType OptionType { get; private set; }

        public override bool Equivalent(Option other)
        {
            return OptionType == ((TcpOption)other).OptionType;
        }

        /// <summary>
        /// Checks if the two options are exactly the same - including type and value.
        /// </summary>
        public virtual bool Equals(TcpOption other)
        {
            if (other == null)
                return false;
            return Equivalent(other);
        }

        /// <summary>
        /// Checks if the two options are exactly the same - including type and value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TcpOption);
        }

        public override int GetHashCode()
        {
            return OptionType.GetHashCode();
        }

        public override string ToString()
        {
            return OptionType.ToString();
        }

        internal override Option Read(byte[] buffer, ref int offset, int length)
        {
            int offsetEnd = offset + length;
            if (offset == offsetEnd)
                return null;

            TcpOptionType optionType = (TcpOptionType)buffer[offset++];
            switch (optionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return End;
                case TcpOptionType.NoOperation:
                    return Nop;

                default:
                    return OptionComplexFactory<TcpOptionType>.Read(optionType, buffer, ref offset, offsetEnd - offset);
            }
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        private static readonly TcpOption _end = new TcpOptionSimple(TcpOptionType.EndOfOptionList);
        private static readonly TcpOption _nop = new TcpOptionSimple(TcpOptionType.NoOperation);
    }
}