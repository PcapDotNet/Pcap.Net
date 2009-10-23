using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IcmpTests
    /// </summary>
    [TestClass]
    public class IcmpTests
    {
        public IcmpTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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
        public void RandomIcmpTest()
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");

            Random random = new Random();

            byte ipV4TypeOfService = random.NextByte();
            ushort ipV4Identification = random.NextUShort();
            byte ipV4Ttl = random.NextByte();
            IpV4Fragmentation ipV4Fragmentation = random.NextIpV4Fragmentation();
            IpV4Address ipV4Source = new IpV4Address(random.NextUInt());
            IpV4Address ipV4Destination = new IpV4Address(random.NextUInt());
            IpV4Options ipV4Options = random.NextIpV4Options();

            for (int i = 0; i != 1000; ++i)
            {
                IcmpMessageType icmpMessageType = random.NextEnum<IcmpMessageType>();

                Packet packet;
                switch (icmpMessageType)
                {
                    case IcmpMessageType.DestinationUnreachable:
                        IcmpCodeDestinationUnrechable code = random.NextEnum<IcmpCodeDestinationUnrechable>();
                        byte icmpIpV4TypeOfService = random.NextByte();
                        ushort icmpIpV4Identification = random.NextUShort();
                        byte icmpIpV4Ttl = random.NextByte();
                        IpV4Fragmentation icmpIpV4Fragmentation = random.NextIpV4Fragmentation();
                        IpV4Protocol icmpIpV4Protocol = random.NextEnum<IpV4Protocol>();
                        IpV4Address icmpIpV4Source = new IpV4Address(random.NextUInt());
                        IpV4Address icmpIpV4Destination = new IpV4Address(random.NextUInt());
                        IpV4Options icmpIpV4Options = random.NextIpV4Options();
                        Datagram icmpIpV4Payload = random.NextDatagram(random.Next(200));

                        packet = PacketBuilder.EthernetIpV4IcmpDestinationUnreachable(DateTime.Now,
                                                                                      ethernetSource, ethernetDestination,
                                                                                      ipV4TypeOfService, ipV4Identification, ipV4Fragmentation,
                                                                                      ipV4Ttl,
                                                                                      ipV4Source, ipV4Destination, ipV4Options,
                                                                                      code,
                                                                                      icmpIpV4TypeOfService, icmpIpV4Identification,
                                                                                      icmpIpV4Fragmentation,
                                                                                      icmpIpV4Ttl, icmpIpV4Protocol, icmpIpV4Source,
                                                                                      icmpIpV4Destination,
                                                                                      icmpIpV4Options, icmpIpV4Payload);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid icmpMessageType " + icmpMessageType);
                }

                Assert.IsTrue(packet.IsValid, "IsValid");
            }
        }
    }
}