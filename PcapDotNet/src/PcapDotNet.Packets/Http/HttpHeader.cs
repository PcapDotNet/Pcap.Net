using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpHeader : IEnumerable<HttpField>, IEquatable<HttpHeader>
    {
        public HttpHeader(IEnumerable<HttpField> fields)
        {
            _fields = fields.ToDictionary(field => field.Name, field => field);
        }

        public HttpHeader(params HttpField[] fields)
            :this((IEnumerable<HttpField>)fields)
        {
        }

        public bool Equals(HttpHeader other)
        {
            return other != null &&
                   _fields.DictionaryEquals(other._fields);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpHeader);
        }

        public override string ToString()
        {
            return this.SequenceToString("\r\n");
        }

        internal HttpHeader(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> fields)
        {
            var mergedFields = new Dictionary<string, IEnumerable<byte>>();
            foreach (var field in fields)
            {
                string fieldName = field.Key;
                IEnumerable<byte> fieldValue;
                if (!mergedFields.TryGetValue(fieldName, out fieldValue))
                {
                    fieldValue = field.Value;
                    mergedFields.Add(fieldName, fieldValue);
                }
                else
                    mergedFields[fieldName] = fieldValue.Concat(AsciiBytes.Space).Concat(fieldValue);
            }

            _fields = mergedFields.ToDictionary(field => field.Key, field => HttpField.Create(field.Key, field.Value));
        }

        public IEnumerator<HttpField> GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Dictionary<string, HttpField> _fields = new Dictionary<string, HttpField>();
    }
}
