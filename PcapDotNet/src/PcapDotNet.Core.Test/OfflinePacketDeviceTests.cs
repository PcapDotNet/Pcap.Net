using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for OfflinePacketDeviceTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class OfflinePacketDeviceTests
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

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

        private static void TestOpenMultipleTimes(int numTimes, string filename)
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPackets = 10;
            Packet expectedPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);
            PacketDevice device = GetOfflineDevice(NumPackets, expectedPacket, TimeSpan.Zero, Path.GetTempPath() + @"dump.pcap", Path.GetTempPath() + filename);
            for (int j = 0; j != numTimes; ++j)
            {
                using (PacketCommunicator communicator = device.Open())
                {
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
        }

        [TestMethod]
        public void OpenOfflineMultipleTimes()
        {
            TestOpenMultipleTimes(1000, @"dump.pcap");
        }

        [TestMethod]
        public void OpenOfflineMultipleTimesUnicode()
        {
            // TODO: Fix so we can go beyond 509 when using unicode filenames. See http://www.winpcap.org/pipermail/winpcap-bugs/2012-December/001547.html
            TestOpenMultipleTimes(100, @"דמפ.pcap");
        }

        [TestMethod]
        public void GetPacketTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPackets = 10;

            Packet expectedPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);

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
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend, int.MaxValue, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend, 0.05, 0.05);
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend / 2, int.MaxValue, PacketCommunicatorReceiveResult.Ok, NumPacketsToSend / 2, 0.05, 0.05);

            // Eof
            TestReceivePackets(NumPacketsToSend, 0, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestReceivePackets(NumPacketsToSend, -1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend + 1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, NumPacketsToSend, 0.05, 0.05);
            TestReceivePackets(0, -1, int.MaxValue, PacketCommunicatorReceiveResult.Eof, 0, 0.05, 0.05);

            // Break loop
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend, NumPacketsToSend / 2, PacketCommunicatorReceiveResult.BreakLoop, NumPacketsToSend / 2, 0.05, 0.05);
            TestReceivePackets(NumPacketsToSend, NumPacketsToSend, 0, PacketCommunicatorReceiveResult.BreakLoop, 0, 0.05, 0.05);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
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
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void GetTotalStatisticsErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                Assert.IsNotNull(communicator.TotalStatistics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void OpenInvalidFileTest()
        {
            using (new OfflinePacketDevice("myinvalidfile").Open())
            {
            }
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void OpenNullFilenameTest()
        {
            using (new OfflinePacketDevice(null).Open())
            {
            }
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void SendPacketErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                communicator.SendPacket(_random.NextEthernetPacket(100));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void SetKernelBufferSizeErrorTest()
        {
            using (PacketCommunicator communicator = OpenOfflineDevice())
            {
                communicator.SetKernelBufferSize(1024 * 1024);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
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
            Packet expectedPacket = _random.NextEthernetPacket(100);
            using (PacketCommunicator communicator = OpenOfflineDevice(101, expectedPacket))
            {
                communicator.SetSamplingMethod(new SamplingMethodOneEveryCount(10));
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
            
            Packet expectedPacket = _random.NextEthernetPacket(100);
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void DumpToBadFileTest()
        {
            OpenOfflineDevice(10, _random.NextEthernetPacket(100), TimeSpan.Zero, "??");
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void EmptyNameTest()
        {
            OpenOfflineDevice(10, _random.NextEthernetPacket(100), TimeSpan.Zero, string.Empty);
        }

        [TestMethod]
        public void ReadWriteIso88591FilenameTest()
        {
            const string DumpFilename = "abc_\u00F9\u00E8.pcap";
            const int NumPackets = 10;
            Packet expectedPacket = PacketBuilder.Build(DateTime.Now, new EthernetLayer { EtherType = EthernetType.IpV4 });
            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, expectedPacket, TimeSpan.FromSeconds(0.1), DumpFilename))
            {
                for (int i = 0; i != NumPackets; ++i)
                {
                    Packet actualPacket;
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, communicator.ReceivePacket(out actualPacket));
                    Assert.AreEqual(expectedPacket, actualPacket);
                }
            }

            Assert.IsTrue(File.Exists(DumpFilename), string.Format("File {0} doesn't exist", DumpFilename));
        }

        // TODO: Add this test once Dumping to files with Unicode filenames is supported. See http://www.winpcap.org/pipermail/winpcap-users/2011-February/004273.html
//        [TestMethod]
//        public void ReadWriteUnicodeFilenameTest()
//        {
//            const string DumpFilename = "abc_\u00F9_\u05D0\u05D1\u05D2.pcap";
//            const int NumPackets = 10;
//            Packet expectedPacket = PacketBuilder.Build(DateTime.Now, new EthernetLayer {EtherType = EthernetType.IpV4});
//            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, expectedPacket, TimeSpan.FromSeconds(0.1), DumpFilename))
//            {
//                for (int i = 0; i != NumPackets; ++i)
//                {
//                    Packet actualPacket;
//                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, communicator.ReceivePacket(out actualPacket));
//                    Assert.AreEqual(expectedPacket, actualPacket);
//                }
//            }
//
//            Assert.IsTrue(File.Exists(DumpFilename), "File " + DumpFilename, " doesn't exist");
//        }

        [TestMethod]
        public void ReadUnicodeFilenameTest()
        {
            const string ReadUnicodeFilename = "abc_\u00F9_\u05D0\u05D1\u05D2.pcap";
            const string DumpAsciiFilename = "abc.pcap";
            const int NumPackets = 10;
            Packet expectedPacket = PacketBuilder.Build(DateTime.Now, new EthernetLayer { EtherType = EthernetType.IpV4 });
            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, expectedPacket, TimeSpan.FromSeconds(0.1), DumpAsciiFilename, ReadUnicodeFilename))
            {
                for (int i = 0; i != NumPackets; ++i)
                {
                    Packet actualPacket;
                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, communicator.ReceivePacket(out actualPacket));
                    Assert.AreEqual(expectedPacket, actualPacket);
                }
            }

            Assert.IsTrue(File.Exists(ReadUnicodeFilename), string.Format("File {0} doesn't exist", ReadUnicodeFilename));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void ReadNonExistingUnicodeFilenameTest()
        {
            const string ReadUnicodeFilename = "abc_non_existing_\u00F9_\u05D0\u05D1\u05D2.pcap";
            OfflinePacketDevice device = new OfflinePacketDevice(ReadUnicodeFilename);
            using (PacketCommunicator communicator = device.Open())
            {
                Assert.Fail();
            }
        }

        private static void TestGetSomePackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop,
                                               PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets,
                                               double expectedMinSeconds, double expectedMaxSeconds)
        {
            string testDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToGet=" + numPacketsToGet +
                                     ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop;

            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Packet expectedPacket = _random.NextEthernetPacket(100, SourceMac, DestinationMac);

            using (PacketCommunicator communicator = OpenOfflineDevice(numPacketsToSend, expectedPacket))
            {
                communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);

                if (numPacketsToBreakLoop == 0)
                    communicator.Break();
                PacketHandler handler = new PacketHandler(expectedPacket, expectedMinSeconds, expectedMaxSeconds, communicator, numPacketsToBreakLoop);

                int numPacketsGot;
                PacketCommunicatorReceiveResult result = communicator.ReceiveSomePackets(out numPacketsGot, numPacketsToGet, handler.Handle);
                Assert.AreEqual(expectedResult, result);
                Assert.AreEqual(expectedNumPackets, numPacketsGot, "NumPacketsGot. Test: " + testDescription);
                Assert.AreEqual(expectedNumPackets, handler.NumPacketsHandled, "NumPacketsHandled. Test: " + testDescription);
            }
        }

        private static void TestReceivePackets(int numPacketsToSend, int numPacketsToGet, int numPacketsToBreakLoop,
                                               PacketCommunicatorReceiveResult expectedResult, int expectedNumPackets,
                                               double expectedMinSeconds, double expectedMaxSeconds)
        {
            string testDescription = "NumPacketsToSend=" + numPacketsToSend + ". NumPacketsToGet=" + numPacketsToGet +
                         ". NumPacketsToBreakLoop=" + numPacketsToBreakLoop;

            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            Packet expectedPacket = _random.NextEthernetPacket(24, SourceMac, DestinationMac);

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

                Assert.AreEqual(expectedResult, result, testDescription);
                Assert.AreEqual(expectedNumPackets, handler.NumPacketsHandled);
            }
        }

        public static OfflinePacketDevice GetOfflineDevice(int numPackets, Packet packet)
        {
            return GetOfflineDevice(numPackets, packet, TimeSpan.Zero);
        }

        public static OfflinePacketDevice GetOfflineDevice(int numPackets, Packet packet, TimeSpan intervalBetweenPackets)
        {
            return GetOfflineDevice(numPackets, packet, intervalBetweenPackets, Path.GetTempPath() + @"dump.pcap");
        }

        public static OfflinePacketDevice GetOfflineDevice(int numPackets, Packet packet, TimeSpan intervalBetweenPackets, string dumpFilename, string readFilename = null)
        {
            if (readFilename == null)
                readFilename = dumpFilename;
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

            if (readFilename != dumpFilename)
            {
                if (File.Exists(readFilename))
                    File.Delete(readFilename);
                File.Move(dumpFilename, readFilename);
            }

            OfflinePacketDevice device = new OfflinePacketDevice(readFilename);
            Assert.AreEqual(0, device.Addresses.Count);
            Assert.AreEqual(string.Empty, device.Description);
            Assert.AreEqual(DeviceAttributes.None, device.Attributes);
            Assert.AreEqual(readFilename, device.Name);

            return device;
        }

        public static PacketCommunicator OpenOfflineDevice()
        {
            return OpenOfflineDevice(10, _random.NextEthernetPacket(100));
        }

        public static PacketCommunicator OpenOfflineDevice(int numPackets, Packet packet)
        {
            return OpenOfflineDevice(numPackets, packet, TimeSpan.Zero);
        }

        public static PacketCommunicator OpenOfflineDevice(int numPackets, Packet packet, TimeSpan intervalBetweenPackets)
        {
            return OpenOfflineDevice(numPackets, packet, intervalBetweenPackets, Path.GetTempPath() + @"dump.pcap");
        }

        private static PacketCommunicator OpenOfflineDevice(int numPackets, Packet packet, TimeSpan intervalBetweenPackets, string dumpFilename, string readFilename = null)
        {
            IPacketDevice device = GetOfflineDevice(numPackets, packet, intervalBetweenPackets, dumpFilename, readFilename);
            PacketCommunicator communicator = device.Open();
            try
            {
                MoreAssert.AreSequenceEqual(new[] {DataLinkKind.Ethernet}.Select(kind => new PcapDataLink(kind)), communicator.SupportedDataLinks);
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

        private static readonly Random _random = new Random();
    }
}