using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for Type.
    /// </summary>
    public static class MoreType
    {
        /// <summary>
        /// Returns all the possible values for the given enum type.
        /// </summary>
        public static IEnumerable<T> GetEnumValues<T>(this Type type)
        {
            return (IEnumerable<T>)Enum.GetValues(type);
        }
    }
}