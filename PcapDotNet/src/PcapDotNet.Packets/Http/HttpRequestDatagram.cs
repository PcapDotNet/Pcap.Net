using System;

namespace PcapDotNet.Packets.Http
{
    public class HttpRequestDatagram : HttpDatagram
    {
        internal HttpRequestDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }

        public override bool IsRequest
        {
            get { return true; }
        }

        public string Method
        {
            get
            {
                ParseFirstLine();
                return _method;
            }
        }
        
        public string Uri
        {
            get
            {
                ParseFirstLine();
                return _uri;
            }
        }

        internal override void ParseSpecificFirstLine(out HttpVersion version, out int? headerOffset)
        {
            HttpParser parser = new HttpParser(Buffer, StartOffset, Length);
            parser.Token(out _method).Space().RequestUri(out _uri).Space().Version(out version).CarraigeReturnLineFeed();
            headerOffset = parser.Success ? (int?)(parser.Offset - StartOffset) : null;
        }

        private string _uri;
        private string _method;
    }
}