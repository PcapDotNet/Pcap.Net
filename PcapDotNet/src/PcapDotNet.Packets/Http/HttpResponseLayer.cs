using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// Represents an HTTP response layer.
    /// </summary>
    public sealed class HttpResponseLayer : HttpLayer, IEquatable<HttpResponseLayer>
    {
        /// <summary>
        /// false since this is a response.
        /// </summary>
        public override bool IsRequest { get { return false; } }

        /// <summary>
        /// The status code of the response.
        /// null if no status code exists.
        /// </summary>
        public uint? StatusCode { get; set; }

        /// <summary>
        /// The data of the reason phrase.
        /// Example: OK
        /// </summary>
        public Datagram ReasonPhrase { get; set; }

        /// <summary>
        /// Two HTTP response layers are equal iff they have the same version, header, body, status code and reason phrase.
        /// </summary>
        public override bool Equals(HttpLayer other)
        {
            return Equals(other as HttpResponseLayer);
        }

        /// <summary>
        /// Two HTTP response layers are equal iff they have the same version, header, body, status code and reason phrase.
        /// </summary>
        public bool Equals(HttpResponseLayer other)
        {
            return base.Equals(other) &&
                   StatusCode == other.StatusCode &&
                   (ReferenceEquals(ReasonPhrase, other.ReasonPhrase) || ReasonPhrase.Equals(other.ReasonPhrase));
        }

        internal override int FirstLineLength
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

        internal override void WriteFirstLine(byte[] buffer, ref int offset)
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

            buffer.WriteCarriageReturnLinefeed(ref offset);
        }
    }
}