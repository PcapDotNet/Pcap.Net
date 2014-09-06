using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2671.
    /// A single option.
    /// </summary>
    public abstract class DnsOption : IEquatable<DnsOption>
    {
        /// <summary>
        /// The minimum number of bytes an option can take.
        /// </summary>
        public const int MinimumLength = sizeof(ushort) + sizeof(ushort);

        /// <summary>
        /// The option code.
        /// </summary>
        public DnsOptionCode Code { get; private set; }

        /// <summary>
        /// The number of bytes the option takes.
        /// </summary>
        public int Length { get { return MinimumLength + DataLength; } }
        
        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public abstract int DataLength { get; }

        /// <summary>
        /// Two options are equal if they are of the same type, have the same code and have equal data.
        /// </summary>
        public bool Equals(DnsOption other)
        {
            return other != null &&
                   Code.Equals(other.Code) && 
                   GetType().Equals(other.GetType()) &&
                   EqualsData(other);
        }

        /// <summary>
        /// Two options are equal if they are of the same type, have the same code and have equal data.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as DnsOption);
        }

        /// <summary>
        /// Returns a hash code based on the option code and data.
        /// </summary>
        public override int GetHashCode()
        {
            return Code.GetHashCode() ^ DataGetHashCode();
        }

        internal DnsOption(DnsOptionCode code)
        {
            Code = code;
        }

        internal abstract bool EqualsData(DnsOption other);

        internal abstract int DataGetHashCode();

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
                    return DnsOptionLongLivedQuery.Read(data);

                case DnsOptionCode.UpdateLease:
                    return DnsOptionUpdateLease.Read(data);

                case DnsOptionCode.NameServerIdentifier:
                default:
                    return new DnsOptionAnything(code, data);
            }
        }
    }
}