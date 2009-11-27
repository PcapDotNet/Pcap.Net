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
    /// Summary description for IgmpTests
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
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = new IpV4Layer
                                      {
                                          TypeOfService = random.NextByte(),
                                          Identification = random.NextUShort(),
                                          Ttl = random.NextByte(),
                                          Fragmentation = random.NextIpV4Fragmentation(),
                                          Source = random.NextIpV4Address(),
                                          Destination = random.NextIpV4Address(),
                                          Options = random.NextIpV4Options()
                                      };

            for (int i = 0; i != 1000; ++i)
            {
//                IgmpMessageType igmpMessageType = random.NextEnum(IgmpMessageType.None);
//                IgmpQueryVersion igmpQueryVersion = IgmpQueryVersion.None;
//                TimeSpan igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(256 * 0.1) - TimeSpan.FromTicks(1));
//                IpV4Address igmpGroupAddress = random.NextIpV4Address();
//                bool? igmpIsSuppressRouterSideProcessing = null;
//                byte? igmpQueryRobustnessVariable = null;
//                TimeSpan? igmpQueryInterval = null;
//                IpV4Address[] igmpSourceAddresses = null;
//                IgmpGroupRecord[] igmpGroupRecords = null;

                IgmpLayer igmpLayer = random.NextIgmpLayer();
//                switch (igmpMessageType)
//                {
//                    case IgmpMessageType.MembershipQuery:
//                        igmpQueryVersion = random.NextEnum(IgmpQueryVersion.None, IgmpQueryVersion.Unknown);
//                        switch (igmpQueryVersion)
//                        {
//                            case IgmpQueryVersion.Version1:
//                                igmpMaxResponseTime = TimeSpan.Zero;
//                                igmpLayer = new IgmpQueryVersion1Layer
//                                                {
//                                                    GroupAddress = igmpGroupAddress
//                                                };
//                                                                
////                                packet = PacketBuilder.EthernetIpV4IgmpQueryVersion1(DateTime.Now,
////                                                                                     ethernetSource, ethernetDestination,
////                                                                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                                     ipV4Source, ipV4Destination, ipV4Options,
////                                                                                     igmpGroupAddress);
//                                break;
//
//                            case IgmpQueryVersion.Version2:
//                                igmpLayer = new IgmpQueryVersion2Layer
//                                                {
//                                                    MaxResponseTime = igmpMaxResponseTime,
//                                                    GroupAddress = igmpGroupAddress
//                                                };
////                                packet = PacketBuilder.EthernetIpV4IgmpQueryVersion2(DateTime.Now,
////                                                                                     ethernetSource, ethernetDestination,
////                                                                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                                     ipV4Source, ipV4Destination, ipV4Options,
////                                                                                     igmpMaxResponseTime, igmpGroupAddress);
//                                break;
//
//                            case IgmpQueryVersion.Version3:
//                                igmpIsSuppressRouterSideProcessing = random.NextBool();
//                                igmpQueryRobustnessVariable = random.NextByte(8);
//                                igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
//                                                                          IgmpDatagram.MaxVersion3MaxResponseTime - TimeSpan.FromTicks(1));
//                                igmpQueryInterval = random.NextTimeSpan(TimeSpan.Zero, IgmpDatagram.MaxQueryInterval - TimeSpan.FromTicks(1));
//                                igmpSourceAddresses = random.NextIpV4Addresses(random.Next(1000));
//                                igmpLayer = new IgmpQueryVersion3Layer
//                                                                {
//                                                                    MaxResponseTime = igmpMaxResponseTime,
//                                                                    GroupAddress = igmpGroupAddress,
//                                                                    IsSuppressRouterSideProcessing = igmpIsSuppressRouterSideProcessing.Value,
//                                                                    QueryRobustnessVariable = igmpQueryRobustnessVariable.Value,
//                                                                    QueryInterval = igmpQueryInterval.Value,
//                                                                    SourceAddresses = igmpSourceAddresses
//                                                                };
//
////                                packet = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
////                                                                                     ethernetSource, ethernetDestination,
////                                                                                     ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                                     ipV4Source, ipV4Destination, ipV4Options,
////                                                                                     igmpMaxResponseTime, igmpGroupAddress,
////                                                                                     igmpIsSuppressRouterSideProcessing.Value,
////                                                                                     igmpQueryRobustnessVariable.Value, igmpQueryInterval.Value,
////                                                                                     igmpSourceAddresses);
//                                break;
//
//                            default:
//                                continue;
//                        }
//                        break;
//
//                    case IgmpMessageType.MembershipReportVersion1:
//                        igmpMaxResponseTime = TimeSpan.Zero;
//                        igmpLayer = new IgmpReportVersion1Layer
//                                        {
//                                            GroupAddress = igmpGroupAddress
//                                        };
//                        
////                        packet = PacketBuilder.EthernetIpV4IgmpReportVersion1(DateTime.Now,
////                                                                              ethernetSource, ethernetDestination,
////                                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                              ipV4Source, ipV4Destination, ipV4Options,
////                                                                              igmpGroupAddress);
//                        break;
//
//                    case IgmpMessageType.MembershipReportVersion2:
//                        igmpLayer = new IgmpReportVersion2Layer
//                                        {
//                                            MaxResponseTime = igmpMaxResponseTime,
//                                            GroupAddress = igmpGroupAddress
//                                        };
//
////                        packet = PacketBuilder.EthernetIpV4IgmpReportVersion2(DateTime.Now,
////                                                                              ethernetSource, ethernetDestination,
////                                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                              ipV4Source, ipV4Destination, ipV4Options,
////                                                                              igmpMaxResponseTime, igmpGroupAddress);
//                        break;
//
//                    case IgmpMessageType.LeaveGroupVersion2:
//                        igmpLayer = new IgmpLeaveGroupVersion2Layer
//                                        {
//                                            MaxResponseTime = igmpMaxResponseTime,
//                                            GroupAddress = igmpGroupAddress
//                                        };
//                        //                        packet = PacketBuilder.EthernetIpV4IgmpLeaveGroupVersion2(DateTime.Now,
////                                                                                  ethernetSource, ethernetDestination,
////                                                                                  ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                                  ipV4Source, ipV4Destination, ipV4Options,
////                                                                                  igmpMaxResponseTime, igmpGroupAddress);
//                        break;
//
//                    case IgmpMessageType.MembershipReportVersion3:
//                        igmpMaxResponseTime = TimeSpan.Zero;
//                        igmpGroupRecords = random.NextIgmpGroupRecords(random.Next(100));
//                        igmpLayer = new IgmpReportVersion3Layer
//                                        {
//                                            GroupRecords = igmpGroupRecords
//                                        };
//                        //                        packet = PacketBuilder.EthernetIpV4IgmpReportVersion3(DateTime.Now,
////                                                                              ethernetSource, ethernetDestination,
////                                                                              ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl,
////                                                                              ipV4Source, ipV4Destination, ipV4Options,
////                                                                              igmpGroupRecords);
//                        break;
//
//                    default:
//                        throw new InvalidOperationException("Invalid message type " + igmpMessageType);
//                }

                Packet packet = new PacketBuilder2(ethernetLayer, ipV4Layer, igmpLayer).Build(DateTime.Now);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // IGMP
                Assert.IsTrue(packet.Ethernet.IpV4.Igmp.IsChecksumCorrect);
                Assert.AreEqual(igmpLayer.MessageType, packet.Ethernet.IpV4.Igmp.MessageType);
                Assert.AreEqual(igmpLayer.QueryVersion, packet.Ethernet.IpV4.Igmp.QueryVersion);
                MoreAssert.IsInRange(igmpLayer.MaxResponseTimeValue.Divide(2), igmpLayer.MaxResponseTimeValue,
                                     packet.Ethernet.IpV4.Igmp.MaxResponseTime, "MaxResponseTime");
                if (igmpLayer.MessageType != IgmpMessageType.MembershipReportVersion3)
                    Assert.AreEqual(((IIgmpLayerWithGroupAddress)igmpLayer).GroupAddress, packet.Ethernet.IpV4.Igmp.GroupAddress, "GroupAddress");

                // Query Version 3
                if (igmpLayer.QueryVersion == IgmpQueryVersion.Version3)
                {
                    IgmpQueryVersion3Layer igmpQueryVersion3Layer = (IgmpQueryVersion3Layer)igmpLayer;
                    Assert.AreEqual(igmpQueryVersion3Layer.IsSuppressRouterSideProcessing, packet.Ethernet.IpV4.Igmp.IsSuppressRouterSideProcessing,
                                    "IsSuppressRouterSideProcessing");
                    MoreAssert.IsInRange(igmpQueryVersion3Layer.QueryInterval.Divide(2), igmpQueryVersion3Layer.QueryInterval, packet.Ethernet.IpV4.Igmp.QueryInterval);
                    Assert.AreEqual(igmpQueryVersion3Layer.QueryRobustnessVariable, packet.Ethernet.IpV4.Igmp.QueryRobustnessVariable);
                    Assert.AreEqual(igmpQueryVersion3Layer.SourceAddresses.Length, packet.Ethernet.IpV4.Igmp.NumberOfSources);
                    MoreAssert.AreSequenceEqual(igmpQueryVersion3Layer.SourceAddresses, packet.Ethernet.IpV4.Igmp.SourceAddresses);
                }

                // Report Version 3
                if (igmpLayer.MessageType == IgmpMessageType.MembershipReportVersion3)
                {
                    IgmpReportVersion3Layer igmpReportVersion3Layer= (IgmpReportVersion3Layer)igmpLayer;
                    Assert.AreEqual(igmpReportVersion3Layer.GroupRecords.Length, packet.Ethernet.IpV4.Igmp.NumberOfGroupRecords);
                    MoreAssert.AreSequenceEqual(igmpReportVersion3Layer.GroupRecords, packet.Ethernet.IpV4.Igmp.GroupRecords.Select(record => record.ToGroupRecord()));
                    Assert.IsTrue(igmpReportVersion3Layer.GroupRecords.All(record => record.ToString() != null));
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IgmpQueryVersion3SmallMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromSeconds(-1), IpV4Address.Zero, false, 0, TimeSpan.FromSeconds(1),
                                                                        new IpV4Address[] {});

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IgmpQueryVersion3BigMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromHours(1), IpV4Address.Zero, false, 0, TimeSpan.FromSeconds(1),
                                                                        new IpV4Address[] { });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IgmpQueryVersion3SmallQueryIntervalTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromSeconds(1), IpV4Address.Zero, false, 0, TimeSpan.FromSeconds(-1),
                                                                        new IpV4Address[] { });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IgmpQueryVersion3BigQueryIntervalTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromSeconds(1), IpV4Address.Zero, false, 0, TimeSpan.FromHours(9),
                                                                        new IpV4Address[] { });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IgmpQueryVersion2SmallMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion2(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromSeconds(-1), IpV4Address.Zero);

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IgmpQueryVersion2BigMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion2(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromMinutes(5), IpV4Address.Zero);

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        public void IgmpInvalidTest()
        {
            Packet queryVersion2 = PacketBuilder.EthernetIpV4IgmpQueryVersion2(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        TimeSpan.FromSeconds(1), IpV4Address.Zero);
            Assert.IsTrue(queryVersion2.IsValid);
            Assert.IsTrue(queryVersion2.Ethernet.IpV4.Igmp.IsChecksumCorrect);

            // Small Packet
            byte[] buffer = new byte[queryVersion2.Length - 1];
            queryVersion2.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            Packet smallQueryVersion2 = new Packet(buffer, queryVersion2.Timestamp, queryVersion2.DataLink);
            Assert.IsFalse(smallQueryVersion2.IsValid);

            // Bad checksum
            buffer = new byte[queryVersion2.Length];
            queryVersion2.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            ++buffer[buffer.Length - 1];
            Packet badChecksumPacket = new Packet(buffer, queryVersion2.Timestamp, queryVersion2.DataLink);
            Assert.IsFalse(badChecksumPacket.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(badChecksumPacket.IsValid);

            // Big query version 3
            Packet queryVersion3 = PacketBuilder.EthernetIpV4IgmpQueryVersion3(DateTime.Now,
                                                                               new MacAddress(), new MacAddress(),
                                                                               0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                               IpV4Options.None,
                                                                               TimeSpan.FromSeconds(1), IpV4Address.Zero, false, 0,
                                                                               TimeSpan.FromSeconds(1), new IpV4Address[] {});
            buffer = new byte[queryVersion3.Length + 1];
            queryVersion3.Buffer.BlockCopy(0, buffer, 0, queryVersion3.Length);
            Packet bigQueryVersion3 = new Packet(buffer, queryVersion3.Timestamp, queryVersion3.DataLink);
            Assert.IsTrue(bigQueryVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(bigQueryVersion3.IsValid);

            // Big report version 1
            Packet reportVersion1 = PacketBuilder.EthernetIpV4IgmpReportVersion1(DateTime.Now,
                                                                               new MacAddress(), new MacAddress(),
                                                                               0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                               IpV4Options.None,
                                                                               IpV4Address.Zero);
            buffer = new byte[reportVersion1.Length + 1];
            reportVersion1.Buffer.BlockCopy(0, buffer, 0, reportVersion1.Length);
            Packet bigReportVersion1 = new Packet(buffer, reportVersion1.Timestamp, reportVersion1.DataLink);
            Assert.IsTrue(bigReportVersion1.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(bigReportVersion1.IsValid);

            // Non zero max response code for report version 1
            buffer = new byte[reportVersion1.Length];
            reportVersion1.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + 1, 1);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + 2, (ushort)0xedfe, Endianity.Big);
            Packet nonZeroMaxResponseCodeReportVersion1 = new Packet(buffer, reportVersion1.Timestamp, reportVersion1.DataLink);
            Assert.IsTrue(nonZeroMaxResponseCodeReportVersion1.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(nonZeroMaxResponseCodeReportVersion1.IsValid);

            // Big report version 2
            Packet reportVersion2 = PacketBuilder.EthernetIpV4IgmpReportVersion2(DateTime.Now,
                                                                               new MacAddress(), new MacAddress(),
                                                                               0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                               IpV4Options.None,
                                                                               TimeSpan.FromSeconds(1), IpV4Address.Zero);
            buffer = new byte[reportVersion2.Length + 1];
            reportVersion2.Buffer.BlockCopy(0, buffer, 0, reportVersion2.Length);
            Packet bigReportVersion2 = new Packet(buffer, reportVersion2.Timestamp, reportVersion2.DataLink);
            Assert.IsTrue(bigReportVersion2.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(bigReportVersion2.IsValid);

            // non zero max response code report version 3
            Packet reportVersion3 = PacketBuilder.EthernetIpV4IgmpReportVersion3(DateTime.Now,
                                                                                 new MacAddress(), new MacAddress(),
                                                                                 0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                                 IpV4Options.None,
                                                                                 new[]
                                                                                     {
                                                                                         new IgmpGroupRecord(
                                                                                             IgmpRecordType.CurrentStateRecordModeIsExclude,
                                                                                             IpV4Address.Zero, new List<IpV4Address>(), Datagram.Empty)
                                                                                     });
            buffer = new byte[reportVersion3.Length];
            reportVersion3.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + 1, 1);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + 2, (ushort)0xdbfd, Endianity.Big);
            Packet nonZeroMaxResponseCodeReportVersion3 = new Packet(buffer, reportVersion3.Timestamp, reportVersion3.DataLink);
            Assert.IsTrue(nonZeroMaxResponseCodeReportVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(nonZeroMaxResponseCodeReportVersion3.IsValid);

            // big report version 3
            buffer = new byte[reportVersion3.Length + 1];
            reportVersion3.Buffer.BlockCopy(0, buffer, 0, reportVersion3.Length);
            Packet bigReportVersion3 = new Packet(buffer, reportVersion3.Timestamp, reportVersion3.DataLink);
            Assert.IsTrue(bigReportVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(bigReportVersion3.IsValid);

            // invalid group record report version 3
            buffer = new byte[reportVersion3.Length];
            reportVersion3.Buffer.BlockCopy(0, buffer, 0, reportVersion3.Length);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + IgmpDatagram.HeaderLength + 1, 1);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + 2, (ushort)0xdbfd, Endianity.Big);
            Packet invalidGroupRecordReportVersion3 = new Packet(buffer, reportVersion3.Timestamp, reportVersion3.DataLink);
            Assert.IsTrue(invalidGroupRecordReportVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(invalidGroupRecordReportVersion3.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IgmpIllegalReportVersionTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpReportVersion1(DateTime.Now,
                                                                         new MacAddress(), new MacAddress(),
                                                                         0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                         IpV4Options.None,
                                                                         IpV4Address.Zero);
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(1, packet.Ethernet.IpV4.Igmp.Version);

            byte[] buffer = new byte[packet.Length];
            packet.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            buffer.Write(EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength, 0);
            Packet illegalPacket = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(illegalPacket.IsValid);
            Assert.IsNull(illegalPacket.Ethernet.IpV4.Igmp.Version);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IgmpIllegalQueryVersionTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4IgmpQueryVersion1(DateTime.Now,
                                                                        new MacAddress(), new MacAddress(),
                                                                        0, 0, IpV4Fragmentation.None, 0, IpV4Address.Zero, IpV4Address.Zero,
                                                                        IpV4Options.None,
                                                                        IpV4Address.Zero);
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(1, packet.Ethernet.IpV4.Igmp.Version);

            byte[] buffer = new byte[packet.Length - 1];
            packet.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            Packet illegalPacket = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(illegalPacket.IsValid);
            Assert.IsNull(illegalPacket.Ethernet.IpV4.Igmp.Version);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IgmpGroupRecordBadAuxiliaryDataLengthTest()
        {
            IgmpGroupRecord record = new IgmpGroupRecord(IgmpRecordType.SourceListChangeAllowNewSources, IpV4Address.Zero, new List<IpV4Address>(),
                                                         new Datagram(new byte[] {1}));
            Assert.IsNotNull(record);
            Assert.Fail();
        }

        [TestMethod]
        public void IgmpGroupRecordTest()
        {
            IgmpGroupRecord record = new IgmpGroupRecord(IgmpRecordType.SourceListChangeAllowNewSources, IpV4Address.Zero, new List<IpV4Address>(),
                                                         Datagram.Empty);
            Assert.IsTrue(record.Equals((object)record));
            Assert.AreEqual(record.GetHashCode(), record.GetHashCode());
            Assert.IsFalse(record.Equals(null));
            Assert.AreNotEqual(record, new IgmpGroupRecord(IgmpRecordType.CurrentStateRecordModeIsExclude, record.MulticastAddress, record.SourceAddresses, record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, new IpV4Address("1.2.3.4"), record.SourceAddresses, record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, record.MulticastAddress, new List<IpV4Address>(new[] {new IpV4Address("2.3.4.5")}), record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, record.MulticastAddress, record.SourceAddresses, new Datagram(new byte[12])));
        }
    }
}