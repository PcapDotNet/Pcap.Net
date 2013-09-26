using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
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
    public abstract class IpV6OptionSmfDpdSequenceBased : IpV6OptionSmfDpd
    {
        private static class Offset
        {
            public const int TaggerIdType = 0;
            public const int TaggerIdLength = TaggerIdType;
            public const int TaggerId = TaggerIdLength + sizeof(byte);
        }

        private static class Mask
        {
            public const byte TaggerIdType = 0x70;
            public const byte TaggerIdLength = 0x0F;
        }

        private static class Shift
        {
            public const int TaggerIdType = 4;
        }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public abstract int TaggerIdLength { get; }

        /// <summary>
        /// DPD packet Identifier.
        /// When the TaggerId field is present, the Identifier can be considered a unique packet identifier 
        /// in the context of the TaggerId:srcAddr:dstAddr tuple.
        /// When the TaggerId field is not present, then it is assumed that the source applied the SMF_DPD option 
        /// and the Identifier can be considered unique in the context of the IPv6 packet header srcAddr:dstAddr tuple.
        /// </summary>
        public DataSegment Identifier { get; private set; }

        /// <summary>
        /// Identifying DPD marking type.
        /// 0 == sequence-based approach with optional TaggerId and a tuple-based sequence number. See <see cref="IpV6OptionSmfDpdSequenceBased"/>.
        /// 1 == indicates a hash assist value (HAV) field follows to aid in avoiding hash-based DPD collisions.
        /// </summary>
        public override bool HashIndicator
        {
            get { return false; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public abstract IpV6TaggerIdType TaggerIdType { get; }

        protected IpV6OptionSmfDpdSequenceBased(DataSegment identifier)
        {
            Identifier = identifier;
        }

        internal override sealed int DataLength
        {
            get { return OptionDataMinimumLength + TaggerIdLength + Identifier.Length; }
        }

        internal sealed override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionSmfDpdSequenceBased);
        }

        internal abstract bool EqualsTaggerId(IpV6OptionSmfDpdSequenceBased other);

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            byte taggerIdInfo = (byte)(((byte)TaggerIdType << Shift.TaggerIdType) & Mask.TaggerIdType);
            if (TaggerIdType != IpV6TaggerIdType.Null)
                taggerIdInfo |= (byte)((TaggerIdLength - 1) & Mask.TaggerIdLength);
            buffer.Write(ref offset, taggerIdInfo);
            WriteTaggerId(buffer, ref offset);
            buffer.Write(ref offset, Identifier);
        }

        internal abstract void WriteTaggerId(byte[] buffer, ref int offset);

        internal static IpV6OptionSmfDpdSequenceBased CreateSpecificInstance(DataSegment data)
        {
            IpV6TaggerIdType taggerIdType = (IpV6TaggerIdType)((data[Offset.TaggerIdType] & Mask.TaggerIdType) >> Shift.TaggerIdType);
            int taggerIdLength = (taggerIdType == IpV6TaggerIdType.Null ? 0 : (data[Offset.TaggerIdLength] & Mask.TaggerIdLength) + 1);
            if (data.Length < Offset.TaggerId + taggerIdLength)
                return null;
            DataSegment identifier = data.Subsegment(Offset.TaggerId + taggerIdLength, data.Length - Offset.TaggerId - taggerIdLength);
            switch (taggerIdType)
            {
                case IpV6TaggerIdType.Null:
                    return new IpV6OptionSmfDpdNull(identifier);
                    
                case IpV6TaggerIdType.Default:
                    return new IpV6OptionSmfDpdDefault(data.Subsegment(Offset.TaggerId, taggerIdLength), identifier);

                case IpV6TaggerIdType.IpV4:
                    if (taggerIdLength != IpV4Address.SizeOf)
                        return null;
                    IpV4Address ipV4Address = data.ReadIpV4Address(Offset.TaggerId, Endianity.Big);
                    return new IpV6OptionSmfDpdIpV4(ipV4Address, identifier);

                case IpV6TaggerIdType.IpV6:
                    if (taggerIdLength != IpV6Address.SizeOf)
                        return null;
                    IpV6Address ipV6Address = data.ReadIpV6Address(Offset.TaggerId, Endianity.Big);
                    return new IpV6OptionSmfDpdIpV6(ipV6Address, identifier);

                default:
                    return null;
            }
        }

        private bool EqualsData(IpV6OptionSmfDpdSequenceBased other)
        {
            return other != null &&
                   Identifier.Equals(other.Identifier) && TaggerIdType == other.TaggerIdType && EqualsTaggerId(other);

        }
    }
}