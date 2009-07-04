using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Packets;

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
        [ExpectedException(typeof(InvalidOperationException))]
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

            Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);
            Packet unexpectedPacket = MoreRandom.BuildRandomPacket(DestinationMac, SourceMac, 100);

            using (BerkeleyPacketFilter filter = new BerkeleyPacketFilter("ether src " + SourceMac + " and ether dst " + DestinationMac, 100, DataLinkKind.Ethernet))
            {
                using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
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
                        result = communicator.GetPacket(out packet);
                        Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                        Assert.AreEqual(expectedPacket, packet);
                    }

                    result = communicator.GetPacket(out packet);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                    Assert.IsNull(packet);
                }
            }
        }
    }
}