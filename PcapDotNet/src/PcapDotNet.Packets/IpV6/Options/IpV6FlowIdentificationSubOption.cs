using System;
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

        public override int Length
        {
            get { return sizeof(byte); }
        }

        internal abstract IpV6FlowIdentificationSubOption CreateInstance(DataSegment data);

        protected IpV6FlowIdentificationSubOption(IpV6FlowIdentificationSubOptionType type)
        {
            OptionType = type;
        }

        public override sealed bool Equals(Option other)
        {
            return Equals(other as IpV6FlowIdentificationSubOption);
        }

        public bool Equals(IpV6FlowIdentificationSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        internal abstract bool EqualsData(IpV6FlowIdentificationSubOption other);

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }
    }
}