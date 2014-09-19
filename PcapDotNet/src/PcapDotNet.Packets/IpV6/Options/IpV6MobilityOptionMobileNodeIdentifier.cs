using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4283.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+--------------+
    /// | 16  | Subtype      |
    /// +-----+--------------+
    /// | 24  | Identifier   |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeIdentifier)]
    public sealed class IpV6MobilityOptionMobileNodeIdentifier : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// The minimum length for identifier.
        /// </summary>
        public const int MinNetworkAccessIdentifierLength = 1;

        private static class Offset
        {
            public const int Subtype = 0;
            public const int Identifier = Subtype + sizeof(byte);
        }

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.Identifier;

        /// <summary>
        /// Creates an instance from subtype and identifier.
        /// </summary>
        /// <param name="subtype">
        /// Defines the specific type of identifier included in the Identifier field.
        /// </param>
        /// <param name="identifier">
        /// A variable length identifier of type, as specified by the Subtype field of this option.
        /// </param>
        public IpV6MobilityOptionMobileNodeIdentifier(IpV6MobileNodeIdentifierSubtype subtype, DataSegment identifier)
            : base(IpV6MobilityOptionType.MobileNodeIdentifier)
        {
            if (subtype == IpV6MobileNodeIdentifierSubtype.NetworkAccessIdentifier && identifier.Length < MinNetworkAccessIdentifierLength)
                throw new ArgumentOutOfRangeException("identifier", identifier,
                                                      string.Format("Network Access Identifier must be at least {0} bytes long.",
                                                                    MinNetworkAccessIdentifierLength));
            Subtype = subtype;
            Identifier = identifier;
        }

        /// <summary>
        /// Defines the specific type of identifier included in the Identifier field.
        /// </summary>
        public IpV6MobileNodeIdentifierSubtype Subtype { get; private set; }

        /// <summary>
        /// A variable length identifier of type, as specified by the Subtype field of this option.
        /// </summary>
        public DataSegment Identifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6MobileNodeIdentifierSubtype subtype = (IpV6MobileNodeIdentifierSubtype)data[Offset.Subtype];
            DataSegment identifier = data.Subsegment(Offset.Identifier, data.Length - Offset.Identifier);
            if (subtype == IpV6MobileNodeIdentifierSubtype.NetworkAccessIdentifier && identifier.Length < MinNetworkAccessIdentifierLength)
                return null;

            return new IpV6MobilityOptionMobileNodeIdentifier(subtype, identifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + Identifier.Length; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeIdentifier);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(Subtype, Identifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Subtype, (byte)Subtype);
            Identifier.Write(buffer, offset + Offset.Identifier);
            offset += DataLength;
        }

        private IpV6MobilityOptionMobileNodeIdentifier()
            : this(IpV6MobileNodeIdentifierSubtype.NetworkAccessIdentifier, new DataSegment(new byte[1]))
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeIdentifier other)
        {
            return other != null &&
                   Subtype == other.Subtype && Identifier.Equals(other.Identifier);
        }
    }
}