namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Value                      |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionULong : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public const int OptionDataLength = sizeof(ulong);

        internal IpV6MobilityOptionULong(IpV6MobilityOptionType type, ulong value)
            : base(type)
        {
            Value = value;
        }

        internal ulong Value { get; private set; }

        internal static bool Read(DataSegment data, out ulong value)
        {
            if (data.Length != OptionDataLength)
            {
                value = 0;
                return false;
            }

            value = data.ReadULong(0, Endianity.Big);
            return true;
        }

        internal sealed override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionULong);
        }

        internal sealed override int GetDataHashCode()
        {
            return Value.GetHashCode();
        }

        internal sealed override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Value, Endianity.Big);
        }

        private bool EqualsData(IpV6MobilityOptionULong other)
        {
            return other != null &&
                   Value == other.Value;
        }
    }
}