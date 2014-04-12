namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5845.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | GRE Key Identifier         |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.GreKey)]
    public sealed class IpV6MobilityOptionGreKey : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int GreKeyIdentifier = sizeof(ushort);
        }

        public const int OptionDataLength = Offset.GreKeyIdentifier + sizeof(uint);

        public IpV6MobilityOptionGreKey(uint greKeyIdentifier)
            : base(IpV6MobilityOptionType.GreKey)
        {
            GreKeyIdentifier = greKeyIdentifier;
        }

        /// <summary>
        /// Contains the downlink or the uplink GRE key.
        /// This field is present in the GRE Key option only if the GRE keys are being exchanged using the Proxy Binding Update and Proxy Binding
        /// Acknowledgement messages.
        /// </summary>
        public uint GreKeyIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            uint greKeyIdentifier = data.ReadUInt(Offset.GreKeyIdentifier, Endianity.Big);
            return new IpV6MobilityOptionGreKey(greKeyIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionGreKey);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.GreKeyIdentifier, GreKeyIdentifier, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionGreKey() 
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionGreKey other)
        {
            return other != null &&
                   GreKeyIdentifier == other.GreKeyIdentifier;
        }
    }
}