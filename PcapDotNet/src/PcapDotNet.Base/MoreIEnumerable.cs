using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PcapDotNet.Base
{
    public static class MoreIEnumerable
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence, T value)
        {
            return sequence.Concat(new[] {value});
        }

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

        public static string SequenceToString<T>(this IEnumerable<T> sequence, string separator, string prefix)
        {
            return sequence.SequenceToString(separator, prefix, string.Empty);
        }

        public static string SequenceToString<T>(this IEnumerable<T> sequence, string separator)
        {
            return sequence.SequenceToString(separator, string.Empty);
        }

        public static string SequenceToString<T>(this IEnumerable<T> sequence)
        {
            return sequence.SequenceToString(string.Empty);
        }
    }
}