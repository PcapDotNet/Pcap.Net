using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    internal static class HttpRegex
    {
        public const string ParameterNameGroupName = "ParameterName";
        public const string ParameterValueGroupName = "ParameterValue";

        public static Regex CarriageReturnLineFeed
        {
            get { return _carriageReturnLineFeed; }
        }

        public static Regex LinearWhiteSpace
        {
            get { return _linearWhiteSpaceRegex; }
        }

        public static Regex Token
        {
            get { return _tokenRegex;}
        }

        public static Regex QuotedString
        {
            get { return _quotedStringRegex; }
        }

        public static Regex OptionalParameters
        {
            get { return _optionalParametersRegex; }
        }

        public static string GetString(byte[] buffer, int offset, int count)
        {
            return _encoding.GetString(buffer, offset, count);
        }

        public static string GetString(byte[] buffer)
        {
            return GetString(buffer, 0, buffer.Length);
        }

//        public static byte[] GetBytes(string pattern)
//        {
//            return _encoding.GetBytes(pattern);
//        }

        public static Regex Build(string pattern)
        {
            return new Regex(Bracket(pattern), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public static Regex Build(char pattern)
        {
            return Build(pattern.ToString());
        }

        public static Regex Concat(params Regex[] regexes)
        {
            return Build(Bracket(regexes.SequenceToString()));
        }

        public static Regex Or(params Regex[] regexes)
        {
            return Build(Bracket(regexes.SequenceToString("|")));
        }

        public static Regex Optional(Regex regex)
        {
            return Build(Bracket(string.Format(CultureInfo.InvariantCulture, "{0}?", regex)));
        }

        public static Regex Any(Regex regex)
        {
            return Build(Bracket(string.Format(CultureInfo.InvariantCulture, "{0}*", regex)));
        }

        public static Regex AtLeastOne(Regex regex)
        {
            return Build(Bracket(string.Format(CultureInfo.InvariantCulture, "{0}+", regex)));
        }

        public static Regex AtLeast(Regex regex, int minCount)
        {
            if (minCount <= 0)
                return Any(regex);
            if (minCount == 1)
                return AtLeastOne(regex);
            return Build(string.Format(CultureInfo.InvariantCulture, "(?:{0}){{{1},}}", regex, minCount));
        }
        
        public static Regex CommaSeparatedRegex(Regex element, int minCount)
        {
            Regex linearWhiteSpacesRegex = Any(LinearWhiteSpace);
            Regex regex = Concat(Build(string.Format(CultureInfo.InvariantCulture, "{0}{1}", linearWhiteSpacesRegex, element)),
                                 AtLeast(Build(string.Format(CultureInfo.InvariantCulture, "{0},{1}{2}", linearWhiteSpacesRegex, linearWhiteSpacesRegex, element)), minCount - 1));
            return minCount <= 0 ? Optional(regex) : regex;
        }

        public static Regex Capture(Regex regex, string captureName)
        {
            return Build(string.Format(CultureInfo.InvariantCulture, "(?<{0}>{1})", captureName, regex));
        }

        public static Regex MatchEntire(Regex regex)
        {
            return Build(string.Format(CultureInfo.InvariantCulture, "^{0}$", regex));
        }

        private static string Bracket(string pattern)
        {
            return string.Format(CultureInfo.InvariantCulture, "(?:{0})", pattern);
        }

        private static readonly Regex _carriageReturnLineFeed = Build(@"\r\n");
        private static readonly Regex _charRegex = Build(@"[\x00-\x7F]");
        private static readonly Regex _quotedPairRegex = Concat(Build(@"\\"), _charRegex);
        private static readonly Regex _linearWhiteSpaceRegex = Concat(Optional(CarriageReturnLineFeed), AtLeastOne(Build(@"[ \t]")));
        private static readonly Regex _qdtextRegex = Or(_linearWhiteSpaceRegex, Build(@"[^\x00-\x1F\x7F""]"));
        private static readonly Regex _quotedStringRegex = Concat(Build('"'), Any(Or(_qdtextRegex, _quotedPairRegex)), Build('"'));
        private static readonly Regex _tokenRegex = AtLeastOne(Build(@"[\x21\x23-\x27\x2A\x2B\x2D\x2E0-9A-Z\x5E-\x7A\x7C\x7E-\xFE]"));
        private static readonly Regex _valueRegex = Or(Token, QuotedString); 
        private static readonly Regex _parameterRegex = Concat(Capture(_tokenRegex, ParameterNameGroupName), Build("="), Capture(_valueRegex, ParameterValueGroupName));
        private static readonly Regex _optionalParametersRegex = Any(Concat(Build(";"), Optional(_linearWhiteSpaceRegex), _parameterRegex));

        private static readonly Encoding _encoding = Encoding.GetEncoding(28591);
    }
}