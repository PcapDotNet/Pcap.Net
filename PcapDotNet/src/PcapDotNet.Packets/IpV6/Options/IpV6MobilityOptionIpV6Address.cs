namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Address                     |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionIpV6Address : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = IpV6Address.SizeOf;

        public IpV6MobilityOptionIpV6Address(IpV6MobilityOptionType type, IpV6Address address)
            : base(type)
        {
            Address = address;
        }

        internal IpV6Address Address { get; private set; }

        internal static bool Read(DataSegment data, out IpV6Address address)
        {
            if (data.Length != OptionDataLength)
            {
                address = IpV6Address.Zero;
                return false;
            }

            address = data.ReadIpV6Address(0, Endianity.Big);
            return true;
        }

        internal sealed override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV6Address);
        }

        internal sealed override int GetDataHashCode()
        {
            return Address.GetHashCode();
        }

        internal sealed override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Address, Endianity.Big);
        }

        private bool EqualsData(IpV6MobilityOptionIpV6Address other)
        {
            return other != null &&
                   other.Address.Equals(other.Address);
        }
    }
}