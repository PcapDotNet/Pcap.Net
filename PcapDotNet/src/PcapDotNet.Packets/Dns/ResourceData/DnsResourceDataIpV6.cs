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
        /// <summary>
        /// Constructs an IPv6 resource data from the given IP.
        /// </summary>
        public DnsResourceDataIpV6(IpV6Address data)
        {
            Data = data;
        }

        /// <summary>
        /// The IPv6 value.
        /// </summary>
        public IpV6Address Data { get; private set; }

        /// <summary>
        /// Two IPv6 resource datas are equal if their IP is equal.
        /// </summary>
        public bool Equals(DnsResourceDataIpV6 other)
        {
            return other != null && Data.Equals(other.Data);
        }

        /// <summary>
        /// Two IPv6 resource datas are equal if their IP is equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataIpV6);
        }

        /// <summary>
        /// Returns the hash code of the IPv6 value.
        /// </summary>
        public override int GetHashCode()
        {
            return Data.GetHashCode();
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