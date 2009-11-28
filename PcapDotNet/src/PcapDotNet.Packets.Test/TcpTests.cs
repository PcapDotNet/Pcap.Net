using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for TcpTests
    /// </summary>
    [TestClass]
    public class TcpTests
    {
        public TcpTests()
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
        public void RandomTcpTest()
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");

            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = ethernetSource,
                                                  Destination = ethernetDestination
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);

            for (int i = 0; i != 1000; ++i)
            {
                TcpLayer tcpLayer = random.NextTcpLayer();

                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(60000));

                Packet packet = new PacketBuilder2(ethernetLayer,ipV4Layer, tcpLayer, payloadLayer).Build(DateTime.Now);

                Assert.IsTrue(packet.IsValid);

                // Ethernet
                ethernetLayer.EtherType = EthernetType.IpV4;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IpV4
                ipV4Layer.Protocol = IpV4Protocol.Tcp;
                ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer(), "IP Layer");
                ipV4Layer.HeaderChecksum = null;

                // TCP
                Assert.AreEqual(tcpLayer, packet.Ethernet.IpV4.Tcp.ExtractLayer(), "TCP Layer");
                foreach (TcpOption option in packet.Ethernet.IpV4.Tcp.Options)
                {
                    Assert.AreEqual(option, option);
                    Assert.AreEqual(option.GetHashCode(), option.GetHashCode());
                    Assert.IsFalse(string.IsNullOrEmpty(option.ToString()));
                    Assert.IsFalse(option.Equals(null));
                    Assert.IsFalse(option.Equals(2));
                }
                Assert.AreEqual(tcpLayer.Options, packet.Ethernet.IpV4.Tcp.Options, "Options");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.Acknowledgment) == TcpControlBits.Acknowledgment, packet.Ethernet.IpV4.Tcp.IsAcknowledgment, "IsAcknowledgment");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.CongestionWindowReduced) == TcpControlBits.CongestionWindowReduced, packet.Ethernet.IpV4.Tcp.IsCongestionWindowReduced, "IsCongestionWindowReduced");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.ExplicitCongestionNotificationEcho) == TcpControlBits.ExplicitCongestionNotificationEcho, packet.Ethernet.IpV4.Tcp.IsExplicitCongestionNotificationEcho, "IsExplicitCongestionNotificationEcho");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.Fin) == TcpControlBits.Fin, packet.Ethernet.IpV4.Tcp.IsFin, "IsFin");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.Push) == TcpControlBits.Push, packet.Ethernet.IpV4.Tcp.IsPush, "IsPush");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.Reset) == TcpControlBits.Reset, packet.Ethernet.IpV4.Tcp.IsReset, "IsReset");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.Synchronize) == TcpControlBits.Synchronize, packet.Ethernet.IpV4.Tcp.IsSynchronize, "IsSynchronize");
                Assert.AreEqual((tcpLayer.ControlBits & TcpControlBits.Urgent) == TcpControlBits.Urgent, packet.Ethernet.IpV4.Tcp.IsUrgent, "IsUrgent");
                Assert.IsFalse(packet.Ethernet.IpV4.Tcp.IsChecksumOptional, "IsChecksumOptional");
                Assert.AreEqual(TcpDatagram.HeaderMinimumLength + tcpLayer.Options.BytesLength + payloadLayer.Length, packet.Ethernet.IpV4.Tcp.Length, "Total Length");
                Assert.IsTrue(packet.Ethernet.IpV4.IsTransportChecksumCorrect, "IsTransportChecksumCorrect");

                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.IpV4.Tcp.Payload, "Payload");
            }
        }

        [TestMethod]
        public void TcpOptionSelectiveAcknowledgmentBlockTest()
        {
            TcpOptionSelectiveAcknowledgmentBlock block1 = new TcpOptionSelectiveAcknowledgmentBlock();
            Assert.AreEqual<uint>(0, block1.LeftEdge);
            Assert.AreEqual<uint>(0, block1.RightEdge);

            block1 = new TcpOptionSelectiveAcknowledgmentBlock(1, 2);
            Assert.AreEqual<uint>(1, block1.LeftEdge);
            Assert.AreEqual<uint>(2, block1.RightEdge);

            TcpOptionSelectiveAcknowledgmentBlock block2 = new TcpOptionSelectiveAcknowledgmentBlock();
            Assert.AreNotEqual(block1, block2);
            Assert.IsTrue(block1 != block2);
            Assert.IsFalse(block1 == block2);

            block2 = new TcpOptionSelectiveAcknowledgmentBlock(1, 2);
            Assert.AreEqual(block1, block2);
            Assert.IsFalse(block1 != block2);
            Assert.IsTrue(block1 == block2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TcpOptionMd5SignatureConstructorErrorDataLengthTest()
        {
            new TcpOptionMd5Signature(new byte[10]);
            Assert.Fail();
        }

        [TestMethod]
        public void TcpOptionMd5SignatureCreateInstanceErrorDataLengthTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4Tcp(DateTime.Now,
                                                          new MacAddress(), new MacAddress(),
                                                          0, 0, new IpV4Fragmentation(), 0, new IpV4Address(), new IpV4Address(), new IpV4Options(),
                                                          0, 0, 0, 0, new TcpControlBits(), 0, 0,
                                                          new TcpOptions(new TcpOptionMd5Signature(new byte[]{1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16})), Datagram.Empty);

            Assert.IsTrue(packet.IsValid);
            Assert.IsTrue(packet.Ethernet.IpV4.Tcp.Options.IsValid);

            byte[] buffer = packet.Buffer;
            buffer[buffer.Length - packet.Ethernet.IpV4.Tcp.Length + TcpDatagram.HeaderMinimumLength + 1] = 2;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);

            Assert.IsFalse(packet.Ethernet.IpV4.Tcp.Options.IsValid);
        }
    }
}