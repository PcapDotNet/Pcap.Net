using System;

namespace PcapDotNet.Packets.Http
{
    public abstract class HttpLayer : SimpleLayer, IEquatable<HttpLayer>
    {
        public HttpVersion Version { get; set; }
        public HttpHeader Header { get; set; }
        public Datagram Body { get; set; }

        public override int Length
        {
            get
            {
                return FirstLineLength +
                       (Header == null ? 0 : Header.BytesLength) +
                       (Body == null ? 0 : Body.Length);
            }
        }

        public override bool Equals(Layer other)
        {
            return Equals(other as HttpLayer);
        }

        public virtual bool Equals(HttpLayer other)
        {
            return other != null &&
                   (ReferenceEquals(Version, other.Version) || Version.Equals(other.Version)) &&
                   (ReferenceEquals(Header, other.Header) || Header.Equals(other.Header)) &&
                   (ReferenceEquals(Body, other.Body) || Body.Equals(other.Body));
        }

        protected override void Write(byte[] buffer, int offset)
        {
            WriteFirstLine(buffer, ref offset);
            if (Header != null)
                Header.Write(buffer, ref offset);
            if (Body != null)
                buffer.Write(offset, Body);
        }

        protected abstract int FirstLineLength { get; }
        protected abstract void WriteFirstLine(byte[] buffer, ref int offset);
    }
}