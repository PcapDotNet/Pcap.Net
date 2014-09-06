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
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionSingleDataSegmentField : IpV6MobilityOptionComplex
    {
        public IpV6MobilityOptionSingleDataSegmentField(IpV6MobilityOptionType type, DataSegment value)
            : base(type)
        {
            Value = value;
        }

        internal DataSegment Value { get; private set; }

        internal sealed override int DataLength
        {
            get { return Value.Length; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionSingleDataSegmentField);
        }

        internal sealed override int GetDataHashCode()
        {
            return Value.GetHashCode();
        }

        internal sealed override void WriteData(byte[] buffer, ref int offset)
        {
            Value.Write(buffer, ref offset);
        }

        private bool EqualsData(IpV6MobilityOptionSingleDataSegmentField other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }
    }
}
