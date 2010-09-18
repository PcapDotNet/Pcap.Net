namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for char structure.
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// True iff the given character is one of the capital english letters.
        /// </summary>
        /// <param name="character">The input character to check.</param>
        /// <returns>True for capital english letters.</returns>
        public static bool IsUppercaseAlpha(this char character)
        {
            return character >= 'A' && character <= 'Z';
        }
    }
}