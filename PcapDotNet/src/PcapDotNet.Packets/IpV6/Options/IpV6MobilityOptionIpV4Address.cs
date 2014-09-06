using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5555, 5844.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | IPv4 address               |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionIpV4Address : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Address = sizeof(ushort);
        }

        public const int OptionDataLength = Offset.Address + IpV4Address.SizeOf;

        internal IpV6MobilityOptionIpV4Address(IpV6MobilityOptionType type, IpV4Address address) 
            : base(type)
        {
            Address = address;
        }

        internal IpV4Address Address { get; private set; }

        internal static bool Read(DataSegment data, out IpV4Address address)
        {
            if (data.Length != OptionDataLength)
            {
                address = IpV4Address.Zero;
                return false;
            }

            address = data.ReadIpV4Address(Offset.Address, Endianity.Big);
            return true;
        }

        internal sealed override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4Address);
        }

        internal override int GetDataHashCode()
        {
            return Address.GetHashCode();
        }

        internal sealed override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            offset += OptionDataLength;
        }

        private bool EqualsData(IpV6MobilityOptionIpV4Address other)
        {
            return other != null &&
                   Address.Equals(other.Address);
        }
    }
}