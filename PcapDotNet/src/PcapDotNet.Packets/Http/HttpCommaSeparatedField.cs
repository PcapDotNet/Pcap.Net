using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using IListExtensions = PcapDotNet.Base.IListExtensions;

namespace PcapDotNet.Packets.Http
{
    public class HttpCommaSeparatedField : HttpField, IEnumerable<HttpCommaSeparatedInnerField>, IEquatable<HttpCommaSeparatedField>
    {

        internal HttpCommaSeparatedField(string name, params HttpCommaSeparatedInnerField[] value)
            :this(name, (IEnumerable<HttpCommaSeparatedInnerField>)value)
        {
        }

        internal HttpCommaSeparatedField(string name, IEnumerable<HttpCommaSeparatedInnerField> value = null)
            :base(name)
        {
            _innerFields = value == null ? null : value.ToDictionary(innerField => innerField.Name, innerField => innerField);
        }

        public bool Equals(HttpCommaSeparatedField other)
        {
            return base.Equals(other) && _innerFields.DictionaryEquals(other._innerFields);
        }

        public override string ToString()
        {
            return base.ToString() + (_innerFields == null ? string.Empty : _innerFields.Values.SequenceToString(","));
        }

        internal HttpCommaSeparatedField(string name, IEnumerable<byte> value)
            :base(name)
        {
            var innerFields = new Dictionary<string, List<ReadOnlyCollection<byte>>>();
            foreach (var commaValue in CommaSeparate(value))
            {
                string innerFieldName = commaValue.Key;

                List<ReadOnlyCollection<byte>> innerFieldValues;
                if (!innerFields.TryGetValue(innerFieldName, out innerFieldValues))
                {
                    innerFieldValues = new List<ReadOnlyCollection<byte>>();
                    innerFields.Add(innerFieldName, innerFieldValues);
                }
                innerFieldValues.Add(commaValue.Value == null ? null : commaValue.Value.ToArray().AsReadOnly());
            }

            _innerFields = innerFields.ToDictionary(field => field.Key, field => new HttpCommaSeparatedInnerField(field.Key, field.Value));
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<byte>>> CommaSeparate(IEnumerable<byte> buffer)
        {
            do
            {
                string key = Encoding.ASCII.GetString(buffer.TakeWhile(b => b.IsToken()).ToArray());
                if (key.Length == 0)
                    yield break;

                buffer = buffer.Skip(key.Length);
                if (!buffer.Any())
                {
                    yield return new KeyValuePair<string, IEnumerable<byte>>(key, null);
                    continue;
                }

                switch (buffer.First())
                {
                    case AsciiBytes.EqualsSign:
                        buffer = buffer.Skip(1);
                        IEnumerable<byte> value = buffer;
                        int count = 0;
                        if (buffer.Any())
                        {
                            byte valueByte = buffer.First();
                            if (valueByte.IsToken()) // Token value
                                count = 1 + buffer.Skip(1).TakeWhile(b => b.IsToken()).Count();
                            else if (!TryExtractQuotedString(buffer, out count)) // Not Quoted value - Illegal value
                                yield break;
                            buffer = buffer.Skip(count);
                        }
                        yield return new KeyValuePair<string, IEnumerable<byte>>(key, value.Take(count));

                        if (!buffer.Any())
                            yield break;
                        switch (buffer.First())
                        {
                            case AsciiBytes.Comma:
                            case AsciiBytes.Space:
                                buffer = buffer.Skip(1).SkipWhile(b => b == AsciiBytes.Space || b == AsciiBytes.Comma);
                                break;

                            default:
                                yield break;
                        }
                        break;

                    case AsciiBytes.Comma:
                    case AsciiBytes.Space:
                        yield return new KeyValuePair<string, IEnumerable<byte>>(key, null);
                        buffer = buffer.Skip(1).SkipWhile(b => b == AsciiBytes.Space || b == AsciiBytes.Comma);
                        break;

                    default:
                        yield break;
                }
            } while (buffer.Any());
        }

        private static bool TryExtractQuotedString(IEnumerable<byte> buffer, out int count)
        {
            count = 0;
            if (!buffer.Any() || buffer.First() != AsciiBytes.DoubleQuotationMark)
                return false;

            ++count;
            buffer = buffer.Skip(1);

            while (buffer.Any())
            {
                ++count;
                byte current = buffer.First();

                if (current == AsciiBytes.DoubleQuotationMark)
                    return true;

                buffer = buffer.Skip(1);
                if (current == AsciiBytes.BackSlash && buffer.Any())
                {
                    current = buffer.First();
                    if (current.IsChar())
                    {
                        buffer = buffer.Skip(1);
                        ++count;
                    }
                }
            }

            return false;
        }

        public IEnumerator<HttpCommaSeparatedInnerField> GetEnumerator()
        {
            return _innerFields.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Dictionary<string, HttpCommaSeparatedInnerField> _innerFields;
    }
}