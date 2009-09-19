using System;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// The class to use to build all the packets.
    /// </summary>
    public static class PacketBuilder
    {
        /// <summary>
        /// Builds an Ethernet packet.
        /// </summary>
        /// <param name="timestamp">The packet timestamp.</param>
        /// <param name="ethernetSource">The Ethernet source mac address.</param>
        /// <param name="ethernetDestination">The Ethernet destination mac address.</param>
        /// <param name="ethernetType">The Ethernet type.</param>
        /// <param name="ethernetPayload">The Ethernet payload.</param>
        /// <returns>A packet with an Ethernet datagram.</returns>
        public static Packet Ethernet(DateTime timestamp,
                                      MacAddress ethernetSource, MacAddress ethernetDestination, EthernetType ethernetType,
                                      Datagram ethernetPayload)
        {
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ethernetPayload.Length];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, ethernetType);
            ethernetPayload.Write(buffer, EthernetDatagram.HeaderLength);
            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }


        /// <summary>
        /// Builds an ARP over Ethernet packet.
        /// The ethernet destination will be ethernet broadcast.
        /// </summary>
        /// <param name="timestamp">The packet timestamp.</param>
        /// <param name="ethernetSource">The Ethernet source mac address.</param>
        /// <param name="arpProtocolType">Each protocol is assigned a number used in this field.</param>
        /// <param name="arpOperation">Specifies the operation the sender is performing.</param>
        /// <param name="arpSenderHardwareAddress">Hardware address of the sender.</param>
        /// <param name="arpSenderProtocolAddress">Protocol address of the sender.</param>
        /// <param name="arpTargetHardwareAddress">Hardware address of the intended receiver. This field is ignored in requests.</param>
        /// <param name="arpTargetProtocolAddress">Protocol address of the intended receiver.</param>
        /// <returns>A packet with an ARP over Ethernet datagram.</returns>
        /// <exception cref="ArgumentException">The sender hardware or protocol addresses have different length.</exception>
        public static Packet EthernetArp(DateTime timestamp,
                                         MacAddress ethernetSource,
                                         EthernetType arpProtocolType, ArpOperation arpOperation,
                                         byte[] arpSenderHardwareAddress, byte[] arpSenderProtocolAddress,
                                         byte[] arpTargetHardwareAddress, byte[] arpTargetProtocolAddress)
        {
            if (arpSenderHardwareAddress.Length != arpTargetHardwareAddress.Length)
            {
                throw new ArgumentException("Sender hardware address length is " + arpSenderHardwareAddress.Length + " bytes " +
                                            "while target hardware address length is " + arpTargetHardwareAddress.Length + " bytes");
            }
            if (arpSenderProtocolAddress.Length != arpTargetProtocolAddress.Length)
            {
                throw new ArgumentException("Sender protocol address length is " + arpSenderProtocolAddress.Length + " bytes " +
                                            "while target protocol address length is " + arpTargetProtocolAddress.Length + " bytes");
            }
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ArpDatagram.GetHeaderLength(arpSenderHardwareAddress.Length, arpSenderProtocolAddress.Length)];

            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, EthernetDatagram.BroadcastAddress, EthernetType.Arp);
            ArpDatagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                    ArpHardwareType.Ethernet, arpProtocolType, arpOperation,
                                    arpSenderHardwareAddress, arpSenderProtocolAddress, arpTargetHardwareAddress, arpTargetProtocolAddress);
            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }

        /// <summary>
        /// Builds an IPv4 over Ethernet packet.
        /// </summary>
        /// <param name="timestamp">The packet timestamp.</param>
        /// <param name="ethernetSource">The ethernet source mac address.</param>
        /// <param name="ethernetDestination">The ethernet destination mac address.</param>
        /// <param name="ipV4TypeOfService">The IPv4 Type of Service.</param>
        /// <param name="ipV4Identification">The IPv4 Identification.</param>
        /// <param name="ipV4Fragmentation">The IPv4 Fragmentation.</param>
        /// <param name="ipV4Ttl">The IPv4 TTL.</param>
        /// <param name="ipV4Protocol">The IPv4 Protocol.</param>
        /// <param name="ipV4SourceAddress">The IPv4 source address.</param>
        /// <param name="ipV4DestinationAddress">The IPv4 destination address.</param>
        /// <param name="ipV4Options">The IPv4 options.</param>
        /// <param name="ipV4Payload">The IPv4 payload.</param>
        /// <returns>A packet with an IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4(DateTime timestamp,
                                  MacAddress ethernetSource, MacAddress ethernetDestination,
                                  byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                  byte ipV4Ttl, IpV4Protocol ipV4Protocol,
                                  IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                  IpV4Options ipV4Options,
                                  Datagram ipV4Payload)
        {
            int ipHeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ipHeaderLength + ipV4Payload.Length];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);
            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, ipV4Protocol,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, ipV4Payload.Length);
            ipV4Payload.Write(buffer, EthernetDatagram.HeaderLength + ipHeaderLength);
            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }

        /// <summary>
        /// Builds a UDP over IPv4 over Ethernet packet.
        /// </summary>
        /// <param name="timestamp">The packet timestamp.</param>
        /// <param name="ethernetSource">The ethernet source mac address.</param>
        /// <param name="ethernetDestination">The ethernet destination mac address.</param>
        /// <param name="ipV4TypeOfService">The IPv4 Type of Service.</param>
        /// <param name="ipV4Identification">The IPv4 Identification.</param>
        /// <param name="ipV4Fragmentation">The IPv4 Fragmentation.</param>
        /// <param name="ipV4Ttl">The IPv4 TTL.</param>
        /// <param name="ipV4SourceAddress">The IPv4 source address.</param>
        /// <param name="ipV4DestinationAddress">The IPv4 destination address.</param>
        /// <param name="ipV4Options">The IPv4 options.</param>
        /// <param name="udpSourcePort">The source udp port.</param>
        /// <param name="udpDestinationPort">The destination udp port.</param>
        /// <param name="udpCalculateChecksum">Whether to calculate udp checksum or leave it empty (UDP checksum is optional).</param>
        /// <param name="udpPayload">The payload of UDP datagram.</param>
        /// <returns>A packet with a UDP over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4Udp(DateTime timestamp,
                                             MacAddress ethernetSource, MacAddress ethernetDestination,
                                             byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                             byte ipV4Ttl, 
                                             IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                             IpV4Options ipV4Options,
                                             ushort udpSourcePort, ushort udpDestinationPort, bool udpCalculateChecksum,
                                             Datagram udpPayload)
        {
            int ipV4HeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            int transportLength = UdpDatagram.HeaderLength + udpPayload.Length;
            int ethernetIpV4HeadersLength = EthernetDatagram.HeaderLength + ipV4HeaderLength;
            byte[] buffer = new byte[ethernetIpV4HeadersLength + transportLength];

            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);

            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, IpV4Protocol.Udp,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, transportLength);

            UdpDatagram.WriteHeader(buffer, ethernetIpV4HeadersLength, udpSourcePort, udpDestinationPort, udpPayload.Length);

            udpPayload.Write(buffer, ethernetIpV4HeadersLength + UdpDatagram.HeaderLength);

            if (udpCalculateChecksum)
                IpV4Datagram.WriteTransportChecksum(buffer, EthernetDatagram.HeaderLength, ipV4HeaderLength, (ushort)transportLength, UdpDatagram.Offset.Checksum, true);

            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }

        /// <summary>
        /// Builds a UDP over IPv4 over Ethernet packet.
        /// </summary>
        /// <param name="timestamp">The packet timestamp.</param>
        /// <param name="ethernetSource">The ethernet source mac address.</param>
        /// <param name="ethernetDestination">The ethernet destination mac address.</param>
        /// <param name="ipV4TypeOfService">The IPv4 Type of Service.</param>
        /// <param name="ipV4Identification">The IPv4 Identification.</param>
        /// <param name="ipV4Fragmentation">The IPv4 Fragmentation.</param>
        /// <param name="ipV4Ttl">The IPv4 TTL.</param>
        /// <param name="ipV4SourceAddress">The IPv4 source address.</param>
        /// <param name="ipV4DestinationAddress">The IPv4 destination address.</param>
        /// <param name="ipV4Options">The IPv4 options.</param>
        /// <param name="tcpSourcePort">The source TCP port.</param>
        /// <param name="tcpDestinationPort">The destination TCP port.</param>
        /// <param name="tcpSequenceNumber">The TCP sequence number.</param>
        /// <param name="tcpAcknowledgmentNumber">The TCP ack number.</param>
        /// <param name="tcpControlBits">The TCP flags.</param>
        /// <param name="tcpWindow">The TCP window size.</param>
        /// <param name="tcpUrgentPointer">The TCP urgent pointer value.</param>
        /// <param name="tcpOptions">The TCP options.</param>
        /// <param name="tcpPayload">The payload of UDP datagram.</param>
        /// <returns>A packet with a UDP over IPv4 over Ethernet datagram.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "pointer")]
        public static Packet EthernetIpV4Tcp(DateTime timestamp,
                                             MacAddress ethernetSource, MacAddress ethernetDestination,
                                             byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                             byte ipV4Ttl,
                                             IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                             IpV4Options ipV4Options,
                                             ushort tcpSourcePort, ushort tcpDestinationPort,
                                             uint tcpSequenceNumber, uint tcpAcknowledgmentNumber,
                                             TcpControlBits tcpControlBits, ushort tcpWindow, ushort tcpUrgentPointer,
                                             TcpOptions tcpOptions,
                                             Datagram tcpPayload)

        {
            int ipV4HeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            int tcpHeaderLength = TcpDatagram.HeaderMinimumLength + tcpOptions.BytesLength;
            int transportLength = tcpHeaderLength + tcpPayload.Length;
            int ethernetIpV4HeadersLength = EthernetDatagram.HeaderLength + ipV4HeaderLength;
            byte[] buffer = new byte[ethernetIpV4HeadersLength + transportLength];

            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);

            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, IpV4Protocol.Tcp,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, transportLength);

            TcpDatagram.WriteHeader(buffer, ethernetIpV4HeadersLength,
                                    tcpSourcePort, tcpDestinationPort,
                                    tcpSequenceNumber, tcpAcknowledgmentNumber,
                                    tcpControlBits, tcpWindow, tcpUrgentPointer,
                                    tcpOptions);

            tcpPayload.Write(buffer, ethernetIpV4HeadersLength + tcpHeaderLength);

            IpV4Datagram.WriteTransportChecksum(buffer, EthernetDatagram.HeaderLength, ipV4HeaderLength, (ushort)transportLength, TcpDatagram.Offset.Checksum, false);

            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }
    }
}