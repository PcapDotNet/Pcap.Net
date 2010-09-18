using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// Represents an HTTP response layer.
    /// </summary>
    public class HttpResponseLayer : HttpLayer, IEquatable<HttpResponseLayer>
    {
        public override bool IsRequest { get { return false; } }

        public uint? StatusCode { get; set; }

        public Datagram ReasonPhrase { get; set; }
        
        public override bool Equals(HttpLayer other)
        {
            return Equals(other as HttpResponseLayer);
        }

        public bool Equals(HttpResponseLayer other)
        {
            return base.Equals(other) &&
                   StatusCode == other.StatusCode &&
                   (ReferenceEquals(ReasonPhrase, other.ReasonPhrase) || ReasonPhrase.Equals(other.ReasonPhrase));
        }

        protected override int FirstLineLength
        {
            get
            {
                int length = 0;

                if (Version == null)
                    return length;
                length += Version.Length + 1;

                if (StatusCode == null)
                    return length;
                length += StatusCode.Value.DigitsCount(10) + 1;

                if (ReasonPhrase == null)
                    return length;

                return length + ReasonPhrase.Length + 2;
            }
        }

        protected override void WriteFirstLine(byte[] buffer, ref int offset)
        {
            if (Version == null)
                return;
            Version.Write(buffer, ref offset);
            buffer.Write(ref offset, AsciiBytes.Space);

            if (StatusCode == null)
                return;
            buffer.WriteDecimal(ref offset, StatusCode.Value);
            buffer.Write(ref offset, AsciiBytes.Space);

            if (ReasonPhrase == null)
                return;
            buffer.Write(ref offset, ReasonPhrase);

            buffer.WriteCarriageReturnLineFeed(ref offset);
        }
    }
}