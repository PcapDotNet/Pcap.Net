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
        public void RandomIgmpTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            for (int i = 0; i != 1000; ++i)
            {
                IgmpLayer igmpLayer = random.NextIgmpLayer();

                Packet packet = new PacketBuilder(ethernetLayer, ipV4Layer, igmpLayer).Build(DateTime.Now);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // Ethernet
                ethernetLayer.EtherType = EthernetType.IpV4;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IPv4
                ipV4Layer.Protocol = IpV4Protocol.InternetGroupManagementProtocol;
                ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer(), "IPv4 Layer");
                ipV4Layer.HeaderChecksum = null;

                // IGMP
                Assert.IsTrue(packet.Ethernet.IpV4.Igmp.IsChecksumCorrect);
                Assert.AreEqual(igmpLayer, packet.Ethernet.IpV4.Igmp.ExtractLayer(), "IGMP Layer");
                Assert.AreEqual(igmpLayer.GetHashCode(), packet.Ethernet.IpV4.Igmp.ExtractLayer().GetHashCode(), "IGMP Layer");
                Assert.AreNotEqual(igmpLayer, null);
                Assert.AreNotEqual(igmpLayer, random.NextPayloadLayer(igmpLayer.Length));
                Assert.AreNotEqual(igmpLayer.GetHashCode(), random.NextPayloadLayer(igmpLayer.Length).GetHashCode());
                if (packet.Ethernet.IpV4.Igmp.QueryVersion != IgmpQueryVersion.Version3)
                    MoreAssert.IsSmallerOrEqual(IgmpDatagram.MaxMaxResponseTime, packet.Ethernet.IpV4.Igmp.MaxResponseTime);
                if (packet.Ethernet.IpV4.Igmp.MessageType != IgmpMessageType.MembershipQuery)
                    Assert.AreEqual(IgmpQueryVersion.None, packet.Ethernet.IpV4.Igmp.QueryVersion);
                foreach (IgmpGroupRecordDatagram groupRecord in packet.Ethernet.IpV4.Igmp.GroupRecords)
                    Assert.IsNotNull(groupRecord.ToString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
            buffer = new byte[queryVersion3.Length + 1];
            queryVersion3.Buffer.BlockCopy(0, buffer, 0, queryVersion3.Length);
            Packet bigQueryVersion3 = new Packet(buffer, queryVersion3.Timestamp, queryVersion3.DataLink);
            Assert.IsTrue(bigQueryVersion3.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(bigQueryVersion3.IsValid);

            // Big report version 1
            Packet reportVersion1 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpReportVersion1Layer());

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
            Packet reportVersion2 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                         new IgmpReportVersion2Layer
                                                             {
                                                                 MaxResponseTime = TimeSpan.FromSeconds(1)
                                                             });

            buffer = new byte[reportVersion2.Length + 1];
            reportVersion2.Buffer.BlockCopy(0, buffer, 0, reportVersion2.Length);
            Packet bigReportVersion2 = new Packet(buffer, reportVersion2.Timestamp, reportVersion2.DataLink);
            Assert.IsTrue(bigReportVersion2.Ethernet.IpV4.Igmp.IsChecksumCorrect);
            Assert.IsFalse(bigReportVersion2.IsValid);

            // non zero max response code report version 3
            Packet reportVersion3 = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(),
                                                        new IgmpReportVersion3Layer(
                                                            new[]
                                                                {
                                                                    new IgmpGroupRecord(
                                                                        IgmpRecordType.CurrentStateRecordModeIsExclude,
                                                                        IpV4Address.Zero, new List<IpV4Address>(), Datagram.Empty)
                                                                }
                                                            ));
            
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
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new IgmpReportVersion1Layer());

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
            Assert.AreEqual(record.ToString(), record.ToString());
            Assert.IsFalse(record.Equals(null));
            Assert.AreNotEqual(record, new IgmpGroupRecord(IgmpRecordType.CurrentStateRecordModeIsExclude, record.MulticastAddress, record.SourceAddresses, record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, new IpV4Address("1.2.3.4"), record.SourceAddresses, record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, record.MulticastAddress, new List<IpV4Address>(new[] {new IpV4Address("2.3.4.5")}), record.AuxiliaryData));
            Assert.AreNotEqual(record, new IgmpGroupRecord(record.RecordType, record.MulticastAddress, record.SourceAddresses, new Datagram(new byte[12])));
            Assert.AreNotEqual(record.ToString(), new IgmpGroupRecord(record.RecordType, record.MulticastAddress, record.SourceAddresses, new Datagram(new byte[12])).ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
    }
}