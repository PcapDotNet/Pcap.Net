using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV4Tests
    /// </summary>
    [TestClass]
    public class IpV4Tests
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
        public void IpV4HttpTest()
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

            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.PayloadLength, "PayloadLength");
            Assert.AreEqual(new MacAddress("00:00:01:00:00:00"), packet.Ethernet.Source, "Ethernet Source");
            Assert.AreEqual(new MacAddress("fe:ff:20:00:01:00"), packet.Ethernet.Destination, "Ethernet Destination");
            Assert.AreEqual(EthernetType.IpV4, packet.Ethernet.EtherType, "Ethernet Type");

            Assert.AreEqual(IpV4Datagram.HeaderMinimumLength, packet.Ethernet.IpV4.HeaderLength, "IP HeaderLength");
            Assert.AreEqual(0, packet.Ethernet.IpV4.TypeOfService, "IP TypeOfService");
            Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.IpV4.TotalLength, "IP TotalLength");
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
            const EthernetType EthernetType = EthernetType.IpV4;

            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = ethernetSource,
                                                  Destination = ethernetDestination,
                                                  EtherType = EthernetType
                                              };

            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                IpV4Layer ipV4Layer = random.NextIpV4Layer();
                ipV4Layer.HeaderChecksum = null;

                PayloadLayer payloadLayer = random.NextPayloadLayer(random.NextInt(0, 50 * 1024));

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, payloadLayer);

                Assert.IsTrue(ipV4Layer.Protocol == IpV4Protocol.Ip ||
                              ipV4Layer.Protocol == IpV4Protocol.IpV6 ||
                              ipV4Layer.Protocol == IpV4Protocol.Udp ||
                              ipV4Layer.Protocol == IpV4Protocol.Tcp ||
                              ipV4Layer.Protocol == IpV4Protocol.InternetGroupManagementProtocol ||
                              ipV4Layer.Protocol == IpV4Protocol.InternetControlMessageProtocol ||
                              ipV4Layer.Protocol == IpV4Protocol.Gre ||
                              packet.IsValid, "IsValid (" + ipV4Layer.Protocol + ")");

                // Ethernet
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IpV4
                ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer(), "IP Layer");
                Assert.AreEqual(ipV4Layer.Destination, packet.Ethernet.IpV4.Destination, "Destination");
                Assert.AreNotEqual(ipV4Layer, null);
                Assert.AreNotEqual(ipV4Layer, new PayloadLayer());
                Assert.IsNotNull(ipV4Layer.ToString());
                Assert.AreEqual(IpV4Datagram.HeaderMinimumLength + ipV4Layer.Options.BytesLength, packet.Ethernet.IpV4.HeaderLength, "IP HeaderLength");
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.IpV4.TotalLength, "IP TotalLength");
                Assert.AreNotEqual(2, packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
                Assert.IsTrue(ipV4Layer.Fragmentation == packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
                Assert.IsFalse(ipV4Layer.Fragmentation != packet.Ethernet.IpV4.Fragmentation, "IP Fragmentation");
                Assert.IsFalse(ipV4Layer.Fragmentation.Equals(0), "IP Fragmentation");
                if (ipV4Layer.Fragmentation.Offset != 0)
                    Assert.AreNotEqual(ipV4Layer.Fragmentation, IpV4Fragmentation.None, "IP Fragmentation");
                Assert.AreEqual(ipV4Layer.Fragmentation.GetHashCode(), packet.Ethernet.IpV4.Fragmentation.GetHashCode(), "IP Fragmentation");
                Assert.AreEqual(ipV4Layer.Fragmentation.Options, packet.Ethernet.IpV4.Fragmentation.Options, "IP Fragmentation");
                Assert.AreEqual(ipV4Layer.Fragmentation.Offset, packet.Ethernet.IpV4.Fragmentation.Offset, "IP Fragmentation");
                if (ipV4Layer.Fragmentation.Equals(IpV4Fragmentation.None))
                {
                    Assert.AreEqual(IpV4FragmentationOptions.None, packet.Ethernet.IpV4.Fragmentation.Options, "IP Fragmentation");
                    Assert.AreEqual(0, packet.Ethernet.IpV4.Fragmentation.Offset, "IP Fragmentation");
                }
                Assert.AreEqual(true, packet.Ethernet.IpV4.IsHeaderChecksumCorrect, "IP HeaderChecksumCorrect");
                Assert.AreNotEqual(null, packet.Ethernet.IpV4.Options, "IP Options");
                Assert.AreNotEqual(packet.Ethernet.IpV4.Options, null, "IP Options");
                Assert.AreNotEqual(new IpV4Options(new IpV4OptionUnknown(0, new byte[35])), packet.Ethernet.IpV4.Options, "IP Options");
                Assert.AreEqual(ipV4Layer.Options.GetHashCode(), packet.Ethernet.IpV4.Options.GetHashCode(), "IP Options HashCode");
                Assert.IsNotNull(packet.Ethernet.IpV4.Options.ToString());
                for (int optionIndex = 0; optionIndex != ipV4Layer.Options.Count; ++optionIndex)
                {
                    IpV4Option option = ipV4Layer.Options[optionIndex];
                    Assert.AreEqual(option, packet.Ethernet.IpV4.Options[optionIndex]);
                    Assert.IsFalse(option.Equals(null));
                }

                if (packet.Ethernet.IpV4.Protocol == IpV4Protocol.Tcp)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV4.Transport, typeof(TcpDatagram));
                else if (packet.Ethernet.IpV4.Protocol == IpV4Protocol.Udp)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV4.Transport, typeof(UdpDatagram));
                else 
                    Assert.IsNull(packet.Ethernet.IpV4.Transport);

                if (packet.Ethernet.IpV4.Protocol == IpV4Protocol.Ip)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV4.Ip, typeof(IpV4Datagram));
                else if (packet.Ethernet.IpV4.Protocol == IpV4Protocol.IpV6)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV4.Ip, typeof(IpV6Datagram));
                else
                    Assert.IsNull(packet.Ethernet.IpV4.Ip);

                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.IpV4.Payload, "IP Payload");
            }
        }

        [TestMethod]
        public void IpV4NullOptionsTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new IpV4Layer
                                                {
                                                    CurrentDestination = new IpV4Address("2.3.4.5"),
                                                    Options = new IpV4Options(new IpV4OptionStrictSourceRouting(new[] {new IpV4Address("1.2.3.4")}, 0)),
                                                    Protocol = IpV4Protocol.Emcon,
                                                });

            Assert.IsTrue(packet.IsValid);
            Assert.IsNotNull(packet.IpV4.Options);
            Assert.AreEqual(new IpV4Address("1.2.3.4"), packet.IpV4.Destination);

            const int newPacketLength = IpV4Datagram.HeaderMinimumLength - 1;
            byte[] newPacketBuffer = new byte[newPacketLength];
            packet.Take(newPacketLength).ToArray().CopyTo(newPacketBuffer, 0);

            Packet newPacket = new Packet(newPacketBuffer, DateTime.Now, DataLinkKind.IpV4);
            Assert.IsFalse(newPacket.IsValid);
            Assert.IsNull(newPacket.IpV4.Options);
            Assert.AreNotEqual(new IpV4Address("1.2.3.4"), newPacket.IpV4.Destination);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV4OptionTimestampOverflowErrorTest()
        {
            Random random = new Random();
            IpV4Option option = new IpV4OptionTimestampOnly(random.NextByte(IpV4OptionTimestamp.OverflowMaxValue + 1, byte.MaxValue + 1), 0);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV4OptionTimestampPointedIndexErrorTest()
        {
            Random random = new Random();
            IpV4Option option = new IpV4OptionTimestampOnly(0, random.NextByte(IpV4OptionTimestamp.PointedIndexMaxValue + 1, byte.MaxValue + 1));
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        public void IpV4OptionTimestampFactoryCreateInstanceErrorTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                 new EthernetLayer(),
                                                 new IpV4Layer
                                                     {
                                                         Protocol = IpV4Protocol.Argus,
                                                         Options = new IpV4Options(new IpV4OptionTimestampOnly(0, 0)),
                                                     });

            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            // Bad Length
            byte[] buffer = packet.Buffer;
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 2;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 4;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 5;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 4;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            // Bad Pointer
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 1;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 5;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 6;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV4OptionRoutePointedAddressIndexErrorTest()
        {
            Random random = new Random();
            IpV4Option option = new IpV4OptionRecordRoute(random.NextByte(IpV4OptionRoute.PointedAddressIndexMaxValue + 1, byte.MaxValue + 1));
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        public void IpV4OptionRouteTryReadErrorTest()
        {
            // Small Length
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                 new EthernetLayer(),
                                                 new IpV4Layer
                                                     {
                                                         Protocol = 0,
                                                         Options = new IpV4Options(new IpV4OptionLooseSourceRouting())
                                                     });

            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);
            byte[] buffer = packet.Buffer;
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 2;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 3;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            // Bad Length
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 4;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 3;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            // Bad Pointer
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 0;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 4;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 5;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4OptionsTooLongErrorTest()
        {
            IpV4Options options = new IpV4Options(
                new IpV4OptionTimestampOnly(1, 2,
                                            new IpV4TimeOfDay(3), new IpV4TimeOfDay(4), new IpV4TimeOfDay(5),
                                            new IpV4TimeOfDay(6), new IpV4TimeOfDay(7), new IpV4TimeOfDay(8),
                                            new IpV4TimeOfDay(9), new IpV4TimeOfDay(10), new IpV4TimeOfDay(11),
                                            new IpV4TimeOfDay(12), new IpV4TimeOfDay(13), new IpV4TimeOfDay(14),
                                            new IpV4TimeOfDay(15), new IpV4TimeOfDay(16), new IpV4TimeOfDay(17)));
            Assert.IsNotNull(options);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4FragmentationOffsetErrorTest()
        {
            IpV4Fragmentation fragmentation = new IpV4Fragmentation(IpV4FragmentationOptions.None, 2);
            Assert.IsNotNull(fragmentation);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
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
            IpV4TimeOfDay timeSinceMidnight = new IpV4TimeOfDay(timeOfDay);
            Assert.AreEqual(timeOfDay, timeSinceMidnight.TimeSinceMidnightUniversalTime);
            Assert.AreEqual<object>(timeSinceMidnight, timeSinceMidnight);
            // ReSharper disable EqualExpressionComparison
            Assert.IsTrue(timeSinceMidnight == timeSinceMidnight);
            Assert.IsFalse(timeSinceMidnight != timeSinceMidnight);
            // ReSharper restore EqualExpressionComparison
        }

        [TestMethod]
        public void IpV4OptionTimedAddressTest()
        {
            IpV4OptionTimedAddress timedAddress1 = new IpV4OptionTimedAddress(new IpV4Address("1.2.3.4"), new IpV4TimeOfDay(new TimeSpan(1, 2, 3)));
            IpV4OptionTimedAddress timedAddress2 = new IpV4OptionTimedAddress(new IpV4Address("1.2.3.4"), new IpV4TimeOfDay(new TimeSpan(1, 2, 3)));

            Assert.AreEqual(timedAddress1, timedAddress2);
            Assert.IsTrue(timedAddress1 == timedAddress2);
            Assert.IsFalse(timedAddress1 != timedAddress2);
            Assert.IsFalse(timedAddress1.Equals(0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV4OptionBasicSecurityConstructorBadLengthTest()
        {
            IpV4OptionBasicSecurity option = new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.Secret, IpV4OptionSecurityProtectionAuthorities.None, 1);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4OptionBasicSecurityConstructorNoProtectionAuthoritiesButWithValueTest()
        {
            IpV4OptionBasicSecurity option = new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.Secret, IpV4OptionSecurityProtectionAuthorities.Nsa, 3);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4OptionBasicSecurityConstructorBadClassificationLevelTest()
        {
            IpV4OptionBasicSecurity option = new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.None);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        public void IpV4OptionBasicSecurityCreateInstanceErrorTest()
        {
            // Invalid Length
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                     new EthernetLayer(),
                                     new IpV4Layer
                                     {
                                         Protocol = 0,
                                         Options = new IpV4Options(new IpV4OptionBasicSecurity()),
                                     });

            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);
            byte[] buffer = packet.Buffer;
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 1] = 2;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            // Invalid classification level
            packet = PacketBuilder.Build(DateTime.Now,
                         new EthernetLayer(),
                         new IpV4Layer
                         {
                             Protocol = 0,
                             Options = new IpV4Options(new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.Secret)),
                         });

            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);
            buffer = packet.Buffer;
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 2] = 0;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            // Invalid protection authorities bytes
            packet = PacketBuilder.Build(DateTime.Now,
                         new EthernetLayer(),
                         new IpV4Layer
                         {
                             Protocol = 0,
                             Options = new IpV4Options(new IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel.Confidential, IpV4OptionSecurityProtectionAuthorities.Nsa, 5)),
                         });

            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);
            buffer = packet.Buffer;
            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 3] = 0;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 3] = 1;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsTrue(packet.Ethernet.IpV4.Options.IsValid);

            buffer[packet.Length - packet.Ethernet.IpV4.Length + IpV4Datagram.HeaderMinimumLength + 4] = 1;
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);
            Assert.IsFalse(packet.Ethernet.IpV4.Options.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4OptionQuickStartBadFunctionTest()
        {
            IpV4OptionQuickStart option = new IpV4OptionQuickStart((IpV4OptionQuickStartFunction)2, 1, 2, 16);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV4OptionQuickStartBadRateTest()
        {
            IpV4OptionQuickStart option = new IpV4OptionQuickStart(IpV4OptionQuickStartFunction.RateRequest, 100, 1, 32);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4OptionQuickStartBadNonceTest()
        {
            IpV4OptionQuickStart option = new IpV4OptionQuickStart(IpV4OptionQuickStartFunction.RateRequest, 1, 1, 2);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4OptionTimestampAndAddressBadTypeTest()
        {
            IpV4OptionTimestampAndAddress option = new IpV4OptionTimestampAndAddress((IpV4OptionTimestampType)166, 1, 2);
            Assert.IsNotNull(option);
            Assert.Fail();
        }

        [TestMethod]
        public void IpV4DatagramInvalidShortTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                         new EthernetLayer
                             {
                                 Source = new MacAddress(1),
                                 Destination = new MacAddress(2),
                             },
                         new IpV4Layer
                         {
                             Identification = 1,
                             Fragmentation = new IpV4Fragmentation(IpV4FragmentationOptions.MoreFragments, 8),
                             Ttl = 1,
                             Protocol = IpV4Protocol.WidebandExpak,
                             Source = new IpV4Address(1),
                             CurrentDestination = new IpV4Address(2),
                         });

            Assert.IsTrue(packet.IsValid);

            byte[] badPacketBuffer = new byte[packet.Length - 5];
            packet.Buffer.BlockCopy(0, badPacketBuffer, 0, badPacketBuffer.Length);
            Packet badPacket = new Packet(badPacketBuffer, DateTime.Now, packet.DataLink);
            Assert.IsFalse(badPacket.IsValid, "badPacket.IsValid");
        }

        [TestMethod]
        public void IpV4DatagramInvalidHeaderChecksumTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer
                                                                                        {
                                                                                            Protocol = 0,
                                                                                        });

            Assert.IsTrue(packet.IsValid);

            byte[] buffer = packet.Buffer;
            ++buffer[packet.Length - packet.Ethernet.IpV4.Length + 1];
            packet = new Packet(buffer, packet.Timestamp, packet.DataLink);

            Assert.IsFalse(packet.IsValid);
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

        [TestMethod]
        public void IpV4BrokenOptionTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(),
                                                new IpV4Layer
                                                    {
                                                        Options = new IpV4Options(IpV4Option.Nop, IpV4Option.Nop,
                                                                                  IpV4Option.Nop, IpV4Option.Nop),
                                                        Protocol = IpV4Protocol.Tcp
                                                    });

            byte[] buffer = (byte[])packet.Buffer.Clone();
            buffer[buffer.Length - 1] = (byte)IpV4OptionType.BasicSecurity;
            packet = new Packet(buffer, DateTime.Now, packet.DataLink);
            Assert.IsFalse(packet.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4NoNextLayerTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(),
                                                new IpV4Layer());
            Assert.IsNull(packet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV4BadNextLayerTest()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(),
                                                new IpV4Layer(),
                                                new PayloadLayer());
            Assert.IsNull(packet);
        }

        [TestMethod]
        public void IpV4LayerNullChecksumAndProtocolGetHashCodeTest()
        {
            IpV4Layer layer = new IpV4Layer
                                  {
                                      HeaderChecksum = null,
                                      Protocol = null,
                                  };
            Assert.IsNotNull(layer.GetHashCode());
        }

        [TestMethod]
        public void IpV4PayloadNotTcp()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(),
                                                new IpV4Layer()
                                                {
                                                    Protocol = IpV4Protocol.InternetControlMessageProtocol
                                                },
                                                new PayloadLayer()
                                                {
                                                    Data = new Datagram(new byte[5])
                                                });

            packet = PacketBuilder.Build(DateTime.Now,
                                         packet.Ethernet.ExtractLayer(),
                                         packet.Ethernet.IpV4.ExtractLayer(),
                                         packet.Ethernet.IpV4.Payload.ExtractLayer());

            Assert.IsNotNull(packet);
        }

        private static Packet HexToPacket(string hexString, DataLinkKind dataLinkKind)
        {
            return Packet.FromHexadecimalString(hexString, DateTime.MinValue, dataLinkKind);
        }
    }
}