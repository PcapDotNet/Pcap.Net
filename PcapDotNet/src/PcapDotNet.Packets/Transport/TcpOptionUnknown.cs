using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// An unknown TCP option.
    /// </summary>
    public sealed class TcpOptionUnknown : TcpOptionComplex, IOptionUnknownFactory<TcpOptionType>, IEquatable<TcpOptionUnknown>
    {
        /// <summary>
        /// The minimum number of bytes this option take.
        /// </summary>
        public const int OptionMinimumLength = 2;

        /// <summary>
        /// The minimum number of bytes this option's value take.
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        /// <summary>
        /// Creates an unknown TCP option by the given type and data.
        /// </summary>
        public TcpOptionUnknown(TcpOptionType optionType, IList<byte> data)
            : base(optionType)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }

        /// <summary>
        /// The default unknown option is with type 255 and no data.
        /// </summary>
        public TcpOptionUnknown()
            : this((TcpOptionType)255, new byte[] { })
        {
        }

        /// <summary>
        /// The data of the unknown option.
        /// </summary>
        public ReadOnlyCollection<byte> Data { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionMinimumLength + Data.Count; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }

        /// <summary>
        /// Two unknown options are equal iff they are of equal type and equal data.
        /// </summary>
        public bool Equals(TcpOptionUnknown other)
        {
            if (other == null)
                return false;

            return OptionType == other.OptionType &&
                   Data.SequenceEqual(other.Data);
        }

        /// <summary>
        /// Two unknown options are equal iff they are of equal type and equal data.
        /// </summary>
        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionUnknown);
        }

        /// <summary>
        /// The hash code for an unknown option is the hash code for the option type xored with the hash code of the data.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Data.BytesSequenceGetHashCode();
        }

        /// <summary>
        /// Creates an unknown option from its type and by reading a buffer for its value.
        /// </summary>
        /// <param name="optionType">The type of the unknown option.</param>
        /// <param name="buffer">The buffer of bytes to read the value of the unknown option.</param>
        /// <param name="offset">The offset in the buffer to start reading the bytes.</param>
        /// <param name="valueLength">The number of bytes to read from the buffer.</param>
        /// <returns>An option created from the given type and buffer.</returns>
        public Option CreateInstance(TcpOptionType optionType, byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength < OptionValueMinimumLength)
                return null;

            byte[] data = buffer.ReadBytes(ref offset, valueLength);
            return new TcpOptionUnknown(optionType, data);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Data);
        }
    }
}