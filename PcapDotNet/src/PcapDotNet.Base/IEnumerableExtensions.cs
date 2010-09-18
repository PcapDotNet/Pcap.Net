using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for IEnumerable of type T.
    /// </summary>
// ReSharper disable InconsistentNaming
    public static class IEnumerableExtensions
// ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// True iff the sequence has no elements.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null) 
                throw new ArgumentNullException("sequence");

            return !sequence.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Concatenates a sequence with more values.
        /// </summary>
        /// <typeparam name="T">The type of an element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to concatenate.</param>
        /// <param name="values">The values that will be concatenated with the sequence.</param>
        /// <returns>A sequence containing all the original sequence elements cocatenated with values elements.</returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence, params T[] values)
        {
            return sequence.Concat((IEnumerable<T>)values);
        }

        /// <summary>
        /// Converts a sequence to a string by converting each element to a string.
        /// </summary>
        /// <typeparam name="T">The type of an element in the sequence.</typeparam>
        /// <param name="sequence">The sequence with the elements to translate to string.</param>
        /// <param name="separator">A separator between the elements.</param>
        /// <param name="prefix">Prefix to the entire string.</param>
        /// <param name="suffix">Suffix to the entire string.</param>
        /// <returns>A string of all the elements.</returns>
        public static string SequenceToString<T>(this IEnumerable<T> sequence, string separator, string prefix, string suffix)
        {
            if (sequence == null) 
                throw new ArgumentNullException("sequence");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(prefix);
            bool isFirst = true;
            foreach (T value in sequence)
            {
                if (isFirst)
                    isFirst = false;
                else
                    stringBuilder.Append(separator);
                stringBuilder.Append(value);
            }
            stringBuilder.Append(suffix);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts a sequence to a string by converting each element to a string.
        /// </summary>
        /// <typeparam name="T">The type of an element in the sequence.</typeparam>
        /// <param name="sequence">The sequence with the elements to translate to string.</param>
        /// <param name="separator">A separator between the elements.</param>
        /// <param name="prefix">Prefix to the entire string.</param>
        /// <returns>A string of all the elements.</returns>
        public static string SequenceToString<T>(this IEnumerable<T> sequence, string separator, string prefix)
        {
            return sequence.SequenceToString(separator, prefix, string.Empty);
        }

        /// <summary>
        /// Converts a sequence to a string by converting each element to a string.
        /// </summary>
        /// <typeparam name="T">The type of an element in the sequence.</typeparam>
        /// <param name="sequence">The sequence with the elements to translate to string.</param>
        /// <param name="separator">A separator between the elements.</param>
        /// <returns>A string of all the elements.</returns>
        public static string SequenceToString<T>(this IEnumerable<T> sequence, string separator)
        {
            return sequence.SequenceToString(separator, string.Empty);
        }

        /// <summary>
        /// Converts a sequence to a string by converting each element to a string.
        /// </summary>
        /// <typeparam name="T">The type of an element in the sequence.</typeparam>
        /// <param name="sequence">The sequence with the elements to translate to string.</param>
        /// <param name="separator">A separator between the elements.</param>
        /// <returns>A string of all the elements.</returns>
        public static string SequenceToString<T>(this IEnumerable<T> sequence, char separator)
        {
            return sequence.SequenceToString(separator.ToString());
        }

        /// <summary>
        /// Converts a sequence to a string by converting each element to a string.
        /// </summary>
        /// <typeparam name="T">The type of an element in the sequence.</typeparam>
        /// <param name="sequence">The sequence with the elements to translate to string.</param>
        /// <returns>A string of all the elements.</returns>
        public static string SequenceToString<T>(this IEnumerable<T> sequence)
        {
            return sequence.SequenceToString(string.Empty);
        }

        /// <summary>
        /// Returns a string by converting all the bytes to a hexadecimal string.
        /// </summary>
        /// <param name="sequence">The bytes to convert to a string.</param>
        /// <param name="separator">The string to put between every two bytes.</param>
        /// <returns>The string resulted by converting all the bytes to hexadecimal strings and putting the separator between them.</returns>
        public static string BytesSequenceToHexadecimalString(this IEnumerable<byte> sequence, string separator)
        {
            return sequence.Aggregate(string.Empty,
                                      delegate(string result, byte value)
                                      {
                                          if (!string.IsNullOrEmpty(result))
                                              result = result + separator;
                                          return result + string.Format(CultureInfo.InvariantCulture, "{0:x2}", value);
                                      });
        }

        /// <summary>
        /// Returns a string by converting all the bytes to a hexadecimal string.
        /// </summary>
        /// <param name="sequence">The bytes to convert to a string.</param>
        /// <returns>The string resulted by converting all the bytes to hexadecimal strings.</returns>
        public static string BytesSequenceToHexadecimalString(this IEnumerable<byte> sequence)
        {
            return sequence.BytesSequenceToHexadecimalString(string.Empty);
        }

        /// <summary>
        /// Creates a hash code by xoring the hash codes of the elements in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence with the elements to create the hash code for.</param>
        /// <returns>The hash code created by xoring all the hash codes of the elements in the sequence.</returns>
        public static int SequenceGetHashCode<T>(this IEnumerable<T> sequence)
        {
            return sequence.Aggregate(0, (valueSoFar, element) => valueSoFar ^ element.GetHashCode());
        }

        /// <summary>
        /// Returns a hash code by xoring all the bytes.
        /// Each byte is xored with the next 8 bits of the integer.
        /// </summary>
        /// <param name="sequence">The bytes to xor.</param>
        /// <returns>The hash code resulted by xoring all the bytes.</returns>
        public static int BytesSequenceGetHashCode(this IEnumerable<byte> sequence)
        {
            int i = 0;
            return sequence.Aggregate(0, (value, b) => value ^ (b << (8 * (i++ % 4))));
        }

        /// <summary>
        /// Returns a hash code by xoring all the ushorts.
        /// Each ushort is xored with the next 16 bits of the integer.
        /// </summary>
        /// <param name="sequence">The ushorts to xor.</param>
        /// <returns>The hash code resulted by xoring all the ushorts.</returns>
        public static int UShortsSequenceGetHashCode(this IEnumerable<ushort> sequence)
        {
            int i = 0;
            return sequence.Aggregate(0, (value, b) => value ^ (b << (16 * (i++ % 2))));
        }

        /// <summary>
        /// Counts the number of types the given value is contained in the given sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence to look for the value in.</param>
        /// <param name="value">The value to look for in the sequence.</param>
        /// <returns>The number of types the given value is contained in the given sequence.</returns>
        public static int Count<T>(this IEnumerable<T> sequence, T value)
        {
            return sequence.Count(element => element.Equals(value));
        }
    }
}