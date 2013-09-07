using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// IPv4 tagger ID.
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
    /// +-----+--------------------+
    /// | 56  | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionSmfDpdIpV4 : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdIpV4(IpV4Address taggerId, DataSegment identifier)
            : base(identifier)
        {
            TaggerId = taggerId;
        }

        public IpV4Address TaggerId { get; private set; }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return IpV4Address.SizeOf; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.IpV4; }
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TaggerId, Endianity.Big);
        }
    }
}