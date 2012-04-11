using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for TcpTests
    /// </summary>
    [TestClass]
    public class TcpTests
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
            ipV4Layer.HeaderChecksum = null;

            for (int i = 0; i != 1000; ++i)
            {
                TcpLayer tcpLayer = random.NextTcpLayer();

                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(60000));

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, tcpLayer, payloadLayer);

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
                tcpLayer.Checksum = packet.Ethernet.IpV4.Tcp.Checksum;
                Assert.AreEqual(tcpLayer, packet.Ethernet.IpV4.Tcp.ExtractLayer(), "TCP Layer");
                Assert.AreNotEqual(random.NextTcpLayer(), packet.Ethernet.IpV4.Tcp.ExtractLayer(), "TCP Layer");
                Assert.AreEqual(tcpLayer.GetHashCode(), packet.Ethernet.IpV4.Tcp.ExtractLayer().GetHashCode(), "TCP Layer");
                Assert.AreNotEqual(random.NextTcpLayer().GetHashCode(), packet.Ethernet.IpV4.Tcp.ExtractLayer().GetHashCode(), "TCP Layer");
                Assert.AreEqual(packet.Ethernet.IpV4.Tcp.SequenceNumber + packet.Ethernet.IpV4.Tcp.PayloadLength, packet.Ethernet.IpV4.Tcp.NextSequenceNumber);
                foreach (TcpOption option in packet.Ethernet.IpV4.Tcp.Options.OptionsCollection)
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
                Assert.AreEqual(0, packet.Ethernet.IpV4.Tcp.Reserved);
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
            Assert.AreNotEqual(block1.ToString(), block2.ToString());
            Assert.AreNotEqual(block1, 0);

            block2 = new TcpOptionSelectiveAcknowledgmentBlock(1, 2);
            Assert.AreEqual(block1, block2);
            Assert.IsFalse(block1 != block2);
            Assert.IsTrue(block1 == block2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void TcpOptionMd5SignatureConstructorErrorDataLengthTest()
        {
            new TcpOptionMd5Signature(new byte[10]);
            Assert.Fail();
        }

        [TestMethod]
        public void TcpOptionMd5SignatureCreateInstanceErrorDataLengthTest()
        {
            Packet packet =
                PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                     new TcpLayer
                                         {
                                             Options =
                                                 new TcpOptions(
                                                 new TcpOptionMd5Signature(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}))
                                         });

            Assert.IsTrue(packet.IsValid);
            Assert.IsTrue(packet.Ethernet.IpV4.Tcp.Options.IsValid);

            byte[] buffer = packet.Buffer;
            buffer[buffer.Length - packet.Ethernet.IpV4.Tcp.Length + TcpDatagram.HeaderMinimumLength + 1] = 2;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);

            Assert.IsFalse(packet.Ethernet.IpV4.Tcp.Options.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void TcpOptionMd5SignatureConstructorNullTest()
        {
            Assert.IsNotNull(new TcpOptionMd5Signature(null));
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void TcpOptionMoodBadEmotionStringTest()
        {
            Assert.IsNotNull(new TcpOptionMood((TcpOptionMoodEmotion)202).EmotionString);
        }

        [TestMethod]
        public void TcpChecksumTest()
        {
            Packet packet = Packet.FromHexadecimalString(
                "72ad58bae3b13638b5e35a3f08004a6c0055fd5400000e0622f341975faa3bfb25ed83130cb2e02103adfc7efbac1c2bb0f402e64800bb641bc8de8fa185e8ff716b60faf864bfe85901040205021ceec26d916419de400347f33fcca9ad44e9ffae8f",
                DateTime.Now, DataLinkKind.Ethernet);

            Assert.IsFalse(packet.Ethernet.IpV4.IsTransportChecksumCorrect);
        }
    }
}