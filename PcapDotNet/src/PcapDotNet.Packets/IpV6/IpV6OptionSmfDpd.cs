namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// <pre>
    /// +-----+---+------------------+
    /// | Bit | 0 | 1-7              |
    /// +-----+---+------------------+
    /// | 0   | Option Type          |
    /// +-----+----------------------+
    /// | 8   | Opt Data Len         |
    /// +-----+---+------------------+
    /// | 16  | H | DPD Identifier   |
    /// +-----+---+ Option Fields    |
    /// | ... | or Hash Assist Value |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.SmfDpd)]
    public abstract class IpV6OptionSmfDpd : IpV6OptionComplex
    {
        private static class Offset
        {
            public const int HashIndicator = 0;
        }

        private static class Mask
        {
            public const int HashIndicator = 0x80;
        }

        public const int OptionDataMinimumLength = Offset.HashIndicator + sizeof(byte);

        protected IpV6OptionSmfDpd()
            : base(IpV6OptionType.SmfDpd)
        {
        }

        /// <summary>
        /// Identifying DPD marking type.
        /// 0 == sequence-based approach with optional TaggerId and a tuple-based sequence number. See <see cref="IpV6OptionSmfDpdSequenceBased"/>.
        /// 1 == indicates a hash assist value (HAV) field follows to aid in avoiding hash-based DPD collisions.
        /// </summary>
        public abstract bool HashIndicator { get; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool hashIndicator = data.ReadBool(Offset.HashIndicator, Mask.HashIndicator);
            if (hashIndicator)
                return new IpV6OptionSmfDpdSequenceHashAssistValue(data);
            return IpV6OptionSmfDpdSequenceBased.CreateSpecificInstance(data);
        }
    }
}