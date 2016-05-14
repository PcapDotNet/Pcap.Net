using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for WiresharkCompareTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WiresharkCompareTests
    {
        private const string WiresharkDiretory = @"C:\Program Files\Wireshark\";
        private const string WiresharkTsharkPath = WiresharkDiretory + @"tshark.exe";

        private static bool IsRetry
        {
            get { return RetryNumber != -1; }
        }

        private const int RetryNumber = -1;

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

            int seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            Random random = new Random(seed);

            try
            {
                for (int i = 0; i != 10; ++i)
                {
                    List<Packet> packets;
                    try
                    {
                        // Create packets
                        packets = new List<Packet>(CreateRandomPackets(random, 200));
                    }
                    catch (Exception exception)
                    {
                        throw new AssertFailedException("Failed creating packets. " + exception, exception);
                    }

                    try
                    {
                        // Compare packets to wireshark
                        ComparePacketsToWireshark(packets);
                    }
                    catch (Exception exception)
                    {
                        throw new AssertFailedException("Failed comparing packets. " + exception, exception);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new AssertFailedException("Failed test with seed " + seed + ". " + exception, exception);
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
            ComparePacketsToWireshark(packet);

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
        public void CompareVLanTaggedFrameTrailerToWiresharkTest()
        {
            const string PacketString =
                "0004f2402ffca870a5002e02810001f408060001080006040001a870a5002e02ac141401000000000000ac1414670000000000000000000000000000";
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

        [TestMethod]
        public void CompareHInvalidHttpRequestUriToWiresharkTest()
        {
            Packet packet = Packet.FromHexadecimalString(
                "0013f7a44dc0000c2973b9bb0800450001642736400080060000c0a80126c0002b0a12710050caa94b1f450b454950180100ae2f0000474554202f442543332542437273742" +
                "f3fefbca120485454502f312e310d0a4163636570743a20746578742f68746d6c2c206170706c69636174696f6e2f7868746d6c2b786d6c2c202a2f2a0d0a52656665726572" +
                "3a20687474703a2f2f6c6f6f6b6f75742e6e65742f746573742f6972692f6d6978656e632e7068700d0a4163636570742d4c616e67756167653a20656e2d55530d0a5573657" +
                "22d4167656e743a204d6f7a696c6c612f352e302028636f6d70617469626c653b204d53494520392e303b2057696e646f7773204e5420362e313b20574f5736343b20547269" +
                "64656e742f352e30290d0a4163636570742d456e636f64696e673a20677a69702c206465666c6174650d0a486f73743a207777772e6578616d706c652e636f6d0d0a436f6e6" +
                "e656374696f6e3a204b6565702d416c6976650d0a0d0a",
                DateTime.Now, DataLinkKind.Ethernet);

            ComparePacketsToWireshark(packet);
        }

        [TestMethod]
        public void CompareWiresharkSupportedDNSLocPacketTest()
        {
            Packet packet = Packet.FromHexadecimalString(
                "0b1b92a8f700de93ce5ea8b68100c0cd08004ac50459df790000ea110b47541a980dd85201b0013408413c1ec33482820896810101010001000014eb45a8043100000b969551000400050002000506656f6c686c710000270000017604646a6c630377766d0970617969667874796e026278067771796d686d0000fb00fe046479696907636777686566780468636b6f027470066965746e6c6208706970677170766a09706176676974746a7902716a08777177697475746e00003b000100001f000102756c076d79667769686f046b6f676b04736c79690376627003767275086175666a726a657203786c79096c666964656271796f0000270004497b4aff002b09796b6364646d626e680765617776667369056d696a6a750266710665756c687174076b7470667578770004656468660000200003210deee900064ada961b83bb0476777275046e67646c046464726e0478666a63036b736f02686f0466756574026963000066000060c2b81d0023fa537aa79bafc900ba72baf5109e3cd3900a2fa97980d88cac526248b486c7e708d1220171076c65616371716c00002200ff06e1d59b00160095c474277a7aeefa494c804b33abd3ed3e7b567e43066a637663676402686b0175000063000445c8d093002a00056b67512e7b06fb4d96e0cb6f01440736025e9cf57be200000005d39c2779b00894d4ea17f164ea86087964636d70767961057274786277046c66666a08696b6f766966696e0974616561766a79647702776800800100fe404b8ca8002c8b8b07005909d78ce4c9b7582b0eb691904fe0f562a7356e188112e9d3d2420efb08f8aa9ec31a0ce73dfc5201740769767974786c71066566766d646400002300030ea3fe8700576cf8a57f094445474c50525355581749009cb5291318a58b11c13f1b67014114f984e9cb7cbb1647b4cded67aedb210dacedc13b42c4a6f9e11e4a61d60164046b687667076970736368626c02747902697103706a740008797965636b7763730171076d726b6865776c09686c6873737879766d066b766c766a70056177646a6c06686f757877680000000001716661fe002a7e21461c5c9fcc412e968302e8bd7966b1a9e4d69d5ac4d753a6a1bfa8b9c949edf944b0e75277dc307008636172746375706f0976666e6b617568616b03757467036a686e000005000062036c830003016e0003786470027364066b62716e747500001d00ff3adb54a5001000376774db335718878e747dc84fa05f0476736d79076e62646a706872077466627471656606736f6571696600001000fe2becff97002e014a08292c1e697201885d025b9101ed07758dffc47e5d5c020c6801230352df0b07d5296bab440b19041f23240a0474777579066e667478697509687269646176636e790001000001725d2830006963c39426563937c3dfa290cf45fd9bc7d2d13d2c502555bd94b8eda68e760974d18a23c25df1a4b207973f14cb7f2238e2b4e4eaca55780b2c294da64c903469991bfdbad9d869e02c8a25b8e540f21495b4c3da911eaa7c1118960da4fd667663d57aaf82675f0a4b",
                DateTime.Now, DataLinkKind.Ethernet);

            ComparePacketsToWireshark(packet);
        }

        [TestMethod]
        public void CompareIpV4DataLinkToWiresharkTest()
        {
            ComparePacketsToWireshark(
                // Normal.
                Packet.FromHexadecimalString("46000028000000000102c48601487eebe0000016940400002200f9010000000104000000e00000fc", DateTime.Now, DataLinkKind.IpV4),
                // dns.response_to.
                Packet.FromHexadecimalString(
                    "45000107400100003a110e4adc9fd4c801487eeb0035c8db00f3ead1fb96818000010001000500050436746f340469707636096d6963726f736f667403636f6d0000010001c00c0001000100000dbb00",
                    DateTime.Now, DataLinkKind.IpV4),
                // TCP Checksum is bad because of too big IP total length.
                Packet.FromHexadecimalString(
                    "450000a8392040003706944a4a7dab3501487eeb0050cb50800f365664e4726250180089df90000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                    DateTime.Now, DataLinkKind.IpV4),
                // TCP Checksum is zero.
                Packet.FromHexadecimalString(
                    "450005dc54224000370674144a7dab3501487eeb0050cb5080a8136c64e4726250100089000000002db2707095328b271a9acf128e85be789f0e5ea6cb9d6f13f32481f6baf855420b60fe5c4053407e",
                    DateTime.Now, DataLinkKind.IpV4));
        }

        [TestMethod]
        public void CompareLinuxSllDataLinkToWiresharkTest()
        {
            ComparePacketsToWireshark(
                Packet.FromHexadecimalString(
                    "000402120000000000000000000008004500003996e1400040118e5a1bfd9042ca209f18140300350025668c10e3010000010000000000000169057974696d67",
                    DateTime.Now, new DataLink(DataLinkKind.LinuxSll)));
        }

        [TestMethod]
        public void CompareMdnsQueryToWiresharkTest()
        {
            ComparePacketsToWireshark(
                Packet.FromHexadecimalString(
                    "9221e942a2a35fe13e477f6d080047ef03095fa200003f117cef97257914f7ef2dc08804d67189037c0014e914eb02edce8b92b103b0000300050002000208797773697971696701660176046c78736d01780867716973656a6f6e07726b6b6563747703766e67026e7600003900ff086a776d7871626f690561656f6374046c7573720372616b066f717072756f0000fa0003076577656c74636e0469687178016c000003000306656a6c766a7904766e786406666a6f706869096f7467716479706d64066b71736d726e086b6963747171786b01790774766b636f656b0977726976736c68666300000900013406fb7a00070575746578670001690279790872676669626e62740663626c6a686a047563737502796302646900000b0004612e5287000d4c0cbd4134281019613edfe69d06646c76766d72046466657200003900fe15ed69250038fe22370c90f301b65c0ea97d6a0b441061ea7586f14b7deb653febba13d87cf9e9f3f09777b40c8599edb6ab467e60914419632ebaa03325036e7978036e766a00002800043c425d7600283f8aaddeefed0494d1a5e6de5975fa744e3e48c0cb6dfbe6ba7d5249311a95e9e976977fb550cab307646c736567687300001e00043be655710031087364646379746462096a70717865636e676f096a636a6d66696c6873016405627369617902706500b64f3e9ec623719b066277726b6a62096a6a68736a7178746e046b777576057770646367066a6c666b736b00000c00015006760d000100026471047071696a08716173747666697408667975676177636806666f656d6e740000fe00fe4b61a27e00467ec11db77e8d0ef41eda501b46a903087e497a411870b17e6e1a0e07661b65d9f67bbc48ebfa9f2001a96ece1e347dd0b0a1d4e72c3c4d0e109f49647f078e02747625caedbb036e76680772676364697770096a7178696864686f7602716c0000ff0000713ccfa7000cb2e3f484503cd3fcde0b2d3f097572696e6a797365650270770468667570086474616d6273696305676d61656400000c000072b2e660001a09646f6d646f78706e680674746c797164077869707066676b00",
                    DateTime.Now, new DataLink(DataLinkKind.Ethernet)));
        }

        [TestMethod]
        public void CompareTcpZeroChecksumToWiresharkTest()
        {
            ComparePacketsToWireshark(
                PacketBuilder.Build(DateTime.Now,
                                    new EthernetLayer(),
                                    new IpV4Layer
                                    {
                                        Ttl = 128
                                    },
                                    new TcpLayer
                                    {
                                        Checksum = 0,
                                        Window = 100,
                                    },
                                    new PayloadLayer
                                    {
                                        Data = new Datagram(new byte[10])
                                    }));
        }


        private static Packet CreateRandomPacket(Random random)
        {
            Packet packet;
            do
            {
                DateTime packetTimestamp =
                    random.NextDateTime(PacketTimestamp.MinimumPacketTimestamp, PacketTimestamp.MaximumPacketTimestamp).ToUniversalTime().ToLocalTime();

                List<ILayer> layers = new List<ILayer>();
                EthernetLayer ethernetLayer = random.NextEthernetLayer();
                layers.Add(ethernetLayer);
                CreateRandomEthernetPayload(random, ethernetLayer, layers);
                packet = PacketBuilder.Build(packetTimestamp, layers);
            } while (packet.Length > 65536);
            return packet;
        }

        private static void CreateRandomEthernetPayload(Random random, EthernetBaseLayer ethernetBaseLayer, List<ILayer> layers)
        {
            if (random.NextBool(20))
            {
                // Finish with payload.
                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(100));
                layers.Add(payloadLayer);
                return;
            }

            ethernetBaseLayer.EtherType = EthernetType.None;
            switch (random.NextInt(0, 7))
            {
                case 0: // VLanTaggedFrame.
                case 1:
                    VLanTaggedFrameLayer vLanTaggedFrameLayer = random.NextVLanTaggedFrameLayer();
                    layers.Add(vLanTaggedFrameLayer);
                    CreateRandomEthernetPayload(random, vLanTaggedFrameLayer, layers);
                    return;

                case 2: // ARP.
                    EthernetLayer ethernetLayer = (ethernetBaseLayer as EthernetLayer);
                    if (ethernetLayer != null)
                        ethernetLayer.Destination = MacAddress.Zero;
                    layers.Add(random.NextArpLayer());
                    return;

                case 3: // IPv4.
                case 4:
                    IpV4Layer ipV4Layer = random.NextIpV4Layer();
                    layers.Add(ipV4Layer);
                    CreateRandomIpPayload(random, ipV4Layer, layers);
                    return;

                case 5: // IPv6
                case 6:
                    IpV6Layer ipV6Layer = random.NextIpV6Layer(random.NextBool(20));
                    layers.Add(ipV6Layer);
                    CreateRandomIpPayload(random, ipV6Layer, layers);
                    return;

                default:
                    throw new InvalidOperationException("Invalid value.");
            }
        }

        private static void CreateRandomIpPayload(Random random, Layer ipLayer, List<ILayer> layers)
        {
            IpV6Layer ipV6Layer = ipLayer as IpV6Layer;
            if (ipV6Layer != null)
            {
                var headers = ipV6Layer.ExtensionHeaders.Headers;
                if (headers.Any() && headers.Last().Protocol == IpV4Protocol.EncapsulatingSecurityPayload)
                    return;
            }

            if (ipV6Layer != null ? ipV6Layer.LastNextHeader != null : random.NextBool(20))
            {
                // Finish with payload.
                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(100));
                layers.Add(payloadLayer);
                return;
            }

            IpV4Layer ipV4Layer = ipLayer as IpV4Layer;
            if (ipV4Layer != null)
            {
                ipV4Layer.Protocol = null;
                if (random.NextBool())
                    ipV4Layer.Fragmentation = IpV4Fragmentation.None;
            }

            switch (random.Next(0, 11))
            {
                case 0: // IpV4.
                case 1:
                    IpV4Layer innerIpV4Layer = random.NextIpV4Layer();
                    layers.Add(innerIpV4Layer);
                    CreateRandomIpPayload(random, innerIpV4Layer, layers);
                    return;

                case 2: // IpV6.
                case 3:
                    IpV6Layer innerIpV6Layer = random.NextIpV6Layer(random.NextBool(20));
                    layers.Add(innerIpV6Layer);
                    CreateRandomIpPayload(random, innerIpV6Layer, layers);
                    return;

                case 4: // Igmp.
                    layers.Add(random.NextIgmpLayer());
                    return;

                case 5: // Icmp.
                    IcmpLayer icmpLayer = random.NextIcmpLayer();
                    layers.Add(icmpLayer);
                    layers.AddRange(random.NextIcmpPayloadLayers(icmpLayer));
                    return;

                case 6: // Gre.
                    GreLayer greLayer = random.NextGreLayer();
                    layers.Add(greLayer);
                    CreateRandomEthernetPayload(random, greLayer, layers);
                    return;
                    
                case 7: // Udp.
                case 8:
                    UdpLayer udpLayer = random.NextUdpLayer();
                    layers.Add(udpLayer);
                    CreateRandomUdpPayload(random, udpLayer, layers);
                    return;

                case 9: // Tcp.
                case 10:
                    TcpLayer tcpLayer = random.NextTcpLayer();
                    layers.Add(tcpLayer);
                    CreateRandomTcpPayload(random, tcpLayer, layers);
                    return;

                default:
                    throw new InvalidOperationException("Invalid value.");
            }
        }

        private static void CreateRandomUdpPayload(Random random, UdpLayer udpLayer, List<ILayer> layers)
        {
            if (random.NextBool(20))
            {
                // Finish with payload.
                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(100));
                layers.Add(payloadLayer);
                return;
            }

            DnsLayer dnsLayer = random.NextDnsLayer();
            layers.Add(dnsLayer);

            ushort specialPort = (ushort)(random.NextBool() ? 53 : 5355);
            if (dnsLayer.IsQuery)
                udpLayer.DestinationPort = specialPort;
            else
                udpLayer.SourcePort = specialPort;
        }

        private static void CreateRandomTcpPayload(Random random, TcpLayer tcpLayer, List<ILayer> layers)
        {
            if (random.NextBool(20))
            {
                // Finish with payload.
                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(100));
                layers.Add(payloadLayer);
                return;
            }

            HttpLayer httpLayer = random.NextHttpLayer();
            layers.Add(httpLayer);
            if (httpLayer.IsRequest)
                tcpLayer.DestinationPort = 80;
            else
                tcpLayer.SourcePort = 80;

            if (random.NextBool())
                return;

            HttpLayer httpLayer2 = httpLayer.IsRequest ? (HttpLayer)random.NextHttpRequestLayer() : random.NextHttpResponseLayer();
            layers.Add(httpLayer2);
        }

        private static IEnumerable<Packet> CreateRandomPackets(Random random, int numPackets)
        {
            for (int i = 0; i != numPackets; ++i)
            {
                Packet packet = CreateRandomPacket(random);
                yield return packet;
            }
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
                PacketDumpFile.Dump(pcapFilename, new PcapDataLink(packets.First().DataLink.Kind), PacketDevice.DefaultSnapshotLength, packets);
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
                                            Arguments = "-o ip.check_checksum:TRUE " + 
                                                        "-o ipv6.use_geoip:FALSE " +
                                                        "-o udp.check_checksum:TRUE " +
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
                                            RedirectStandardError = true,
                                            CreateNoWindow = true
                                        };
                Console.WriteLine("Starting process " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);
                Assert.IsTrue(process.Start());
                string output = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(errorOutput);
                File.WriteAllText(documentFilename, output);
                Assert.AreEqual(0, process.ExitCode);
            }

            // Fix pdml file
            string fixedDocumentFilename = documentFilename + ".fixed";
            File.WriteAllBytes(fixedDocumentFilename, new List<byte>(from b in File.ReadAllBytes(documentFilename)
                                                                     select b == 0x0A || b == 0x0D
                                                                                ? b
                                                                                : Math.Max((byte)0x20, Math.Min(b, (byte)0x7F))).ToArray());

            try
            {
                Compare(XDocument.Load(fixedDocumentFilename, LoadOptions.None), packets);
            }
            catch (Exception exception)
            {
                throw new AssertFailedException("Failed comparing packets in file " + pcapFilename + ". " + exception, exception);
            }
        }

        private static void Compare(XDocument document, IEnumerable<Packet> packets)
        {
            IEnumerator<Packet> packetEnumerator = packets.GetEnumerator();

            List<Packet> failedPackets = new List<Packet>();
            StringBuilder failureMessage = new StringBuilder();

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
                    failedPackets.Add(packet);
                    failureMessage.Append(new AssertFailedException("Failed comparing packet " + i + ". " + e.Message, e) + Environment.NewLine);
                }
                ++i;
            }

            if (failedPackets.Any())
            {
                PacketDumpFile.Dump(Path.GetTempPath() + "temp." + 1000 + ".pcap", failedPackets.First().DataLink.Kind, 65536, failedPackets);
                throw new AssertFailedException("Failed comparing " + failedPackets.Count + " packets:" + Environment.NewLine + failureMessage);
            }
        }

        private static void ComparePacket(Packet packet, XElement documentPacket)
        {
            object currentDatagram = packet;
            CompareProtocols(currentDatagram, documentPacket, true);
        }

        internal static void CompareProtocols(object currentDatagram, XElement layersContainer, bool parentLayerSuccess)
        {
            Dictionary<string, int> layerNameToCount = new Dictionary<string, int>();
            foreach (var layer in layersContainer.Protocols())
            {
                string layerName = layer.Name();
                if (!layerNameToCount.ContainsKey(layerName))
                    layerNameToCount[layerName] = 1;
                else
                    ++layerNameToCount[layerName];
                switch (layerName)
                {
                    case "geninfo":
                    case "raw":
                        break;

                    case "frame":
                        CompareFrame(layer, (Packet)currentDatagram);
                        break;

                    default:
                        var comparer = WiresharkDatagramComparer.GetComparer(layer.Name(), layerNameToCount[layerName], layersContainer.Name(),
                                                                             parentLayerSuccess);
                        if (comparer == null)
                            return;
                        currentDatagram = comparer.Compare(layer, currentDatagram);
                        if (currentDatagram == null)
                            return;
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
                        double timeEpoch = double.Parse(field.Show(), CultureInfo.InvariantCulture);
                        // TODO: Remove this condition when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=11744 is fixed.
                        if (timeEpoch >= int.MinValue)
                        {
                            DateTime fieldTimestamp = new DateTime(1970, 1, 1).AddSeconds(timeEpoch);
                            MoreAssert.IsInRange(fieldTimestamp.AddMilliseconds(-1), fieldTimestamp.AddMilliseconds(1), packet.Timestamp.ToUniversalTime(),
                                                 "Timestamp");
                        }
                        break;

                    case "frame.cap_len":
                        field.AssertShowDecimal(packet.Length);
                        break;
                }
            }
        }
    }
}