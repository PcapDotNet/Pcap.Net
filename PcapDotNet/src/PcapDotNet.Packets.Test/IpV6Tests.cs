using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV6Tests
    /// </summary>
    [TestClass]
    public class IpV6Tests
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
        public void RandomIpV6Test()
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");
            const EthernetType EthernetType = EthernetType.IpV6;

            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = ethernetSource,
                                                  Destination = ethernetDestination,
                                                  EtherType = EthernetType
                                              };

            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                IpV6Layer ipV6Layer = random.NextIpV6Layer();

                PayloadLayer payloadLayer = random.NextPayloadLayer(random.NextInt(0, 50 * 1024));

                List<ILayer> layers = new List<ILayer> {ethernetLayer, ipV6Layer};
                if (ipV6Layer.ExtensionHeaders.LastHeader != IpV4Protocol.EncapsulatingSecurityPayload)
                    layers.Add(payloadLayer);
                Packet packet = PacketBuilder.Build(DateTime.Now, layers);

                Assert.IsTrue(packet.IsValid, string.Format("IsValid ({0}...{1})", ipV6Layer.NextHeader, ipV6Layer.ExtensionHeaders.NextHeader));

                // Ethernet
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IpV6
                Assert.AreEqual(ipV6Layer, packet.Ethernet.IpV6.ExtractLayer(), "IP Layer");
                Assert.IsNotNull(ipV6Layer.GetHashCode());
                Assert.AreEqual(string.Format("{0} -> {1} ({2})", ipV6Layer.Source, ipV6Layer.CurrentDestination, ipV6Layer.NextHeader), ipV6Layer.ToString());
                foreach (IpV6ExtensionHeader extensionHeader in packet.Ethernet.IpV6.ExtensionHeaders)
                {
                    IpV6ExtensionHeaderMobility extensionHeaderMobility = extensionHeader as IpV6ExtensionHeaderMobility;
                    if (extensionHeaderMobility != null)
                    {
                        Assert.IsFalse(extensionHeaderMobility.Equals(2));
                        Assert.IsTrue(extensionHeaderMobility.Equals((object)extensionHeader));
                        foreach (IpV6MobilityOption option in extensionHeaderMobility.MobilityOptions)
                        {
                            switch (option.OptionType)
                            {
                                case IpV6MobilityOptionType.BindingIdentifier:
                                    IpV6MobilityOptionBindingIdentifier optionBindingIdentifier = (IpV6MobilityOptionBindingIdentifier)option;
                                    if (optionBindingIdentifier.IpV4CareOfAddress.HasValue)
                                        Assert.AreEqual(optionBindingIdentifier.IpV4CareOfAddress.Value, optionBindingIdentifier.CareOfAddress);
                                    else if (optionBindingIdentifier.IpV6CareOfAddress.HasValue)
                                        Assert.AreEqual(optionBindingIdentifier.IpV6CareOfAddress.Value, optionBindingIdentifier.CareOfAddress);
                                    else
                                        Assert.IsNull(optionBindingIdentifier.CareOfAddress);
                                    break;

                                case IpV6MobilityOptionType.AccessNetworkIdentifier:
                                    IpV6MobilityOptionAccessNetworkIdentifier optionAccessNetworkIdentifier = (IpV6MobilityOptionAccessNetworkIdentifier)option;
                                    foreach (IpV6AccessNetworkIdentifierSubOption subOption in optionAccessNetworkIdentifier.SubOptions)
                                    {
                                        switch (subOption.OptionType)
                                        {
                                            case IpV6AccessNetworkIdentifierSubOptionType.GeoLocation:
                                                IpV6AccessNetworkIdentifierSubOptionGeoLocation subOptionGeoLocation = (IpV6AccessNetworkIdentifierSubOptionGeoLocation)subOption;
                                                MoreAssert.IsBiggerOrEqual(-90, subOptionGeoLocation.LatitudeDegreesReal);
                                                MoreAssert.IsSmallerOrEqual(90, subOptionGeoLocation.LatitudeDegreesReal);
                                                MoreAssert.IsBiggerOrEqual(-180, subOptionGeoLocation.LongitudeDegreesReal);
                                                MoreAssert.IsSmallerOrEqual(180, subOptionGeoLocation.LongitudeDegreesReal);
                                                IpV6AccessNetworkIdentifierSubOptionGeoLocation subOptionGetLocationFromReal =
                                                    IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues(
                                                        subOptionGeoLocation.LatitudeDegreesReal,
                                                        subOptionGeoLocation.LongitudeDegreesReal);
                                                Assert.AreEqual(subOptionGeoLocation, subOptionGetLocationFromReal);
                                                break;
                                        }
                                    }
                                    break;

                                case IpV6MobilityOptionType.Timestamp:
                                    IpV6MobilityOptionTimestamp optionTimestamp = (IpV6MobilityOptionTimestamp)option;
                                    MoreAssert.IsBiggerOrEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), optionTimestamp.TimestampDateTime);
                                    break;
                            }
                        }
                    }
                }
                /*
                if (packet.Ethernet.IpV6.NextHeader == IpV4Protocol.Tcp)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV6.Transport, typeof(TcpDatagram));
                else if (packet.Ethernet.IpV6.NextHeader == IpV4Protocol.Udp)
                    Assert.IsInstanceOfType(packet.Ethernet.IpV6.Transport, typeof(UdpDatagram));
                else
                    Assert.IsNull(packet.Ethernet.IpV6.Transport);
                */
//                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.IpV6.Payload, "IP Payload");
            }
        }

        [TestMethod]
        public void AutomaticIpV6NextHeader()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV6Layer(), new UdpLayer());
            Assert.AreEqual(IpV4Protocol.Udp, packet.Ethernet.IpV6.NextHeader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticIpV6NextHeaderNoNextLayer()
        {
            PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV6Layer());
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticIpV6NextHeaderUnknownNextLayer()
        {
            PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV6Layer(), new PayloadLayer());
            Assert.Fail();
        }

        [TestMethod]
        public void IpV6OptionCalipsoChecksum()
        {
            Packet packet = PacketBuilder.Build(
                DateTime.Now,
                new EthernetLayer(),
                new IpV6Layer
                    {
                        ExtensionHeaders = new IpV6ExtensionHeaders(
                            new IpV6ExtensionHeaderDestinationOptions(
                                IpV4Protocol.Udp,
                                new IpV6Options(new IpV6OptionCalipso(IpV6CalipsoDomainOfInterpretation.Null, 0, null, DataSegment.Empty)))),
                    },
                new UdpLayer());
            Assert.IsTrue(packet.IsValid);
            Assert.IsTrue(((IpV6OptionCalipso)((IpV6ExtensionHeaderDestinationOptions)packet.Ethernet.IpV6.ExtensionHeaders[0]).Options[0]).IsChecksumCorrect);
        }

        [TestMethod]
        public void IpV6AccessNetworkIdentifierSubOptionUnknown()
        {
            IpV6AccessNetworkIdentifierSubOptionUnknown subOption =
                new IpV6AccessNetworkIdentifierSubOptionUnknown((IpV6AccessNetworkIdentifierSubOptionType)100, DataSegment.Empty);
            Packet packet = PacketBuilder.Build(
                DateTime.Now,
                new EthernetLayer(),
                new IpV6Layer
                    {
                        ExtensionHeaders = new IpV6ExtensionHeaders(
                            new IpV6ExtensionHeaderMobilityBindingError(
                                IpV4Protocol.Skip, 0, IpV6BindingErrorStatus.UnrecognizedMhTypeValue, IpV6Address.Zero,
                                new IpV6MobilityOptions(
                                    new IpV6MobilityOptionAccessNetworkIdentifier(
                                        new IpV6AccessNetworkIdentifierSubOptions(subOption)))))
                    });
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(subOption,
                            ((IpV6MobilityOptionAccessNetworkIdentifier)
                             ((IpV6ExtensionHeaderMobility)packet.Ethernet.IpV6.ExtensionHeaders[0]).MobilityOptions[0]).SubOptions[0]);
        }

        [TestMethod]
        public void IpV6FlowIdentificationSubOptionUnknown()
        {
            IpV6FlowIdentificationSubOptionUnknown subOption =
                new IpV6FlowIdentificationSubOptionUnknown((IpV6FlowIdentificationSubOptionType)100, DataSegment.Empty);
            Packet packet = PacketBuilder.Build(
                DateTime.Now,
                new EthernetLayer(),
                new IpV6Layer
                    {
                        ExtensionHeaders = new IpV6ExtensionHeaders(
                            new IpV6ExtensionHeaderMobilityBindingError(
                                IpV4Protocol.Skip, 0, IpV6BindingErrorStatus.UnrecognizedMhTypeValue, IpV6Address.Zero,
                                new IpV6MobilityOptions(
                                    new IpV6MobilityOptionFlowIdentification(0, 0, IpV6FlowIdentificationStatus.FlowIdentifierNotFound,
                                                                             new IpV6FlowIdentificationSubOptions(subOption)))))
                    });
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(subOption,
                            ((IpV6MobilityOptionFlowIdentification)
                             ((IpV6ExtensionHeaderMobility)packet.Ethernet.IpV6.ExtensionHeaders[0]).MobilityOptions[0]).SubOptions[0]);
        }

        [TestMethod]
        public void IpV6OptionUnknown()
        {
            IpV6OptionUnknown option = new IpV6OptionUnknown((IpV6OptionType)0xBB, DataSegment.Empty);
            Packet packet = PacketBuilder.Build(
                DateTime.Now,
                new EthernetLayer(),
                new IpV6Layer
                {
                    ExtensionHeaders = new IpV6ExtensionHeaders(
                        new IpV6ExtensionHeaderDestinationOptions(IpV4Protocol.Skip, new IpV6Options(option)))
                });
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(option, ((IpV6ExtensionHeaderDestinationOptions)packet.Ethernet.IpV6.ExtensionHeaders[0]).Options[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6ExtensionHeaderRoutingRplCommonPrefixLengthForNonLastAddressesTooBig()
        {
            Assert.IsNull(new IpV6ExtensionHeaderRoutingRpl(IpV4Protocol.Skip, 0, 16, 0, new IpV6Address[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6ExtensionHeaderRoutingRplCommonPrefixLengthForLastAddressTooBig()
        {
            Assert.IsNull(new IpV6ExtensionHeaderRoutingRpl(IpV4Protocol.Skip, 0, 0, 16, new IpV6Address[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV6ExtensionHeadersEncapsulatingSecurityPayloadBeforeLast()
        {
            Assert.IsNull(new IpV6ExtensionHeaders(new IpV6ExtensionHeaderEncapsulatingSecurityPayload(0, 0, DataSegment.Empty),
                                                   new IpV6ExtensionHeaderFragmentData(IpV4Protocol.Skip, 0, false, 0)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void IpV6ExtensionHeaderAuthenticationNonIntegralMultipleOf4Bytes()
        {
            Assert.IsNull(new IpV6ExtensionHeaderAuthentication(IpV4Protocol.Skip, 0, 0, new DataSegment(new byte[6])));
        }

        [TestMethod]
        public void IpV6ExtensionHeadersConstructors()
        {
            IEnumerable<IpV6ExtensionHeader> extensionHeadersEnumerable = new IpV6ExtensionHeader[0];
            IList<IpV6ExtensionHeader> extensionHeadersIList = new IpV6ExtensionHeader[0];
            Assert.AreEqual(new IpV6ExtensionHeaders(extensionHeadersEnumerable), new IpV6ExtensionHeaders(extensionHeadersIList));
        }

        [TestMethod]
        public void IpV6MobilityOptionFlowSummaryConstructors()
        {
            IEnumerable<ushort> flowIdentifiersEnumerable = new ushort[1];
            IList<ushort> flowIdentifiersIList = new ushort[1];
            Assert.AreEqual(new IpV6MobilityOptionFlowSummary(flowIdentifiersEnumerable), new IpV6MobilityOptionFlowSummary(flowIdentifiersIList));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6MobilityOptionFlowSummaryNoIdentifiers()
        {
            Assert.IsNull(new IpV6MobilityOptionFlowSummary(new ushort[0]));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationLatitudeIntegerTooBig()
        {
            Assert.IsNull(new IpV6AccessNetworkIdentifierSubOptionGeoLocation((UInt24)0x7FFFFF, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationLatitudeIntegerTooSmall()
        {
            Assert.IsNull(new IpV6AccessNetworkIdentifierSubOptionGeoLocation((UInt24)0x800000, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationLongtitudeIntegerTooBig()
        {
            Assert.IsNull(new IpV6AccessNetworkIdentifierSubOptionGeoLocation(0, (UInt24)0x7FFFFF));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationLongtitudeIntegerTooSmall()
        {
            Assert.IsNull(new IpV6AccessNetworkIdentifierSubOptionGeoLocation(0, (UInt24)0x800000));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationCreateFromRealValuesLatitudeTooBig()
        {
            Assert.IsNull(IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues(90.1, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationCreateFromRealValuesLatitudeTooSmall()
        {
            Assert.IsNull(IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues(-90.1, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationCreateFromRealValuesLongtitudeTooBig()
        {
            Assert.IsNull(IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues(0, 180.1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionGeoLocationCreateFromRealValuesLongtitudeTooSmall()
        {
            Assert.IsNull(IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues(0, -180.1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionNetworkIdentifierNetworkNameTooLong()
        {
            Assert.IsNull(new IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier(false, new DataSegment(new byte[256]), DataSegment.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6AccessNetworkIdentifierSubOptionNetworkIdentifierAccessPointNameTooLong()
        {
            Assert.IsNull(new IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier(false, DataSegment.Empty, new DataSegment(new byte[256])));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IpV6ExtensionHeaderRoutingRplCommonPrefixNotCommon()
        {
            Assert.IsNull(new IpV6ExtensionHeaderRoutingRpl(IpV4Protocol.Skip, 5, 4, 4,
                                                            new IpV6Address("0000:0000:9ABC:DEF0:1234:5678:9ABC:DEF0"),
                                                            new IpV6Address("0000:0001:9ABC:DEF0:1234:5678:9ABC:DEF0")));
        }
    }
}
