using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for UdpTests
    /// </summary>
    [TestClass]
    public class UdpTests
    {
        public UdpTests()
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
        public void RandomUdpTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            for (int i = 0; i != 1000; ++i)
            {
                UdpLayer udpLayer = random.NextUdpLayer();
                udpLayer.Checksum = null;

                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(60000));

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, payloadLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // Ethernet
                ethernetLayer.EtherType = EthernetType.IpV4;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IpV4
                ipV4Layer.Protocol = IpV4Protocol.Udp;
                ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer(), "IP Layer");
                ipV4Layer.HeaderChecksum = null;

                // UDP
                udpLayer.Checksum = packet.Ethernet.IpV4.Udp.Checksum;
                Assert.AreEqual(udpLayer, packet.Ethernet.IpV4.Udp.ExtractLayer(), "UDP Layer");
                Assert.AreEqual(UdpDatagram.HeaderLength + payloadLayer.Length, packet.Ethernet.IpV4.Udp.TotalLength, "Total Length");
                Assert.IsTrue(!udpLayer.CalculateChecksum && packet.Ethernet.IpV4.Udp.Checksum == 0 ||
                              udpLayer.CalculateChecksum && packet.Ethernet.IpV4.IsTransportChecksumCorrect, "IsTransportChecksumCorrect");
                Assert.IsTrue(packet.Ethernet.IpV4.Udp.IsChecksumOptional, "IsChecksumOptional");
                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.IpV4.Udp.Payload, "Payload");
            }
        }

        [TestMethod]
        public void UdpChecksumTest()
        {
            Packet packet = Packet.FromHexadecimalString(
                "3352c58e71ffc4f39ec3bae508004cfe0043361200008611eec22ea2c8d11e9eb7b9520c2a33f2bbbed998980bba4404f941019404eb51880496ce00000005a87a270013a683f572c10e1504a0df15448a",
                DateTime.Now, DataLinkKind.Ethernet);

            Assert.IsTrue(packet.Ethernet.IpV4.IsTransportChecksumCorrect);
        }

        [TestMethod]
        public void UdpZeroChecksumTest()
        {
            byte[] payload = new byte[2];
            payload.Write(0, (ushort)65498, Endianity.Big);
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                new UdpLayer
                                                    {
                                                        CalculateChecksumValue = true
                                                    },
                                                new PayloadLayer
                                                    {
                                                        Data = new Datagram(payload)
                                                    });
            Assert.IsTrue(packet.Ethernet.IpV4.IsTransportChecksumCorrect);
            Assert.AreEqual(0xFFFF, packet.Ethernet.IpV4.Udp.Checksum);
        }
    }
}