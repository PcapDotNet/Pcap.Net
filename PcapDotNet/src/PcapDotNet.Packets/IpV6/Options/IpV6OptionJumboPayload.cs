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
        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public const int OptionDataLength = sizeof(uint);

        /// <summary>
        /// Creates an option from the given jumbo payload length.
        /// </summary>
        /// <param name="jumboPayloadLength">
        /// Length of the IPv6 packet in octets, excluding the IPv6 header but including the Hop-by-Hop Options header and any other extension headers present.
        /// Must be greater than 65,535.
        /// </param>
        public IpV6OptionJumboPayload(uint jumboPayloadLength) : base(IpV6OptionType.JumboPayload)
        {
            JumboPayloadLength = jumboPayloadLength;
        }

        /// <summary>
        /// Length of the IPv6 packet in octets, excluding the IPv6 header but including the Hop-by-Hop Options header and any other extension headers present.
        /// Must be greater than 65,535.
        /// </summary>
        public uint JumboPayloadLength { get; private set; }

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
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

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionJumboPayload);
        }

        internal override int GetDataHashCode()
        {
            return JumboPayloadLength.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, JumboPayloadLength, Endianity.Big);
        }

        private IpV6OptionJumboPayload()
            : this(0)
        {
        }

        private bool EqualsData(IpV6OptionJumboPayload other)
        {
            return other != null &&
                   JumboPayloadLength == other.JumboPayloadLength;
        }
    }
}