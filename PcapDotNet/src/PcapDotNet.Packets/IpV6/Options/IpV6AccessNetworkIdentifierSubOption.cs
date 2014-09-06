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

        internal abstract IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data);

        protected IpV6AccessNetworkIdentifierSubOption(IpV6AccessNetworkIdentifierSubOptionType type)
        {
            OptionType = type;
        }

        public sealed override int Length
        {
            get { return sizeof(byte) + sizeof(byte) + DataLength; }
        }

        public bool Equals(IpV6AccessNetworkIdentifierSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV6AccessNetworkIdentifierSubOption);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(OptionType, GetDataHashCode());
        }

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