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
using PcapDotNet.Packets.Ethernet;
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
            for (int i = 0; i != 10; ++i)
            {
                // Create packets
                List<Packet> packets = new List<Packet>(CreateRandomPackets(100));

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
            IpV4,
            Udp,
            Tcp
        }

        private static IEnumerable<Packet> CreateRandomPackets(int numPackets)
        {
            Random random = new Random();
            for (int i = 0; i != numPackets; ++i)
            {
                DateTime packetTimestamp = random.NextDateTime(PacketTimestamp.MinimumPacketTimestamp, PacketTimestamp.MaximumPacketTimestamp).ToUniversalTime().ToLocalTime();
//                switch (PacketType.Tcp)
                switch (random.NextEnum<PacketType>())
                {
                    case PacketType.Ethernet:
                        yield return PacketBuilder.Ethernet(packetTimestamp,
                                                            random.NextMacAddress(), random.NextMacAddress(), random.NextEnum<EthernetType>(),
                                                            random.NextDatagram(random.Next(100)));
                        break;

                    case PacketType.IpV4:
                        yield return PacketBuilder.EthernetIpV4(packetTimestamp,
                                                                random.NextMacAddress(), random.NextMacAddress(),
                                                                random.NextByte(), random.NextUShort(), random.NextIpV4Fragmentation(),
                                                                random.NextByte(),
                                                                random.NextEnum<IpV4Protocol>(),
                                                                random.NextIpV4Address(), random.NextIpV4Address(), random.NextIpV4Options(),
                                                                random.NextDatagram(random.Next(100)));
                        break;

                    case PacketType.Udp:
                        yield return PacketBuilder.EthernetIpV4Udp(packetTimestamp,
                                                                   random.NextMacAddress(), random.NextMacAddress(),
                                                                   random.NextByte(), random.NextUShort(),
                                                                   random.NextBool() ? IpV4Fragmentation.None : random.NextIpV4Fragmentation(),
                                                                   random.NextByte(),
                                                                   random.NextIpV4Address(), random.NextIpV4Address(), random.NextIpV4Options(),
                                                                   random.NextUShort(), random.NextUShort(), random.NextBool(),
                                                                   random.NextDatagram(random.Next(100)));
                        break;

                    case PacketType.Tcp:
                        yield return PacketBuilder.EthernetIpV4Tcp(packetTimestamp,
                                                                   random.NextMacAddress(), random.NextMacAddress(),
                                                                   random.NextByte(), random.NextUShort(),
                                                                   random.NextBool() ? IpV4Fragmentation.None : random.NextIpV4Fragmentation(),
                                                                   random.NextByte(),
                                                                   random.NextIpV4Address(), random.NextIpV4Address(), random.NextIpV4Options(),
                                                                   random.NextUShort(), random.NextUShort(), random.NextUInt(), random.NextUInt(),
                                                                   random.NextFlags<TcpFlags>(), 
                                                                   random.NextUShort(), random.NextUShort(), new TcpOptions(), 
                                                                   random.NextDatagram(random.Next(100)));
                        break;
                }
            }
        }

        private static void ComparePacketsToWireshark(IEnumerable<Packet> packets)
        {
            string pcapFilename = Path.GetTempPath() + "temp.pcap";
            PacketDumpFile.Dump(pcapFilename, new PcapDataLink(DataLinkKind.Ethernet), PacketDevice.DefaultSnapshotLength, packets);

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

                    case "ip":
                        PropertyInfo ipV4Property = currentDatagram.GetType().GetProperty("IpV4");
                        if (ipV4Property == null)
                            break;
                        currentDatagram = ipV4Property.GetValue(currentDatagram);
                        CompareIpV4(layer, (IpV4Datagram)currentDatagram);
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
                        field.AssertShow(ipV4Datagram.Source.ToString());
                        break;

                    case "ip.dst":
                        field.AssertShow(ipV4Datagram.Destination.ToString());
                        break;

                    case "":
                        CompareIpV4Options(field, ipV4Datagram.Options);
                        break;
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
                                  field.Show().StartsWith("Unknown") ||
                                  field.Show().StartsWith("Security") ||
                                  field.Show().StartsWith("Router Alert (with option length = ") ||
                                  field.Show().Contains("with too") ||
                                  field.Show().Contains(" bytes says option goes past end of options") ||
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
                var optionShows = from f in field.Fields() select f.Show();

                Assert.IsTrue(optionShows.SequenceEqual(option.GetWiresharkSubfieldStrings()));
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
                    case "tcp.srcport":
                        field.AssertShowDecimal(tcpDatagram.SourcePort);
                        break;

                    case "tcp.dstport":
                        field.AssertShowDecimal(tcpDatagram.DestinationPort);
                        break;

                    case "tcp.seq":
                        field.AssertShowDecimal(tcpDatagram.SequenceNumber);
                        break;

                    case "tcp.ack":
                        field.AssertShowDecimal(tcpDatagram.AcknowledgmentNumber);
                        break;

                    case "tcp.hdr_len":
                        field.AssertShowDecimal(tcpDatagram.HeaderLength);
                        break;

                    case "tcp.flags":
                        field.AssertShowHex((byte)tcpDatagram.Flags);
                        foreach (var flagField in field.Fields())
                        {
                            switch (flagField.Name())
                            {
                                case "tcp.flags.cwr":
                                    flagField.AssertShowDecimal(tcpDatagram.IsCwr);
                                    break;

                                case "tcp.flags.ecn":
                                    flagField.AssertShowDecimal(tcpDatagram.IsEce);
                                    break;

                                case "tcp.flags.urg":
                                    flagField.AssertShowDecimal(tcpDatagram.IsUrg);
                                    break;

                                case "tcp.flags.ack":
                                    flagField.AssertShowDecimal(tcpDatagram.IsAck);
                                    break;

                                case "tcp.flags.push":
                                    flagField.AssertShowDecimal(tcpDatagram.IsPush);
                                    break;

                                case "tcp.flags.reset":
                                    flagField.AssertShowDecimal(tcpDatagram.IsReset);
                                    break;

                                case "tcp.flags.syn":
                                    flagField.AssertShowDecimal(tcpDatagram.IsSyn);
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
                }
            }
        }
    }
}