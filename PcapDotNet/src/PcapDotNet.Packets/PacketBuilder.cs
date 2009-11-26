using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
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
            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
        }

        /// <summary>
        /// Builds an ICMP over IPv4 over Ethernet packet.
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
        /// <param name="icmpCode">The ICMP code.</param>
        /// <param name="icmpIpV4TypeOfService">The IPv4 over the ICMP's Type of Service.</param>
        /// <param name="icmpIpV4Identification">The IPv4 over the ICMP's Identification.</param>
        /// <param name="icmpIpV4Fragmentation">The IPv4 over the ICMP's Fragmentation.</param>
        /// <param name="icmpIpV4Ttl">The IPv4 over the ICMP's TTL.</param>
        /// <param name="icmpIpV4Protocol">The IPv4 over the ICMP's Protocol.</param>
        /// <param name="icmpIpV4SourceAddress">The IPv4 over the ICMP's source address.</param>
        /// <param name="icmpIpV4DestinationAddress">The IPv4 over the ICMP's destination address.</param>
        /// <param name="icmpIpV4Options">The IPv4 over the ICMP's options.</param>
        /// <param name="icmpIpV4Payload">The IPv4 over the ICMP's payload.</param>
        /// <returns>A packet with an ICMP Destination Unreachable over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IcmpDestinationUnreachable(DateTime timestamp,
                                                                    MacAddress ethernetSource, MacAddress ethernetDestination,
                                                                    byte ipV4TypeOfService, ushort ipV4Identification,
                                                                    IpV4Fragmentation ipV4Fragmentation,
                                                                    byte ipV4Ttl,
                                                                    IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                                    IpV4Options ipV4Options,
                                                                    IcmpCodeDestinationUnrechable icmpCode,
                                                                    byte icmpIpV4TypeOfService, ushort icmpIpV4Identification,
                                                                    IpV4Fragmentation icmpIpV4Fragmentation,
                                                                    byte icmpIpV4Ttl, IpV4Protocol icmpIpV4Protocol,
                                                                    IpV4Address icmpIpV4SourceAddress, IpV4Address icmpIpV4DestinationAddress,
                                                                    IpV4Options icmpIpV4Options,
                                                                    Datagram icmpIpV4Payload)
        {
            return EthernetIpV4IcmpWithIpV4Payload(timestamp,
                                                   ethernetSource, ethernetDestination,
                                                   ipV4TypeOfService, ipV4Identification,
                                                   ipV4Fragmentation,
                                                   ipV4Ttl,
                                                   ipV4SourceAddress, ipV4DestinationAddress,
                                                   ipV4Options,
                                                   (byte)icmpCode, 0,
                                                   icmpIpV4TypeOfService, icmpIpV4Identification,
                                                   icmpIpV4Fragmentation,
                                                   icmpIpV4Ttl, icmpIpV4Protocol,
                                                   icmpIpV4SourceAddress, icmpIpV4DestinationAddress,
                                                   icmpIpV4Options,
                                                   icmpIpV4Payload);
        }

        /// <summary>
        /// Builds an ICMP over IPv4 over Ethernet packet.
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
        /// <param name="icmpCode">The ICMP code.</param>
        /// <param name="icmpIpV4TypeOfService">The IPv4 over the ICMP's Type of Service.</param>
        /// <param name="icmpIpV4Identification">The IPv4 over the ICMP's Identification.</param>
        /// <param name="icmpIpV4Fragmentation">The IPv4 over the ICMP's Fragmentation.</param>
        /// <param name="icmpIpV4Ttl">The IPv4 over the ICMP's TTL.</param>
        /// <param name="icmpIpV4Protocol">The IPv4 over the ICMP's Protocol.</param>
        /// <param name="icmpIpV4SourceAddress">The IPv4 over the ICMP's source address.</param>
        /// <param name="icmpIpV4DestinationAddress">The IPv4 over the ICMP's destination address.</param>
        /// <param name="icmpIpV4Options">The IPv4 over the ICMP's options.</param>
        /// <param name="icmpIpV4Payload">The IPv4 over the ICMP's payload.</param>
        /// <returns>A packet with an ICMP Destination Unreachable over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IcmpTimeExceeded(DateTime timestamp,
                                                          MacAddress ethernetSource, MacAddress ethernetDestination,
                                                          byte ipV4TypeOfService, ushort ipV4Identification,
                                                          IpV4Fragmentation ipV4Fragmentation,
                                                          byte ipV4Ttl,
                                                          IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                          IpV4Options ipV4Options,
                                                          IcmpCodeDestinationUnrechable icmpCode,
                                                          byte icmpIpV4TypeOfService, ushort icmpIpV4Identification,
                                                          IpV4Fragmentation icmpIpV4Fragmentation,
                                                          byte icmpIpV4Ttl, IpV4Protocol icmpIpV4Protocol,
                                                          IpV4Address icmpIpV4SourceAddress, IpV4Address icmpIpV4DestinationAddress,
                                                          IpV4Options icmpIpV4Options,
                                                          Datagram icmpIpV4Payload)
        {
            return EthernetIpV4IcmpWithIpV4Payload(timestamp,
                                                   ethernetSource, ethernetDestination,
                                                   ipV4TypeOfService, ipV4Identification,
                                                   ipV4Fragmentation,
                                                   ipV4Ttl,
                                                   ipV4SourceAddress, ipV4DestinationAddress,
                                                   ipV4Options,
                                                   (byte)icmpCode, 0,
                                                   icmpIpV4TypeOfService, icmpIpV4Identification,
                                                   icmpIpV4Fragmentation,
                                                   icmpIpV4Ttl, icmpIpV4Protocol,
                                                   icmpIpV4SourceAddress, icmpIpV4DestinationAddress,
                                                   icmpIpV4Options,
                                                   icmpIpV4Payload);
        }

        private static Packet EthernetIpV4IcmpWithIpV4Payload(DateTime timestamp,
                                                              MacAddress ethernetSource, MacAddress ethernetDestination,
                                                              byte ipV4TypeOfService, ushort ipV4Identification,
                                                              IpV4Fragmentation ipV4Fragmentation,
                                                              byte ipV4Ttl,
                                                              IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                              IpV4Options ipV4Options,
                                                              byte icmpCode, uint icmpValueAccordingToType,
                                                              byte icmpIpV4TypeOfService, ushort icmpIpV4Identification,
                                                              IpV4Fragmentation icmpIpV4Fragmentation,
                                                              byte icmpIpV4Ttl, IpV4Protocol icmpIpV4Protocol,
                                                              IpV4Address icmpIpV4SourceAddress, IpV4Address icmpIpV4DestinationAddress,
                                                              IpV4Options icmpIpV4Options,
                                                              Datagram icmpIpV4Payload)
        {
            int ipHeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            int icmpIpHeaderLength = IpV4Datagram.HeaderMinimumLength + icmpIpV4Options.BytesLength;
            int ipPayloadLength = IcmpDatagram.HeaderLength + icmpIpHeaderLength + icmpIpV4Payload.Length;
            int icmpOffset = EthernetDatagram.HeaderLength + ipHeaderLength;
            byte[] buffer = new byte[icmpOffset + ipPayloadLength];

            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);
            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, IpV4Protocol.InternetControlMessageProtocol,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, ipPayloadLength);
            IcmpDatagram.WriteHeader(buffer, icmpOffset, IcmpMessageType.DestinationUnreachable, icmpCode, icmpValueAccordingToType);
            IpV4Datagram.WriteHeader(buffer, icmpOffset + IcmpDatagram.HeaderLength,
                                     icmpIpV4TypeOfService, icmpIpV4Identification,
                                     icmpIpV4Fragmentation,
                                     icmpIpV4Ttl, icmpIpV4Protocol,
                                     icmpIpV4SourceAddress, icmpIpV4DestinationAddress,
                                     icmpIpV4Options, icmpIpV4Payload.Length);
            icmpIpV4Payload.Write(buffer, icmpOffset + IcmpDatagram.HeaderLength + icmpIpHeaderLength);
            IcmpDatagram.WriteChecksum(buffer, icmpOffset, ipPayloadLength);
            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
        }

        /// <summary>
        /// Builds an IGMP query version 1 over IPv4 over Ethernet packet.
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
        /// <param name="igmpGroupAddress">
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </param>
        /// <returns>A packet with an IGMP query version 1 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpQueryVersion1(DateTime timestamp,
                                                           MacAddress ethernetSource, MacAddress ethernetDestination,
                                                           byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                           byte ipV4Ttl,
                                                           IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                           IpV4Options ipV4Options,
                                                           IpV4Address igmpGroupAddress)
        {
            return EthernetIpV4Igmp(timestamp,
                                    ethernetSource, ethernetDestination,
                                    ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                    ipV4Ttl,
                                    ipV4SourceAddress, ipV4DestinationAddress,
                                    ipV4Options,
                                    IgmpMessageType.MembershipQuery, TimeSpan.Zero, igmpGroupAddress);
        }

        /// <summary>
        /// Builds an IGMP report version 1 over IPv4 over Ethernet packet.
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
        /// <param name="igmpGroupAddress">
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </param>
        /// <returns>A packet with an IGMP report version 1 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpReportVersion1(DateTime timestamp,
                                                            MacAddress ethernetSource, MacAddress ethernetDestination,
                                                            byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                            byte ipV4Ttl,
                                                            IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                            IpV4Options ipV4Options,
                                                            IpV4Address igmpGroupAddress)
        {
            return EthernetIpV4Igmp(timestamp,
                                    ethernetSource, ethernetDestination,
                                    ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                    ipV4Ttl,
                                    ipV4SourceAddress, ipV4DestinationAddress,
                                    ipV4Options,
                                    IgmpMessageType.MembershipReportVersion1, TimeSpan.Zero, igmpGroupAddress);
        }

        /// <summary>
        /// Builds an IGMP query version 2 over IPv4 over Ethernet packet.
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
        /// <param name="igmpMaxResponseTime">The actual time allowed.</param>
        /// <param name="igmpGroupAddress">
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </param>
        /// <returns>A packet with an IGMP query version 2 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpQueryVersion2(DateTime timestamp,
                                                           MacAddress ethernetSource, MacAddress ethernetDestination,
                                                           byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                           byte ipV4Ttl,
                                                           IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                           IpV4Options ipV4Options,
                                                           TimeSpan igmpMaxResponseTime, IpV4Address igmpGroupAddress)
        {
            return EthernetIpV4Igmp(timestamp,
                                    ethernetSource, ethernetDestination,
                                    ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                    ipV4Ttl,
                                    ipV4SourceAddress, ipV4DestinationAddress,
                                    ipV4Options,
                                    IgmpMessageType.MembershipQuery, igmpMaxResponseTime, igmpGroupAddress);
        }

        /// <summary>
        /// Builds an IGMP report version 2 over IPv4 over Ethernet packet.
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
        /// <param name="igmpMaxResponseTime">The actual time allowed.</param>
        /// <param name="igmpGroupAddress">
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </param>
        /// <returns>A packet with an IGMP report version 2 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpReportVersion2(DateTime timestamp,
                                                            MacAddress ethernetSource, MacAddress ethernetDestination,
                                                            byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                            byte ipV4Ttl,
                                                            IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                            IpV4Options ipV4Options,
                                                            TimeSpan igmpMaxResponseTime, IpV4Address igmpGroupAddress)
        {
            return EthernetIpV4Igmp(timestamp,
                                    ethernetSource, ethernetDestination,
                                    ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                    ipV4Ttl,
                                    ipV4SourceAddress, ipV4DestinationAddress,
                                    ipV4Options,
                                    IgmpMessageType.MembershipReportVersion2, igmpMaxResponseTime, igmpGroupAddress);
        }

        /// <summary>
        /// Builds an IGMP leave group version 2 over IPv4 over Ethernet packet.
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
        /// <param name="igmpMaxResponseTime">The actual time allowed.</param>
        /// <param name="igmpGroupAddress">
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </param>
        /// <returns>A packet with an IGMP leave group version 2 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpLeaveGroupVersion2(DateTime timestamp,
                                                                MacAddress ethernetSource, MacAddress ethernetDestination,
                                                                byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                                byte ipV4Ttl,
                                                                IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                                IpV4Options ipV4Options,
                                                                TimeSpan igmpMaxResponseTime, IpV4Address igmpGroupAddress)
        {
            return EthernetIpV4Igmp(timestamp,
                                    ethernetSource, ethernetDestination,
                                    ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                    ipV4Ttl,
                                    ipV4SourceAddress, ipV4DestinationAddress,
                                    ipV4Options,
                                    IgmpMessageType.LeaveGroupVersion2, igmpMaxResponseTime, igmpGroupAddress);
        }

        /// <summary>
        /// Builds an IGMP query version 3 over IPv4 over Ethernet packet.
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
        /// <param name="igmpMaxResponseTime">The actual time allowed.</param>
        /// <param name="igmpGroupAddress">
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// </param>
        /// <param name="igmpIsSuppressRouterSideProcessing">
        /// When set to one, the S Flag indicates to any receiving multicast routers that they are to suppress the normal timer updates they perform upon hearing a Query.  
        /// It does not, however, suppress the querier election or the normal "host-side" processing of a Query 
        /// that a router may be required to perform as a consequence of itself being a group member.
        /// </param>
        /// <param name="igmpQueryRobustnessVariable">
        /// If non-zero, the QRV field contains the [Robustness Variable] value used by the querier, i.e., the sender of the Query.  
        /// If the querier's [Robustness Variable] exceeds 7, the maximum value of the QRV field, the QRV is set to zero.  
        /// Routers adopt the QRV value from the most recently received Query as their own [Robustness Variable] value, 
        /// unless that most recently received QRV was zero, in which case the receivers use the default [Robustness Variable] value or a statically configured value.
        /// </param>
        /// <param name="igmpQueryInterval">Interval, called the Querier's Query Interval (QQI).</param>
        /// <param name="igmpSourceAddresses">
        /// The Source Address [i] fields are a vector of n IP unicast addresses,
        /// where n is the value in the Number of Sources (N) field.
        /// </param>
        /// <returns>A packet with an IGMP query version 3 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpQueryVersion3(DateTime timestamp,
                                                           MacAddress ethernetSource, MacAddress ethernetDestination,
                                                           byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                           byte ipV4Ttl,
                                                           IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                           IpV4Options ipV4Options,
                                                           TimeSpan igmpMaxResponseTime, IpV4Address igmpGroupAddress,
                                                           bool igmpIsSuppressRouterSideProcessing, byte igmpQueryRobustnessVariable,
                                                           TimeSpan igmpQueryInterval, IEnumerable<IpV4Address> igmpSourceAddresses)
        {
            int ipHeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            int igmpLength = IgmpDatagram.GetQueryVersion3Length(igmpSourceAddresses.Count());
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ipHeaderLength + igmpLength];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);
            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, IpV4Protocol.InternetGroupManagementProtocol,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, igmpLength);
            IgmpDatagram.WriteQueryVersion3(buffer, EthernetDatagram.HeaderLength + ipHeaderLength,
                                            igmpMaxResponseTime, igmpGroupAddress, igmpIsSuppressRouterSideProcessing, igmpQueryRobustnessVariable,
                                            igmpQueryInterval, igmpSourceAddresses);
            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
        }

        /// <summary>
        /// Builds an IGMP report version 3 over IPv4 over Ethernet packet.
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
        /// <param name="igmpGroupRecords">Each Group Record is a block of fields containing information pertaining to the sender's membership in a single multicast group on the interface from which the Report is sent.</param>
        /// <returns>A packet with an IGMP report version 3 over IPv4 over Ethernet datagram.</returns>
        public static Packet EthernetIpV4IgmpReportVersion3(DateTime timestamp,
                                                            MacAddress ethernetSource, MacAddress ethernetDestination,
                                                            byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                                            byte ipV4Ttl,
                                                            IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                                            IpV4Options ipV4Options,
                                                            IEnumerable<IgmpGroupRecord> igmpGroupRecords)
        {
            int ipHeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            int igmpLength = IgmpDatagram.GetReportVersion3Length(igmpGroupRecords);
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ipHeaderLength + igmpLength];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);
            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, IpV4Protocol.InternetGroupManagementProtocol,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, igmpLength);
            IgmpDatagram.WriteReportVersion3(buffer, EthernetDatagram.HeaderLength + ipHeaderLength,
                                            igmpGroupRecords);
            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
        }

        private static Packet EthernetIpV4Igmp(DateTime timestamp,
                                               MacAddress ethernetSource, MacAddress ethernetDestination,
                                               byte ipV4TypeOfService, ushort ipV4Identification, IpV4Fragmentation ipV4Fragmentation,
                                               byte ipV4Ttl,
                                               IpV4Address ipV4SourceAddress, IpV4Address ipV4DestinationAddress,
                                               IpV4Options ipV4Options,
                                               IgmpMessageType igmpMessageType, TimeSpan igmpMaxResponseTime, IpV4Address igmpGroupAddress)
        {
            int ipHeaderLength = IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength;
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ipHeaderLength + IgmpDatagram.HeaderLength];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, EthernetType.IpV4);
            IpV4Datagram.WriteHeader(buffer, EthernetDatagram.HeaderLength,
                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                     ipV4Ttl, IpV4Protocol.InternetGroupManagementProtocol,
                                     ipV4SourceAddress, ipV4DestinationAddress,
                                     ipV4Options, IgmpDatagram.HeaderLength);
            IgmpDatagram.WriteHeader(buffer, EthernetDatagram.HeaderLength + ipHeaderLength,
                                     igmpMessageType, igmpMaxResponseTime, igmpGroupAddress);
            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
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

            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
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

            IpV4Datagram.WriteTransportChecksum(buffer, EthernetDatagram.HeaderLength, ipV4HeaderLength, (ushort)transportLength,
                                                TcpDatagram.Offset.Checksum, false);

            return new Packet(buffer, timestamp, DataLinkKind.Ethernet);
        }
    }
}