using System;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// Represents an HTTP request.
    /// </summary>
    public sealed class HttpRequestDatagram : HttpDatagram
    {
        private class ParseInfo : ParseInfoBase
        {
            public HttpRequestMethod Method { get; set; }
            public string Uri { get; set; }
        }

        /// <summary>
        /// True since the message is a request.
        /// </summary>
        public override bool IsRequest
        {
            get { return true; }
        }

        /// <summary>
        /// The HTTP Request Method.
        /// </summary>
        public HttpRequestMethod Method { get; private set; }

        /// <summary>
        /// The HTTP Request URI.
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new HttpRequestLayer
                   {
                       Version = Version,
                       Method = Method,
                       Uri = Uri,
                       Header = Header,
                       Body = Body,
                   };
        }

        /// <summary>
        /// An HTTP Request has a valid start if it contains a method, an URI, and a version.
        /// </summary>
        protected override bool CalculateIsValidStart()
        {
            return Method != null && !string.IsNullOrEmpty(Uri) && Version != null;
        }

        internal HttpRequestDatagram(byte[] buffer, int offset, int length) 
            : this(buffer, offset, Parse(buffer, offset, length))
        {
        }

        private HttpRequestDatagram(byte[] buffer, int offset, ParseInfo parseInfo)
            :base(buffer, offset, parseInfo.Length, parseInfo.Version, parseInfo.Header, parseInfo.Body)
        {
            Method = parseInfo.Method;
            Uri = parseInfo.Uri;
        }

        private static ParseInfo Parse(byte[] buffer, int offset, int length)
        {
            // First Line
            HttpParser parser = new HttpParser(buffer, offset, length);
            string method;
            string uri;
            HttpVersion version;
            parser.Token(out method).Space().RequestUri(out uri).Space().Version(out version).CarriageReturnLineFeed();
            ParseInfo parseInfo = new ParseInfo
                                  {
                                      Length = length,
                                      Version = version,
                                      Method = method == null ? null : new HttpRequestMethod(method),
                                      Uri = uri,
                                  };
            if (!parser.Success)
                return parseInfo;

            int firstLineLength = parser.Offset - offset;

            // Header
            int? endHeaderOffset;
            HttpHeader header = new HttpHeader(GetHeaderFields(out endHeaderOffset, buffer, offset + firstLineLength, length - firstLineLength));
            parseInfo.Header = header;
            if (endHeaderOffset == null)
                return parseInfo;

            int headerLength = endHeaderOffset.Value - offset - firstLineLength;

            // Body
            Datagram body = ParseBody(buffer, offset + firstLineLength + headerLength, length - firstLineLength - headerLength, IsBodyPossible(header), header);
            parseInfo.Body = body;
            parseInfo.Length = firstLineLength + headerLength + body.Length;
            return parseInfo;
        }

        private static bool IsBodyPossible(HttpHeader header)
        {
            return header.ContentLength != null;
        }
    }
}