using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV6Tests
    /// </summary>
    [TestClass]
    public class IpV6Tests
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
        public void RandomIpV6Test()
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");
            const EthernetType EthernetType = EthernetType.IpV6;

            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = ethernetSource,
                                                  Destination = ethernetDestination,
                                                  EtherType = EthernetType
                                              };

            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                IpV6Layer ipV6Layer = random.NextIpV6Layer();

                PayloadLayer payloadLayer = random.NextPayloadLayer(random.NextInt(0, 50 * 1024));

                List<ILayer> layers = new List<ILayer> {ethernetLayer, ipV6Layer};
                if (ipV6Layer.ExtensionHeaders.LastHeader != IpV4Protocol.EncapsulatingSecurityPayload)
                    layers.Add(payloadLayer);
                Packet packet = PacketBuilder.Build(DateTime.Now, layers);

                Assert.IsTrue(packet.IsValid, string.Format("IsValid ({0}...{1})", ipV6Layer.NextHeader, ipV6Layer.ExtensionHeaders.NextHeader));

                // Ethernet
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IpV6
                Assert.AreEqual(ipV6Layer, packet.Ethernet.IpV6.ExtractLayer(), "IP Layer");
                /*
                if (packet.Ethernet.IpV6.NextHeader == IpV4Protocol.Tcp)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV6.Transport, typeof(TcpDatagram));
                else if (packet.Ethernet.IpV6.NextHeader == IpV4Protocol.Udp)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV6.Transport, typeof(UdpDatagram));
                else
                    Assert.IsNull(packet.Ethernet.IpV6.Transport);
                */
//                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.IpV6.Payload, "IP Payload");
            }
        }

        [TestMethod]
        public void AutomaticIpV6NextHeader()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV6Layer(), new UdpLayer());
            Assert.AreEqual(IpV4Protocol.Udp, packet.Ethernet.IpV6.NextHeader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticIpV6NextHeaderNoNextLayer()
        {
            PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV6Layer());
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticIpV6NextHeaderUnknownNextLayer()
        {
            PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV6Layer(), new PayloadLayer());
            Assert.Fail();
        }
    }
}