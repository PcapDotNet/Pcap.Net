using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// </summary>
    public abstract class IpV6FlowIdentificationSubOption : Option, IEquatable<IpV6FlowIdentificationSubOption>
    {
        /// <summary>
        /// The type of the option.
        /// </summary>
        public IpV6FlowIdentificationSubOptionType OptionType { get; private set; }

        /// <summary>
        /// The number of bytes this option takes.
        /// </summary>
        public override int Length
        {
            get { return sizeof(byte); }
        }

        /// <summary>
        /// True iff the two options are equal.
        /// </summary>
        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV6FlowIdentificationSubOption);
        }

        /// <summary>
        /// True iff the two options are equal.
        /// </summary>
        public bool Equals(IpV6FlowIdentificationSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        /// <summary>
        /// Returns a hash code of the option.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Sequence.GetHashCode(OptionType, GetDataHashCode());
        }

        internal abstract IpV6FlowIdentificationSubOption CreateInstance(DataSegment data);

        internal IpV6FlowIdentificationSubOption(IpV6FlowIdentificationSubOptionType type)
        {
            OptionType = type;
        }

        internal abstract bool EqualsData(IpV6FlowIdentificationSubOption other);

        internal abstract object GetDataHashCode();

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }
    }
}