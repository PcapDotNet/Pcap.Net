using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
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
    public abstract class IpV6Option : Option, IEquatable<IpV6Option>
    {
        /// <summary>
        /// The type of the IP option.
        /// </summary>
        public IpV6OptionType OptionType { get; private set; }

        /// <summary>
        /// True iff the option type and data are equal.
        /// </summary>
        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV6Option);
        }

        /// <summary>
        /// True iff the option type and data are equal.
        /// </summary>
        public bool Equals(IpV6Option other)
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

        internal IpV6Option(IpV6OptionType type)
        {
            OptionType = type;
        }

        internal abstract bool EqualsData(IpV6Option other);

        internal abstract int GetDataHashCode();

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }
    }
}