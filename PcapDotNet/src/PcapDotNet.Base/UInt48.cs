using System.Globalization;
using System.Runtime.InteropServices;

namespace PcapDotNet.Base
{
    /// <summary>
    /// A 48 bit unsigned integer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UInt48
    {
        /// <summary>
        /// The number of bytes this type will take.
        /// </summary>
        public const int SizeOf = 6;

        /// <summary>
        /// The maximum value of this type.
        /// </summary>
        public static readonly UInt48 MaxValue = (UInt48)0x0000FFFFFFFFFFFF;

        /// <summary>
        /// Converts a 64 bit signed integer to a 48 bit unsigned integer by taking the 48 least significant bits.
        /// </summary>
        /// <param name="value">The 64 bit value to convert.</param>
        /// <returns>The 48 bit value created by taking the 48 least significant bits of the 64 bit value.</returns>
        public static explicit operator UInt48(long value)
        {
            return new UInt48(value);
        }

        /// <summary>
        /// Converts the 48 bits unsigned integer to a 64 bits signed integer.
        /// </summary>
        /// <param name="value">The 48 bit value to convert.</param>
        /// <returns>The 64 bit value converted from the 48 bit value.</returns>
        public static implicit operator long(UInt48 value)
        {
            return value.ToLong();
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="other">The value to compare to.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public bool Equals(UInt48 other)
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
            return (obj is UInt48) &&
                   Equals((UInt48)obj);
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public static bool operator ==(UInt48 value1, UInt48 value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Returns true iff the two values represent different values.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent different values.</returns>
        public static bool operator !=(UInt48 value1, UInt48 value2)
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
            return ((long)this).GetHashCode();
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