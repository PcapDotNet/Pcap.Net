using System.Text;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for Encoding class.
    /// </summary>
    public static class EncodingExtensions
    {
        /// <summary>
        /// ISO-8859-1 Encoding.
        /// </summary>
        public static Encoding Iso88591
        {
            get { return _iso88591; }
        }

        private static readonly Encoding _iso88591 = Encoding.GetEncoding(28591);
    }
}