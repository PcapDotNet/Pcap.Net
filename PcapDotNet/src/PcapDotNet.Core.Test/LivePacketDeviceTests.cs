using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

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

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext{ get; set;}

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
                PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                Assert.AreEqual<uint>(0, communicator.TotalStatistics.PacketsCaptured);
                MoreAssert.IsInRange(TimeSpan.FromSeconds(0.99), TimeSpan.FromSeconds(1.075), finishedWaiting - startWaiting);

                Packet sentPacket = _random.NextEthernetPacket(24, SourceMac, DestinationMac);

                DateTime startSendingTime = DateTime.Now;

                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                DateTime endSendingTime = DateTime.Now;

                result = communicator.ReceivePacket(out packet);

                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                Assert.AreEqual<uint>(NumPacketsToSend, communicator.TotalStatistics.PacketsCaptured);
                Assert.AreEqual(sentPacket.Length, packet.Length);
                MoreAssert.IsInRange(startSendingTime - TimeSpan.FromSeconds(1), endSendingTime + TimeSpan.FromSeconds(30), packet.Timestamp);
            }
        }

        [TestMethod]
        public void ReceiveSomePacketsTest()
        {
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Test normal mode
            TestReceiveSomePackets(0, 0, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, 0, 1, 1.03);
            TestReceiveSomePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestReceiveSomePackets(NumPacketsToSend, 0, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestReceiveSomePackets(NumPacketsToSend, -1, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);
            TestReceiveSomePackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.031);

            // Test non blocking
            TestReceiveSomePackets(0, 0, int.MaxValue, PacketSize, true, PacketCommunicatorReceiveResult.Ok, 0, 0, 0.02);
            TestReceiveSomePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.02);

            // Test break loop
            TestReceiveSomePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketSize, false, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);
            TestReceiveSomePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketSize, true, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);
            TestReceiveSomePackets(NumPacketsToSend, NumPacketsToSend, 0, PacketSize, false, PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0.02);
        }

        [TestMethod]
        public void ReceivePacketsTest()
        {        
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Normal
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0, 0.041);

            // Wait for less packets
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0, 0.02);

            // Wait for more packets
            TestReceivePackets(NumPacketsToSend, 0, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.043);
            TestReceivePackets(NumPacketsToSend, -1, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.3);
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, 2, PacketSize, PacketCommunicatorReceiveResult.None, NumPacketsToSend, 2, 2.035);

            // Break loop
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend, 0, 2, PacketSize, PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0.027);
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, 2, PacketSize, PacketCommunicatorReceiveResult.BreakLoop, NumPacketsToSend / 2, 0, 0.02);
        }

        [TestMethod]
        public void ReceivePacketsEnumerableTest()
        {
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            // Normal
            TestReceivePacketsEnumerable(NumPacketsToSend, NumPacketsToSend, int.MaxValue, 2, PacketSize, NumPacketsToSend, 0, 0.3);

            // Wait for less packets
            TestReceivePacketsEnumerable(NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, 2, PacketSize, NumPacketsToSend / 2, 0, 0.027);

            // Wait for more packets
            TestReceivePacketsEnumerable(NumPacketsToSend, -1, int.MaxValue, 2, PacketSize, NumPacketsToSend, 2, 2.024);
            TestReceivePacketsEnumerable(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, 2, PacketSize, NumPacketsToSend, 2, 2.13);

            // Break loop
            TestReceivePacketsEnumerable(NumPacketsToSend, NumPacketsToSend, 0, 2, PacketSize, 0, 0, 0.02);
            TestReceivePacketsEnumerable(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, 2, PacketSize, NumPacketsToSend / 2, 0, 0.031);
        }

        [TestMethod]
        [Timeout(10 * 1000)]
        public void ReceivePacketsGcCollectTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            const int NumPackets = 2;

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);

                for (int i = 0; i != NumPackets; ++i)
                    communicator.SendPacket(sentPacket);

                PacketCommunicatorReceiveResult result = communicator.ReceivePackets(NumPackets, delegate
                                                                                                 {
                                                                                                     GC.Collect();
                                                                                                 });
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
            }
        }

        [TestMethod]
        public void ReceiveSomePacketsGcCollectTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            const int NumPackets = 2;

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);

                for (int i = 0; i != NumPackets; ++i)
                    communicator.SendPacket(sentPacket);

                int numGot;
                PacketCommunicatorReceiveResult result = communicator.ReceiveSomePackets(out numGot, NumPackets,
                                                                                         delegate 
                                                                                         {
                                                                                             GC.Collect();
                                                                                         });
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                Assert.AreEqual(NumPackets, numGot);
            }
        }

        [TestMethod]
        public void ReceiveStatisticsGcCollectTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            const int NumStatistics = 2;

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.Mode = PacketCommunicatorMode.Statistics;

                PacketCommunicatorReceiveResult result = communicator.ReceiveStatistics(NumStatistics, delegate
                                                                                                       {
                                                                                                           GC.Collect();
                                                                                                       });
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
            }
        }

        [TestMethod]
        public void ReceiveStatisticsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPacketsToSend = 100;
            const int PacketSize = 100;

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = _random.NextEthernetPacket(PacketSize, SourceMac, DestinationMac);

                PacketSampleStatistics statistics;
                PacketCommunicatorReceiveResult result = communicator.ReceiveStatistics(out statistics);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                MoreAssert.IsInRange(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1), statistics.Timestamp);
                Assert.AreEqual<uint>(0, statistics.AcceptedPackets);
                Assert.AreEqual<uint>(0, statistics.AcceptedBytes);

                for (int i = 0; i != NumPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                result = communicator.ReceiveStatistics(out statistics);

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
                              PacketCommunicatorReceiveResult.Ok, NumStatisticsToGather, NumPacketsToSend, NumStatisticsToGather, NumStatisticsToGather + 0.05);

            // Wait for less statistics
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather / 2, int.MaxValue, 5, PacketSize,
                              PacketCommunicatorReceiveResult.Ok, NumStatisticsToGather / 2, NumPacketsToSend, NumStatisticsToGather / 2, NumStatisticsToGather / 2 + 0.05);

            // Wait for more statistics
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, 0, int.MaxValue, 5.5, PacketSize,
                              PacketCommunicatorReceiveResult.None, 5, NumPacketsToSend, 5.5, 5.85);

            // Break loop
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather, 0, 5, PacketSize,
                              PacketCommunicatorReceiveResult.BreakLoop, 0, 0, 0, 0.04);
            TestGetStatistics(SourceMac, DestinationMac, NumPacketsToSend, NumStatisticsToGather, NumStatisticsToGather / 2, 5, PacketSize,
                              PacketCommunicatorReceiveResult.BreakLoop, NumStatisticsToGather / 2, NumPacketsToSend, NumStatisticsToGather / 2, NumStatisticsToGather / 2 + 0.1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetStatisticsOnCaptureModeErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                PacketSampleStatistics statistics;
                communicator.ReceiveStatistics(out statistics);
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
                communicator.ReceivePacket(out packet);
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

        // this test is removed for now since it doens't throw an exception for such big value
//        [TestMethod]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void SetBigKernelBufferSizeErrorTest()
//        {
//            using (PacketCommunicator communicator = OpenLiveDevice())
//            {
//                communicator.SetKernelBufferSize(1024 * 1024 * 1024);
//            }
//        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetPacketErrorTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetKernelBufferSize(10);
                Packet packet = _random.NextEthernetPacket(100, SourceMac, DestinationMac);
                communicator.SendPacket(packet);
                communicator.ReceivePacket(out packet);
            }
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetSomePacketsErrorTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetKernelBufferSize(10);
                Packet packet = _random.NextEthernetPacket(100, SourceMac, DestinationMac);
                communicator.SendPacket(packet);
                int numPacketsGot;
                communicator.ReceiveSomePackets(out numPacketsGot, 1, delegate { });
            }
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetPacketsErrorTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetKernelBufferSize(10);
                Packet packet = _random.NextEthernetPacket(100, SourceMac, DestinationMac);
                communicator.SendPacket(packet);
                Exception exception = null;
                Thread thread = new Thread(delegate()
                                           {
                                               try
                                               {
                                                   communicator.ReceivePackets(1, delegate { });
                                               }
                                               catch (Exception e)
                                               {
                                                   exception = e;
                                               }
                                           });
                thread.Start();
                if (!thread.Join(TimeSpan.FromSeconds(5)))
                    thread.Abort();

                if (exception != null)
                    throw exception;
            }

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetNextStatisticsErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                communicator.SetKernelBufferSize(10);
                PacketSampleStatistics statistics;
                communicator.ReceiveStatistics(out statistics);
            }

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetStatisticsErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                communicator.SetKernelBufferSize(10);
                communicator.ReceiveStatistics(1, delegate { Assert.Fail(); });
            }
            Assert.Fail();
        }

        [TestMethod]
        public void SetBigKernelMinimumBytesToCopyTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetKernelMinimumBytesToCopy(1024 * 1024);
                Packet expectedPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);
                for (int i = 0; i != 5; ++i)
                {
                    communicator.SendPacket(expectedPacket);
                    Packet packet;
                    DateTime start = DateTime.Now;
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    DateTime end = DateTime.Now;
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, packet);
                    MoreAssert.IsBigger(TimeSpan.FromSeconds(0.9), end - start);
                }
            }
        }

        [TestMethod]
        public void SetSmallKernelMinimumBytesToCopyTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetKernelMinimumBytesToCopy(1);
                Packet expectedPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);
                for (int i = 0; i != 100; ++i)
                {
                    communicator.SendPacket(expectedPacket);
                    Packet packet;
                    DateTime start = DateTime.Now;
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    DateTime end = DateTime.Now;
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, packet);
                    MoreAssert.IsSmallerOrEqual(TimeSpan.FromSeconds(0.07), end - start);
                }
            }
        }

        [TestMethod]
        public void SetSamplingMethodOneEveryNTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetSamplingMethod(new SamplingMethodOneEveryCount(5));
                for (int i = 0; i != 20; ++i)
                {
                    Packet expectedPacket = _random.NextEthernetPacket(60 * (i + 1), SourceMac, DestinationMac);
                    communicator.SendPacket(expectedPacket);
                }

                Packet packet;
                PacketCommunicatorReceiveResult result;
                for (int i = 0; i != 4; ++i)
                {
                    result = communicator.ReceivePacket(out packet);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(60 * 5 * (i + 1), packet.Length);
                }
                result = communicator.ReceivePacket(out packet);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                Assert.IsNull(packet);
            }
        }

        [TestMethod]
        public void SetSamplingMethodFirstAfterIntervalTest()
        {
            Random random = new Random();

            MacAddress sourceMac = random.NextMacAddress();
            MacAddress destinationMac = random.NextMacAddress();

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + sourceMac + " and ether dst " + destinationMac);
                communicator.SetSamplingMethod(new SamplingMethodFirstAfterInterval(TimeSpan.FromSeconds(1)));
                int numPacketsGot;
                communicator.ReceiveSomePackets(out numPacketsGot, 100, p => { });
                Packet expectedPacket = _random.NextEthernetPacket(60, sourceMac, destinationMac);
                List<Packet> packets = new List<Packet>(6);
                Thread thread = new Thread(() => packets.AddRange(communicator.ReceivePackets(6)));
                thread.Start();
                communicator.SendPacket(expectedPacket);
                Thread.Sleep(TimeSpan.FromSeconds(0.75));
                for (int i = 0; i != 10; ++i)
                {
                    expectedPacket = _random.NextEthernetPacket(60 * (i + 2), sourceMac, destinationMac);
                    communicator.SendPacket(expectedPacket);
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }

                if (!thread.Join(TimeSpan.FromSeconds(10)))
                    thread.Abort();
                Assert.AreEqual(6, packets.Count);
                Packet packet;
                for (int i = 0; i != 6; ++i)
                {
//                    result = communicator.ReceivePacket(out packet);
//                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
//                    Assert.AreEqual(60 * (i * 2 + 1), packet.Length, i.ToString());
                    Assert.AreEqual(60 * (i * 2 + 1), packets[i].Length, i.ToString());
                }
                PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                Assert.IsNull(packet);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetSamplingMethodOneEveryNErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetSamplingMethod(new SamplingMethodOneEveryCount(0));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetSamplingMethodFirstAfterIntervalNegativeMsErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetSamplingMethod(new SamplingMethodFirstAfterInterval(-1));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetSamplingMethodFirstAfterIntervalNegativeTimespanErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetSamplingMethod(new SamplingMethodFirstAfterInterval(TimeSpan.FromSeconds(-1)));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetSamplingMethodFirstAfterIntervalBigTimespanErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetSamplingMethod(new SamplingMethodFirstAfterInterval(TimeSpan.FromDays(25)));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetInvalidDataLink()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.DataLink = new PcapDataLink(0);
                Assert.AreEqual(new PcapDataLink(0), communicator.DataLink);
            }
        }

        [TestMethod]
        public void SendZeroPacket()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SendPacket(new Packet(new byte[0], DateTime.Now, DataLinkKind.Ethernet));
            }
        }

        private static void TestGetStatistics(string sourceMac, string destinationMac, int numPacketsToSend, int numStatisticsToGather, int numStatisticsToBreakLoop, double secondsToWait, int packetSize,
                                              PacketCommunicatorReceiveResult expectedResult, int expectedNumStatistics, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;

                communicator.SetFilter("ether src " + sourceMac + " and ether dst " + destinationMac);

                Packet sentPacket = _random.NextEthernetPacket(packetSize, sourceMac, destinationMac);

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
                    result = communicator.ReceiveStatistics(numStatisticsToGather,
                                                     delegate(PacketSampleStatistics statistics)
                                                     {
                                                         Assert.IsNotNull(statistics.ToString());
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

        private static void TestReceiveSomePackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop, int packetSize, bool nonBlocking,
                                                   PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            string TestDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToGet=" + numPacketsToGet +
                                     ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop + ". PacketSize=" + packetSize +
                                     ". NonBlocking=" + nonBlocking;

            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Packet packetToSend = _random.NextEthernetPacket(packetSize, SourceMac, DestinationMac);

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.NonBlocking = nonBlocking;
                Assert.AreEqual(nonBlocking, communicator.NonBlocking);
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                int numPacketsGot;
                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(packetToSend);

                if (numPacketsToBreakLoop == 0)
                    communicator.Break();

                PacketHandler handler = new PacketHandler(packetToSend, communicator, numPacketsToBreakLoop);
                DateTime startWaiting = DateTime.Now;
                PacketCommunicatorReceiveResult result = communicator.ReceiveSomePackets(out numPacketsGot, numPacketsToGet,
                                                                                         handler.Handle);
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedNumPackets, numPacketsGot, "NumPacketsGot. Test: " + TestDescription);
                Assert.AreEqual(expectedNumPackets, handler.NumPacketsHandled, "NumPacketsHandled. Test: " + TestDescription);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds, TestDescription);
            }
        }

        private static void TestReceivePackets(int numPacketsToSend, int numPacketsToWait, int numPacketsToBreakLoop, double secondsToWait, int packetSize,
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

                Packet sentPacket = _random.NextEthernetPacket(packetSize, SourceMac, DestinationMac);

                PacketCommunicatorReceiveResult result = PacketCommunicatorReceiveResult.None;

                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                PacketHandler handler = new PacketHandler(sentPacket, communicator, numPacketsToBreakLoop);

                Thread thread = new Thread(delegate()
                {
                    if (numPacketsToBreakLoop == 0)
                        communicator.Break();
                    result = communicator.ReceivePackets(numPacketsToWait, handler.Handle);
                });

                DateTime startWaiting = DateTime.Now;
                thread.Start();

                if (!thread.Join(TimeSpan.FromSeconds(secondsToWait)))
                    thread.Abort();
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(expectedResult, result, TestDescription);
                Assert.AreEqual(expectedNumPackets, handler.NumPacketsHandled);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
            }
        }

        private static void TestReceivePacketsEnumerable(int numPacketsToSend, int numPacketsToWait, int numPacketsToBreakLoop, double secondsToWait,
                                                         int packetSize, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            string TestDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToWait=" + numPacketsToWait +
                                     ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop + ". SecondsToWait=" +
                                     secondsToWait + ". PacketSize=" + packetSize;


            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                Packet sentPacket = _random.NextEthernetPacket(packetSize, SourceMac, DestinationMac);

                for (int i = 0; i != numPacketsToSend; ++i)
                    communicator.SendPacket(sentPacket);

                int actualPacketsReceived = 0;
                Thread thread = new Thread(delegate()
                {
                    if (numPacketsToBreakLoop == 0)
                        communicator.Break();
                    IEnumerable<Packet> packets = numPacketsToWait == -1
                                                      ? communicator.ReceivePackets()
                                                      : communicator.ReceivePackets(numPacketsToWait);
                    foreach (Packet packet in packets)
                    {
                        Assert.AreEqual(sentPacket, packet);
                        ++actualPacketsReceived;
                        if (actualPacketsReceived == numPacketsToBreakLoop)
                            break;
                    }
                });

                DateTime startWaiting = DateTime.Now;
                thread.Start();

                if (!thread.Join(TimeSpan.FromSeconds(secondsToWait)))
                    thread.Abort();
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(expectedNumPackets, actualPacketsReceived, TestDescription);
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds, TestDescription);
            }
        }

        public static PacketCommunicator OpenLiveDevice()
        {
            IList<LivePacketDevice> devices = LivePacketDevice.AllLocalMachine;
            MoreAssert.IsBiggerOrEqual(1, devices.Count);
            LivePacketDevice device = devices[0];
            MoreAssert.IsMatch(@"Network adapter '.*' on local host", device.Description);
            Assert.AreEqual(DeviceAttributes.None, device.Attributes);
            Assert.AreNotEqual(MacAddress.Zero, device.GetMacAddress());
            MoreAssert.IsInRange(1, 2, device.Addresses.Count);
            foreach (DeviceAddress address in device.Addresses)
            {
                if (address.Address.Family == SocketAddressFamily.Internet)
                {
                    MoreAssert.IsMatch("Address: " + SocketAddressFamily.Internet + @" [0-9]+\.[0-9]+\.[0-9]+\.[0-9]+ " +
                                       "Netmask: " + SocketAddressFamily.Internet + @" 255\.[0-9]+\.[0-9]+\.[0-9]+ " +
                                       "Broadcast: " + SocketAddressFamily.Internet + @" 255.255.255.255",
                                       address.ToString());
                }
                else
                {
                    Assert.AreEqual(SocketAddressFamily.Internet6, address.Address.Family);
                    MoreAssert.IsMatch("Address: " + SocketAddressFamily.Internet6 + @" (?:[0-9A-F]{4}:){7}[0-9A-F]{4} " +
                                       "Netmask: " + SocketAddressFamily.Unspecified + @" " + IpV6Address.Zero + " " +
                                       "Broadcast: " + SocketAddressFamily.Unspecified + @" " + IpV6Address.Zero,
                                       address.ToString());
                }
            }
            PacketCommunicator communicator = device.Open();
            try
            {
                PacketTotalStatistics totalStatistics = communicator.TotalStatistics;
                Assert.AreEqual<object>(totalStatistics, totalStatistics);
                Assert.AreNotEqual(null, totalStatistics);
                Assert.AreEqual(totalStatistics.GetHashCode(), totalStatistics.GetHashCode());
                Assert.IsTrue(totalStatistics.Equals(totalStatistics));
                Assert.IsFalse(totalStatistics.Equals(null));
                Assert.AreNotEqual(null, totalStatistics);
                Assert.AreNotEqual(totalStatistics, 2);
                MoreAssert.IsSmallerOrEqual<uint>(1, totalStatistics.PacketsCaptured, "PacketsCaptured");
                Assert.AreEqual<uint>(0, totalStatistics.PacketsDroppedByDriver, "PacketsDroppedByDriver");
                Assert.AreEqual<uint>(0, totalStatistics.PacketsDroppedByInterface, "PacketsDroppedByInterface");
                MoreAssert.IsSmallerOrEqual<uint>(1, totalStatistics.PacketsReceived, "PacketsReceived");
                Assert.IsNotNull(totalStatistics.ToString());
                communicator.SetKernelBufferSize(2 * 1024 * 1024); // 2 MB instead of 1
                communicator.SetKernelMinimumBytesToCopy(10); // 10 bytes minimum to copy
                communicator.SetSamplingMethod(new SamplingMethodNone());
                Assert.AreEqual(DataLinkKind.Ethernet, communicator.DataLink.Kind);
                communicator.DataLink = communicator.DataLink;
                Assert.AreEqual("EN10MB (Ethernet)", communicator.DataLink.ToString());
                Assert.AreEqual(communicator.DataLink, new PcapDataLink(communicator.DataLink.Name));
                Assert.IsTrue(communicator.IsFileSystemByteOrder);
                Assert.AreEqual(PacketCommunicatorMode.Capture, communicator.Mode);
                Assert.IsFalse(communicator.NonBlocking);
                Assert.AreEqual(PacketDevice.DefaultSnapshotLength, communicator.SnapshotLength);
                return communicator;
            }
            catch (Exception)
            {
                communicator.Dispose();
                throw;
            }
        }

        private static Random _random = new Random();
    }
}