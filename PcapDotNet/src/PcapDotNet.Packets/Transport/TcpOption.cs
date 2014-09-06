using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Represents a TCP option according to rfc 793. 
    /// </summary>
    public abstract class TcpOption : V4Option, IEquatable<TcpOption>
    {
        /// <summary>
        /// This option code indicates the end of the option list.  
        /// This might not coincide with the end of the TCP header according to the Data Offset field.  
        /// This is used at the end of all options, not the end of each option, 
        /// and need only be used if the end of the options would not otherwise coincide with the end of the TCP header.
        /// </summary>
        public static TcpOption End
        {
            get { return _end; }
        }

        /// <summary>
        /// This option code may be used between options, 
        /// for example, to align the beginning of a subsequent option on a word boundary.
        /// There is no guarantee that senders will use this option, 
        /// so receivers must be prepared to process options even if they do not begin on a word boundary.
        /// </summary>
        public static TcpOption Nop
        {
            get { return _nop; }
        }

        /// <summary>
        /// The type of the TCP option.
        /// </summary>
        public TcpOptionType OptionType { get; private set; }

        /// <summary>
        /// Checks whether two options have equivalent type.
        /// Useful to check if an option that must appear at most once appears in the list.
        /// </summary>
        public sealed override bool Equivalent(Option other)
        {
            return OptionType == ((TcpOption)other).OptionType;
        }

        /// <summary>
        /// Checks if the two options are exactly the same - including type and value.
        /// </summary>
        public bool Equals(TcpOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && EqualsData(other);
        }

        /// <summary>
        /// Checks if the two options are exactly the same - including type and value.
        /// </summary>
        public sealed override bool Equals(Option other)
        {
            return Equals(other as TcpOption);
        }

        /// <summary>
        /// The hash code for a tcp option is the hash code for the option type.
        /// This should be overridden by tcp options with data.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Sequence.GetHashCode(OptionType, GetDataHashCode());
        }

        /// <summary>
        /// The string that represents the option type.
        /// </summary>
        public sealed override string ToString()
        {
            return OptionType.ToString();
        }

        internal abstract bool EqualsData(TcpOption other);

        internal abstract int GetDataHashCode();

        internal sealed override V4Option Read(byte[] buffer, ref int offset, int length)
        {
            int offsetEnd = offset + length;
            if (offset == offsetEnd)
                return null;

            TcpOptionType optionType = (TcpOptionType)buffer[offset++];
            switch (optionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return End;
                case TcpOptionType.NoOperation:
                    return Nop;

                default:
                    return (V4Option)OptionComplexFactory<TcpOptionType>.Read(optionType, buffer, ref offset, offsetEnd - offset);
            }
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        /// <summary>
        /// Initializes the option type.
        /// </summary>
        protected TcpOption(TcpOptionType optionType)
        {
            OptionType = optionType;
        }
        
        private static readonly TcpOption _end = new TcpOptionSimple(TcpOptionType.EndOfOptionList);
        private static readonly TcpOption _nop = new TcpOptionSimple(TcpOptionType.NoOperation);
    }
}