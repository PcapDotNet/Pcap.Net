using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded 
    /// with chunked transfer-coding.
    /// <pre>
    /// Trailer  = "Trailer" ":" 1#field-name
    /// </pre>
    /// An HTTP/1.1 message should include a Trailer header field in a message using chunked transfer-coding with a non-empty trailer.
    /// Doing so allows the recipient to know which header fields to expect in the trailer.
    /// 
    /// If no Trailer header field is present, the trailer should not include any header fields.
    /// 
    /// Message header fields listed in the Trailer header field must not include the following header fields:
    /// * Transfer-Encoding.
    /// * Content-Length.
    /// * Trailer.
    /// </summary>
    public sealed class HttpTrailerField : HttpField, IEquatable<HttpTrailerField>
    {
        /// <summary>
        /// The field name.
        /// </summary>
        public const string FieldName = "Trailer";

        /// <summary>
        /// The field name in uppercase.
        /// </summary>
        public const string FieldNameUpper = "TRAILER";

        /// <summary>
        /// Creates a Trailer Field according to the given field names.
        /// </summary>
        /// <param name="fieldsNames">The names of the fields that should be encoded in the chunked body trailer.</param>
        public HttpTrailerField(IEnumerable<string> fieldsNames)
            : base(FieldName, fieldsNames.SequenceToString(','))
        {
            SetFieldNames(fieldsNames);
        }

        /// <summary>
        /// The names of the fields that should be encoded in the chunked body trailer.
        /// </summary>
        public ReadOnlyCollection<string> FieldsNames { get { return _fieldNames; } }

        /// <summary>
        /// True iff the two fields are equal.
        /// Two trailer fields are equal iff they have the fields names in the same order.
        /// </summary>
        public bool Equals(HttpTrailerField other)
        {
            return other != null &&
                   FieldsNames.SequenceEqual(other.FieldsNames);
        }

        /// <summary>
        /// True iff the two fields are equal.
        /// Two trailer fields are equal iff they have the fields names in the same order.
        /// </summary>
        public override bool Equals(HttpField other)
        {
            return Equals(other as HttpTrailerField);
        }

        internal HttpTrailerField(byte[] fieldValue)
            : base(FieldName, fieldValue)
        {
            string fieldValueString = HttpRegex.GetString(fieldValue);
            Match match = _regex.Match(fieldValueString);
            if (!match.Success)
                return;

            SetFieldNames(match.GroupCapturesValues(FieldNameGroupName));
        }

        private void SetFieldNames(IEnumerable<string> fieldNames)
        {
            _fieldNames = fieldNames.Select(name => name.ToUpperInvariant()).ToArray().AsReadOnly();
        }

        private const string FieldNameGroupName = "FieldName";

        private static readonly Regex _regex =
            HttpRegex.MatchEntire(HttpRegex.CommaSeparatedRegex(HttpRegex.Capture(HttpRegex.Token, FieldNameGroupName), 1));

        private ReadOnlyCollection<string> _fieldNames;
    }
}