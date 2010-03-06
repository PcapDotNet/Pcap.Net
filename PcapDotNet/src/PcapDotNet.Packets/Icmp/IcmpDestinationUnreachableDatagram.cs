using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792 and RFC 1191.
    /// <pre>
    /// +-----+------+------+--------------+
    /// | Bit | 0-7  | 8-15 | 16-31        |
    /// +-----+------+------+--------------+
    /// | 0   | Type | Code | Checksum     |
    /// +-----+------+------+--------------+
    /// | 32  | unused      | Next-Hop MTU |
    /// +-----+-------------+--------------+
    /// | 64  | Internet Header            |
    /// |     | + 64 bits of               |
    /// |     | Original Data Datagram     |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public class IcmpDestinationUnreachableDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        /// <summary>
        /// The minimum value of the maximum transmission unit for FragmentationNeededAndDoNotFragmentSet code.
        /// </summary>
        public const ushort MinimumMaximumTransmissionUnit = 68;

        private static class Offset
        {
            public const int NextHopMtu = 6;
        }

        internal IcmpDestinationUnreachableDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// The size in octets of the largest datagram that could be forwarded, 
        /// along the path of the original datagram, without being fragmented at this router.  
        /// The size includes the IP header and IP data, and does not include any lower-level headers.
        /// </summary>
        public ushort NextHopMaximumTransmissionUnit
        {
            get { return ReadUShort(Offset.NextHopMtu, Endianity.Big); }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpDestinationUnreachableLayer
                       {
                           Code = (IcmpCodeDestinationUnreachable)Code,
                           Checksum = Checksum,
                           NextHopMaximumTransmissionUnit = NextHopMaximumTransmissionUnit,
                       };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct, the code is in the expected range,
        /// the IPv4 payload contains at least an IPv4 header, the IPv4's payload is in the expected size
        /// and if the NextHopMaximumTransmissionUnit is at least 68 for FragmentationNeededAndDoNotFragmentSet code or exactly 0 for other codes.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (!base.CalculateIsValid())
                return false;

            if ((IcmpCodeDestinationUnreachable)Code == IcmpCodeDestinationUnreachable.FragmentationNeededAndDoNotFragmentSet)
                return NextHopMaximumTransmissionUnit >= MinimumMaximumTransmissionUnit;

            return NextHopMaximumTransmissionUnit == 0;
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

        private static readonly byte _minCode = (byte)typeof(IcmpCodeDestinationUnreachable).GetEnumValues<IcmpCodeDestinationUnreachable>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeDestinationUnreachable).GetEnumValues<IcmpCodeDestinationUnreachable>().Max();
    }
}