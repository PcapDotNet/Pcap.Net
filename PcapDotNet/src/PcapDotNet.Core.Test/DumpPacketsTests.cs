using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

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

            /*
            using (PacketCommunicator communicator = LivePacketDeviceTests.OpenLiveDevice())
            {
                using (PacketDumpFile dumpFile = communicator.OpenDump(@"c:\wow.pcap"))
                {
                    for (int i = 0; i != 1000; ++i)
                    {
                        byte ipV4TypeOfService = random.NextByte();
                        ushort ipV4Identification = random.NextUShort();
                        byte ipV4Ttl = random.NextByte();
                        IpV4FragmentationOptions ipV4FragmentationFlags = random.NextEnum<IpV4FragmentationOptions>();
                        ushort ipV4FragmentationOffset = random.NextUShort();
                        IpV4Fragmentation ipV4Fragmentation = new IpV4Fragmentation(ipV4FragmentationFlags, ipV4FragmentationOffset);
                        IpV4Protocol ipV4Protocol = random.NextEnum<IpV4Protocol>();
                        IpV4Address ipV4Source = new IpV4Address(random.NextUInt());
                        IpV4Address ipV4Destination = new IpV4Address(random.NextUInt());
                        IpV4Options ipV4Options = random.NextIpV4Options();

                        byte[] ipV4PayloadBuffer = new byte[random.Next(0, 50 * 1024)];
                        random.NextBytes(ipV4PayloadBuffer);
                        Datagram ipV4Payload = new Datagram(ipV4PayloadBuffer);

                        Packet packet = PacketBuilder.EthernetIpV4(DateTime.Now,
                                                                   ethernetSource, ethernetDestination, ethernetType,
                                                                   ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl, ipV4Protocol,
                                                                   ipV4Source, ipV4Destination, ipV4Options,
                                                                   ipV4Payload);

                        dumpFile.Dump(packet);
                        dumpFile.Flush();
                    }
                }
            }
            */ 
        }
    }
}