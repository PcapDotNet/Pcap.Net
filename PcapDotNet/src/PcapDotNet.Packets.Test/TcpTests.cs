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

            Random random = new Random();

            byte ipV4TypeOfService = random.NextByte();
            ushort ipV4Identification = random.NextUShort();
            byte ipV4Ttl = random.NextByte();
            IpV4FragmentationOptions ipV4FragmentationOptions = random.NextEnum<IpV4FragmentationOptions>();
            ushort ipV4FragmentationOffset = (ushort)(random.NextUShort(ushort.MaxValue / 8) * 8);
            IpV4Fragmentation ipV4Fragmentation = new IpV4Fragmentation(ipV4FragmentationOptions, ipV4FragmentationOffset);
            IpV4Address ipV4Source = new IpV4Address(random.NextUInt());
            IpV4Address ipV4Destination = new IpV4Address(random.NextUInt());
            IpV4Options ipV4Options = random.NextIpV4Options();

            for (int i = 0; i != 1000; ++i)
            {
                ushort tcpSourcePort = random.NextUShort();
                ushort tcpDestinationPort = random.NextUShort();
                uint tcpSequenceNumber = random.NextUInt();
                uint tcpAcknowledgmentNumber = random.NextUInt();
                TcpControlBits tcpControlBits = random.NextFlags<TcpControlBits>();
                ushort tcpWindow = random.NextUShort();
                ushort tcpUrgentPointer = random.NextUShort();
                TcpOptions tcpOptions = random.NextTcpOptions();
                Datagram tcpPayload = random.NextDatagram(random.Next(60000));

                Packet packet = new PacketBuilder2(new EthernetLayer
                                                       {
                                                           Source = ethernetSource,
                                                           Destination = ethernetDestination
                                                       },
                                                   new IpV4Layer
                                                       {
                                                           TypeOfService = ipV4TypeOfService,
                                                           Identification = ipV4Identification,
                                                           Fragmentation = ipV4Fragmentation,
                                                           Ttl = ipV4Ttl,
                                                           Source = ipV4Source,
                                                           Destination = ipV4Destination,
                                                           Options = ipV4Options,
                                                       },
                                                   new TcpLayer
                                                       {
                                                           SourcePort = tcpSourcePort,
                                                           DestinationPort = tcpDestinationPort,
                                                           SequenceNumber = tcpSequenceNumber,
                                                           AcknowledgmentNumber = tcpAcknowledgmentNumber,
                                                           ControlBits = tcpControlBits,
                                                           Window = tcpWindow,
                                                           UrgentPointer = tcpUrgentPointer,
                                                           Options = tcpOptions,
                                                       },
                                                   new PayloadLayer
                                                       {
                                                           Data = tcpPayload
                                                       }
                    ).Build(DateTime.Now);
//                Packet packet = PacketBuilder.EthernetIpV4Tcp(DateTime.Now,
//                                                              ethernetSource, ethernetDestination,
//                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
//                                                              ipV4Source, ipV4Destination, ipV4Options,
//                                                              tcpSourcePort, tcpDestinationPort, tcpSequenceNumber, tcpAcknowledgmentNumber, tcpControlBits, tcpWindow, tcpUrgentPointer,
//                                                              tcpOptions,
//                                                              tcpPayload);

                Assert.IsTrue(packet.IsValid);

                // TCP
                Assert.AreEqual(tcpSourcePort, packet.Ethernet.IpV4.Tcp.SourcePort, "Source Port");
                Assert.AreEqual(tcpDestinationPort, packet.Ethernet.IpV4.Tcp.DestinationPort, "Destination Port");
                Assert.AreEqual(tcpSequenceNumber, packet.Ethernet.IpV4.Tcp.SequenceNumber, "Sequence Number");
                Assert.AreEqual(tcpAcknowledgmentNumber, packet.Ethernet.IpV4.Tcp.AcknowledgmentNumber, "Acknowledgment Number");
                Assert.AreEqual(tcpControlBits, packet.Ethernet.IpV4.Tcp.ControlBits, "Flags");
                Assert.AreEqual(tcpWindow, packet.Ethernet.IpV4.Tcp.Window, "Window");
                Assert.AreEqual(tcpUrgentPointer, packet.Ethernet.IpV4.Tcp.UrgentPointer, "Urgent Pointer");
                Assert.AreEqual(tcpOptions, packet.Ethernet.IpV4.Tcp.Options, "Options");
                foreach (TcpOption option in tcpOptions)
                {
                    Assert.AreEqual(option, option);
                    Assert.AreEqual(option.GetHashCode(), option.GetHashCode());
                    Assert.IsFalse(string.IsNullOrEmpty(option.ToString()));
                    Assert.IsFalse(option.Equals(null));
                    Assert.IsFalse(option.Equals(2));
                }
                Assert.AreEqual(tcpOptions, packet.Ethernet.IpV4.Tcp.Options, "Options");
                Assert.AreEqual((tcpControlBits & TcpControlBits.Acknowledgment) == TcpControlBits.Acknowledgment, packet.Ethernet.IpV4.Tcp.IsAcknowledgment, "IsAcknowledgment");
                Assert.AreEqual((tcpControlBits & TcpControlBits.CongestionWindowReduced) == TcpControlBits.CongestionWindowReduced, packet.Ethernet.IpV4.Tcp.IsCongestionWindowReduced, "IsCongestionWindowReduced");
                Assert.AreEqual((tcpControlBits & TcpControlBits.ExplicitCongestionNotificationEcho) == TcpControlBits.ExplicitCongestionNotificationEcho, packet.Ethernet.IpV4.Tcp.IsExplicitCongestionNotificationEcho, "IsExplicitCongestionNotificationEcho");
                Assert.AreEqual((tcpControlBits & TcpControlBits.Fin) == TcpControlBits.Fin, packet.Ethernet.IpV4.Tcp.IsFin, "IsFin");
                Assert.AreEqual((tcpControlBits & TcpControlBits.Push) == TcpControlBits.Push, packet.Ethernet.IpV4.Tcp.IsPush, "IsPush");
                Assert.AreEqual((tcpControlBits & TcpControlBits.Reset) == TcpControlBits.Reset, packet.Ethernet.IpV4.Tcp.IsReset, "IsReset");
                Assert.AreEqual((tcpControlBits & TcpControlBits.Synchronize) == TcpControlBits.Synchronize, packet.Ethernet.IpV4.Tcp.IsSynchronize, "IsSynchronize");
                Assert.AreEqual((tcpControlBits & TcpControlBits.Urgent) == TcpControlBits.Urgent, packet.Ethernet.IpV4.Tcp.IsUrgent, "IsUrgent");
                Assert.IsFalse(packet.Ethernet.IpV4.Tcp.IsChecksumOptional, "IsChecksumOptional");
                Assert.AreEqual(TcpDatagram.HeaderMinimumLength + tcpOptions.BytesLength + tcpPayload.Length, packet.Ethernet.IpV4.Tcp.Length, "Total Length");
                Assert.IsTrue(packet.Ethernet.IpV4.IsTransportChecksumCorrect, "IsTransportChecksumCorrect");
                Assert.AreEqual(tcpPayload, packet.Ethernet.IpV4.Tcp.Payload, "Payload");
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