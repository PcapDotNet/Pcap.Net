namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+-------------+
    /// | Bit | 0-7          | 8-15        |
    /// +-----+--------------+-------------+
    /// | 0   | Sub-Opt Type | Sub-Opt Len |
    /// +-----+--------------+-------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6FlowIdentificationSubOptionComplex : IpV6FlowIdentificationSubOption
    {
        /// <summary>
        /// The number of bytes this option takes.
        /// </summary>
        public sealed override int Length
        {
            get { return base.Length + sizeof(byte) + DataLength; }
        }

        internal IpV6FlowIdentificationSubOptionComplex(IpV6FlowIdentificationSubOptionType type)
            : base(type)
        {
        }

        internal abstract int DataLength { get; }

        internal sealed override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }
}