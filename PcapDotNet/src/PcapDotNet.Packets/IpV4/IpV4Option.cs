using System;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents an ip option according to rfc 791. 
    /// </summary>
    public abstract class IpV4Option : V4Option, IEquatable<IpV4Option>
    {
        ///<summary>
        /// This option indicates the end of the option list.<br/>
        /// This might not coincide with the end of the internet header according to the internet header length.
        /// This is used at the end of all options, not the end of each option, and need only be used if the end of the options would not otherwise coincide with the end of the internet header.
        /// May be copied, introduced, or deleted on fragmentation, or for any other reason.
        ///</summary>
        public static IpV4OptionSimple End
        {
            get { return _end; }
        }

        /// <summary>
        /// This option may be used between options, for example, to align the beginning of a subsequent option on a 32 bit boundary.
        /// May be copied, introduced, or deleted on fragmentation, or for any other reason.
        /// </summary>
        public static IpV4OptionSimple Nop
        {
            get { return _nop; }
        }

        /// <summary>
        /// The type of the IP option.
        /// </summary>
        public IpV4OptionType OptionType
        {
            get { return _type; }
        }

        /// <summary>
        /// Checks whether two options have equivalent type.
        /// Useful to check if an option that must appear at most once appears in the list.
        /// </summary>
        public sealed override bool Equivalent(Option other)
        {
            return OptionType == ((IpV4Option)other).OptionType;
        }

        /// <summary>
        /// Checks if the two options are exactly the same - including type and value.
        /// </summary>
        public virtual bool Equals(IpV4Option other)
        {
            if (other == null)
                return false;
            return Equivalent(other);
        }

        /// <summary>
        /// Checks if the two options are exactly the same - including type and value.
        /// </summary>
        public sealed override bool Equals(Option other)
        {
            return Equals(other as IpV4Option);
        }

        /// <summary>
        /// The hash code of the option is the hash code of the option type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return OptionType.GetHashCode();
        }

        /// <summary>
        /// The string of the option is the string of the option type.
        /// </summary>
        public sealed override string ToString()
        {
            return OptionType.ToString();
        }

        internal sealed override V4Option Read(byte[] buffer, ref int offset, int length)
        {
            int offsetEnd = offset + length;
            if (offset == offsetEnd)
                return null;

            IpV4OptionType optionType = (IpV4OptionType)buffer[offset++];
            switch (optionType)
            {
                case IpV4OptionType.EndOfOptionList:
                    return End;
                case IpV4OptionType.NoOperation:
                    return Nop;

                default:
                    return (V4Option)OptionComplexFactory<IpV4OptionType>.Read(optionType, buffer, ref offset, offsetEnd - offset);
            }
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        /// <summary>
        /// Constructs the option by type.
        /// </summary>
        protected IpV4Option(IpV4OptionType type)
        {
            _type = type;
        }

        private static readonly IpV4OptionSimple _end = new IpV4OptionSimple(IpV4OptionType.EndOfOptionList);
        private static readonly IpV4OptionSimple _nop = new IpV4OptionSimple(IpV4OptionType.NoOperation);

        private readonly IpV4OptionType _type;
    }
}