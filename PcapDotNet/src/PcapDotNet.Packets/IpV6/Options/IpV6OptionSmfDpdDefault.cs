namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// Default Tagger ID.
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
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6OptionSmfDpdDefault : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdDefault(DataSegment taggerId, DataSegment identifier)
            : base(identifier)
        {
            TaggerId = taggerId;
        }

        public DataSegment TaggerId { get; private set; }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return TaggerId.Length; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.Default; }
        }

        internal override bool EqualsTaggerId(IpV6OptionSmfDpdSequenceBased other)
        {
            return EqualsTaggerId(other as IpV6OptionSmfDpdDefault);
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TaggerId);
        }

        private bool EqualsTaggerId(IpV6OptionSmfDpdDefault other)
        {
            return other != null &&
                   TaggerId.Equals(other.TaggerId);
        }
    }
}