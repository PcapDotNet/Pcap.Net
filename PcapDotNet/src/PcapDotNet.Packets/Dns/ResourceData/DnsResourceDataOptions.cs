using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2671.
    /// <pre>
    /// 0 Or more of:
    /// +-----+---------------+
    /// | bit | 0-15          |
    /// +-----+---------------+
    /// | 0   | OPTION-CODE   |
    /// +-----+---------------+
    /// | 16  | OPTION-LENGTH |
    /// +-----+---------------+
    /// | 32  | OPTION-DATA   |
    /// | ... |               |
    /// +-----+---------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Opt)]
    public sealed class DnsResourceDataOptions : DnsResourceDataSimple, IEquatable<DnsResourceDataOptions>
    {
        public DnsResourceDataOptions(DnsOptions options)
        {
            Options = options;
        }

        public DnsOptions Options { get; private set; }

        public bool Equals(DnsResourceDataOptions other)
        {
            return other != null &&
                   Options.Equals(other.Options);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataOptions);
        }

        public override int GetHashCode()
        {
            return Options.GetHashCode();
        }

        internal DnsResourceDataOptions()
            : this(DnsOptions.None)
        {
        }

        internal override int GetLength()
        {
            return Options.BytesLength;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            Options.Write(buffer, offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsOptions options = DnsOptions.Read(data);
            if (options == null)
                return null;
            return new DnsResourceDataOptions(options);
        }
    }
}