using System;
using System.Linq;

namespace Packets
{
    public abstract class IpV4OptionRoute : IpV4Option, IEquatable<IpV4OptionRoute>
    {
        public const int OptionMinimumLength = 3;
        public const byte PointedAddressIndexMaxValue = byte.MaxValue / 4 - 1;

        public byte PointedAddressIndex
        {
            get { return _pointedAddressIndex; }
        }

        public override int Length
        {
            get { return OptionMinimumLength + 4 * _addresses.Length; }
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        public bool Equals(IpV4OptionRoute other)
        {
            if (other == null)
                return false;

            return Equivalent(other) &&
                   PointedAddressIndex == other.PointedAddressIndex &&
                   _addresses.SequenceEqual(other._addresses);
        }

        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionRoute);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
            buffer[offset++] = (byte)(OptionMinimumLength + 1 + PointedAddressIndex * 4);
            for (int i = 0; i != _addresses.Length; ++i)
                buffer.Write(ref offset, _addresses[i].ToValue(), Endianity.Big);
        }

        protected static bool TryRead(out IpV4Address[] addresses, out byte pointedAddressIndex,
                                      byte[] buffer, ref int offset, int length)
        {
            addresses = null;
            pointedAddressIndex = 0;

            if (length < OptionMinimumLength - 1)
                return false;

            byte optionLength = buffer[offset++];
            if (optionLength < OptionMinimumLength || optionLength > length + 1 || optionLength % 4 != 3)
                return false;

            byte pointer = buffer[offset++];
            if (pointer % 4 != 0 || pointer < 4)
                return false;

            pointedAddressIndex = (byte)(pointer / 4 - 1);

            int numAddresses = (optionLength - 3) / 4;
            addresses = new IpV4Address[numAddresses];
            for (int i = 0; i != numAddresses; ++i)
                addresses[i] = new IpV4Address(buffer.ReadUInt(ref offset, Endianity.Big));

            return true;
        }

        protected IpV4OptionRoute(IpV4OptionType optionType, IpV4Address[] addresses, byte pointedAddressIndex)
            : base(optionType)
        {
            _addresses = addresses;
            _pointedAddressIndex = pointedAddressIndex;
        }

        private readonly IpV4Address[] _addresses;
        private readonly byte _pointedAddressIndex;
    }
}