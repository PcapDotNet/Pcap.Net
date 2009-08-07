using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for IEnumerable of type T.
    /// </summary>
    public static class MoreIEnumerable
    {
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
        /// <returns>A string of all the elements.</returns>
        public static string SequenceToString<T>(this IEnumerable<T> sequence)
        {
            return sequence.SequenceToString(string.Empty);
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
    }
}