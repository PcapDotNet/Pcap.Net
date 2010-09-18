using System;
using System.Collections.Generic;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// Represents an HTTP response.
    /// </summary>
    public class HttpResponseDatagram : HttpDatagram
    {
        private class ParseInfo : ParseInfoBase
        {
            public Datagram ReasonPhrase { get; set; }
            public uint? StatusCode{ get; set;}
        }

        public override bool IsRequest
        {
            get { return false; }
        }

        public uint? StatusCode{get; private set;}

        public Datagram ReasonPhrase { get; private set;}

        public override ILayer ExtractLayer()
        {
            return new HttpResponseLayer
            {
                Version = Version,
                StatusCode = StatusCode,
                ReasonPhrase = ReasonPhrase,
                Header = Header,
                Body = Body,
            };
        }

        internal HttpResponseDatagram(byte[] buffer, int offset, int length) 
            : this(buffer, offset, Parse(buffer, offset, length))
        {
        }

        private HttpResponseDatagram(byte[] buffer, int offset, ParseInfo parseInfo)
            :base(buffer, offset, parseInfo.Length, parseInfo.Version, parseInfo.Header, parseInfo.Body)
        {
            StatusCode = parseInfo.StatusCode;
            ReasonPhrase = parseInfo.ReasonPhrase;
        }

        private static ParseInfo Parse(byte[] buffer, int offset, int length)
        {
            // First Line
            HttpParser parser = new HttpParser(buffer, offset, length);
            HttpVersion version;
            uint? statusCode;
            Datagram reasonPhrase;
            parser.Version(out version).Space().DecimalNumber(3, out statusCode).Space().ReasonPhrase(out reasonPhrase).CarriageReturnLineFeed();
            ParseInfo parseInfo = new ParseInfo
                                  {
                                      Length = length,
                                      Version = version,
                                      StatusCode = statusCode,
                                      ReasonPhrase = reasonPhrase
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
            Datagram body = ParseBody(buffer, offset + firstLineLength + headerLength, length - firstLineLength - headerLength, IsBodyPossible(statusCode.Value), header);
            parseInfo.Body = body;
            parseInfo.Length = firstLineLength + headerLength + body.Length;
            return parseInfo;
        }

        private static bool IsBodyPossible(uint statusCode)
        {
            if (statusCode >= 100 && statusCode <= 199 || statusCode == 204 || statusCode == 205 || statusCode == 304)
                return false;
            // if (IsResponseToHeadRequest)
            //     return false;
            return true;
        }
    }
}