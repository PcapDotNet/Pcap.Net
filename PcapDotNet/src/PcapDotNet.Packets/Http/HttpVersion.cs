using System;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpVersion : IEquatable<HttpVersion>
    {
        public HttpVersion(uint major, uint minor)
        {
            Major = major;
            Minor = minor;
        }

        public static HttpVersion Version10 { get { return _version10; } }
        public static HttpVersion Version11 { get { return _version11; } }

        public uint Major { get; private set; }
        public uint Minor { get; private set; }

        public int Length
        {
            get { return _httpSlashBytes.Length + Major.NumDigits(10) + 1 + Minor.NumDigits(10); }
        }

        public override string ToString()
        {
            return string.Format("HTTP/{0}.{1}", Major, Minor);
        }

        public bool Equals(HttpVersion other)
        {
            return other != null &&
                   Major == other.Major &&
                   Minor == other.Minor;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpVersion);
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