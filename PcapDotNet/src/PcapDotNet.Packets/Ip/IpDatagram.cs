using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Ip
{
    /// <summary>
    /// RFCs 791, 2460.
    /// Represents an IP datagram.
    /// <pre>
    /// +-----+---------+
    /// | Bit | 0-3     |
    /// +-----+---------+
    /// | 0   | Version |
    /// +-----+---------+
    /// | 4   |         |
    /// | ... |         |
    /// +-----+---------+
    /// </pre>
    /// </summary>
    public abstract class IpDatagram : Datagram
    {
        private static class Offset
        {
            public const int Version = 0;
        }

        private static class Mask
        {
            public const byte Version = 0xF0;
        }

        private static class Shift
        {
            public const int Version = 4;
        }

        /// <summary>
        /// Indicates the format of the internet header (Internet Protocol version).
        /// </summary>
        public byte Version
        {
            get { return (byte)((this[Offset.Version] & Mask.Version) >> Shift.Version); }
        }

        public abstract int TotalLength { get; }

        /// <summary>
        /// Returns whether the TCP or UDP checksum is correct.
        /// The protocol must be TCP or UDP.
        /// For UDP, the checksum is optional, so 0 checksum is still correct.
        /// </summary>
        public bool IsTransportChecksumCorrect
        {
            get
            {
                if (_isTransportChecksumCorrect == null)
                {
                    ushort transportChecksum = Transport.Checksum;
                    _isTransportChecksumCorrect = Length >= TotalLength &&
                                                  (Transport.IsChecksumOptional && transportChecksum == 0) ||
                                                  (CalculateTransportChecksum() == transportChecksum);
                }
                return _isTransportChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// The payload of the datagram.
        /// </summary>
        public Datagram Payload
        {
            get
            {
                if (_payload == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _payload = new Datagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _payload;
            }
        }

        /// <summary>
        /// The payload of the datagram as an IPv4 datagram (IP over IP).
        /// </summary>
        public IpV4Datagram IpV4
        {
            get
            {
                if (_ipV4 == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _ipV4 = new IpV4Datagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _ipV4;
            }
        }

        /// <summary>
        /// The payload of the datagram as an IPv6 datagram (IP over IP).
        /// </summary>
        public IpV6Datagram IpV6
        {
            get
            {
                if (_ipV6 == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _ipV6 = new IpV6Datagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _ipV6;
            }
        }

        /// <summary>
        /// The payload of the datagram as an ICMP datagram.
        /// </summary>
        public IcmpDatagram Icmp
        {
            get
            {
                if (_icmp == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _icmp = IcmpDatagram.CreateDatagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _icmp;
            }
        }

        /// <summary>
        /// The payload of the datagram as an IGMP datagram.
        /// </summary>
        public IgmpDatagram Igmp
        {
            get
            {
                if (_igmp == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _igmp = new IgmpDatagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _igmp;
            }
        }

        /// <summary>
        /// The payload of the datagram as a TCP datagram.
        /// </summary>
        public TcpDatagram Tcp
        {
            get
            {
                if (_tcp == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _tcp = new TcpDatagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _tcp;
            }
        }

        /// <summary>
        /// The payload of the datagram as a GRE datagram.
        /// </summary>
        public GreDatagram Gre
        {
            get
            {
                if (_gre == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _gre = new GreDatagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _gre;
            }
        }

        /// <summary>
        /// The payload of the datagram as a UDP datagram.
        /// </summary>
        public UdpDatagram Udp
        {
            get
            {
                if (_udp == null)
                {
                    DataSegment payload = GetPayload();
                    if (payload != null)
                        _udp = new UdpDatagram(payload.Buffer, payload.StartOffset, payload.Length);
                }
                return _udp;
            }
        }

        /// <summary>
        /// Returns the Tranposrt Datagram.
        /// This is either a TCP Datagram or a UDP Datagram (according to the payload protocol).
        /// </summary>
        public TransportDatagram Transport
        {
            get
            {
                switch (PayloadProtocol)
                {
                    case IpV4Protocol.Tcp:
                        return Tcp;

                    case IpV4Protocol.Udp:
                        return Udp;

                    default:
                        return null;
                }
            }
        }

        protected abstract ushort CalculateTransportChecksum();

        internal IpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal abstract IpV4Protocol PayloadProtocol { get; }

        internal abstract DataSegment GetPayload();

        private bool? _isTransportChecksumCorrect;
        private Datagram _payload;
        private IpV4Datagram _ipV4;
        private IpV6Datagram _ipV6;
        private IcmpDatagram _icmp;
        private IgmpDatagram _igmp;
        private GreDatagram _gre;
        private TcpDatagram _tcp;
        private UdpDatagram _udp;
    }
}