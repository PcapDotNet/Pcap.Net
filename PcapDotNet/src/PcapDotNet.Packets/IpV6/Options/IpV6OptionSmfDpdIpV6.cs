namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// IPv6 tagger ID.
    /// <pre>
    /// +-----+---+-------+--------+
    /// | Bit | 0 | 1-3   | 4-7    |
    /// +-----+---+-------+--------+
    /// | 0   | Option Type        |
    /// +-----+--------------------+
    /// | 8   | Opt Data Len       |
    /// +-----+---+-------+--------+
    /// | 16  | 0 | TidTy | TidLen |
    /// +-----+---+-------+--------+
    /// | 24  | TaggerId           |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// +-----+--------------------+
    /// | 152 | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6OptionSmfDpdIpV6 : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdIpV6(IpV6Address taggerId, DataSegment identifier)
            : base(identifier)
        {
            TaggerId = taggerId;
        }

        public IpV6Address TaggerId { get; private set; }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return IpV6Address.SizeOf; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.IpV6; }
        }

        internal override bool EqualsTaggerId(IpV6OptionSmfDpdSequenceBased other)
        {
            return EqualsTaggerId(other as IpV6OptionSmfDpdIpV6);
        }

        internal override int GetTaggerIdHashCode()
        {
            return TaggerId.GetHashCode();
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TaggerId, Endianity.Big);
        }

        private bool EqualsTaggerId(IpV6OptionSmfDpdIpV6 other)
        {
            return other != null &&
                   TaggerId.Equals(other.TaggerId);
        }
    }
}