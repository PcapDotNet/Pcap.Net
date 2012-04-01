using System;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// Represents an HTTP request layer.
    /// </summary>
    public sealed class HttpRequestLayer : HttpLayer, IEquatable<HttpRequestLayer>
    {
        /// <summary>
        /// True since the message is a request.
        /// </summary>
        public override bool IsRequest { get { return true; } }

        /// <summary>
        /// The HTTP Request Method.
        /// </summary>
        public HttpRequestMethod Method { get; set; }

        /// <summary>
        /// The HTTP Request URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Two HTTP Request layers are equal iff they have the same version, header, body, method and uri.
        /// </summary>
        public override bool Equals(HttpLayer other)
        {
            return Equals(other as HttpRequestLayer);
        }

        /// <summary>
        /// Two HTTP Request layers are equal iff they have the same version, header, body, method and uri.
        /// </summary>
        public bool Equals(HttpRequestLayer other)
        {
            return base.Equals(other) &&
                   (ReferenceEquals(Method, other.Method) || Method.Equals(other.Method)) &&
                   (ReferenceEquals(Uri, other.Uri) || Uri.Equals(other.Uri));
        }

        internal override int FirstLineLength
        {
            get
            {
                int length = 0;
                if (Method == null)
                    return length;
                length += Method.Length + 1;

                if (Uri == null)
                    return length;
                length += Uri.Length + 1;

                if (Version == null)
                    return length;
                return length + Version.Length + 2;
            }
        }

        internal override void WriteFirstLine(byte[] buffer, ref int offset)
        {
            if (Method == null)
                return;
            Method.Write(buffer, ref offset);
            buffer.Write(ref offset, AsciiBytes.Space);

            if (Uri == null)
                return;
            buffer.Write(ref offset, Uri, EncodingExtensions.Iso88591);
            buffer.Write(ref offset, AsciiBytes.Space);

            if (Version == null)
                return;
            Version.Write(buffer, ref offset);
            buffer.WriteCarriageReturnLinefeed(ref offset);
        }
    }
}