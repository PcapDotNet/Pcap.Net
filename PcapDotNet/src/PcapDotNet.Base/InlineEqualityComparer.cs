using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    public class InlineEqualityComparer<T> : IEqualityComparer<T>
    {
        public InlineEqualityComparer(Func<T, T, bool> equals, Func<T,int> getHashCode)
        {
            EqualsFunc = equals;
            GetHashCodeFunc = getHashCode;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return EqualsFunc(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return GetHashCodeFunc(obj);
        }

        private Func<T, T, bool> EqualsFunc { get; set; }
        private Func<T, int> GetHashCodeFunc { get; set; }
    }
}