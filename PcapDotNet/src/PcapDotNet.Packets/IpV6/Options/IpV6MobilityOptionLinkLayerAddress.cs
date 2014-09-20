using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+--------------+
    /// | 16  | Option-Code  |
    /// +-----+--------------+
    /// | 24  | LLA          |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LinkLayerAddress)]
    public sealed class IpV6MobilityOptionLinkLayerAddress : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int OptionCode = 0;
            public const int LinkLayerAddress = OptionCode + sizeof(byte);
        }

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.LinkLayerAddress;

        /// <summary>
        /// Creates an instance from code and link layer address.
        /// </summary>
        /// <param name="code">The type of link layer address option.</param>
        /// <param name="linkLayerAddress">Variable-length link-layer address.</param>
        public IpV6MobilityOptionLinkLayerAddress(IpV6MobilityLinkLayerAddressCode code, DataSegment linkLayerAddress)
            : base(IpV6MobilityOptionType.LinkLayerAddress)
        {
            Code = code;
            LinkLayerAddress = linkLayerAddress;
        }

        /// <summary>
        /// The type of link layer address option.
        /// </summary>
        public IpV6MobilityLinkLayerAddressCode Code { get; private set; }

        /// <summary>
        /// Variable-length link-layer address.
        /// </summary>
        public DataSegment LinkLayerAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6MobilityLinkLayerAddressCode code = (IpV6MobilityLinkLayerAddressCode)data[Offset.OptionCode];
            DataSegment linkLayerAddress = data.Subsegment(Offset.LinkLayerAddress, data.Length - Offset.LinkLayerAddress);

            return new IpV6MobilityOptionLinkLayerAddress(code, linkLayerAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + LinkLayerAddress.Length; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionLinkLayerAddress);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(Code, LinkLayerAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.OptionCode, (byte)Code);
            LinkLayerAddress.Write(buffer, offset + Offset.LinkLayerAddress);
            offset += DataLength;
        }

        private IpV6MobilityOptionLinkLayerAddress()
            : this(IpV6MobilityLinkLayerAddressCode.Wildcard, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionLinkLayerAddress other)
        {
            return other != null &&
                   Code == other.Code && LinkLayerAddress.Equals(other.LinkLayerAddress);

        }
    }
}