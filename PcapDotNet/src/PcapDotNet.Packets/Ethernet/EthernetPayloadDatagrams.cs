using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Ethernet
{
    internal class EthernetPayloadDatagrams
    {
        public EthernetPayloadDatagrams(Datagram payload)
        {
            _payload = payload;
        }

        public Datagram Get(EthernetType ethernetType)
        {
            switch (ethernetType)
            {
                case EthernetType.IpV4:
                    return IpV4;

                case EthernetType.Arp:
                    return Arp;

                case EthernetType.VLanTaggedFrame:
                    return VLanTaggedFrame;

                default:
                    return null;
            }
        }

        /// <summary>
        /// The Ethernet payload as an IPv4 datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get
            {
                if (_ipV4 == null && _payload != null)
                    _ipV4 = new IpV4Datagram(_payload.Buffer, _payload.StartOffset, IpV4Datagram.GetTotalLength(_payload));
                return _ipV4;
            }
        }

        /// <summary>
        /// The Ethernet payload as an ARP datagram.
        /// </summary>
        public ArpDatagram Arp
        {
            get
            {
                if (_arp == null && _payload != null)
                    _arp = ArpDatagram.CreateInstance(_payload.Buffer, _payload.StartOffset, _payload.Length);
                return _arp;
            }
        }

        public VLanTaggedFrameDatagram VLanTaggedFrame
        {
            get
            {
                if (_vLanTaggedFrame == null && _payload != null)
                    _vLanTaggedFrame = new VLanTaggedFrameDatagram(_payload.Buffer, _payload.StartOffset, _payload.Length);
                return _vLanTaggedFrame;
            }
        }

        /// <summary>
        /// The Ethernet payload.
        /// </summary>
        public Datagram Payload
        {
            get { return _payload; }
        }

        private readonly Datagram _payload;
        private IpV4Datagram _ipV4;
        private ArpDatagram _arp;
        private VLanTaggedFrameDatagram _vLanTaggedFrame;
    }
}