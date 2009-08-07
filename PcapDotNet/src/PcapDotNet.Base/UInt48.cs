using System;
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
        /// Converts the string representation of a number in a specified style to its 48-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="value">A string representing the number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of NumberStyles values that indicates the permitted format of s. 
        /// A typical value to specify is NumberStyles.Integer.
        /// </param>
        /// <param name="provider">An System.IFormatProvider that supplies culture-specific formatting information about value.</param>
        /// <returns>A 48-bit unsigned integer equivalent to the number specified in s.</returns>
        public static UInt48 Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            ulong parsedValue = ulong.Parse(value, style, provider);
            if (parsedValue > MaxValue)
                throw new FormatException("parsed value " + parsedValue + " is larger than max value " + MaxValue);

            return (UInt48)parsedValue;
        }

        /// <summary>
        /// Converts a 32 bit unsigned integer to a 48 bit unsigned integer by taking all the 32 bits.
        /// </summary>
        /// <param name="value">The 32 bit value to convert.</param>
        /// <returns>The 48 bit value created by taking all the 32 bits of the 32bit value.</returns>
        public static implicit operator UInt48(uint value)
        {
            return new UInt48(value);
        }

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
        /// Converts a 64 bit unsigned integer to a 48 bit unsigned integer by taking the 48 least significant bits.
        /// </summary>
        /// <param name="value">The 64 bit value to convert.</param>
        /// <returns>The 48 bit value created by taking the 48 least significant bits of the 64 bit value.</returns>
        public static explicit operator UInt48(ulong value)
        {
            return new UInt48((long)value);
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
        /// Converts the 48 bits unsigned integer to a 64 bits unsigned integer.
        /// </summary>
        /// <param name="value">The 48 bit value to convert.</param>
        /// <returns>The 64 bit value converted from the 48 bit value.</returns>
        public static implicit operator ulong(UInt48 value)
        {
            return (ulong)value.ToLong();
        }

        /// <summary>
        /// Converts the 48 bits unsigned integer to an 8 bits unsigned integer.
        /// </summary>
        /// <param name="value">The 48 bit value to convert.</param>
        /// <returns>The 8 bit value converted from the 48 bit value.</returns>
        public static explicit operator byte(UInt48 value)
        {
            return (byte)value.ToByte();
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

        private byte ToByte()
        {
            return (byte)_leastSignificant;
        }

        private readonly uint _leastSignificant;
        private readonly ushort _mostSignificant;

    }
}