using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    public static class IDictionaryExtensions
    {
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2, IEqualityComparer<TValue> valueComparer)
        {
            if (dictionary1 == null || dictionary2 == null)
                return dictionary1 == null && dictionary2 == null;

            if (dictionary1.Count != dictionary2.Count)
                return false;

            foreach (var pair in dictionary1)
            {
                TValue otherValue;
                if (!dictionary2.TryGetValue(pair.Key, out otherValue))
                    return false;
                if (!valueComparer.Equals(pair.Value, otherValue))
                {
                    if (otherValue is string)
                    {
                        string otherString = otherValue as string;
                        string thisString = pair.Value as string;
                        for (int i = 0; i != otherString.Length; ++i)
                        {
                            if (!thisString[i].Equals(otherString[i]))
                            {
                                Console.WriteLine("a");
                            }
                            else
                            {
                                Console.WriteLine("b");
                            }
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            return dictionary1.DictionaryEquals(dictionary2, EqualityComparer<TValue>.Default);
        }
    }
}