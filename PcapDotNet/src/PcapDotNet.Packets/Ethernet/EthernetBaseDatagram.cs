using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

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
        public DataSegment Trailer
        {
            get
            {
                DataSegment trailerWithFrameCheckSequence = TrailerWithFrameCheckSequence;
                if (trailerWithFrameCheckSequence == null)
                    return null;

                DataSegment frameCheckSequence = FrameCheckSequence;
                if (frameCheckSequence == null)
                    return trailerWithFrameCheckSequence;
                return trailerWithFrameCheckSequence.Subsegment(0, trailerWithFrameCheckSequence.Length - frameCheckSequence.Length);
            }
        }

        /// <summary>
        /// A sequence of bytes that includes the trailer bytes and the framce check sequence bytes.
        /// </summary>
        public DataSegment TrailerWithFrameCheckSequence
        {
            get
            {
                DataSegment payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                int payloadLength = PayloadByEtherType.Length;
                return new DataSegment(Buffer, StartOffset + HeaderLength + payloadLength, Length - HeaderLength - payloadLength);
            }
        }

        /// <summary>
        /// The 4 bytes of the Frame Check Sequence (FCS).
        /// Usually, these bytes won't be available because the device remvoed them after checking their validity.
        /// We assume they exist when we see that the Ethernet padding pads to 68 bytes or more.
        /// If the padding isn't that long or we don't know how to calculate the real payload length, <see langword="null"/> will be returned.
        /// </summary>
        public DataSegment FrameCheckSequence
        {
            get
            {
                if (Length < 68)
                    return null;

                DataSegment trailerWithFrameCheckSequence = TrailerWithFrameCheckSequence;
                if (trailerWithFrameCheckSequence == null)
                    return null;

                if (trailerWithFrameCheckSequence.Length >= 4)
                    return trailerWithFrameCheckSequence.Subsegment(trailerWithFrameCheckSequence.Length - 4, 4);
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
        /// The Ethernet payload as an IPv6 datagram.
        /// </summary>
        public IpV6Datagram IpV6
        {
            get { return PayloadDatagrams.IpV6; }
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