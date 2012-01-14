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
    public class DnsOptionLongLivedQuery : DnsOption
    {
        private static class Offset
        {
            public const int Version = 0;
            public const int Opcode = Version + sizeof(ushort);
            public const int ErrorCode = Opcode + sizeof(ushort);
            public const int Id = ErrorCode + sizeof(ushort);
            public const int LeaseLife = Id + sizeof(ulong);
        }

        public const int MinimumDataLength = Offset.LeaseLife + sizeof(uint);

        public DnsOptionLongLivedQuery(ushort version, DnsLongLivedQueryOpcode opcode, DnsLongLivedQueryErrorCode errorCode, ulong id, uint leaseLife)
            : base(DnsOptionCode.LongLivedQuery)
        {
            Version = version;
            Opcode = opcode;
            ErrorCode = errorCode;
            Id = id;
            LeaseLife = leaseLife;
        }

        public ushort Version { get; private set; }
        public DnsLongLivedQueryOpcode Opcode { get; private set; }
        public DnsLongLivedQueryErrorCode ErrorCode { get; private set; }
        public ulong Id { get; private set; }
        public uint LeaseLife { get; private set; }

        public override int DataLength
        {
            get { return MinimumDataLength; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            DnsOptionLongLivedQuery castedOther = (DnsOptionLongLivedQuery)other;
            return Version.Equals(castedOther.Version) &&
                   Opcode.Equals(castedOther.Opcode) &&
                   ErrorCode.Equals(castedOther.ErrorCode) &&
                   Id.Equals(castedOther.Id) &&
                   LeaseLife.Equals(castedOther.LeaseLife);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Version, Version, Endianity.Big);
            buffer.Write(offset + Offset.Opcode, (ushort)Opcode, Endianity.Big);
            buffer.Write(offset + Offset.ErrorCode, (ushort)ErrorCode, Endianity.Big);
            buffer.Write(offset + Offset.Id, Id, Endianity.Big);
            buffer.Write(offset + Offset.LeaseLife, LeaseLife, Endianity.Big);
            offset += DataLength;
        }

        internal static DnsOptionLongLivedQuery Read(DataSegment data)
        {
            if (data.Length < MinimumDataLength)
                return null;
            ushort version = data.ReadUShort(Offset.Version, Endianity.Big);
            DnsLongLivedQueryOpcode opcode = (DnsLongLivedQueryOpcode)data.ReadUShort(Offset.Opcode, Endianity.Big);
            DnsLongLivedQueryErrorCode errorCode = (DnsLongLivedQueryErrorCode)data.ReadUShort(Offset.ErrorCode, Endianity.Big);
            ulong id = data.ReadULong(Offset.Id, Endianity.Big);
            uint leaseLife = data.ReadUInt(Offset.LeaseLife, Endianity.Big);

            return new DnsOptionLongLivedQuery(version, opcode, errorCode, id, leaseLife);
        }
    }
}