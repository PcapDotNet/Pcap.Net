using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 950.
    /// <pre>
    /// +-----+------+------+-----------------+
    /// | Bit | 0-7  | 8-15 | 16-31           |
    /// +-----+------+------+-----------------+
    /// | 0   | Type | Code | Checksum        |
    /// +-----+------+------+-----------------+
    /// | 32  | Identifier  | Sequence Number |
    /// +-----+-------------+-----------------+
    /// | 64  | Address Mask                  |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpAddressMaskRequestDatagram : IcmpIdentifiedDatagram
    {
        public const int DatagramLength = HeaderLength + PayloadLength;
        public const int PayloadLength = 4;

        private class Offset
        {
            public const int AddressMask = 8;
        }

        /// <summary>
        /// A 32-bit mask.
        /// </summary>
        public IpV4Address AddressMask
        {
            get { return ReadIpV4Address(Offset.AddressMask, Endianity.Big); }
        }

        internal IcmpAddressMaskRequestDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, IpV4Address addressMask)
        {
            buffer.Write(offset, addressMask, Endianity.Big);
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpAddressMaskRequestLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber,
                           AddressMask = AddressMask,
                       };
        }
    }

    public class IcmpAddressMaskReplyDatagram : IcmpAddressMaskRequestDatagram
    {
        internal IcmpAddressMaskReplyDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpAddressMaskReplyLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber,
                           AddressMask = AddressMask
                       };
        }
    }
}