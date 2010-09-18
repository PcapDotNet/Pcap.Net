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

        /// <summary>
        /// Returns an enumerable of all the elements in the given list starting in a specific offset and taking no more than a specific count.
        /// </summary>
        /// <typeparam name="T">The type of an element in the collection.</typeparam>
        /// <param name="list">The list to take the elements from.</param>
        /// <param name="offset">The offset of the first element to take.</param>
        /// <param name="count">The maximum number of elements to take.</param>
        /// <returns>An enumerable of all the elements in the given list starting in a specific offset and taking no more than a specific count.</returns>
        public static IEnumerable<T> Range<T>(this IList<T> list, int offset, int count)
        {
            int length = Math.Min(offset + count, list.Count);
            for (int i = offset; i < length; ++i)
                yield return list[i];
        }
    }
}