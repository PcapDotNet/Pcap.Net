using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    internal class HttpParser
    {
        public HttpParser(byte[] buffer, int offset, int length)
        {
            _buffer = buffer;
            _offset = offset;
            _totalLength = offset + length;
            Success = offset >= 0 && length >= 0 && offset + length <= buffer.Length;
        }

        public bool Success { get; private set; }

        public HttpParser Token(out string token)
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

        private HttpParser Single(byte value)
        {
            if (!Success)
                return this;

            if (Range.First() != value)
                return Fail();

            ++_offset;
            return this;
        }

        public HttpParser Colon()
        {
            return Single(AsciiBytes.Colon);
        }

        public HttpParser Space()
        {
            return Single(AsciiBytes.Space);
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
                    if (range.FirstOrDefault() != AsciiBytes.LineFeed) // CR without LF
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
            fieldContent = Range.TakeWhile(value => !value.IsSpaceOrHorizontalTab() && value != AsciiBytes.CarriageReturn);
            _offset += fieldContent.Count();
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

        private HttpParser Fail()
        {
            Success = false;
            return this;
        }

        private IEnumerable<byte> Range
        {
            get { return _buffer.Range(_offset, _totalLength - _offset); }
        }

        private readonly byte[] _buffer;
        private int _offset;
        private readonly int _totalLength;

        public HttpParser RequestUri()
        {
            throw new NotImplementedException();
        }

        public HttpParser Version()
        {
            throw new NotImplementedException();
        }

        public HttpParser CarraigeReturnLineFeed()
        {
            return Single(AsciiBytes.CarriageReturn).Single(AsciiBytes.LineFeed);
        }

        public HttpParser DecimalNumber(int numDigits, out int number)
        {
            var digits = Range.Take(numDigits).TakeWhile(value => value.IsDigit());
            if (digits.Count() != numDigits)
            {
                number = 0;
                return Fail();
            }
            number = digits.Select(value => value - AsciiBytes.Zero).Aggregate(0, (accumulated, value) => 10 * accumulated + value);
            _offset += numDigits;
            return this;
        }

        public HttpParser ReasonPhrase()
        {
            throw new NotImplementedException();
        }
    }
}