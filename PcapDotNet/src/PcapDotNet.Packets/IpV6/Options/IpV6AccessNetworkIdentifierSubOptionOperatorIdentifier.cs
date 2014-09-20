using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+---------------------+
    /// | Bit | 0-7                 |
    /// +-----+---------------------+
    /// | 0   | ANI Type            |
    /// +-----+---------------------+
    /// | 8   | ANI Length          |
    /// +-----+---------------------+
    /// | 16  | Op-ID Type          | 
    /// +-----+---------------------+
    /// | 24  | Operator-Identifier |
    /// | ... |                     | 
    /// +-----+---------------------+
    /// </pre>
    /// </summary>
    [IpV6AccessNetworkIdentifierSubOptionTypeRegistration(IpV6AccessNetworkIdentifierSubOptionType.OperatorIdentifier)]
    public sealed class IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier : IpV6AccessNetworkIdentifierSubOption
    {
        private static class Offset
        {
            public const int IdentifierType = 0;
            public const int Identifier = IdentifierType + sizeof(byte);
        }

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.Identifier;

        /// <summary>
        /// Creates an instance from identifier type and identifier.
        /// </summary>
        /// <param name="identifierType">Indicates the type of the Operator-Identifier.</param>
        /// <param name="identifier">
        /// Up to 253 octets of the Operator-Identifier.
        /// The encoding of the identifier depends on the used Operator-Identifier Type.
        /// </param>
        public IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier(IpV6AccessNetworkIdentifierOperatorIdentifierType identifierType, DataSegment identifier)
            : base(IpV6AccessNetworkIdentifierSubOptionType.OperatorIdentifier)
        {
            IdentifierType = identifierType;
            Identifier = identifier;
        }

        /// <summary>
        /// Indicates the type of the Operator-Identifier.
        /// </summary>
        public IpV6AccessNetworkIdentifierOperatorIdentifierType IdentifierType { get; private set; }

        /// <summary>
        /// Up to 253 octets of the Operator-Identifier.
        /// The encoding of the identifier depends on the used Operator-Identifier Type.
        /// </summary>
        public DataSegment Identifier { get; private set; }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6AccessNetworkIdentifierOperatorIdentifierType identifierType = (IpV6AccessNetworkIdentifierOperatorIdentifierType)data[Offset.IdentifierType];
            DataSegment identifier = data.Subsegment(Offset.Identifier, data.Length - Offset.Identifier);

            return new IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier(identifierType, identifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + Identifier.Length; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(IdentifierType, Identifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.IdentifierType, (byte)IdentifierType);
            Identifier.Write(buffer, offset + Offset.Identifier);
            offset += DataLength;
        }

        private IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier()
            : this(IpV6AccessNetworkIdentifierOperatorIdentifierType.PrivateEnterpriseNumber, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier other)
        {
            return other != null &&
                   IdentifierType == other.IdentifierType && Identifier.Equals(other.Identifier);
        }
    }
}