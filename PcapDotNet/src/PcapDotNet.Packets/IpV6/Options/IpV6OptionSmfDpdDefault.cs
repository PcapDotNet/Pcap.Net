using System;

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
        /// <summary>
        /// Creates an instance from tagger id and identifier.
        /// </summary>
        /// <param name="taggerId">
        /// Used to differentiate multiple ingressing border gateways that may commonly apply the SMF_DPD option header to packets from a particular source.
        /// Non-specific context.
        /// </param>
        /// <param name="identifier">
        /// DPD packet Identifier.
        /// When the TaggerId field is present, the Identifier can be considered a unique packet identifier 
        /// in the context of the TaggerId:srcAddr:dstAddr tuple.
        /// When the TaggerId field is not present, then it is assumed that the source applied the SMF_DPD option 
        /// and the Identifier can be considered unique in the context of the IPv6 packet header srcAddr:dstAddr tuple.
        /// </param>
        public IpV6OptionSmfDpdDefault(DataSegment taggerId, DataSegment identifier)
            : base(identifier)
        {
            if (taggerId.Length > TaggerIdMaxLength)
            {
                throw new ArgumentOutOfRangeException("taggerId", taggerId,
                                                      string.Format("Length is {0} but it must not be longer than {1} bytes.", taggerId.Length,
                                                                    TaggerIdMaxLength));
            }
            if (taggerId.Length == 0)
            {
                throw new ArgumentOutOfRangeException("taggerId", taggerId,
                                                      string.Format("Length is {0} but it must be longer than 0 bytes.", taggerId.Length));
            }
            TaggerId = taggerId;
        }

        /// <summary>
        /// Used to differentiate multiple ingressing border gateways that may commonly apply the SMF_DPD option header to packets from a particular source.
        /// Non-specific context.
        /// </summary>
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

        internal override int GetTaggerIdHashCode()
        {
            return TaggerId.GetHashCode();
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