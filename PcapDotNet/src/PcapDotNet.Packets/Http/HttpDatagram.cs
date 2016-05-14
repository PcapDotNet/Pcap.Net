using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616
    /// 
    /// Message:
    /// <pre>
    /// HTTP-message     = Request | Response
    /// 
    /// generic-message  = start-line
    ///                    *(message-header CRLF)
    ///                    CRLF
    ///                    [ message-body ]
    /// 
    /// start-line       = Request-Line | Status-Line
    /// 
    /// message-header   = field-name ":" [ field-value ]
    /// field-name       = token
    /// field-value      = *( field-content | LWS )
    /// field-content    = &lt;the OCTETs making up the field-value and consisting of either *TEXT or combinations of token, separators, and quoted-string>
    /// 
    /// message-body     = entity-body
    ///                  | &lt;entity-body encoded as per Transfer-Encoding>
    /// general-header   = Cache-Control          
    ///                  | Connection             
    ///                  | Date                   
    ///                  | Pragma                 
    ///                  | Trailer                
    ///                  | Transfer-Encoding      
    ///                  | Upgrade                
    ///                  | Via                    
    ///                  | Warning                
    /// </pre>
    /// 
    /// Request:
    /// <pre>
    /// Request          = Request-Line             
    ///                    *(( general-header       
    ///                     | request-header        
    ///                     | entity-header ) CRLF) 
    ///                    CRLF
    ///                    [ message-body ]         
    /// 
    /// Request-Line     = Method SP Request-URI SP HTTP-Version CRLF
    /// 
    /// Method           = "OPTIONS"              
    ///                  | "GET"                  
    ///                  | "HEAD"                 
    ///                  | "POST"                 
    ///                  | "PUT"                  
    ///                  | "DELETE"               
    ///                  | "TRACE"                
    ///                  | "CONNECT"              
    ///                  | extension-method
    /// 
    /// extension-method = token
    /// 
    /// Request-URI      = "*" | absoluteURI | abs_path | authority
    /// absoluteURI      = scheme ":" ( hier_part | opaque_part )
    /// scheme           = alpha *( alpha | digit | "+" | "-" | "." )
    /// hier_part        = ( net_path | abs_path ) [ "?" query ]
    /// opaque_part      = uric_no_slash *uric
    /// net_path         = "//" authority [ abs_path ]
    /// abs_path         = "/"  path_segments
    /// query            = *uric
    /// uric_no_slash    = unreserved | escaped | ";" | "?" | ":" | "@" | "&amp;" | "=" | "+" | "$" | ","
    /// uric             = reserved | unreserved | escaped
    /// authority        = server | reg_name
    /// path_segments    = segment *( "/" segment )
    /// unreserved       = alphanum | mark
    /// escaped          = "%" hex hex
    /// reserved         = ";" | "/" | "?" | ":" | "@" | "&amp;" | "=" | "+" | "$" | ","
    /// server           = [ [ userinfo "@" ] hostport ]
    /// reg_name         = 1*( unreserved | escaped | "$" | "," | ";" | ":" | "@" | "&amp;" | "=" | "+" )
    /// segment          = *pchar *( ";" param )
    /// mark             = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
    /// userinfo         = *( unreserved | escaped | ";" | ":" | "&amp;" | "=" | "+" | "$" | "," )
    /// hostport         = host [ ":" port ]
    /// pchar            = unreserved | escaped | ":" | "@" | "&amp;" | "=" | "+" | "$" | ","
    /// param            = *pchar
    /// host             = hostname | IPv4address
    /// port             = *digit
    /// hostname         = *( domainlabel "." ) toplabel [ "." ]
    /// IPv4address      = 1*digit "." 1*digit "." 1*digit "." 1*digit
    /// domainlabel      = alphanum | alphanum *( alphanum | "-" ) alphanum
    /// toplabel         = alpha | alpha *( alphanum | "-" ) alphanum
    /// alphanum         = alpha | digit
    /// alpha            = lowalpha | upalpha
    /// lowalpha         = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" | "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" |
    ///                    "w" | "x" | "y" | "z"
    /// upalpha          = "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" | "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" |
    ///                    "W" | "X" | "Y" | "Z"
    /// digit            = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
    /// 
    /// request-header   = Accept                  
    ///                  | Accept-Charset          
    ///                  | Accept-Encoding         
    ///                  | Accept-Language         
    ///                  | Authorization           
    ///                  | Expect                  
    ///                  | From                    
    ///                  | Host                    
    ///                  | If-Match                
    ///                  | If-Modified-Since       
    ///                  | If-None-Match           
    ///                  | If-Range                
    ///                  | If-Unmodified-Since     
    ///                  | Max-Forwards            
    ///                  | Proxy-Authorization     
    ///                  | Range                   
    ///                  | Referer                 
    ///                  | TE                      
    ///                  | User-Agent              
    /// </pre>
    /// 
    /// Response:
    /// <pre>
    /// Response         = Status-Line             
    ///                    *(( general-header      
    ///                     | response-header      
    ///                     | entity-header ) CRLF)
    ///                    CRLF
    ///                    [ message-body ]        
    /// 
    /// Status-Line      = HTTP-Version SP Status-Code SP Reason-Phrase CRLF
    /// 
    /// Status-Code      = "100"  
    ///                  | "101"  
    ///                  | "200"  
    ///                  | "201"  
    ///                  | "202"  
    ///                  | "203"  
    ///                  | "204"  
    ///                  | "205"  
    ///                  | "206"  
    ///                  | "300"  
    ///                  | "301"  
    ///                  | "302"  
    ///                  | "303"  
    ///                  | "304"  
    ///                  | "305"  
    ///                  | "307"  
    ///                  | "400"  
    ///                  | "401"  
    ///                  | "402"  
    ///                  | "403"  
    ///                  | "404"  
    ///                  | "405"  
    ///                  | "406"  
    ///                  | "407"  
    ///                  | "408"  
    ///                  | "409"  
    ///                  | "410"  
    ///                  | "411"  
    ///                  | "412"  
    ///                  | "413"  
    ///                  | "414"  
    ///                  | "415"  
    ///                  | "416"  
    ///                  | "417"  
    ///                  | "500"  
    ///                  | "501"  
    ///                  | "502"  
    ///                  | "503"  
    ///                  | "504"  
    ///                  | "505"  
    ///                  | extension-code
    /// 
    /// extension-code   = 3DIGIT
    /// Reason-Phrase    = *&lt;TEXT, excluding CR, LF>
    /// 
    /// response-header  = Accept-Ranges       
    ///                  | Age                 
    ///                  | ETag                
    ///                  | Location            
    ///                  | Proxy-Authenticate  
    ///                  | Retry-After         
    ///                  | Server              
    ///                  | Vary                
    ///                  | WWW-Authenticate    
    ///
    /// entity-header    = Allow               
    ///                  | Content-Encoding    
    ///                  | Content-Language    
    ///                  | Content-Length      
    ///                  | Content-Location    
    ///                  | Content-MD5         
    ///                  | Content-Range       
    ///                  | Content-Type        
    ///                  | Expires             
    ///                  | Last-Modified       
    ///                  | extension-header
    ///
    /// extension-header = message-header
    /// 
    /// entity-body      = *OCTET
    /// 
    /// entity-body     := Content-Encoding( Content-Type( data ) )
    /// </pre>
    /// </summary>
    public abstract class HttpDatagram : Datagram
    {
        internal class ParseInfoBase
        {
            public int Length { get; set; }
            public HttpVersion Version { get; set; }
            public HttpHeader Header { get; set; }
            public Datagram Body { get; set; }
        }

        /// <summary>
        /// True iff the message is a request and iff the message is not a response.
        /// </summary>
        public abstract bool IsRequest { get; }

        /// <summary>
        /// True iff the message is a response and iff the message is not a request.
        /// </summary>
        public bool IsResponse { get { return !IsRequest; } }

        /// <summary>
        /// The version of this HTTP message.
        /// </summary>
        public HttpVersion Version  { get; private set;}
        
        /// <summary>
        /// The header of the HTTP message.
        /// </summary>
        public HttpHeader Header { get; private set;}

        /// <summary>
        /// Message Body.
        /// </summary>
        public Datagram Body { get; private set; }

        internal static HttpDatagram CreateDatagram(byte[] buffer, int offset, int length)
        {
            if (length >= _httpSlash.Length && buffer.SequenceEqual(offset, _httpSlash, 0, _httpSlash.Length))
                return new HttpResponseDatagram(buffer, offset, length);
            return new HttpRequestDatagram(buffer, offset, length);
        }

        internal HttpDatagram(byte[] buffer, int offset, int length, 
            HttpVersion version, HttpHeader header, Datagram body) 
            : base(buffer, offset, length)
        {
            Version = version;
            Header = header;
            Body = body;
        }

        internal static Datagram ParseBody(byte[] buffer, int offset, int length,
                                           bool isBodyPossible, HttpHeader header)
        {
            if (!isBodyPossible)
                return Empty;

            HttpTransferEncodingField transferEncodingField = header.TransferEncoding;
            if (transferEncodingField != null &&
                transferEncodingField.TransferCodings != null &&
                transferEncodingField.TransferCodings.Any(coding => coding != "identity"))
            {
                return ParseChunkedBody(buffer, offset, length);
            }

            HttpContentLengthField contentLengthField = header.ContentLength;
            if (contentLengthField != null)
            {
                uint? contentLength = contentLengthField.ContentLength;
                if (contentLength != null)
                    return new Datagram(buffer, offset, Math.Min((int)contentLength.Value, length));
            }

            HttpContentTypeField contentTypeField = header.ContentType;
            if (contentTypeField != null)
            {
                if (contentTypeField.MediaType == "multipart" &&
                    contentTypeField.MediaSubtype == "byteranges")
                {
                    string boundary = contentTypeField.Parameters["boundary"];
                    if (boundary != null)
                    {
                        byte[] lastBoundaryBuffer = Encoding.ASCII.GetBytes(string.Format(CultureInfo.InvariantCulture, "\r\n--{0}--", boundary));
                        int lastBoundaryOffset = buffer.Find(offset, length, lastBoundaryBuffer);
                        int lastBoundaryEnd = lastBoundaryOffset + lastBoundaryBuffer.Length;
                        return new Datagram(buffer, offset,
                                             Math.Min(lastBoundaryEnd - offset, length));
                    }
                }
            }

            return new Datagram(buffer, offset, length);
        }

        private static Datagram ParseChunkedBody(byte[] buffer, int offset, int length)
        {
            List<Datagram> contentData = new List<Datagram>();
            HttpParser parser = new HttpParser(buffer, offset, length);
            uint? chunkSize;
            while (parser.HexadecimalNumber(out chunkSize).SkipChunkExtensions().CarriageReturnLineFeed().Success)
            {
                uint chunkSizeValue = chunkSize.Value;
                if (chunkSizeValue == 0)
                {
                    int? endOffset;
                    GetHeaderFields(out endOffset, buffer, parser.Offset, offset + length - parser.Offset);
                    if (endOffset != null)
                        parser.Skip(endOffset.Value - parser.Offset);
                    break;
                }

                int actualChunkSize = (int)Math.Min(chunkSizeValue, offset + length - parser.Offset);
                contentData.Add(new Datagram(buffer, parser.Offset, actualChunkSize));
                parser.Skip(actualChunkSize);
                parser.CarriageReturnLineFeed();
            }

            int contentLength = contentData.Sum(datagram => datagram.Length);
            byte[] contentBuffer = new byte[contentLength];
            int contentBufferOffset = 0;
            foreach (Datagram datagram in contentData)
            {
                datagram.Write(contentBuffer, contentBufferOffset);
                contentBufferOffset += datagram.Length;
            }

            return new Datagram(buffer, offset, parser.Offset - offset);
        }

        internal static List<KeyValuePair<string, IEnumerable<byte>>> GetHeaderFields(out int? endOffset, byte[] buffer, int offset, int length)
        {
            endOffset = null;
            var headerFields = new List<KeyValuePair<string, IEnumerable<byte>>>();
            HttpParser parser = new HttpParser(buffer, offset, length);
            while (parser.Success)
            {
                if (parser.IsCarriageReturnLineFeed())
                {
                    endOffset = parser.Offset + 2;
                    break;
                }
                string fieldName;
                IEnumerable<byte> fieldValue;
                parser.Token(out fieldName).Colon().FieldValue(out fieldValue).CarriageReturnLineFeed();
                if (parser.Success)
                    headerFields.Add(new KeyValuePair<string, IEnumerable<byte>>(fieldName, fieldValue));
            }
            return headerFields;
        }

        private static readonly byte[] _httpSlash = Encoding.ASCII.GetBytes("HTTP/");
    }
}