using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    public class InlineEqualityComparer<T> : IEqualityComparer<T>
    {
        public InlineEqualityComparer(Func<T, T, bool> equals, Func<T,int> getHashCode)
        {
            Equals = equals;
            GetHashCode = getHashCode;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return Equals(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return GetHashCode(obj);
        }

        private Func<T, T, bool> Equals { get; set; }
        private Func<T, int> GetHashCode { get; set; }
    }
}