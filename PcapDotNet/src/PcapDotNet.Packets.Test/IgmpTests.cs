using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IgmpTests
    /// </summary>
    [TestClass]
    public class IgmpTests
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

        [TestMethod]
        public void RandomIgmpTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            int seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            Random random = new Random(seed);

            for (int i = 0; i != 200; ++i)
            {
                IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
                ipV4Layer.HeaderChecksum = null;
                Layer ipLayer = random.NextBool() ? (Layer)ipV4Layer : random.NextIpV6Layer(IpV4Protocol.InternetGroupManagementProtocol, false);

                IgmpLayer igmpLayer = random.NextIgmpLayer();

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipLayer, igmpLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // Ethernet
                ethernetLayer.EtherType = ipLayer == ipV4Layer ? EthernetType.IpV4 : EthernetType.IpV6;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");
                ethernetLayer.EtherType = EthernetType.None;

                // IP.
                if (ipV4Layer == ipLayer)
                {
                    // IPv4.
                    ipV4Layer.Protocol = IpV4Protocol.InternetGroupManagementProtocol;
                    ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                    Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer(), "IPv4 Layer");
                    ipV4Layer.HeaderChecksum = null;
                }
                else
                {
                    // IPv6.
                    Assert.AreEqual(ipLayer, packet.Ethernet.IpV6.ExtractLayer(), "IPv6 Layer");
                }

                // IGMP
                Assert.IsTrue(packet.Ethernet.Ip.Igmp.IsChecksumCorrect);
                Assert.AreEqual(igmpLayer, packet.Ethernet.Ip.Igmp.ExtractLayer(), "IGMP Layer");
                Assert.AreEqual(igmpLayer.GetHashCode(), packet.Ethernet.Ip.Igmp.ExtractLayer().GetHashCode(), "IGMP Layer");
                Assert.AreNotEqual(igmpLayer, null);
                Assert.AreNotEqual(igmpLayer, random.NextPayloadLayer(igmpLayer.Length));
                Assert.AreNotEqual(igmpLayer.GetHashCode(), random.NextPayloadLayer(igmpLayer.Length).GetHashCode());
                if (packet.Ethernet.Ip.Igmp.QueryVersion != IgmpQueryVersion.Version3)
                    MoreAssert.IsSmallerOrEqual(IgmpDatagram.MaxMaxResponseTime, packet.Ethernet.Ip.Igmp.MaxResponseTime);
                if (packet.Ethernet.Ip.Igmp.MessageType != IgmpMessageType.MembershipQuery)
                    Assert.AreEqual(IgmpQueryVersion.None, packet.Ethernet.Ip.Igmp.QueryVersion);
                switch (igmpLayer.MessageType)
                {
                    case IgmpMessageType.MembershipQuery:
                        switch (igmpLayer.QueryVersion)
                        {
                            case IgmpQueryVersion.Version1:
                                Assert.AreEqual(1, packet.Ethernet.Ip.Igmp.Version);
                                break;

                            case IgmpQueryVersion.Version2:
                                Assert.AreEqual(2, packet.Ethernet.Ip.Igmp.Version);
                                break;

                            case IgmpQueryVersion.Version3:
                                Assert.AreEqual(3, packet.Ethernet.Ip.Igmp.Version);
                                break;

                            default:
                                Assert.Fail(igmpLayer.QueryVersion.ToString());
                                break;
                        }
                        break;

                    case IgmpMessageType.MembershipReportVersion1:
                        Assert.AreEqual(1, packet.Ethernet.Ip.Igmp.Version);
                        break;

                    case IgmpMessageType.MembershipReportVersion2:
                    case IgmpMessageType.LeaveGroupVersion2:
                        Assert.AreEqual(2, packet.Ethernet.Ip.Igmp.Version);
                        break;

                    case IgmpMessageType.MembershipReportVersion3:
                        Assert.AreEqual(3, packet.Ethernet.Ip.Igmp.Version);
                        break;

                    default:
                        Assert.Fail(igmpLayer.MessageType.ToString());
                        break;
                }
                foreach (IgmpGroupRecordDatagram groupRecord in packet.Ethernet.Ip.Igmp.GroupRecords)
                    Assert.IsNotNull(groupRecord.ToString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpQueryVersion3SmallMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                 new IgmpQueryVersion3Layer
                                                 {
                                                     MaxResponseTime = TimeSpan.FromSeconds(-1),
                                                     QueryInterval = TimeSpan.FromSeconds(1)
                                                 });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpQueryVersion3BigMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                 new IgmpQueryVersion3Layer
                                                     {
                                                         MaxResponseTime = TimeSpan.FromHours(1),
                                                         QueryInterval = TimeSpan.FromSeconds(1)
                                                     });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpQueryVersion3SmallQueryIntervalTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                 new IgmpQueryVersion3Layer
                                                 {
                                                     MaxResponseTime = TimeSpan.FromSeconds(1),
                                                     QueryInterval = TimeSpan.FromSeconds(-1)
                                                 });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpQueryVersion3BigQueryIntervalTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                     new IgmpQueryVersion3Layer
                                     {
                                         MaxResponseTime = TimeSpan.FromSeconds(1),
                                         QueryInterval = TimeSpan.FromHours(9)
                                     });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpQueryVersion2SmallMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                 new EthernetLayer(), new IpV4Layer(),
                                                 new IgmpQueryVersion2Layer
                                                     {
                                                         MaxResponseTime = TimeSpan.FromSeconds(-1)
                                                     });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpQueryVersion2BigMaxResponseTimeTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                 new IgmpQueryVersion2Layer
                                                     {
                                                         MaxResponseTime = TimeSpan.FromMinutes(5)
                                                     });

            Assert.IsTrue(packet.IsValid);
            Assert.Fail();
        }

        [TestMethod]
        public void IgmpInvalidTest()
        {
            Packet queryVersion2 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                        new IgmpQueryVersion2Layer
                                                            {
                                                                MaxResponseTime = TimeSpan.FromSeconds(1),
                                                            });

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
            Packet queryVersion3 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                        new IgmpQueryVersion3Layer
                                                            {
                                                                MaxResponseTime = TimeSpan.FromSeconds(1),
                                                                QueryInterval = TimeSpan.FromSeconds(1),
                                                            });
            Assert.IsTrue(queryVersion3.IsValid, "IsValid");
            buffer = new byte[queryVersion3.Length + 2];
            queryVersion3.Buffer.BlockCopy(0, buffer, 0, queryVersion3.Length);
            buffer[EthernetDatagram.HeaderLengthValue + 3] += 2;
            buffer[EthernetDatagram.HeaderLengthValue + 11] -= 2;
            Packet bigQueryVersion3 = new Packet(buffer, queryVersion3.Timestamp, queryVersion3.DataLink);
            Assert.IsTrue(bigQueryVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(bigQueryVersion3.Ethernet.IpV4.IsHeaderChecksumCorrect, "IpV4.IsHeaderChecksumCorrect");
            Assert.IsFalse(bigQueryVersion3.IsValid, "bigQueryVersion3.IsValid");

            // Big report version 1
            Packet reportVersion1 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpReportVersion1Layer());

            buffer = new byte[reportVersion1.Length + 2];
            reportVersion1.Buffer.BlockCopy(0, buffer, 0, reportVersion1.Length);
            buffer[EthernetDatagram.HeaderLengthValue + 3] += 2;
            buffer[EthernetDatagram.HeaderLengthValue + 11] -= 2;
            Packet bigReportVersion1 = new Packet(buffer, reportVersion1.Timestamp, reportVersion1.DataLink);
            Assert.IsTrue(bigReportVersion1.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(bigReportVersion1.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.IsFalse(bigReportVersion1.IsValid);

            // Non zero max response code for report version 1
            buffer = new byte[reportVersion1.Length];
            reportVersion1.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + 1, 1);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + 2, (ushort)0xedfe, Endianity.Big);
            Packet nonZeroMaxResponseCodeReportVersion1 = new Packet(buffer, reportVersion1.Timestamp, reportVersion1.DataLink);
            Assert.IsTrue(nonZeroMaxResponseCodeReportVersion1.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(nonZeroMaxResponseCodeReportVersion1.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.IsFalse(nonZeroMaxResponseCodeReportVersion1.IsValid);

            // Big report version 2
            Packet reportVersion2 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                         new IgmpReportVersion2Layer
                                                             {
                                                                 MaxResponseTime = TimeSpan.FromSeconds(1)
                                                             });

            buffer = new byte[reportVersion2.Length + 2];
            reportVersion2.Buffer.BlockCopy(0, buffer, 0, reportVersion2.Length);
            buffer[EthernetDatagram.HeaderLengthValue + 3] += 2;
            buffer[EthernetDatagram.HeaderLengthValue + 11] -= 2;
            Packet bigReportVersion2 = new Packet(buffer, reportVersion2.Timestamp, reportVersion2.DataLink);
            Assert.IsTrue(bigReportVersion2.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(bigReportVersion2.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.IsFalse(bigReportVersion2.IsValid);

            // non zero max response code report version 3
            Packet reportVersion3 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                        new IgmpReportVersion3Layer
                                                        {
                                                            GroupRecords = new[]
                                                                           {
                                                                               new IgmpGroupRecord(
                                                                                   IgmpRecordType.CurrentStateRecordModeIsExclude,
                                                                                   IpV4Address.Zero, new List<IpV4Address>(), Datagram.Empty)
                                                                           }.AsReadOnly()
                                                        });
            
            buffer = new byte[reportVersion3.Length];
            reportVersion3.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + 1, 1);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + 2, (ushort)0xdbfd, Endianity.Big);
            Packet nonZeroMaxResponseCodeReportVersion3 = new Packet(buffer, reportVersion3.Timestamp, reportVersion3.DataLink);
            Assert.IsTrue(nonZeroMaxResponseCodeReportVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(nonZeroMaxResponseCodeReportVersion3.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.IsFalse(nonZeroMaxResponseCodeReportVersion3.IsValid);

            // big report version 3
            buffer = new byte[reportVersion3.Length + 2];
            reportVersion3.Buffer.BlockCopy(0, buffer, 0, reportVersion3.Length);
            buffer[EthernetDatagram.HeaderLengthValue + 3] += 2;
            buffer[EthernetDatagram.HeaderLengthValue + 11] -= 2;
            Packet bigReportVersion3 = new Packet(buffer, reportVersion3.Timestamp, reportVersion3.DataLink);
            Assert.IsTrue(bigReportVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(bigReportVersion3.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.IsFalse(bigReportVersion3.IsValid);

            // invalid group record report version 3
            buffer = new byte[reportVersion3.Length];
            reportVersion3.Buffer.BlockCopy(0, buffer, 0, reportVersion3.Length);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + IgmpDatagram.HeaderLength + 1, 1);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + 2, (ushort)0xdbfd, Endianity.Big);
            Packet invalidGroupRecordReportVersion3 = new Packet(buffer, reportVersion3.Timestamp, reportVersion3.DataLink);
            Assert.IsTrue(invalidGroupRecordReportVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsTrue(invalidGroupRecordReportVersion3.Ethernet.IpV4.IsHeaderChecksumCorrect);
            Assert.IsFalse(invalidGroupRecordReportVersion3.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void IgmpIllegalReportVersionTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpReportVersion1Layer());

            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(1, packet.Ethernet.IpV4.Igmp.Version);

            byte[] buffer = new byte[packet.Length];
            packet.Buffer.BlockCopy(0, buffer, 0, buffer.Length);
            buffer.Write(EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength, 0);
            Packet illegalPacket = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(illegalPacket.IsValid);
            Assert.IsNull(illegalPacket.Ethernet.IpV4.Igmp.Version);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void IgmpIllegalQueryVersionTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpQueryVersion1Layer());

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
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
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
            Assert.AreEqual(record.ToString(), record.ToString());
            Assert.IsFalse(record.Equals(null));
            Assert.AreNotEqual(record, new IgmpGroupRecord(IgmpRecordType.CurrentStateRecordModeIsExclude, record.MulticastAddress, record.SourceAddresses, record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, new IpV4Address("1.2.3.4"), record.SourceAddresses, record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, record.MulticastAddress, new List<IpV4Address>(new[] {new IpV4Address("2.3.4.5")}), record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, record.MulticastAddress, record.SourceAddresses, new Datagram(new byte[12])));
            Assert.AreNotEqual(record.ToString(), new IgmpGroupRecord(record.RecordType, record.MulticastAddress, record.SourceAddresses, new Datagram(new byte[12])).ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void IgmpExtractLayerBadMessageTypeTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpReportVersion1Layer());
            Assert.IsNotNull(packet.Ethernet.IpV4.Igmp.ExtractLayer());
            byte[] buffer = (byte[])packet.Buffer.Clone();
            buffer[packet.Length - packet.Ethernet.IpV4.Igmp.Length] = 0xFF;
            packet = new Packet(buffer, DateTime.Now, packet.DataLink);
            Assert.IsFalse(packet.IsValid);
            Assert.IsNotNull(packet.Ethernet.IpV4.Igmp.ExtractLayer());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IgmpTooBigQueryRobustnessVariableTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpQueryVersion3Layer
                                                                                                        {
                                                                                                            QueryRobustnessVariable = 255
                                                                                                        });
            Assert.IsNull(packet);
        }

        [TestMethod]
        public void DifferentIgmpSimpleLayersTest()
        {
            IgmpSimpleLayer layer1 = new IgmpQueryVersion1Layer
                                         {
                                             GroupAddress = new IpV4Address("1.2.3.4")
                                         };
            IgmpSimpleLayer layer2 = new IgmpQueryVersion2Layer
                                         {
                                             GroupAddress = new IpV4Address("1.2.3.4"),
                                             MaxResponseTime = TimeSpan.FromMinutes(55)
                                         };
            Assert.IsFalse(layer1.Equals(layer2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void IgmpGroupRecordConstructorNullTest()
        {
            Assert.IsNotNull(new IgmpGroupRecord(IgmpRecordType.FilterModeChangeToExclude, IpV4Address.Zero, new IpV4Address[0], null));
            Assert.Fail();
        }
    }
}