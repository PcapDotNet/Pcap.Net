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

                Packet packet;
                DateTime startWaiting = DateTime.Now;
                PacketCommunicatorReceiveResult result = communicator.GetPacket(out packet);
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                Assert.AreEqual<uint>(0, communicator.TotalStatistics.PacketsCaptured);
                MoreAssert.IsInRange(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.02), finishedWaiting - startWaiting);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           GetRandomDatagram(10));

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
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Test normal mode
            TestGetSomePackets(SourceMac, DestinationMac, 0, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, 0, 1, 1.02);
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Test non blocking
            TestGetSomePackets(SourceMac, DestinationMac, 0, int.MaxValue, PacketSize, true, PacketCommunicatorReceiveResult.Ok, 0, 0, 0.02);
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, int.MaxValue, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Test break loop
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, NumPacketsToSend / 2, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, NumPacketsToSend / 2, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, 0, PacketSize, false, PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0.02);
        }

        [TestMethod]
        public void GetPacketsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Normal
            TestGetPackets(SourceMac, DestinationMac, NumPacketsToSend, NumPacketsToSend, int.MaxValue, 2, PacketSize,
                           PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Wait for less packets
            TestGetPackets(SourceMac, DestinationMac, NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, 2, PacketSize,
                           PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);

            // Wait for more packets
            TestGetPackets(SourceMac, DestinationMac, NumPacketsToSend, 0, int.MaxValue, 2, PacketSize,
                           PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.02);

            // Break loop
            TestGetPackets(SourceMac, DestinationMac, NumPacketsToSend, NumPacketsToSend, 0, 2, PacketSize,
                           PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0.02);
            TestGetPackets(SourceMac, DestinationMac, NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, 2, PacketSize,
                           PacketCommunicatorReceiveResult.BreakLoop, NumPacketsToSend / 2, 0, 0.02);
        }

        [TestMethod]
        public void DumpPacketsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            string dumpFilename = Path.GetTempPath() + @"dump.pcap";
            const int NumPackets = 10;

            Packet expectedPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           GetRandomDatagram(10));

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                using (PacketDumpFile dumpFile = communicator.OpenDump(dumpFilename))
                {
                    for (int i = 0; i != NumPackets; ++i)
                    {
                        dumpFile.Dump(expectedPacket);
                        dumpFile.Flush();
                    }
                }
            }

            using (PacketCommunicator communicator = new OfflinePacketDevice(dumpFilename).Open())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                PacketCommunicatorReceiveResult result;
                Packet actualPacket;
                for (int i = 0; i != NumPackets; ++i)
                {
                    result = communicator.GetPacket(out actualPacket);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, actualPacket);
                    MoreAssert.IsInRange(expectedPacket.Timestamp.AddSeconds(-0.05), expectedPacket.Timestamp.AddSeconds(0.05),
                                         actualPacket.Timestamp);
                }

                result = communicator.GetPacket(out actualPacket);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Eof, result);
                Assert.IsNull(actualPacket);
            }
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

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           GetRandomDatagram(PacketSize - EthernetDatagram.HeaderLength));

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

        private void TestGetStatistics(string sourceMac, string destinationMac, int numPacketsToSend, int numStatisticsToGather, int numStatisticsToBreakLoop, double secondsToWait, int packetSize,
                                       PacketCommunicatorReceiveResult expectedResult, int expectedNumStatistics, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;

                communicator.SetFilter("ether src " + sourceMac + " and ether dst " + destinationMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(sourceMac),
                                                           new MacAddress(destinationMac),
                                                           EthernetType.IpV4,
                                                           GetRandomDatagram(packetSize - EthernetDatagram.HeaderLength));

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

        private void TestGetSomePackets(string sourceMac, string destinationMac, int numPacketsToSend, int numPacketsToBreakLoop, int packetSize, bool nonBlocking,
    PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            Packet packetToSend = PacketBuilder.Ethernet(DateTime.Now,
                                                         new MacAddress(sourceMac),
                                                         new MacAddress(destinationMac),
                                                         EthernetType.IpV4,
                                                         GetRandomDatagram(packetSize - EthernetDatagram.HeaderLength));


            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.NonBlocking = nonBlocking;
                communicator.SetFilter("ether src " + sourceMac + " and ether dst " + destinationMac);

                int numPacketsGot;
                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(packetToSend);

                int numPacketsHandled = 0;
                if (numPacketsToBreakLoop == 0)
                    communicator.Break();
                DateTime startWaiting = DateTime.Now;
                PacketCommunicatorReceiveResult result = communicator.GetSomePackets(out numPacketsGot, numPacketsToSend,
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

        private void TestGetPackets(string sourceMac, string destinationMac, int numPacketsToSend,
                            int numPacketsToWait, int numPacketsToBreakLoop, double secondsToWait, int packetSize,
                            PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets,
                            double expectedMinSeconds, double expectedMaxSeconds)
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + sourceMac + " and ether dst " + destinationMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(sourceMac),
                                                           new MacAddress(destinationMac),
                                                           EthernetType.IpV4,
                                                           GetRandomDatagram(packetSize - EthernetDatagram.HeaderLength));

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

                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedNumPackets, numPacketsHandled);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
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

        private Datagram GetRandomDatagram(int length)
        {
            byte[] buffer = new byte[length];
            _random.NextBytes(buffer);
            return new Datagram(buffer);
        }

        private Random _random = new Random();
    }
}