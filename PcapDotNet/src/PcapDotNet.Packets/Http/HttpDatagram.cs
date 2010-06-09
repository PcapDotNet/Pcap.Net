using System;
using System.Collections.Generic;
using PcapDotNet.Base;
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
    public abstract class HttpDatagram : Datagram
    {
        public abstract bool IsRequest { get; }
        public bool IsResponse { get { return !IsRequest; } }

        public HttpHeader Header
        {
            get
            {
                if (_header == null)
                    _header = new HttpHeader(Buffer, StartOffset + HeaderOffset, Length - HeaderOffset);
                return _header;
            }
        }

        protected abstract int HeaderOffset { get; }

        internal HttpDatagram CreateDatagram(byte[] buffer, int offset, int length)
        {
            if (length >= _httpSlash.Length && buffer.SequenceEqual(offset, _httpSlash, 0, _httpSlash.Length))
                return new HttpResponseDatagram(buffer, offset, length);
            return new HttpRequestDatagram(buffer, offset, length);
        }

        internal HttpDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }

        private static readonly byte[] _httpSlash = Encoding.ASCII.GetBytes("HTTP/");
        private HttpHeader _header;
    }

    public class HttpHeader
    {
        private class Parser
        {
            public Parser(byte[] buffer, int offset, int length)
            {
                _buffer = buffer;
                _offset = offset;
                _totalLength = offset + length;
                Success = offset >= 0 && length >= 0 && offset + length <= buffer.Length;
            }

            public bool Success { get; private set;}

            public Parser Token(out string token)
            {
                if (!Success)
                {
                    token = null;
                    return this;
                }

                int tokenLength = Range.TakeWhile(value => value.IsToken()).Count();
                if (tokenLength == 0)
                {
                    token = null;
                    return Fail();
                }

                token = Encoding.ASCII.GetString(_buffer, _offset, tokenLength);
                _offset += token.Length;
                return this;
            }

            public Parser Colon()
            {
                if (!Success)
                    return this;

                if (Range.First() != AsciiBytes.Colon)
                    return Fail();

                ++_offset;
                return this;
            }

            public Parser FieldValue()
            {
                if (!Success)
                    return this;

                throw new NotImplementedException();
//                SkipLws();
//                while (Success)
//                {
//                    
//                }
            }

            private Parser Fail()
            {
                Success = false;
                return this;
            }

            private IEnumerable<byte> Range
            {
                get { return _buffer.Range(_offset, _totalLength - _offset);}
            }

            private readonly byte[] _buffer;
            private int _offset;
            private readonly int _totalLength;
        }

        internal HttpHeader(byte[] buffer, int offset, int length)
        {
            Parser parser = new Parser(buffer, offset, length);
            string fieldName;
            parser.Token(out fieldName).Colon().FieldValue();
//            int totalLength = offset + length;
//            Datagram data = new Datagram(buffer, offset, totalLength - offset);

            // Parse field-name = token
//            string fieldName;
//            if (!TryParseToken(buffer, offset, totalLength - offset, out fieldName))
//                return;
//            offset += fieldName.Length;
//            data = new Datagram(buffer, offset, totalLength - offset);

            // Parse ":"
//            if (data.FirstOrDefault() != AsciiBytes.Colon)
//                return;
//
//            ++offset;
            
            // Parse field-value
//            if (!TryParseFieldValue(buffer, offset, totalLength - offset))
//                return;

//            data = new Datagram(buffer, offset, totalLength - offset);

            // Parse field-value = *( field-content | LWS )
//            IEnumerable<byte> fieldValue = new byte[0];
//
//            int lwsCount = data.CountLinearWhiteSpaces();
//            if (lwsCount != 0)
//            {
//                offset += lwsCount;
//                data = new Datagram(buffer, offset, totalLength - offset);
//            }
//
//            int fieldContentCount;
//            IEnumerable<byte> fieldContent = data.TakeFieldContent(out fieldContentCount);
//
//            if (fieldContentCount != 0)
//            {
//                if (fieldValue.Any())
//                    fieldValue = fieldValue.Concat(AsciiBytes.Space);
//                fieldValue = fieldValue.Concat(fieldContent);
//                offset += fieldContentCount;
//            }
//            data.SkipWhile
//            buffer..Take(count).TakeWhile(value => value.IsToken()).Count))
//            Encoding.ASCII.GetString()
        }

//        private HttpHeaderPart _generalHeader;
//        private HttpHeaderPart _requestHeader;
//        private HttpHeaderPart _responseHeader;
//        private HttpHeaderPart _entityHeader;
//        private Dictionary<string, >
    }

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

        protected override int HeaderOffset
        {
            get { throw new NotImplementedException(); }
        }
    }

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

        protected override int HeaderOffset
        {
            get { throw new NotImplementedException(); }
        }
    }

    internal static class IEnumerableExtensions
    {
        public static int CountLinearWhiteSpaces(this IEnumerable<byte> sequence)
        {
            int count = 0;
            while (true)
            {
                byte first = sequence.FirstOrDefault();
                if (first == AsciiBytes.CarriageReturn) // CR
                {
                    IEnumerable<byte> skippedSequence = sequence.Skip(1);
                    if (skippedSequence.FirstOrDefault() == AsciiBytes.LineFeed) // CRLF
                    {
                        skippedSequence = skippedSequence.Skip(1);
                        if (skippedSequence.FirstOrDefault().IsSpaceOrHorizontalTab()) // CRLF ( SP | HT )
                        {
                            sequence = skippedSequence.Skip(1);
                            count += 3;
                        }
                        else // CRLF without ( SP | HT )
                            return count;
                    }
                    else // CR without LF
                        return count;
                }
                else if (first.IsSpaceOrHorizontalTab()) // ( SP | HT )
                {
                    ++count;
                    sequence = sequence.Skip(1);
                }
                else // Doesn't start with ( CR | SP | HT )
                    return count;
            }
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

        public static bool IsSpaceOrHorizontalTab(this byte value)
        {
            return value == AsciiBytes.Space || value == AsciiBytes.HorizontalTab;
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