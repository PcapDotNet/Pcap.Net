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
using PcapDotNet.TestUtils;

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
                DnsLayer dnsLayer;
                do
                {
                    dnsLayer = random.NextDnsLayer();
                } while (dnsLayer.Length > 65000);

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, dnsLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // DNS
                DnsLayer actualLayer = (DnsLayer)packet.Ethernet.IpV4.Udp.Dns.ExtractLayer();
                Assert.AreEqual(dnsLayer, actualLayer, "DNS Layer");
            }
        }

        [TestMethod]
        public void DnsDomainNameCompressionTest()
        {
            DnsLayer dnsLayer = new DnsLayer();
            TestDomainNameCompression(0, dnsLayer);
            
            dnsLayer.Queries = new List<DnsQueryResourceRecord>();
            dnsLayer.Answers = new List<DnsDataResourceRecord>();
            dnsLayer.Authorities = new List<DnsDataResourceRecord>();
            dnsLayer.Additionals = new List<DnsDataResourceRecord>();
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Queries.Add(new DnsQueryResourceRecord(new DnsDomainName(""), DnsType.Any, DnsClass.In));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName(""), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(3, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("def.abc"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(6, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(6, dnsLayer);

            dnsLayer.Authorities.Add(new DnsDataResourceRecord(new DnsDomainName("abc.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(13, dnsLayer);

            dnsLayer.Authorities.Add(new DnsDataResourceRecord(new DnsDomainName("abd.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(16, dnsLayer);

            dnsLayer.Additionals.Add(new DnsDataResourceRecord(new DnsDomainName("hello.abd.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(23, dnsLayer);
        }

        [TestMethod]
        public void DnsDomainNameCompressionTooLongTest()
        {
            DnsLayer dnsLayer = new DnsLayer();
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Queries = new List<DnsQueryResourceRecord>();
            dnsLayer.Answers = new List<DnsDataResourceRecord>();
            dnsLayer.Authorities = new List<DnsDataResourceRecord>();
            dnsLayer.Additionals = new List<DnsDataResourceRecord>();
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("aaa"), DnsType.Null, DnsClass.In, 100, new DnsResourceDataAnything(new DataSegment(new byte[20000]))));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("bbb.aaa"), DnsType.Null, DnsClass.In, 100, new DnsResourceDataAnything(new DataSegment(new byte[1]))));
            TestDomainNameCompression(3, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("bbb.aaa"), DnsType.Null, DnsClass.In, 100, new DnsResourceDataAnything(new DataSegment(new byte[1]))));
            TestDomainNameCompression(6, dnsLayer);
        }

        [TestMethod]
        public void DnsOptResourceRecordTest()
        {
            Random random = new Random();
            for (int i = 0; i != 100; ++i)
            {
                DnsDomainName domainName = random.NextDnsDomainName();
                ushort sendersUdpPayloadSize = random.NextUShort();
                byte extendedRcode = random.NextByte();
                DnsOptVersion version = (DnsOptVersion)random.NextByte();
                DnsOptFlags flags = (DnsOptFlags)random.NextUShort();
                DnsResourceDataOptions data = (DnsResourceDataOptions)random.NextDnsResourceData(DnsType.Opt);

                DnsOptResourceRecord record = new DnsOptResourceRecord(domainName, sendersUdpPayloadSize, extendedRcode, version, flags, data);
                
                Assert.AreEqual(domainName, record.DomainName);
                Assert.AreEqual(sendersUdpPayloadSize, record.SendersUdpPayloadSize);
                Assert.AreEqual(extendedRcode, record.ExtendedRcode);
                Assert.AreEqual(version, record.Version);
                Assert.AreEqual(flags, record.Flags);
                Assert.AreEqual(data, record.Data);
            }
        }

        [TestMethod]
        public void DnsResourceDataNextDomainTest()
        {
            DataSegment bitMap = DnsResourceDataNextDomain.CreateTypeBitMap(new[] {DnsType.A, DnsType.Aaaa});
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitMap);
            Assert.IsFalse(resourceData.Equals(null));
            Assert.IsTrue(resourceData.IsTypePresentForOwner(DnsType.A));
            Assert.IsTrue(resourceData.IsTypePresentForOwner(DnsType.Aaaa));
            Assert.IsFalse(resourceData.IsTypePresentForOwner(DnsType.Ns));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainTooLongBitmapTest()
        {
            DataSegment bitMap = new DataSegment(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17});
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitMap);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainZeroEndedBitmapTest()
        {
            DataSegment bitMap = new DataSegment(new byte[] { 1, 0 });
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitMap);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        private static void TestDomainNameCompression(int expectedCompressionBenefit, DnsLayer dnsLayer)
        {
            dnsLayer.DomainNameCompressionMode = DnsDomainNameCompressionMode.Nothing;
            Packet uncompressedPacket = PacketBuilder.Build(DateTime.Now,
                                                            new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                            dnsLayer);
            Assert.IsTrue(uncompressedPacket.IsValid, "IsValid");
            ILayer uncompressedPacketLayer = uncompressedPacket.Ethernet.IpV4.Udp.Dns.ExtractLayer();

            dnsLayer.DomainNameCompressionMode = DnsDomainNameCompressionMode.All;
            Packet compressedPacket = PacketBuilder.Build(DateTime.Now,
                                                            new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                            dnsLayer);
            Assert.IsTrue(compressedPacket.IsValid, "IsValid");
            ILayer compressedPacketLayer = compressedPacket.Ethernet.IpV4.Udp.Dns.ExtractLayer();

            Assert.AreEqual(dnsLayer, uncompressedPacketLayer);
            Assert.AreEqual(dnsLayer, compressedPacketLayer);
            Assert.AreEqual(compressedPacketLayer, uncompressedPacketLayer);

            Assert.AreEqual(uncompressedPacket.Length, compressedPacket.Length + expectedCompressionBenefit, expectedCompressionBenefit.ToString());
        }
    }
}