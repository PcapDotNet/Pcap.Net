using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+-------------+--------------+----------+
    /// | Bit | 0-7         | 8-13         | 14-15    |
    /// +-----+-------------+--------------+----------+
    /// | 0   | Option Type | Opt Data Len            |
    /// +-----+-------------+--------------+----------+
    /// | 16  | Status      | Prefix-len   | Reserved |
    /// +-----+-------------+--------------+----------+
    /// | 32  | IPv4 home address                     |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4AddressAcknowledgement)]
    public sealed class IpV6MobilityOptionIpV4AddressAcknowledgement : IpV6MobilityOptionComplex
    {
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int Status = 0;
            public const int PrefixLength = Status + sizeof(byte);
            public const int HomeAddress = PrefixLength + sizeof(byte);
        }

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        public IpV6MobilityOptionIpV4AddressAcknowledgement(IpV6AddressAcknowledgementStatus status, byte prefixLength, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4AddressAcknowledgement)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Exceeded maximum value {0}", MaxPrefixLength));

            Status = status;
            PrefixLength = prefixLength;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Indicates success or failure for the IPv4 home address binding.
        /// Values from 0 to 127 indicate success.
        /// Higher values indicate failure.
        /// </summary>
        public IpV6AddressAcknowledgementStatus Status { get; private set; }

        /// <summary>
        /// The prefix length of the address allocated.
        /// This field is only valid in case of success and must be set to zero and ignored in case of failure.
        /// This field overrides what the mobile node requested (if not equal to the requested length).
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// The IPv4 home address that the home agent will use in the binding cache entry.
        /// This could be a public or private address.
        /// This field must contain the mobile node's IPv4 home address.
        /// If the address were dynamically allocated, the home agent will add the address to inform the mobile node.
        /// Otherwise, if the address is statically allocated to the mobile node, the home agent will copy it from the binding update message.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6AddressAcknowledgementStatus status = (IpV6AddressAcknowledgementStatus)data[Offset.Status];
            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4AddressAcknowledgement(status, prefixLength, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4AddressAcknowledgement);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Status, (byte)Status);
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4AddressAcknowledgement()
            : this(IpV6AddressAcknowledgementStatus.Success, 0, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4AddressAcknowledgement other)
        {
            return other != null &&
                   Status == other.Status && PrefixLength == other.PrefixLength && HomeAddress.Equals(other.HomeAddress);
        }
    }
}