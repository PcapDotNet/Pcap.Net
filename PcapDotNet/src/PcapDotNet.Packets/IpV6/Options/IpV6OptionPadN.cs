namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+
    /// | Bit | 0-7         |
    /// +-----+-------------+
    /// | 0   | Option Type |
    /// +-----+-------------+
    /// | 8   | N           |
    /// +-----+-------------+
    /// | 16  | 0           |
    /// | ... |             |
    /// +-----+-------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.PadN)]
    public sealed class IpV6OptionPadN : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        /// <summary>
        /// Creates an option from padding data length.
        /// </summary>
        /// <param name="paddingDataLength">The size of the padding in bytes.</param>
        public IpV6OptionPadN(int paddingDataLength) : base(IpV6OptionType.PadN)
        {
            PaddingDataLength = paddingDataLength;
        }

        /// <summary>
        /// The size of the padding in bytes.
        /// </summary>
        public int PaddingDataLength { get; private set; }

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        public IpV6Option CreateInstance(DataSegment data)
        {
            return new IpV6OptionPadN(data.Length);
        }

        internal override int DataLength
        {
            get { return PaddingDataLength; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionPadN);
        }

        internal override int GetDataHashCode()
        {
            return PaddingDataLength.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }

        private IpV6OptionPadN()
            : this(0)
        {
        }

        private bool EqualsData(IpV6OptionPadN other)
        {
            return other != null &&
                   PaddingDataLength == other.PaddingDataLength;
        }
    }
}