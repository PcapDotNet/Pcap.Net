using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A DNS string.
    /// Any segment of bytes up to 255 characters is valid.
    /// The format is one byte for the length of the string and then the specified number of bytes.
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.X25)]
    public sealed class DnsResourceDataString : DnsResourceDataSimple, IEquatable<DnsResourceDataString>
    {
        public DnsResourceDataString(DataSegment value)
        {
            String = value;
        }

        public DataSegment String { get; private set; }

        public bool Equals(DnsResourceDataString other)
        {
            return other != null &&
                   String.Equals(other.String);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataString);
        }

        public override int GetHashCode()
        {
            return String.GetHashCode();
        }

        internal DnsResourceDataString()
            : this(DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return GetStringLength(String);
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            WriteString(buffer, ref offset, String);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            int offset = 0;
            DataSegment str = ReadString(data, ref offset);
            if (str == null)
                return null;
            return new DnsResourceDataString(str);
        }
    }
}