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
    public abstract class HttpStartLineDatagram : Datagram
    {
        private static readonly byte[] _httpSlash = Encoding.ASCII.GetBytes("HTTP/");
//        public HttpHeaderDatagram Header
//        {
//            get {return }
//        }

        internal HttpStartLineDatagram CreateDatagram(byte[] buffer, int offset, int length)
        {
            if (length >= _httpSlash.Length && buffer.SequenceEqual(offset, _httpSlash, 0, _httpSlash.Length))
                return new HttpResponseStartLineDatagram(buffer, offset, length);
            return new HttpRequestStartLineDatagram(buffer, offset, length);
        }

        internal HttpStartLineDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
    }

    public class HttpRequestStartLineDatagram : HttpStartLineDatagram
    {
        internal HttpRequestStartLineDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
    }

    public class HttpResponseStartLineDatagram : HttpStartLineDatagram
    {
        internal HttpResponseStartLineDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
    }

    public class HttpHeaderDatagram : Datagram
    {
        internal HttpHeaderDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
        
//        public HttpEntityBodyDatagram EntityBody { get;}
    }

    public class HttpEntityBodyDatagram : Datagram
    {
        internal HttpEntityBodyDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
//        public HttpMessageBodyDatagram MessageBody { get;}
    }

    public class HttpMessageBodyDatagram : Datagram
    {
        internal HttpMessageBodyDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
    }

    public class HttpRequestHeaderDatagram : HttpHeaderDatagram
    {
        internal HttpRequestHeaderDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
    }

    public class HttpResponseHeaderDatagram : HttpHeaderDatagram
    {
        internal HttpResponseHeaderDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }
    }

    internal static class AsciiBytes
    {
        public const byte UpperA = (byte)'A';
        public const byte UpperF = (byte)'F';
        public const byte UpperZ = (byte)'Z';
        public const byte LowerA = (byte)'a';
        public const byte LowerF = (byte)'f';
        public const byte LowerZ = (byte)'z';
        public const byte Zero = (byte)'0';
        public const byte Nine = (byte)'9';
        public const byte Delete = 127; // DEL
        public const byte CarriageReturn = 13; // CR
        public const byte LineFeed = 10; // LF
        public const byte Space = (byte)' '; // SP
        public const byte HorizontalTab = 9; // HT
        public const byte DoubleQuotationMark = (byte)'"';
        public const byte LeftRoundBracket = (byte)'(';
        public const byte RightRoundBracket = (byte)')';
        public const byte LowerThan = (byte)'<';
        public const byte BiggerThan = (byte)'>';
        public const byte AtSign = (byte)'@';
        public const byte Comma = (byte)',';
        public const byte Semicolon = (byte)';';
        public const byte Colon = (byte)':';
        public const byte BackSlash = (byte)'\\';
        public const byte Slash = (byte)'/';
        public const byte LeftSquareBracket = (byte)'[';
        public const byte RightSquareBracket = (byte)']';
        public const byte QuestionMark = (byte)'?';
        public const byte EqualsSign = (byte)'=';
        public const byte LeftCurlyBracket = (byte)'{';
        public const byte RightCurlyBracket = (byte)'}';
    }

    internal static class ByteExtensions
    {

        // CHAR
        public static bool IsChar(this byte value)
        {
            return value <= 127;
        }

        // UPALPHA        
        public static bool IsUpAlpha(this byte value)
        {
            return value >= AsciiBytes.UpperA && value <= AsciiBytes.UpperZ;
        }

        // LOALPHA        
        public static bool IsLowerAlpha(this byte value)
        {
            return value >= AsciiBytes.LowerA && value <= AsciiBytes.LowerZ;
        }

        // ALPHA          
        public static bool IsAlpha(this byte value)
        {
            return value.IsUpAlpha() || value.IsLowerAlpha();
        }

        // DIGIT          
        public static bool IsDigit(this byte value)
        {
            return value >= AsciiBytes.Zero && value <= AsciiBytes.Nine;
        }

        // HEX
        public static bool IsHexadecimalDigit(this byte value)
        {
            return value >= AsciiBytes.UpperA && value <= AsciiBytes.UpperF ||
                   value >= AsciiBytes.LowerA && value <= AsciiBytes.LowerF ||
                   value.IsDigit();
        }

        // CTL            
        public static bool IsControl(this byte value)
        {
            return value <= 31 || value == AsciiBytes.Delete;
        }

        // separators     
        public static bool IsSeparator(this byte value)
        {
            switch (value)
            {
                case AsciiBytes.LeftRoundBracket:
                case AsciiBytes.RightRoundBracket:
                case AsciiBytes.LowerThan:
                case AsciiBytes.BiggerThan:
                case AsciiBytes.AtSign:
                case AsciiBytes.Comma:
                case AsciiBytes.Semicolon:
                case AsciiBytes.Colon:
                case AsciiBytes.BackSlash:
                case AsciiBytes.DoubleQuotationMark:
                case AsciiBytes.Slash:
                case AsciiBytes.LeftSquareBracket:
                case AsciiBytes.RightSquareBracket:
                case AsciiBytes.QuestionMark:
                case AsciiBytes.EqualsSign:
                case AsciiBytes.LeftCurlyBracket:
                case AsciiBytes.RightCurlyBracket:
                case AsciiBytes.Space:
                case AsciiBytes.HorizontalTab:
                    return true;

                default:
                    return false;
            }
        }

        // token          
        public static bool IsToken(this byte value)
        {
            return value.IsChar() && !value.IsControl() && !value.IsSeparator();
        }
    }
}