using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpContentTypeField : HttpField, IEquatable<HttpContentTypeField>
    {
        public const string FieldName = "Content-Type";
        public const string FieldNameUpper = "CONTENT-TYPE";

        public HttpContentTypeField(string mediaType, string mediaSubtype, HttpFieldParameters parameters)
            : base(FieldName, string.Format(CultureInfo.InvariantCulture, "{0}/{1}{2}", mediaType, mediaSubtype, parameters))
        {
            MediaType = mediaType;
            MediaSubtype = mediaSubtype;
            Parameters = parameters;
        }

        public bool Equals(HttpContentTypeField other)
        {
            return other != null &&
                   MediaType == other.MediaType &&
                   MediaSubtype == other.MediaSubtype &&
                   Parameters.Equals(other.Parameters);
        }

        public string MediaType { get; private set; }
        public string MediaSubtype { get; private set; }
        public HttpFieldParameters Parameters { get; private set;}

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