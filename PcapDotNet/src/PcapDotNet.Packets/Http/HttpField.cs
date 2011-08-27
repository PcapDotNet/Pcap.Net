using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// An HTTP field with a name and value.
    /// </summary>
    public class HttpField : IEquatable<HttpField>
    {
        /// <summary>
        /// Creates a field according to the field name and value.
        /// </summary>
        /// <param name="fieldName">The name of the field to create.</param>
        /// <param name="fieldValue">The bytes value of the field to create.</param>
        /// <returns>The constructed HTTP field.</returns>
        public static HttpField CreateField(string fieldName, byte[] fieldValue)
        {
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            switch (fieldName.ToUpperInvariant())
            {
                case HttpTransferEncodingField.FieldNameUpper:
                    return new HttpTransferEncodingField(fieldValue);
                case HttpContentLengthField.FieldNameUpper:
                    return new HttpContentLengthField(fieldValue);
                case HttpContentTypeField.FieldNameUpper:
                    return new HttpContentTypeField(fieldValue);

                default:
                    return new HttpField(fieldName, fieldValue.ToArray());
            }
        }

        /// <summary>
        /// Creates a field according to the field name and encoded string value.
        /// </summary>
        /// <param name="fieldName">The name of the field to create.</param>
        /// <param name="fieldValue">The value of the field to create encoded in the given encoding.</param>
        /// <param name="fieldValueEncoding">The encoding that encodes the given field value.</param>
        /// <returns>The constructed HTTP field.</returns>
        public static HttpField CreateField(string fieldName, string fieldValue, Encoding fieldValueEncoding)
        {
            if (fieldValueEncoding == null) 
                throw new ArgumentNullException("fieldValueEncoding");

            return CreateField(fieldName, fieldValueEncoding.GetBytes(NormalizeValue(fieldValue)));
        }

        /// <summary>
        /// Creates a field according to the field name and encoded string value.
        /// </summary>
        /// <param name="fieldName">The name of the field to create.</param>
        /// <param name="fieldValue">The value of the field to create encoded in ISO-8859-1 encoding.</param>
        /// <returns>The constructed HTTP field.</returns>
        public static HttpField CreateField(string fieldName, string fieldValue)
        {
            return CreateField(fieldName, fieldValue, _defaultEncoding);
        }

        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The Value of the field.
        /// </summary>
        public ReadOnlyCollection<byte> Value { get; private set; }

        /// <summary>
        /// The Value of the field as a string using ISO-8859-1 encoding.
        /// </summary>
        public string ValueString
        {
            get
            {
                return _defaultEncoding.GetString(Value.ToArray());
            }
        }

        /// <summary>
        /// The number of bytes the field will take in the HTTP protocol.
        /// </summary>
        public int Length
        {
            get
            {
                return Name.Length + 2 + Value.Count + 2;
            }
        }

        /// <summary>
        /// True iff the two HTTP fields are of equal value.
        /// Two fields are equal iff they have the same name (case insensitive) and the same bytes value.
        /// </summary>
        public virtual bool Equals(HttpField other)
        {
            return other != null && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) && Value.SequenceEqual(other.Value);
        }

        /// <summary>
        /// True iff the two HTTP fields are of equal value.
        /// Two fields are equal iff they have the same name (case insensitive) and the same bytes value.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as HttpField);
        }

        /// <summary>
        /// Returns a hash code of this field according to the name and value.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Name.ToUpperInvariant().GetHashCode() ^ Value.BytesSequenceGetHashCode();
        }

        /// <summary>
        /// A string representing the field similar to how it would like in the HTTP protocol.
        /// </summary>
        /// <returns></returns>
        public sealed override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", Name, ValueString);
        }

        internal HttpField(string name, string value)
            : this(name, value, _defaultEncoding)
        {
        }

        internal HttpField(string name, string value, Encoding encoding)
            : this(name, encoding == null ? null : encoding.GetBytes(NormalizeValue(value)))
        {
        }

        internal HttpField(string name, IList<byte> value)
            : this(name, value.AsReadOnly())
        {
        }

        internal HttpField(string name, ReadOnlyCollection<byte> value)
        {
            Name = name;
            Value = value;
        }
        
        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Name, Encoding.ASCII);
            buffer.Write(ref offset, AsciiBytes.Colon);
            buffer.Write(ref offset, AsciiBytes.Space);
            buffer.Write(ref offset, Value);
            buffer.WriteCarriageReturnLinefeed(ref offset);
        }

        private static string NormalizeValue(string value)
        {
            StringBuilder stringBuilder = new StringBuilder(value.Length);
            int offset = 0;
            while (offset != value.Length)
            {
                if (value[offset] == '"')
                {
                    int start = offset;
                    ++offset;
                    while (offset != value.Length && value[offset] != '"')
                    {
                        if (value[offset] == '\\' && offset != value.Length - 1)
                            ++offset;
                        ++offset;
                    }
                    if (value[offset] == '"')
                        ++offset;
                    stringBuilder.Append(value.Substring(start, offset - start));
                }
                else if (value[offset] == '\t' || value[offset] == ' ' || value[offset] == '\r' || value[offset] == '\n')
                {
                    stringBuilder.Append(' ');
                    ++offset;
                    while (offset != value.Length && (value[offset] == '\t' || value[offset] == ' ' || value[offset] == '\r' || value[offset] == '\n'))
                        ++offset;
                }
                else
                {
                    stringBuilder.Append(value[offset]);
                    ++offset;
                }
            }
            return stringBuilder.ToString();
        }

        private static readonly Encoding _defaultEncoding = EncodingExtensions.Iso88591;
    }
}