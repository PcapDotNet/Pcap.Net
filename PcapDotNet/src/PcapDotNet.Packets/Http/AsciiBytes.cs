namespace PcapDotNet.Packets.Http
{
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
        public const byte Asterisk = (byte)'*';
        public const byte Dot = (byte)'.';
    }
}