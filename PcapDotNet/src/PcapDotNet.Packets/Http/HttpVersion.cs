using System;
using System.Globalization;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// Represents an HTTP version.
    /// </summary>
    public sealed class HttpVersion : IEquatable<HttpVersion>
    {
        /// <summary>
        /// Creates a version from the major and minor version numbers.
        /// </summary>
        /// <param name="major">The major version number. 0 for 0.9, 1 for 1.0 or 1.1.</param>
        /// <param name="minor">The minor version number. 9 for 0.9, 0, for 1.0 and 1 for 1.1.</param>
        public HttpVersion(uint major, uint minor)
        {
            Major = major;
            Minor = minor;
        }

        /// <summary>
        /// A built version for HTTP/1.0.
        /// </summary>
        public static HttpVersion Version10 { get { return _version10; } }

        /// <summary>
        /// A built version for HTTP/1.1.
        /// </summary>
        public static HttpVersion Version11 { get { return _version11; } }

        /// <summary>
        /// The major version number.
        /// </summary>
        public uint Major { get; private set; }

        /// <summary>
        /// The minor version number.
        /// </summary>
        public uint Minor { get; private set; }

        /// <summary>
        /// The number of bytes this version takes.
        /// </summary>
        public int Length
        {
            get { return _httpSlashBytes.Length + Major.DigitsCount(10) + 1 + Minor.DigitsCount(10); }
        }

        /// <summary>
        /// A string represneting the version.
        /// Example: &quot;HTTP/1.1&quot;.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "HTTP/{0}.{1}", Major, Minor);
        }

        /// <summary>
        /// Two HTTP versions are equal iff they have the same major and minor versions.
        /// </summary>
        public bool Equals(HttpVersion other)
        {
            return other != null &&
                   Major == other.Major &&
                   Minor == other.Minor;
        }

        /// <summary>
        /// Two HTTP versions are equal iff they have the same major and minor versions.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as HttpVersion);
        }

        /// <summary>
        /// The hash code of an http version is the xor of the hash codes of the minor version and the major version.
        /// </summary>
        public override int GetHashCode()
        {
            return Minor.GetHashCode() ^ Major.GetHashCode();
        }

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, _httpSlashBytes);
            buffer.WriteDecimal(ref offset, Major);
            buffer.Write(ref offset, AsciiBytes.Dot);
            buffer.WriteDecimal(ref offset, Minor);
        }

        private static readonly HttpVersion _version10 = new HttpVersion(1,0);
        private static readonly HttpVersion _version11 = new HttpVersion(1,1);
        private static readonly byte[] _httpSlashBytes = Encoding.ASCII.GetBytes("HTTP/");
    }
}