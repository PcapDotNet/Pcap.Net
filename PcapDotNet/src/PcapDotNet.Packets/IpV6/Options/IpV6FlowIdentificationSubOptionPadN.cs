namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Sub-Opt Type |
    /// +-----+--------------+
    /// | 8   | N            |
    /// +-----+--------------+
    /// | 16  | 0            |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6FlowIdentificationSubOptionTypeRegistration(IpV6FlowIdentificationSubOptionType.PadN)]
    public sealed class IpV6FlowIdentificationSubOptionPadN : IpV6FlowIdentificationSubOptionComplex
    {
        /// <summary>
        /// Creates an instance from padding data length.
        /// </summary>
        /// <param name="paddingDataLength">The size of the padding in bytes.</param>
        public IpV6FlowIdentificationSubOptionPadN(int paddingDataLength)
            : base(IpV6FlowIdentificationSubOptionType.PadN)
        {
            PaddingDataLength = paddingDataLength;
        }

        /// <summary>
        /// The size of the padding in bytes.
        /// </summary>
        public int PaddingDataLength { get; private set; }

        internal override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            return new IpV6FlowIdentificationSubOptionPadN(data.Length);
        }

        internal override int DataLength
        {
            get { return PaddingDataLength; }
        }

        internal override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return EqualsData(other as IpV6FlowIdentificationSubOptionPadN);
        }

        internal override object GetDataHashCode()
        {
            return PaddingDataLength.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }

        private IpV6FlowIdentificationSubOptionPadN()
            : this(0)
        {
        }

        private bool EqualsData(IpV6FlowIdentificationSubOptionPadN other)
        {
            return other != null &&
                   PaddingDataLength == other.PaddingDataLength;
        }
    }
}