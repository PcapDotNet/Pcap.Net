using System;

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
    public class IcmpSecurityFailuresDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        private class Offset
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

        internal IcmpSecurityFailuresDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpSecurityFailuresLayer
                       {
                           Code = (IcmpCodeSecurityFailures)Code,
                           Checksum = Checksum,
                           Pointer = Pointer
                       };
        }
    }
}