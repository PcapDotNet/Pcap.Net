namespace PcapDotNet.Packets.Transport
{
    public abstract class TcpOptionComplex : TcpOption
    {
        public const int OptionHeaderLength = 2;

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
        }

        protected TcpOptionComplex(TcpOptionType type)
            : base(type)
        {
        }
    }
}