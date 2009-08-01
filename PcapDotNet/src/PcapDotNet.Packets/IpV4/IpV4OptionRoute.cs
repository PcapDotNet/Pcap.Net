using System;
using System.Linq;

namespace PcapDotNet.Packets
{
    public abstract class IpV4OptionRoute : IpV4OptionComplex, IEquatable<IpV4OptionRoute>
    {
        public const int OptionMinimumLength = 3;
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;
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

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   PointedAddressIndex ^
                   _addresses.Aggregate(0, (value, address) => value ^ address.GetHashCode());
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
            buffer[offset++] = (byte)(OptionMinimumLength + 1 + PointedAddressIndex * 4);
            for (int i = 0; i != _addresses.Length; ++i)
                buffer.Write(ref offset, _addresses[i], Endianity.Big);
        }

        protected static bool TryRead(out IpV4Address[] addresses, out byte pointedAddressIndex,
                                      byte[] buffer, ref int offset, byte valueLength)
        {
            addresses = null;
            pointedAddressIndex = 0;

            if (valueLength < OptionValueMinimumLength)
                return false;

            if (valueLength % 4 != 1)
                return false;

            byte pointer = buffer[offset++];
            if (pointer % 4 != 0 || pointer < 4)
                return false;

            pointedAddressIndex = (byte)(pointer / 4 - 1);

            int numAddresses = valueLength / 4;
            addresses = new IpV4Address[numAddresses];
            for (int i = 0; i != numAddresses; ++i)
                addresses[i] = buffer.ReadIpV4Address(ref offset, Endianity.Big);

            return true;
        }

        protected IpV4OptionRoute(IpV4OptionType optionType, IpV4Address[] addresses, byte pointedAddressIndex)
            : base(optionType)
        {
            if (pointedAddressIndex > PointedAddressIndexMaxValue)
                throw new ArgumentOutOfRangeException("pointedAddressIndex", pointedAddressIndex, "Maximum value is " + PointedAddressIndexMaxValue);

            _addresses = addresses;
            _pointedAddressIndex = pointedAddressIndex;
        }

        private readonly IpV4Address[] _addresses;
        private readonly byte _pointedAddressIndex;
    }
}