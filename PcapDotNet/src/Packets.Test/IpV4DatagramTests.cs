using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Packets.Test
{
    /// <summary>
    /// Summary description for IpV4DatagramTests
    /// </summary>
    [TestClass]
    public class IpV4DatagramTests
    {
        public IpV4DatagramTests()
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
        public void IpV4DatagramTest()
        {
//            PacketBuilder.IpV4(DateT)
        }
    }

    /// <summary>
    /// Summary description for PacketsTests
    /// </summary>
    [TestClass]
    public class PacketsTests
    {
        public PacketsTests()
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
        public void HttpTest()
        {
            Packet packet = HexToPacket(
                "0123456789abba98765432100800" + // Ethernet
                "4500042f" + // IPv4 Version, IHL, Type of Service, Total Length
                "787a4000" + // IPv4 Identification, Flags, Fragment Offset
                "80061234" + // IPv4 Time to Live, Protocol, Header Checksum
                "c0a80160" + // IPv4 Source Address
                "12345678" + // IPv4 Destination Address
                "06d900504a5d60e5f48579935018440c70c70000474554202f20485454502f312e310d0a" +
                "486f73743a2070636170646f746e65742e636f6465706c65782e636f6d0d0a" +
                "557365722d4167656e743a204d6f7a696c6c612f352e30202857696e646f77733b20553b2057696e646f7773204e5420352e313b20656e2d55533b2072763a312e392e302e313229204765636b6f2f323030393037303631312046697265666f782f332e302e313220282e4e455420434c5220332e352e3330373239290d0a" +
                "4163636570743a20746578742f68746d6c2c6170706c69636174696f6e2f7868746d6c2b786d6c2c6170706c69636174696f6e2f786d6c3b713d302e392c2a2f2a3b713d302e380d0a" + 
                "4163636570742d4c616e67756167653a20656e2d75732c656e3b713d302e350d0a" +
                "4163636570742d456e636f64696e673a20677a69702c6465666c6174650d0a" +
                "4163636570742d436861727365743a2049534f2d383835392d312c7574662d383b713d302e372c2a3b713d302e370d0a" +
                "4b6565702d416c6976653a203330300d0a" +
                "436f6e6e656374696f6e3a206b6565702d616c6976650d0a" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d650d0a" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
                "123456780d0a" +
                "0d0a",
                DataLinkKind.Ethernet);

            Assert.IsTrue(packet.IsValid);

            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength);
            Assert.AreEqual(new MacAddress("ba:98:76:54:32:10"), packet.Ethernet.Source);
            Assert.AreEqual(new MacAddress("01:23:45:67:89:ab"), packet.Ethernet.Destination);
            Assert.AreEqual(EthernetType.IpV4, packet.Ethernet.EtherType);

            Assert.AreEqual(IpV4Datagram.HeaderMinimumLength, packet.Ethernet.IpV4.HeaderLength);
            Assert.AreEqual(0, packet.Ethernet.IpV4.TypeOfService);
            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.IpV4.TotalLength);
            Assert.AreEqual(new IpV4Fragmentation(IpV4FragmentationFlags.DontFragment, 0), packet.Ethernet.IpV4.Fragmentation);
            Assert.AreEqual(128, packet.Ethernet.IpV4.Ttl);
            Assert.AreEqual(IpV4Protocol.Tcp, packet.Ethernet.IpV4.Protocol);
            Assert.AreEqual(0x1234, packet.Ethernet.IpV4.HeaderChecksum);
//            Assert.AreEqual(true, packet.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.AreEqual(new IpV4Address("192.168.1.96"), packet.Ethernet.IpV4.Source);
            Assert.AreEqual(new IpV4Address("18.52.86.120"), packet.Ethernet.IpV4.Destination);
//            Assert.AreEqual(IpV4Options.None, packet.Ethernet.IpV4.Options);
        }

        private static Packet HexToPacket(string hexString, DataLinkKind dataLinkKind)
        {
            return HexToPacket(hexString, DateTime.MinValue, dataLinkKind);
        }

        private static Packet HexToPacket(string hexString, DateTime timestamp, DataLinkKind dataLinkKind)
        {
            byte[] bytes = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return new Packet(bytes, timestamp, dataLinkKind);
        }
    }
}