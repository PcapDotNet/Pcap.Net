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

            TestGetSomePackets(SourceMac, DestinationMac, 0, PacketSize, false, PacketCommunicatorReceiveResult.Ok, 0, 1, 1.02);
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestGetSomePackets(SourceMac, DestinationMac, 0, PacketSize, true, PacketCommunicatorReceiveResult.Ok, 0, 0, 0.02);
            TestGetSomePackets(SourceMac, DestinationMac, NumPacketsToSend, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
        }

        private void TestGetSomePackets(string sourceMac, string destinationMac, int numPacketsToSend, int packetSize, bool nonBlocking, 
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
                DateTime startWaiting = DateTime.Now;
                PacketCommunicatorReceiveResult result = communicator.GetSomePackets(out numPacketsGot, numPacketsToSend,
                                                                                     delegate(Packet packet)
                                                                                         {
                                                                                            Assert.AreEqual(packetToSend, packet);
                                                                                             ++numPacketsHandled;
                                                                                         });
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedNumPackets, numPacketsGot);
                Assert.AreEqual(expectedNumPackets, numPacketsHandled);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
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
                                                   GetRandomDatagram(10));

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
        public void GetPacketsTest()
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
                                                           GetRandomDatagram(10));

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

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;

                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = PacketBuilder.Ethernet(DateTime.Now,
                                                           new MacAddress(SourceMac),
                                                           new MacAddress(DestinationMac),
                                                           EthernetType.IpV4,
                                                           GetRandomDatagram(PacketSize - EthernetDatagram.HeaderLength));

                PacketCommunicatorReceiveResult result = PacketCommunicatorReceiveResult.None;
                int numStatisticsGot = 0;
                ulong totalPackets = 0;
                ulong totalBytes = 0;
                Thread thread = new Thread(delegate()
                {
                    result = communicator.GetStatistics(NumStatisticsToGather,
                                                     delegate(PacketSampleStatistics statistics)
                                                     {
                                                         totalPackets += statistics.AcceptedPackets;
                                                         totalBytes += statistics.AcceptedBytes;
                                                         ++numStatisticsGot;
                                                     });
                });
                thread.Start();

                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                if (!thread.Join(TimeSpan.FromSeconds(5)))
                    thread.Abort();

                Assert.AreEqual(NumStatisticsToGather, numStatisticsGot, "NumStatistics");
                Assert.AreEqual<ulong>(NumPacketsToSend, totalPackets, "NumPackets");
                // Todo check bytes statistics
//                Assert.AreEqual<ulong>((ulong)(NumPacketsToSend * sentPacket.Length), totalBytes, "NumBytes");
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result, "Result");
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