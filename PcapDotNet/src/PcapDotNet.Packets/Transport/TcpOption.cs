using System;

namespace PcapDotNet.Packets.Transport
{
    public enum TcpOptionType : byte
    {
        EndOfOptionList = 0,
        NoOperation = 1
    }

    public abstract class TcpOption : Option
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

        public override bool Equivalent(Option other)
        {
            return OptionType == ((TcpOption)other).OptionType;
        }

        public TcpOptionType OptionType { get; private set; }

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