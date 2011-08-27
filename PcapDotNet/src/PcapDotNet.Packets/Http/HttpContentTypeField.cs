using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// The Content-Type entity-header field indicates the media type of the entity-body sent to the recipient or, in the case of the HEAD method,
    /// the media type that would have been sent had the request been a GET.
    /// 
    /// <pre>
    /// Content-Type   = "Content-Type" ":" media-type
    /// </pre>
    /// 
    /// An example of the field is
    /// <pre>
    /// Content-Type: text/html; charset=ISO-8859-4
    /// </pre>
    /// </summary>
    public sealed class HttpContentTypeField : HttpField, IEquatable<HttpContentTypeField>
    {
        /// <summary>
        /// The field name.
        /// </summary>
        public const string FieldName = "Content-Type";

        /// <summary>
        /// The field name in uppercase.
        /// </summary>
        public const string FieldNameUpper = "CONTENT-TYPE";

        /// <summary>
        /// Creates a Content Type Field according to the given media type, media subtype and parameters.
        /// </summary>
        /// <param name="mediaType">The main type of the content of this HTTP message.</param>
        /// <param name="mediaSubtype">The subtype of the content of this HTTP message.</param>
        /// <param name="parameters">Parameters on the specific type.</param>
        public HttpContentTypeField(string mediaType, string mediaSubtype, HttpFieldParameters parameters)
            : base(FieldName, string.Format(CultureInfo.InvariantCulture, "{0}/{1}{2}", mediaType, mediaSubtype, parameters))
        {
            MediaType = mediaType;
            MediaSubtype = mediaSubtype;
            Parameters = parameters;
        }

        /// <summary>
        /// The main type of the content of this HTTP message.
        /// </summary>
        public string MediaType { get; private set; }

        /// <summary>
        /// The subtype of the content of this HTTP message.
        /// </summary>
        public string MediaSubtype { get; private set; }

        /// <summary>
        /// Parameters on the specific type.
        /// </summary>
        public HttpFieldParameters Parameters { get; private set; }

        /// <summary>
        /// True iff the two fields are equal.
        /// Two content type fields are equal if they have the same media type and subtype and same parameters.
        /// </summary>
        public bool Equals(HttpContentTypeField other)
        {
            return other != null &&
                   MediaType == other.MediaType &&
                   MediaSubtype == other.MediaSubtype &&
                   Parameters.Equals(other.Parameters);
        }

        /// <summary>
        /// True iff the two fields are equal.
        /// Two content type fields are equal if they have the same media type and subtype and same parameters.
        /// </summary>
        public override bool Equals(HttpField other)
        {
            return Equals(other as HttpContentTypeField);
        }

        internal HttpContentTypeField(byte[] fieldValue)
            : base(FieldName, fieldValue)
        {
            string fieldValueString = HttpRegex.GetString(fieldValue);
            Match match = _regex.Match(fieldValueString);
            if (!match.Success)
                return;

            MediaType = match.Groups[MediaTypeGroupName].Captures.Cast<Capture>().First().Value;
            MediaSubtype = match.Groups[MediaSubTypeGroupName].Captures.Cast<Capture>().First().Value;
            Parameters = new HttpFieldParameters(match.GroupCapturesValues(HttpRegex.ParameterNameGroupName),
                                                 match.GroupCapturesValues(HttpRegex.ParameterValueGroupName));
        }

        private const string MediaTypeGroupName = "MediaType";
        private const string MediaSubTypeGroupName = "MediaSubType";

        private static readonly Regex _regex =
            HttpRegex.MatchEntire(HttpRegex.Concat(HttpRegex.Capture(HttpRegex.Token, MediaTypeGroupName),
                                                   HttpRegex.Build('/'),
                                                   HttpRegex.Capture(HttpRegex.Token, MediaSubTypeGroupName),
                                                   HttpRegex.OptionalParameters));
    }
}