using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Dhcp;
using PcapDotNet.Packets.Dhcp.Options;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for ArpTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DhcpTests
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

        #endregion Additional test attributes

        [TestMethod]
        public void RandomDhcpTest()
        {
            int seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            Random random = new Random(seed);

            for (int i = 0; i != 1000; ++i)
            {
                EthernetLayer ethernetLayer = random.NextEthernetLayer(EthernetType.None);

                IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
                ipV4Layer.HeaderChecksum = null;

                UdpLayer udpLayer = random.NextUdpLayer();
                udpLayer.Checksum = null;

                DhcpLayer dhcpLayer = random.NextDhcpLayer();

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, dhcpLayer);

                Assert.IsTrue(packet.IsValid, "IsValid");
                DhcpLayer actualLayer = (DhcpLayer)packet.Ethernet.Ip.Udp.Dhcp.ExtractLayer();
                if (!dhcpLayer.Equals(actualLayer))
                {
                    Console.WriteLine("");
                }

                Assert.AreEqual(dhcpLayer, actualLayer, "DHCP Layer");
            }
        }

        [TestMethod]
        public void DhcpDatagramClientMacAddressTest()
        {
            Random random = new Random(4711);

            EthernetLayer ethernetLayer = random.NextEthernetLayer(EthernetType.None);

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            UdpLayer udpLayer = random.NextUdpLayer();
            udpLayer.Checksum = null;

            DhcpLayer dhcpLayer = new DhcpLayer()
            {
                HardwareAddressLength = 6,
                ClientHardwareAddress = new Packets.DataSegment(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xED, 0xCB, 0xA9, 0x87, 0x65, 0x43, 0x21 })
            };

            Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, dhcpLayer);

            Assert.AreEqual(new MacAddress("01:23:45:67:89:AB"), packet.Ethernet.IpV4.Udp.Dhcp.ClientMacAddress);

            Assert.AreEqual(new MacAddress("01:23:45:67:89:AB"), ((DhcpLayer)packet.Ethernet.IpV4.Udp.Dhcp.ExtractLayer()).ClientMacAddress);
        }

        [TestMethod]
        public void DhcpLayerClientMacAddressTest()
        {
            DhcpLayer dhcpLayer = new DhcpLayer()
            {
                HardwareAddressLength = 6,
                ClientHardwareAddress = new Packets.DataSegment(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xED, 0xCB, 0xA9, 0x87, 0x65, 0x43, 0x21 })
            };

            Assert.AreEqual(new MacAddress("01:23:45:67:89:AB"), dhcpLayer.ClientMacAddress);

            dhcpLayer = new DhcpLayer();

            Assert.AreEqual(MacAddress.Zero, dhcpLayer.ClientMacAddress);

            dhcpLayer.ClientMacAddress = new MacAddress("01:23:45:67:89:AB");
            Assert.AreEqual(new MacAddress("01:23:45:67:89:AB"), dhcpLayer.ClientMacAddress);
        }

        [TestMethod]
        public void DhcpOptionEqualsTest()
        {
            DhcpBootfileNameOption option = new DhcpBootfileNameOption("file1");
            Assert.IsFalse(option.Equals(null));
            Assert.AreNotEqual(option, new DhcpAnyOption(new DataSegment(Encoding.ASCII.GetBytes("file1")), DhcpOptionCode.AllSubnetsAreLocal));
            Assert.AreNotEqual(option, new DhcpAnyOption(new DataSegment(Encoding.ASCII.GetBytes("filelong")), DhcpOptionCode.BootfileName));
            Assert.AreNotEqual(option, new DhcpAnyOption(new DataSegment(Encoding.ASCII.GetBytes("file2")), DhcpOptionCode.BootfileName));
            Assert.AreEqual(option, new DhcpBootfileNameOption("file1"));
            Assert.AreEqual(option, new DhcpAnyOption(new DataSegment(Encoding.ASCII.GetBytes("file1")), DhcpOptionCode.BootfileName));
        }

        [TestMethod]
        public void DhcpAllSubnetsAreLocalOptionTest()
        {
            DhcpAllSubnetsAreLocalOption option = new DhcpAllSubnetsAreLocalOption(true);
            Assert.AreEqual(DhcpOptionCode.AllSubnetsAreLocal, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpAllSubnetsAreLocalOption(false);
            Assert.AreEqual(DhcpOptionCode.AllSubnetsAreLocal, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpAnyOptionTest()
        {
            DataSegment data = new DataSegment(new byte[10]);
            DhcpAnyOption option = new DhcpAnyOption(data, (DhcpOptionCode)240);
            Assert.AreEqual((DhcpOptionCode)240, option.OptionCode);
            Assert.AreEqual(10, option.Length);
            Assert.IsNotNull(option.Data);
            Assert.AreEqual(data, option.Data);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpAnyOptionCtorNullTest()
        {
            new DhcpAnyOption(null, (DhcpOptionCode)240);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpAnyOptionCtorToLargeTest()
        {
            new DhcpAnyOption(new DataSegment(new byte[byte.MaxValue + 1]), (DhcpOptionCode)240);
        }

        [TestMethod]
        public void DhcpArpCacheTimeoutOptionTest()
        {
            DhcpArpCacheTimeoutOption option = new DhcpArpCacheTimeoutOption(4711);
            Assert.AreEqual(DhcpOptionCode.ArpCacheTimeout, option.OptionCode);
            Assert.AreEqual(4, option.Length);
            Assert.AreEqual<uint>(4711, option.Time);

            option.Time = 801;
            Assert.AreEqual<uint>(801, option.Time);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpBootfileNameOptionTest()
        {
            DhcpBootfileNameOption option = new DhcpBootfileNameOption("bootfile");
            Assert.AreEqual(DhcpOptionCode.BootfileName, option.OptionCode);
            Assert.AreEqual("bootfile", option.BootfileName);

            option.BootfileName = "changed";
            Assert.AreEqual("changed", option.BootfileName);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpBootfileNameOptionCtorNullTest()
        {
            new DhcpBootfileNameOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpBootfileNameOptionCtorEmptryTest()
        {
            new DhcpBootfileNameOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpBootfileNameOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpBootfileNameOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpBootFileSizeOptionTest()
        {
            DhcpBootFileSizeOption option = new DhcpBootFileSizeOption(4711);
            Assert.AreEqual(DhcpOptionCode.BootFileSize, option.OptionCode);
            Assert.AreEqual<ushort>(4711, option.FileSize);

            option.FileSize = 999;
            Assert.AreEqual<ushort>(999, option.FileSize);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpBroadcastAddressOptionTest()
        {
            DhcpBroadcastAddressOption option = new DhcpBroadcastAddressOption(new IpV4Address("10.20.30.40"));
            Assert.AreEqual(DhcpOptionCode.BroadcastAddress, option.OptionCode);
            Assert.AreEqual(new IpV4Address("10.20.30.40"), option.BroadcastAddress);

            option.BroadcastAddress = new IpV4Address("11.12.13.14");
            Assert.AreEqual(new IpV4Address("11.12.13.14"), option.BroadcastAddress);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpClientIdentifierOptionTest()
        {
            DataSegment testData = new DataSegment(new byte[] { 1, 4, 5, 6, 3, 2 });
            DhcpClientIdentifierOption option = new DhcpClientIdentifierOption(5, testData);
            Assert.AreEqual(DhcpOptionCode.ClientIdentifier, option.OptionCode);
            Assert.AreEqual(5, option.ClientIdentifierType);
            Assert.AreEqual(testData, option.ClientIdentifier);

            testData = new DataSegment(new byte[] { 3, 2, 4, 52, 2, 3, 4 });
            option.ClientIdentifierType = 47;
            option.ClientIdentifier = testData;
            Assert.AreEqual(47, option.ClientIdentifierType);
            Assert.AreEqual(testData, option.ClientIdentifier);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpClientIdentifierOptionCtorNullTest()
        {
            new DhcpClientIdentifierOption(5, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpClientIdentifierOptionCtorEmptyTest()
        {
            new DhcpClientIdentifierOption(5, new DataSegment(new byte[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpClientIdentifierOptionCtorToLongest()
        {
            new DhcpClientIdentifierOption(5, new DataSegment(new byte[byte.MaxValue - 1]));
        }

        [TestMethod]
        public void DhcpCookieServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpCookieServerOption option = new DhcpCookieServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.CookieServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpCookieServerOptionCtorNullTest()
        {
            new DhcpCookieServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpCookieServerOptionCtorEmptyTest()
        {
            new DhcpCookieServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpCookieServerOptionCtorToLongest()
        {
            new DhcpCookieServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpDefaultFingerServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpDefaultFingerServerOption option = new DhcpDefaultFingerServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.DefaultFingerServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpDefaultFingerServerOptionCtorNullTest()
        {
            new DhcpCookieServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDefaultFingerServerOptionCtorEmptyTest()
        {
            new DhcpCookieServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDefaultFingerServerOptionCtorToLongest()
        {
            new DhcpCookieServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpDefaultInternetRelayChatServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpDefaultInternetRelayChatServerOption option = new DhcpDefaultInternetRelayChatServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.DefaultInternetRelayChatServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpDefaultInternetRelayChatServerOptionCtorNullTest()
        {
            new DhcpDefaultInternetRelayChatServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDefaultInternetRelayChatServerOptionCtorEmptyTest()
        {
            new DhcpDefaultInternetRelayChatServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDefaultInternetRelayChatServerOptionCtorToLongest()
        {
            new DhcpDefaultInternetRelayChatServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpDefaultIPTimeToLiveOptionTest()
        {
            DhcpDefaultIPTimeToLiveOption option = new DhcpDefaultIPTimeToLiveOption(47);
            Assert.AreEqual(DhcpOptionCode.DefaultIpTimeToLive, option.OptionCode);
            Assert.AreEqual(47, option.Ttl);

            option.Ttl = 200;
            Assert.AreEqual(200, option.Ttl);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpDefaultWorldWideWebServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpDefaultWorldWideWebServerOption option = new DhcpDefaultWorldWideWebServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.DefaultWorldWideWebServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpDefaultWorldWideWebServerOptionCtorNullTest()
        {
            new DhcpDefaultWorldWideWebServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDefaultWorldWideWebServerOptionCtorEmptyTest()
        {
            new DhcpDefaultWorldWideWebServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDefaultWorldWideWebServerOptionCtorToLongest()
        {
            new DhcpDefaultWorldWideWebServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpDomainNameOptionTest()
        {
            DhcpDomainNameOption option = new DhcpDomainNameOption("domainName");
            Assert.AreEqual(DhcpOptionCode.DomainName, option.OptionCode);
            Assert.AreEqual("domainName", option.DomainName);

            option.DomainName = "changed";
            Assert.AreEqual("changed", option.DomainName);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpDomainNameOptionCtorNullTest()
        {
            new DhcpDomainNameOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDomainNameOptionCtorEmptyTest()
        {
            new DhcpDomainNameOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDomainNameOptionCtorToLongest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpDomainNameOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpDomainNameServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpDomainNameServerOption option = new DhcpDomainNameServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.DomainNameServerServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpDomainNameServerOptionCtorNullTest()
        {
            new DhcpDomainNameServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDomainNameServerOptionCtorEmptyTest()
        {
            new DhcpDomainNameServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpDomainNameServerOptionCtorToLongest()
        {
            new DhcpDomainNameServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpEndOptionTest()
        {
            DhcpEndOption option = new DhcpEndOption();
            Assert.AreEqual(DhcpOptionCode.End, option.OptionCode);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpEthernetEncapsulationOptionTest()
        {
            DhcpEthernetEncapsulationOption option = new DhcpEthernetEncapsulationOption(true);
            Assert.AreEqual(DhcpOptionCode.EthernetEncapsulation, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpEthernetEncapsulationOption(false);
            Assert.AreEqual(DhcpOptionCode.EthernetEncapsulation, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpExtensionsPathOptionTest()
        {
            DhcpExtensionsPathOption option = new DhcpExtensionsPathOption("extensionPathname");
            Assert.AreEqual(DhcpOptionCode.ExtensionsPath, option.OptionCode);
            Assert.AreEqual("extensionPathname", option.ExtensionsPathname);

            option.ExtensionsPathname = "changed";
            Assert.AreEqual("changed", option.ExtensionsPathname);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpExtensionsPathOptionCtorNullTest()
        {
            new DhcpExtensionsPathOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpExtensionsPathOptionCtorEmptyTest()
        {
            new DhcpExtensionsPathOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpExtensionsPathOptionCtorToLongest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpExtensionsPathOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpHostNameOptionTest()
        {
            DhcpHostNameOption option = new DhcpHostNameOption("hostName");
            Assert.AreEqual(DhcpOptionCode.HostName, option.OptionCode);
            Assert.AreEqual("hostName", option.HostName);

            option.HostName = "changed";
            Assert.AreEqual("changed", option.HostName);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpHostNameOptionCtorNullTest()
        {
            new DhcpHostNameOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpHostNameOptionCtorEmptyTest()
        {
            new DhcpHostNameOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpHostNameOptionCtorToLongest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpHostNameOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpImpressServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpImpressServerOption option = new DhcpImpressServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.ImpressServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpImpressServerOptionCtorNullTest()
        {
            new DhcpImpressServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpImpressServerOptionCtorEmptyTest()
        {
            new DhcpImpressServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpImpressServerOptionCtorToLongest()
        {
            new DhcpImpressServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpInterfaceMtuOptionTest()
        {
            DhcpInterfaceMtuOption option = new DhcpInterfaceMtuOption(100);
            Assert.AreEqual(DhcpOptionCode.InterfaceMtu, option.OptionCode);
            Assert.AreEqual(100, option.Mtu);

            option.Mtu = 200;
            Assert.AreEqual(200, option.Mtu);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpInterfaceMtuOptionCtorMinMTUTest()
        {
            new DhcpInterfaceMtuOption(67);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpInterfaceMtuOptionMinMTUTest()
        {
            new DhcpInterfaceMtuOption(500).Mtu = 67;
        }

        [TestMethod]
        public void DhcpIPAddressLeaseTimeOptionTest()
        {
            DhcpIPAddressLeaseTimeOption option = new DhcpIPAddressLeaseTimeOption(100);
            Assert.AreEqual(DhcpOptionCode.IPAddressLeaseTime, option.OptionCode);
            Assert.AreEqual<uint>(100, option.LeaseTime);

            option.LeaseTime = 200;
            Assert.AreEqual<uint>(200, option.LeaseTime);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpIPForwardingEnableOptionTest()
        {
            DhcpIPForwardingEnableOption option = new DhcpIPForwardingEnableOption(true);
            Assert.AreEqual(DhcpOptionCode.IPForwardingEnable, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpIPForwardingEnableOption(false);
            Assert.AreEqual(DhcpOptionCode.IPForwardingEnable, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpLogServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpLogServerOption option = new DhcpLogServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.LogServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpLogServerOptionCtorNullTest()
        {
            new DhcpLogServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpLogServerOptionCtorEmptyTest()
        {
            new DhcpLogServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpLogServerOptionCtorToLongest()
        {
            new DhcpLogServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpLprServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpLprServerOption option = new DhcpLprServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.LprServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpLprServerOptionCtorNullTest()
        {
            new DhcpLprServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpLprServerOptionCtorEmptyTest()
        {
            new DhcpLprServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpLprServerOptionCtorToLongest()
        {
            new DhcpLprServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpMaskSupplierOptionTest()
        {
            DhcpMaskSupplierOption option = new DhcpMaskSupplierOption(true);
            Assert.AreEqual(DhcpOptionCode.MaskSupplier, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpMaskSupplierOption(false);
            Assert.AreEqual(DhcpOptionCode.MaskSupplier, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpMaximumDatagramReassemblySizeOptionTest()
        {
            DhcpMaximumDatagramReassemblySizeOption option = new DhcpMaximumDatagramReassemblySizeOption(600);
            Assert.AreEqual(DhcpOptionCode.MaximumDatagramReassemblySize, option.OptionCode);
            Assert.AreEqual(600, option.Size);

            option.Size = 8000;
            Assert.AreEqual(8000, option.Size);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMaximumDatagramReassemblySizeOptionCtorMinSizeTest()
        {
            new DhcpMaximumDatagramReassemblySizeOption(575);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMaximumDatagramReassemblySizeOptionMinSizeTest()
        {
            new DhcpMaximumDatagramReassemblySizeOption(600).Size = 575;
        }

        [TestMethod]
        public void DhcpMaximumDhcpMessageSizeOptionTest()
        {
            DhcpMaximumDhcpMessageSizeOption option = new DhcpMaximumDhcpMessageSizeOption(600);
            Assert.AreEqual(DhcpOptionCode.MaximumDhcpMessageSize, option.OptionCode);
            Assert.AreEqual(600, option.MaxLength);

            option.MaxLength = 8000;
            Assert.AreEqual(8000, option.MaxLength);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpMeritDumpFileOptionTest()
        {
            DhcpMeritDumpFileOption option = new DhcpMeritDumpFileOption("DumpFilePathname");
            Assert.AreEqual(DhcpOptionCode.MeritDumpFile, option.OptionCode);
            Assert.AreEqual("DumpFilePathname", option.DumpFilePathname);

            option.DumpFilePathname = "changed";
            Assert.AreEqual("changed", option.DumpFilePathname);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpMeritDumpFileOptionCtorNullTest()
        {
            new DhcpMeritDumpFileOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMeritDumpFileOptionCtorEmptryTest()
        {
            new DhcpMeritDumpFileOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMeritDumpFileOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpMeritDumpFileOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpMessageOptionTest()
        {
            DhcpMessageOption option = new DhcpMessageOption("Text");
            Assert.AreEqual(DhcpOptionCode.Message, option.OptionCode);
            Assert.AreEqual("Text", option.Text);

            option.Text = "changed";
            Assert.AreEqual("changed", option.Text);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpMessageOptionCtorNullTest()
        {
            new DhcpMessageOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMessageOptionCtorEmptryTest()
        {
            new DhcpMessageOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMessageOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpMessageOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpMessageTypeOptionTest()
        {
            DhcpMessageTypeOption option = new DhcpMessageTypeOption(DhcpMessageTypeOption.MessageType.Ack);
            Assert.AreEqual(DhcpOptionCode.MessageType, option.OptionCode);
            Assert.AreEqual(DhcpMessageTypeOption.MessageType.Ack, option.Type);

            option.Type = DhcpMessageTypeOption.MessageType.Release;
            Assert.AreEqual(DhcpMessageTypeOption.MessageType.Release, option.Type);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpBootfileNameCtorInvalidTest()
        {
            new DhcpMessageTypeOption((DhcpMessageTypeOption.MessageType)77);
        }

        [TestMethod]
        public void DhcpMobileIPHomeAgentOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpMobileIPHomeAgentOption option = new DhcpMobileIPHomeAgentOption(addresses);
            Assert.AreEqual(DhcpOptionCode.MobileIPHomeAgent, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpMobileIPHomeAgentOptionCtorNullTest()
        {
            new DhcpMobileIPHomeAgentOption(null);
        }

        [TestMethod]
        public void DhcpMobileIPHomeAgentOptionCtorEmptyTest()
        {
            new DhcpMobileIPHomeAgentOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpMobileIPHomeAgentOptionCtorToLongest()
        {
            new DhcpMobileIPHomeAgentOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNameServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNameServerOption option = new DhcpNameServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NameServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNameServerOptionCtorNullTest()
        {
            new DhcpNameServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNameServerOptionCtorEmptyTest()
        {
            new DhcpNameServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNameServerOptionCtorToLongest()
        {
            new DhcpNameServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNetBiosOverTcpIpDatagramDistributionServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNetBiosOverTcpIpDatagramDistributionServerOption option = new DhcpNetBiosOverTcpIpDatagramDistributionServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NetBiosOverTcpIpDatagramDistributionServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetBiosOverTcpIpDatagramDistributionServerOptionCtorNullTest()
        {
            new DhcpNetBiosOverTcpIpDatagramDistributionServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetBiosOverTcpIpDatagramDistributionServerOptionCtorEmptyTest()
        {
            new DhcpNetBiosOverTcpIpDatagramDistributionServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetBiosOverTcpIpDatagramDistributionServerOptionCtorToLongest()
        {
            new DhcpNetBiosOverTcpIpDatagramDistributionServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNetBiosOverTcpIpNameServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNetBiosOverTcpIpNameServerOption option = new DhcpNetBiosOverTcpIpNameServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NetBiosOverTcpIpNameServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetBiosOverTcpIpNameServerOptionCtorNullTest()
        {
            new DhcpNetBiosOverTcpIpNameServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetBiosOverTcpIpNameServerOptionCtorEmptyTest()
        {
            new DhcpNetBiosOverTcpIpNameServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetBiosOverTcpIpNameServerOptionCtorToLongest()
        {
            new DhcpNetBiosOverTcpIpNameServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNetBiosOverTcpIpNodeTypeOptionTest()
        {
            DhcpNetBiosOverTcpIpNodeTypeOption option = new DhcpNetBiosOverTcpIpNodeTypeOption(DhcpNetBiosOverTcpIpNodeTypeOption.NodeType.BNode);
            Assert.AreEqual(DhcpOptionCode.NetBiosOverTcpIpNodeType, option.OptionCode);
            Assert.AreEqual(DhcpNetBiosOverTcpIpNodeTypeOption.NodeType.BNode, option.Type);

            option.Type = DhcpNetBiosOverTcpIpNodeTypeOption.NodeType.HNode | DhcpNetBiosOverTcpIpNodeTypeOption.NodeType.PNode;
            Assert.AreEqual(DhcpNetBiosOverTcpIpNodeTypeOption.NodeType.HNode | DhcpNetBiosOverTcpIpNodeTypeOption.NodeType.PNode, option.Type);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetBiosOverTcpIpNodeTypeOptionCtorInvalidTest()
        {
            new DhcpNetBiosOverTcpIpNodeTypeOption(0);
        }

        [TestMethod]
        public void DhcpNetBiosOverTcpIpScopeOptionTest()
        {
            DataSegment data = new DataSegment(new byte[10]);
            DhcpNetBiosOverTcpIpScopeOption option = new DhcpNetBiosOverTcpIpScopeOption(data);
            Assert.IsNotNull(option.NetBiosScope);
            Assert.AreEqual(data, option.NetBiosScope);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetBiosOverTcpIpScopeOptionCtorNullTest()
        {
            new DhcpNetBiosOverTcpIpScopeOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetBiosOverTcpIpScopeOptionToLargeTest()
        {
            new DhcpNetBiosOverTcpIpScopeOption(new DataSegment(new byte[byte.MaxValue + 1]));
        }

        [TestMethod]
        public void DhcpNetworkInformationServersOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNetworkInformationServersOption option = new DhcpNetworkInformationServersOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NetworkInformationServers, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetworkInformationServersOptionCtorNullTest()
        {
            new DhcpNetworkInformationServersOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServersOptionCtorEmptyTest()
        {
            new DhcpNetworkInformationServersOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServersOptionCtorToLongest()
        {
            new DhcpNetworkInformationServersOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNetworkInformationServiceDomainOptionTest()
        {
            DhcpNetworkInformationServiceDomainOption option = new DhcpNetworkInformationServiceDomainOption("NIS Domain Name");
            Assert.AreEqual(DhcpOptionCode.NetworkInformationServiceDomain, option.OptionCode);
            Assert.AreEqual("NIS Domain Name", option.NisDomainName);

            option.NisDomainName = "changed";
            Assert.AreEqual("changed", option.NisDomainName);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetworkInformationServiceDomainOptionCtorNullTest()
        {
            new DhcpNetworkInformationServiceDomainOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServiceDomainOptionCtorEmptryTest()
        {
            new DhcpNetworkInformationServiceDomainOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServiceDomainOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpNetworkInformationServiceDomainOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpNetworkInformationServicePlusDomainOptionOptionTest()
        {
            DhcpNetworkInformationServicePlusDomainOption option = new DhcpNetworkInformationServicePlusDomainOption("NIS Client Domain Name");
            Assert.AreEqual(DhcpOptionCode.NetworkInformationServicePlusDomain, option.OptionCode);
            Assert.AreEqual("NIS Client Domain Name", option.NisClientDomainName);

            option.NisClientDomainName = "changed";
            Assert.AreEqual("changed", option.NisClientDomainName);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetworkInformationServicePlusDomainOptionCtorNullTest()
        {
            new DhcpNetworkInformationServicePlusDomainOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServicePlusDomainOptionCtorEmptryTest()
        {
            new DhcpNetworkInformationServicePlusDomainOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServicePlusDomainOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpNetworkInformationServicePlusDomainOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpNetworkInformationServicePlusServersOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNetworkInformationServicePlusServersOption option = new DhcpNetworkInformationServicePlusServersOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NetworkInformationServicePlusServers, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetworkInformationServicePlusServersOptionCtorNullTest()
        {
            new DhcpNetworkInformationServicePlusServersOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServicePlusServersOptionCtorEmptyTest()
        {
            new DhcpNetworkInformationServicePlusServersOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkInformationServicePlusServersOptionCtorToLongest()
        {
            new DhcpNetworkInformationServicePlusServersOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNetworkNewsTransportProtocolServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNetworkNewsTransportProtocolServerOption option = new DhcpNetworkNewsTransportProtocolServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NetworkNewsTransportProtocolServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetworkNewsTransportProtocolServerOptionCtorNullTest()
        {
            new DhcpNetworkNewsTransportProtocolServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkNewsTransportProtocolServerOptionCtorEmptyTest()
        {
            new DhcpNetworkNewsTransportProtocolServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkNewsTransportProtocolServerOptionCtorToLongest()
        {
            new DhcpNetworkNewsTransportProtocolServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNetworkTimeProtocolServersOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpNetworkTimeProtocolServersOption option = new DhcpNetworkTimeProtocolServersOption(addresses);
            Assert.AreEqual(DhcpOptionCode.NetworkTimeProtocolServers, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpNetworkTimeProtocolServersOptionCtorNullTest()
        {
            new DhcpNetworkTimeProtocolServersOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkTimeProtocolServersOptionCtorEmptyTest()
        {
            new DhcpNetworkTimeProtocolServersOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpNetworkTimeProtocolServersOptionCtorToLongest()
        {
            new DhcpNetworkTimeProtocolServersOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpNonLocalSourceRoutingEnableOptionnTest()
        {
            DhcpNonLocalSourceRoutingEnableOption option = new DhcpNonLocalSourceRoutingEnableOption(true);
            Assert.AreEqual(DhcpOptionCode.NonLocalSourceRoutingEnable, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpNonLocalSourceRoutingEnableOption(false);
            Assert.AreEqual(DhcpOptionCode.NonLocalSourceRoutingEnable, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpOptionOverloadOptionTest()
        {
            DhcpOptionOverloadOption option = new DhcpOptionOverloadOption(DhcpOptionOverloadOption.OptionOverloadValue.SName);
            Assert.AreEqual(DhcpOptionCode.OptionOverload, option.OptionCode);
            Assert.AreEqual(DhcpOptionOverloadOption.OptionOverloadValue.SName, option.Value);

            option.Value = DhcpOptionOverloadOption.OptionOverloadValue.Both;
            Assert.AreEqual(DhcpOptionOverloadOption.OptionOverloadValue.Both, option.Value);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpOptionOverloadOptionCtorInvalidTest()
        {
            new DhcpOptionOverloadOption(0);
        }

        [TestMethod]
        public void DhcpPadOptionTest()
        {
            DhcpPadOption option = new DhcpPadOption();
            Assert.AreEqual(DhcpOptionCode.Pad, option.OptionCode);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpParameterRequestListOptionTest()
        {
            DhcpOptionCode[] options = new DhcpOptionCode[] { DhcpOptionCode.End, DhcpOptionCode.ParameterRequestList };
            DhcpParameterRequestListOption option = new DhcpParameterRequestListOption(options);
            Assert.AreEqual(DhcpOptionCode.ParameterRequestList, option.OptionCode);
            CollectionAssert.AreEqual(options, option.OptionCodes.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpParameterRequestListOptionCtorNullTest()
        {
            new DhcpParameterRequestListOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpParameterRequestListOptionCtorEmptyTest()
        {
            new DhcpParameterRequestListOption(new DhcpOptionCode[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpParameterRequestListOptionCtorToLongest()
        {
            new DhcpParameterRequestListOption(new DhcpOptionCode[255 / sizeof(DhcpOptionCode) + 1]);
        }

        [TestMethod]
        public void DhcpPathMtuAgingTimeoutOptionTest()
        {
            DhcpPathMtuAgingTimeoutOption option = new DhcpPathMtuAgingTimeoutOption(4711);
            Assert.AreEqual(DhcpOptionCode.PathMtuAgingTimeout, option.OptionCode);
            Assert.AreEqual<uint>(4711, option.Timeout);

            option.Timeout = 999;
            Assert.AreEqual<uint>(999, option.Timeout);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpPathMtuPlateauTableOptionTest()
        {
            ushort[] sizes = new ushort[] { 4711, 666 };
            DhcpPathMtuPlateauTableOption option = new DhcpPathMtuPlateauTableOption(sizes);
            Assert.AreEqual(DhcpOptionCode.PathMtuPlateauTable, option.OptionCode);
            CollectionAssert.AreEqual(sizes, option.Sizes.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpPathMtuPlateauTableOptionCtorNullTest()
        {
            new DhcpPathMtuPlateauTableOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpPathMtuPlateauTableOptionCtorEmptyTest()
        {
            new DhcpPathMtuPlateauTableOption(new ushort[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpPathMtuPlateauTableOptionCtorToLongest()
        {
            new DhcpPathMtuPlateauTableOption(new ushort[255 / sizeof(ushort) + 1]);
        }

        [TestMethod]
        public void DhcpPerformMaskDiscoveryOptionTest()
        {
            DhcpPerformMaskDiscoveryOption option = new DhcpPerformMaskDiscoveryOption(true);
            Assert.AreEqual(DhcpOptionCode.PerformMaskDiscovery, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpPerformMaskDiscoveryOption(false);
            Assert.AreEqual(DhcpOptionCode.PerformMaskDiscovery, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpPerformRouterDiscoveryOptionTest()
        {
            DhcpPerformRouterDiscoveryOption option = new DhcpPerformRouterDiscoveryOption(true);
            Assert.AreEqual(DhcpOptionCode.PerformRouterDiscovery, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpPerformRouterDiscoveryOption(false);
            Assert.AreEqual(DhcpOptionCode.PerformRouterDiscovery, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpPolicyFilterOptionTest()
        {
            DhcpPolicyFilterOption.IpV4AddressWithMask[] filters = new DhcpPolicyFilterOption.IpV4AddressWithMask[] { new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("10.20.30.40"), new IpV4Address("255.255.255.0")), new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("1.2.3.4"), new IpV4Address("222.0.0.0")) };
            DhcpPolicyFilterOption option = new DhcpPolicyFilterOption(filters);
            Assert.AreEqual(DhcpOptionCode.PolicyFilter, option.OptionCode);
            CollectionAssert.AreEqual(filters, option.Filters.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpPolicyFilterOptionCtorNullTest()
        {
            new DhcpPolicyFilterOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpPolicyFilterOptionCtorEmptyTest()
        {
            new DhcpPolicyFilterOption(new DhcpPolicyFilterOption.IpV4AddressWithMask[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpPolicyFilterOptionCtorToLongest()
        {
            new DhcpPolicyFilterOption(new DhcpPolicyFilterOption.IpV4AddressWithMask[255 / DhcpPolicyFilterOption.IpV4AddressWithMask.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpPolicyFilterOptionIpV4AddressWithMaskEqualsTest()
        {
            DhcpPolicyFilterOption.IpV4AddressWithMask a = new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("10.20.30.40"), new IpV4Address("255.255.255.0"));
            DhcpPolicyFilterOption.IpV4AddressWithMask a2 = new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("10.20.30.40"), new IpV4Address("255.255.255.0"));
            DhcpPolicyFilterOption.IpV4AddressWithMask b = new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("1.2.3.4"), new IpV4Address("222.0.0.0"));
            DhcpPolicyFilterOption.IpV4AddressWithMask b2 = new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("10.20.30.40"), new IpV4Address("222.0.0.0"));
            DhcpPolicyFilterOption.IpV4AddressWithMask b3 = new DhcpPolicyFilterOption.IpV4AddressWithMask(new IpV4Address("1.2.3.4"), new IpV4Address("255.255.255.0"));

            Assert.IsTrue(a.Equals(a2));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a.Equals(new object()));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(b2));
            Assert.IsFalse(a.Equals(b3));

            Assert.IsTrue(a == a2);
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsFalse(a == b);
            Assert.IsFalse(a == b2);
            Assert.IsFalse(a == b3);

            Assert.IsFalse(a != a2);
            Assert.IsTrue(a != null);
            Assert.IsTrue(null != a);
            Assert.IsTrue(a != b);
            Assert.IsTrue(a != b2);
            Assert.IsTrue(a != b3);
        }

        [TestMethod]
        public void DhcpPostOfficeProtocolServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpPostOfficeProtocolServerOption option = new DhcpPostOfficeProtocolServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.PostOfficeProtocolServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpPostOfficeProtocolServerOptionCtorNullTest()
        {
            new DhcpPostOfficeProtocolServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpPostOfficeProtocolServerOptionCtorEmptyTest()
        {
            new DhcpPostOfficeProtocolServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpPostOfficeProtocolServerOptionCtorToLongest()
        {
            new DhcpPostOfficeProtocolServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpRebindingTimeValueOptionTest()
        {
            DhcpRebindingTimeValueOption option = new DhcpRebindingTimeValueOption(4711);
            Assert.AreEqual(DhcpOptionCode.RebindingTimeValue, option.OptionCode);
            Assert.AreEqual<uint>(4711, option.T2Interval);

            option.T2Interval = 999;
            Assert.AreEqual<uint>(999, option.T2Interval);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpRenewalTimeValueOptionTest()
        {
            DhcpRenewalTimeValueOption option = new DhcpRenewalTimeValueOption(4711);
            Assert.AreEqual(DhcpOptionCode.RenewalTimeValue, option.OptionCode);
            Assert.AreEqual<uint>(4711, option.T1Interval);

            option.T1Interval = 999;
            Assert.AreEqual<uint>(999, option.T1Interval);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpRequestedIPAddressOptionTest()
        {
            DhcpRequestedIPAddressOption option = new DhcpRequestedIPAddressOption(new IpV4Address("10.20.30.40"));
            Assert.AreEqual(DhcpOptionCode.RequestedIPAddress, option.OptionCode);
            Assert.AreEqual(new IpV4Address("10.20.30.40"), option.Address);

            option.Address = new IpV4Address("1.2.3.4");
            Assert.AreEqual(new IpV4Address("1.2.3.4"), option.Address);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpResourceLocationServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpResourceLocationServerOption option = new DhcpResourceLocationServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.ResourceLocationServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpResourceLocationServerOptionCtorNullTest()
        {
            new DhcpResourceLocationServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpResourceLocationServerOptionCtorEmptyTest()
        {
            new DhcpResourceLocationServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpResourceLocationServerOptionCtorToLongest()
        {
            new DhcpResourceLocationServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpRootPathOptionTest()
        {
            DhcpRootPathOption option = new DhcpRootPathOption("Root Disk Pathname");
            Assert.AreEqual(DhcpOptionCode.RootPath, option.OptionCode);
            Assert.AreEqual("Root Disk Pathname", option.RootDiskPathname);

            option.RootDiskPathname = "changed";
            Assert.AreEqual("changed", option.RootDiskPathname);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpRootPathOptionCtorNullTest()
        {
            new DhcpRootPathOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpRootPathOptionCtorEmptryTest()
        {
            new DhcpRootPathOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpRootPathOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpRootPathOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpRouterOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpRouterOption option = new DhcpRouterOption(addresses);
            Assert.AreEqual(DhcpOptionCode.Router, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpRouterOptionCtorNullTest()
        {
            new DhcpRouterOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpRouterOptionCtorEmptyTest()
        {
            new DhcpRouterOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpRouterOptionCtorToLongest()
        {
            new DhcpRouterOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpRouterSolicitationAddressOptionTest()
        {
            DhcpRouterSolicitationAddressOption option = new DhcpRouterSolicitationAddressOption(new IpV4Address("10.20.30.40"));
            Assert.AreEqual(DhcpOptionCode.RouterSolicitationAddress, option.OptionCode);
            Assert.AreEqual(new IpV4Address("10.20.30.40"), option.Address);

            option.Address = new IpV4Address("1.2.3.4");
            Assert.AreEqual(new IpV4Address("1.2.3.4"), option.Address);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpServerIdentifierOptionTest()
        {
            DhcpServerIdentifierOption option = new DhcpServerIdentifierOption(new IpV4Address("10.20.30.40"));
            Assert.AreEqual(DhcpOptionCode.ServerIdentifier, option.OptionCode);
            Assert.AreEqual(new IpV4Address("10.20.30.40"), option.Address);

            option.Address = new IpV4Address("1.2.3.4");
            Assert.AreEqual(new IpV4Address("1.2.3.4"), option.Address);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpSimpleMailTransportProtocolServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpSimpleMailTransportProtocolServerOption option = new DhcpSimpleMailTransportProtocolServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.SimpleMailTransportProtocolServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpSimpleMailTransportProtocolServerOptionCtorNullTest()
        {
            new DhcpSimpleMailTransportProtocolServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpSimpleMailTransportProtocolServerOptionCtorEmptyTest()
        {
            new DhcpSimpleMailTransportProtocolServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpSimpleMailTransportProtocolServerOptionCtorToLongest()
        {
            new DhcpSimpleMailTransportProtocolServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpStaticRouteOptionTest()
        {
            DhcpStaticRouteOption.IpV4AddressRoute[] filters = new DhcpStaticRouteOption.IpV4AddressRoute[] { new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("10.20.30.40"), new IpV4Address("255.255.255.0")), new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("1.2.3.4"), new IpV4Address("222.0.0.0")) };
            DhcpStaticRouteOption option = new DhcpStaticRouteOption(filters);
            Assert.AreEqual(DhcpOptionCode.StaticRoute, option.OptionCode);
            CollectionAssert.AreEqual(filters, option.Routes.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpStaticRouteOptionCtorNullTest()
        {
            new DhcpStaticRouteOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpStaticRouteOptionCtorEmptyTest()
        {
            new DhcpStaticRouteOption(new DhcpStaticRouteOption.IpV4AddressRoute[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpStaticRouteOptionCtorToLongest()
        {
            new DhcpStaticRouteOption(new DhcpStaticRouteOption.IpV4AddressRoute[255 / DhcpStaticRouteOption.IpV4AddressRoute.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpStaticRouteOptionIpV4AddressRouteEqualsTest()
        {
            DhcpStaticRouteOption.IpV4AddressRoute a = new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("10.20.30.40"), new IpV4Address("255.255.255.0"));
            DhcpStaticRouteOption.IpV4AddressRoute a2 = new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("10.20.30.40"), new IpV4Address("255.255.255.0"));
            DhcpStaticRouteOption.IpV4AddressRoute b = new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("1.2.3.4"), new IpV4Address("222.0.0.0"));
            DhcpStaticRouteOption.IpV4AddressRoute b2 = new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("10.20.30.40"), new IpV4Address("222.0.0.0"));
            DhcpStaticRouteOption.IpV4AddressRoute b3 = new DhcpStaticRouteOption.IpV4AddressRoute(new IpV4Address("1.2.3.4"), new IpV4Address("255.255.255.0"));

            Assert.IsTrue(a.Equals(a2));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a.Equals(new object()));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(b2));
            Assert.IsFalse(a.Equals(b3));

            Assert.IsTrue(a == a2);
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsFalse(a == b);
            Assert.IsFalse(a == b2);
            Assert.IsFalse(a == b3);

            Assert.IsFalse(a != a2);
            Assert.IsTrue(a != null);
            Assert.IsTrue(null != a);
            Assert.IsTrue(a != b);
            Assert.IsTrue(a != b2);
            Assert.IsTrue(a != b3);
        }

        [TestMethod]
        public void DhcpStreetTalkDirectoryAssistanceServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpStreetTalkDirectoryAssistanceServerOption option = new DhcpStreetTalkDirectoryAssistanceServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.StreetTalkDirectoryAssistanceServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpStreetTalkDirectoryAssistanceServerOptionCtorNullTest()
        {
            new DhcpStreetTalkDirectoryAssistanceServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpStreetTalkDirectoryAssistanceServerOptionCtorEmptyTest()
        {
            new DhcpStreetTalkDirectoryAssistanceServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpStreetTalkDirectoryAssistanceServerOptionCtorToLongest()
        {
            new DhcpStreetTalkDirectoryAssistanceServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpStreetTalkServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpStreetTalkServerOption option = new DhcpStreetTalkServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.StreetTalkServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpStreetTalkServerOptionCtorNullTest()
        {
            new DhcpStreetTalkServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpStreetTalkServerOptionCtorEmptyTest()
        {
            new DhcpStreetTalkServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpStreetTalkServerOptionCtorToLongest()
        {
            new DhcpStreetTalkServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpSubnetMaskOptionTest()
        {
            DhcpSubnetMaskOption option = new DhcpSubnetMaskOption(new IpV4Address("10.20.30.40"));
            Assert.AreEqual(DhcpOptionCode.SubnetMask, option.OptionCode);
            Assert.AreEqual(new IpV4Address("10.20.30.40"), option.SubnetMask);

            option.SubnetMask = new IpV4Address("255.255.255.255");
            Assert.AreEqual(new IpV4Address("255.255.255.255"), option.SubnetMask);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpSwapServerOptionTest()
        {
            DhcpSwapServerOption option = new DhcpSwapServerOption(new IpV4Address("10.20.30.40"));
            Assert.AreEqual(DhcpOptionCode.SwapServer, option.OptionCode);
            Assert.AreEqual(new IpV4Address("10.20.30.40"), option.SwapServerAddress);

            option.SwapServerAddress = new IpV4Address("1.2.3.4");
            Assert.AreEqual(new IpV4Address("1.2.3.4"), option.SwapServerAddress);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpTcpDefaultTtlOptionTest()
        {
            DhcpTcpDefaultTtlOption option = new DhcpTcpDefaultTtlOption(100);
            Assert.AreEqual(DhcpOptionCode.TcpDefaultTtl, option.OptionCode);
            Assert.AreEqual<byte>(100, option.Ttl);

            option.Ttl = 42;
            Assert.AreEqual<byte>(42, option.Ttl);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpTcpKeepaliveGarbageOptionTest()
        {
            DhcpTcpKeepaliveGarbageOption option = new DhcpTcpKeepaliveGarbageOption(true);
            Assert.AreEqual(DhcpOptionCode.TcpKeepaliveGarbage, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpTcpKeepaliveGarbageOption(false);
            Assert.AreEqual(DhcpOptionCode.TcpKeepaliveGarbage, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpTcpKeepaliveIntervalOptionTest()
        {
            DhcpTcpKeepaliveIntervalOption option = new DhcpTcpKeepaliveIntervalOption(4711);
            Assert.AreEqual(DhcpOptionCode.TcpKeepaliveInterval, option.OptionCode);
            Assert.AreEqual<uint>(4711, option.Time);

            option.Time = 42;
            Assert.AreEqual<uint>(42, option.Time);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpTFtpServerNameOptionTest()
        {
            DhcpTFtpServerNameOption option = new DhcpTFtpServerNameOption("TFTP server");
            Assert.AreEqual(DhcpOptionCode.TfptServerName, option.OptionCode);
            Assert.AreEqual("TFTP server", option.TFtpServer);

            option.TFtpServer = "changed";
            Assert.AreEqual("changed", option.TFtpServer);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpTFtpServerNameOptionCtorNullTest()
        {
            new DhcpTFtpServerNameOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpTFtpServerNameOptionCtorEmptryTest()
        {
            new DhcpTFtpServerNameOption("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpTFtpServerNameOptionCtorToLongTest()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byte.MaxValue; i++)
                sb.Append('a');
            new DhcpTFtpServerNameOption(sb.ToString());
        }

        [TestMethod]
        public void DhcpTimeOffsetOptionTest()
        {
            DhcpTimeOffsetOption option = new DhcpTimeOffsetOption(4711);
            Assert.AreEqual(DhcpOptionCode.TimeOffset, option.OptionCode);
            Assert.AreEqual<int>(4711, option.TimeOffset);

            option.TimeOffset = -42;
            Assert.AreEqual<int>(-42, option.TimeOffset);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpTimeServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpTimeServerOption option = new DhcpTimeServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.TimeServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpTimeServerOptionCtorNullTest()
        {
            new DhcpTimeServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpTimeServerOptionCtorEmptyTest()
        {
            new DhcpTimeServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpTimeServerOptionCtorToLongest()
        {
            new DhcpTimeServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpTrailerEncapsulationOptionTest()
        {
            DhcpTrailerEncapsulationOption option = new DhcpTrailerEncapsulationOption(true);
            Assert.AreEqual(DhcpOptionCode.TrailerEncapsulation, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsTrue(option.Value);
            option.Value = false;
            Assert.IsFalse(option.Value);

            option = new DhcpTrailerEncapsulationOption(false);
            Assert.AreEqual(DhcpOptionCode.TrailerEncapsulation, option.OptionCode);
            Assert.AreEqual(1, option.Length);
            Assert.IsFalse(option.Value);

            TestOption(option);
        }

        [TestMethod]
        public void DhcpVendorClassidentifierOptionTest()
        {
            DataSegment data = new DataSegment(new byte[10]);
            DhcpVendorClassidentifierOption option = new DhcpVendorClassidentifierOption(data);
            Assert.AreEqual(DhcpOptionCode.VendorClassidentifier, option.OptionCode);
            Assert.AreEqual(10, option.Length);
            Assert.AreEqual(data, option.VendorClassIdentifier);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpVendorClassidentifierOptionCtorNullTest()
        {
            new DhcpVendorClassidentifierOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpVendorClassidentifierOptionCtorToLargeTest()
        {
            new DhcpVendorClassidentifierOption(new DataSegment(new byte[byte.MaxValue + 1]));
        }

        [TestMethod]
        public void DhcpVendorSpecificInformationOptionTest()
        {
            DataSegment data = new DataSegment(new byte[10]);
            DhcpVendorSpecificInformationOption option = new DhcpVendorSpecificInformationOption(data);
            Assert.AreEqual(DhcpOptionCode.VendorSpecificInformation, option.OptionCode);
            Assert.AreEqual(10, option.Length);
            Assert.AreEqual(data, option.VendorSpecificInformation);

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpVendorSpecificInformationOptionCtorNullTest()
        {
            new DhcpVendorSpecificInformationOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpVendorSpecificInformationOptionCtorToLargeTest()
        {
            new DhcpVendorSpecificInformationOption(new DataSegment(new byte[byte.MaxValue + 1]));
        }

        [TestMethod]
        public void DhcpXWindowSystemDisplayManagerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpXWindowSystemDisplayManagerOption option = new DhcpXWindowSystemDisplayManagerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.XWindowSystemDisplayManager, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpXWindowSystemDisplayManagerOptionCtorNullTest()
        {
            new DhcpXWindowSystemDisplayManagerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpXWindowSystemDisplayManagerOptionCtorEmptyTest()
        {
            new DhcpXWindowSystemDisplayManagerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpXWindowSystemDisplayManagerOptionCtorToLongest()
        {
            new DhcpXWindowSystemDisplayManagerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        [TestMethod]
        public void DhcpXWindowSystemFontServerOptionTest()
        {
            IpV4Address[] addresses = new IpV4Address[] { new IpV4Address("10.20.30.40"), new IpV4Address("11.22.33.44") };
            DhcpXWindowSystemFontServerOption option = new DhcpXWindowSystemFontServerOption(addresses);
            Assert.AreEqual(DhcpOptionCode.XWindowSystemFontServer, option.OptionCode);
            CollectionAssert.AreEqual(addresses, option.Addresses.ToArray());

            TestOption(option);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DhcpXWindowSystemFontServerOptionCtorNullTest()
        {
            new DhcpXWindowSystemFontServerOption(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpXWindowSystemFontServerOptionCtorEmptyTest()
        {
            new DhcpXWindowSystemFontServerOption(new IpV4Address[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DhcpXWindowSystemFontServerOptionCtorToLongest()
        {
            new DhcpXWindowSystemFontServerOption(new IpV4Address[255 / IpV4Address.SizeOf + 1]);
        }

        private void TestOption(DhcpOption option)
        {
            int seed = new Random().Next();
            Console.WriteLine("Seed-TestOption: " + seed);
            Random random = new Random(seed);

            EthernetLayer ethernetLayer = random.NextEthernetLayer(EthernetType.None);

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            UdpLayer udpLayer = random.NextUdpLayer();
            udpLayer.Checksum = null;

            DhcpLayer dhcpLayer = random.NextDhcpLayer();
            dhcpLayer.Options.Clear();
            dhcpLayer.Options.Add(option);

            Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, udpLayer, dhcpLayer);

            Assert.AreEqual(option, packet.Ethernet.IpV4.Udp.Dhcp.Options.Single());
            Assert.AreEqual(option.GetHashCode(), packet.Ethernet.IpV4.Udp.Dhcp.Options.Single().GetHashCode());
        }
    }
}