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
        /// <summary>
        /// The number of bytes this Datagram should take.
        /// </summary>
        public const int DatagramLength = HeaderLength + PayloadLength;

        /// <summary>
        /// The number of bytes this ICMP payload should take.
        /// </summary>
        public const int PayloadLength = 4;

        private static class Offset
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

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
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

        /// <summary>
        /// ICMP is valid if the datagram's length is OK, the checksum is correct and the code is in the expected range.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Length == DatagramLength;
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, IpV4Address addressMask)
        {
            buffer.Write(offset, addressMask, Endianity.Big);
        }
    }
}