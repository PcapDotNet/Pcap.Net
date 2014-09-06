using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4285.
    /// <pre>
    /// +-----+---------------------+
    /// | Bit | 0-7                 |
    /// +-----+---------------------+
    /// | 0   | Option Type         |
    /// +-----+---------------------+
    /// | 8   | Opt Data Len        |
    /// +-----+---------------------+
    /// | 16  | Subtype             |
    /// +-----+---------------------+
    /// | 24  | Mobility SPI        |
    /// |     |                     |
    /// |     |                     |
    /// |     |                     |
    /// +-----+---------------------+
    /// | 56  | Authentication Data |
    /// | ... |                     |
    /// +-----+---------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Authentication)]
    public sealed class IpV6MobilityOptionAuthentication : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Subtype = 0;
            public const int MobilitySecurityParameterIndex = Subtype + sizeof(byte);
            public const int AuthenticationData = MobilitySecurityParameterIndex + sizeof(uint);
        }

        public const int OptionDataMinimumLength = Offset.AuthenticationData;

        public IpV6MobilityOptionAuthentication(IpV6AuthenticationSubtype subtype, uint mobilitySecurityParameterIndex, DataSegment authenticationData)
            : base(IpV6MobilityOptionType.Authentication)
        {
            Subtype = subtype;
            MobilitySecurityParameterIndex = mobilitySecurityParameterIndex;
            AuthenticationData = authenticationData;
        }

        /// <summary>
        /// A number assigned to identify the entity and/or mechanism to be used to authenticate the message.
        /// </summary>
        public IpV6AuthenticationSubtype Subtype { get; private set; }

        /// <summary>
        /// A number in the range [0-4294967296] used to index into the shared-key-based mobility security associations.
        /// </summary>
        public uint MobilitySecurityParameterIndex { get; private set; }

        /// <summary>
        /// Has the information to authenticate the relevant mobility entity.
        /// This protects the message beginning at the Mobility Header up to and including the mobility SPI field.
        /// </summary>
        public DataSegment AuthenticationData { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6AuthenticationSubtype subtype = (IpV6AuthenticationSubtype)data[Offset.Subtype];
            uint mobilitySecurityParameterIndex = data.ReadUInt(Offset.MobilitySecurityParameterIndex, Endianity.Big);
            DataSegment authenticationData = data.Subsegment(Offset.AuthenticationData, data.Length - Offset.AuthenticationData);

            return new IpV6MobilityOptionAuthentication(subtype, mobilitySecurityParameterIndex, authenticationData);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + AuthenticationData.Length; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionAuthentication);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(Subtype, MobilitySecurityParameterIndex, AuthenticationData);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Subtype, (byte)Subtype);
            buffer.Write(offset + Offset.MobilitySecurityParameterIndex, MobilitySecurityParameterIndex, Endianity.Big);
            AuthenticationData.Write(buffer, offset + Offset.AuthenticationData);
            offset += DataLength;
        }

        private IpV6MobilityOptionAuthentication()
            : this(IpV6AuthenticationSubtype.HomeAgent, 0, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionAuthentication other)
        {
            return other != null &&
                   Subtype == other.Subtype && MobilitySecurityParameterIndex == other.MobilitySecurityParameterIndex &&
                   AuthenticationData.Equals(other.AuthenticationData);
        }
    }
}