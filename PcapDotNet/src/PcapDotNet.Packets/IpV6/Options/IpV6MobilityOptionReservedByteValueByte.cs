namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved    | Value        |
    /// +-----+-------------+--------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionReservedByteValueByte : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Value = sizeof(byte);
        }

        public const int OptionDataLength = Offset.Value + sizeof(byte);

        public IpV6MobilityOptionReservedByteValueByte(IpV6MobilityOptionType type, byte value)
            : base(type)
        {
            Value = value;
        }

        internal byte Value { get; private set; }

        internal static bool Read(DataSegment data, out byte value)
        {
            if (data.Length != OptionDataLength)
            {
                value = 0;
                return false;
            }

            value = data[Offset.Value];
            return true;
        }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionReservedByteValueByte);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Value, Value);
            offset += DataLength;
        }

        private bool EqualsData(IpV6MobilityOptionReservedByteValueByte other)
        {
            return other != null &&
                   Value == other.Value;
        }
    }
}