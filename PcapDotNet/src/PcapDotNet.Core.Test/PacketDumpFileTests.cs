using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PacketDumpFileTests
    /// </summary>
    [TestClass]
    public class PacketDumpFileTests
    {
        public PacketDumpFileTests()
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
        public void DumpWithoutDeviceTest()
        {
            string filename = Path.GetTempPath() + @"dump.pcap";

            Packet expectedPacket = PacketBuilder.Build(DateTime.Now,
                                                         new EthernetLayer
                                                             {
                                                                 Source = new MacAddress(1),
                                                                 Destination = new MacAddress(2),
                                                                 EtherType = EthernetType.QInQ,
                                                             },
                                                         new PayloadLayer
                                                             {
                                                                 Data = new Datagram(new byte[] {1, 2, 3})
                                                             });
            PacketDumpFile.Dump(filename, new PcapDataLink(DataLinkKind.Ethernet), PacketDevice.DefaultSnapshotLength,
                                new[] {expectedPacket});

            using (PacketCommunicator communicator = new OfflinePacketDevice(filename).Open())
            {
                Packet actualPacket;
                PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out actualPacket);
                Assert.AreEqual(PacketCommunicatorReceiveResult.Ok, result);
                Assert.AreEqual(expectedPacket, actualPacket);
            }
        }

//        [TestMethod]
//        public void Temp()
//        {
//            EthernetLayer ethernetLayer = new EthernetLayer
//                                              {
//                                                  Source = new MacAddress("00:01:02:03:04:05"),
//                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
//                                              };
//
//            IpV4Layer ipV4Layer = new IpV4Layer
//                                      {
//                                          Source = new IpV4Address("1.2.3.4"),
//                                          Ttl = 128,
//                                      };
//
//            IcmpEchoLayer icmpLayer = new IcmpEchoLayer();
//
//            PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer);
//
//            List<Packet> packets = new List<Packet>();
//
//            for (int i = 0; i != 100; ++i)
//            {
//                ipV4Layer.Destination = new IpV4Address("2.3.4." + i);
//                ipV4Layer.Identification = (ushort)i;
//                icmpLayer.SequenceNumber = (ushort)i;
//                icmpLayer.Identifier = (ushort)i;
//
//                packets.Add(builder.Build(DateTime.Now));
//            }
//
//            PacketDumpFile.Dump(@"c:\users\boaz\temp.pcap", new PcapDataLink(DataLinkKind.Ethernet), int.MaxValue, packets);
//        }
    }
}