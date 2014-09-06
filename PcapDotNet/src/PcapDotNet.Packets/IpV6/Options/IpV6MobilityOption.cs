using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Option Type | Opt Data Len (optional) |
    /// +-----+-------------+-------------------------+
    /// | 16  | Option Data (optional)                |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOption : Option, IEquatable<IpV6MobilityOption>
    {
        /// <summary>
        /// The type of the IP option.
        /// </summary>
        public IpV6MobilityOptionType OptionType { get; private set; }

        public override int Length
        {
            get { return sizeof(byte); }
        }

        public sealed override bool Equals(Option option)
        {
            return Equals(option as IpV6MobilityOption);
        }

        public bool Equals(IpV6MobilityOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        public sealed override int GetHashCode()
        {
            return Sequence.GetHashCode(OptionType, GetDataHashCode());
        }

        public virtual bool IsValid { get { return true; } }

        protected IpV6MobilityOption(IpV6MobilityOptionType type)
        {
            OptionType = type;
        }

        internal abstract int GetDataHashCode();

        internal abstract IpV6MobilityOption CreateInstance(DataSegment data);

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        internal abstract bool EqualsData(IpV6MobilityOption other);
    }
}