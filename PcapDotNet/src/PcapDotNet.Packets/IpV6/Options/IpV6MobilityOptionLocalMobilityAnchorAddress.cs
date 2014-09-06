using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+-------------+-----------------+
    /// | Bit | 0-7         | 8-15            |
    /// +-----+-------------+-----------------+
    /// | 0   | Option Type | Opt Data Len    |
    /// +-----+-------------+-----------------+
    /// | 16  | Option-Code | Reserved        |
    /// +-----+-------------+-----------------+
    /// | 32  | Local Mobility Anchor Address |
    /// |     |                               |
    /// | ... |                               |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LocalMobilityAnchorAddress)]
    public sealed class IpV6MobilityOptionLocalMobilityAnchorAddress : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Code = 0;
            public const int LocalMobilityAnchorAddress = Code + sizeof(byte) + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.LocalMobilityAnchorAddress;

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6LocalMobilityAnchorAddressCode code, IpV4Address localMobilityAnchorAddress)
            : this(code, localMobilityAnchorAddress, null)
        {
        }

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6LocalMobilityAnchorAddressCode code, IpV6Address localMobilityAnchorAddress)
            : this(code, null, localMobilityAnchorAddress)
        {
        }

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV4Address localMobilityAnchorAddress)
            : this(IpV6LocalMobilityAnchorAddressCode.IpV4, localMobilityAnchorAddress)
        {
        }

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6Address localMobilityAnchorAddress)
            : this(IpV6LocalMobilityAnchorAddressCode.IpV6, localMobilityAnchorAddress)
        {
        }

        /// <summary>
        /// Determines the type of the local mobility anchor address.
        /// </summary>
        public IpV6LocalMobilityAnchorAddressCode Code { get; private set; }

        /// <summary>
        /// If the Code IPv6, the LMA IPv6 address (LMAA), otherwise null.
        /// </summary>
        public IpV6Address? LocalMobilityAnchorAddressIpV6 { get; private set; }

        /// <summary>
        /// If the Code is IPv4, the LMA IPv4 address (IPv4-LMA), otherwise null.
        /// </summary>
        public IpV4Address? LocalMobilityAnchorAddressIpV4 { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6LocalMobilityAnchorAddressCode code = (IpV6LocalMobilityAnchorAddressCode)data[Offset.Code];
            switch (code)
            {
                case IpV6LocalMobilityAnchorAddressCode.IpV6:
                {
                    if (data.Length != Offset.LocalMobilityAnchorAddress + IpV6Address.SizeOf)
                        return null;
                    IpV6Address localMobilityAnchorAddress = data.ReadIpV6Address(Offset.LocalMobilityAnchorAddress, Endianity.Big);
                    return new IpV6MobilityOptionLocalMobilityAnchorAddress(localMobilityAnchorAddress);
                }

                case IpV6LocalMobilityAnchorAddressCode.IpV4:
                {
                    if (data.Length != Offset.LocalMobilityAnchorAddress + IpV4Address.SizeOf)
                        return null;
                    IpV4Address localMobilityAnchorAddress = data.ReadIpV4Address(Offset.LocalMobilityAnchorAddress, Endianity.Big);
                    return new IpV6MobilityOptionLocalMobilityAnchorAddress(localMobilityAnchorAddress);
                }

                default:
                    return null;
            }
        }

        internal override int DataLength
        {
            get
            {
                return OptionDataMinimumLength +
                       (LocalMobilityAnchorAddressIpV4.HasValue ? IpV4Address.SizeOf : 0) +
                       (LocalMobilityAnchorAddressIpV6.HasValue ? IpV6Address.SizeOf : 0);
            }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionLocalMobilityAnchorAddress);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(Code, LocalMobilityAnchorAddressIpV4, LocalMobilityAnchorAddressIpV6);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Code, (byte)Code);
            if (LocalMobilityAnchorAddressIpV4.HasValue)
                buffer.Write(offset + Offset.LocalMobilityAnchorAddress, LocalMobilityAnchorAddressIpV4.Value, Endianity.Big);
            else if (LocalMobilityAnchorAddressIpV6.HasValue)
                buffer.Write(offset + Offset.LocalMobilityAnchorAddress, LocalMobilityAnchorAddressIpV6.Value, Endianity.Big);
            offset += DataLength;
        }

        private IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6LocalMobilityAnchorAddressCode code, IpV4Address? localMobilityAnchorAddressIpV4,
                                                             IpV6Address? localMobilityAnchorAddressIpV6)
            : base(IpV6MobilityOptionType.LocalMobilityAnchorAddress)
        {
            Code = code;
            LocalMobilityAnchorAddressIpV6 = localMobilityAnchorAddressIpV6;
            LocalMobilityAnchorAddressIpV4 = localMobilityAnchorAddressIpV4;
        }

        private IpV6MobilityOptionLocalMobilityAnchorAddress()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionLocalMobilityAnchorAddress other)
        {
            return other != null &&
                   Code == other.Code && LocalMobilityAnchorAddressIpV4.Equals(other.LocalMobilityAnchorAddressIpV4) &&
                   LocalMobilityAnchorAddressIpV6.Equals(other.LocalMobilityAnchorAddressIpV6);
        }
    }
}