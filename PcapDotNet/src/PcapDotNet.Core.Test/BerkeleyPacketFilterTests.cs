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
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set;}

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
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
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
            Packet expectedPacket = random.NextEthernetPacket(1000, SourceMac, DestinationMac);
            Packet unexpectedPacket = random.NextEthernetPacket(1000, DestinationMac, SourceMac);

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
            Packet expectedPacket = random.NextEthernetPacket(1000, SourceMac, DestinationMac);
            Packet unexpectedPacket = random.NextEthernetPacket(1000, DestinationMac, SourceMac);

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
            const int SnapshotLength = 500;

            Random random = new Random();

            using (BerkeleyPacketFilter filter = new BerkeleyPacketFilter("ether src " + SourceMac + " and ether dst " + DestinationMac, SnapshotLength, DataLinkKind.Ethernet))
            {
                Assert.IsTrue(filter.Test(random.NextEthernetPacket(SnapshotLength / 2, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernetPacket(SnapshotLength - 1, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernetPacket(SnapshotLength, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernetPacket(SnapshotLength + 1, SourceMac, DestinationMac)));
                Assert.IsTrue(filter.Test(random.NextEthernetPacket(SnapshotLength * 2, SourceMac, DestinationMac)));

                Assert.IsFalse(filter.Test(random.NextEthernetPacket(SnapshotLength / 2, DestinationMac, SourceMac)));

                int actualSnapshotLength;
                Assert.IsTrue(filter.Test(out actualSnapshotLength, random.NextEthernetPacket(SnapshotLength / 2, SourceMac, DestinationMac)));
                Assert.AreEqual(SnapshotLength, actualSnapshotLength);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void TestNullTest()
        {
            using (BerkeleyPacketFilter filter = new BerkeleyPacketFilter("ether src 11:22:33:44:55:66", PacketDevice.DefaultSnapshotLength, DataLinkKind.Ethernet))
            {
                filter.Test(null);
            }
            Assert.Fail();
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