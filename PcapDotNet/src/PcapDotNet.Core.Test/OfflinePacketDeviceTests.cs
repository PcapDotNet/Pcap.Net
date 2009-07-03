using System;
using System.Collections.Generic;
using System.IO;
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
        public void DumpPacketsTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            const int NumPackets = 10;

            Packet expectedPacket = MoreRandom.BuildRandomPacket(SourceMac, DestinationMac, 24);

            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, expectedPacket))
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
        public void SetNonBlockTest()
        {
            const int NumPackets = 10;

            Packet packet = MoreRandom.BuildRandomPacket(24);

            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, packet))
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
        public void GetTotalStatisticsTest()
        {
            const int NumPackets = 10;

            Packet packet = MoreRandom.BuildRandomPacket(24);

            using (PacketCommunicator communicator = OpenOfflineDevice(NumPackets, packet))
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

        public static PacketCommunicator OpenOfflineDevice(int numPackets, Packet packet)
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
                        dumpFile.Dump(packet);
                        MoreAssert.IsBigger(lastPosition, dumpFile.Position);
                        lastPosition = dumpFile.Position;
                        dumpFile.Flush();
                    }
                }
            }

            communicator = new OfflinePacketDevice(dumpFilename).Open();
            try
            {
                Assert.AreEqual(DataLinkKind.Ethernet, communicator.DataLink.Kind);
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
    }
}