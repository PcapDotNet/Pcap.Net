using System;
using System.Globalization;

namespace PcapDotNet.Base
{
    public struct SerialNumber32 : IEquatable<SerialNumber32>, IComparable<SerialNumber32>
    {
        public const int SizeOf = sizeof(uint);
        public const int SerialBits = 32;
        public const uint MaxAdditiveNumber = ((uint)1 << (SerialBits - 1)) - 1;

        public SerialNumber32(uint value)
        {
            _value = value;
        }

        public uint Value { get { return _value; } }

        public SerialNumber32 Add(uint value)
        {
            if (value > MaxAdditiveNumber)
                throw new ArgumentOutOfRangeException("value", value,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot add a number bigger than {0}", MaxAdditiveNumber));

            return _value + value;
        }

        public bool Equals(SerialNumber32 other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is SerialNumber32 &&
                   Equals((SerialNumber32)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(SerialNumber32 other)
        {
            if (Equals(other))
                return 0;

            if (Value < other.Value)
                return (other.Value - Value < MaxAdditiveNumber + 1 ? -1 : 1);
            return (Value - other.Value < MaxAdditiveNumber + 1 ? 1 : -1);
        }

        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        public static implicit operator SerialNumber32(uint value)
        {
            return new SerialNumber32(value);
        }

        public static bool operator ==(SerialNumber32 value1, SerialNumber32 value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(SerialNumber32 value1, SerialNumber32 value2)
        {
            return !(value1 == value2);
        }

        public static bool operator <(SerialNumber32 value1, SerialNumber32 value2)
        {
            return value1.CompareTo(value2) < 0;
        }

        public static bool operator >(SerialNumber32 value1, SerialNumber32 value2)
        {
            return value1.CompareTo(value2) > 0;
        }

        private readonly uint _value;
    }
}