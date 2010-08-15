namespace PcapDotNet.Base
{
    public static class CharExtensions
    {
        public static bool IsUpperCaseAlpha(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }
    }
}