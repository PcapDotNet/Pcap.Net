using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for LivePacketDeviceTests
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

                Packet packet;
                DateTime startWaiting = DateTime.Now;
                PacketCommunicatorReceiveResult result = communicator.GetPacket(out packet);
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                Assert.AreEqual<uint>(0, communicator.TotalStatistics.PacketsCaptured);
                MoreAssert.IsInRange(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.02), finishedWaiting - startWaiting);

                Packet sentPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 24);

                DateTime startSendingTime = DateTime.Now;

                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                DateTime endSendingTime = DateTime.Now;

                result = communicator.GetPacket(out packet);

                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                Assert.AreEqual<uint>(NumPacketsToSend, communicator.TotalStatistics.PacketsCaptured);
                Assert.AreEqual(sentPacket.Length, packet.Length);
                MoreAssert.IsInRange(startSendingTime - TimeSpan.FromSeconds(1), endSendingTime + TimeSpan.FromSeconds(30), packet.Timestamp);
            }
        }

        [TestMethod]
        public void GetSomePacketsTest()
        {
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Test normal mode
            TestGetSomePackets(0, 0, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, 0, 1, 1.02);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestGetSomePackets(NumPacketsToSend, 0, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestGetSomePackets(NumPacketsToSend, -1, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Test non blocking
            TestGetSomePackets(0, 0, int.MaxValue, PacketSize, true, PacketCommunicatorReceiveResult.Ok, 0, 0, 0.02);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Test break loop
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, 0, PacketSize, false, PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0.02);
        }

        [TestMethod]
        public void GetPacketsTest()
        {        
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Normal
            TestGetPackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Wait for less packets
            TestGetPackets(NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);

            // Wait for more packets
            TestGetPackets(NumPacketsToSend, 0, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.02);
            TestGetPackets(NumPacketsToSend, -1, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.02);
            TestGetPackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.02);

            // Break loop
            TestGetPackets(NumPacketsToSend, NumPacketsToSend, 0, 2, PacketSize, PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0.02);
            TestGetPackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, 2, PacketSize, PacketCommunicatorReceiveResult.BreakLoop, NumPacketsToSend / 2, 0, 0.02);
        }

        [TestMethod]
        public void GetNextStatisticsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, PacketSize);

                PacketSampleStatistics statistics;
                PacketCommunicatorReceiveResult result = communicator.GetNextStatistics(out statistics);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                MoreAssert.IsInRange(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1), statistics.Timestamp);
                Assert.AreEqual<uint>(0, statistics.AcceptedPackets);
                Assert.AreEqual<uint>(0, statistics.AcceptedBytes);

                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                result = communicator.GetNextStatistics(out statistics);

                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                MoreAssert.IsInRange(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1), statistics.Timestamp);
                Assert.AreEqual<uint>(NumPacketsToSend, statistics.AcceptedPackets, "AcceptedPackets");
                // Todo check byte statistics
//                Assert.AreEqual<uint>((uint)(sentPacket.Length * NumPacketsToSend), statistics.AcceptedBytes,
//                                      "AcceptedBytes. Diff Per Packet: " +
//                                      (statistics.AcceptedBytes - sentPacket.Length * NumPacketsToSend) /
//                                      ((double)NumPacketsToSend));
            }
        }

        [TestMethod]
        public void GetStatisticsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 100;
            const int NumStatisticsToGather = 3;
            const int PacketSize = 100;

            // Normal
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather, int.MaxValue, 5, PacketSize,
                              PacketCommunicatorReceiveResult.Ok, NumStatisticsToGather, NumPacketsToSend, NumStatisticsToGather, NumStatisticsToGather + 0.02);

            // Wait for less statistics
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather / 2, int.MaxValue, 5, PacketSize,
                              PacketCommunicatorReceiveResult.Ok, NumStatisticsToGather / 2, NumPacketsToSend, NumStatisticsToGather / 2, NumStatisticsToGather / 2 + 0.02);

            // Wait for more statistics
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, 0, int.MaxValue, 5.5, PacketSize,
                              PacketCommunicatorReceiveResult.None, 5, NumPacketsToSend, 5.5, 5.52);

            // Break loop
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather, 0, 5, PacketSize,
                              PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0, 0.02);
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather, NumStatisticsToGather / 2, 5, PacketSize,
                              PacketCommunicatorReceiveResult.BreakLoop, NumStatisticsToGather / 2, NumPacketsToSend, NumStatisticsToGather / 2, NumStatisticsToGather / 2 + 0.02);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetStatisticsOnCaptureModeErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                PacketSampleStatistics statistics;
                communicator.GetNextStatistics(out statistics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPacketOnStatisticsModeErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                Packet packet;
                communicator.GetPacket(out packet);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetInvalidModeErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = (PacketCommunicatorMode)(-99);
            }
        }

        private void TestGetStatistics(string sourceMac, string destinationMac, int numPacketsToSend, int numStatisticsToGather, int numStatisticsToBreakLoop, double secondsToWait, int packetSize,
                                       PacketCommunicatorReceiveResult expectedResult, int expectedNumStatistics, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;

                communicator.SetFilter("ether src " + sourceMac + " and ether dst " + destinationMac);

                Packet sentPacket = MoreRandom.BuildRandomPacket(sourceMac, destinationMac, packetSize);

                PacketCommunicatorReceiveResult result = PacketCommunicatorReceiveResult.None;
                int numStatisticsGot = 0;
                ulong totalPackets = 0;
                ulong totalBytes = 0;
                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                if (numStatisticsToBreakLoop == 0)
                    communicator.Break();
                Thread thread = new Thread(delegate()
                {
                    result = communicator.GetStatistics(numStatisticsToGather,
                                                     delegate(PacketSampleStatistics statistics)
                                                     {
                                                         totalPackets += statistics.AcceptedPackets;
                                                         totalBytes += statistics.AcceptedBytes;
                                                         ++numStatisticsGot;
                                                         if (numStatisticsGot >= numStatisticsToBreakLoop)
                                                             communicator.Break();
                                                     });
                });

                DateTime startWaiting = DateTime.Now;
                thread.Start();

                if (!thread.Join(TimeSpan.FromSeconds(secondsToWait)))
                    thread.Abort();
                DateTime finishedWaiting = DateTime.Now;
                Assert.AreEqual(expectedResult, result, "Result");
                Assert.AreEqual(expectedNumStatistics, numStatisticsGot, "NumStatistics");
                Assert.AreEqual((ulong)expectedNumPackets, totalPackets, "NumPackets");
                // Todo check bytes statistics
                //                Assert.AreEqual<ulong>((ulong)(NumPacketsToSend * sentPacket.Length), totalBytes, "NumBytes");
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
            }
        }

        private static void TestGetSomePackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop, int packetSize, bool nonBlocking,
                                               PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Packet packetToSend = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, packetSize);

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.NonBlocking = nonBlocking;
                Assert.AreEqual(nonBlocking, communicator.NonBlocking);
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                int numPacketsGot;
                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(packetToSend);

                int numPacketsHandled = 0;
                if (numPacketsToBreakLoop == 0)
                    communicator.Break();
                DateTime startWaiting = DateTime.Now;
                PacketCommunicatorReceiveResult result = communicator.GetSomePackets(out numPacketsGot, numPacketsToGet,
                                                                                     delegate(Packet packet)
                                                                                     {
                                                                                         Assert.AreEqual(packetToSend, packet);
                                                                                         ++numPacketsHandled;
                                                                                         if (numPacketsHandled >= numPacketsToBreakLoop)
                                                                                             communicator.Break();
                                                                                     });
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedNumPackets, numPacketsGot);
                Assert.AreEqual(expectedNumPackets, numPacketsHandled);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
            }
        }

        private void TestGetPackets(int numPacketsToSend, int numPacketsToWait, int numPacketsToBreakLoop, double secondsToWait, int packetSize,
                                    PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets,
                                    double expectedMinSeconds, double expectedMaxSeconds)
        {
            string TestDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToWait=" + numPacketsToWait +
                                     ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop + ". SecondsToWait=" +
                                     secondsToWait + ". PacketSize=" + packetSize;


            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, packetSize);

                PacketCommunicatorReceiveResult result = PacketCommunicatorReceiveResult.None;
                int numPacketsHandled = 0;

                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                Thread thread = new Thread(delegate()
                {
                    if (numPacketsToBreakLoop == 0)
                        communicator.Break();
                    result = communicator.GetPackets(numPacketsToWait,
                                                     delegate(Packet packet)
                                                     {
                                                         Assert.AreEqual(sentPacket, packet);
                                                         ++numPacketsHandled;
                                                         if (numPacketsHandled >= numPacketsToBreakLoop)
                                                             communicator.Break();
                                                     });
                });

                DateTime startWaiting = DateTime.Now;
                thread.Start();

                if (!thread.Join(TimeSpan.FromSeconds(secondsToWait)))
                    thread.Abort();
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(expectedResult, result, TestDescription);
                Assert.AreEqual(expectedNumPackets, numPacketsHandled);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
            }
        }

        public static PacketCommunicator OpenLiveDevice()
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