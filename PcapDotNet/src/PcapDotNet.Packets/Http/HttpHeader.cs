using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpHeader : IEnumerable<HttpField>, IEquatable<HttpHeader>
    {
        public static HttpHeader Empty { get { return _empty; } }

        public HttpHeader(IEnumerable<HttpField> fields)
        {
            _fields = fields.ToDictionary(field => field.Name, field => field, StringComparer.InvariantCultureIgnoreCase);
        }

        public HttpHeader(params HttpField[] fields)
            :this((IEnumerable<HttpField>)fields)
        {
        }

        public HttpTransferEncodingField TransferEncoding
        {
            get
            {
                return GetField<HttpTransferEncodingField>(HttpTransferEncodingField.Name);
            }
        }

        private T GetField<T>(string fieldName) where T : HttpField
        {
            HttpField field;
            if (!_fields.TryGetValue(fieldName, out field))
                return null;
            return (T)field;
        }

        public HttpContentLengthField ContentLength
        {
            get
            {
                return GetField<HttpContentLengthField>(HttpContentLengthField.Name);
            }
        }

        public HttpContentTypeField ContentType
        {
            get
            {
                return GetField<HttpContentTypeField>(HttpContentTypeField.Name);
            }
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

        public IEnumerator<HttpField> GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal HttpHeader(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> fields)
        {
            var mergedFields = new Dictionary<string, IEnumerable<byte>>(StringComparer.InvariantCultureIgnoreCase);
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
                    mergedFields[fieldName] = fieldValue.Concat(AsciiBytes.Comma).Concat(field.Value);
            }

            _fields = mergedFields.ToDictionary(field => field.Key, field => HttpField.CreateField(field.Key, field.Value.ToArray()), StringComparer.InvariantCultureIgnoreCase);
        }

        private static readonly HttpHeader _empty = new HttpHeader();
        private readonly Dictionary<string, HttpField> _fields = new Dictionary<string, HttpField>(StringComparer.InvariantCultureIgnoreCase);
    }
}
