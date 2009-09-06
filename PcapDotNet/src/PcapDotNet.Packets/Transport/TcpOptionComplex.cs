namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Represents a complex TCP option.
    /// Complex option means that it contains data and not just the type.
    /// </summary>
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