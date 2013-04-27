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
        public static bool AreAllCharactersInRange(this string value, char minValue, char maxValue)
        {
            return value.All(c => c >= minValue && c <= maxValue);
        }
    }
}