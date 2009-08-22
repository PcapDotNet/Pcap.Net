using System;
using System.Collections.Generic;

namespace PcapDotNet.Base
{
    public static class MoreType
    {
        public static IEnumerable<T> GetEnumValues<T>(this Type type)
        {
            return (IEnumerable<T>)Enum.GetValues(type);
        }
    }
}