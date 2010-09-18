using System.Collections.Generic;

namespace PcapDotNet.Base
{
    public static class IDictionaryExtensions
    {
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2, IEqualityComparer<TValue> valueComparer)
        {
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

        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            return dictionary1.DictionaryEquals(dictionary2, EqualityComparer<TValue>.Default);
        }
    }
}