using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for DnsTests
    /// </summary>
    [TestClass]
    public class DnsTests
    {
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
        public void RandomDnsTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
            {
                Source = new MacAddress("00:01:02:03:04:05"),
                Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
            };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            UdpLayer udpLayer = random.NextUdpLayer();
            udpLayer.Checksum = null;

            for (int i = 0; i != 1000; ++i)
            {
                DnsLayer dnsLayer = random.NextDnsLayer();

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, dnsLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // DNS
                Assert.AreEqual(dnsLayer, packet.Ethernet.IpV4.Udp.Dns.ExtractLayer(), "DNS Layer");
            }
        }

        [TestMethod]
        public void SimpleDnsTest()
        {
            DnsLayer dnsLayer = new DnsLayer
                                {
                                    //Queries = new[] {new DnsQueryResourceRecord(new DnsDomainName("abc.def."), DnsType.A, DnsClass.In)}.ToList(),
                                    Answers =
                                        new[]
                                        {
                                    //        new DnsDataResourceRecord(new DnsDomainName("abc.def."), DnsType.A, DnsClass.In, 100,
                                      //                                new DnsResourceDataUnknown(new DataSegment(Encoding.ASCII.GetBytes("abcd")))),
                                        //    new DnsDataResourceRecord(new DnsDomainName("abc.def."), DnsType.A, DnsClass.In, 100,
                                          //                            new DnsResourceDataUnknown(new DataSegment(Encoding.ASCII.GetBytes("abce")))),
                                          new DnsDataResourceRecord(new DnsDomainName(""), DnsType.A, DnsClass.Any, 1, new DnsResourceDataUnknown(new DataSegment(new byte[0])))
                                        }.ToList(),
//                                    Authorities =
//                                        new[]
//                                        {
//                                            new DnsDataResourceRecord(new DnsDomainName("def"), DnsType.Ns, DnsClass.In, 2222,
//                                                                      new DnsResourceDataUnknown(new DataSegment(Encoding.ASCII.GetBytes("123"))))
//                                        }.ToList(),
//                                    Additionals =
//                                        new[]
//                                        {
//                                            new DnsDataResourceRecord(new DnsDomainName(""), DnsType.A, DnsClass.In, 2222,
//                                                                      new DnsResourceDataUnknown(new DataSegment(Encoding.ASCII.GetBytes("444")))),
//                                            new DnsDataResourceRecord(new DnsDomainName("123"), DnsType.A, DnsClass.In, 2222,
//                                                                      new DnsResourceDataUnknown(new DataSegment(Encoding.ASCII.GetBytes("444")))),
//                                        }.ToList(),
//                                    DomainNameCompressionMode = DnsDomainNameCompressionMode.All,
//                                    FutureUse = 6,
//                                    Id = 16365,
//                                    IsAuthoritiveAnswer = false,
//                                    IsQuery = false,
//                                    IsRecusionAvailable = true,
//                                    IsRecusionDesired = true,
//                                    Opcode = DnsOpcode.Query,
//                                    ResponseCode = DnsResponseCode.ServerFailure,
                                };

            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                dnsLayer);
                Assert.IsTrue(packet.IsValid, "IsValid");

                // DNS
                Assert.AreEqual(dnsLayer, packet.Ethernet.IpV4.Udp.Dns.ExtractLayer(), "DNS Layer");
        }
    }
}