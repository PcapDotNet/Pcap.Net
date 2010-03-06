using System;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 2521.
    /// <pre>
    /// +-----+------+------+----------+
    /// | Bit | 0-7  | 8-15 | 16-31    |
    /// +-----+------+------+----------+
    /// | 0   | Type | Code | Checksum |
    /// +-----+------+------+----------+
    /// | 32  | Reserved    | Pointer  |
    /// +-----+-------------+----------+
    /// | 64  | Internet Header        |
    /// |     | + 64 bits of           |
    /// |     | Original Data Datagram |
    /// +-----+------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.SecurityFailures)]
    public class IcmpSecurityFailuresDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        private static class Offset
        {
            public const int Pointer = 6;
        }

        /// <summary>
        /// An offset into the Original Internet Headers that locates the most significant octet of the offending SPI.  
        /// Will be zero when no SPI is present.
        /// </summary>
        public ushort Pointer
        {
            get { return ReadUShort(Offset.Pointer, Endianity.Big); }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpSecurityFailuresLayer
            {
                Code = (IcmpCodeSecurityFailure)Code,
                Checksum = Checksum,
                Pointer = Pointer
            };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct, the code is in the expected range,
        /// the IPv4 payload contains at least an IPv4 header, the IPv4's payload is in the expected size and the Pointer points to a byte in the IPv4 payload.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Pointer < IpV4.Length;
        }

        /// <summary>
        /// The minimum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected override byte MinCodeValue
        {
            get { return _minCode; }
        }

        /// <summary>
        /// The maximum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected override byte MaxCodeValue
        {
            get { return _maxCode; }
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpSecurityFailuresDatagram(buffer, offset, length);
        }

        private IcmpSecurityFailuresDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeSecurityFailure).GetEnumValues<IcmpCodeSecurityFailure>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeSecurityFailure).GetEnumValues<IcmpCodeSecurityFailure>().Max();
    }
}