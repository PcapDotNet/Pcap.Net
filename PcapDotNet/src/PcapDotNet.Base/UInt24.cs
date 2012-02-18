using System.Globalization;
using System.Runtime.InteropServices;

namespace PcapDotNet.Base
{
    /// <summary>
    /// A 24 bit unsigned integer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UInt24
    {
        /// <summary>
        /// The number of bytes this type will take.
        /// </summary>
        public const int SizeOf = 3;

        /// <summary>
        /// The maximum value of this type.
        /// </summary>
        public static readonly UInt24 MaxValue = (UInt24)0x00FFFFFF;

        /// <summary>
        /// Converts a 32 bit signed integer to a 24 bit unsigned integer by taking the 24 least significant bits.
        /// </summary>
        /// <param name="value">The 32 bit value to convert.</param>
        /// <returns>The 24 bit value created by taking the 24 least significant bits of the 32 bit value.</returns>
        public static explicit operator UInt24(int value)
        {
            return new UInt24(value);
        }

        /// <summary>
        /// Converts a 32 bit unsigned integer to a 24 bit unsigned integer by taking the 24 least significant bits.
        /// </summary>
        /// <param name="value">The 32 bit value to convert.</param>
        /// <returns>The 24 bit value created by taking the 24 least significant bits of the 32 bit value.</returns>
        public static explicit operator UInt24(uint value)
        {
            return new UInt24((int)value);
        }

        /// <summary>
        /// Converts the 24 bits unsigned integer to a 32 bits signed integer.
        /// </summary>
        /// <param name="value">The 24 bit value to convert.</param>
        /// <returns>The 32 bit value converted from the 24 bit value.</returns>
        public static implicit operator int(UInt24 value)
        {
            return value.ToInt();
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="other">The value to compare to.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public bool Equals(UInt24 other)
        {
            return _mostSignificant == other._mostSignificant &&
                   _leastSignificant == other._leastSignificant;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return (obj is UInt24) &&
                   Equals((UInt24)obj);
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public static bool operator ==(UInt24 value1, UInt24 value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Returns true iff the two values represent different values.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent different values.</returns>
        public static bool operator !=(UInt24 value1, UInt24 value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return ((int)this).ToString(CultureInfo.InvariantCulture);
        }

        private UInt24(int value)
        {
            _mostSignificant = (byte)(value >> 16);
            _leastSignificant = (ushort)value;
        }

        private int ToInt()
        {
            return (_mostSignificant << 16) + _leastSignificant;
        }

        private readonly ushort _leastSignificant;
        private readonly byte _mostSignificant;
    }
}
