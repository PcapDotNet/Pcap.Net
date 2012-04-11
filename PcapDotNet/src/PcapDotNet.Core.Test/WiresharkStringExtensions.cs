using System.Linq;
using System.Text;

namespace PcapDotNet.Core.Test
{
    public static class WiresharkStringExtensions
    {
        public static string ToWiresharkLiteral(this string value, bool putLeadingZerosInHexAndBackslashesBeforeSpecialCharacters = true, bool escapeSpecialChars = true)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i != value.Length; ++i)
            {
                char currentChar = value[i];
                if (currentChar == '\0')
                    return result.ToString();
                if (escapeSpecialChars)
                {
                    switch (currentChar)
                    {
                        case '\\':
                        case '"':
                            result.Append('\\');
                            result.Append(currentChar);
                            continue;

                        case '\r':
                            result.Append(@"\r");
                            continue;

                        case '\n':
                            result.Append(@"\n");
                            continue;
                    }
                }
 
                if (currentChar >= 0x7F || currentChar < 0x20)
                {
                    result.Append(@"\x");
                    result.Append(((int)currentChar).ToString("x" + (putLeadingZerosInHexAndBackslashesBeforeSpecialCharacters ? "2" : "")));
                }
                else
                    result.Append(currentChar);
            }

            return result.ToString();
        }

        public static string ToWiresharkLowerLiteral(this string literalString)
        {
            literalString = literalString.ToLowerInvariant();
            foreach (byte charValue in Enumerable.Range(0xC0, 0xD7 - 0xC0).Concat(Enumerable.Range(0xD8, 0xDF - 0xD8)))
                literalString = literalString.Replace(@"\x" + charValue.ToString("x"), @"\x" + (charValue + 0x20).ToString("x"));

            return literalString;
        }
    }
}