using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Packets.Test
{
    /// <summary>
    /// Summary description for EthernetDatagramTests
    /// </summary>
    [TestClass]
    public class EthernetDatagramTests
    {
        public EthernetDatagramTests()
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
        public void EthernetDatagramTest()
        {
            MacAddress ethernetSource = new MacAddress("1:2:3:4:5:6");
            MacAddress ethernetDestination = new MacAddress("7:8:9:10:11:12");
            const EthernetType ethernetType = EthernetType.IpV4;
            Datagram ethernetPayload = new Datagram(new byte[] {1, 2, 3, 4, 5});
            Packet goodPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                       ethernetSource, ethernetDestination,
                                                       ethernetType,
                                                       ethernetPayload);
            Packet badPacket = new Packet(new byte[]{1,2,3,4,5}, DateTime.Now, new DataLink(DataLinkKind.Ethernet));

            Assert.IsTrue(goodPacket.Ethernet.IsValid);
            Assert.AreEqual(ethernetSource, goodPacket.Ethernet.Source);
            Assert.AreEqual(ethernetDestination, goodPacket.Ethernet.Destination);
            Assert.AreEqual(ethernetType, goodPacket.Ethernet.EtherType);

            Assert.IsFalse(badPacket.Ethernet.IsValid);
        }
    }
}