using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+-------------+------------------+
    /// | Bit | 0-7         | 8-15             |
    /// +-----+-------------+------------------+
    /// | 0   | Option Type | Opt Data Len     |
    /// +-----+-------------+------------------+
    /// | 16  | Alternate IPv4 Care-of Address |
    /// |     |                                |
    /// +-----+--------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AlternateIpV4CareOfAddress)]
    public sealed class IpV6MobilityOptionAlternateIpV4CareOfAddress : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = IpV4Address.SizeOf;

        public IpV6MobilityOptionAlternateIpV4CareOfAddress(IpV4Address alternateCareOfAddress)
            : base(IpV6MobilityOptionType.AlternateIpV4CareOfAddress)
        {
            AlternateCareOfAddress = alternateCareOfAddress;
        }

        /// <summary>
        /// An IPv4 equivalent of the RFC 6275 Alternate Care-of Address option for IPv6.
        /// In the context of PMIPv6, its semantic is equivalent to the Alternate Care-of Address option for IPv6.
        /// </summary>
        public IpV4Address AlternateCareOfAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionAlternateIpV4CareOfAddress(data.ReadIpV4Address(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionAlternateIpV4CareOfAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, AlternateCareOfAddress, Endianity.Big);
        }

        private IpV6MobilityOptionAlternateIpV4CareOfAddress()
            : this(IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionAlternateIpV4CareOfAddress other)
        {
            return other != null &&
                   AlternateCareOfAddress.Equals(other.AlternateCareOfAddress);
        }
    }
}