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
    public abstract class IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetection : IpV6OptionComplex
    {
        private static class Offset
        {
            public const int HashIndicator = 0;
        }

        private static class Mask
        {
            public const int HashIndicator = 0x80;
        }

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.HashIndicator + sizeof(byte);

        /// <summary>
        /// Identifying DPD marking type.
        /// 0 == sequence-based approach with optional TaggerId and a tuple-based sequence number. See <see cref="IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionSequenceBased"/>.
        /// 1 == indicates a hash assist value (HAV) field follows to aid in avoiding hash-based DPD collisions.
        /// </summary>
        public abstract bool HashIndicator { get; }

        internal IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetection()
            : base(IpV6OptionType.SimplifiedMulticastForwardingDuplicatePacketDetection)
        {
        }

        internal static IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool hashIndicator = data.ReadBool(Offset.HashIndicator, Mask.HashIndicator);
            if (hashIndicator)
                return new IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionSequenceHashAssistValue(data);
            return IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionSequenceBased.CreateSpecificInstance(data);
        }
    }

    [IpV6OptionTypeRegistration(IpV6OptionType.SimplifiedMulticastForwardingDuplicatePacketDetection)]
    internal class IpV6OptionSmfDpdFactory : IIpV6OptionComplexFactory
    {
        public IpV6Option CreateInstance(DataSegment data)
        {
            return IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetection.CreateInstance(data);
        }
    }
}