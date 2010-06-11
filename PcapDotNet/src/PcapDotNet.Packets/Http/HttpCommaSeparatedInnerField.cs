using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class SequenceEqualityComparator<T> : IEqualityComparer<IEnumerable<T>>
    {
        public static SequenceEqualityComparator<T> Instance
        {
            get { return _instance;}
        }

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            return obj.SequenceGetHashCode();
        }

        private static readonly SequenceEqualityComparator<T> _instance = new SequenceEqualityComparator<T>();
    }

    public class HttpCommaSeparatedInnerField : IEquatable<HttpCommaSeparatedInnerField>
    {
        public HttpCommaSeparatedInnerField(string name)
            :this(name,null as ReadOnlyCollection<byte>)
        {
        }

        public HttpCommaSeparatedInnerField(string name, ReadOnlyCollection<ReadOnlyCollection<byte>> values)
        {
            Name = name;
            Values = values;
        }

        public HttpCommaSeparatedInnerField(string name, IList<ReadOnlyCollection<byte>> values)
            :this(name, values.AsReadOnly())
        {
        }

        public HttpCommaSeparatedInnerField(string name, params ReadOnlyCollection<byte>[] values)
            :this(name, (IList<ReadOnlyCollection<byte>>)values)
        {
        }

        public bool Equals(HttpCommaSeparatedInnerField other)
        {
            return other != null && Name == other.Name && Values.SequenceEqual(other.Values, SequenceEqualityComparator<byte>.Instance);
        }

        public override string ToString()
        {
            return Values.Select(value => Name + (value == null ? string.Empty : "=" + Encoding.ASCII.GetString(value.ToArray()))).SequenceToString(",");
        }

        public string Name { get; private set; }
        public ReadOnlyCollection<ReadOnlyCollection<byte>> Values { get; private set;}
    }
}