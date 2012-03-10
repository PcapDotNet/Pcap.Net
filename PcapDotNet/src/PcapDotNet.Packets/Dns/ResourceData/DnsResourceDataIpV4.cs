using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// <pre>
    /// +-----+------+
    /// | bit | 0-31 |
    /// +-----+------+
    /// | 0   | IP   |
    /// +-----+------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.A)]
    public sealed class DnsResourceDataIpV4 : DnsResourceDataSimple, IEquatable<DnsResourceDataIpV4>
    {
        /// <summary>
        /// Constructs an IPv4 resource data from the given IP.
        /// </summary>
        public DnsResourceDataIpV4(IpV4Address data)
        {
            Data = data;
        }

        /// <summary>
        /// The IPv4 value.
        /// </summary>
        public IpV4Address Data { get; private set; }

        /// <summary>
        /// Two IPv4 resource datas are equal if their IP is equal.
        /// </summary>
        public bool Equals(DnsResourceDataIpV4 other)
        {
            return other != null && Data.Equals(other.Data);
        }

        /// <summary>
        /// Two IPv4 resource datas are equal if their IP is equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataIpV4);
        }

        /// <summary>
        /// Returns the hash code of the IPv4 value.
        /// </summary>
        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

        internal DnsResourceDataIpV4()
            : this(IpV4Address.Zero)
        {
        }

        internal override int GetLength()
        {
            return IpV4Address.SizeOf;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data, Endianity.Big);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length != IpV4Address.SizeOf)
                return null;
            return new DnsResourceDataIpV4(data.ReadIpV4Address(0, Endianity.Big));
        }
    }
}