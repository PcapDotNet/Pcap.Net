using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// Represents HTTP header.
    /// The header is a container for HTTP fields.
    /// Insensitive to the case of HTTP field names.
    /// </summary>
    public sealed class HttpHeader : IEnumerable<HttpField>, IEquatable<HttpHeader>
    {
        /// <summary>
        /// An empty HTTP header.
        /// </summary>
        public static HttpHeader Empty { get { return _empty; } }

        /// <summary>
        /// Creates a header from an enumerable of fields.
        /// </summary>
        public HttpHeader(IEnumerable<HttpField> fields)
        {
            _fields = fields.ToDictionary(field => field.Name, field => field, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a header from an array of fields.
        /// </summary>
        public HttpHeader(params HttpField[] fields)
            :this((IEnumerable<HttpField>)fields)
        {
        }

        /// <summary>
        /// The number of bytes the header takes.
        /// </summary>
        public int BytesLength
        {
            get
            {
                return this.Sum(field => field.Length) + 2;
            }
        }

        /// <summary>
        /// Returns the field with the given field name or null if it doesn't exist.
        /// Case insensitive.
        /// </summary>
        /// <param name="fieldName">The case insensitive name of the field.</param>
        /// <returns>The field with the matching case insensitive name or null if it doesn't exist.</returns>
        public HttpField this[string fieldName]
        {
            get { return GetField<HttpField>(fieldName); }
        }

        /// <summary>
        /// The HTTP Transfer Encoding field if it exists (null otherwise).
        /// </summary>
        public HttpTransferEncodingField TransferEncoding
        {
            get
            {
                return GetField<HttpTransferEncodingField>(HttpTransferEncodingField.FieldName);
            }
        }

        /// <summary>
        /// The HTTP Content Length field if it exists (null otherwise).
        /// </summary>
        public HttpContentLengthField ContentLength
        {
            get
            {
                return GetField<HttpContentLengthField>(HttpContentLengthField.FieldName);
            }
        }

        /// <summary>
        /// The HTTP Content Type field if it exists (null otherwise).
        /// </summary>
        public HttpContentTypeField ContentType
        {
            get
            {
                return GetField<HttpContentTypeField>(HttpContentTypeField.FieldName);
            }
        }

        /// <summary>
        /// The HTTP Trailer field if it exists (null otherwise).
        /// </summary>
        public HttpTrailerField Trailer
        {
            get
            {
                return GetField<HttpTrailerField>(HttpTrailerField.FieldName);
            }
        }

        /// <summary>
        /// Two HTTP headers are equal if they have the same fields with the same values.
        /// </summary>
        public bool Equals(HttpHeader other)
        {
            return other != null &&
                   _fields.DictionaryEquals(other._fields);
        }

        /// <summary>
        /// Two HTTP headers are equal if they have the same fields with the same values.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as HttpHeader);
        }

        /// <summary>
        /// Xor of the hash codes of the fields.
        /// </summary>
        public override int GetHashCode()
        {
            return _fields.Select(pair => pair.Value).SequenceGetHashCode();
        }

        /// <summary>
        /// Returns a string of all the fields with endline separators.
        /// </summary>
        public override string ToString()
        {
            return this.SequenceToString("\r\n");
        }

        /// <summary>
        /// Enumerates over the HTTP fields.
        /// </summary>
        public IEnumerator<HttpField> GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Writes the HTTP header to the given buffer in the given offset.
        /// </summary>
        /// <param name="buffer">The buffer to write the header to.</param>
        /// <param name="offset">The offset in the given buffer to start writing the header.</param>
        public void Write(byte[] buffer, int offset)
        {
            Write(buffer, ref offset);
        }

        /// <summary>
        /// Writes the HTTP header to the given buffer in the given offset.
        /// Increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the header to.</param>
        /// <param name="offset">The offset in the given buffer to start writing the header. Incremented by the number of bytes written.</param>
        public void Write(byte[] buffer, ref int offset)
        {
            foreach (HttpField field in this)
                field.Write(buffer, ref offset);
            buffer.WriteCarriageReturnLinefeed(ref offset);
        }

        internal HttpHeader(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> fields)
        {
            var mergedFields = new Dictionary<string, IEnumerable<byte>>(StringComparer.OrdinalIgnoreCase);
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

            _fields = mergedFields.ToDictionary(field => field.Key, field => HttpField.CreateField(field.Key, field.Value.ToArray()), StringComparer.OrdinalIgnoreCase);
        }

        private T GetField<T>(string fieldName) where T : HttpField
        {
            HttpField field;
            if (!_fields.TryGetValue(fieldName, out field))
                return null;
            return (T)field;
        }

        private static readonly HttpHeader _empty = new HttpHeader();
        private readonly Dictionary<string, HttpField> _fields = new Dictionary<string, HttpField>(StringComparer.OrdinalIgnoreCase);
    }
}
