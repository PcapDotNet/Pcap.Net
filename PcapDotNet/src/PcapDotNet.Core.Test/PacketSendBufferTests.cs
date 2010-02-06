using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PacketSendQueueTests
    /// </summary>
    [TestClass]
    public class PacketSendQueueTests
    {
        public PacketSendQueueTests()
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
        public void TransmitQueueToLiveTest()
        {
            TestTransmitQueueToLive(0, 100, 0.5, false);
            TestTransmitQueueToLive(10, 60, 0.5, false);
            TestTransmitQueueToLive(10, 600, 0.5, false);
            TestTransmitQueueToLive(10, 1500, 0.5, false);
            TestTransmitQueueToLive(10, 60, 0.5, true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TransmitQueueToOfflineTest()
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            List<Packet> packetsToSend;
            using (PacketSendBuffer queue = BuildQueue(out packetsToSend, 100, 100, SourceMac, DestinationMac, 0.5))
            {
                using (PacketCommunicator communicator = OfflinePacketDeviceTests.OpenOfflineDevice())
                {
                    communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                    communicator.Transmit(queue, false);
                }
            }
        }

        private static void TestTransmitQueueToLive(int numPacketsToSend, int packetSize, double secondsBetweenTimestamps, bool isSynced)
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";

            List<Packet> packetsToSend;
            using (PacketSendBuffer queue = BuildQueue(out packetsToSend, numPacketsToSend, packetSize, SourceMac, DestinationMac, secondsBetweenTimestamps))
            {
                using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
                {
                    communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                    communicator.Transmit(queue, isSynced);

                    DateTime lastTimestamp = DateTime.MinValue;
                    int numPacketsHandled = 0;
                    int numPacketsGot;
                    PacketCommunicatorReceiveResult result =
                        communicator.ReceiveSomePackets(out numPacketsGot, numPacketsToSend,
                                                    delegate(Packet packet)
                                                        {
                                                            Assert.AreEqual(packetsToSend[numPacketsHandled], packet);
                                                            if (numPacketsHandled > 0)
                                                            {
                                                                TimeSpan expectedDiff;
                                                                if (isSynced)
                                                                {
                                                                    expectedDiff =
                                                                        packetsToSend[numPacketsHandled].Timestamp -
                                                                        packetsToSend[numPacketsHandled - 1].Timestamp;
                                                                }
                                                                else
                                                                {
                                                                    expectedDiff = TimeSpan.Zero;
                                                                }
                                                                TimeSpan actualDiff = packet.Timestamp - lastTimestamp;
                                                                MoreAssert.IsInRange(
                                                                    expectedDiff.Subtract(TimeSpan.FromSeconds(0.01)),
                                                                    expectedDiff.Add(TimeSpan.FromSeconds(0.03)),
                                                                    actualDiff);
                                                            }
                                                            lastTimestamp = packet.Timestamp;
                                                            ++numPacketsHandled;
                                                        });

                    Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                    Assert.AreEqual(numPacketsToSend, numPacketsGot);
                    Assert.AreEqual(numPacketsToSend, numPacketsHandled);
                }
            }
        }

        private static PacketSendBuffer BuildQueue(out List<Packet> packetsToSend, int numPackets, int packetSize, string sourceMac, string destinationMac, double secondsBetweenTimestamps)
        {
            int rawPacketSize = packetSize + 16; // I don't know why 16

            PacketSendBuffer queue = new PacketSendBuffer((uint)(numPackets * rawPacketSize));
            try
            {
                DateTime timestamp = DateTime.Now.AddSeconds(-100);
                packetsToSend = new List<Packet>(numPackets);
                for (int i = 0; i != numPackets; ++i)
                {
                    Packet packetToSend = _random.NextEthernetPacket(packetSize, timestamp, sourceMac, destinationMac);
                    queue.Enqueue(packetToSend);
                    packetsToSend.Add(packetToSend);
                    timestamp = timestamp.AddSeconds(secondsBetweenTimestamps);
                }
            }
            catch (Exception)
            {
                queue.Dispose();
                throw;
            }

            return queue;
        }

        private static Random _random = new Random();
    }
}