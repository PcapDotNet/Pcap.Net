using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Packets;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PcapLibTests
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
        public void TransmitQueueTest()
        {
            TestTransmitQueue(0, 100, 0.5, false);
            TestTransmitQueue(10, 60, 0.5, false);
            TestTransmitQueue(10, 600, 0.5, false);
            TestTransmitQueue(10, 1500, 0.5, false);
            TestTransmitQueue(10, 60, 0.5, true);
        }

        private static void TestTransmitQueue(int numPacketsToSend, int packetSize, double secondsBetweenTimestamps, bool isSynced)
        {
            const string SourceMac = "11:22:33:44:55:66";
            const string DestinationMac = "77:88:99:AA:BB:CC";
            int rawPacketSize = packetSize + 16; // I don't know why 16

            using (PacketSendQueue queue = new PacketSendQueue((uint)(numPacketsToSend * rawPacketSize)))
            {
                DateTime timestamp = DateTime.Now.AddSeconds(-100);
                List<Packet> packetsToSend = new List<Packet>(numPacketsToSend);
                for (int i = 0; i != numPacketsToSend; ++i)
                {
                    Packet packetToSend = MoreRandom.BuildRandomPacket(timestamp, SourceMac, DestinationMac, packetSize);
                    queue.Enqueue(packetToSend);
                    packetsToSend.Add(packetToSend);
                    timestamp = timestamp.AddSeconds(secondsBetweenTimestamps);
                }

                using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
                {
                    communicator.SetFilter("ether src " + SourceMac + " and ether dst " + DestinationMac);
                    communicator.Transmit(queue, isSynced);

                    DateTime lastTimestamp = DateTime.MinValue;
                    int numPacketsHandled = 0;
                    int numPacketsGot;
                    PacketCommunicatorReceiveResult result =
                        communicator.GetSomePackets(out numPacketsGot, numPacketsToSend,
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
                                                                Assert.AreEqual(expectedDiff, actualDiff);
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
    }
}