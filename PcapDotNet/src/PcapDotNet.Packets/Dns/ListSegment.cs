using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets.Dns
{
    internal class ListSegment<T> : IEnumerable<T>
    {
        public ListSegment(IList<T> data, int startIndex, int count)
        {
            _data = data;
            _startIndex = startIndex;
            Count = count;
        }

        public ListSegment(IList<T> data, int startIndex)
            : this(data, startIndex, data.Count - startIndex)
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i != Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get; private set; }


        public T this[int index]
        {
            get { return _data[_startIndex + index]; }
        }

        private readonly IList<T> _data;
        private readonly int _startIndex;
    }
}