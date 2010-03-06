using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PcapDotNet.Base
{
    /// <summary>
    /// A 128 bit unsigned integer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UInt128
    {
        /// <summary>
        /// The number of bytes this type will take.
        /// </summary>
        public const int SizeOf = 16;

        /// <summary>
        /// The maximum value of this type.
        /// </summary>
        public static readonly UInt128 MaxValue = Parse("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        /// <summary>
        /// A Zero UInt128 value.
        /// The minimum UInt128 value.
        /// </summary>
        public static readonly UInt128 Zero = Parse("00000000000000000000000000000000", NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        /// <summary>
        /// Creates a value using two 64 bit values.
        /// </summary>
        /// <param name="mostSignificant">The most significant 64 bits of the value.</param>
        /// <param name="leastSignificant">The least significant 64 bits of the value.</param>
        public UInt128(ulong mostSignificant, ulong leastSignificant)
        {
            _mostSignificant = mostSignificant;
            _leastSignificant = leastSignificant;
        }
/*
        /// <summary>
        /// Creates a value using 8 16 bits values.
        /// </summary>
        /// <param name="values">The 16 bits values ordered so that the first value is the most significant.</param>
//        public UInt128(ushort[] values)
//        {
//            if (values.Length != 8)
//                throw new ArgumentException("UInt128 must be created by 8 ushorts and not " + values.Length + " ushorts");
//
//            _mostSignificant =
//                ((ulong)values[0] << 48) +
//                ((ulong)values[1] << 32) +
//                ((ulong)values[2] << 16) +
//                values[3];
//
//            _leastSignificant =
//                ((ulong)values[4] << 48) +
//                ((ulong)values[5] << 32) +
//                ((ulong)values[6] << 16) +
//                values[7];
//        }
        */
        /// <summary>
        /// Converts the string representation of a number in a specified style to its 128-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="value">A string representing the number to convert.</param>
        /// <param name="style">
        /// A bitwise combination of NumberStyles values that indicates the permitted format of value.
        /// A typical value to specify is NumberStyles.Integer.
        /// </param>
        /// <param name="provider">An System.IFormatProvider that supplies culture-specific formatting information about value.</param>
        /// <returns>A 128-bit unsigned integer equivalent to the number specified in s.</returns>
        public static UInt128 Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            if (style != NumberStyles.HexNumber)
                throw new NotSupportedException("Only " + NumberStyles.HexNumber + " style is supported");

            ulong mostSignficantLong = 0;
            ulong leastSignficantLong = 0;
            if (value.Length > 16)
            {
                leastSignficantLong = ulong.Parse(value.Substring(value.Length - 16, 16), style, provider);
                value = value.Substring(0, value.Length - 16);
                mostSignficantLong = ulong.Parse(value, style, provider);
            }
            else
                leastSignficantLong = ulong.Parse(value, style, provider);

            return new UInt128(mostSignficantLong, leastSignficantLong);
        }

        /// <summary>
        /// Converts a 64 bit unsigned integer to a 128 bit unsigned integer by taking all the 64 bits.
        /// </summary>
        /// <param name="value">The 64 bit value to convert.</param>
        /// <returns>The 128 bit value created by taking all the 64 bits of the 64 bit value.</returns>
        public static implicit operator UInt128(ulong value)
        {
            return new UInt128(0, value);
        }

        /// <summary>
        /// Converts the 128 bits unsigned integer to a 64 bits unsigned integer.
        /// </summary>
        /// <param name="value">The 128 bit value to convert.</param>
        /// <returns>The 64 bit value converted from the 128 bit value.</returns>
        public static explicit operator ulong(UInt128 value)
        {
            return value._leastSignificant;
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="other">The value to compare to.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public bool Equals(UInt128 other)
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
            return (obj is UInt128) &&
                   Equals((UInt128)obj);
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public static bool operator ==(UInt128 value1, UInt128 value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Returns true iff the two values represent different values.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent different values.</returns>
        public static bool operator !=(UInt128 value1, UInt128 value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Shifts its first operand right by the number of bits specified by its second operand.
        /// </summary>
        /// <param name="value">The value to shift.</param>
        /// <param name="numberOfBits">The number of bits to shift.</param>
        /// <returns>The value after it was shifted by the given number of bits.</returns>
        public static UInt128 operator >> (UInt128 value, int numberOfBits)
        {
            return RightShift(value, numberOfBits);
        }

        /// <summary>
        /// Shifts its first operand right by the number of bits specified by its second operand.
        /// </summary>
        /// <param name="value">The value to shift.</param>
        /// <param name="numberOfBits">The number of bits to shift.</param>
        /// <returns>The value after it was shifted by the given number of bits.</returns>
        public static UInt128 RightShift(UInt128 value, int numberOfBits)
        {
            numberOfBits %= 128;
            if (numberOfBits >= 64)
                return new UInt128(0, value._mostSignificant >> (numberOfBits - 64));
            if (numberOfBits == 0)
                return value;
            return new UInt128(value._mostSignificant >> numberOfBits, (value._leastSignificant >> numberOfBits) + (value._mostSignificant << (64 - numberOfBits)));
        }

        /// <summary>
        /// Bitwise ands between two values.
        /// </summary>
        /// <param name="value1">The first value to do bitwise and.</param>
        /// <param name="value2">The second value to do bitwise and.</param>
        /// <returns>The two values after they were bitwise anded.</returns>
        public static UInt128 operator &(UInt128 value1, UInt128 value2)
        {
            return BitwiseAnd(value1, value2);
        }

        /// <summary>
        /// Bitwise ands between two values.
        /// </summary>
        /// <param name="value1">The first value to do bitwise and.</param>
        /// <param name="value2">The second value to do bitwise and.</param>
        /// <returns>The two values after they were bitwise anded.</returns>
        public static UInt128 BitwiseAnd(UInt128 value1, UInt128 value2)
        {
            return new UInt128(value1._mostSignificant & value2._mostSignificant, value1._leastSignificant & value2._leastSignificant);
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
            return ((ulong)this).GetHashCode();
        }

        /// <summary>
        /// Returns the hexadecimal string representation of the 128 bits unsigned integer.
        /// </summary>
        public string ToString(string format)
        {
            if (format != "X32")
                throw new NotSupportedException("Only X32 format is supported");
            return _mostSignificant.ToString("X16", CultureInfo.InvariantCulture) + _leastSignificant.ToString("X16", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Currently not supported since only X32 string format is supported (and not decimal).
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            throw new NotSupportedException("Only X32 format is supported");
        }

        private readonly ulong _leastSignificant;
        private readonly ulong _mostSignificant;
    }
}