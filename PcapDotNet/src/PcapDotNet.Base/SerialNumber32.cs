using System;
using System.Globalization;

namespace PcapDotNet.Base
{
    /// <summary>
    /// A 32 bit serial number as defined in RFC 1982.
    /// </summary>
    public struct SerialNumber32 : IEquatable<SerialNumber32>, IComparable<SerialNumber32>
    {
        /// <summary>
        /// The number of bytes this type takes.
        /// </summary>
        public const int SizeOf = sizeof(uint);

        /// <summary>
        /// Number of bits of the serial number.
        /// </summary>
        public const int SerialBits = 32;

        /// <summary>
        /// The maximum value that can be added to the serial number.
        /// </summary>
        public const uint MaxAdditiveNumber = ((uint)1 << (SerialBits - 1)) - 1;

        /// <summary>
        /// Constructs a serial number from an unsigned value.
        /// </summary>
        /// <param name="value">The value to set the serial number.</param>
        public SerialNumber32(uint value)
        {
            _value = value;
        }

        /// <summary>
        /// The value of the serial number.
        /// </summary>
        public uint Value { get { return _value; } }

        /// <summary>
        /// Adds a value to the serial number and returns the result.
        /// <paramref name="value"/> should not be bigger than <see cref="MaxAdditiveNumber"/>.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>A new serial number that represents the sum of the original serial number and <paramref name="value"/>.</returns>
        public SerialNumber32 Add(uint value)
        {
            if (value > MaxAdditiveNumber)
                throw new ArgumentOutOfRangeException("value", value,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot add a number bigger than {0}", MaxAdditiveNumber));

            return _value + value;
        }

        /// <summary>
        /// Two serial numbers are equal if their value is equal.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>True iff the two serial numbers are equal.</returns>
        public bool Equals(SerialNumber32 other)
        {
            return Value == other.Value;
        }

        /// <summary>
        /// Two serial numbers are equal if their value is equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True iff the two serial numbers are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is SerialNumber32 &&
                   Equals((SerialNumber32)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has the following meanings:
        /// Value Meaning Less than zero This object is less than the <paramref name="other"/>.
        /// parameter.Zero This object is equal to <paramref name="other"/>.
        /// Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(SerialNumber32 other)
        {
            if (Equals(other))
                return 0;

            if (Value < other.Value)
                return (other.Value - Value < MaxAdditiveNumber + 1 ? -1 : 1);
            return (Value - other.Value < MaxAdditiveNumber + 1 ? 1 : -1);
        }

        /// <summary>
        /// The string representation of this serial number.
        /// </summary>
        /// <returns>A string representing this serial number.</returns>
        public override string ToString()
        {
            return ToString(CultureInfo.InvariantCulture);
        }

        ///<summary>
        /// The string representation of this serial number using the given format provider.
        ///</summary>
        ///<param name="provider">The format of the output string.</param>
        ///<returns>A string representing this serial number using the given format provider.</returns>
        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        /// <summary>
        /// Implicitly cast a uint to a serial number.
        /// </summary>
        /// <param name="value">The value to cast.</param>
        /// <returns>The casted value.</returns>
        public static implicit operator SerialNumber32(uint value)
        {
            return new SerialNumber32(value);
        }

        /// <summary>
        /// Returns true iff the two serial numbers are equal.
        /// </summary>
        /// <param name="value1">First serial number to compare.</param>
        /// <param name="value2">Second serial number to compare.</param>
        /// <returns>True iff the two serial numbers are equal.</returns>
        public static bool operator ==(SerialNumber32 value1, SerialNumber32 value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Returns true iff the two serial numbers are not equal.
        /// </summary>
        /// <param name="value1">First serial number to compare.</param>
        /// <param name="value2">Second serial number to compare.</param>
        /// <returns>True iff the two serial numbers are not equal.</returns>
        public static bool operator !=(SerialNumber32 value1, SerialNumber32 value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Returns true iff the first serial number is smaller than the second serial number.
        /// </summary>
        /// <param name="value1">First serial number to compare.</param>
        /// <param name="value2">Second serial number to compare.</param>
        /// <returns>True iff the first serial number is smaller than the second serial number..</returns>
        public static bool operator <(SerialNumber32 value1, SerialNumber32 value2)
        {
            return value1.CompareTo(value2) < 0;
        }

        /// <summary>
        /// Returns true iff the first serial number is greater than the second serial number.
        /// </summary>
        /// <param name="value1">First serial number to compare.</param>
        /// <param name="value2">Second serial number to compare.</param>
        /// <returns>True iff the first serial number is greater than the second serial number..</returns>
        public static bool operator >(SerialNumber32 value1, SerialNumber32 value2)
        {
            return value1.CompareTo(value2) > 0;
        }

        private readonly uint _value;
    }
}