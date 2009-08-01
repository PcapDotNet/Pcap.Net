using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapDotNet.Base
{
    public struct UInt24
    {
        public static readonly UInt24 MaxValue = (UInt24)0x00FFFFFF;

        private UInt24(int value)
        {
            _mostSignificant = (byte)((value >> 16) & 0x00FF);
            _leastSignificant = (ushort)(value & 0x0000FFFF);
        }

        public static explicit operator UInt24(int value)
        {
            return new UInt24(value);
        }

        public static implicit operator int(UInt24 value)
        {
            return value.ToInt();
        }

        private int ToInt()
        {
            return _mostSignificant << 16 + _leastSignificant;
        }


        public bool Equals(UInt24 other)
        {
            return other._mostSignificant == _mostSignificant && other._leastSignificant == _leastSignificant;
        }

        public override bool Equals(object obj)
        {
            return (obj is UInt24) &&
                   Equals((UInt24)obj);
        }

        public static bool operator ==(UInt24 value1, UInt24 value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(UInt24 value1, UInt24 value2)
        {
            return !(value1 == value2);
        }

        public override int GetHashCode()
        {
            return this;
        }

        private readonly byte _mostSignificant;
        private readonly ushort _leastSignificant;
    }
}
