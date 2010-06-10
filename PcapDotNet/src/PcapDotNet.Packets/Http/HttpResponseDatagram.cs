using System;

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
            int statusCode;
            parser.Version().Space().DecimalNumber(3, out statusCode).Space().ReasonPhrase().CarraigeReturnLineFeed();
        }
    }
}