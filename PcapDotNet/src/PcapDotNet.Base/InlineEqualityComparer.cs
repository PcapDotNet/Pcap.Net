using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    /// <summary>
    /// An equality comparer that is implemented using the given equals and getHashCode functions.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class InlineEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Constructs the comparer using the given equals and getHashCode functions.
        /// </summary>
        /// <param name="equals">The function to use to implement Equals().</param>
        /// <param name="getHashCode">The function to use to implement GetHashCode().</param>
        public InlineEqualityComparer(Func<T, T, bool> equals, Func<T,int> getHashCode)
        {
            EqualsFunc = equals;
            GetHashCodeFunc = getHashCode;
        }

        /// <summary>
        /// Determines whether the specified objects are equal using the equals function that was given in the constructor.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <see cref="T:System.Object"/> to compare.</param><param name="y">The second object of type <see cref="T:System.Object"/> to compare.</param>
        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return EqualsFunc(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object using the getHashCode function that was given in the constructor.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return GetHashCodeFunc(obj);
        }

        private Func<T, T, bool> EqualsFunc { get; set; }
        private Func<T, int> GetHashCodeFunc { get; set; }
    }
}