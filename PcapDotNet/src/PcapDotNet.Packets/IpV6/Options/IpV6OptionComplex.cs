using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6OptionComplex : IpV6Option
    {
        /// <summary>
        /// The number of bytes the option takes.
        /// </summary>
        public sealed override int Length
        {
            get { return sizeof(byte) + sizeof (byte) + DataLength; }
        }

        internal IpV6OptionComplex(IpV6OptionType type)
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
