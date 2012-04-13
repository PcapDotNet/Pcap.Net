using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.Packets.VLanTaggedFrame;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for VLanTaggedFrameTests.
    /// </summary>
    [TestClass]
    public class VLanTaggedFrameTests
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
        public void RandomVLanTaggedFrameTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                EthernetLayer ethernetLayer = random.NextEthernetLayer(EthernetType.None);
                VLanTaggedFrameLayer vLanTaggedFrameLayer = random.NextVLanTaggedFrameLayer();
                int payloadLength = random.Next(1500);
                PayloadLayer payloadLayer = new PayloadLayer
                {
                    Data = random.NextDatagram(payloadLength),
                };
                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, vLanTaggedFrameLayer, payloadLayer);

                ethernetLayer.EtherType = EthernetType.VLanTaggedFrame;

                // Test equality.
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer());
                Assert.AreEqual(vLanTaggedFrameLayer, packet.Ethernet.VLanTaggedFrame.ExtractLayer());
                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.VLanTaggedFrame.Payload);
            }
        }

        [TestMethod]
        public void AutoSetEtherTypeTest()
        {
            Random random = new Random();
            EthernetLayer ethernetLayer = random.NextEthernetLayer(EthernetType.None);
            VLanTaggedFrameLayer vLanTaggedFrameLayer = random.NextVLanTaggedFrameLayer(EthernetType.None);
            IpV4Layer ipV4Layer = random.NextIpV4Layer();
            Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, vLanTaggedFrameLayer, ipV4Layer);

            ethernetLayer.EtherType = EthernetType.VLanTaggedFrame;
            vLanTaggedFrameLayer.EtherType = EthernetType.IpV4;

            // Test equality.
            Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer());
            Assert.AreEqual(EthernetType.IpV4, packet.Ethernet.VLanTaggedFrame.EtherType);
            Assert.AreEqual(vLanTaggedFrameLayer, packet.Ethernet.VLanTaggedFrame.ExtractLayer());
            Assert.AreEqual(ipV4Layer, packet.Ethernet.VLanTaggedFrame.IpV4.ExtractLayer());
        }
    }
}