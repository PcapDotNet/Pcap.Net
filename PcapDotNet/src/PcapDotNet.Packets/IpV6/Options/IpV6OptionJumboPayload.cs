namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2675.
    /// <pre>
    /// +-----+-------------+--------+
    /// | Bit | 0-7         | 8-15   |
    /// +-----+-------------+--------+
    /// | 0   | Option Type | 4      |
    /// +-----+-------------+--------+
    /// | 16  | Jumbo Payload Length |
    /// |     |                      |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.JumboPayload)]
    public sealed class IpV6OptionJumboPayload : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        public const int OptionDataLength = sizeof(uint);

        public IpV6OptionJumboPayload(uint jumboPayloadLength) : base(IpV6OptionType.JumboPayload)
        {
            JumboPayloadLength = jumboPayloadLength;
        }

        /// <summary>
        /// Length of the IPv6 packet in octets, excluding the IPv6 header but including the Hop-by-Hop Options header and any other extension headers present.
        /// Must be greater than 65,535.
        /// </summary>
        public uint JumboPayloadLength { get; private set; }

        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionJumboPayload(data.ReadUInt(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, JumboPayloadLength, Endianity.Big);
        }

        private IpV6OptionJumboPayload()
            : this(0)
        {
        }
    }
}