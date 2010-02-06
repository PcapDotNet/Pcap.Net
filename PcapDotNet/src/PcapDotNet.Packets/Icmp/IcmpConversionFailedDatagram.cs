using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1475.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | pointer to problem area |
    /// +-----+-------------------------+
    /// | 64  | copy of datagram that   |
    /// |     | could not be converted  |
    /// |     | ...                     |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public class IcmpConversionFailedDatagram : IcmpIpV4PayloadDatagram
    {
        private class Offset
        {
            public const int Pointer = 4;
        }

        /// <summary>
        /// An offset from the start of the original datagram to the beginning of the offending field.
        /// </summary>
        public uint Pointer
        {
            get { return ReadUInt(Offset.Pointer, Endianity.Big); }
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpConversionFailedLayer
                       {
                           Checksum = Checksum,
                           Code = (IcmpCodeConversionFailed)Code,
                           Pointer = Pointer
                       };
        }

        internal IcmpConversionFailedDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}