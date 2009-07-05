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
                PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                DateTime finishedWaiting = DateTime.Now;

                Assert.AreEqual(PacketCommunicatorReceiveResult.Timeout, result);
                Assert.AreEqual<uint>(0, communicator.TotalStatistics.PacketsCaptured);
                MoreAssert.IsInRange(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1.02), finishedWaiting - startWaiting);

                Packet sentPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 24);

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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetBigKernelBufferSizeErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetKernelBufferSize(1024 * 1024 * 1024);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetPacketErrorTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetKernelBufferSize(50);
                Packet packet = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);
                communicator.SendPacket(packet);
                communicator.ReceivePacket(out packet);
            }
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
                communicator.SetKernelBufferSize(50);
                Packet packet = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);
                communicator.SendPacket(packet);
                int numPacketsGot;
                communicator.ReceiveSomePackets(out numPacketsGot, 1, delegate { });
            }
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
                communicator.SetKernelBufferSize(50);
                Packet packet = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);
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
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetNextStatisticsErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                communicator.SetKernelBufferSize(50);
                PacketSampleStatistics statistics;
                communicator.ReceiveStatistics(out statistics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetSmallKernelBufferSizeGetStatisticsErrorTest()
        {
            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
                communicator.SetKernelBufferSize(50);
                communicator.ReceiveStatistics(1, delegate { });
            }
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
                Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);
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
                Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);
                for (int i = 0; i != 100; ++i)
                {
                    communicator.SendPacket(expectedPacket);
                    Packet packet;
                    DateTime start = DateTime.Now;
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    DateTime end = DateTime.Now;
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, packet);
                    MoreAssert.IsSmallerOrEqual(TimeSpan.FromSeconds(0.02), end - start);
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
                communicator.SetSamplingMethod(new SamplingMethodOneEveryN(5));
                for (int i = 0; i != 20; ++i)
                {
                    Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 60 * (i + 1));
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
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            using (PacketCommunicator communicator = OpenLiveDevice())
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                communicator.SetSamplingMethod(new SamplingMethodFirstAfterInterval(TimeSpan.FromSeconds(1)));
                Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 60);
                communicator.SendPacket(expectedPacket);
                Thread.Sleep(TimeSpan.FromSeconds(0.75));
                for (int i = 0; i != 10; ++i)
                {
                    expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 60 * (i + 2));
                    communicator.SendPacket(expectedPacket);
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }

                Packet packet;
                PacketCommunicatorReceiveResult result;
                for (int i = 0; i != 6; ++i)
                {
                    result = communicator.ReceivePacket(out packet);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(60 * (i * 2 + 1), packet.Length);
                }
                result = communicator.ReceivePacket(out packet);
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
                communicator.SetSamplingMethod(new SamplingMethodOneEveryN(0));
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

        private static void TestGetStatistics(string sourceMac, string destinationMac, int numPacketsToSend, int numStatisticsToGather, int numStatisticsToBreakLoop, double secondsToWait, int packetSize,
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

        private static void TestGetSomePackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop, int packetSize, bool nonBlocking,
                                               PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets, double expectedMinSeconds, double expectedMaxSeconds)
        {
            string TestDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToGet=" + numPacketsToGet +
                                     ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop + ". PacketSize=" + packetSize +
                                     ". NonBlocking=" + nonBlocking;

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
                MoreAssert.IsInRange(expectedMinSeconds, expectedMaxSeconds, (finishedWaiting - startWaiting).TotalSeconds);
            }
        }

        private static void TestGetPackets(int numPacketsToSend, int numPacketsToWait, int numPacketsToBreakLoop, double secondsToWait, int packetSize,
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