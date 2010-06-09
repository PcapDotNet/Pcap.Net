using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for IList of type T.
    /// </summary>
// ReSharper disable InconsistentNaming
    public static class IListExtensions
// ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// Wraps a list with a ReadOnlyCollection.
        /// </summary>
        /// <typeparam name="T">The type of an element in the collection.</typeparam>
        /// <param name="list">The list to wrap in a ReadOnlyCollection.</param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }

        public static IEnumerable<T> Range<T>(this IList<T> list, int offset, int count)
        {
            int length = Math.Min(offset + count, list.Count);
            for (int i = offset; i < length; ++i)
                yield return list[i];
        }
    }
}