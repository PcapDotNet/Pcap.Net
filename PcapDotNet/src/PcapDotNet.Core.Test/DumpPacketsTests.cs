using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Packets;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for DumpPacketsTests
    /// </summary>
    [TestClass]
    public class DumpPacketsTests
    {
        public DumpPacketsTests()
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
        public void DumpIpV4PacketTest()
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");
            const EthernetType ethernetType = EthernetType.IpV4;

            Random random = new Random();

            byte ipV4TypeOfService = (byte)random.Next(256);
            ushort ipV4Identification = (ushort)random.Next(65536);
            byte ipV4Ttl = (byte)random.Next(256);
            IpV4FragmentationFlags ipV4FragmentationFlags = (IpV4FragmentationFlags)(random.Next(4) << 13);
            ushort ipV4FragmentationOffset = (ushort)random.Next(65536);
            IpV4Fragmentation ipV4Fragmentation = new IpV4Fragmentation(ipV4FragmentationFlags, ipV4FragmentationOffset);
            const IpV4Protocol ipV4Protocol = IpV4Protocol.Tcp;
            IpV4Address ipV4Source = new IpV4Address((uint)random.Next());
            IpV4Address ipV4Destination = new IpV4Address((uint)random.Next());
            IpV4Options ipV4Options = IpV4Options.None;

            byte[] ipV4PayloadBuffer = new byte[random.Next(0, 50 * 1024)];
            random.NextBytes(ipV4PayloadBuffer);
            Datagram ipV4Payload = new Datagram(ipV4PayloadBuffer);

            Packet packet = PacketBuilder.IpV4(DateTime.Now,
                                               ethernetSource, ethernetDestination, ethernetType,
                                               ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl, ipV4Protocol,
                                               ipV4Source, ipV4Destination, ipV4Options,
                                               ipV4Payload);

            using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
            {
//                using (PacketDumpFile dumpFile = communicator.OpenDump(@"c:\wow.pcap"))
//                {
//                    dumpFile.Dump(packet);
//                }
            }
        }
    }
}