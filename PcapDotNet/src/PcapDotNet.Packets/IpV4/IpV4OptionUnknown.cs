using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// An unknown IPv4 option.
    /// </summary>
    public sealed class IpV4OptionUnknown : IpV4OptionComplex, IOptionUnknownFactory<IpV4OptionType>, IEquatable<IpV4OptionUnknown>
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
        /// Creates an unknown IPv4 option by the given type and data.
        /// </summary>
        public IpV4OptionUnknown(IpV4OptionType optionType, IList<byte> data)
            : base(optionType)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }

        /// <summary>
        /// The default unknown option is with type 255 and no data.
        /// </summary>
        public IpV4OptionUnknown()
            : this((IpV4OptionType)255, new byte[] {})
        {
        }

        /// <summary>
        /// Returns the Data of the option.
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
        public bool Equals(IpV4OptionUnknown other)
        {
            if (other == null)
                return false;

            return OptionType == other.OptionType &&
                   Data.SequenceEqual(other.Data);
        }

        /// <summary>
        /// Two unknown options are equal iff they are of equal type and equal data.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionUnknown);
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
        public Option CreateInstance(IpV4OptionType optionType, byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength < OptionValueMinimumLength)
                return null;

            byte[] data = new byte[valueLength];
            buffer.BlockCopy(offset, data, 0, valueLength);
            offset += valueLength;

            return new IpV4OptionUnknown(optionType, data);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            foreach (byte value in Data)
                buffer.Write(ref offset, value);
        }
    }
}