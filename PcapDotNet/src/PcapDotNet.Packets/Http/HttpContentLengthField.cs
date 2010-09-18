namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// The Content-Length entity-header field indicates the size of the entity-body, in decimal number of OCTETs, sent to the recipient or, 
    /// in the case of the HEAD method, the size of the entity-body that would have been sent had the request been a GET.
    /// </summary>
    public class HttpContentLengthField : HttpField
    {
        /// <summary>
        /// The field name.
        /// </summary>
        public const string Name = "Content-Length";

        /// <summary>
        /// The field name in lowercase.
        /// </summary>
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