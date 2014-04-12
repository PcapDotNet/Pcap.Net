using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+---+---+-----+--------------+
    /// | Bit | 0 | 1 | 2-7 | 8-15         |
    /// +-----+---+---+-----+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+---+---+-----+--------------+
    /// | 16  | K | N | Reserved           |
    /// +-----+---+---+--------------------+
    /// | 32  | r2LMA Address              |
    /// |     |                            |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Redirect)]
    public sealed class IpV6MobilityOptionRedirect : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int IsIpV6 = 0;
            public const int IsIpV4 = IsIpV6;
            public const int LocalMobilityAddress = IsIpV4 + sizeof(byte) + sizeof(byte);
        }

        private static class Mask
        {
            public const byte IsIpV6 = 0x80;
            public const byte IsIpV4 = 0x40;
        }

        public const int OptionDataMinimumLength = Offset.LocalMobilityAddress;

        public IpV6MobilityOptionRedirect(IpV4Address localMobilityAddress)
            : this(localMobilityAddress, null)
        {
        }

        public IpV6MobilityOptionRedirect(IpV6Address localMobilityAddress)
            : this(null, localMobilityAddress)
        {
        }

        /// <summary>
        /// The IPv4 address of the r2LMA.
        /// This value is present when the corresponding PBU was sourced from an IPv4 address.
        /// </summary>
        public IpV4Address? LocalMobilityAddressIpV4 { get; private set; }

        /// <summary>
        /// The unicast IPv6 address of the r2LMA.
        /// This value is present when the corresponding PBU was sourced from an IPv6 address.
        /// </summary>
        public IpV6Address? LocalMobilityAddressIpV6 { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool isIpV4 = data.ReadBool(Offset.IsIpV4, Mask.IsIpV4);
            bool isIpV6 = data.ReadBool(Offset.IsIpV6, Mask.IsIpV6);

            if (isIpV4 && !isIpV6)
            {
                if (data.Length != OptionDataMinimumLength + IpV4Address.SizeOf)
                    return null;
                IpV4Address localMobilityAddress = data.ReadIpV4Address(Offset.LocalMobilityAddress, Endianity.Big);
                return new IpV6MobilityOptionRedirect(localMobilityAddress);
            }
            if (isIpV6 && !isIpV4)
            {
                if (data.Length != OptionDataMinimumLength + IpV6Address.SizeOf)
                    return null;
                IpV6Address localMobilityAddress = data.ReadIpV6Address(Offset.LocalMobilityAddress, Endianity.Big);
                return new IpV6MobilityOptionRedirect(localMobilityAddress);
            }

            return null;
        }

        internal override int DataLength
        {
            get
            {
                return OptionDataMinimumLength +
                       (LocalMobilityAddressIpV4.HasValue ? IpV4Address.SizeOf : 0) +
                       (LocalMobilityAddressIpV6.HasValue ? IpV6Address.SizeOf : 0);
            }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionRedirect);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            if (LocalMobilityAddressIpV4.HasValue)
            {
                buffer.Write(offset + Offset.IsIpV4, Mask.IsIpV4);
                buffer.Write(offset + Offset.LocalMobilityAddress, LocalMobilityAddressIpV4.Value, Endianity.Big);
                offset += OptionDataMinimumLength + IpV4Address.SizeOf;
                return;
            }
            buffer.Write(offset + Offset.IsIpV6, Mask.IsIpV6);
            buffer.Write(offset + Offset.LocalMobilityAddress, LocalMobilityAddressIpV6.Value, Endianity.Big);
            offset += OptionDataMinimumLength + IpV6Address.SizeOf;
        }

        private IpV6MobilityOptionRedirect(IpV4Address? localMobilityAddressIpV4, IpV6Address? localMobilityAddressIpV6)
            : base(IpV6MobilityOptionType.Redirect)
        {
            LocalMobilityAddressIpV4 = localMobilityAddressIpV4;
            LocalMobilityAddressIpV6 = localMobilityAddressIpV6;
        }

        private IpV6MobilityOptionRedirect()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionRedirect other)
        {
            return other != null &&
                   LocalMobilityAddressIpV4.Equals(other.LocalMobilityAddressIpV4) && LocalMobilityAddressIpV6.Equals(other.LocalMobilityAddressIpV6);
        }
    }
}