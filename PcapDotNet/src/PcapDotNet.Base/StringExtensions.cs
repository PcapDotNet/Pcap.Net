using System;
using System.Globalization;
using System.Linq;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for String.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns whether all of the chars in the given string are in the [minValue, maxValue] range.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <param name="minValue">The first char in the chars range.</param>
        /// <param name="maxValue">The last char in the chars range.</param>
        /// <returns>True iff all of the string's chars are in the given range.</returns>
        public static bool AreAllCharactersInRange(this string value, char minValue, char maxValue)
        {
            return value.All(c => c >= minValue && c <= maxValue);
        }
    }
}