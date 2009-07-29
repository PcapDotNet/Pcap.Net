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
                "feff200001000000010000000800" +
                "45000207" + // IPv4 Version, IHL, Type of Service, Total Length
                "0f454000" + // IPv4 Identification, Flags, Fragment Offset
                "80069010" + // IPv4 Time to Live, Protocol, Header Checksum
                "91fea0ed" + // IPv4 Source Address
                "41d0e4df" + // IPv4 Destination Address
                "0d2c005038affe14114c618c501825bca9580000474554202f646f776e6c6f61642e68746d6c20485454502f312e310d0a486f73743a207777772e657468657265616c2e636f6d0d0a557365722d4167656e743a204d6f7a696c6c612f352e30202857696e646f77733b20553b2057696e646f7773204e5420352e313b20656e2d55533b2072763a312e3629204765636b6f2f32303034303131330d0a4163636570743a20746578742f786d6c2c6170706c69636174696f6e2f786d6c2c6170706c69636174696f6e2f7868746d6c2b786d6c2c746578742f68746d6c3b713d302e392c746578742f706c61696e3b713d302e382c696d6167652f706e672c696d6167652f6a7065672c696d6167652f6769663b713d302e322c2a2f2a3b713d302e310d0a4163636570742d4c616e67756167653a20656e2d75732c656e3b713d302e350d0a4163636570742d456e636f64696e673a20677a69702c6465666c6174650d0a4163636570742d436861727365743a2049534f2d383835392d312c7574662d383b713d302e372c2a3b713d302e370d0a4b6565702d416c6976653a203330300d0a436f6e6e656374696f6e3a206b6565702d616c6976650d0a526566657265723a20687474703a2f2f7777772e657468657265616c2e636f6d2f646576656c6f706d656e742e68746d6c0d0a0d0a",
                DataLinkKind.Ethernet);
//            Packet packet = HexToPacket(
//
//                "0123456789abba98765432100800" + // Ethernet
//                "4500042f" + // IPv4 Version, IHL, Type of Service, Total Length
//                "787a4000" + // IPv4 Identification, Flags, Fragment Offset
//                "80061234" + // IPv4 Time to Live, Protocol, Header Checksum
//                "c0a80160" + // IPv4 Source Address
//                "12345678" + // IPv4 Destination Address
//                "06d900504a5d60e5f48579935018440c70c70000474554202f20485454502f312e310d0a" +
//                "486f73743a2070636170646f746e65742e636f6465706c65782e636f6d0d0a" +
//                "557365722d4167656e743a204d6f7a696c6c612f352e30202857696e646f77733b20553b2057696e646f7773204e5420352e313b20656e2d55533b2072763a312e392e302e313229204765636b6f2f323030393037303631312046697265666f782f332e302e313220282e4e455420434c5220332e352e3330373239290d0a" +
//                "4163636570743a20746578742f68746d6c2c6170706c69636174696f6e2f7868746d6c2b786d6c2c6170706c69636174696f6e2f786d6c3b713d302e392c2a2f2a3b713d302e380d0a" + 
//                "4163636570742d4c616e67756167653a20656e2d75732c656e3b713d302e350d0a" +
//                "4163636570742d456e636f64696e673a20677a69702c6465666c6174650d0a" +
//                "4163636570742d436861727365743a2049534f2d383835392d312c7574662d383b713d302e372c2a3b713d302e370d0a" +
//                "4b6565702d416c6976653a203330300d0a" +
//                "436f6e6e656374696f6e3a206b6565702d616c6976650d0a" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d650d0a" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "526566657265723a20687474703a2f2f70636170646f746e65742e636f6465706c65782e636f6d2f77696b692f636f6d6d656e74732f766965773f7469746c653d486f6d65" +
//                "123456780d0a" +
//                "0d0a",
//                DataLinkKind.Ethernet);

            Assert.IsTrue(packet.IsValid);

            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength, "PayloadLength");
            Assert.AreEqual(new MacAddress("00:00:01:00:00:00"), packet.Ethernet.Source, "Ethernet Source");
            Assert.AreEqual(new MacAddress("fe:ff:20:00:01:00"), packet.Ethernet.Destination, "Ethernet Destination");
            Assert.AreEqual(EthernetType.IpV4, packet.Ethernet.EtherType, "Ethernet Type");

            Assert.AreEqual(IpV4Datagram.HeaderMinimumLength, packet.Ethernet.IpV4.HeaderLength, "IP HeaderLength");
            Assert.AreEqual(0, packet.Ethernet.IpV4.TypeOfService, "IP TypeOfService");
            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.IpV4.TotalLength, "IP TotalLength");
            Assert.AreEqual(new IpV4Fragmentation(IpV4FragmentationFlags.DontFragment, 0), packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
            Assert.AreEqual(128, packet.Ethernet.IpV4.Ttl, "IP Ttl");
            Assert.AreEqual(IpV4Protocol.Tcp, packet.Ethernet.IpV4.Protocol, "IP Protocol");
            Assert.AreEqual(0x9010, packet.Ethernet.IpV4.HeaderChecksum, "IP HeaderChecksum");
            Assert.AreEqual(true, packet.Ethernet.IpV4.IsHeaderChecksumCorrect, "IP HeaderChecksumCorrect");
            Assert.AreEqual(new IpV4Address("145.254.160.237"), packet.Ethernet.IpV4.Source, "IP Source");
            Assert.AreEqual(new IpV4Address("65.208.228.223"), packet.Ethernet.IpV4.Destination, "IP Destination");
            Assert.AreEqual(IpV4Options.None, packet.Ethernet.IpV4.Options, "IP Options");
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