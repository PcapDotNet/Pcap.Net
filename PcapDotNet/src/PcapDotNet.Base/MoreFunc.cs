using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for Func of type T.
    /// </summary>
    public static class MoreFunc
    {
        public static T[] GenerateArray<T>(this Func<T> generator, int size)
        {
            T[] array = new T[size];
            for (int i = 0; i != size; ++i)
                array[i] = generator.Invoke();
            return array;
        }
    }
}