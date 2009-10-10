using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+--------------------------+
    /// | Bit | 0-31                     |
    /// +-----+--------------------------+
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
        private class Offset
        {
            public const int GatewayInternetAddress = 0;
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
    }
}