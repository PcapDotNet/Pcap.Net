using System;
using System.Threading;
using BPacket;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PcapLibTests
    /// </summary>
    [TestClass]
    public class PcapLiveDeviceTests
    {
        public PcapLiveDeviceTests()
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
        public void SendAndReceievePacketTest()
        {
            const string sourceMac = "11:22:33:44:55:66";
            const string dstMac = "77:88:99:AA:BB:CC";

            IPcapDevice device = PcapLiveDevice.AllLocalMachine[0];
            using (PcapDeviceHandler deviceHandler = device.Open())
            {
                Assert.AreEqual(DataLinkKind.Ethernet, deviceHandler.DataLink.Kind);

                deviceHandler.SetFilter("ether src " + sourceMac + " and ether dst " + dstMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(sourceMac),
                                                           new MacAddress(dstMac),
                                                           EthernetType.IpV4,
                                                           new Datagram(new byte[10], 0, 10));

                DateTime startSendingTime = DateTime.Now;

                for (int i = 0; i != 10; ++i)
                    deviceHandler.SendPacket(sentPacket);

                DateTime endSendingTime = DateTime.Now;

                Thread.Sleep(TimeSpan.FromSeconds(5));

                Packet packet;
                DeviceHandlerResult result = deviceHandler.GetPacket(out packet);

                Assert.AreEqual(DeviceHandlerResult.Ok, result);
                Assert.AreEqual(sentPacket.Length, packet.Length);
                MoreAssert.IsInRange(startSendingTime, endSendingTime + TimeSpan.FromSeconds(2), packet.Timestamp);
            }
        }
    }
}