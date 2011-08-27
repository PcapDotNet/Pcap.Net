using System;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// Represents an HTTP layer.
    /// </summary>
    public abstract class HttpLayer : SimpleLayer, IEquatable<HttpLayer>
    {
        /// <summary>
        /// True iff the message is a request and iff the message is not a response.
        /// </summary>
        public abstract bool IsRequest { get; }

        /// <summary>
        /// True iff the message is a response and iff the message is not a request.
        /// </summary>
        public bool IsResponse { get { return !IsRequest; } }

        /// <summary>
        /// The version of this HTTP message.
        /// </summary>
        public HttpVersion Version { get; set; }

        /// <summary>
        /// The header of the HTTP message.
        /// </summary>
        public HttpHeader Header { get; set; }

        /// <summary>
        /// Message Body.
        /// </summary>
        public Datagram Body { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public sealed override int Length
        {
            get
            {
                return FirstLineLength +
                       (Header == null ? 0 : Header.BytesLength) +
                       (Body == null ? 0 : Body.Length);
            }
        }

        /// <summary>
        /// Two HTTP layers are equal iff they have the same version, header and body.
        /// Extended by specific HTTP layers types for more checks.
        /// </summary>
        public sealed override bool Equals(Layer other)
        {
            return Equals(other as HttpLayer);
        }

        /// <summary>
        /// Two HTTP layers are equal iff they have the same version, header and body.
        /// Extended by specific HTTP layers types for more checks.
        /// </summary>
        public virtual bool Equals(HttpLayer other)
        {
            return other != null &&
                   (ReferenceEquals(Version, other.Version) || Version.Equals(other.Version)) &&
                   (ReferenceEquals(Header, other.Header) || Header.Equals(other.Header)) &&
                   (ReferenceEquals(Body, other.Body) || Body.Equals(other.Body));
        }

        /// <summary>
        /// Writes the HTTP layer to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected sealed override void Write(byte[] buffer, int offset)
        {
            WriteFirstLine(buffer, ref offset);
            if (Header != null)
                Header.Write(buffer, ref offset);
            if (Body != null)
                buffer.Write(offset, Body);
        }

        internal  abstract int FirstLineLength { get; }
        internal abstract void WriteFirstLine(byte[] buffer, ref int offset);
    }
}