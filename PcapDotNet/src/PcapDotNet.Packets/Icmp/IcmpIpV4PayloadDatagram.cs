using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// <pre>
    /// +-----+-------------------------+
    /// | Bit | 0-31                    |
    /// +-----+-------------------------+
    /// | 0   | unused                  |
    /// +-----+-------------------------+
    /// | 32  | IpV4 datagram           |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public abstract class IcmpIpV4PayloadDatagram : IcmpTypedDatagram
    {
        internal IcmpIpV4PayloadDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        protected IpV4Datagram IpV4Payload
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