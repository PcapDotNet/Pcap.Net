using System;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+------------+
    /// | Bit | 0-7  | 8-15 | 16-31      |
    /// +-----+------+------+------------+
    /// | 0   | Type | Code | Checksum   |
    /// +-----+------+------+------------+
    /// | 0   | Gateway Internet Address |
    /// +-----+--------------------------+
    /// | 32  | Internet Header          |
    /// |     | + 64 bits of             |
    /// |     | Original Data Datagram   |
    /// +-----+--------------------------+
    /// </pre>
    /// </summary>
    public class IcmpRedirectDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        private static class Offset
        {
            public const int GatewayInternetAddress = 4;
        }

        /// <summary>
        /// Address of the gateway to which traffic for the network specified in the internet destination network field of the original datagram's data should be sent.
        /// </summary>
        public IpV4Address GatewayInternetAddress
        {
            get { return ReadIpV4Address(Offset.GatewayInternetAddress, Endianity.Big); }
        }

        internal IcmpRedirectDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpRedirectLayer
                       {
                           Code = (IcmpCodeRedirect)Code,
                           Checksum = Checksum,
                           GatewayInternetAddress = GatewayInternetAddress
                       };
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

        private static readonly byte _minCode = (byte)typeof(IcmpCodeRedirect).GetEnumValues<IcmpCodeRedirect>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeRedirect).GetEnumValues<IcmpCodeRedirect>().Max();
    }
}