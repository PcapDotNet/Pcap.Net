using System;
using System.Globalization;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+-------------+----------+-------+
    /// | Bit | 0-7         | 8-13     | 14-15 |
    /// +-----+-------------+----------+-------+
    /// | 0   | Option Type | Opt Data Len     |
    /// +-----+-------------+----------+-------+
    /// | 16  | Status      | Pref-len | Res   |
    /// +-----+-------------+----------+-------+
    /// | 32  | IPv4 home address              |
    /// |     |                                |
    /// +-----+--------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4HomeAddressReply)]
    public sealed class IpV6MobilityOptionIpV4HomeAddressReply : IpV6MobilityOptionComplex, IIpV6MobilityOptionIpV4HomeAddress
    {
        /// <summary>
        /// The maximum value for Prefix Length.
        /// </summary>
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int Status = 0;
            public const int PrefixLength = Status + sizeof(byte);
            public const int HomeAddress = PrefixLength + sizeof(byte);
        }

        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        /// <summary>
        /// Creates an instance from status, Prefix Length and Home Address.
        /// </summary>
        /// <param name="status">
        /// Indicates success or failure for the IPv4 home address assignment.
        /// Values from 0 to 127 indicate success.
        /// Higher values (128 to 255) indicate failure.
        /// </param>
        /// <param name="prefixLength">
        /// Used to carry the prefix length of the mobile node's IPv4 home network corresponding to the IPv4 home address contained in the option.
        /// </param>
        /// <param name="homeAddress">
        /// Used to carry the IPv4 home address assigned to the mobile node.
        /// </param>
        public IpV6MobilityOptionIpV4HomeAddressReply(IpV6IpV4HomeAddressReplyStatus status, byte prefixLength, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4HomeAddressReply)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength,
                                                      string.Format(CultureInfo.InvariantCulture, "Max prefix length is {0}", MaxPrefixLength));

            Status = status;
            PrefixLength = prefixLength;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Indicates success or failure for the IPv4 home address assignment.
        /// Values from 0 to 127 indicate success.
        /// Higher values (128 to 255) indicate failure.
        /// </summary>
        public IpV6IpV4HomeAddressReplyStatus Status { get; private set; }

        /// <summary>
        /// Used to carry the prefix length of the mobile node's IPv4 home network corresponding to the IPv4 home address contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Used to carry the IPv4 home address assigned to the mobile node.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6IpV4HomeAddressReplyStatus status = (IpV6IpV4HomeAddressReplyStatus)data[Offset.Status];
            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4HomeAddressReply(status, prefixLength, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4HomeAddressReply);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((byte)Status, PrefixLength), HomeAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Status, (byte)Status);
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4HomeAddressReply()
            : this(IpV6IpV4HomeAddressReplyStatus.Success, 0, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4HomeAddressReply other)
        {
            return other != null &&
                   Status == other.Status && PrefixLength == other.PrefixLength && HomeAddress.Equals(other.HomeAddress);
        }
    }
}