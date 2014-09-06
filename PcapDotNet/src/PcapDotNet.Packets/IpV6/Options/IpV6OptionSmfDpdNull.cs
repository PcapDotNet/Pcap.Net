namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// Null Tagger ID.
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
    /// | 24  | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6OptionSmfDpdNull : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdNull(DataSegment identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return 0; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.Null; }
        }

        internal override bool EqualsTaggerId(IpV6OptionSmfDpdSequenceBased other)
        {
            return true;
        }

        internal override int GetTaggerIdHashCode()
        {
            return 0;
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
        }
    }
}