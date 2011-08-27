using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | unused                  |
    /// +-----+-------------------------+
    /// | 64  | Internet Header         |
    /// |     | + 64 bits of            |
    /// |     | Original Data Datagram  |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.TimeExceeded)]
    public sealed class IcmpTimeExceededDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpTimeExceededLayer
                       {
                           Code = (IcmpCodeTimeExceeded)Code,
                           Checksum = Checksum
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

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpTimeExceededDatagram(buffer, offset, length);
        }

        private IcmpTimeExceededDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeTimeExceeded).GetEnumValues<IcmpCodeTimeExceeded>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeTimeExceeded).GetEnumValues<IcmpCodeTimeExceeded>().Max();
    }
}