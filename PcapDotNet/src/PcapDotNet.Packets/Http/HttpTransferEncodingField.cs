using System;
using System.Collections.Generic;

namespace PcapDotNet.Packets.Http
{
    public class HttpTransferEncodingField : HttpField
    {
        public const string Name = "Transfer-Encoding";

        public HttpTransferEncodingField(byte[] fieldValue)
            : base(Name, fieldValue)
        {
            HttpParser parser = new HttpParser(fieldValue);
            List<HttpTransferCoding> transferCodings;
            parser.CommaSeparated(parser.TransferCoding, out transferCodings);
        }
    }
}