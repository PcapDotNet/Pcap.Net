using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for IDictionary&lt;TKey,TValue> interface.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Tests for equality between dictionaries.
        /// Two dictionaries are equal if they have the same pairs.
        /// Keys are compared using Equals() and values are compared using the given comparator.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the value of the dictionary.</typeparam>
        /// <param name="dictionary1">The first dictionary to compare.</param>
        /// <param name="dictionary2">The second dictionary to compare.</param>
        /// <param name="valueComparer">The comparator to check for values equality.</param>
        /// <returns>True iff the dictionaries are equal.</returns>
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2, IEqualityComparer<TValue> valueComparer)
        {
            if (valueComparer == null)
                throw new ArgumentNullException("valueComparer");

            if (ReferenceEquals(dictionary1, dictionary2))
                return true;

            if (dictionary1 == null || dictionary2 == null)
                return false;

            if (dictionary1.Count != dictionary2.Count)
                return false;

            foreach (var pair in dictionary1)
            {
                TValue otherValue;
                if (!dictionary2.TryGetValue(pair.Key, out otherValue))
                    return false;
                if (!valueComparer.Equals(pair.Value, otherValue))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests for equality between dictionaries.
        /// Two dictionaries are equal if they have the same pairs.
        /// Keys are compared using Equals() and values are compared using the default EqualityComparer.
        /// </summary>
        /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the value of the dictionary.</typeparam>
        /// <param name="dictionary1">The first dictionary to compare.</param>
        /// <param name="dictionary2">The second dictionary to compare.</param>
        /// <returns>True iff the dictionaries are equal.</returns>
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            return dictionary1.DictionaryEquals(dictionary2, EqualityComparer<TValue>.Default);
        }
    }
}