using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for UdpTests
    /// </summary>
    [TestClass]
    public class IgmpTests
    {
        public IgmpTests()
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
        public void RandomIgmpTest()
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
                IgmpMessageType igmpMessageType = random.NextEnum(IgmpMessageType.None);
                IgmpQueryVersion igmpQueryVersion = IgmpQueryVersion.None;
                TimeSpan igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(256 * 0.1) - TimeSpan.FromTicks(1));
                IpV4Address igmpGroupAddress = random.NextIpV4Address();
                bool? igmpIsSuppressRouterSideProcessing = null;
                byte? igmpQueryRobustnessVariable = null;
                TimeSpan? igmpQueryInterval = null;
                IpV4Address[] igmpSourceAddresses = null;
                IgmpGroupRecord[] igmpGroupRecords = null;

                Packet packet;
                switch (igmpMessageType)
                {
                    case IgmpMessageType.MembershipQuery:
                        igmpQueryVersion = random.NextEnum(IgmpQueryVersion.None, IgmpQueryVersion.Unknown);
                        switch (igmpQueryVersion)
                        {
                            case IgmpQueryVersion.Version1:
                                igmpMaxResponseTime = TimeSpan.Zero;
                                packet = PacketBuilder.EthernetIpV4IgmpQueryVersion1(DateTime.Now,
                                                                                     ethernetSource, ethernetDestination,
                                                                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                                     ipV4Source, ipV4Destination, ipV4Options,
                                                                                     igmpGroupAddress);
                                break;

                            case IgmpQueryVersion.Version2:
                                packet = PacketBuilder.EthernetIpV4IgmpQueryVersion2(DateTime.Now,
                                                                                     ethernetSource, ethernetDestination,
                                                                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                                     ipV4Source, ipV4Destination, ipV4Options,
                                                                                     igmpMaxResponseTime, igmpGroupAddress);
                                break;

                            case IgmpQueryVersion.Version3:
                                igmpIsSuppressRouterSideProcessing = random.NextBool();
                                igmpQueryRobustnessVariable = random.NextByte(8);
                                igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                          IgmpDatagram.MaxVersion3MaxResponseTime - TimeSpan.FromTicks(1));
                                igmpQueryInterval = random.NextTimeSpan(TimeSpan.Zero, IgmpDatagram.MaxVersion3QueryInterval - TimeSpan.FromTicks(1));
                                igmpSourceAddresses = random.NextIpV4Addresses(random.Next(1000));
                                packet = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
                                                                                     ethernetSource, ethernetDestination,
                                                                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                                     ipV4Source, ipV4Destination, ipV4Options,
                                                                                     igmpMaxResponseTime, igmpGroupAddress,
                                                                                     igmpIsSuppressRouterSideProcessing.Value,
                                                                                     igmpQueryRobustnessVariable.Value, igmpQueryInterval.Value,
                                                                                     igmpSourceAddresses);
                                break;

                            default:
                                continue;
                        }
                        break;

                    case IgmpMessageType.MembershipReportVersion1:
                        igmpMaxResponseTime = TimeSpan.Zero;
                        packet = PacketBuilder.EthernetIpV4IgmpReportVersion1(DateTime.Now,
                                                                              ethernetSource, ethernetDestination,
                                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                              ipV4Source, ipV4Destination, ipV4Options,
                                                                              igmpGroupAddress);
                        break;

                    case IgmpMessageType.MembershipReportVersion2:
                        packet = PacketBuilder.EthernetIpV4IgmpReportVersion2(DateTime.Now,
                                                                              ethernetSource, ethernetDestination,
                                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                              ipV4Source, ipV4Destination, ipV4Options,
                                                                              igmpMaxResponseTime, igmpGroupAddress);
                        break;

                    case IgmpMessageType.LeaveGroupVersion2:
                        packet = PacketBuilder.EthernetIpV4IgmpLeaveGroupVersion2(DateTime.Now,
                                                                                  ethernetSource, ethernetDestination,
                                                                                  ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                                  ipV4Source, ipV4Destination, ipV4Options,
                                                                                  igmpMaxResponseTime, igmpGroupAddress);
                        break;

                    case IgmpMessageType.MembershipReportVersion3:
                        igmpMaxResponseTime = TimeSpan.Zero;
                        igmpGroupRecords = random.NextIgmpGroupRecords(random.Next(100));
                        packet = PacketBuilder.EthernetIpV4IgmpReportVersion3(DateTime.Now,
                                                                              ethernetSource, ethernetDestination,
                                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
                                                                              ipV4Source, ipV4Destination, ipV4Options,
                                                                              igmpGroupRecords);
                        break;

                    default:
                        continue;
                }

                Assert.IsTrue(packet.IsValid, "IsValid");

                // IGMP
                Assert.IsTrue(packet.Ethernet.IpV4.Igmp.IsChecksumCorrect);
                Assert.AreEqual(igmpMessageType, packet.Ethernet.IpV4.Igmp.MessageType);
                Assert.AreEqual(igmpQueryVersion, packet.Ethernet.IpV4.Igmp.QueryVersion);
                MoreAssert.IsInRange(igmpMaxResponseTime.Divide(2), igmpMaxResponseTime,
                                     packet.Ethernet.IpV4.Igmp.MaxResponseTime, "MaxResponseTime");
                if (igmpMessageType != IgmpMessageType.MembershipReportVersion3)
                    Assert.AreEqual(igmpGroupAddress, packet.Ethernet.IpV4.Igmp.GroupAddress, "GroupAddress");

                // Query Version 3
                if (igmpQueryVersion == IgmpQueryVersion.Version3)
                {
                    Assert.AreEqual(igmpIsSuppressRouterSideProcessing.Value, packet.Ethernet.IpV4.Igmp.IsSuppressRouterSideProcessing,
                                    "IsSuppressRouterSideProcessing");
                    MoreAssert.IsInRange(igmpQueryInterval.Value.Divide(2), igmpQueryInterval.Value, packet.Ethernet.IpV4.Igmp.QueryInterval);
                    Assert.AreEqual(igmpQueryRobustnessVariable, packet.Ethernet.IpV4.Igmp.QueryRobustnessVariable);
                    Assert.AreEqual(igmpSourceAddresses.Length, packet.Ethernet.IpV4.Igmp.NumberOfSources);
                    MoreAssert.AreSequenceEqual(igmpSourceAddresses, packet.Ethernet.IpV4.Igmp.SourceAddresses);
                }

                if (igmpMessageType == IgmpMessageType.MembershipReportVersion3)
                {
                    Assert.AreEqual(igmpGroupRecords.Length, packet.Ethernet.IpV4.Igmp.NumberOfGroupRecords);
                    MoreAssert.AreSequenceEqual(igmpGroupRecords, packet.Ethernet.IpV4.Igmp.GroupRecords.Select(record => record.ToGroupRecord()));
                }
            }
        }
    }
}