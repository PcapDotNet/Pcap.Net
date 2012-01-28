﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for DnsTests
    /// </summary>
    [TestClass]
    public class DnsTests
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
        public void RandomDnsTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
            {
                Source = new MacAddress("00:01:02:03:04:05"),
                Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
            };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            UdpLayer udpLayer = random.NextUdpLayer();
            udpLayer.Checksum = null;

            for (int i = 0; i != 1000; ++i)
            {
                DnsLayer dnsLayer;
                do
                {
                    dnsLayer = random.NextDnsLayer();
                } while (dnsLayer.Length > 65000);

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, dnsLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // DNS
                DnsLayer actualLayer = (DnsLayer)packet.Ethernet.IpV4.Udp.Dns.ExtractLayer();
                Assert.AreEqual(dnsLayer, actualLayer, "DNS Layer");
                Assert.IsTrue(packet.Ethernet.IpV4.Udp.Dns.IsValid);

                DnsDataResourceRecord opt = packet.Ethernet.IpV4.Udp.Dns.Additionals.FirstOrDefault(additional => additional.Type == DnsType.Opt);
                Assert.AreEqual(opt, packet.Ethernet.IpV4.Udp.Dns.OptionsRecord);

                foreach (var record in packet.Ethernet.IpV4.Udp.Dns.ResourceRecords)
                {
                    Assert.IsTrue(record.Equals(record));
                    Assert.IsTrue(record.DomainName.Equals((object)record.DomainName));
                    Assert.IsTrue(record.DomainName.Equals((object)record.DomainName));
                }

                foreach (var record in packet.Ethernet.IpV4.Udp.Dns.DataResourceRecords)
                {
                    MoreAssert.IsBiggerOrEqual(9, record.ToString().Length);
                    Assert.IsTrue(record.Equals((object)record));
                    Assert.IsInstanceOfType(record.Data, DnsResourceData.GetDnsResourceDataType(record.Type) ?? typeof(DnsResourceDataAnything));
                    Assert.IsTrue(record.DomainName.Equals((object)record.DomainName));
                }
            }
        }

        [TestMethod]
        public void DnsDomainNameCompressionTest()
        {
            DnsLayer dnsLayer = new DnsLayer();
            TestDomainNameCompression(0, dnsLayer);
            
            dnsLayer.Queries = new List<DnsQueryResourceRecord>();
            dnsLayer.Answers = new List<DnsDataResourceRecord>();
            dnsLayer.Authorities = new List<DnsDataResourceRecord>();
            dnsLayer.Additionals = new List<DnsDataResourceRecord>();
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Queries.Add(new DnsQueryResourceRecord(new DnsDomainName(""), DnsType.Any, DnsClass.In));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName(""), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(3, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("def.abc"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(6, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(6, dnsLayer);

            dnsLayer.Authorities.Add(new DnsDataResourceRecord(new DnsDomainName("abc.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(13, dnsLayer);

            dnsLayer.Authorities.Add(new DnsDataResourceRecord(new DnsDomainName("abd.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(16, dnsLayer);

            dnsLayer.Additionals.Add(new DnsDataResourceRecord(new DnsDomainName("hello.abd.def"), DnsType.Any, DnsClass.In, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(23, dnsLayer);
        }

        [TestMethod]
        public void DnsDomainNameCompressionTooLongTest()
        {
            DnsLayer dnsLayer = new DnsLayer();
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Queries = new List<DnsQueryResourceRecord>();
            dnsLayer.Answers = new List<DnsDataResourceRecord>();
            dnsLayer.Authorities = new List<DnsDataResourceRecord>();
            dnsLayer.Additionals = new List<DnsDataResourceRecord>();
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("aaa"), DnsType.Null, DnsClass.In, 100, new DnsResourceDataAnything(new DataSegment(new byte[20000]))));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("bbb.aaa"), DnsType.Null, DnsClass.In, 100, new DnsResourceDataAnything(new DataSegment(new byte[1]))));
            TestDomainNameCompression(3, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("bbb.aaa"), DnsType.Null, DnsClass.In, 100, new DnsResourceDataAnything(new DataSegment(new byte[1]))));
            TestDomainNameCompression(6, dnsLayer);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DnsCompressionInvalidModeTest()
        {
            DnsLayer dnsLayer = new DnsLayer
                                {
                                    DomainNameCompressionMode = (DnsDomainNameCompressionMode)int.MaxValue,
                                    Answers =
                                        new List<DnsDataResourceRecord>(new[]
                                                                        {
                                                                            new DnsDataResourceRecord(new DnsDomainName("a"), DnsType.A, DnsClass.In, 10,
                                                                                                      new DnsResourceDataIpV4(IpV4Address.Zero))
                                                                        }),
                                };
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                dnsLayer);
            Assert.IsNull(packet);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsOptResourceRecordTest()
        {
            Random random = new Random();
            for (int i = 0; i != 100; ++i)
            {
                DnsDomainName domainName = random.NextDnsDomainName();
                ushort sendersUdpPayloadSize = random.NextUShort();
                byte extendedRcode = random.NextByte();
                DnsOptVersion version = (DnsOptVersion)random.NextByte();
                DnsOptFlags flags = (DnsOptFlags)random.NextUShort();
                DnsResourceDataOptions data = (DnsResourceDataOptions)random.NextDnsResourceData(DnsType.Opt);

                DnsOptResourceRecord record = new DnsOptResourceRecord(domainName, sendersUdpPayloadSize, extendedRcode, version, flags, data);
                
                Assert.AreEqual(domainName, record.DomainName);
                Assert.AreEqual(sendersUdpPayloadSize, record.SendersUdpPayloadSize);
                Assert.AreEqual(extendedRcode, record.ExtendedRcode);
                Assert.AreEqual(version, record.Version);
                Assert.AreEqual(flags, record.Flags);
                Assert.AreEqual(data, record.Data);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DnsQueryResourceRecordTtlGetTest()
        {
            var query = new DnsQueryResourceRecord(DnsDomainName.Root, DnsType.A, DnsClass.In);
            Assert.IsNull(query.Ttl);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DnsQueryResourceRecordDataGetTest()
        {
            var query = new DnsQueryResourceRecord(DnsDomainName.Root, DnsType.A, DnsClass.In);
            Assert.IsNull(query.Data);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNextDomainTest()
        {
            DataSegment bitMap = DnsResourceDataNextDomain.CreateTypeBitMap(new[] {DnsType.A, DnsType.Aaaa});
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitMap);
            Assert.IsFalse(resourceData.Equals(null));
            Assert.IsTrue(resourceData.IsTypePresentForOwner(DnsType.A));
            Assert.IsTrue(resourceData.IsTypePresentForOwner(DnsType.Aaaa));
            Assert.IsFalse(resourceData.IsTypePresentForOwner(DnsType.Ns));

            bitMap = DnsResourceDataNextDomain.CreateTypeBitMap(new DnsType[] { 0 });
            Assert.AreEqual(DataSegment.Empty, bitMap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainTooBigDnsType()
        {
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), DataSegment.Empty);
            Assert.IsNull(resourceData.IsTypePresentForOwner((DnsType)(8 * 16 + 1)));
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainTooLongBitmapTest()
        {
            DataSegment bitMap = new DataSegment(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17});
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitMap);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainZeroEndedBitmapTest()
        {
            DataSegment bitMap = new DataSegment(new byte[] { 1, 0 });
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitMap);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNamingAuthorityPointerTest()
        {
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NaPtr,
                                                        new DnsResourceDataNamingAuthorityPointer(0, 0, DataSegment.Empty, DataSegment.Empty,
                                                                                                  DataSegment.Empty, DnsDomainName.Root),
                                                        -1);

            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NaPtr,
                                                        new DnsResourceDataNamingAuthorityPointer(0, 0, new DataSegment(Encoding.ASCII.GetBytes("abcd")),
                                                                                                  DataSegment.Empty,
                                                                                                  DataSegment.Empty, DnsDomainName.Root),
                                                        -4);

            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NaPtr,
                                                        new DnsResourceDataNamingAuthorityPointer(0, 0, DataSegment.Empty,
                                                                                                  new DataSegment(Encoding.ASCII.GetBytes("abc")),
                                                                                                  DataSegment.Empty, DnsDomainName.Root),
                                                        -3);

            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NaPtr,
                                                        new DnsResourceDataNamingAuthorityPointer(0, 0, DataSegment.Empty, DataSegment.Empty,
                                                                                                  new DataSegment(Encoding.ASCII.GetBytes("ab")),
                                                                                                  DnsDomainName.Root),
                                                        -2);

            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NaPtr,
                                                        new DnsResourceDataNamingAuthorityPointer(0, 0, DataSegment.Empty, DataSegment.Empty,
                                                                                                  DataSegment.Empty, new DnsDomainName("a")),
                                                        -1);

            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NaPtr,
                                                        new DnsResourceDataNamingAuthorityPointer(0, 0, DataSegment.Empty, DataSegment.Empty,
                                                                                                  DataSegment.Empty, DnsDomainName.Root),
                                                        1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DnsResourceDataNamingAuthorityPointerIllegalFlagsTest()
        {
            var resourceData = new DnsResourceDataNamingAuthorityPointer(0, 0, new DataSegment(new[] {(byte)'%'}),
                                                                         DataSegment.Empty, DataSegment.Empty,
                                                                         DnsDomainName.Root);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataTransactionKeyTooBigKeyTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.KeyDeletion, DnsResponseCode.NoError,
                                                                 new DataSegment(new byte[ushort.MaxValue + 1]), DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataTransactionKeyTooBigOtherTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.KeyDeletion, DnsResponseCode.NoError,
                                                                 DataSegment.Empty, new DataSegment(new byte[ushort.MaxValue + 1]));
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataTransactionSignatureTooBigMessageAuthenticationCodeTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(DnsDomainName.Root, 0, 0, new DataSegment(new byte[ushort.MaxValue + 1]), 0,
                                                                       DnsResponseCode.NoError, DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataTransactionSignatureTooBigOtherTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(DnsDomainName.Root, 0, 0, DataSegment.Empty, 0,
                                                                       DnsResponseCode.NoError, new DataSegment(new byte[ushort.MaxValue + 1]));
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataHostIdentityProtocolTooBigHostIdentityTagTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(new DataSegment(new byte[byte.MaxValue + 1]), DnsPublicKeyAlgorithm.None,
                                                                       DataSegment.Empty, new DnsDomainName[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataHostIdentityProtocolTooBigPublicKeyTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(DataSegment.Empty, DnsPublicKeyAlgorithm.None,
                                                                       new DataSegment(new byte[ushort.MaxValue + 1]), new DnsDomainName[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataLocationInformationInvalidSizeTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 9000000001L, 0, 0, 0, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataLocationInformationInvalidHorizontalPrecisionTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 0, 9000000001L, 0, 0, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataLocationInformationInvalidVerticalPrecisionTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 0, 0, 9000000001L, 0, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNextDomainSecureTest()
        {
            var types = new[] {DnsType.A, DnsType.Aaaa, DnsType.A6, DnsType.Any, DnsType.NaPtr};
            var resourceData = new DnsResourceDataNextDomainSecure(DnsDomainName.Root, types);
            foreach (var type in Enum.GetValues(typeof(DnsType)))
            {
                Assert.AreEqual(types.Contains((DnsType)type), resourceData.IsTypePresentForOwner((DnsType)type));
            }

            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec, new DnsResourceDataNextDomainSecure(DnsDomainName.Root, new DnsType[0]), -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec, new DnsResourceDataNextDomainSecure(DnsDomainName.Root, new DnsType[0]), 9000);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec, new DnsResourceDataNextDomainSecure(DnsDomainName.Root, new[] {DnsType.A}), -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec, new DnsResourceDataNextDomainSecure(DnsDomainName.Root, new[] {DnsType.A, DnsType.Any}),
                                                        -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNetworkServiceAccessPointAreaAddressTooSmallTest()
        {
            var resourceData = new DnsResourceDataNetworkServiceAccessPoint(DataSegment.Empty, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNetworkServiceAccessPointTest()
        {
            var resourceData = new DnsResourceDataNetworkServiceAccessPoint(new DataSegment(new byte[]{1,2,3,4,5}), 0, 0);
            Assert.AreEqual(1, resourceData.AuthorityAndFormatIdentifier);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsAddressPrefixAddressFamilyDependentPartTooBigTest()
        {
            var dnsAddressPrefix = new DnsAddressPrefix(AddressFamily.IpV4, 0, false, new DataSegment(new byte[128]));
            Assert.IsNull(dnsAddressPrefix);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsAddressPrefixAddressFamilyDependentPartTest()
        {
            var dnsAddressPrefix = new DnsAddressPrefix(AddressFamily.IpV4, 0, false, new DataSegment(new byte[127]));
            Assert.IsTrue(dnsAddressPrefix.Equals((object)dnsAddressPrefix));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainSecure3NextHashedOwnerNameTooBigTest()
        {
            var resourceData = new DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0, DataSegment.Empty,
                                                                    new DataSegment(new byte[byte.MaxValue + 1]), new DnsType[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataNextDomainSecure3SaltTooBigTest()
        {
            var resourceData = new DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0,
                                                                    new DataSegment(new byte[byte.MaxValue + 1]), DataSegment.Empty, new DnsType[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsGatewayTest()
        {
            DnsGateway gateway = new DnsGatewayIpV6(IpV6Address.Zero);
            Assert.IsTrue(gateway.Equals((object)gateway));
            Assert.IsFalse(gateway.Equals(null as object));
        }

        [TestMethod]
        public void DnsOptionTest()
        {
            DnsOption option = new DnsOptionAnything(DnsOptionCode.UpdateLease, DataSegment.Empty);
            Assert.IsTrue(option.Equals((object)option));
            Assert.IsFalse(option.Equals(null as object));
        }

        [TestMethod]
        public void DnsOptionsTest()
        {
            DnsOptions options = new DnsOptions();
            Assert.IsTrue(options.Equals((object)options));
            Assert.IsFalse(options.Equals(null as object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DnsResourceDataCertificationAuthorityAuthorizationTagTooBigTest()
        {
            var resourceData = new DnsResourceDataCertificationAuthorityAuthorization(DnsCertificationAuthorityAuthorizationFlags.Critical,
                                                                                      new DataSegment(new byte[byte.MaxValue + 1]), DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        private static void TestDomainNameCompression(int expectedCompressionBenefit, DnsLayer dnsLayer)
        {
            dnsLayer.DomainNameCompressionMode = DnsDomainNameCompressionMode.Nothing;
            Packet uncompressedPacket = PacketBuilder.Build(DateTime.Now,
                                                            new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                            dnsLayer);
            Assert.IsTrue(uncompressedPacket.IsValid, "IsValid");
            ILayer uncompressedPacketLayer = uncompressedPacket.Ethernet.IpV4.Udp.Dns.ExtractLayer();

            dnsLayer.DomainNameCompressionMode = DnsDomainNameCompressionMode.All;
            Packet compressedPacket = PacketBuilder.Build(DateTime.Now,
                                                            new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                            dnsLayer);
            Assert.IsTrue(compressedPacket.IsValid, "IsValid");
            ILayer compressedPacketLayer = compressedPacket.Ethernet.IpV4.Udp.Dns.ExtractLayer();

            Assert.AreEqual(dnsLayer, uncompressedPacketLayer);
            Assert.AreEqual(dnsLayer, compressedPacketLayer);
            Assert.AreEqual(compressedPacketLayer, uncompressedPacketLayer);

            Assert.AreEqual(uncompressedPacket.Length, compressedPacket.Length + expectedCompressionBenefit, expectedCompressionBenefit.ToString());
        }

        private static void TestResourceRecordIsNotCreatedWithNewLength(DnsType dnsType, DnsResourceData resourceData, int dataLengthDiff)
        {
            var resourceRecord = new DnsDataResourceRecord(DnsDomainName.Root, dnsType, DnsClass.In, 0, resourceData);
            var paddingResourceRecord = new DnsDataResourceRecord(DnsDomainName.Root, DnsType.Null, DnsClass.In, 0,
                                                                  new DnsResourceDataAnything(new DataSegment(new byte[100 + Math.Abs(dataLengthDiff)])));
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                new DnsLayer
                                                {
                                                    Answers = new List<DnsDataResourceRecord>(new[]
                                                                                              {
                                                                                                  resourceRecord,
                                                                                                  paddingResourceRecord
                                                                                              }),
                                                });

            Assert.AreEqual(2, packet.Ethernet.IpV4.Udp.Dns.Answers.Count);
            Assert.AreEqual(resourceRecord, packet.Ethernet.IpV4.Udp.Dns.Answers[0]);
            Assert.AreEqual(paddingResourceRecord, packet.Ethernet.IpV4.Udp.Dns.Answers[1]);

            byte[] buffer = new byte[packet.Length];
            buffer.Write(0, packet.Ethernet);
            const int dataLengthOffset =
                EthernetDatagram.HeaderLength + IpV4Datagram.HeaderMinimumLength + UdpDatagram.HeaderLength + DnsDatagram.HeaderLength + 5 + 4;
            ushort oldDataLength = buffer.ReadUShort(dataLengthOffset, Endianity.Big);
            ushort newDataLength = (ushort)(oldDataLength + dataLengthDiff);
            buffer.Write(dataLengthOffset, newDataLength, Endianity.Big);
            packet = new Packet(buffer, DateTime.Now, DataLinkKind.Ethernet);

            Assert.IsFalse(packet.Ethernet.IpV4.Udp.Dns.Answers.Any());
        }
    }
}