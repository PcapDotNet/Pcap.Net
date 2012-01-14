using System;

namespace PcapDotNet.Packets.Dns
{
    public abstract class DnsOption : IEquatable<DnsOption>
    {
        public const int MinimumLength = sizeof(ushort) + sizeof(ushort);

        public DnsOptionCode Code { get; private set; }
        public int Length { get { return MinimumLength + DataLength; } }
        public abstract int DataLength { get; }

        public bool Equals(DnsOption other)
        {
            return other != null &&
                   Code.Equals(other.Code) && 
                   GetType().Equals(other.GetType()) &&
                   EqualsData(other);
        }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as DnsOption);
        }

        internal DnsOption(DnsOptionCode code)
        {
            Code = code;
        }

        internal abstract bool EqualsData(DnsOption other);

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (ushort)Code, Endianity.Big);
            buffer.Write(ref offset, (ushort)DataLength, Endianity.Big);
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);

        internal static DnsOption CreateInstance(DnsOptionCode code, DataSegment data)
        {
            switch (code)
            {
                case DnsOptionCode.LongLivedQuery:
                    if (data.Length < DnsOptionLongLivedQuery.MinimumDataLength)
                        return null;
                    return DnsOptionLongLivedQuery.Read(data);

                case DnsOptionCode.UpdateLease:
                    if (data.Length < DnsOptionUpdateLease.MinimumDataLength)
                        return null;
                    return DnsOptionUpdateLease.Read(data);

                case DnsOptionCode.NameServerIdentifier:
                default:
                    return new DnsOptionAnything(code, data);
            }
        }
    }
}