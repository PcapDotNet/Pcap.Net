using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// </summary>
    public abstract class IpV6AccessNetworkIdentifierSubOption : Option, IEquatable<IpV6AccessNetworkIdentifierSubOption>
    {
        /// <summary>
        /// The type of the option.
        /// </summary>
        public IpV6AccessNetworkIdentifierSubOptionType OptionType { get; private set; }

        /// <summary>
        /// The number of bytes this option takes.
        /// </summary>
        public sealed override int Length
        {
            get { return sizeof(byte) + sizeof(byte) + DataLength; }
        }

        /// <summary>
        /// True iff the two options are equal.
        /// Two options are equal iff their type, length and data are equal.
        /// </summary>
        public bool Equals(IpV6AccessNetworkIdentifierSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        /// <summary>
        /// True iff the two options are equal.
        /// Two options are equal iff their type, length and data are equal.
        /// </summary>
        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV6AccessNetworkIdentifierSubOption);
        }

        /// <summary>
        /// Returns the option hash code.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Sequence.GetHashCode(OptionType, GetDataHashCode());
        }

        internal IpV6AccessNetworkIdentifierSubOption(IpV6AccessNetworkIdentifierSubOptionType type)
        {
            OptionType = type;
        }

        internal abstract IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data);

        internal abstract int DataLength { get; }

        internal abstract bool EqualsData(IpV6AccessNetworkIdentifierSubOption other);

        internal abstract int GetDataHashCode();

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }
}