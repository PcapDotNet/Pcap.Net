using System;
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

        protected IpV6Option(IpV6OptionType type)
        {
            OptionType = type;
        }

        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV6Option);
        }

        public bool Equals(IpV6Option other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        internal abstract bool EqualsData(IpV6Option other);

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }
    }

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

        internal abstract IpV6MobilityOption CreateInstance(DataSegment data);

        protected IpV6MobilityOption(IpV6MobilityOptionType type)
        {
            OptionType = type;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

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

        internal abstract bool EqualsData(IpV6MobilityOption other);
    }
}