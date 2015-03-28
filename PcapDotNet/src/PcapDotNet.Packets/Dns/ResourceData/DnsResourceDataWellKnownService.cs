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
    [DnsTypeRegistration(Type = DnsType.WellKnownService)]
    public sealed class DnsResourceDataWellKnownService : DnsResourceDataSimple, IEquatable<DnsResourceDataWellKnownService>
    {
        private static class Offset
        {
            public const int Address = 0;
            public const int Protocol = Address + IpV4Address.SizeOf;
            public const int Bitmap = Protocol + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.Bitmap;

        /// <summary>
        /// Constructs an instance from the address, protocol and bitmap fields.
        /// </summary>
        /// <param name="address">The service address.</param>
        /// <param name="protocol">Specifies an IP protocol number.</param>
        /// <param name="bitmap">Has one bit per port of the specified protocol.</param>
        public DnsResourceDataWellKnownService(IpV4Address address, IpV4Protocol protocol, DataSegment bitmap)
        {
            Address = address;
            Protocol = protocol;
            Bitmap = bitmap;
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
        public DataSegment Bitmap { get; private set; }

        /// <summary>
        /// Two DnsResourceDataWellKnownService are equal iff their address, protocol and bitmap fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataWellKnownService other)
        {
            return other != null &&
                   Address.Equals(other.Address) &&
                   Protocol.Equals(other.Protocol) &&
                   Bitmap.Equals(other.Bitmap);
        }

        /// <summary>
        /// Two DnsResourceDataWellKnownService are equal iff their address, protocol and bitmap fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataWellKnownService);
        }
        
        /// <summary>
        /// A hash code based on the address, protocol and bitmap fields.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Address, Protocol, Bitmap);
        }

        internal DnsResourceDataWellKnownService()
            : this(IpV4Address.Zero, IpV4Protocol.Ip, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Bitmap.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            buffer.Write(offset + Offset.Protocol, (byte)Protocol);
            Bitmap.Write(buffer, offset + Offset.Bitmap);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            IpV4Address address = data.ReadIpV4Address(Offset.Address, Endianity.Big);
            IpV4Protocol protocol = (IpV4Protocol)data[Offset.Protocol];
            DataSegment bitmap = data.Subsegment(Offset.Bitmap, data.Length - Offset.Bitmap);

            return new DnsResourceDataWellKnownService(address, protocol, bitmap);
        }
    }
}