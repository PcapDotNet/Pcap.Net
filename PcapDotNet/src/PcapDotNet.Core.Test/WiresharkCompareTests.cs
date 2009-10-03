using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for WiresharkCompareTests
    /// </summary>
    [TestClass]
    public class WiresharkCompareTests
    {
        private const string WiresharkDiretory = @"C:\Program Files\Wireshark\";
        private const string WiresharkTsharkPath = WiresharkDiretory + @"tshark.exe";

        public WiresharkCompareTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ComparePacketsToWiresharkTest()
        {
            for (int i = 0; i != 50; ++i)
            {
                // Create packets
                List<Packet> packets = new List<Packet>(CreateRandomPackets(200));

                // Compare packets to wireshark
                ComparePacketsToWireshark(packets);
            }
        }

        [TestMethod]
        public void CompareTimestampPacketsToWiresharkTest()
        {
            const long ticks = 633737178954260865;
            DateTime timestamp = new DateTime(ticks).ToUniversalTime().ToLocalTime();

            // Create packet
            Packet packet = new Packet(new byte[14], timestamp, DataLinkKind.Ethernet);

            // Compare packet to wireshark
            ComparePacketsToWireshark(new[]{packet});
        }

        private enum PacketType
        {
            Ethernet,
            Arp,
            IpV4,
            Igmp,
            Udp,
            Tcp
        }

        private static Packet CreateRandomIgmpPacket(DateTime packetTimestamp, Random random)
        {
            switch (random.NextEnum(IgmpMessageType.None))
            {
                case IgmpMessageType.MembershipQuery:
                    switch (random.NextEnum(IgmpQueryVersion.None, IgmpQueryVersion.Unknown))
                    {
                        case IgmpQueryVersion.Version1:
                            return PacketBuilder.EthernetIpV4IgmpQueryVersion1(packetTimestamp,
                                                                               random.NextMacAddress(), random.NextMacAddress(),
                                                                               random.NextByte(), random.NextUShort(),
                                                                               IpV4Fragmentation.None, random.NextByte(),
                                                                               random.NextIpV4Address(), random.NextIpV4Address(),
                                                                               random.NextIpV4Options(),
                                                                               random.NextIpV4Address());

                        case IgmpQueryVersion.Version2:
                            return PacketBuilder.EthernetIpV4IgmpQueryVersion2(packetTimestamp,
                                                                               random.NextMacAddress(), random.NextMacAddress(),
                                                                               random.NextByte(), random.NextUShort(),
                                                                               IpV4Fragmentation.None, random.NextByte(),
                                                                               random.NextIpV4Address(), random.NextIpV4Address(),
                                                                               random.NextIpV4Options(),
                                                                               random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                                                   IgmpDatagram.MaxMaxResponseTime),
                                                                               random.NextIpV4Address());

                        case IgmpQueryVersion.Version3:
                            return PacketBuilder.EthernetIpV4IgmpQueryVersion3(packetTimestamp,
                                                                               random.NextMacAddress(), random.NextMacAddress(),
                                                                               random.NextByte(), random.NextUShort(),
                                                                               IpV4Fragmentation.None, random.NextByte(),
                                                                               random.NextIpV4Address(), random.NextIpV4Address(),
                                                                               random.NextIpV4Options(),
                                                                               random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                                                   IgmpDatagram.MaxVersion3MaxResponseTime),
                                                                               random.NextIpV4Address(), random.NextBool(),
                                                                               random.NextByte(IgmpDatagram.MaxQueryRobustnessVariable + 1),
                                                                               random.NextTimeSpan(TimeSpan.Zero,
                                                                                                   IgmpDatagram.MaxQueryInterval),
                                                                               random.NextIpV4Addresses(random.Next(100)));

                        default:
                            throw new InvalidOperationException();
                    }

                case IgmpMessageType.LeaveGroupVersion2:
                    return PacketBuilder.EthernetIpV4IgmpLeaveGroupVersion2(packetTimestamp,
                                                                            random.NextMacAddress(), random.NextMacAddress(),
                                                                            random.NextByte(), random.NextUShort(),
                                                                            IpV4Fragmentation.None, random.NextByte(),
                                                                            random.NextIpV4Address(), random.NextIpV4Address(),
                                                                            random.NextIpV4Options(),
                                                                            random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                                                IgmpDatagram.MaxMaxResponseTime),
                                                                            random.NextIpV4Address());
                case IgmpMessageType.MembershipReportVersion1:
                    return PacketBuilder.EthernetIpV4IgmpReportVersion1(packetTimestamp,
                                                                        random.NextMacAddress(), random.NextMacAddress(),
                                                                        random.NextByte(), random.NextUShort(),
                                                                        IpV4Fragmentation.None, random.NextByte(),
                                                                        random.NextIpV4Address(), random.NextIpV4Address(),
                                                                        random.NextIpV4Options(),
                                                                        random.NextIpV4Address());

                case IgmpMessageType.MembershipReportVersion2:
                    return PacketBuilder.EthernetIpV4IgmpReportVersion2(packetTimestamp,
                                                                        random.NextMacAddress(), random.NextMacAddress(),
                                                                        random.NextByte(), random.NextUShort(),
                                                                        IpV4Fragmentation.None, random.NextByte(),
                                                                        random.NextIpV4Address(), random.NextIpV4Address(),
                                                                        random.NextIpV4Options(),
                                                                        random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                                            IgmpDatagram.MaxMaxResponseTime),
                                                                        random.NextIpV4Address());

                case IgmpMessageType.MembershipReportVersion3:
                    return PacketBuilder.EthernetIpV4IgmpReportVersion3(packetTimestamp,
                                                                        random.NextMacAddress(), random.NextMacAddress(),
                                                                        random.NextByte(), random.NextUShort(),
                                                                        IpV4Fragmentation.None, random.NextByte(),
                                                                        random.NextIpV4Address(), random.NextIpV4Address(),
                                                                        random.NextIpV4Options(),
                                                                        random.NextIgmpGroupRecords(random.Next(10)));

                default:
                    throw new InvalidOperationException();
            }
        }

        private static Packet CreateRandomPacket(Random random)
        {
            DateTime packetTimestamp =
                random.NextDateTime(PacketTimestamp.MinimumPacketTimestamp, PacketTimestamp.MaximumPacketTimestamp).ToUniversalTime().ToLocalTime();

            switch (random.NextEnum<PacketType>())
            {
                case PacketType.Ethernet:
                    return PacketBuilder.Ethernet(packetTimestamp,
                                                  random.NextMacAddress(), random.NextMacAddress(), random.NextEnum<EthernetType>(),
                                                  random.NextDatagram(random.Next(100)));

                case PacketType.Arp:
                    byte hardwareAddressLength = random.NextByte();
                    byte protocolAddressLength = random.NextByte();
                    return PacketBuilder.EthernetArp(packetTimestamp,
                                                     random.NextMacAddress(),
                                                     random.NextEnum<EthernetType>(), random.NextEnum<ArpOperation>(),
                                                     random.NextBytes(hardwareAddressLength), random.NextBytes(protocolAddressLength),
                                                     random.NextBytes(hardwareAddressLength), random.NextBytes(protocolAddressLength));

                case PacketType.IpV4:
                    return PacketBuilder.EthernetIpV4(packetTimestamp,
                                                      random.NextMacAddress(), random.NextMacAddress(),
                                                      random.NextByte(), random.NextUShort(), random.NextIpV4Fragmentation(),
                                                      random.NextByte(),
                                                      random.NextEnum<IpV4Protocol>(),
                                                      random.NextIpV4Address(), random.NextIpV4Address(), random.NextIpV4Options(),
                                                      random.NextDatagram(random.Next(100)));

                case PacketType.Igmp:
                    return CreateRandomIgmpPacket(packetTimestamp, random);

                case PacketType.Udp:
                    return PacketBuilder.EthernetIpV4Udp(packetTimestamp,
                                                         random.NextMacAddress(), random.NextMacAddress(),
                                                         random.NextByte(), random.NextUShort(),
                                                         random.NextBool() ? IpV4Fragmentation.None : random.NextIpV4Fragmentation(),
                                                         random.NextByte(),
                                                         random.NextIpV4Address(), random.NextIpV4Address(), random.NextIpV4Options(),
                                                         random.NextUShort(), random.NextUShort(), random.NextBool(),
                                                         random.NextDatagram(random.Next(100)));

                case PacketType.Tcp:
                    return PacketBuilder.EthernetIpV4Tcp(packetTimestamp,
                                                         random.NextMacAddress(), random.NextMacAddress(),
                                                         random.NextByte(), random.NextUShort(),
                                                         random.NextBool() ? IpV4Fragmentation.None : random.NextIpV4Fragmentation(),
                                                         random.NextByte(),
                                                         random.NextIpV4Address(), random.NextIpV4Address(), random.NextIpV4Options(),
                                                         random.NextUShort(), random.NextUShort(), random.NextUInt(), random.NextUInt(),
                                                         random.NextFlags<TcpControlBits>(),
                                                         random.NextUShort(), random.NextUShort(),
                                                         random.NextTcpOptions(),
                                                         random.NextDatagram(random.Next(100)));

                default:
                    throw new InvalidOperationException();
            }
        }

        private static IEnumerable<Packet> CreateRandomPackets(int numPackets)
        {
            Random random = new Random();
            for (int i = 0; i != numPackets; ++i)
                yield return CreateRandomPacket(random);
        }

        private static void ComparePacketsToWireshark(IEnumerable<Packet> packets)
        {
            string pcapFilename = Path.GetTempPath() + "temp.pcap";
            PacketDumpFile.Dump(pcapFilename, new PcapDataLink(DataLinkKind.Ethernet), PacketDevice.DefaultSnapshotLength, packets);
//            List<Packet> packetsList = new List<Packet>();
//            new OfflinePacketDevice(pcapFilename).Open().ReceivePackets(1000, packetsList.Add);
//            packets = packetsList;

            // Create pdml file
            string documentFilename = pcapFilename + ".pdml";
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                                        {
                                            FileName = WiresharkTsharkPath,
                                            Arguments = "-o udp.check_checksum:TRUE " +
                                                        "-o tcp.relative_sequence_numbers:FALSE " +
                                                        "-o tcp.analyze_sequence_numbers:FALSE " +
                                                        "-o tcp.track_bytes_in_flight:FALSE " +
                                                        "-o tcp.desegment_tcp_streams:FALSE " +
                                                        "-o tcp.check_checksum:TRUE " +
                                                        "-t r -n -r \"" + pcapFilename + "\" -T pdml",
                                            WorkingDirectory = WiresharkDiretory,
                                            UseShellExecute = false,
                                            RedirectStandardOutput = true,
                                            CreateNoWindow = true
                                        };
                Console.WriteLine("Starting process " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                File.WriteAllText(documentFilename, output);
            }

            // Fix pdml file
            string fixedDocumentFilename = documentFilename + ".fixed";
            File.WriteAllBytes(fixedDocumentFilename, new List<byte>(from b in File.ReadAllBytes(documentFilename)
                                                                     select b == 0x0A || b == 0x0D
                                                                                ? b
                                                                                : Math.Max((byte)0x20, Math.Min(b, (byte)0x7F))).ToArray());

            Compare(XDocument.Load(fixedDocumentFilename), packets);
        }

        private static void Compare(XDocument document, IEnumerable<Packet> packets)
        {
            IEnumerator<Packet> packetEnumerator = packets.GetEnumerator();

            // Parse XML
            int i = 1;
            foreach (var documentPacket in document.Element("pdml").Elements("packet"))
            {
                Console.WriteLine("Checking packet " + i);
                packetEnumerator.MoveNext();
                Packet packet = packetEnumerator.Current;

                ComparePacket(packet, documentPacket);
                ++i;
            }
        }

        private static void ComparePacket(Packet packet, XElement documentPacket)
        {
            object currentDatagram = packet;
            foreach (var layer in documentPacket.Elements("proto"))
            {
                switch (layer.Name())
                {
                    case "geninfo":
                        break;

                    case "frame":
                        CompareFrame(layer, packet);
                        break;

                    case "eth":
                        PropertyInfo ethernetProperty = currentDatagram.GetType().GetProperty("Ethernet");
                        if (ethernetProperty == null)
                            break;
                        currentDatagram = ethernetProperty.GetValue(currentDatagram);
                        CompareEtherent(layer, (EthernetDatagram)currentDatagram);
                        break;

                    case "arp":
                        PropertyInfo arpProperty = currentDatagram.GetType().GetProperty("Arp");
                        if (arpProperty == null)
                            break;
                        currentDatagram = arpProperty.GetValue(currentDatagram);
                        CompareArp(layer, (ArpDatagram)currentDatagram);
                        break;

                    case "ip":
                        PropertyInfo ipV4Property = currentDatagram.GetType().GetProperty("IpV4");
                        if (ipV4Property == null)
                            break;
                        currentDatagram = ipV4Property.GetValue(currentDatagram);
                        CompareIpV4(layer, (IpV4Datagram)currentDatagram);
                        break;

                    case "igmp":
                        PropertyInfo igmpProperty = currentDatagram.GetType().GetProperty("Igmp");
                        if (igmpProperty == null)
                            break;
                        currentDatagram = igmpProperty.GetValue(currentDatagram);
                        CompareIgmp(layer, (IgmpDatagram)currentDatagram);
                        break;

                    case "udp":
                        PropertyInfo udpProperty = currentDatagram.GetType().GetProperty("Udp");
                        if (udpProperty == null)
                            break;
                        {
                            Datagram ipDatagram = (Datagram)currentDatagram;
                            currentDatagram = udpProperty.GetValue(currentDatagram);
                            CompareUdp(layer, (IpV4Datagram)ipDatagram);
                        }
                        break;

                    case "tcp":
                        PropertyInfo tcpProperty = currentDatagram.GetType().GetProperty("Tcp");
                        if (tcpProperty == null)
                            break;
                        {
                            Datagram ipDatagram = (Datagram)currentDatagram;
                            currentDatagram = tcpProperty.GetValue(currentDatagram);
                            CompareTcp(layer, (IpV4Datagram)ipDatagram);
                        }
                        break;

                    default:
                        return;
                }
            }
        }

        private static void CompareFrame(XElement frame, Packet packet)
        {
            foreach (var field in frame.Fields())
            {
                switch (field.Name())
                {
                    case "frame.time":
                        string fieldShow = field.Show();
                        if (fieldShow == "Not representable")
                            break;
                        fieldShow = fieldShow.Substring(0, fieldShow.Length - 2);
                        DateTime fieldTimestamp = fieldShow[4] == ' '
                                                      ? DateTime.ParseExact(fieldShow, "MMM  d, yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture)
                                                      : DateTime.ParseExact(fieldShow, "MMM dd, yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
                        MoreAssert.IsInRange(fieldTimestamp.AddSeconds(-2), fieldTimestamp.AddSeconds(2), packet.Timestamp);
                        break;

                    case "frame.len":
                        field.AssertShowDecimal(packet.Length);
                        break;
                }
            }
        }

        private static void CompareEtherent(XElement ethernet, EthernetDatagram ethernetDatagram)
        {
            foreach (var field in ethernet.Fields())
            {
                switch (field.Name())
                {
                    case "eth.dst":
                        CompareEthernetAddress(field, ethernetDatagram.Destination);
                        break;

                    case "eth.src":
                        CompareEthernetAddress(field, ethernetDatagram.Source);
                        break;

                    case "eth.type":
                        field.AssertShowHex((ushort)ethernetDatagram.EtherType);
                        break;

                    case "eth.trailer":
                    case "":
                        break;

                    default:
                        throw new InvalidOperationException("Invalid etherent field " + field.Name());
                }
            }
        }

        private static void CompareArp(XElement arp, ArpDatagram arpDatagram)
        {
            foreach (var field in arp.Fields())
            {
                switch (field.Name())
                {
                    case "arp.hw.type":
                        field.AssertShowHex((ushort)arpDatagram.HardwareType);
                        break;

                    case "arp.proto.type":
                        field.AssertShowHex((ushort)arpDatagram.ProtocolType);
                        break;

                    case "arp.hw.size":
                        field.AssertShowDecimal(arpDatagram.HardwareLength);
                        break;

                    case "arp.proto.size":
                        field.AssertShowDecimal(arpDatagram.ProtocolLength);
                        break;

                    case "arp.opcode":
                        field.AssertShowHex((ushort)arpDatagram.Operation);
                        break;

                    case "arp.src.hw":
                    case "arp.src.hw_mac":
                        field.AssertShow(arpDatagram.SenderHardwareAddress.BytesSequenceToHexadecimalString(":"));
                        break;


                    case "arp.src.proto":
                        field.AssertShow(arpDatagram.SenderProtocolAddress.BytesSequenceToHexadecimalString(":"));
                        break;

                    case "arp.src.proto_ipv4":
                        field.AssertShow(arpDatagram.SenderProtocolIpV4Address.ToString());
                        break;

                    case "arp.dst.hw":
                    case "arp.dst.hw_mac":
                        field.AssertShow(arpDatagram.TargetHardwareAddress.BytesSequenceToHexadecimalString(":"));
                        break;

                    case "arp.dst.proto":
                        field.AssertShow(arpDatagram.TargetProtocolAddress.BytesSequenceToHexadecimalString(":"));
                        break;

                    case "arp.dst.proto_ipv4":
                        field.AssertShow(arpDatagram.TargetProtocolIpV4Address.ToString());
                        break;

                    case "arp.isgratuitous":
                        break;

                    default:
                        throw new InvalidOperationException("Invalid arp field " + field.Name());
                }
            }
        }

        private static void CompareEthernetAddress(XElement element, MacAddress address)
        {
            foreach (var field in element.Fields())
            {
                switch (field.Name())
                {
                    case "eth.addr":
                        field.AssertShow(address.ToString().ToLower());
                        break;

                    case "eth.ig":
                    case "eth.lg":
                        break;

                    default:
                        throw new InvalidOperationException("Invalid ethernet address field " + field.Name());
                }
            }
        }

        private static void CompareIpV4(XElement ethernet, IpV4Datagram ipV4Datagram)
        {
            foreach (var field in ethernet.Fields())
            {
                switch (field.Name())
                {
                    case "ip.version":
                        field.AssertShowDecimal(ipV4Datagram.Version);
                        break;

                    case "ip.hdr_len":
                        field.AssertShowDecimal(ipV4Datagram.HeaderLength);
                        break;

                    case "ip.dsfield":
                        field.AssertShowDecimal((int)ipV4Datagram.TypeOfService);
                        break;

                    case "ip.len":
                        field.AssertShowDecimal(ipV4Datagram.TotalLength);
                        break;

                    case "ip.id":
                        field.AssertShowHex(ipV4Datagram.Identification);
                        break;

                    case "ip.flags":
                        field.AssertShowHex((byte)((ushort)ipV4Datagram.Fragmentation.Options >> 12));
                        break;

                    case "ip.frag_offset":
                        field.AssertShowDecimal(ipV4Datagram.Fragmentation.Offset);
                        break;

                    case "ip.ttl":
                        field.AssertShowDecimal(ipV4Datagram.Ttl);
                        break;

                    case "ip.proto":
                        field.AssertShowHex((byte)ipV4Datagram.Protocol);
                        break;

                    case "ip.checksum":
                        field.AssertShowHex(ipV4Datagram.HeaderChecksum);
                        foreach (var checksumField in field.Fields())
                        {
                            switch (checksumField.Name())
                            {
                                case "ip.checksum_good":
                                    checksumField.AssertShowDecimal(ipV4Datagram.IsHeaderChecksumCorrect);
                                    break;

                                case "ip.checksum_bad":
                                    if (ipV4Datagram.Length < IpV4Datagram.HeaderMinimumLength ||
                                        ipV4Datagram.Length < ipV4Datagram.HeaderLength)
                                        break;

                                    checksumField.AssertShowDecimal(!ipV4Datagram.IsHeaderChecksumCorrect);
                                    break;
                            }
                        }
                        break;

                    case "ip.src":
                    case "ip.src_host":
                        field.AssertShow(ipV4Datagram.Source.ToString());
                        break;

                    case "ip.dst":
                    case "ip.dst_host":
                        field.AssertShow(ipV4Datagram.Destination.ToString());
                        break;

                    case "ip.addr":
                    case "ip.host":
                        Assert.IsTrue(field.Show() == ipV4Datagram.Source.ToString() ||
                                      field.Show() == ipV4Datagram.Destination.ToString());
                        break;

                    case "":
                        CompareIpV4Options(field, ipV4Datagram.Options);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid ip field " + field.Name());
                }
            }
        }

        private static void CompareIpV4Options(XElement element, IpV4Options options)
        {
            int currentOptionIndex = 0;
            foreach (var field in element.Fields())
            {
                if (currentOptionIndex >= options.Count)
                {
                    Assert.IsFalse(options.IsValid);
                    Assert.IsTrue(field.Show() == "Commercial IP security option" ||
                                  field.Show() == "Loose source route (length byte past end of options)" ||
                                  field.Show().StartsWith("Unknown") ||
                                  field.Show().StartsWith("Security") ||
                                  field.Show().StartsWith("Router Alert (with option length = ") ||
                                  field.Show().StartsWith("Stream identifier (with option length = ") ||
                                  field.Show().Contains("with too") ||
                                  field.Show().Contains(" bytes says option goes past end of options") ||
                                  field.Fields().First().Show().StartsWith("Pointer: ") && field.Fields().First().Show().EndsWith(" (points to middle of address)") ||
                                  field.Fields().Where(value => value.Show() == "(suboption would go past end of option)").Count() != 0, field.Show());
                    break;
                }
                IpV4Option option = options[currentOptionIndex++];
                if (option.OptionType == IpV4OptionType.BasicSecurity ||
                    option.OptionType == IpV4OptionType.TraceRoute)
                {
                    Assert.IsTrue(field.Show().StartsWith(option.GetWiresharkString()));
                    continue; // Wireshark doesn't support 
                }
                field.AssertShow(option.GetWiresharkString());

                if ((option is IpV4OptionUnknown)) 
                    continue;

                var optionShows = from f in field.Fields() select f.Show();
                MoreAssert.AreSequenceEqual(optionShows, option.GetWiresharkSubfieldStrings());
            }
        }

        private static void CompareIgmp(XElement igmp, IgmpDatagram igmpDatagram)
        {
            int groupRecordIndex = 0;
            int sourceAddressIndex = 0;
            foreach (var field in igmp.Fields())
            {
                switch (field.Name())
                {
                    case "igmp.version":
                        field.AssertShowDecimal(igmpDatagram.Version);
                        break;

                    case "igmp.type":
                        field.AssertShowHex((byte)igmpDatagram.MessageType);
                        break;

                    case "igmp.checksum":
                        field.AssertShowHex(igmpDatagram.Checksum);
                        break;

                    case "igmp.maddr":
                        field.AssertShow(igmpDatagram.GroupAddress.ToString());
                        break;

                    case "igmp.max_resp":
                        field.AssertShowDecimal((int)((igmpDatagram.MaxResponseTime.TotalSeconds + 0.05) * 10));
                        break;

                    case "igmp.checksum_bad":
                        field.AssertShowDecimal(!igmpDatagram.IsChecksumCorrect);
                        break;

                    case "igmp.num_grp_recs":
                        field.AssertShowDecimal(igmpDatagram.NumberOfGroupRecords);
                        break;

                    case "":
                        switch (igmpDatagram.MessageType)
                        {
                            case IgmpMessageType.MembershipReportVersion3:
                                CompareIgmpGroupRecord(field, igmpDatagram.GroupRecords[groupRecordIndex++]);
                                break;

                            case IgmpMessageType.MembershipQuery:
                                CompareIgmp(field, igmpDatagram);
                                break;

                            default:
                                throw new InvalidOperationException("Invalid message type " + igmpDatagram.MessageType);
                        }

                        break;

                    case "igmp.s":
                        field.AssertShowDecimal(igmpDatagram.IsSuppressRouterSideProcessing);
                        break;

                    case "igmp.qrv":
                        field.AssertShowDecimal(igmpDatagram.QueryRobustnessVariable);
                        break;

                    case "igmp.qqic":
                        field.AssertShowDecimal(igmpDatagram.QueryIntervalCode);
                        break;

                    case "igmp.num_src":
                        field.AssertShowDecimal(igmpDatagram.NumberOfSources);
                        break;

                    case "igmp.saddr":
                        field.AssertShow(igmpDatagram.SourceAddresses[sourceAddressIndex++].ToString());
                        break;

                    default:
                        throw new InvalidOperationException("Invalid igmp field " + field.Name());
                }
            }
        }

        private static void CompareIgmpGroupRecord(XElement groupRecord, IgmpGroupRecordDatagram groupRecordDatagram)
        {
            int sourceAddressIndex = 0;
            foreach (var field in groupRecord.Fields())
            {
                switch (field.Name())
                {
                    case "igmp.record_type":
                        field.AssertShowDecimal((byte)groupRecordDatagram.RecordType);
                        break;

                    case "igmp.aux_data_len":
                        field.AssertShowDecimal(groupRecordDatagram.AuxiliaryDataLength / 4);
                        break;

                    case "igmp.num_src":
                        field.AssertShowDecimal(groupRecordDatagram.NumberOfSources);
                        break;

                    case "igmp.maddr":
                        field.AssertShow(groupRecordDatagram.MulticastAddress.ToString());
                        break;

                    case "igmp.saddr":
                        field.AssertShow(groupRecordDatagram.SourceAddresses[sourceAddressIndex++].ToString());
                        break;

                    case "igmp.aux_data":
                        field.AssertShow(groupRecordDatagram.AuxiliaryData.BytesSequenceToHexadecimalString(":"));
                        break;

                    default:
                        throw new InvalidOperationException("Invalid igmp group record field " + field.Name());
                }
            }
        }

        private static void CompareUdp(XElement udpElement, IpV4Datagram ipV4Datagram)
        {
            UdpDatagram udpDatagram = ipV4Datagram.Udp;

            foreach (var field in udpElement.Fields())
            {
                switch (field.Name())
                {
                    case "udp.srcport":
                        field.AssertShowDecimal(udpDatagram.SourcePort);
                        break;

                    case "udp.dstport":
                        field.AssertShowDecimal(udpDatagram.DestinationPort);
                        break;

                    case "udp.port":
                        Assert.IsTrue(ushort.Parse(field.Show()) == udpDatagram.SourcePort ||
                                      ushort.Parse(field.Show()) == udpDatagram.DestinationPort);
                        break;

                    case "udp.length":
                        field.AssertShowDecimal(udpDatagram.TotalLength);
                        break;

                    case "udp.checksum":
                        field.AssertShowHex(udpDatagram.Checksum);
                        if (udpDatagram.Checksum != 0)
                        {
                            foreach (var checksumField in field.Fields())
                            {
                                switch (checksumField.Name())
                                {
                                    case "udp.checksum_good":
                                        checksumField.AssertShowDecimal(ipV4Datagram.IsTransportChecksumCorrect);
                                        break;

                                    case "udp.checksum_bad":
                                        checksumField.AssertShowDecimal(!ipV4Datagram.IsTransportChecksumCorrect);
                                        break;
                                }
                            }
                        }
                        break;

                    case "udp.checksum_coverage":
                        field.AssertShowDecimal(udpDatagram.Length);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid udp field " + field.Name());
                }
            }
        }

        private static void CompareTcp(XElement tcpElement, IpV4Datagram ipV4Datagram)
        {
            TcpDatagram tcpDatagram = ipV4Datagram.Tcp;

            foreach (var field in tcpElement.Fields())
            {
                switch (field.Name())
                {
                    case "tcp.len":
                        field.AssertShowDecimal(tcpDatagram.Payload.Length);
                        break;

                    case "tcp.srcport":
                        field.AssertShowDecimal(tcpDatagram.SourcePort);
                        break;

                    case "tcp.dstport":
                        field.AssertShowDecimal(tcpDatagram.DestinationPort);
                        break;

                    case "tcp.port":
                        Assert.IsTrue(ushort.Parse(field.Show()) == tcpDatagram.SourcePort ||
                                      ushort.Parse(field.Show()) == tcpDatagram.DestinationPort);
                        break;


                    case "tcp.seq":
                        field.AssertShowDecimal(tcpDatagram.SequenceNumber);
                        break;

                    case "tcp.nxtseq":
                        field.AssertShowDecimal(tcpDatagram.NextSequenceNumber);
                        break;

                    case "tcp.ack":
                        field.AssertShowDecimal(tcpDatagram.AcknowledgmentNumber);
                        break;

                    case "tcp.hdr_len":
                        field.AssertShowDecimal(tcpDatagram.HeaderLength);
                        break;

                    case "tcp.flags":
                        field.AssertShowHex((byte)tcpDatagram.ControlBits);
                        foreach (var flagField in field.Fields())
                        {
                            switch (flagField.Name())
                            {
                                case "tcp.flags.cwr":
                                    flagField.AssertShowDecimal(tcpDatagram.IsCongestionWindowReduced);
                                    break;

                                case "tcp.flags.ecn":
                                    flagField.AssertShowDecimal(tcpDatagram.IsExplicitCongestionNotificationEcho);
                                    break;

                                case "tcp.flags.urg":
                                    flagField.AssertShowDecimal(tcpDatagram.IsUrgent);
                                    break;

                                case "tcp.flags.ack":
                                    flagField.AssertShowDecimal(tcpDatagram.IsAcknowledgment);
                                    break;

                                case "tcp.flags.push":
                                    flagField.AssertShowDecimal(tcpDatagram.IsPush);
                                    break;

                                case "tcp.flags.reset":
                                    flagField.AssertShowDecimal(tcpDatagram.IsReset);
                                    break;

                                case "tcp.flags.syn":
                                    flagField.AssertShowDecimal(tcpDatagram.IsSynchronize);
                                    break;

                                case "tcp.flags.fin":
                                    flagField.AssertShowDecimal(tcpDatagram.IsFin);
                                    break;
                            }
                        }
                        break;

                    case "tcp.window_size":
                        field.AssertShowDecimal(tcpDatagram.Window);
                        break;

                    case "tcp.checksum":
                        field.AssertShowHex(tcpDatagram.Checksum);
                        foreach (var checksumField in field.Fields())
                        {
                            switch (checksumField.Name())
                            {
                                case "tcp.checksum_good":
                                    checksumField.AssertShowDecimal(ipV4Datagram.IsTransportChecksumCorrect);
                                    break;

                                case "tcp.checksum_bad":
                                    checksumField.AssertShowDecimal(!ipV4Datagram.IsTransportChecksumCorrect);
                                    break;
                            }
                        }
                        break;

                    case "tcp.urgent_pointer":
                        field.AssertShowDecimal(tcpDatagram.UrgentPointer);
                        break;

                    case "tcp.options":
                        CompareTcpOptions(field, tcpDatagram.Options);
                        break;

                    case "tcp.stream":
                    case "tcp.pdu.size":
                    case "":
                        break;

                    default:
                        throw new InvalidOperationException("Invalid tcp field " + field.Name());
                }
            }
        }

        private static void CompareTcpOptions(XElement element, TcpOptions options)
        {
            int currentOptionIndex = 0;
            foreach (var field in element.Fields())
            {
                if (currentOptionIndex >= options.Count)
                {
                    Assert.IsFalse(options.IsValid);
                    Assert.Fail();
                }

                TcpOption option = options[currentOptionIndex];
                switch (field.Name())
                {
                    case "":
                        if (option.OptionType == (TcpOptionType)21)
                            Assert.IsTrue(field.Show().StartsWith(option.GetWiresharkString()));
                        else
                            field.AssertShow(option.GetWiresharkString());
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.mss":
                        field.AssertShowDecimal(option is TcpOptionMaximumSegmentSize);
                        break;

                    case "tcp.options.mss_val":
                        field.AssertShowDecimal(((TcpOptionMaximumSegmentSize)option).MaximumSegmentSize);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.wscale":
                        field.AssertShowDecimal(option is TcpOptionWindowScale);
                        break;

                    case "tcp.options.wscale_val":
                        field.AssertShowDecimal(((TcpOptionWindowScale)option).ScaleFactorLog);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.echo":
                        Assert.IsTrue(option is TcpOptionEchoReply || option is TcpOptionEcho);
                        field.AssertShowDecimal(1);
                        break;

                    case "tcp.options.time_stamp":
                        Assert.IsTrue(option is TcpOptionTimestamp);
                        field.AssertShowDecimal(1);
                        break;

                    case "tcp.options.cc":
                        Assert.IsTrue(option is TcpOptionConnectionCountBase);
                        field.AssertShowDecimal(1);
                        break;

                    case "tcp.options.scps.vector":
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
                        if (field.Show() == "0")
                            ++currentOptionIndex;
                        break;

                    case "tcp.options.scps":
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.snack":
                    case "tcp.options.snack.offset":
                    case "tcp.options.snack.size":
                        Assert.AreEqual((TcpOptionType)21, option.OptionType);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid tcp options field " + field.Name());
                }

                if ((option is TcpOptionUnknown))
                    continue;
                
                var optionShows = from f in field.Fields() select f.Show();
                MoreAssert.AreSequenceEqual(optionShows, option.GetWiresharkSubfieldStrings());
            }
        }
    }
}