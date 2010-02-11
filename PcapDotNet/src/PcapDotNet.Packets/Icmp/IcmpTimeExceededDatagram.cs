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
    public class IcmpTimeExceededDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        public IcmpTimeExceededDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpTimeExceededLayer
                       {
                           Code = (IcmpCodeTimeExceeded)Code,
                           Checksum = Checksum
                       };
        }

        protected override byte MinCodeValue
        {
            get { return _minCode; }
        }

        protected override byte MaxCodeValue
        {
            get { return _maxCode; }
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeTimeExceeded).GetEnumValues<IcmpCodeTimeExceeded>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeTimeExceeded).GetEnumValues<IcmpCodeTimeExceeded>().Max();

    }
}