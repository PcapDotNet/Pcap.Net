using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6602.
    /// <pre>
    /// +-----+-------------+----------------+
    /// | Bit | 0-7         | 8-15           |
    /// +-----+-------------+----------------+
    /// | 0   | Option Type | Opt Data Len   |
    /// +-----+-------------+----------------+
    /// | 16  | Sub-type    | Reserved       |
    /// +-----+-------------+----------------+
    /// | 32  | Mobile Node Group Identifier |
    /// |     |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeGroupIdentifier)]
    public sealed class IpV6MobilityOptionMobileNodeGroupIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int SubType = 0;
            public const int MobileNodeGroupIdentifier = SubType + sizeof(byte) + sizeof(byte);
        }

        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = Offset.MobileNodeGroupIdentifier + sizeof(uint);

        /// <summary>
        /// Creates an instance from sub type and mobile node group identifier.
        /// </summary>
        /// <param name="subtype">
        /// Identifies the specific mobile node's group type.
        /// </param>
        /// <param name="mobileNodeGroupIdentifier">
        /// Contains the mobile node's group identifier.
        /// The value of (0) is reserved and should not be used.
        /// The value of (1) ALL-SESSIONS is the default group of all mobility sessions established between a given local mobility anchor and a mobile access
        /// gateway.
        /// </param>
        public IpV6MobilityOptionMobileNodeGroupIdentifier(IpV6MobileNodeGroupIdentifierSubtype subtype, uint mobileNodeGroupIdentifier)
            : base(IpV6MobilityOptionType.MobileNodeGroupIdentifier)
        {
            Subtype = subtype;
            MobileNodeGroupIdentifier = mobileNodeGroupIdentifier;
        }

        /// <summary>
        /// Identifies the specific mobile node's group type.
        /// </summary>
        public IpV6MobileNodeGroupIdentifierSubtype Subtype { get; private set; }

        /// <summary>
        /// Contains the mobile node's group identifier.
        /// The value of (0) is reserved and should not be used.
        /// The value of (1) ALL-SESSIONS is the default group of all mobility sessions established between a given local mobility anchor and a mobile access
        /// gateway.
        /// </summary>
        public uint MobileNodeGroupIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6MobileNodeGroupIdentifierSubtype subtype = (IpV6MobileNodeGroupIdentifierSubtype)data[Offset.SubType];
            uint mobileNodeGroupIdentifier = data.ReadUInt(Offset.MobileNodeGroupIdentifier, Endianity.Big);

            return new IpV6MobilityOptionMobileNodeGroupIdentifier(subtype, mobileNodeGroupIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeGroupIdentifier);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(Subtype, MobileNodeGroupIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.SubType, (byte)Subtype);
            buffer.Write(offset + Offset.MobileNodeGroupIdentifier, MobileNodeGroupIdentifier, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionMobileNodeGroupIdentifier()
            : this(IpV6MobileNodeGroupIdentifierSubtype.BulkBindingUpdateGroup, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeGroupIdentifier other)
        {
            return other != null &&
                   Subtype == other.Subtype && MobileNodeGroupIdentifier == other.MobileNodeGroupIdentifier;
        }
    }
}