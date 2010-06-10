namespace PcapDotNet.Packets.Http
{
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