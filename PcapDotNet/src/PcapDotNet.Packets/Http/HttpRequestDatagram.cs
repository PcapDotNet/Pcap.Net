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

        internal override void ParseFirstLine(HttpParser parser)
        {
            string method;
            string uri;
            HttpVersion version;
            parser.Token(out method).Space().RequestUri(out uri).Space().Version(out version).CarraigeReturnLineFeed();
        }
    }
}