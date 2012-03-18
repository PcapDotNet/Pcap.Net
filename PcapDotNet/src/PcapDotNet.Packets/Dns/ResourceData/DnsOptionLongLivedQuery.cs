using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
    /// <pre>
    /// +-----+------------+
    /// | bit | 0-15       |
    /// +-----+------------+
    /// | 0   | VERSION    |
    /// +-----+------------+
    /// | 16  | LLQ-OPCODE |
    /// +-----+------------+
    /// | 32  | ERROR-CODE |
    /// +-----+------------+
    /// | 48  | LLQ-ID     |
    /// |     |            |
    /// |     |            |
    /// |     |            |
    /// +-----+------------+
    /// | 112 | LEASE-LIFE |
    /// |     |            |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    public sealed class DnsOptionLongLivedQuery : DnsOption
    {
        private static class Offset
        {
            public const int Version = 0;
            public const int OpCode = Version + sizeof(ushort);
            public const int ErrorCode = OpCode + sizeof(ushort);
            public const int Id = ErrorCode + sizeof(ushort);
            public const int LeaseLife = Id + sizeof(ulong);
        }

        private const int ConstDataLength = Offset.LeaseLife + sizeof(uint);

        /// <summary>
        /// Constructs an instance out of the version, opcode, error code, id and lease life fields.
        /// </summary>
        /// <param name="version">Version of LLQ protocol implemented.</param>
        /// <param name="opCode">Identifies LLQ operation.</param>
        /// <param name="errorCode">Identifies LLQ errors.</param>
        /// <param name="id">Identifier for an LLQ.</param>
        /// <param name="leaseLife">Requested or granted life of LLQ, in seconds.</param>
        public DnsOptionLongLivedQuery(ushort version, DnsLongLivedQueryOpCode opCode, DnsLongLivedQueryErrorCode errorCode, ulong id, uint leaseLife)
            : base(DnsOptionCode.LongLivedQuery)
        {
            Version = version;
            OpCode = opCode;
            ErrorCode = errorCode;
            Id = id;
            LeaseLife = leaseLife;
        }

        /// <summary>
        /// Version of LLQ protocol implemented.
        /// </summary>
        public ushort Version { get; private set; }

        /// <summary>
        /// Identifies LLQ operation.
        /// </summary>
        public DnsLongLivedQueryOpCode OpCode { get; private set; }

        /// <summary>
        /// Identifies LLQ errors.
        /// </summary>
        public DnsLongLivedQueryErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Identifier for an LLQ.
        /// </summary>
        public ulong Id { get; private set; }

        /// <summary>
        /// Requested or granted life of LLQ, in seconds.
        /// </summary>
        public uint LeaseLife { get; private set; }

        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public override int DataLength
        {
            get { return ConstDataLength; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            DnsOptionLongLivedQuery castedOther = (DnsOptionLongLivedQuery)other;
            return Version.Equals(castedOther.Version) &&
                   OpCode.Equals(castedOther.OpCode) &&
                   ErrorCode.Equals(castedOther.ErrorCode) &&
                   Id.Equals(castedOther.Id) &&
                   LeaseLife.Equals(castedOther.LeaseLife);
        }

        internal override int DataGetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(Version, (ushort)OpCode), ErrorCode, Id, LeaseLife);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Version, Version, Endianity.Big);
            buffer.Write(offset + Offset.OpCode, (ushort)OpCode, Endianity.Big);
            buffer.Write(offset + Offset.ErrorCode, (ushort)ErrorCode, Endianity.Big);
            buffer.Write(offset + Offset.Id, Id, Endianity.Big);
            buffer.Write(offset + Offset.LeaseLife, LeaseLife, Endianity.Big);
            offset += DataLength;
        }

        internal static DnsOptionLongLivedQuery Read(DataSegment data)
        {
            if (data.Length != ConstDataLength)
                return null;
            ushort version = data.ReadUShort(Offset.Version, Endianity.Big);
            DnsLongLivedQueryOpCode opCode = (DnsLongLivedQueryOpCode)data.ReadUShort(Offset.OpCode, Endianity.Big);
            DnsLongLivedQueryErrorCode errorCode = (DnsLongLivedQueryErrorCode)data.ReadUShort(Offset.ErrorCode, Endianity.Big);
            ulong id = data.ReadULong(Offset.Id, Endianity.Big);
            uint leaseLife = data.ReadUInt(Offset.LeaseLife, Endianity.Big);

            return new DnsOptionLongLivedQuery(version, opCode, errorCode, id, leaseLife);
        }
    }
}