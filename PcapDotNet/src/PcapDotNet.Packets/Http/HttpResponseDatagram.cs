using System;
using System.Collections.Generic;

namespace PcapDotNet.Packets.Http
{
    public class HttpResponseDatagram : HttpDatagram
    {
        internal HttpResponseDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }

        public override bool IsRequest
        {
            get { return false; }
        }

        internal override void ParseFirstLine(HttpParser parser)
        {
            uint statusCode;
            HttpVersion version;
            IEnumerable<byte> reasonPhrase;
            parser.Version(out version).Space().DecimalNumber(3, out statusCode).Space().ReasonPhrase(out reasonPhrase).CarraigeReturnLineFeed();
        }
    }
}