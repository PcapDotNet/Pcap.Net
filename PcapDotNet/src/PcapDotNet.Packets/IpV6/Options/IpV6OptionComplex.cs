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
        protected IpV6OptionComplex(IpV6OptionType type) 
            : base(type)
        {
        }

        public override sealed int Length
        {
            get { return sizeof(byte) + sizeof (byte) + DataLength; }
        }

        internal abstract int DataLength { get; }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            // TODO: Get rid of this.
            if (DataLength > byte.MaxValue)
                throw new InvalidOperationException("DataLength is " + DataLength);
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }

    /// <summary>
    /// RFC 6275.
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
    public abstract class IpV6MobilityOptionComplex : IpV6MobilityOption
    {
        protected IpV6MobilityOptionComplex(IpV6MobilityOptionType type)
            : base(type)
        {
        }

        public override sealed int Length
        {
            get { return base.Length + sizeof(byte) + DataLength; }
        }

        internal abstract int DataLength { get; }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }
}