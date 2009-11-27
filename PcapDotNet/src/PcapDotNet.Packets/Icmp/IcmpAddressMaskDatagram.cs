using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 950.
    /// <pre>
    /// +-----+------------+-----------------+
    /// | Bit | 0-15       | 16-31           |
    /// +-----+------------+-----------------+
    /// | 0   | Identifier | Sequence Number |
    /// +-----+------------+-----------------+
    /// | 32  | Address Mask                 |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpAddressMaskDatagram : IcmpIdentifiedDatagram
    {
        public const int HeaderLength = HeaderMinimumLength + HeaderAdditionalLength;
        public const int HeaderAdditionalLength = 4;

        private class Offset
        {
            public const int AddressMask = 4;
        }

        /// <summary>
        /// A 32-bit mask.
        /// </summary>
        public IpV4Address AddressMask
        {
            get { return ReadIpV4Address(Offset.AddressMask, Endianity.Big); }
        }

        internal IcmpAddressMaskDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, IpV4Address addressMask)
        {
            buffer.Write(offset, addressMask, Endianity.Big);
        }
    }
}