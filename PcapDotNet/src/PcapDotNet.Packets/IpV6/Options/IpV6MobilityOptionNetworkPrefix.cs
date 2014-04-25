namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 3963, 5213.
    /// <pre>
    /// +-----+--------------+---------------+
    /// | Bit | 0-7          | 8-15          |
    /// +-----+--------------+---------------+
    /// | 0   | Option Type  | Opt Data Len  |
    /// +-----+--------------+---------------+
    /// | 16  | Reserved     | Prefix Length |
    /// +-----+--------------+---------------+
    /// | 32  | Network Prefix               |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionNetworkPrefix : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int PrefixLength = sizeof(byte);
            public const int NetworkPrefix = PrefixLength + sizeof(byte);
        }

        public const int OptionDataLength = Offset.NetworkPrefix + IpV6Address.SizeOf;

        public IpV6MobilityOptionNetworkPrefix(IpV6MobilityOptionType type, byte prefixLength, IpV6Address networkPrefix)
            : base(type)
        {
            PrefixLength = prefixLength;
            NetworkPrefix = networkPrefix;
        }

        /// <summary>
        /// Indicates the prefix length of the IPv6 prefix contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Contains the Network Prefix.
        /// </summary>
        public IpV6Address NetworkPrefix { get; private set; }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override sealed bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionNetworkPrefix);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.NetworkPrefix, NetworkPrefix, Endianity.Big);
            offset += OptionDataLength;
        }

        internal static bool Read(DataSegment data, out byte prefixLength, out IpV6Address networkPrefix)
        {
            if (data.Length != OptionDataLength)
            {
                prefixLength = 0;
                networkPrefix = IpV6Address.Zero;
                return false;
            }

            prefixLength = data[Offset.PrefixLength];
            networkPrefix = data.ReadIpV6Address(Offset.NetworkPrefix, Endianity.Big);
            return true;
        }

        private bool EqualsData(IpV6MobilityOptionNetworkPrefix other)
        {
            return other != null &&
                   PrefixLength == other.PrefixLength && NetworkPrefix.Equals(other.NetworkPrefix);
        }
    }
}