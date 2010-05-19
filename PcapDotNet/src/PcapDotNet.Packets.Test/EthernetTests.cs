using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for EthernetTests
    /// </summary>
    [TestClass]
    public class EthernetTests
    {
        public EthernetTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext{ get; set;}

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
        public void RandomEthernetTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                EthernetLayer ethernetLayer = random.NextEthernetLayer();
                int ethernetPayloadLength = random.Next(1500);
                PayloadLayer payloadLayer = new PayloadLayer
                                                {
                                                    Data = random.NextDatagram(ethernetPayloadLength),
                                                };
                Packet packet = new PacketBuilder(ethernetLayer, payloadLayer).Build(DateTime.Now);

                // Ethernet
                Assert.IsTrue(new[] {EthernetType.IpV4, EthernetType.Arp}.Contains(packet.Ethernet.EtherType) ||
                              packet.IsValid, "IsValid - EtherType = " + packet.Ethernet.EtherType);
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");
                Assert.AreEqual(ethernetLayer.GetHashCode(), packet.Ethernet.ExtractLayer().GetHashCode(), "Ethernet Layer Hash Code");
                Assert.AreNotEqual(random.NextEthernetLayer().GetHashCode(), packet.Ethernet.ExtractLayer().GetHashCode(), "Ethernet Layer Hash Code");
                Assert.AreEqual(ethernetLayer.ToString(), packet.Ethernet.ExtractLayer().ToString(), "Ethernet Layer ToString()");
                Assert.AreNotEqual(random.NextEthernetLayer().ToString(), packet.Ethernet.ExtractLayer().ToString(), "Ethernet Layer ToString()");
                Assert.AreNotEqual(2, packet.Ethernet.Source, "Ethernet Source");

                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.Payload);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticEthernetTypeNoNextLayer()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer());
            Assert.IsTrue(packet.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticEthernetTypeBadNextLayer()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new TcpLayer());
            Assert.IsTrue(packet.IsValid);
        }
    }
}