using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

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
        public void RandomEthernetTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                MacAddress ethernetSource = random.NextMacAddress();
                MacAddress ethernetDestination = random.NextMacAddress();
                EthernetType ethernetType = random.NextEnum(EthernetType.None);
                int ethernetPayloadLength = random.Next(1500);
                Datagram ethernetPayload = random.NextDatagram(ethernetPayloadLength);

                Packet packet = PacketBuilder.Ethernet(DateTime.Now,
                                                       ethernetSource, ethernetDestination, ethernetType,
                                                       ethernetPayload);


                Assert.IsTrue(packet.IsValid);

                // Ethernet
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetSource, packet.Ethernet.Source, "Ethernet Source");
                Assert.AreEqual(ethernetDestination, packet.Ethernet.Destination, "Ethernet Destination");
                Assert.AreEqual(ethernetType, packet.Ethernet.EtherType, "Ethernet Type");

                Assert.AreEqual(ethernetPayload, packet.Ethernet.Payload);
            }
        }
    }
}