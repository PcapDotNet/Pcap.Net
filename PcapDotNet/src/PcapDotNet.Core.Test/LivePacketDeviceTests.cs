using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PcapLibTests
    /// </summary>
    [TestClass]
    public class LivePacketDeviceTests
    {
        public LivePacketDeviceTests()
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

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           new Datagram(new byte[10], 0, 10));

                DateTime startSendingTime = DateTime.Now;

                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                DateTime endSendingTime = DateTime.Now;

                Packet packet;
                PacketCommunicatorReceiveResult result = communicator.GetPacket(out packet);

                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                Assert.AreEqual<uint>(NumPacketsToSend, communicator.TotalStatistics.PacketsCaptured);
                Assert.AreEqual(sentPacket.Length, packet.Length);
                MoreAssert.IsInRange(startSendingTime - TimeSpan.FromSeconds(1), endSendingTime + TimeSpan.FromSeconds(30), packet.Timestamp);
            }
        }

        [TestMethod]
        public void DumpPacketsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            string dumpFilename = Path.GetTempPath() + @"dump.pcap";

            Packet expectedPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                   new MacAddress(SourceMac),
                                                   new MacAddress(DestinationMac),
                                                   EthernetType.IpV4,
                                                   new Datagram(new byte[10], 0, 10));

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                using (PacketDumpFile dumpFile = communicator.OpenDump(dumpFilename))
                {
                    dumpFile.Dump(expectedPacket);
                }
            }

            using (PacketCommunicator communicator = new OfflinePacketDevice(dumpFilename).Open())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet actualPacket;
                communicator.GetPacket(out actualPacket);
                Assert.AreEqual(expectedPacket, actualPacket);
                MoreAssert.IsInRange(expectedPacket.Timestamp.AddSeconds(-1), expectedPacket.Timestamp.AddSeconds(1),
                                     actualPacket.Timestamp);
            }
        }

        [TestMethod]
        public void ReceiveManyPacketsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 100;

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           new Datagram(new byte[10], 0, 10));

                PacketCommunicatorReceiveResult result = PacketCommunicatorReceiveResult.None;
                int numPacketsGot = 0;
                Thread thread = new Thread(delegate()
                                               {
                                                   result = communicator.GetPackets(NumPacketsToSend,
                                                                                    delegate(Packet packet)
                                                                                        {
                                                                                            Assert.AreEqual(sentPacket, packet);
                                                                                            ++numPacketsGot;
                                                                                        });
                                               });
                thread.Start();
                
                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                if (!thread.Join(TimeSpan.FromSeconds(5)))
                    thread.Abort();

                Assert.AreEqual(NumPacketsToSend, numPacketsGot);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
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
            PacketCommunicator communicator = device.Open();
            try
            {
                Assert.AreEqual(DataLinkKind.Ethernet, communicator.DataLink.Kind);
                Assert.AreEqual("EN10MB (Ethernet)", communicator.DataLink.ToString());
                Assert.AreEqual(communicator.DataLink, new PcapDataLink(communicator.DataLink.Name));
                Assert.IsTrue(communicator.IsFileSystemByteOrder);
                Assert.AreEqual(PacketCommunicatorMode.Capture, communicator.Mode);
                Assert.IsFalse(communicator.NonBlocking);
                Assert.AreEqual(PacketDevice.DefaultSnapshotLength, communicator.SnapshotLength);
                Assert.AreEqual(new PacketTotalStatistics(0, 0, 0, 0), communicator.TotalStatistics);
                return communicator;
            }
            catch (Exception)
            {
                communicator.Dispose();
                throw;
            }
        }
    }
}