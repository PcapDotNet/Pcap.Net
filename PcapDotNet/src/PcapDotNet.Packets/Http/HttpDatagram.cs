using System;
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

        public Datagram Body
        {
            get
            {
                if (_body == null)
                {
                    ParseHeader();
                    if (_bodyOffset != null)
                    {
                        int bodyOffsetValue = _bodyOffset.Value;
                        if (!IsBodyPossible)
                        {
                            _body = Empty;
                            return _body;
                        }

                        HttpTransferEncodingField transferEncodingField = Header.TransferEncoding;
                        if (transferEncodingField != null)
                        {
                            if (transferEncodingField.TransferCodings.Any(coding => coding != "identity"))
                            {
                                _body = ReadChunked();
                                return _body;
                            }
                        }

                        HttpContentLengthField contentLengthField = Header.ContentLength;
                        if (contentLengthField != null)
                        {
                            uint? contentLength = contentLengthField.ContentLength;
                            if (contentLength != null)
                            {
                                _body = new Datagram(Buffer, StartOffset + bodyOffsetValue, Math.Min((int)contentLength.Value, Length - bodyOffsetValue));
                                return _body;
                            }
                        }

                        HttpContentTypeField contentTypeField = Header.ContentType;
                        if (contentTypeField != null)
                        {
                            if (contentTypeField.MediaType == "multipart" &&
                                contentTypeField.MediaSubType == "byteranges")
                            {
                                string boundary = contentTypeField.Parameters["boundary"];
                                if (boundary != null)
                                {
                                    byte[] lastBoundaryBuffer = Encoding.ASCII.GetBytes(string.Format("--{0}--\r\n", boundary));
                                    int lastBoundaryOffset = Buffer.Find(StartOffset + bodyOffsetValue, Length - bodyOffsetValue, lastBoundaryBuffer);
                                    int lastBoundaryEnd = lastBoundaryOffset + lastBoundaryBuffer.Length;
                                    _body = new Datagram(Buffer, StartOffset + bodyOffsetValue,
                                                         Math.Min(lastBoundaryEnd - StartOffset - bodyOffsetValue, Length - bodyOffsetValue));
                                    return _body;
                                }
                            }
                        }

                        _body = new Datagram(Buffer, StartOffset + bodyOffsetValue, Length - bodyOffsetValue);
                    }
                }
                return _body;
            }
        }

        private Datagram ReadChunked()
        {
            List<Datagram> contentData = new List<Datagram>();
            HttpParser parser = new HttpParser(Buffer, StartOffset + _bodyOffset.Value, Length - _bodyOffset.Value);
            uint? chunkSize;
            while (parser.HexadecimalNumber(out chunkSize).SkipChunkExtensions().CarriageReturnLineFeed().Success)
            {
                uint chunkSizeValue = chunkSize.Value;
                if (chunkSizeValue == 0)
                {
                    int? endOffset;
                    HttpHeader trailerHeader = new HttpHeader(GetHeaderFields(out endOffset, Buffer, parser.Offset, Buffer.Length - parser.Offset));
                    parser.CarriageReturnLineFeed();
                    break;
                }

                int actualChunkSize = (int)Math.Min(chunkSizeValue, Buffer.Length - parser.Offset);
                contentData.Add(new Datagram(Buffer, parser.Offset, actualChunkSize));
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
            Datagram content = new Datagram(contentBuffer);

            return new Datagram(Buffer, StartOffset + _bodyOffset.Value, parser.Offset - StartOffset);
        }

        protected abstract bool IsBodyPossible { get; }

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

        private List<KeyValuePair<string, IEnumerable<byte>>> GetHeaderFields()
        {
            int headerOffsetValue = _headerOffset.Value;
            int? endOffset;
            var result = GetHeaderFields(out endOffset, Buffer, StartOffset + headerOffsetValue, Length - headerOffsetValue);
            if (endOffset != null)
                _bodyOffset = endOffset.Value - StartOffset;
            return result;
        }

        private static List<KeyValuePair<string, IEnumerable<byte>>> GetHeaderFields(out int? endOffset, byte[] buffer, int offset, int length)
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

        private bool _isParsedFirstLine;
        private bool _isParsedHeader;
        private int? _headerOffset;
        private int? _bodyOffset;
        private HttpVersion _version;
        private HttpHeader _header;

        private Datagram _body;
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