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

        /// <summary>
        /// Returns true iff the given option is an equivalent option.
        /// </summary>
        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV6MobilityOption);
        }

        /// <summary>
        /// Returns true iff the given option is an equivalent option.
        /// </summary>
        public bool Equals(IpV6MobilityOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        /// <summary>
        /// Returns a hash code for the option.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Sequence.GetHashCode(OptionType, GetDataHashCode());
        }

        /// <summary>
        /// True iff parsing of this option didn't encounter issues.
        /// </summary>
        public virtual bool IsValid { get { return true; } }

        internal IpV6MobilityOption(IpV6MobilityOptionType type)
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