using System.Globalization;
using System.Runtime.InteropServices;

namespace PcapDotNet.Base
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UInt48
    {
        public const int SizeOf = 6;
        public static readonly UInt48 MaxValue = (UInt48)0x0000FFFFFFFFFFFF;

        public static explicit operator UInt48(long value)
        {
            return new UInt48(value);
        }

        public static implicit operator long(UInt48 value)
        {
            return value.ToLong();
        }

        public bool Equals(UInt48 other)
        {
            return _mostSignificant == other._mostSignificant &&
                   _leastSignificant == other._leastSignificant;
        }

        public override bool Equals(object obj)
        {
            return (obj is UInt48) &&
                   Equals((UInt48)obj);
        }

        public static bool operator ==(UInt48 value1, UInt48 value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(UInt48 value1, UInt48 value2)
        {
            return !(value1 == value2);
        }

        public override int GetHashCode()
        {
            return ((long)this).GetHashCode();
        }

        public override string ToString()
        {
            return ((long)this).ToString(CultureInfo.InvariantCulture);
        }

        private UInt48(long value)
        {
            _mostSignificant = (ushort)(value >> 32);
            _leastSignificant = (uint)value;
        }

        private long ToLong()
        {
            return (((long)_mostSignificant) << 32) + _leastSignificant;
        }

        private readonly uint _leastSignificant;
        private readonly ushort _mostSignificant;
    }
}