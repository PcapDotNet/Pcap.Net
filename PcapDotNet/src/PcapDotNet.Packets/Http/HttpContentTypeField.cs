using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpContentTypeField : HttpField, IEquatable<HttpContentTypeField>
    {
        public const string Name = "Content-Type";
        public const string NameLower = "content-type";

        public HttpContentTypeField(string mediaType, string mediaSubType, HttpFieldParameters parameters)
            :base(Name, string.Format("{0}/{1}{2}", mediaType, mediaSubType, parameters))
        {
            MediaType = mediaType;
            MediaSubType = mediaSubType;
            Parameters = parameters;
        }

        public bool Equals(HttpContentTypeField other)
        {
            return other != null &&
                   MediaType == other.MediaType &&
                   MediaSubType == other.MediaSubType &&
                   Parameters.Equals(other.Parameters);
        }

        public string MediaType { get; private set; }
        public string MediaSubType { get; private set; }
        public HttpFieldParameters Parameters { get; private set;}

        public override bool Equals(HttpField other)
        {
            return Equals(other as HttpContentTypeField);
        }

        internal HttpContentTypeField(byte[] fieldValue)
            : base(Name, fieldValue)
        {
            string fieldValueString = HttpRegex.GetString(fieldValue);
            Match match = _regex.Match(fieldValueString);
            if (!match.Success)
                return;

            MediaType = match.Groups[MediaTypeGroupName].Captures.Cast<Capture>().First().Value;
            MediaSubType = match.Groups[MediaSubTypeGroupName].Captures.Cast<Capture>().First().Value;
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