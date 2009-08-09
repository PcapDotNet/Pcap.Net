using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV4Tests
    /// </summary>
    [TestClass]
    public class IpV4Tests
    {
        public IpV4Tests()
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
        public void HttpTest()
        {
            Packet packet = HexToPacket(
                "feff200001000000010000000800" +
                "45000207" + // IPv4 Version, IHL, Type of Service, Total Length
                "0f454000" + // IPv4 Identification, Flags, Fragment Offset
                "80069010" + // IPv4 Time to Live, Protocol, Header Checksum
                "91fea0ed" + // IPv4 Source Address
                "41d0e4df" + // IPv4 Destination Address
                "0d2c005038affe14114c618c501825bca9580000474554202f646f776e6c6f61642e68746d6c20485454502f312e310d0a486f73743a207777772e657468657265616c2e636f6d0d0a557365722d4167656e743a204d6f7a696c6c612f352e30202857696e646f77733b20553b2057696e646f7773204e5420352e313b20656e2d55533b2072763a312e3629204765636b6f2f32303034303131330d0a4163636570743a20746578742f786d6c2c6170706c69636174696f6e2f786d6c2c6170706c69636174696f6e2f7868746d6c2b786d6c2c746578742f68746d6c3b713d302e392c746578742f706c61696e3b713d302e382c696d6167652f706e672c696d6167652f6a7065672c696d6167652f6769663b713d302e322c2a2f2a3b713d302e310d0a4163636570742d4c616e67756167653a20656e2d75732c656e3b713d302e350d0a4163636570742d456e636f64696e673a20677a69702c6465666c6174650d0a4163636570742d436861727365743a2049534f2d383835392d312c7574662d383b713d302e372c2a3b713d302e370d0a4b6565702d416c6976653a203330300d0a436f6e6e656374696f6e3a206b6565702d616c6976650d0a526566657265723a20687474703a2f2f7777772e657468657265616c2e636f6d2f646576656c6f706d656e742e68746d6c0d0a0d0a",
                DataLinkKind.Ethernet);

            Assert.IsTrue(packet.IsValid);

            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength, "PayloadLength");
            Assert.AreEqual(new MacAddress("00:00:01:00:00:00"), packet.Ethernet.Source, "Ethernet Source");
            Assert.AreEqual(new MacAddress("fe:ff:20:00:01:00"), packet.Ethernet.Destination, "Ethernet Destination");
            Assert.AreEqual(EthernetType.IpV4, packet.Ethernet.EtherType, "Ethernet Type");

            Assert.AreEqual(IpV4Datagram.HeaderMinimumLength, packet.Ethernet.IpV4.HeaderLength, "IP HeaderLength");
            Assert.AreEqual(0, packet.Ethernet.IpV4.TypeOfService, "IP TypeOfService");
            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.IpV4.TotalLength, "IP TotalLength");
            Assert.AreEqual(new IpV4Fragmentation(IpV4FragmentationOptions.DoNotFragment, 0), packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
            Assert.AreEqual(128, packet.Ethernet.IpV4.Ttl, "IP Ttl");
            Assert.AreEqual(IpV4Protocol.Tcp, packet.Ethernet.IpV4.Protocol, "IP Protocol");
            Assert.AreEqual(0x9010, packet.Ethernet.IpV4.HeaderChecksum, "IP HeaderChecksum");
            Assert.AreEqual(true, packet.Ethernet.IpV4.IsHeaderChecksumCorrect, "IP HeaderChecksumCorrect");
            Assert.AreEqual(new IpV4Address("145.254.160.237"), packet.Ethernet.IpV4.Source, "IP Source");
            Assert.AreEqual(new IpV4Address("65.208.228.223"), packet.Ethernet.IpV4.Destination, "IP Destination");
            Assert.AreEqual(IpV4Options.None, packet.Ethernet.IpV4.Options, "IP Options");
        }

        [TestMethod]
        public void RandomIpV4Test()
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");
            const EthernetType ethernetType = EthernetType.IpV4;

            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                byte ipV4TypeOfService = random.NextByte();
                ushort ipV4Identification = random.NextUShort();
                byte ipV4Ttl = random.NextByte();
                IpV4FragmentationOptions ipV4FragmentationOptions = random.NextEnum<IpV4FragmentationOptions>();
                ushort ipV4FragmentationOffset = (ushort)(random.NextUShort(ushort.MaxValue / 8) * 8);
                IpV4Fragmentation ipV4Fragmentation = new IpV4Fragmentation(ipV4FragmentationOptions, ipV4FragmentationOffset);
                IpV4Protocol ipV4Protocol = random.NextEnum<IpV4Protocol>();
                IpV4Address ipV4Source = new IpV4Address(random.NextUInt());
                IpV4Address ipV4Destination = new IpV4Address(random.NextUInt());
                IpV4Options ipV4Options = random.NextIpV4Options();

                byte[] ipV4PayloadBuffer = new byte[random.Next(0, 50 * 1024)];
                random.NextBytes(ipV4PayloadBuffer);
                Datagram ipV4Payload = new Datagram(ipV4PayloadBuffer);

                Packet packet = PacketBuilder.EthernetIpV4(DateTime.Now,
                                                           ethernetSource, ethernetDestination,
                                                           ipV4TypeOfService, ipV4Identification, ipV4Fragmentation, ipV4Ttl, ipV4Protocol,
                                                           ipV4Source, ipV4Destination, ipV4Options,
                                                           ipV4Payload);


                Assert.IsTrue(ipV4Protocol == IpV4Protocol.Udp || packet.IsValid, "IsValid");

                // Ethernet
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetSource, packet.Ethernet.Source, "Ethernet Source");
                Assert.AreEqual(ethernetDestination, packet.Ethernet.Destination, "Ethernet Destination");
                Assert.AreEqual(ethernetType, packet.Ethernet.EtherType, "Ethernet Type");

                // IpV4
                Assert.AreEqual(IpV4Datagram.HeaderMinimumLength + ipV4Options.BytesLength, packet.Ethernet.IpV4.HeaderLength, "IP HeaderLength");
                Assert.AreEqual(ipV4TypeOfService, packet.Ethernet.IpV4.TypeOfService, "IP TypeOfService");
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLength, packet.Ethernet.IpV4.TotalLength, "IP TotalLength");
                Assert.AreEqual(ipV4Identification, packet.Ethernet.IpV4.Identification, "IP Identification");
                Assert.AreEqual(ipV4Fragmentation, packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
                Assert.IsTrue(ipV4Fragmentation == packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
                Assert.IsFalse(ipV4Fragmentation != packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
                Assert.AreEqual(ipV4Fragmentation.GetHashCode(), packet.Ethernet.IpV4.Fragmentation.GetHashCode(), "IP Fragmentation");
                Assert.AreEqual(ipV4Fragmentation.Options, packet.Ethernet.IpV4.Fragmentation.Options, "IP Fragmentation");
                Assert.AreEqual(ipV4Fragmentation.Offset, packet.Ethernet.IpV4.Fragmentation.Offset, "IP Fragmentation");
                if (ipV4Fragmentation.Equals(IpV4Fragmentation.None))
                {
                    Assert.AreEqual(IpV4FragmentationOptions.None, packet.Ethernet.IpV4.Fragmentation.Options, "IP Fragmentation");
                    Assert.AreEqual(0, packet.Ethernet.IpV4.Fragmentation.Offset, "IP Fragmentation");
                }
                Assert.AreEqual(ipV4Ttl, packet.Ethernet.IpV4.Ttl, "IP Ttl");
                Assert.AreEqual(ipV4Protocol, packet.Ethernet.IpV4.Protocol, "IP Protocol");
//                Assert.AreEqual(0x9010, packet.Ethernet.IpV4.HeaderChecksum, "IP HeaderChecksum");
                Assert.AreEqual(true, packet.Ethernet.IpV4.IsHeaderChecksumCorrect, "IP HeaderChecksumCorrect");
                Assert.AreEqual(ipV4Source, packet.Ethernet.IpV4.Source, "IP Source");
                Assert.AreEqual(ipV4Destination, packet.Ethernet.IpV4.Destination, "IP Destination");
                Assert.AreEqual(ipV4Options, packet.Ethernet.IpV4.Options, "IP Options");
                Assert.AreEqual(ipV4Options.GetHashCode(), packet.Ethernet.IpV4.Options.GetHashCode(), "IP Options HashCode");
                Assert.IsNotNull(packet.Ethernet.IpV4.Options.ToString());
                for (int optionIndex = 0; optionIndex != ipV4Options.Count; ++optionIndex)
                    Assert.AreEqual(ipV4Options[optionIndex], packet.Ethernet.IpV4.Options[optionIndex]);

                Assert.AreEqual(ipV4Payload, packet.Ethernet.IpV4.Payload, "IP Payload");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IpV4OptionTimestampOverflowErrorTest()
        {
            Random random = new Random();
            IpV4Option option = new IpV4OptionTimestampOnly(random.NextByte(IpV4OptionTimestamp.OverflowMaxValue + 1, byte.MaxValue + 1), 0);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IpV4OptionTimestampPointedIndexErrorTest()
        {
            Random random = new Random();
            IpV4Option option = new IpV4OptionTimestampOnly(0, random.NextByte(IpV4OptionTimestamp.PointedIndexMaxValue + 1, byte.MaxValue + 1));
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IpV4OptionRoutePointedAddressIndexErrorTest()
        {
            Random random = new Random();
            IpV4Option option = new IpV4OptionRecordRoute(random.NextByte(IpV4OptionRecordRoute.PointedAddressIndexMaxValue + 1, byte.MaxValue + 1));
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4OptionsTooLongErrorTest()
        {
            IpV4Options options = new IpV4Options(
                new IpV4OptionTimestampOnly(1, 2,
                                            new IpV4OptionTimeOfDay(3), new IpV4OptionTimeOfDay(4), new IpV4OptionTimeOfDay(5),
                                            new IpV4OptionTimeOfDay(6), new IpV4OptionTimeOfDay(7), new IpV4OptionTimeOfDay(8),
                                            new IpV4OptionTimeOfDay(9), new IpV4OptionTimeOfDay(10), new IpV4OptionTimeOfDay(11),
                                            new IpV4OptionTimeOfDay(12), new IpV4OptionTimeOfDay(13), new IpV4OptionTimeOfDay(14),
                                            new IpV4OptionTimeOfDay(15), new IpV4OptionTimeOfDay(16), new IpV4OptionTimeOfDay(17)));
            Assert.IsNotNull(options);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4FragmentationOffsetErrorTest()
        {
            IpV4Fragmentation fragmentation = new IpV4Fragmentation(IpV4FragmentationOptions.None, 2);
            Assert.IsNotNull(fragmentation);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4FragmentationOptionsErrorTest()
        {
            IpV4Fragmentation fragmentation = new IpV4Fragmentation((IpV4FragmentationOptions)12345, 8);
            Assert.IsNotNull(fragmentation);
            Assert.Fail();
        }

        [TestMethod]
        public void IpV4OptionTimeOfDayTest()
        {
            TimeSpan timeOfDay = new TimeSpan(1, 2, 3);
            IpV4OptionTimeOfDay timeSinceMidnight = new IpV4OptionTimeOfDay(timeOfDay);
            Assert.AreEqual(timeOfDay, timeSinceMidnight.TimeSinceMidnightUniversalTime);
            Assert.AreEqual<object>(timeSinceMidnight, timeSinceMidnight);
            Assert.IsTrue(timeSinceMidnight == timeSinceMidnight);
            Assert.IsFalse(timeSinceMidnight != timeSinceMidnight);
        }

        [TestMethod]
        public void IpV4OptionTimedAddressTest()
        {
            IpV4OptionTimedAddress timedAddress1 = new IpV4OptionTimedAddress(new IpV4Address("1.2.3.4"), new IpV4OptionTimeOfDay(new TimeSpan(1, 2, 3)));
            IpV4OptionTimedAddress timedAddress2 = new IpV4OptionTimedAddress(new IpV4Address("1.2.3.4"), new IpV4OptionTimeOfDay(new TimeSpan(1, 2, 3)));

            Assert.AreEqual(timedAddress1, timedAddress2);
            Assert.IsTrue(timedAddress1 == timedAddress2);
            Assert.IsFalse(timedAddress1 != timedAddress2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IpV4OptionBasicSecurtiyBadLengthTest()
        {
            IpV4OptionBasicSecurity option = new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.Secret, IpV4OptionSecurityProtectionAuthorities.None, 1);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4OptionBasicSecurtiyNoProtectionAuthoritiesButWithValueTest()
        {
            IpV4OptionBasicSecurity option = new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.Secret, IpV4OptionSecurityProtectionAuthorities.Nsa, 3);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4OptionQuickStartBadFunctionTest()
        {
            IpV4OptionQuickStart option = new IpV4OptionQuickStart((IpV4OptionQuickStartFunction)2, 1, 2, 16);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IpV4OptionQuickStartBadRateTest()
        {
            IpV4OptionQuickStart option = new IpV4OptionQuickStart(IpV4OptionQuickStartFunction.RateRequest, 100, 1, 32);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4OptionQuickStartBadNonceTest()
        {
            IpV4OptionQuickStart option = new IpV4OptionQuickStart(IpV4OptionQuickStartFunction.RateRequest, 1, 1, 2);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV4OptionTimestampAndAddressBadTypeTest()
        {
            IpV4OptionTimestampAndAddress option = new IpV4OptionTimestampAndAddress((IpV4OptionTimestampType)166, 1, 2);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        public void IpV4DatagramInvalidShortTest()
        {
            Packet packet = PacketBuilder.EthernetIpV4(DateTime.Now,
                                                       new MacAddress(1), new MacAddress(2),
                                                       0, 1, new IpV4Fragmentation(IpV4FragmentationOptions.MoreFragments, 8), 1,
                                                       IpV4Protocol.WidebandExpak, new IpV4Address(1), new IpV4Address(2), new IpV4Options(),
                                                       Datagram.Empty);
            Assert.IsTrue(packet.IsValid);

            byte[] badPacketBuffer = new byte[packet.Length - 5];
            packet.Buffer.BlockCopy(0, badPacketBuffer, 0, badPacketBuffer.Length);
            Packet badPacket = new Packet(badPacketBuffer, DateTime.Now, packet.DataLink);
            Assert.IsFalse(badPacket.IsValid, "badPacket.IsValid");
        }

        [TestMethod]
        public void IpV4BadChecksumTest()
        {
            Packet packet =
                HexToPacket(
                    "14f50eac02b964f8ce8b889908004a800061088400007b11fffb9434d5228511ae3b88044b379404429319088f008b13b7f8000000002cea09e70039a38af80cf279bad8b22df584ea50177467c686df799957540db1ab0015140be458517507e456d123f14680b9f1f36b834d908c",
                    DataLinkKind.Ethernet);

            Assert.IsTrue(packet.Ethernet.IpV4.IsHeaderChecksumCorrect);
        }

        private static Packet HexToPacket(string hexString, DataLinkKind dataLinkKind)
        {
            return Packet.FromHexadecimalString(hexString, DateTime.MinValue, dataLinkKind);
        }
    }
}