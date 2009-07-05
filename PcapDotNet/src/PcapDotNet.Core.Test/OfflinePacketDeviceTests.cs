using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Packets;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for OfflinePacketDeviceTests
    /// </summary>
    [TestClass]
    public class OfflinePacketDeviceTests
    {
        public OfflinePacketDeviceTests()
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
        public void GetPacketTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPackets = 10;

            Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);

            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, expectedPacket))
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                PacketCommunicatorReceiveResult result;
                Packet actualPacket;
                for (int i = 0; i != NumPackets; ++i)
                {
                    result = communicator.ReceivePacket(out actualPacket);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, actualPacket);
                    MoreAssert.IsInRange(expectedPacket.Timestamp.AddSeconds(-0.05), expectedPacket.Timestamp.AddSeconds(0.05),
                                         actualPacket.Timestamp);
                }

                result = communicator.ReceivePacket(out actualPacket);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Eof, result);
                Assert.IsNull(actualPacket);
            }
        }

        [TestMethod]
        public void GetSomePacketsTest()
        {
            const int NumPacketsToSend = 100;

            // Normal
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0.05, 0.05);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0.05, 0.05);
            
            // Eof
            TestGetSomePackets(NumPacketsToSend, 0, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestGetSomePackets(NumPacketsToSend, -1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);

            // Break loop
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0.05, 0.05);
            TestGetSomePackets(NumPacketsToSend, NumPacketsToSend, 0, PacketCommunicatorReceiveResult.BreakLoop, 0, 0.05, 0.05);
        }

        [TestMethod]
        public void GetPacketsTest()
        {
            const int NumPacketsToSend = 100;

            // Normal
            TestGetPackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0.05, 0.05);
            TestGetPackets(NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0.05, 0.05);

            // Eof
            TestGetPackets(NumPacketsToSend, 0, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestGetPackets(NumPacketsToSend, -1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestGetPackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);

            // Break loop
            TestGetPackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketCommunicatorReceiveResult.BreakLoop, NumPacketsToSend / 2, 0.05, 0.05);
            TestGetPackets(NumPacketsToSend, NumPacketsToSend, 0, PacketCommunicatorReceiveResult.BreakLoop, 0, 0.05, 0.05);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void StatisticsModeErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                communicator.Mode = PacketCommunicatorMode.Statistics;
            }
        }

        [TestMethod]
        public void SetNonBlockTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                Assert.AreEqual(false, communicator.NonBlocking);
                communicator.NonBlocking = false;
                Assert.AreEqual(false, communicator.NonBlocking);
                communicator.NonBlocking = true;
                Assert.AreEqual(false, communicator.NonBlocking);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetTotalStatisticsErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                Assert.IsNotNull(communicator.TotalStatistics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OpenInvalidFileTest()
        {
            using (PacketCommunicator communicator = new OfflinePacketDevice("myinvalidfile").Open())
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendPacketErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                communicator.SendPacket(MoreRandom.BuildRandomPacket(100));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetKernelBufferSizeErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                communicator.SetKernelBufferSize(1024 * 1024);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetlKernelMinimumBytesToCopyErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                communicator.SetKernelMinimumBytesToCopy(1024);
            }
        }

        [TestMethod]
        public void SetSamplingMethodOneEveryNTest()
        {
            Packet expectedPacket = MoreRandom.BuildRandomPacket(100);
            using (PacketCommunicator communicator = OpenOfflineDevice(101, expectedPacket))
            {
                communicator.SetSamplingMethod(new SamplingMethodOneEveryN(10));
                PacketCommunicatorReceiveResult result;
                Packet packet;
                for (int i = 0; i != 10; ++i)
                {
                    result = communicator.ReceivePacket(out packet);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, packet);
                }
                result = communicator.ReceivePacket(out packet);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Eof, result);
                Assert.IsNull(packet);
            }
        }

        [TestMethod]
        public void SetSamplingMethodFirstAfterIntervalTest()
        {
            const int NumPackets = 10;
            
            Packet expectedPacket = MoreRandom.BuildRandomPacket(100);
            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, expectedPacket, TimeSpan.FromSeconds(1)))
            {
                communicator.SetSamplingMethod(new SamplingMethodFirstAfterInterval(TimeSpan.FromSeconds(2)));

                PacketCommunicatorReceiveResult result;
                Packet packet;
                for (int i = 0; i != 5; ++i)
                {
                    result = communicator.ReceivePacket(out packet);
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(expectedPacket, packet);
                    DateTime expectedTimestamp = expectedPacket.Timestamp.AddSeconds(i * 2);
                    MoreAssert.IsInRange(expectedTimestamp.AddSeconds(-0.01), expectedTimestamp.AddSeconds(0.01), packet.Timestamp);
                }
                result = communicator.ReceivePacket(out packet);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Eof, result);
                Assert.IsNull(packet);
            }
        }

        private static void TestGetSomePackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop,
                                               PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets,
                                               double expectedMinSeconds, double expectedMaxSeconds)
        {
            string TestDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToGet=" + numPacketsToGet +
                                     ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop;

            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 100);

            using (PacketCommunicator communicator = OpenOfflineDevice(numPacketsToSend, expectedPacket))
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                if (numPacketsToBreakLoop == 0)
                    communicator.Break();
                PacketHandler handler = new PacketHandler(expectedPacket, expectedMinSeconds, expectedMaxSeconds, communicator, numPacketsToBreakLoop);

                int numPacketsGot;
                PacketCommunicatorReceiveResult result = communicator.ReceiveSomePackets(out numPacketsGot, numPacketsToGet, handler.Handle);
                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedNumPackets, numPacketsGot, "NumPacketsGot. Test: " + TestDescription);
                Assert.AreEqual(expectedNumPackets, handler.NumPacketsHandled, "NumPacketsHandled. Test: " + TestDescription);
            }
        }

        private static void TestGetPackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop,
                                           PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets,
                                           double expectedMinSeconds, double expectedMaxSeconds)
        {
            string TestDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToGet=" + numPacketsToGet +
                         ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop;

            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 24);

            using (PacketCommunicator communicator = OpenOfflineDevice(numPacketsToSend, expectedPacket))
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                if (numPacketsToBreakLoop == 0)
                    communicator.Break();
                PacketHandler handler = new PacketHandler(expectedPacket, expectedMinSeconds, expectedMaxSeconds, communicator, numPacketsToBreakLoop);

                PacketCommunicatorReceiveResult result = PacketCommunicatorReceiveResult.None;
                Thread thread = new Thread(delegate()
                {
                    result = communicator.ReceivePackets(numPacketsToGet, handler.Handle);
                });
                thread.Start();

                if (!thread.Join(TimeSpan.FromSeconds(5)))
                {
                    thread.Abort();
                }

                Assert.AreEqual(expectedResult, result, TestDescription);
                Assert.AreEqual(expectedNumPackets, handler.NumPacketsHandled);
            }
        }

        public static PacketCommunicator OpenOfflineDevice()
        {
            return OpenOfflineDevice(10, MoreRandom.BuildRandomPacket(100));
        }


        public static PacketCommunicator OpenOfflineDevice(int numPackets, Packet packet)
        {
            return OpenOfflineDevice(numPackets, packet, TimeSpan.Zero);
        }

        public static PacketCommunicator OpenOfflineDevice(int numPackets, Packet packet, TimeSpan intervalBetweenPackets)
        {
            string dumpFilename = Path.GetTempPath() + @"dump.pcap";

            PacketCommunicator communicator;
            using (communicator = LivePacketDeviceTests.OpenLiveDevice())
            {
                using (PacketDumpFile dumpFile = communicator.OpenDump(dumpFilename))
                {
                    int lastPosition = 0;
                    for (int i = 0; i != numPackets; ++i)
                    {
                        if (intervalBetweenPackets != TimeSpan.Zero && i != 0)
                        {
//                            Thread.Sleep(intervalBetweenPackets);
                            DateTime timestamp = packet.Timestamp;
                            timestamp = timestamp.Add(intervalBetweenPackets);
                            packet = new Packet(packet.Buffer, timestamp, packet.DataLink);
                        }
                        dumpFile.Dump(packet);
                        MoreAssert.IsBigger(lastPosition, dumpFile.Position);
                        lastPosition = dumpFile.Position;
                        dumpFile.Flush();
                    }
                }
            }

            IPacketDevice device = new OfflinePacketDevice(dumpFilename);
            Assert.AreEqual(0, device.Addresses.Count);
            Assert.AreEqual(string.Empty, device.Description);
            Assert.AreEqual(DeviceFlags.None, device.Flags);
            Assert.AreEqual(dumpFilename, device.Name);
            communicator = device.Open();
            try
            {
                Assert.AreEqual(DataLinkKind.Ethernet, communicator.DataLink.Kind);
                Assert.AreEqual("EN10MB (Ethernet)", communicator.DataLink.ToString());
                Assert.AreEqual(communicator.DataLink, new PcapDataLink(communicator.DataLink.Name));
                Assert.IsTrue(communicator.IsFileSystemByteOrder);
                Assert.AreEqual(PacketCommunicatorMode.Capture, communicator.Mode);
                Assert.IsFalse(communicator.NonBlocking);
                Assert.AreEqual(PacketDevice.DefaultSnapshotLength, communicator.SnapshotLength);
                Assert.AreEqual(2, communicator.FileMajorVersion);
                Assert.AreEqual(4, communicator.FileMinorVersion);
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