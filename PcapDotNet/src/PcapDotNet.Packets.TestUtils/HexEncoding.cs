using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.TestUtils
{
    [ExcludeFromCodeCoverage]
    public sealed class HexEncoding : Encoding 
    {
        public static HexEncoding Instance { get { return _instance; } }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            int numHexChars = 0;
            // remove all none A-F, 0-9, characters
            for (int i=0; i<count; ++i)
            {
                if (IsHexDigit(chars[index+i]))
                    ++numHexChars;
            }
            // if odd number of characters, discard last character
            return numHexChars / 2; // 2 characters per byte
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            int originalByteIndex = byteIndex;
            IEnumerable<char> hexChars = Enumerable.Where<char>(chars.Range(charIndex, charCount), IsHexDigit);
            IEnumerator<char> hexCharsEnumerator = hexChars.GetEnumerator();
            while (hexCharsEnumerator.MoveNext())
            {
                char firstChar = hexCharsEnumerator.Current;
                if (!hexCharsEnumerator.MoveNext())
                    break;
                char secondChar = hexCharsEnumerator.Current;
                bytes[byteIndex++] = HexToByte(firstChar, secondChar);
            }

            return byteIndex - originalByteIndex;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count * 2;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int originalCharIndex = charIndex;
            foreach (byte b in bytes.Range(byteIndex, byteCount))
            {
                ByteToHex(b, out chars[charIndex], out chars[charIndex + 1]);
                charIndex += 2;
            }
            return charIndex - originalCharIndex;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount / 2;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount * 2;
        }

        private HexEncoding()
        {
        }

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9' ||
                    c >= 'a' && c <= 'f' ||
                    c >= 'A' && c <= 'F');
        }

        private static byte HexToByte(char mostSignificantDigit, char leastSignificantDigit)
        {
            return (byte)(DigitToByte(mostSignificantDigit) * 16 + DigitToByte(leastSignificantDigit));
        }

        private void ByteToHex(byte b, out char mostSignificantDigit, out char leastSignificantDigit)
        {
            mostSignificantDigit = ByteToDigit((byte)(b / 16));
            leastSignificantDigit = ByteToDigit((byte)(b % 16));
        }

        private static byte DigitToByte(char digit)
        {
            if (digit >= '0' && digit <= '9')
                return (byte)(digit - '0');

            if (digit >= 'a' && digit <= 'f')
                return (byte)(digit - 'a' + 10);

            if (digit >= 'A' && digit <= 'F')
                return (byte)(digit - 'A' + 10);

            throw new ArgumentOutOfRangeException("digit", digit, "digit is not a legal hexadecimal character");
        }

        private static char ByteToDigit(byte b)
        {
            if (b <= 9)
                return (char)('0' + b);

            if (b <= 15)
                return (char)('A' + b);

           throw new ArgumentOutOfRangeException("b", b, "value must be between 0 and 15");
        }

        private static readonly HexEncoding _instance = new HexEncoding();
    }
}