using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ip;
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
        /// The bytes padding the Ethernet packet beyond the actual Ethernet payload until it is a 60 bytes packet.
        /// This assumes we know how to calculate the actual payload length (For example, by using the Total Length of the IPv4 payload).
        /// If we don't know how to calculate the actual payload length <see langword="null"/> will be returned.
        /// The Padding doesn't include the <see cref="Trailer"/> and the <see cref="FrameCheckSequence"/> if any exist.
        /// </summary>
        public DataSegment Padding
        {
            get
            {
                if (Length < 60)
                    return DataSegment.Empty;
                DataSegment payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                int payloadLength = payloadByEtherType.Length;
                int dataLength = HeaderLength + payloadLength;
                if (dataLength >= 60)
                    return DataSegment.Empty;
                return new DataSegment(Buffer, StartOffset + dataLength, 60 - dataLength);
            }
        }

        /// <summary>
        /// The bytes padding the Ethernet packet beyond the actual Ethernet payload and beyond the first 60 bytes of the packet.
        /// This assumes we know how to calculate the actual payload length (For example, by using the Total Length of the IPv4 payload).
        /// If we don't know how to calculate the actual payload length <see langword="null"/> will be returned.
        /// The trailer doesn't include the <see cref="Padding"/> and the <see cref="FrameCheckSequence"/> if any exist.
        /// </summary>
        public DataSegment Trailer
        {
            get
            {
                DataSegment extraBytes = ExtraData;
                if (extraBytes == null)
                    return null;

                DataSegment frameCheckSequence = FrameCheckSequence;
                if (frameCheckSequence == null)
                    return extraBytes;
                return extraBytes.Subsegment(Padding.Length, extraBytes.Length - frameCheckSequence.Length - Padding.Length);
            }
        }

        /// <summary>
        /// The 4 bytes of the Frame Check Sequence (FCS).
        /// Usually, these bytes won't be available because the device remvoed them after checking their validity.
        /// We assume they exist when we see that the Ethernet extra bytes pads to 68 bytes or more.
        /// If the packet isn't that long or we don't know how to calculate the real payload length, <see langword="null"/> will be returned.
        /// </summary>
        public DataSegment FrameCheckSequence
        {
            get
            {
                if (Length < 68)
                    return null;

                DataSegment extraBytes = ExtraData;
                if (extraBytes == null)
                    return null;

                if (extraBytes.Length >= 4)
                    return extraBytes.Subsegment(extraBytes.Length - 4, 4);
                return null;
            }
        }

        /// <summary>
        /// A sequence of bytes that includes the padding bytes, trailer bytes and the frame check sequence bytes.
        /// </summary>
        public DataSegment ExtraData
        {
            get
            {
                DataSegment payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                int payloadLength = payloadByEtherType.Length;
                return new DataSegment(Buffer, StartOffset + HeaderLength + payloadLength, Length - HeaderLength - payloadLength);
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
        /// Returns the IP Datagram.
        /// This is either an IPv4 Datagram or an IPv6 Datagram (according to the Ethernet Type).
        /// </summary>
        public IpDatagram Ip
        {
            get
            {
                switch (EtherType)
                {
                    case EthernetType.IpV4:
                        return IpV4;

                    case EthernetType.IpV6:
                        return IpV6;

                    default:
                        return null;
                }
            }
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