using System.Collections.Generic;
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
    /// uric_no_slash    = unreserved | escaped | ";" | "?" | ":" | "@" | "&" | "=" | "+" | "$" | ","
    /// uric             = reserved | unreserved | escaped
    /// authority        = server | reg_name
    /// path_segments    = segment *( "/" segment )
    /// unreserved       = alphanum | mark
    /// escaped          = "%" hex hex
    /// reserved         = ";" | "/" | "?" | ":" | "@" | "&" | "=" | "+" | "$" | ","
    /// server           = [ [ userinfo "@" ] hostport ]
    /// reg_name         = 1*( unreserved | escaped | "$" | "," | ";" | ":" | "@" | "&" | "=" | "+" )
    /// segment          = *pchar *( ";" param )
    /// mark             = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
    /// userinfo         = *( unreserved | escaped | ";" | ":" | "&" | "=" | "+" | "$" | "," )
    /// hostport         = host [ ":" port ]
    /// pchar            = unreserved | escaped | ":" | "@" | "&" | "=" | "+" | "$" | ","
    /// param            = *pchar
    /// host             = hostname | IPv4address
    /// port             = *digit
    /// hostname         = *( domainlabel "." ) toplabel [ "." ]
    /// IPv4address      = 1*digit "." 1*digit "." 1*digit "." 1*digit
    /// domainlabel      = alphanum | alphanum *( alphanum | "-" ) alphanum
    /// toplabel         = alpha | alpha *( alphanum | "-" ) alphanum
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
        public abstract bool IsRequest { get; }
        public bool IsResponse { get { return !IsRequest; } }

        public HttpVersion Version
        {
            get
            {
                ParseFirstLine();
                return _version;
            } 
        }

        public HttpHeader Header
        {
            get
            {
                ParseHeader();
                return _header;
            }
        }
        
        internal static HttpDatagram CreateDatagram(byte[] buffer, int offset, int length)
        {
            if (length >= _httpSlash.Length && buffer.SequenceEqual(offset, _httpSlash, 0, _httpSlash.Length))
                return new HttpResponseDatagram(buffer, offset, length);
            return new HttpRequestDatagram(buffer, offset, length);
        }

        internal HttpDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }

        internal void ParseFirstLine()
        {
            if (_isParsedFirstLine)
                return;
            _isParsedFirstLine = true;

            ParseSpecificFirstLine(out _version, out _headerOffset);
        }

        internal abstract void ParseSpecificFirstLine(out HttpVersion version, out int? headerOffset);

        private void ParseHeader()
        {
            if (_isParsedHeader)
                return;
            _isParsedHeader = true;

            ParseFirstLine();
            if (_headerOffset == null)
                return;

            _header = new HttpHeader(GetHeaderFields());
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<byte>>> GetHeaderFields()
        {
            int headerOffsetValue = _headerOffset.Value;
            HttpParser parser = new HttpParser(Buffer, StartOffset + headerOffsetValue, Length - headerOffsetValue);
            while (parser.Success)
            {
                string fieldName;
                IEnumerable<byte> fieldValue;
                parser.Token(out fieldName).Colon().FieldValue(out fieldValue).CarraigeReturnLineFeed();
                if (parser.Success)
                    yield return new KeyValuePair<string, IEnumerable<byte>>(fieldName, fieldValue);
            }
        }

        private static readonly byte[] _httpSlash = Encoding.ASCII.GetBytes("HTTP/");

        private bool _isParsedFirstLine;
        private bool _isParsedHeader;
        private int? _headerOffset;
        private HttpVersion _version;
        private HttpHeader _header;
    }


    //    internal static class IEnumerableExtensions
//    {
//        public static int CountLinearWhiteSpaces(this IEnumerable<byte> sequence)
//        {
//            int count = 0;
//            while (true)
//            {
//                byte first = sequence.FirstOrDefault();
//                if (first == AsciiBytes.CarriageReturn) // CR
//                {
//                    IEnumerable<byte> skippedSequence = sequence.Skip(1);
//                    if (skippedSequence.FirstOrDefault() == AsciiBytes.LineFeed) // CRLF
//                    {
//                        skippedSequence = skippedSequence.Skip(1);
//                        if (skippedSequence.FirstOrDefault().IsSpaceOrHorizontalTab()) // CRLF ( SP | HT )
//                        {
//                            sequence = skippedSequence.Skip(1);
//                            count += 3;
//                        }
//                        else // CRLF without ( SP | HT )
//                            return count;
//                    }
//                    else // CR without LF
//                        return count;
//                }
//                else if (first.IsSpaceOrHorizontalTab()) // ( SP | HT )
//                {
//                    ++count;
//                    sequence = sequence.Skip(1);
//                }
//                else // Doesn't start with ( CR | SP | HT )
//                    return count;
//            }
//        }
//    }
}