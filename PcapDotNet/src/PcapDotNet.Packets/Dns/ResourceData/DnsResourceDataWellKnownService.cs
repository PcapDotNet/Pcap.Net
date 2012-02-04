using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// <pre>
    /// +-----+----------+---------+
    /// | bit | 0-7      | 8-31    |
    /// +-----+----------+---------+
    /// | 0   | Address            |
    /// +-----+----------+---------+
    /// | 32  | Protocol | Bit Map | (Bit Map is variable multiple of 8 bits length)
    /// +-----+----------+---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Wks)]
    public sealed class DnsResourceDataWellKnownService : DnsResourceDataSimple, IEquatable<DnsResourceDataWellKnownService>
    {
        private static class Offset
        {
            public const int Address = 0;
            public const int Protocol = Address + IpV4Address.SizeOf;
            public const int BitMap = Protocol + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.BitMap;

        public DnsResourceDataWellKnownService(IpV4Address address, IpV4Protocol protocol, DataSegment bitMap)
        {
            Address = address;
            Protocol = protocol;
            BitMap = bitMap;
        }

        /// <summary>
        /// The service address.
        /// </summary>
        public IpV4Address Address { get; private set; }

        /// <summary>
        /// Specifies an IP protocol number.
        /// </summary>
        public IpV4Protocol Protocol { get; private set; }

        /// <summary>
        /// Has one bit per port of the specified protocol.
        /// </summary>
        public DataSegment BitMap { get; private set; }

        public bool Equals(DnsResourceDataWellKnownService other)
        {
            return other != null &&
                   Address.Equals(other.Address) &&
                   Protocol.Equals(other.Protocol) &&
                   BitMap.Equals(other.BitMap);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataWellKnownService);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Address, Protocol, BitMap);
        }

        internal DnsResourceDataWellKnownService()
            : this(IpV4Address.Zero, IpV4Protocol.Ip, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + BitMap.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            buffer.Write(offset + Offset.Protocol, (byte)Protocol);
            BitMap.Write(buffer, offset + Offset.BitMap);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            IpV4Address address = data.ReadIpV4Address(Offset.Address, Endianity.Big);
            IpV4Protocol protocol = (IpV4Protocol)data[Offset.Protocol];
            DataSegment bitMap = data.SubSegment(Offset.BitMap, data.Length - Offset.BitMap);

            return new DnsResourceDataWellKnownService(address, protocol, bitMap);
        }
    }
}