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
            const EthernetType ethernetType = EthernetType.IpV4;

            Random random = new Random();

            byte ipV4TypeOfService = random.NextByte();
            ushort ipV4Identification = random.NextUShort();
            byte ipV4Ttl = random.NextByte();
            IpV4FragmentationOptions ipV4FragmentationOptions = random.NextEnum<IpV4FragmentationOptions>();
            ushort ipV4FragmentationOffset = (ushort)(random.NextUShort(ushort.MaxValue / 8) * 8);
            IpV4Fragmentation ipV4Fragmentation = new IpV4Fragmentation(ipV4FragmentationOptions, ipV4FragmentationOffset);
            IpV4Protocol ipV4Protocol = random.NextEnum<IpV4Protocol>();
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
                TcpOptions tcpOptions = new TcpOptions();
                Datagram tcpPayload = random.NextDatagram(random.Next(60000));

                Packet packet = PacketBuilder.EthernetIpV4Tcp(DateTime.Now,
                                                              ethernetSource, ethernetDestination,
                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                              ipV4Source, ipV4Destination, ipV4Options,
                                                              tcpSourcePort, tcpDestinationPort, tcpSequenceNumber, tcpAcknowledgmentNumber, tcpFlags, tcpWindow, tcpUrgentPointer,
                                                              tcpOptions,
                                                              tcpPayload);

                Assert.IsTrue(packet.IsValid);

                // UDP
                Assert.AreEqual(tcpSourcePort, packet.Ethernet.IpV4.Tcp.SourcePort, "Source Port");
                Assert.AreEqual(tcpDestinationPort, packet.Ethernet.IpV4.Tcp.DestinationPort, "Destination Port");
                Assert.AreEqual(tcpSequenceNumber, packet.Ethernet.IpV4.Tcp.SequenceNumber, "Sequence Number");
                Assert.AreEqual(tcpAcknowledgmentNumber, packet.Ethernet.IpV4.Tcp.AcknowledgmentNumber, "Acknowledgment Number");
                Assert.AreEqual(tcpFlags, packet.Ethernet.IpV4.Tcp.Flags, "Flags");
                Assert.AreEqual(tcpWindow, packet.Ethernet.IpV4.Tcp.Window, "Window");
                Assert.AreEqual(tcpUrgentPointer, packet.Ethernet.IpV4.Tcp.UrgentPointer, "Urgent Pointer");
                Assert.AreEqual(TcpDatagram.HeaderMinimumLength + tcpOptions.BytesLength + tcpPayload.Length, packet.Ethernet.IpV4.Tcp.Length, "Total Length");
                Assert.IsTrue(packet.Ethernet.IpV4.IsTransportChecksumCorrect, "IsTransportChecksumCorrect");
                Assert.AreEqual(tcpPayload, packet.Ethernet.IpV4.Tcp.Payload, "Payload");
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
    }
}