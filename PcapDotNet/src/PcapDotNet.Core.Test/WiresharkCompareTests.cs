using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.Icmp;
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

        private const bool IsRetry
//                        = true;
            = false;
        private const byte RetryNumber = 190;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

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
#pragma warning disable 162 // This code is unreachable on purpose
            if (IsRetry)
            {
                ComparePacketsToWireshark(null);
                return;
            }
#pragma warning restore 162

            Random random = new Random();
            for (int i = 0; i != 10; ++i)
            {
                // Create packets
                List<Packet> packets = new List<Packet>(CreateRandomPackets(random, 200));

                // Compare packets to wireshark
                ComparePacketsToWireshark(packets);
            }
        }

        [TestMethod]
        public void CompareTimestampPacketsToWiresharkTest()
        {
            const long Ticks = 633737178954260865;
            DateTime timestamp = new DateTime(Ticks).ToUniversalTime().ToLocalTime();

            // Create packet
            Packet packet = new Packet(new byte[14], timestamp, DataLinkKind.Ethernet);

            // Compare packet to wireshark
            ComparePacketsToWireshark(new[] { packet });

            // BUG: Limited timestamp due to Windows bug: https://connect.microsoft.com/VisualStudio/feedback/details/559198/net-4-datetime-tolocaltime-is-sometimes-wrong
            // Now check different dates.
//            packet = new Packet(new byte[14], new DateTime(2004,08,25,10,36,41, DateTimeKind.Utc).ToLocalTime(), DataLinkKind.Ethernet);
//            ComparePacketsToWireshark(new[] { packet });
        }

        [TestMethod]
        public void CompareEthernetTrailerToWiresharkTest()
        {
            const string PacketString = "001120cf0900000c29566988080045000029627b00008006de80c0a8640bc0a81477a42cc03bdd3c481c6cfcd72050104278a5a90000000e01bf0101";
            Packet packet = Packet.FromHexadecimalString(PacketString, DateTime.Now, DataLinkKind.Ethernet);
            ComparePacketsToWireshark(packet);
        }

        [TestMethod]
        public void CompareEthernetFcsToWiresharkTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(),
                                                new IpV4Layer
                                                {
                                                    Protocol = IpV4Protocol.Udp
                                                });
            byte[] buffer = new byte[packet.Length + 100];
            new Random().NextBytes(buffer);
            packet.CopyTo(buffer, 0);
            packet = new Packet(buffer, DateTime.Now, DataLinkKind.Ethernet);
            ComparePacketsToWireshark(packet);
        }

        private enum PacketType
        {
            Ethernet,
            Arp,
            IpV4,
            IpV4OverIpV4,
            Igmp,
            Icmp,
            Gre,
            Udp,
            Tcp,
            Http,
        }

        private static Packet CreateRandomPacket(Random random)
        {
            // BUG: Limited timestamp due to Windows bug: https://connect.microsoft.com/VisualStudio/feedback/details/559198/net-4-datetime-tolocaltime-is-sometimes-wrong
            DateTime packetTimestamp =
                random.NextDateTime(new DateTime(2010,1,1), new DateTime(2010,12,31)).ToUniversalTime().ToLocalTime();
                //random.NextDateTime(PacketTimestamp.MinimumPacketTimestamp, PacketTimestamp.MaximumPacketTimestamp).ToUniversalTime().ToLocalTime();
            
            EthernetLayer ethernetLayer = random.NextEthernetLayer();
            IpV4Layer ipV4Layer = random.NextIpV4Layer();
            PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(100));

            switch (random.NextEnum<PacketType>())
//            switch (PacketType.Icmp)
            {
                case PacketType.Ethernet:
                    return PacketBuilder.Build(DateTime.Now, ethernetLayer, payloadLayer);

                case PacketType.Arp:
                    ethernetLayer.EtherType = EthernetType.None;
                    ethernetLayer.Destination = MacAddress.Zero;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, random.NextArpLayer());

                case PacketType.IpV4:
                    ethernetLayer.EtherType = EthernetType.None;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, payloadLayer);

                case PacketType.IpV4OverIpV4:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, random.NextIpV4Layer(), payloadLayer);

                case PacketType.Igmp:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, random.NextIgmpLayer());

                case PacketType.Icmp:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    IcmpLayer icmpLayer = random.NextIcmpLayer();
                    IEnumerable<ILayer> icmpPayloadLayers = random.NextIcmpPayloadLayers(icmpLayer);
                    return PacketBuilder.Build(packetTimestamp, new ILayer[]{ethernetLayer, ipV4Layer, icmpLayer}.Concat(icmpPayloadLayers));

                case PacketType.Gre:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    GreLayer greLayer = random.NextGreLayer();
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, greLayer, payloadLayer);

                case PacketType.Udp:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    if (random.NextBool())
                        ipV4Layer.Fragmentation = IpV4Fragmentation.None;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, random.NextUdpLayer(), payloadLayer);

                case PacketType.Tcp:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    if (random.NextBool())
                        ipV4Layer.Fragmentation = IpV4Fragmentation.None;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, random.NextTcpLayer(), payloadLayer);

                case PacketType.Http:
                    ethernetLayer.EtherType = EthernetType.None;
                    ipV4Layer.Protocol = null;
                    if (random.NextBool())
                        ipV4Layer.Fragmentation = IpV4Fragmentation.None;
                    TcpLayer tcpLayer = random.NextTcpLayer();

                    HttpLayer httpLayer = random.NextHttpLayer();
                    if (httpLayer.IsRequest)
                        tcpLayer.DestinationPort = 80;
                    else
                        tcpLayer.SourcePort = 80;
                    return PacketBuilder.Build(packetTimestamp, ethernetLayer, ipV4Layer, tcpLayer, random.NextHttpLayer());

                default:
                    throw new InvalidOperationException();
            }
        }

        private static IEnumerable<Packet> CreateRandomPackets(Random random, int numPackets)
        {
            for (int i = 0; i != numPackets; ++i)
                yield return CreateRandomPacket(random);
        }

        private static void ComparePacketsToWireshark(params Packet[] packets)
        {
            ComparePacketsToWireshark((IEnumerable<Packet>)packets);
        }

        private static void ComparePacketsToWireshark(IEnumerable<Packet> packets)
        {
            string pcapFilename = Path.GetTempPath() + "temp." + new Random().NextByte() + ".pcap";
#pragma warning disable 162
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (!IsRetry)
            {
                PacketDumpFile.Dump(pcapFilename, new PcapDataLink(DataLinkKind.Ethernet), PacketDevice.DefaultSnapshotLength, packets);
            }
            else
            {
                pcapFilename = Path.GetTempPath() + "temp." + RetryNumber + ".pcap";
                List<Packet> packetsList = new List<Packet>();
                using (PacketCommunicator communicator = new OfflinePacketDevice(pcapFilename).Open())
                {
                    communicator.ReceivePackets(-1, packetsList.Add);
                }
                packets = packetsList;
            }
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse
#pragma warning restore 162

            // Create pdml file
            string documentFilename = pcapFilename + ".pdml";
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                                    {
                                        // Wireshark's preferences file is %APPDATA%\Wireshark\preferences
                                            FileName = WiresharkTsharkPath,
                                            Arguments = "-o udp.check_checksum:TRUE " +
                                                        "-o tcp.relative_sequence_numbers:FALSE " +
                                                        "-o tcp.analyze_sequence_numbers:FALSE " +
                                                        "-o tcp.track_bytes_in_flight:FALSE " +
                                                        "-o tcp.desegment_tcp_streams:FALSE " +
                                                        "-o tcp.check_checksum:TRUE " +
                                                        "-o http.dechunk_body:FALSE " +
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

            try
            {
                Compare(XDocument.Load(fixedDocumentFilename), packets);
            }
            catch (AssertFailedException exception)
            {
                throw new AssertFailedException("Failed comparing packets in file " + pcapFilename + ". Message: " + exception.Message, exception);
            }
        }

        private static void Compare(XDocument document, IEnumerable<Packet> packets)
        {
            IEnumerator<Packet> packetEnumerator = packets.GetEnumerator();

            // Parse XML
            int i = 1;
            foreach (var documentPacket in document.Element("pdml").Elements("packet"))
            {
                packetEnumerator.MoveNext();
                Packet packet = packetEnumerator.Current;

                try
                {
                    ComparePacket(packet, documentPacket);
                }
                catch (Exception e)
                {
                    throw new AssertFailedException("Failed comparing packet " + i + ". " + e.Message, e);
                }
                ++i;
            }
        }

        private static void ComparePacket(Packet packet, XElement documentPacket)
        {
            object currentDatagram = packet;
            CompareProtocols(currentDatagram, documentPacket);
        }

        internal static void CompareProtocols(object currentDatagram, XElement layersContainer)
        {
            foreach (var layer in layersContainer.Protocols())
            {
                switch (layer.Name())
                {
                    case "geninfo":
                        break;

                    case "frame":
                        CompareFrame(layer, (Packet)currentDatagram);
                        break;

                    default:
                        var comparer = WiresharkDatagramComparer.GetComparer(layer.Name());
                        if (comparer == null)
                            return;
                        currentDatagram = comparer.Compare(layer, currentDatagram);
                        break;
                }
            }
        }

        private static void CompareFrame(XElement frame, Packet packet)
        {
            foreach (var field in frame.Fields())
            {
                switch (field.Name())
                {
//                    case "frame.time":
//                        string fieldShow = field.Show();
//                        if (fieldShow == "Not representable")
//                            break;
//                        fieldShow = fieldShow.Substring(0, fieldShow.Length - 2);
//                        DateTime fieldTimestamp = fieldShow[4] == ' '
//                                                      ? DateTime.ParseExact(fieldShow, "MMM  d, yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture)
//                                                      : DateTime.ParseExact(fieldShow, "MMM dd, yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
//                        MoreAssert.IsInRange(fieldTimestamp.AddSeconds(-2), fieldTimestamp.AddSeconds(2), packet.Timestamp, "Timestamp");
//                        break;

                    case "frame.time_epoch":
                        double timeEpoch = double.Parse(field.Show());
                        DateTime fieldTimestamp = new DateTime(1970, 1, 1).AddSeconds(timeEpoch);
                        MoreAssert.IsInRange(fieldTimestamp.AddMilliseconds(-1), fieldTimestamp.AddMilliseconds(1), packet.Timestamp.ToUniversalTime(), "Timestamp");
                        break;

                    case "frame.len":
                        field.AssertShowDecimal(packet.Length);
                        break;
                }
            }
        }
    }
}