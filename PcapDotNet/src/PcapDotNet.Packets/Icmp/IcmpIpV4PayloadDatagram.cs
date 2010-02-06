using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | unused                  |
    /// +-----+-------------------------+
    /// | 64  | IpV4 datagram           |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public abstract class IcmpIpV4PayloadDatagram : IcmpDatagram
    {
        internal IcmpIpV4PayloadDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        protected IpV4Datagram IpV4
        {
            get
            {
                if (_ipV4 == null && Length >= HeaderLength)
                    _ipV4 = new IpV4Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);
                return _ipV4;
            }
        }

        private IpV4Datagram _ipV4;
    }
}