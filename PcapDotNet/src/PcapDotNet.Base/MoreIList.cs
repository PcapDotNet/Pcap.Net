using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Base
{
    public static class MoreIList
    {
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
    }
}