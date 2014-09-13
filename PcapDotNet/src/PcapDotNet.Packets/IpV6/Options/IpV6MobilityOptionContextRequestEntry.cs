using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+----------+------------+
    /// | Bit | 0-7      | 8-15       |
    /// +-----+----------+------------+
    /// | 0   | Req-type | Req-length |
    /// +-----+----------+------------+
    /// | 16  | Req-option            |
    /// | ... |                       |
    /// +-----+-----------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6MobilityOptionContextRequestEntry : IEquatable<IpV6MobilityOptionContextRequestEntry>
    {
        /// <summary>
        /// Creates a context request entry according to the given request type and option.
        /// </summary>
        /// <param name="requestType">The type value for the requested option.</param>
        /// <param name="option">The optional data to uniquely identify the requested context for the requested option.</param>
        public IpV6MobilityOptionContextRequestEntry(byte requestType, DataSegment option)
        {
            if (option.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("option", option, string.Format("Option length must not exceed {0}", byte.MaxValue));
            RequestType = requestType;
            Option = option;
        }

        /// <summary>
        /// The total length of the request in bytes.
        /// </summary>
        public int Length
        {
            get { return sizeof(byte) + sizeof(byte) + OptionLength; }
        }

        /// <summary>
        /// The type value for the requested option.
        /// </summary>
        public byte RequestType { get; private set; }

        /// <summary>
        /// The length of the requested option, excluding the Request Type and Request Length fields.
        /// </summary>
        public byte OptionLength
        {
            get { return (byte)Option.Length; }
        }

        /// <summary>
        /// The optional data to uniquely identify the requested context for the requested option.
        /// </summary>
        public DataSegment Option { get; private set; }

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RequestType);
            buffer.Write(ref offset, OptionLength);
            Option.Write(buffer, ref offset);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV6MobilityOptionContextRequestEntry);
        }

        public bool Equals(IpV6MobilityOptionContextRequestEntry other)
        {
            return (other != null && RequestType.Equals(other.RequestType) && Option.Equals(other.Option));
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(RequestType, Option);
        }
    }
}