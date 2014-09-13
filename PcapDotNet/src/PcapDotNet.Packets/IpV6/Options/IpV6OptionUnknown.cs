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
    public sealed class IpV6OptionUnknown : IpV6OptionComplex
    {
        /// <summary>
        /// Creates an unknown option according to the given type and data.
        /// </summary>
        /// <param name="type">The type of the IP option.</param>
        /// <param name="data">The data of the IP option.</param>
        public IpV6OptionUnknown(IpV6OptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        /// <summary>
        /// The data of the option.
        /// </summary>
        public DataSegment Data { get; private set; }

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionUnknown);
        }

        internal override int GetDataHashCode()
        {
            return Data.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }

        private bool EqualsData(IpV6OptionUnknown other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }
    }
}
