using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// Base class for all datagrams that behave like Ethernet.
    /// Contains a header with an Ethernet type and a payload that should be according to this Ethernet type.
    /// </summary>
    public abstract class EthernetBaseDatagram : Datagram
    {
        /// <summary>
        /// Header length in bytes.
        /// </summary>
        public abstract int HeaderLength { get; }

        /// <summary>
        /// Ethernet type (next protocol).
        /// </summary>
        public abstract EthernetType EtherType { get; }

        /// <summary>
        /// The Ethernet payload.
        /// </summary>
        public Datagram Payload
        {
            get { return PayloadDatagrams.Payload; }
        }

        /// <summary>
        /// The bytes padding the Ethernet packet beyond the actual Ethernet payload.
        /// This assumes we know how to calculate the actual payload length (For example, by using the Total Length of the IPv4 payload).
        /// If we don't know how to calculate the actual payload length <see langword="null"/> will be returned.
        /// The trailer doesn't include the <see cref="FrameCheckSequence"/> if it exists.
        /// </summary>
        public Datagram Trailer
        {
            get
            {
                Datagram payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                int payloadLength = PayloadByEtherType.Length;
                Datagram fcs = FrameCheckSequence;
                return new Datagram(Buffer, StartOffset + HeaderLength + payloadLength, Length - HeaderLength - payloadLength - (fcs == null ? 0 : fcs.Length));
            }
        }

        /// <summary>
        /// The 4 bytes of the Frame Check Sequence (FCS).
        /// Usually, these bytes won't be available because the device remvoed them after checking their validity.
        /// We assume they exist when we see that the Ethernet padding pads to 68 bytes or more.
        /// If the padding isn't that long or we don't know how to calculate the real payload length, <see langword="null"/> will be returned.
        /// </summary>
        public Datagram FrameCheckSequence
        {
            get
            {
                Datagram payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                if (Length - HeaderLength - payloadByEtherType.Length >= 4 && Length >= 68)
                    return new Datagram(Buffer, Length - 4, 4);
                return null;
            }
        }

        /// <summary>
        /// The Ethernet payload as a VLAN Tagged Frame datagram.
        /// </summary>
        public VLanTaggedFrameDatagram VLanTaggedFrame
        {
            get { return PayloadDatagrams.VLanTaggedFrame; }
        }

        /// <summary>
        /// The Ethernet payload as an IPv4 datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get { return PayloadDatagrams.IpV4; }
        }

        /// <summary>
        /// The Ethernet payload as an ARP datagram.
        /// </summary>
        public ArpDatagram Arp
        {
            get { return PayloadDatagrams.Arp; }
        }

        internal Datagram PayloadByEtherType
        {
            get { return PayloadDatagrams.Get(EtherType); }
        }

        internal EthernetBaseDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private EthernetPayloadDatagrams PayloadDatagrams
        {
            get
            {
                return _payloadDatagrams ?? (_payloadDatagrams = new EthernetPayloadDatagrams(Length >= HeaderLength
                                                                                                  ? new Datagram(Buffer, StartOffset + HeaderLength,
                                                                                                                 Length - HeaderLength)
                                                                                                  : null));
            }
        }

        private EthernetPayloadDatagrams _payloadDatagrams;
    }
}