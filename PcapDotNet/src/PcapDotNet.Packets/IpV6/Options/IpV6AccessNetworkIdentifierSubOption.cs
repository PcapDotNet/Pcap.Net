using System;
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

        public override sealed int Length
        {
            get { return sizeof(byte) + sizeof(byte) + DataLength; }
        }

        public bool Equals(IpV6AccessNetworkIdentifierSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        public override sealed bool Equals(Option other)
        {
            return Equals(other as IpV6AccessNetworkIdentifierSubOption);
        }

        internal abstract int DataLength { get; }

        internal abstract bool EqualsData(IpV6AccessNetworkIdentifierSubOption other);

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
            // TODO: Remove this check.
            if (DataLength > byte.MaxValue)
                throw new InvalidOperationException("Option length is too long.");
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }
}