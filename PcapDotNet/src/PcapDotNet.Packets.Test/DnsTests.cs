using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            int seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            Random random = new Random(seed);

            for (int i = 0; i != 1000; ++i)
            {
                IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
                ipV4Layer.HeaderChecksum = null;
                Layer ipLayer = random.NextBool() ? (Layer)ipV4Layer : random.NextIpV6Layer(true);

                UdpLayer udpLayer = random.NextUdpLayer();
                udpLayer.Checksum = null;

                DnsLayer dnsLayer;
                do
                {
                    dnsLayer = random.NextDnsLayer();
                } while (dnsLayer.Length > 65000 - ipLayer.Length);

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipLayer, udpLayer, dnsLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");

                // DNS
                DnsLayer actualLayer = (DnsLayer)packet.Ethernet.Ip.Udp.Dns.ExtractLayer();
                Assert.AreEqual(dnsLayer, actualLayer, "DNS Layer");
                Assert.IsTrue(packet.Ethernet.Ip.Udp.Dns.IsValid);

                DnsDataResourceRecord opt = packet.Ethernet.Ip.Udp.Dns.Additionals.FirstOrDefault(additional => additional.DnsType == DnsType.Opt);
                Assert.AreEqual(opt, packet.Ethernet.Ip.Udp.Dns.OptionsRecord);

                foreach (var record in packet.Ethernet.Ip.Udp.Dns.ResourceRecords)
                {
                    Assert.IsTrue(record.Equals(record));
                    Assert.IsTrue(record.DomainName.Equals((object)record.DomainName));
                    Assert.IsTrue(record.DomainName.Equals((object)record.DomainName));
                    Assert.AreEqual(record.GetHashCode(), record.GetHashCode());
                }

                foreach (var record in packet.Ethernet.Ip.Udp.Dns.DataResourceRecords)
                {
                    MoreAssert.IsBiggerOrEqual(9, record.ToString().Length);
                    Assert.IsTrue(record.Equals((object)record));
                    Assert.IsInstanceOfType(record.Data, DnsResourceData.GetDnsResourceDataType(record.DnsType) ?? typeof(DnsResourceDataAnything));
                    Assert.IsTrue(record.DomainName.Equals((object)record.DomainName));
                    Assert.IsFalse(record.Data.Equals(null));
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

            dnsLayer.Queries.Add(new DnsQueryResourceRecord(new DnsDomainName(""), DnsType.Any, DnsClass.Internet));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName(""), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(3, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("def.abc"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(6, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("abc.def"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(6, dnsLayer);

            dnsLayer.Authorities.Add(new DnsDataResourceRecord(new DnsDomainName("abc.def"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(13, dnsLayer);

            dnsLayer.Authorities.Add(new DnsDataResourceRecord(new DnsDomainName("abd.def"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
            TestDomainNameCompression(16, dnsLayer);

            dnsLayer.Additionals.Add(new DnsDataResourceRecord(new DnsDomainName("hello.abd.def"), DnsType.Any, DnsClass.Internet, 100, new DnsResourceDataAnything(DataSegment.Empty)));
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

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("aaa"), DnsType.Null, DnsClass.Internet, 100, new DnsResourceDataAnything(new DataSegment(new byte[20000]))));
            TestDomainNameCompression(0, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("bbb.aaa"), DnsType.Null, DnsClass.Internet, 100, new DnsResourceDataAnything(new DataSegment(new byte[1]))));
            TestDomainNameCompression(3, dnsLayer);

            dnsLayer.Answers.Add(new DnsDataResourceRecord(new DnsDomainName("bbb.aaa"), DnsType.Null, DnsClass.Internet, 100, new DnsResourceDataAnything(new DataSegment(new byte[1]))));
            TestDomainNameCompression(6, dnsLayer);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void DnsCompressionInvalidModeTest()
        {
            DnsLayer dnsLayer = new DnsLayer
                                {
                                    DomainNameCompressionMode = (DnsDomainNameCompressionMode)int.MaxValue,
                                    Answers =
                                        new List<DnsDataResourceRecord>(new[]
                                                                        {
                                                                            new DnsDataResourceRecord(new DnsDomainName("a"), DnsType.A, DnsClass.Internet, 10,
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
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsDomainNameConstructorNullStringTest()
        {
            DnsDomainName domainName = new DnsDomainName(null);
            Assert.IsNotNull(domainName);
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
                Assert.AreEqual(extendedRcode, record.ExtendedReturnCode);
                Assert.AreEqual(version, record.Version);
                Assert.AreEqual(flags, record.Flags);
                Assert.AreEqual(data, record.Data);
            }
        }

        [TestMethod]
        public void DnsResourceDataOptionsParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataOptions(new DnsOptions(new DnsOptionLongLivedQuery(0, DnsLongLivedQueryOpCode.Setup,
                                                                                                     DnsLongLivedQueryErrorCode.Static, 1, 2)));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Opt, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Opt, resourceData, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void DnsQueryResourceRecordTtlGetTest()
        {
            var query = new DnsQueryResourceRecord(DnsDomainName.Root, DnsType.A, DnsClass.Internet);
            Assert.IsNull(query.Ttl);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
        public void DnsQueryResourceRecordDataGetTest()
        {
            var query = new DnsQueryResourceRecord(DnsDomainName.Root, DnsType.A, DnsClass.Internet);
            Assert.IsNull(query.Data);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNextDomainTest()
        {
            DataSegment bitmap = DnsResourceDataNextDomain.CreateTypeBitmap(new[] {DnsType.A, DnsType.Aaaa});
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitmap);
            Assert.IsFalse(resourceData.Equals(null));
            Assert.IsTrue(resourceData.IsTypePresentForOwner(DnsType.A));
            Assert.IsTrue(resourceData.IsTypePresentForOwner(DnsType.Aaaa));
            Assert.IsFalse(resourceData.IsTypePresentForOwner(DnsType.Ns));
            Assert.IsFalse(resourceData.IsTypePresentForOwner(DnsType.UInfo));
            MoreAssert.AreSequenceEqual(new[] {DnsType.A, DnsType.Aaaa}, resourceData.TypesExist);

            bitmap = DnsResourceDataNextDomain.CreateTypeBitmap(new DnsType[] { 0 });
            Assert.AreEqual(DataSegment.Empty, bitmap);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainTooBigDnsType()
        {
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), DataSegment.Empty);
            Assert.IsNull(resourceData.IsTypePresentForOwner((DnsType)(8 * 16 + 1)));
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainTooLongBitmapTest()
        {
            DataSegment bitmap = new DataSegment(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17});
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitmap);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainZeroEndedBitmapTest()
        {
            DataSegment bitmap = new DataSegment(new byte[] { 1, 0 });
            DnsResourceDataNextDomain resourceData = new DnsResourceDataNextDomain(new DnsDomainName("a.b.c"), bitmap);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNextDomainParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataNextDomain(new DnsDomainName("pcapdot.net"),
                                                             DnsResourceDataNextDomain.CreateTypeBitmap(new[] {DnsType.A, DnsType.A6}));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NextDomain, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NextDomain, resourceData, 12);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NextDomain, resourceData, -6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainConstructorNullTypeBitmapTest()
        {
            var resourceData = new DnsResourceDataNextDomain(DnsDomainName.Root, null);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainCreateTypeBitmapNullInputTest()
        {
            var bitmap = DnsResourceDataNextDomain.CreateTypeBitmap(null);
            Assert.IsNull(bitmap);
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
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void DnsResourceDataNamingAuthorityPointerIllegalFlagsTest()
        {
            var resourceData = new DnsResourceDataNamingAuthorityPointer(0, 0, new DataSegment(new[] {(byte)'%'}),
                                                                         DataSegment.Empty, DataSegment.Empty,
                                                                         DnsDomainName.Root);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionKeyConstructorNullKeyTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.KeyDeletion, DnsResponseCode.NoError,
                                                                 null, DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionKeyConstructorNullOtherTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.KeyDeletion, DnsResponseCode.NoError,
                                                                 DataSegment.Empty, null);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionKeyTooBigKeyTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.KeyDeletion, DnsResponseCode.NoError,
                                                                 new DataSegment(new byte[ushort.MaxValue + 1]), DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionKeyTooBigOtherTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.KeyDeletion, DnsResponseCode.NoError,
                                                                 DataSegment.Empty, new DataSegment(new byte[ushort.MaxValue + 1]));
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataTransactionKeyParseTooShortTest()
        {
            var resourceData = new DnsResourceDataTransactionKey(new DnsDomainName("pcapdot.net"), 0, 0, DnsTransactionKeyMode.KeyDeletion,
                                                                 DnsResponseCode.NoError, new DataSegment(new byte[5]), new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TKey, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TKey, resourceData, -6);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TKey, resourceData, -11);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TKey, resourceData, -23);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionSignatureConstructorNullMessageAuthenticationCodeTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(DnsDomainName.Root, 0, 0, null, 0, DnsResponseCode.NoError, DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionSignatureConstructorNullOtherTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(DnsDomainName.Root, 0, 0, DataSegment.Empty, 0, DnsResponseCode.NoError, null);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionSignatureTooBigMessageAuthenticationCodeTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(DnsDomainName.Root, 0, 0, new DataSegment(new byte[ushort.MaxValue + 1]), 0,
                                                                       DnsResponseCode.NoError, DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataTransactionSignatureTooBigOtherTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(DnsDomainName.Root, 0, 0, DataSegment.Empty, 0,
                                                                       DnsResponseCode.NoError, new DataSegment(new byte[ushort.MaxValue + 1]));
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataTransactionSignatureParseWrongSizeTest()
        {
            var resourceData = new DnsResourceDataTransactionSignature(new DnsDomainName("pcapdot.net"), 0, 0, new DataSegment(new byte[5]), 0,
                                                                       DnsResponseCode.NoError, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TransactionSignature, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TransactionSignature, resourceData, -6);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TransactionSignature, resourceData, -11);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TransactionSignature, resourceData, -23);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataHostIdentityProtocolTooBigHostIdentityTagTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(new DataSegment(new byte[byte.MaxValue + 1]), DnsPublicKeyAlgorithm.None,
                                                                       DataSegment.Empty, new DnsDomainName[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataHostIdentityProtocolTooBigPublicKeyTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(DataSegment.Empty, DnsPublicKeyAlgorithm.None,
                                                                       new DataSegment(new byte[ushort.MaxValue + 1]), new DnsDomainName[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataHostIdentityProtocolConstructorNullHostIdentityTagTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(null, DnsPublicKeyAlgorithm.None, DataSegment.Empty, new DnsDomainName[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataHostIdentityProtocolConstructorNullPublicKeyTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(DataSegment.Empty, DnsPublicKeyAlgorithm.None, null, new DnsDomainName[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataHostIdentityProtocolParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataHostIdentityProtocol(new DataSegment(new byte[5]), DnsPublicKeyAlgorithm.None, new DataSegment(new byte[5]),
                                                                       new[]
                                                                       {
                                                                           new DnsDomainName("pcapdot.net"),
                                                                           new DnsDomainName("pcapdotnet.codeplex.com")
                                                                       });
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Hip, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Hip, resourceData, -39);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Hip, resourceData, -49);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataLocationInformationInvalidSizeTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 9000000001L, 0, 0, 0, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataLocationInformationInvalidHorizontalPrecisionTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 0, 9000000001L, 0, 0, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataLocationInformationInvalidVerticalPrecisionTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 0, 0, 9000000001L, 0, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataLocationInformationParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataLocationInformation(0, 1000, 2000, 3000, 100, 200, 300);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Loc, resourceData, 1);
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
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
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
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataNetworkServiceAccessPointConstructorNullAreaAddressTest()
        {
            var resourceData = new DnsResourceDataNetworkServiceAccessPoint(null, 0, 0);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNetworkServiceAccessPointParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataNetworkServiceAccessPoint(new DataSegment(new byte[5]), 0, 0);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NetworkServiceAccessPoint, resourceData, -5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
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
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsAddressPrefixConstructorNullAddressFamilyDependentPartTtest()
        {
            var dnsAddressPrefix = new DnsAddressPrefix(AddressFamily.IpV4, 0, false, null);
            Assert.IsNull(dnsAddressPrefix);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainSecure3NextHashedOwnerNameTooBigTest()
        {
            var resourceData = new DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0, DataSegment.Empty,
                                                                    new DataSegment(new byte[byte.MaxValue + 1]), new DnsType[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataNextDomainSecure3SaltTooBigTest()
        {
            var resourceData = new DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0,
                                                                    new DataSegment(new byte[byte.MaxValue + 1]), DataSegment.Empty, new DnsType[0]);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNextDomainSecure3ParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0, new DataSegment(new byte[5]),
                                                                    new DataSegment(new byte[5]), new[] {DnsType.A, DnsType.A6,});
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3, resourceData, -8);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3, resourceData, -13);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3, resourceData, -14);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3, resourceData, -19);
        }

        [TestMethod]
        public void DnsGatewayTest()
        {
            DnsGateway gateway = new DnsGatewayIpV6(IpV6Address.Zero);
            Assert.IsTrue(gateway.Equals((object)gateway));
            Assert.IsFalse(gateway.Equals(null as object));
            Assert.IsFalse(new DnsGatewayIpV4(IpV4Address.Zero).Equals(null));
            Assert.IsFalse(new DnsGatewayDomainName(DnsDomainName.Root).Equals(null));
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
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataCertificationAuthorityAuthorizationTagTooBigTest()
        {
            var resourceData = new DnsResourceDataCertificationAuthorityAuthorization(DnsCertificationAuthorityAuthorizationFlags.Critical,
                                                                                      new DataSegment(new byte[byte.MaxValue + 1]), DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataCertificationAuthorityAuthorizationConstructorNullTagTest()
        {
            var resourceData = new DnsResourceDataCertificationAuthorityAuthorization(DnsCertificationAuthorityAuthorizationFlags.Critical, null,
                                                                                      DataSegment.Empty);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataCertificationAuthorityAuthorizationParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataCertificationAuthorityAuthorization(DnsCertificationAuthorityAuthorizationFlags.Critical,
                                                                                      new DataSegment(new byte[5]), new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.CertificationAuthorityAuthorization, resourceData, -6);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.CertificationAuthorityAuthorization, resourceData, -11);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataA6ConstructorAddressSuffixTooSmallTest()
        {
            var resourceData = new DnsResourceDataA6(127, IpV6Address.Zero, DnsDomainName.Root);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataA6ConstructorAddressSuffixTooBigTest()
        {
            var resourceData = new DnsResourceDataA6(1, IpV6Address.MaxValue, DnsDomainName.Root);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataA6ParseToShortTest()
        {
            var resourceData = new DnsResourceDataA6(100, new IpV6Address("::F12:3456"), new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.A6, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.A6, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.A6, resourceData, -14);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.A6, resourceData, -17);
        }

        [TestMethod]
        public void DnsResourceDataAddressPrefixListParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataAddressPrefixList(new DnsAddressPrefix(AddressFamily.IpV4, 0, false, new DataSegment(new byte[5])));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Apl, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Apl, resourceData, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataNInfoConstructorNullStringsTest()
        {
            var resourceData = new DnsResourceDataNInfo(null as ReadOnlyCollection<DataSegment>);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void DnsResourceDataNInfoConstructorTooFewStringsTest()
        {
            var resourceData = new DnsResourceDataNInfo();
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataNInfoParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataNInfo(new DataSegment(new byte[5]), new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NInfo, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NInfo, resourceData, -12);
        }

        [TestMethod]
        public void DnsResourceDataGeographicalPositionParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataGeographicalPosition("5.03", "-44.4", "22.1");
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.GPos, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.GPos, resourceData, -5);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.GPos, resourceData, -10);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.GPos, resourceData, -14);
        }

        [TestMethod]
        public void DnsResourceDataX400PointerParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataX400Pointer(0, new DnsDomainName("pcapdot.net"), new DnsDomainName("pcapdotnet.codeplex.com"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.PointerX400, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.PointerX400, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.PointerX400, resourceData, -26);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.PointerX400, resourceData, -39);
        }

        [TestMethod]
        public void DnsResourceDataIpSecKeyParseWrongLengthTest()
        {
            var resourceDataIpV4 = new DnsResourceDataIpSecKey(1, new DnsGatewayIpV4(IpV4Address.Zero), DnsPublicKeyAlgorithm.Rsa, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.IpSecKey, resourceDataIpV4, -6);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.IpSecKey, resourceDataIpV4, -10);

            var resourceDataIpV6 = new DnsResourceDataIpSecKey(1, new DnsGatewayIpV6(IpV6Address.Zero), DnsPublicKeyAlgorithm.Rsa, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.IpSecKey, resourceDataIpV6, -6);

            var resourceDataDomainName = new DnsResourceDataIpSecKey(1, new DnsGatewayDomainName(new DnsDomainName("pcapdot.net")), DnsPublicKeyAlgorithm.Rsa,
                                                                     new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.IpSecKey, resourceDataDomainName, -6);
        }

        [TestMethod]
        public void DnsResourceDataServerSelectionParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataServerSelection(0, 0, 0, new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.ServerSelection, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.ServerSelection, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.ServerSelection, resourceData, -14);
        }

        [TestMethod]
        public void DnsResourceDataStartOfAuthorityParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataStartOfAuthority(new DnsDomainName("pcapdot.net"), new DnsDomainName("pcapdotnet.codeplex.com"), 1, 2, 3, 4, 5);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.StartOfAuthority, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.StartOfAuthority, resourceData, -21);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.StartOfAuthority, resourceData, -46);
        }

        [TestMethod]
        public void DnsResourceDataTrustAnchorLinkParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataTrustAnchorLink(new DnsDomainName("pcapdot.net"), new DnsDomainName("pcapdotnet.codeplex.com"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TrustAnchorLink, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TrustAnchorLink, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TrustAnchorLink, resourceData, -25);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.TrustAnchorLink, resourceData, -37);
        }

        [TestMethod]
        public void DnsResourceDataMailExchangeParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataMailExchange(1, new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.MailExchange, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.MailExchange, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.MailExchange, resourceData, -14);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataDelegationSignerConstructorNullDigestTest()
        {
            var resourceData = new DnsResourceDataDelegationSigner(1, DnsAlgorithm.PrivateDns, DnsDigestType.Sha1, null);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }
        
        [TestMethod]
        public void DnsResourceDataDomainNameParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataDomainName(new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Ns, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Ns, resourceData, -1);
        }

        [TestMethod]
        public void DnsResourceDataIsdnParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataIsdn(new DataSegment(new byte[5]), new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Isdn, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Isdn, resourceData, -1);
        }

        [TestMethod]
        public void DnsResourceDataKeyParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataKey(false, true, false, true, false, true, DnsKeyNameType.NonZoneEntity, DnsKeySignatoryAttributes.General,
                                                      DnsKeyProtocol.Email, DnsAlgorithm.Indirect, 1, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Key, resourceData, -8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void DnsResourceDataNamingAuthorityPointerConstructorNullFlagsTest()
        {
            var resourceData = new DnsResourceDataNamingAuthorityPointer(1, 2, null, DataSegment.Empty, DataSegment.Empty, DnsDomainName.Root);
            Assert.IsNull(resourceData);
            Assert.Fail();
        }

        [TestMethod]
        public void DnsResourceDataSignatureParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataSignature(DnsType.A, DnsAlgorithm.RsaSha512, 2, 3, 4, 5, 6, new DnsDomainName("pcapdot.net"),
                                                            new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Signature, resourceData, -6);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Signature, resourceData, -19);
        }

        [TestMethod]
        public void DnsResourceDataUriParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataUri(1, 2, new List<DataSegment> {new DataSegment(new byte[5]), new DataSegment(new byte[5])});
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Uri, resourceData, -1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Uri, resourceData, -13);
        }

        [TestMethod]
        public void DnsResourceDataAtmAddressParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataAtmAddress(DnsAtmAddressFormat.E164, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.AtmA, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataCertificateParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataCertificate(DnsCertificateType.IPkix, 1, DnsAlgorithm.PrivateDns, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Cert, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataDnsKeyParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataDnsKey(false, true, false, 2, DnsAlgorithm.RsaSha512, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.DnsKey, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataMailingListInfoParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataMailingListInfo(new DnsDomainName("pcapdot.net"), new DnsDomainName("pcapdotnet.codeplex.com"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.MInfo, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.MInfo, resourceData, -1);
        }

        [TestMethod]
        public void DnsResourceDataHostInformationParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataHostInformation(new DataSegment(new byte[5]), new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.HInfo, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.HInfo, resourceData, -1);
        }

        [TestMethod]
        public void DnsResourceDataIpV4ParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataIpV4(IpV4Address.Zero);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.A, resourceData, 1);
        }

        [TestMethod]
        public void DnsResourceDataIpV6ParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataIpV6(IpV6Address.Zero);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Aaaa, resourceData, 1);
        }

        [TestMethod]
        public void DnsResourceDataNextDomainSecure3ParametersParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataNextDomainSecure3Parameters(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.OptOut, 1,
                                                                              new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3Parameters, resourceData, 1);
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.NSec3Parameters, resourceData, -1);
        }

        [TestMethod]
        public void DnsResourceDataRKeyParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataRKey(1, 2, DnsAlgorithm.Indirect, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.RKey, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataSinkParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataSink(DnsSinkCodingSubCoding.TextTaggedDataPrivate, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Sink, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataStringParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataString(new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.X25, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataWellKnownServiceParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataWellKnownService(IpV4Address.Zero, IpV4Protocol.IpV6Opts, new DataSegment(new byte[5]));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.Wks, resourceData, -6);
        }

        [TestMethod]
        public void DnsResourceDataAfsDatabaseParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataAfsDatabase(DnsAfsDatabaseSubtype.DceNcaCell, new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.AfsDatabase, resourceData, 1);
        }

        [TestMethod]
        public void DnsResourceDataKeyExchangerParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataKeyExchanger(1, new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.KeyExchanger, resourceData, 1);
        }

        [TestMethod]
        public void DnsResourceDataResponsiblePersonParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataResponsiblePerson(new DnsDomainName("pcapdotnet.codeplex.com"), new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.ResponsiblePerson, resourceData, 1);
        }

        [TestMethod]
        public void DnsResourceDataRouteThroughParseWrongLengthTest()
        {
            var resourceData = new DnsResourceDataRouteThrough(1, new DnsDomainName("pcapdot.net"));
            TestResourceRecordIsNotCreatedWithNewLength(DnsType.RouteThrough, resourceData, 1);
        }

        [TestMethod]
        public void DnsResourceDataKeyKeyTagTest()
        {
            var resourceData = new DnsResourceDataKey(false, false, false, false, false, false, DnsKeyNameType.UserOrAccountAtEndEntity,
                                                      DnsKeySignatoryAttributes.General, DnsKeyProtocol.IpSec, DnsAlgorithm.RsaSha1, null,
                                                      new DataSegment(new byte[] {1, 2, 3, 4}));
            Assert.AreEqual(2060, resourceData.KeyTag);

            resourceData = new DnsResourceDataKey(true, true, true, true, true, true, DnsKeyNameType.UserOrAccountAtEndEntity,
                                                  DnsKeySignatoryAttributes.General, DnsKeyProtocol.IpSec, DnsAlgorithm.RsaSha1, 123,
                                                  new DataSegment(new byte[] {1, 2, 3, 4}));
            Assert.AreEqual(64839, resourceData.KeyTag);

            resourceData = new DnsResourceDataKey(true, true, true, true, true, true, DnsKeyNameType.UserOrAccountAtEndEntity,
                                                  DnsKeySignatoryAttributes.General, DnsKeyProtocol.IpSec, DnsAlgorithm.RsaMd5, 123,
                                                  new DataSegment(new byte[] {1, 2, 3, 4}));
            Assert.AreEqual(515, resourceData.KeyTag);
        }

        [TestMethod]
        public void DnsResourceDataDnsKeyKeyTagTest()
        {
            var resourceData = new DnsResourceDataDnsKey(false, false, false, 123, DnsAlgorithm.RsaSha256, new DataSegment(new byte[] { 1, 2, 3, 4 }));
            Assert.AreEqual(32526, resourceData.KeyTag);

            resourceData = new DnsResourceDataDnsKey(true, true, true, 123, DnsAlgorithm.RsaSha256, new DataSegment(new byte[] { 1, 2, 3, 4 }));
            Assert.AreEqual(32911, resourceData.KeyTag);

            resourceData = new DnsResourceDataDnsKey(true, true, true, 123, DnsAlgorithm.RsaMd5, new DataSegment(new byte[] { 1, 2, 3, 4 }));
            Assert.AreEqual(515, resourceData.KeyTag);
        }

        [TestMethod]
        public void DnsOptionClientSubnetTooShort()
        {
            DnsLayer dnsLayer =
                new DnsLayer
                    {
                        Answers =
                            new List<DnsDataResourceRecord>(
                            new[]
                                {
                                    new DnsDataResourceRecord(new DnsDomainName("a"), DnsType.Opt, DnsClass.Internet, 10,
                                                              new DnsResourceDataOptions(
                                                                  new DnsOptions(new DnsOptionClientSubnet(AddressFamily.IpV4, 1, 2,
                                                                                                           new DataSegment(new byte[] {3, 4, 5, 6})))))
                                }),
                    };
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(), new IpV4Layer(), new UdpLayer(),
                                                dnsLayer);
            packet = new Packet(packet.Buffer, DateTime.Now, DataLinkKind.Ethernet);
            Assert.IsTrue(packet.Ethernet.IpV4.Udp.Dns.IsValid);
            packet.Buffer[66] -= 5;
            packet.Buffer[70] -= 5;
            packet = new Packet(packet.Buffer, DateTime.Now, DataLinkKind.Ethernet);
            Assert.IsFalse(packet.Ethernet.IpV4.Udp.Dns.IsValid);
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
            var resourceRecord = new DnsDataResourceRecord(DnsDomainName.Root, dnsType, DnsClass.Internet, 0, resourceData);
            var paddingResourceRecord = new DnsDataResourceRecord(DnsDomainName.Root, DnsType.Null, DnsClass.Internet, 0,
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
                EthernetDatagram.HeaderLengthValue + IpV4Datagram.HeaderMinimumLength + UdpDatagram.HeaderLength + DnsDatagram.HeaderLength + 5 + 4;
            ushort oldDataLength = buffer.ReadUShort(dataLengthOffset, Endianity.Big);
            ushort newDataLength = (ushort)(oldDataLength + dataLengthDiff);
            buffer.Write(dataLengthOffset, newDataLength, Endianity.Big);
            packet = new Packet(buffer, DateTime.Now, DataLinkKind.Ethernet);

            Assert.IsFalse(packet.Ethernet.IpV4.Udp.Dns.Answers.Any());
        }
    }
}