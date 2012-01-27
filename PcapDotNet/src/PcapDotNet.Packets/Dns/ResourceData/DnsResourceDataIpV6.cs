using System;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 3596.
    /// <pre>
    /// +-----+-------+
    /// | bit | 0-127 |
    /// +-----+-------+
    /// | 0   | IP    |
    /// +-----+-------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Aaaa)]
    public sealed class DnsResourceDataIpV6 : DnsResourceDataSimple, IEquatable<DnsResourceDataIpV6>
    {
        public DnsResourceDataIpV6(IpV6Address data)
        {
            Data = data;
        }

        public IpV6Address Data { get; private set; }

        public bool Equals(DnsResourceDataIpV6 other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataIpV6);
        }

        internal DnsResourceDataIpV6()
            : this(IpV6Address.Zero)
        {
        }

        internal override int GetLength()
        {
            return IpV6Address.SizeOf;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data, Endianity.Big);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length != IpV6Address.SizeOf)
                return null;
            return new DnsResourceDataIpV6(data.ReadIpV6Address(0, Endianity.Big));
        }
    }
}