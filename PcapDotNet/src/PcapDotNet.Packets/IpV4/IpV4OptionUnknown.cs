using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    public class IpV4OptionUnknown : IpV4OptionComplex, IOptionUnkownFactory<IpV4OptionType>, IEquatable<IpV4OptionUnknown>
    {
        /// <summary>
        /// The minimum number of bytes this option take.
        /// </summary>
        public const int OptionMinimumLength = 2;

        /// <summary>
        /// The minimum number of bytes this option's value take.
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        public IpV4OptionUnknown(IpV4OptionType optionType, IList<byte> data)
            : base(optionType)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }

        public IpV4OptionUnknown()
            : this((IpV4OptionType)255, new byte[] {})
        {
        }

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

        public bool Equals(IpV4OptionUnknown other)
        {
            if (other == null)
                return false;

            return OptionType == other.OptionType &&
                   Data.SequenceEqual(other.Data);
        }

        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionUnknown);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Data.BytesSequenceGetHashCode();
        }

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