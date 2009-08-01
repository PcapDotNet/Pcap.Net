using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for BerkeleyPacketFilterTests
    /// </summary>
    [TestClass]
    public class BerkeleyPacketFilterTests
    {
        public BerkeleyPacketFilterTests()
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
        [ExpectedException(typeof(ArgumentException))]
        public void BadFilterErrorTest()
        {
            using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
            {
                communicator.SetFilter("illegal filter string");
            }
        }

        [TestMethod]
        public void NoCommunicatorConstructorTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Random random = new Random();
            Packet expectedPacket = random.NextEthernet(1000, SourceMac, DestinationMac);
            Packet unexpectedPacket = random.NextEthernet(1000, DestinationMac, SourceMac);

            using (BerkeleyPacketFilter filter = new BerkeleyPacketFilter("ether src " + SourceMac + " and ether dst " + DestinationMac, 1000, DataLinkKind.Ethernet))
            {
                using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
                {
                    TestFilter(communicator, filter, expectedPacket, unexpectedPacket);
                }
            }
        }

        [TestMethod]
        public void NoCommunicatorConstructorWithNetmaskTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Random random = new Random();
            Packet expectedPacket = random.NextEthernet(1000, SourceMac, DestinationMac);
            Packet unexpectedPacket = random.NextEthernet(1000, DestinationMac, SourceMac);

            using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
            {
                using (BerkeleyPacketFilter filter = new BerkeleyPacketFilter("ether src " + SourceMac + " and ether dst " + DestinationMac, 1000, DataLinkKind.Ethernet, communicator.IpV4Netmask))
                {
                    TestFilter(communicator, filter, expectedPacket, unexpectedPacket);
                }
            }
        }

        [TestMethod]
        public void TestTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int snapshotLength = 500;

            Random random = new Random();

            using (BerkeleyPacketFilter filter = new BerkeleyPacketFilter("ether src " + SourceMac + " and ether dst " + DestinationMac, snapshotLength, DataLinkKind.Ethernet))
            {
                Assert.IsTrue(filter.Test(random.NextEthernet(snapshotLength / 2, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernet(snapshotLength - 1, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernet(snapshotLength, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernet(snapshotLength + 1, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernet(snapshotLength * 2, SourceMac, DestinationMac)));

                Assert.IsFalse(filter.Test(random.NextEthernet(snapshotLength / 2, DestinationMac, SourceMac)));

                int actualSnapshotLength;
                Assert.IsTrue(filter.Test(out actualSnapshotLength, random.NextEthernet(snapshotLength / 2, SourceMac, DestinationMac)));
                Assert.AreEqual(snapshotLength, actualSnapshotLength);
            }
        }

        private static void TestFilter(PacketCommunicator communicator, BerkeleyPacketFilter filter, Packet expectedPacket, Packet unexpectedPacket)
        {
            communicator.SetFilter(filter);
            for (int i = 0; i != 5; ++i)
            {
                communicator.SendPacket(expectedPacket);
                communicator.SendPacket(unexpectedPacket);
            }

            Packet packet;
            PacketCommunicatorReceiveResult result;
            for (int i = 0; i != 5; ++i)
            {
                result = communicator.ReceivePacket(out packet);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                Assert.AreEqual(expectedPacket, packet);
            }

            result = communicator.ReceivePacket(out packet);
            Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
            Assert.IsNull(packet);
        }
    }
}