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
    public class IcmpDestinationUnreachableDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        public IcmpDestinationUnreachableDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpDestinationUnreachableLayer
                       {
                           Code = (IcmpCodeDestinationUnrechable)Code,
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

        private static readonly byte _minCode = (byte)typeof(IcmpCodeDestinationUnrechable).GetEnumValues<IcmpCodeDestinationUnrechable>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeDestinationUnrechable).GetEnumValues<IcmpCodeDestinationUnrechable>().Max();
    }
}