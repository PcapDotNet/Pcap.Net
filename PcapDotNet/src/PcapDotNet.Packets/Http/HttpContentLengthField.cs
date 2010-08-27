namespace PcapDotNet.Packets.Http
{
    public class HttpContentLengthField : HttpField
    {
        public const string Name = "Content-Length";
        public const string NameLower = "content-length";

        public HttpContentLengthField(uint contentLength)
            :base(Name, contentLength.ToString())
        {
            ContentLength = contentLength;
        }

        public uint? ContentLength { get; private set;}

        internal HttpContentLengthField(byte[] fieldValue)
            :base(Name, fieldValue)
        {
            HttpParser parser = new HttpParser(fieldValue);
            uint? contentLength;
            parser.DecimalNumber(out contentLength);
            if (!parser.Success)
                return;

            ContentLength = contentLength;
        }
   }
}