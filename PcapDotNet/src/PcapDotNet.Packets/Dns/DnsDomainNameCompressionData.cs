using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets.Dns
{
    public class ListSegment<T> : IList<T>
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

        public void Add(T item)
        {
            throw new NotSupportedException("ListSegment<T> is read-only");
        }

        public void Clear()
        {
            throw new NotSupportedException("ListSegment<T> is read-only");
        }

        public bool Contains(T item)
        {
            return Enumerable.Contains(this, item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T value in this)
                array[arrayIndex++] = value;
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("ListSegment<T> is read-only");
        }

        public int Count { get; private set; }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            if (ReferenceEquals(item, null))
            {
                for (int i = 0; i != Count; ++i)
                {
                    if (ReferenceEquals(this[i], null))
                        return i;
                }
                return -1;
            }

            for (int i = 0; i != Count; ++i)
            {
                if (item.Equals(this[i]))
                    return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("ListSegment<T> is read-only");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("ListSegment<T> is read-only");
        }

        public T this[int index]
        {
            get { return _data[_startIndex + index]; }
            set { throw new NotSupportedException("ListSegment<T> is read-only"); }
        }

        public ListSegment<T> SubSegment(int startIndex, int count)
        {
            return new ListSegment<T>(_data, _startIndex + startIndex, count);
        }

        public ListSegment<T> SubSegment(int startIndex)
        {
            return SubSegment(startIndex, Count - startIndex);
        }

        private readonly IList<T> _data;
        private readonly int _startIndex;
    }

    internal class DnsDomainNameCompressionData
    {
        private class LabelNode
        {
            public int? DnsOffset { get; private set; }

            public bool IsAvailable(ListSegment<DataSegment> labels)
            {
                if (labels.Count == 0)
                    return DnsOffset != null;
                LabelNode nextNode;
                if (!_nextLabels.TryGetValue(labels[0], out nextNode))
                    return false;
                return nextNode.IsAvailable(labels.SubSegment(1));
            }

            public void AddLabels(ListSegment<DataSegment> labels, int dnsOffset)
            {
                if (labels.Count == 0)
                {
                    DnsOffset = dnsOffset;
                    return;
                }
                _nextLabels[labels[0]].AddLabels(labels.SubSegment(1), dnsOffset);
            }

            private readonly Dictionary<DataSegment, LabelNode> _nextLabels = new Dictionary<DataSegment, LabelNode>();
        }

        public DnsDomainNameCompressionData(DnsDomainNameCompressionMode domainNameCompressionMode)
        {
            DomainNameCompressionMode = domainNameCompressionMode;
        }

        public DnsDomainNameCompressionMode DomainNameCompressionMode { get; private set; }

        public bool IsAvailable(ListSegment<DataSegment> labels)
        {
            switch (DomainNameCompressionMode)
            {
                case DnsDomainNameCompressionMode.All:
                    return _root.IsAvailable(labels);
                case DnsDomainNameCompressionMode.Nothing:
                    return false;
                default:
                    throw new InvalidOperationException(string.Format("Invalid DomainNameCompressionMode {0}", DomainNameCompressionMode));
            }
        }

        public void AddCompressionData(ListSegment<DataSegment> labels, int dnsOffset)
        {
            switch (DomainNameCompressionMode)
            {
                case DnsDomainNameCompressionMode.All:
                    _root.AddLabels(labels, dnsOffset);
                    return;
                case DnsDomainNameCompressionMode.Nothing:
                    return;
                default:
                    throw new InvalidOperationException(string.Format("Invalid DomainNameCompressionMode {0}", DomainNameCompressionMode));
            }
        }

        private readonly LabelNode _root = new LabelNode();
    }
}