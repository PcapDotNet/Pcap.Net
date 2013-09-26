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
        public IpV6OptionPadN(int paddingDataLength) : base(IpV6OptionType.PadN)
        {
            PaddingDataLength = paddingDataLength;
        }

        public int PaddingDataLength { get; private set; }

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
            return true;
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }

        private IpV6OptionPadN()
            : this(0)
        {
        }
    }
}