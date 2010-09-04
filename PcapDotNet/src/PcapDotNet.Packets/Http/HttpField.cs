using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpField : IEquatable<HttpField>
    {
        public static HttpField CreateField(string fieldName, byte[] fieldValue)
        {
            switch (fieldName.ToLowerInvariant())
            {
                case HttpTransferEncodingField.NameLower:
                    return new HttpTransferEncodingField(fieldValue);
                case HttpContentLengthField.NameLower:
                    return new HttpContentLengthField(fieldValue);
                case HttpContentTypeField.NameLower:
                    return new HttpContentTypeField(fieldValue);

                default:
                    return new HttpField(fieldName, fieldValue.ToArray());
            }
        }

        public HttpField(string name, string value)
            : this(name, value, _defaultEncoding)
        {
        }

        public HttpField(string name, string value, Encoding encoding)
            : this(name, encoding.GetBytes(NormalizeValue(value)))
        {
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

        public HttpField(string name, IEnumerable<byte> value)
            : this(name, value.ToArray())
        {
        }

        public HttpField(string name, IList<byte> value)
            :this(name, value.AsReadOnly())
        {
        }

        public HttpField(string name, ReadOnlyCollection<byte> value)
        {
            Name = name;
            Value = value;
        }
//        public static HttpField Create(string name, byte[] value)
//        {
//            switch (name)
//            {
                // general-header
//                case "Cache-Control":
//                    return new HttpCommaSeparatedField(name, value);
//                case "Connection":
//                case "Date":
//                case "Pragma":
//                case "Trailer":
//                case "Transfer-Encoding":
//                case "Upgrade":
//                case "Via":
//                case "Warning":
//                    break;
//            }
//
//            return new HttpField(name);
//        }

        public string Name { get; private set; }
        public ReadOnlyCollection<byte> Value { get; private set; }
        public string ValueString
        {
            get
            {
                return _defaultEncoding.GetString(Value.ToArray());
            }
        }

        public int Length
        {
            get
            {
                return Name.Length + 2 + Value.Count + 2;
            }
        }

        public virtual bool Equals(HttpField other)
        {
            return other != null && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase) && Value.SequenceEqual(other.Value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpField);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, ValueString);
        }

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Name, Encoding.ASCII);
            buffer.Write(ref offset, AsciiBytes.Colon);
            buffer.Write(ref offset, AsciiBytes.Space);
            buffer.Write(ref offset, Value);
            buffer.WriteCarriageReturnLineFeed(ref offset);
        }

        private static readonly Encoding _defaultEncoding = Encoding.GetEncoding(28591);
    }
}