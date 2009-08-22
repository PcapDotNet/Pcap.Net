using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Transport
{
    public class TcpOptionUnknown : TcpOptionComplex, IOptionUnkownFactory<TcpOptionType>, IEquatable<TcpOptionUnknown>
    {
        /// <summary>
        /// The minimum number of bytes this option take.
        /// </summary>
        public const int OptionMinimumLength = 2;

        /// <summary>
        /// The minimum number of bytes this option's value take.
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        public TcpOptionUnknown(TcpOptionType optionType, IList<byte> data)
            : base(optionType)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }

        public TcpOptionUnknown()
            : this((TcpOptionType)255, new byte[] { })
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

        public bool Equals(TcpOptionUnknown other)
        {
            if (other == null)
                return false;

            return OptionType == other.OptionType &&
                   Data.SequenceEqual(other.Data);
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionUnknown);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Data.BytesSequenceGetHashCode();
        }

        public Option CreateInstance(TcpOptionType optionType, byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength < OptionValueMinimumLength)
                return null;

            byte[] data = new byte[valueLength];
            buffer.BlockCopy(offset, data, 0, valueLength);
            offset += valueLength;

            return new TcpOptionUnknown(optionType, data);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            foreach (byte value in Data)
                buffer.Write(ref offset, value);
        }
    }
}