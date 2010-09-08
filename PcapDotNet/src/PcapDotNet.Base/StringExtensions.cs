using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

namespace PcapDotNet.Base
{
    public static class StringExtensions
    {
        public static string ToLiteral(this string value)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i != value.Length; ++i)
            {
                char currentChar = value[i];
                switch (currentChar)
                {
                    case '\\':
                    case '"':
                        result.Append('\\');
                        result.Append(currentChar);
                        break;

                    case '\t':
//                        result.Append(@"\t");
                        result.Append(@"\x9");
                        break;

                    case '\r':
                        result.Append(@"\r");
                        break;

                    case '\n':
                        result.Append(@"\n");
                        break;

                    default:
                        if (currentChar >= 0x7F || currentChar < 0x20)
                        {
                            result.Append(@"\x");
                            result.Append(((int)currentChar).ToString("x"));
                            break;
                        }

                        result.Append(currentChar);
                        break;
                }
            }

            return result.ToString();
        }

        public static string ToLowerLiteral(this string literalString)
        {
            literalString = literalString.ToLowerInvariant();
            foreach (byte charValue in Enumerable.Range(0xC0, 0xD7 - 0xC0).Concat(Enumerable.Range(0xD8, 0xDF - 0xD8)))
                literalString = literalString.Replace(@"\x" + charValue.ToString("x"), @"\x" + (charValue + 0x20).ToString("x"));

            return literalString;
        }
    }
}