using System;
using System.Collections.Generic;
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
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 10;

            using (PacketCommunicator deviceHandler = OpenLiveDevice())
            {
                deviceHandler.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           new Datagram(new byte[10], 0, 10));

                DateTime startSendingTime = DateTime.Now;

                for (int i = 0; i != NumPacketsToSend; ++i)
                    deviceHandler.SendPacket(sentPacket);

                DateTime endSendingTime = DateTime.Now;

                Packet packet;
                DeviceHandlerResult result = deviceHandler.GetPacket(out packet);

                Assert.AreEqual(DeviceHandlerResult.Ok, result);
                Assert.AreEqual<uint>(NumPacketsToSend, deviceHandler.TotalStatistics.PacketsCaptured);
                Assert.AreEqual(sentPacket.Length, packet.Length);
                MoreAssert.IsInRange(startSendingTime - TimeSpan.FromSeconds(1), endSendingTime + TimeSpan.FromSeconds(30), packet.Timestamp);
            }
        }

        private static PacketCommunicator OpenLiveDevice()
        {
            IList<LivePacketDevice> devices = LivePacketDevice.AllLocalMachine;
            MoreAssert.IsBiggerOrEqual(1, devices.Count);
            LivePacketDevice device = devices[0];
            Assert.AreEqual("Network adapter 'Atheros AR8121/AR8113 PCI-E Ethernet Controller (Microsoft's Packet Scheduler) ' on local host", device.Description);
            Assert.AreEqual(DeviceFlags.None, device.Flags);
            Assert.AreEqual(1, device.Addresses.Count);
            DeviceAddress address = device.Addresses[0];
            Assert.AreEqual("Address: INET 10.0.0.2 Netmask: INET 255.0.0.0 Broadcast: INET 255.255.255.255", address.ToString());
            PacketCommunicator deviceHandler = device.Open();
            try
            {
                Assert.AreEqual(DataLinkKind.Ethernet, deviceHandler.DataLink.Kind);
                Assert.AreEqual("EN10MB (Ethernet)", deviceHandler.DataLink.ToString());
                Assert.AreEqual(deviceHandler.DataLink, new PcapDataLink(deviceHandler.DataLink.Name));
                Assert.IsTrue(deviceHandler.IsFileSystemByteOrder);
                Assert.AreEqual(DeviceHandlerMode.Capture, deviceHandler.Mode);
                Assert.IsFalse(deviceHandler.NonBlocking);
                Assert.AreEqual(PacketDevice.DefaultSnapshotLength, deviceHandler.SnapshotLength);
                Assert.AreEqual(new PacketTotalStatistics(0, 0, 0, 0), deviceHandler.TotalStatistics);
                return deviceHandler;
            }
            catch (Exception)
            {
                deviceHandler.Dispose();
                throw;
            }
        }
    }
}