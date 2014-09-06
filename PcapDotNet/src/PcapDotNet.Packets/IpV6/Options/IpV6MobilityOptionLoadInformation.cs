using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+-------------+--------------+----------+
    /// | Bit | 0-7         | 8-15         | 16-31    |
    /// +-----+-------------+--------------+----------+
    /// | 0   | Option Type | Opt Data Len | Priority |
    /// +-----+-------------+--------------+----------+
    /// | 32  | Sessions in Use                       |
    /// +-----+---------------------------------------+
    /// | 64  | Maximum Sessions                      |
    /// +-----+---------------------------------------+
    /// | 96  | Used Capacity                         |
    /// +-----+---------------------------------------+
    /// | 128 | Maximum Capacity                      |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LoadInformation)]
    public sealed class IpV6MobilityOptionLoadInformation : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Priority = 0;
            public const int SessionsInUse = Priority + sizeof(ushort);
            public const int MaximumSessions = SessionsInUse + sizeof(uint);
            public const int UsedCapacity = MaximumSessions + sizeof(uint);
            public const int MaximumCapacity = UsedCapacity + sizeof(uint);
        }

        public const int OptionDataLength = Offset.MaximumCapacity + sizeof(uint);

        public IpV6MobilityOptionLoadInformation(ushort priority, uint sessionsInUse, uint maximumSessions, uint usedCapacity, uint maximumCapacity)
            : base(IpV6MobilityOptionType.LoadInformation)
        {
            Priority = priority;
            SessionsInUse = sessionsInUse;
            MaximumSessions = maximumSessions;
            UsedCapacity = usedCapacity;
            MaximumCapacity = maximumCapacity;
        }

        /// <summary>
        /// Represents the priority of an LMA.
        /// The lower value, the higher the priority.
        /// The priority only has meaning among a group of LMAs under the same administration, for example, determined by a common LMA FQDN, a domain name,
        /// or a realm.
        /// </summary>
        public ushort Priority { get; private set; }

        /// <summary>
        /// Represents the number of parallel mobility sessions the LMA has in use.
        /// </summary>
        public uint SessionsInUse { get; private set; }

        /// <summary>
        /// Represents the maximum number of parallel mobility sessions the LMA is willing to accept.
        /// </summary>
        public uint MaximumSessions { get; private set; }

        /// <summary>
        /// Represents the used bandwidth/throughput capacity of the LMA in kilobytes per second.
        /// </summary>
        public uint UsedCapacity { get; private set; }

        /// <summary>
        /// Represents the maximum bandwidth/throughput capacity in kilobytes per second the LMA is willing to accept.
        /// </summary>
        public uint MaximumCapacity { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ushort priority = data.ReadUShort(Offset.Priority, Endianity.Big);
            uint sessionsInUse = data.ReadUInt(Offset.SessionsInUse, Endianity.Big);
            uint maximumSessions = data.ReadUInt(Offset.MaximumSessions, Endianity.Big);
            uint usedCapacity = data.ReadUInt(Offset.UsedCapacity, Endianity.Big);
            uint maximumCapacity = data.ReadUInt(Offset.MaximumCapacity, Endianity.Big);

            return new IpV6MobilityOptionLoadInformation(priority, sessionsInUse, maximumSessions, usedCapacity, maximumCapacity);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionLoadInformation);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(Priority, SessionsInUse, MaximumSessions, UsedCapacity, MaximumCapacity);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Priority, Priority, Endianity.Big);
            buffer.Write(offset + Offset.SessionsInUse, SessionsInUse, Endianity.Big);
            buffer.Write(offset + Offset.MaximumSessions, MaximumSessions, Endianity.Big);
            buffer.Write(offset + Offset.UsedCapacity, UsedCapacity, Endianity.Big);
            buffer.Write(offset + Offset.MaximumCapacity, MaximumCapacity, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionLoadInformation()
            : this(0, 0, 0, 0, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionLoadInformation other)
        {
            return other != null &&
                   Priority == other.Priority && SessionsInUse == other.SessionsInUse && MaximumSessions == other.MaximumSessions &&
                   UsedCapacity == other.UsedCapacity && MaximumCapacity == other.MaximumCapacity;
        }
    }
}