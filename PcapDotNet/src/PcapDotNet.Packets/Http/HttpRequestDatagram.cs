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
            parser.Token(out method).Space().RequestUri().Space().Version().CarraigeReturnLineFeed();
        }
    }
}