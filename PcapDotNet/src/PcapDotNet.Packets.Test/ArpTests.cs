using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for ArpTests
    /// </summary>
    [TestClass]
    public class ArpTests
    {
        public ArpTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void RandomArpTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                MacAddress ethernetSource = random.NextMacAddress();
                EthernetLayer ethernetLayer = new EthernetLayer
                                                  {
                                                      Source = ethernetSource,
                                                  };

                ArpLayer arpLayer = random.NextArpLayer();

                Packet packet = new PacketBuilder2(ethernetLayer, arpLayer).Build(DateTime.Now);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // Ethernet
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength, "PayloadLength");

                Assert.AreEqual(ethernetSource, packet.Ethernet.Source, "Ethernet Source");
                Assert.AreEqual(EthernetDatagram.BroadcastAddress, packet.Ethernet.Destination, "Ethernet Destination");
                Assert.AreEqual(EthernetType.Arp, packet.Ethernet.EtherType, "Ethernet EtherType");

                // Arp
                Assert.AreEqual(ArpDatagram.HeaderBaseLength + 2 * arpLayer.SenderHardwareAddress.Count + 2 * arpLayer.SenderProtocolAddress.Count, packet.Ethernet.Arp.Length, "Arp length");
                Assert.AreEqual(ArpHardwareType.Ethernet, packet.Ethernet.Arp.HardwareType, "Arp hardware type");
                Assert.AreEqual(arpLayer, packet.Ethernet.Arp.ExtractLayer(), "ARP Layer");
            }
        }

        [TestMethod]
        public void ArpProtocolIpV4Address()
        {
            Packet packet = PacketBuilder2.Build(DateTime.Now,
                                     new EthernetLayer
                                     {
                                         Source = new MacAddress(),
                                         EtherType = EthernetType.QInQ
                                     },
                                     new ArpLayer
                                     {
                                         Operation = ArpOperation.Request,
                                         SenderHardwareAddress = new byte[8],
                                         SenderProtocolAddress = new byte[]{1,2,3,4},
                                         TargetHardwareAddress = new byte[8],
                                         TargetProtocolAddress = new byte[]{11,22,33,44}
                                     });

            Assert.AreEqual(new IpV4Address("1.2.3.4"), packet.Ethernet.Arp.SenderProtocolIpV4Address);
            Assert.AreEqual(new IpV4Address("11.22.33.44"), packet.Ethernet.Arp.TargetProtocolIpV4Address);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArpIncosistentSenderAddressSizeTest()
        {
            Packet packet = PacketBuilder2.Build(DateTime.Now,
                                                 new EthernetLayer
                                                     {
                                                         Source = new MacAddress(),
                                                         EtherType = EthernetType.IpV4
                                                     },
                                                 new ArpLayer
                                                     {
                                                         Operation = ArpOperation.Request,
                                                         SenderHardwareAddress = new byte[4],
                                                         SenderProtocolAddress = new byte[6],
                                                         TargetHardwareAddress = new byte[5],
                                                         TargetProtocolAddress = new byte[6]
                                                     });
            Assert.IsNull(packet);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArpIncosistentTargetAddressSizeTest()
        {
            Packet packet = PacketBuilder2.Build(DateTime.Now,
                                                 new EthernetLayer
                                                 {
                                                     Source = new MacAddress(),
                                                     EtherType = EthernetType.IpV4
                                                 },
                                                 new ArpLayer
                                                 {
                                                     Operation = ArpOperation.Request,
                                                     SenderHardwareAddress = new byte[4],
                                                     SenderProtocolAddress = new byte[6],
                                                     TargetHardwareAddress = new byte[4],
                                                     TargetProtocolAddress = new byte[7]
                                                 });
            Assert.IsNull(packet);
            Assert.Fail();
        }
    }
}