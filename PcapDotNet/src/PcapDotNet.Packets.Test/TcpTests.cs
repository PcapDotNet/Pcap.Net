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
                TcpFlags tcpFlags = random.NextFlags<TcpFlags>();
                ushort tcpWindow = random.NextUShort();
                ushort tcpUrgentPointer = random.NextUShort();
                TcpOptions tcpOptions = random.NextTcpOptions();
                Datagram tcpPayload = random.NextDatagram(random.Next(60000));

                Packet packet = PacketBuilder.EthernetIpV4Tcp(DateTime.Now,
                                                              ethernetSource, ethernetDestination,
                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                              ipV4Source, ipV4Destination, ipV4Options,
                                                              tcpSourcePort, tcpDestinationPort, tcpSequenceNumber, tcpAcknowledgmentNumber, tcpFlags, tcpWindow, tcpUrgentPointer,
                                                              tcpOptions,
                                                              tcpPayload);

                Assert.IsTrue(packet.IsValid);

                // TCP
                Assert.AreEqual(tcpSourcePort, packet.Ethernet.IpV4.Tcp.SourcePort, "Source Port");
                Assert.AreEqual(tcpDestinationPort, packet.Ethernet.IpV4.Tcp.DestinationPort, "Destination Port");
                Assert.AreEqual(tcpSequenceNumber, packet.Ethernet.IpV4.Tcp.SequenceNumber, "Sequence Number");
                Assert.AreEqual(tcpAcknowledgmentNumber, packet.Ethernet.IpV4.Tcp.AcknowledgmentNumber, "Acknowledgment Number");
                Assert.AreEqual(tcpFlags, packet.Ethernet.IpV4.Tcp.Flags, "Flags");
                Assert.AreEqual(tcpWindow, packet.Ethernet.IpV4.Tcp.Window, "Window");
                Assert.AreEqual(tcpUrgentPointer, packet.Ethernet.IpV4.Tcp.UrgentPointer, "Urgent Pointer");
                Assert.AreEqual(tcpOptions, packet.Ethernet.IpV4.Tcp.Options, "Options");
                Assert.AreEqual((tcpFlags & TcpFlags.Ack) == TcpFlags.Ack, packet.Ethernet.IpV4.Tcp.IsAck, "IsAck");
                Assert.AreEqual((tcpFlags & TcpFlags.Cwr) == TcpFlags.Cwr, packet.Ethernet.IpV4.Tcp.IsCwr, "IsCwr");
                Assert.AreEqual((tcpFlags & TcpFlags.Ece) == TcpFlags.Ece, packet.Ethernet.IpV4.Tcp.IsEce, "IsEce");
                Assert.AreEqual((tcpFlags & TcpFlags.Fin) == TcpFlags.Fin, packet.Ethernet.IpV4.Tcp.IsFin, "IsFin");
                Assert.AreEqual((tcpFlags & TcpFlags.Psh) == TcpFlags.Psh, packet.Ethernet.IpV4.Tcp.IsPush, "IsPush");
                Assert.AreEqual((tcpFlags & TcpFlags.Rst) == TcpFlags.Rst, packet.Ethernet.IpV4.Tcp.IsReset, "IsReset");
                Assert.AreEqual((tcpFlags & TcpFlags.Syn) == TcpFlags.Syn, packet.Ethernet.IpV4.Tcp.IsSyn, "IsSyn");
                Assert.AreEqual((tcpFlags & TcpFlags.Urg) == TcpFlags.Urg, packet.Ethernet.IpV4.Tcp.IsUrg, "IsUrg");
                Assert.IsFalse(packet.Ethernet.IpV4.Tcp.IsChecksumOptional, "IsChecksumOptional");
                Assert.AreEqual(TcpDatagram.HeaderMinimumLength + tcpOptions.BytesLength + tcpPayload.Length, packet.Ethernet.IpV4.Tcp.Length, "Total Length");
                Assert.IsTrue(packet.Ethernet.IpV4.IsTransportChecksumCorrect, "IsTransportChecksumCorrect");
                Assert.AreEqual(tcpPayload, packet.Ethernet.IpV4.Tcp.Payload, "Payload");
            }
        }
    }
}