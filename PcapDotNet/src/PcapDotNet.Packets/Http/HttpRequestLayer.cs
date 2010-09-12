using System;
using System.Text;

namespace PcapDotNet.Packets.Http
{
    public class HttpRequestLayer : HttpLayer, IEquatable<HttpRequestLayer>
    {
        public override bool IsRequest { get { return true; } }

        public HttpRequestMethod Method { get; set; }

        public string Uri { get; set; }

        public override bool Equals(HttpLayer other)
        {
            return Equals(other as HttpRequestLayer);
        }

        public bool Equals(HttpRequestLayer other)
        {
            return base.Equals(other) &&
                   (ReferenceEquals(Method, other.Method) || Method.Equals(other.Method)) &&
                   (ReferenceEquals(Uri, other.Uri) || Uri.Equals(other.Uri));
        }

        protected override int FirstLineLength
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

        protected override void WriteFirstLine(byte[] buffer, ref int offset)
        {
            if (Method == null)
                return;
            Method.Write(buffer, ref offset);
            buffer.Write(ref offset, AsciiBytes.Space);

            if (Uri == null)
                return;
            buffer.Write(ref offset, Uri, Encoding.ASCII);
            buffer.Write(ref offset, AsciiBytes.Space);

            if (Version == null)
                return;
            Version.Write(buffer, ref offset);
            buffer.WriteCarriageReturnLineFeed(ref offset);
        }
    }
}