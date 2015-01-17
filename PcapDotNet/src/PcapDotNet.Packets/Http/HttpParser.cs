using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    internal class HttpParser
    {
        public HttpParser(byte[] buffer)
            :this(buffer, 0, buffer.Length)
        {
        }

        public HttpParser(byte[] buffer, int offset, int length)
        {
            _buffer = buffer;
            _offset = offset;
            _totalLength = offset + length;
            Success = offset >= 0 && length >= 0 && offset + length <= buffer.Length;
        }

        public bool Success { get; private set; }

        public int Offset
        {
            get { return _offset; }
        }

        public HttpParser Token(out Datagram token)
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
            
            token = new Datagram(_buffer, _offset, tokenLength);
            _offset += token.Length;
            return this;
        }

        public HttpParser Token(out string token)
        {
            Datagram bytesToken;
            Token(out bytesToken);
            token = Success ? bytesToken.Decode(Encoding.ASCII) : null;
            return this;
        }

        public HttpParser Colon()
        {
            return Bytes(AsciiBytes.Colon);
        }

        public HttpParser Dot()
        {
            return Bytes(AsciiBytes.Dot);
        }

        public HttpParser Space()
        {
            return Bytes(AsciiBytes.Space);
        }

        public HttpParser SkipSpaces()
        {
            while (IsNext(AsciiBytes.Space))
                Skip(1);
            return this;
        }

        public HttpParser SkipLws()
        {
            while (true)
            {
                IEnumerable<byte> range = Range;
                byte first = range.FirstOrDefault();
                if (first.IsSpaceOrHorizontalTab()) // ( SP | HT )
                    ++_offset;
                else if (first == AsciiBytes.CarriageReturn) // CR
                {
                    range = range.Skip(1);
                    if (range.FirstOrDefault() != AsciiBytes.Linefeed) // CR without LF
                        break;

                    // CRLF
                    range = range.Skip(1);
                    if (!range.FirstOrDefault().IsSpaceOrHorizontalTab()) // CRLF without ( SP | HT )
                        break;

                    // CRLF ( SP | HT )
                    _offset += 3;
                }
                else
                    break;
            }

            return this;
        }

        public HttpParser FieldContent(out IEnumerable<byte> fieldContent)
        {
            int originalOffset = Offset;
            fieldContent = null;
            while (Success)
            {
                fieldContent = new Datagram(_buffer, originalOffset, Offset - originalOffset);

                if (IsNext(AsciiBytes.Space) || IsNext(AsciiBytes.CarriageReturn) || IsNext(AsciiBytes.HorizontalTab) || IsNext(AsciiBytes.Linefeed))
                    break;

                if (IsNext(AsciiBytes.DoubleQuotationMark))
                {
                    Datagram quotedString;
                    QuotedString(out quotedString);
                }
                else
                {
                    var text = Range.TakeWhile(value => value > 0x20 && value != AsciiBytes.DoubleQuotationMark);
                    if (!text.Any())
                        return Fail();
                    _offset += text.Count();
                }
            }
            
            return this;
        }

        public HttpParser FieldValue(out IEnumerable<byte> fieldValue)
        {
            if (!Success)
            {
                fieldValue = null;
                return this;
            }

            SkipLws();

            FieldContent(out fieldValue);
            if (!fieldValue.Any())
                return this;

            while (Success)
            {
                IEnumerable<byte> fieldContent;
                SkipLws();

                FieldContent(out fieldContent);
                if (!fieldContent.Any())
                    break;

                fieldValue = fieldValue.Concat(AsciiBytes.Space).Concat(fieldContent);
            }

            return this;
        }

        public HttpParser RequestUri(out string uri)
        {
            if (!Success)
            {
                uri = null;
                return this;
            }

            var range = Range;
            int numChars = range.TakeWhile(value => value > 32).Count();
            uri = EncodingExtensions.Iso88591.GetString(_buffer, _offset, numChars);
            _offset += numChars;
            return this;
        }

        public HttpParser Version(out HttpVersion version)
        {
            uint? major;
            uint? minor;
            Bytes(_httpSlash).DecimalNumber(out major).Dot().DecimalNumber(out minor);
            version = major != null && minor != null ? new HttpVersion(major.Value, minor.Value) : null;
            return this;
        }

        public HttpParser CarriageReturnLineFeed()
        {
            return Bytes(AsciiBytes.CarriageReturn, AsciiBytes.Linefeed);
        }

        public bool IsCarriageReturnLineFeed()
        {
            return IsNext(AsciiBytes.CarriageReturn) && IsNextNext(AsciiBytes.Linefeed);
        }

        public HttpParser DecimalNumber(int numDigits, out uint? number)
        {
            if (numDigits > 8 || numDigits < 0)
                throw new ArgumentOutOfRangeException("numDigits", numDigits, "Only between 0 and 8 digits are supported");

            if (!Success)
            {
                number = null;
                return this;
            }

            var digits = Range.Take(numDigits).TakeWhile(value => value.IsDigit());
            if (digits.Count() != numDigits)
            {
                number = null;
                return Fail();
            }
            number = digits.Select(value => (uint)(value - AsciiBytes.Zero)).Aggregate<uint, uint>(0, (accumulated, value) => 10 * accumulated + value);
            _offset += numDigits;
            return this;
        }

        public HttpParser DecimalNumber(out uint? number)
        {
            if (!Success)
            {
                number = null;
                return this;
            }

            var digits = Range.TakeWhile(value => value.IsDigit());
            if (!digits.Any())
            {
                number = null;
                return Fail();
            }

            int numDigits = digits.Count();
            int numInsignificantDigits = digits.TakeWhile(value => value == AsciiBytes.Zero).Count();
            if (numDigits - numInsignificantDigits > 9)
            {
                number = null;
                return Fail();
            }

            uint numberValue = digits.Select(value => (uint)(value - AsciiBytes.Zero)).Aggregate<uint, uint>(0, (accumulated, value) => 10 * accumulated + value);
            number = numberValue;
            _offset += numDigits;
            return this;
        }

        public HttpParser HexadecimalNumber(out uint? number)
        {
            if (!Success)
            {
                number = null;
                return this;
            }

            var digits = Range.TakeWhile(value => value.IsHexadecimalDigit());
            if (!digits.Any())
            {
                number = null;
                return Fail();
            }

            int numDigits = digits.Count();
            int numInsignificantDigits = digits.TakeWhile(value => value == AsciiBytes.Zero).Count();
            if (numDigits - numInsignificantDigits > 8)
            {
                number = null;
                return Fail();
            }
            uint numberValue = digits.Select(value => (uint)(value.ToHexadecimalValue())).Aggregate<uint, uint>(0, (accumulated, value) => 16 * accumulated + value);
            number = numberValue;
            _offset += numDigits;
            return this;
        }

        public HttpParser ReasonPhrase(out Datagram reasonPhrase)
        {
            if (!Success)
            {
                reasonPhrase = null;
                return this;
            }

            int count = 0;
            foreach (byte b in Range)
            {
                if (!b.IsControl() || b == AsciiBytes.HorizontalTab)
                    ++count;
                else
                    break;
            }
//            Console.WriteLine(count);
            int reasonPhraseLength = Range.TakeWhile(value => !value.IsControl() || value == AsciiBytes.HorizontalTab).Count();
            reasonPhrase = new Datagram(_buffer, _offset, reasonPhraseLength);
            _offset += reasonPhraseLength;
            return this;
        }

        public HttpParser QuotedString(out Datagram quotedString)
        {
            quotedString = null;
            int startOffset = _offset;

            // Parse first "
            if (!Bytes(AsciiBytes.DoubleQuotationMark).Success)
                return this;  // This cannot happen, since we call this method only when we have '"'.

            while (IsNext())
            {
                byte next = Next();

                // Parse last "
                if (next == AsciiBytes.DoubleQuotationMark)
                {
                    ++_offset;
                    quotedString = new Datagram(_buffer, startOffset, _offset - startOffset);
                    return this;
                }

                // Parse \char
                if (next == AsciiBytes.BackSlash && IsNextNext() && NextNext().IsChar())
                    _offset += 2;
                else
                {
                    // parse text
                    int original = _offset;
                    SkipLws();
                    if (original == _offset)
                    {
                        // text isn't LWS - parse a byte that isn't control
                        if (!next.IsControl())
                            ++_offset;
                        else
                            return Fail(); // illegal byte
                    }
                }
            }

            // no " found
            return Fail();
        }

        public HttpParser SkipChunkExtensions()
        {
            while (Success && IsNext(AsciiBytes.Semicolon))
            {
                Bytes(AsciiBytes.Semicolon);
                
                string chunkExtensionName;
                Token(out chunkExtensionName);
                if (IsNext(AsciiBytes.EqualsSign))
                {
                    Bytes(AsciiBytes.EqualsSign);

                    Datagram chunkExtensionValue;
                    if (IsNext(AsciiBytes.DoubleQuotationMark))
                        QuotedString(out chunkExtensionValue);
                    else
                        Token(out chunkExtensionValue);
                }
            }

            return this;
        }

        private bool IsNext()
        {
            return _offset < _totalLength;
        }

        private bool IsNext(byte next)
        {
            return (IsNext() && Next() == next);
        }

        private bool IsNextNext()
        {
            return _offset + 1 < _totalLength;
        }

        private bool IsNextNext(byte nextNext)
        {
            return (IsNextNext() && NextNext() == nextNext);
        }

        private byte Next()
        {
            return _buffer[_offset];
        }

        private byte NextNext()
        {
            return _buffer[_offset + 1];
        }

        private HttpParser Fail()
        {
            Success = false;
            return this;
        }

        private IEnumerable<byte> Range
        {
            get { return _buffer.Range(_offset, _totalLength - _offset); }
        }

        private HttpParser Bytes(byte value)
        {
            if (!Success)
                return this;

            var range = Range;
            if (!range.Any() || range.First() != value)
                return Fail();

            ++_offset;
            return this;
        }

        private HttpParser Bytes(params byte[] values)
        {
            if (!Success)
                return this;

            if (!Range.Take(values.Length).SequenceEqual(values))
                return Fail();

            _offset += values.Length;
            return this;
        }

        public HttpParser Skip(int count)
        {
            _offset += count;
            if (_offset > _totalLength)
                return Fail();
            return this;
        }

        private static readonly byte[] _httpSlash = Encoding.ASCII.GetBytes("HTTP/");
    
        private readonly byte[] _buffer;
        private int _offset;
        private readonly int _totalLength;
    }
}