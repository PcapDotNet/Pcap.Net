using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for GreTests
    /// </summary>
    [TestClass]
    public class GreTests
    {
        public GreTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        public void RandomGreTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            for (int i = 0; i != 200; ++i)
            {
                GreLayer greLayer = random.NextGreLayer();
                PayloadLayer payloadLayer = random.NextPayloadLayer(random.Next(100));

                PacketBuilder packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, greLayer, payloadLayer);

                Packet packet = packetBuilder.Build(DateTime.Now);
                if (greLayer.Checksum == null &&
                    !new[] { EthernetType.IpV4, EthernetType.Arp }.Contains(packet.Ethernet.IpV4.Gre.ProtocolType))
                {
                    Assert.IsTrue(packet.IsValid, "IsValid, ProtocolType=" + packet.Ethernet.IpV4.Gre.ProtocolType);
                }

                // Ethernet
                ethernetLayer.EtherType = EthernetType.IpV4;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IPv4
                ipV4Layer.Protocol = IpV4Protocol.Gre;
                ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer());
                ipV4Layer.HeaderChecksum = null;
                Assert.AreEqual(ipV4Layer.Length, packet.Ethernet.IpV4.HeaderLength);
                Assert.IsTrue(packet.Ethernet.IpV4.IsHeaderChecksumCorrect);
                Assert.AreEqual(ipV4Layer.Length + greLayer.Length + payloadLayer.Length,
                                packet.Ethernet.IpV4.TotalLength);
                Assert.AreEqual(IpV4Datagram.DefaultVersion, packet.Ethernet.IpV4.Version);

                // GRE
                GreDatagram actualGre = packet.Ethernet.IpV4.Gre;
                GreLayer actualGreLayer = (GreLayer)actualGre.ExtractLayer();
                if (greLayer.ChecksumPresent && greLayer.Checksum == null)
                {
                    Assert.IsTrue(actualGre.IsChecksumCorrect);
                    greLayer.Checksum = actualGre.Checksum;
                }
                Assert.AreEqual(greLayer, actualGreLayer, "Layer");
                if (actualGreLayer.Key != null)
                    actualGreLayer.SetKey(actualGreLayer.KeyPayloadLength.Value, actualGreLayer.KeyCallId.Value);
                else
                {
                    Assert.IsNull(actualGreLayer.KeyPayloadLength);
                    Assert.IsNull(actualGreLayer.KeyCallId);
                }
                Assert.AreEqual(greLayer, actualGreLayer, "Layer");
                if (actualGre.KeyPresent)
                {
                    Assert.AreEqual(greLayer.KeyPayloadLength, actualGre.KeyPayloadLength, "KeyPayloadLength");
                    Assert.AreEqual(greLayer.KeyCallId, actualGre.KeyCallId, "KeyCallId");
                }
                Assert.AreNotEqual(random.NextGreLayer(), actualGreLayer, "Not Layer");
                Assert.AreEqual(greLayer.Length, actualGre.HeaderLength);
                Assert.IsTrue(actualGre.KeyPresent ^ (greLayer.Key == null));
                MoreAssert.IsSmaller(8, actualGre.RecursionControl);
                MoreAssert.IsSmaller(32, actualGre.FutureUseBits);
                Assert.IsTrue(actualGre.RoutingPresent ^ (greLayer.Routing == null && greLayer.RoutingOffset == null));
                Assert.IsTrue(actualGre.SequenceNumberPresent ^ (greLayer.SequenceNumber == null));
                Assert.IsTrue(!actualGre.StrictSourceRoute || actualGre.RoutingPresent);
                if (actualGre.RoutingPresent)
                {
                    Assert.IsNotNull(actualGre.ActiveSourceRouteEntryIndex);
                    if (actualGre.ActiveSourceRouteEntryIndex < actualGre.Routing.Count)
                        Assert.IsNotNull(actualGre.ActiveSourceRouteEntry);

                    foreach (GreSourceRouteEntry entry in actualGre.Routing)
                    {
                        Assert.AreNotEqual(entry, 2);
                        switch (entry.AddressFamily)
                        {
                            case GreSourceRouteEntryAddressFamily.AsSourceRoute:
                                GreSourceRouteEntryAs asEntry = (GreSourceRouteEntryAs)entry;
                                MoreAssert.IsInRange(0, asEntry.AsNumbers.Count, asEntry.NextAsNumberIndex);
                                if (asEntry.NextAsNumberIndex != asEntry.AsNumbers.Count)
                                    Assert.AreEqual(asEntry.AsNumbers[asEntry.NextAsNumberIndex], asEntry.NextAsNumber);
                                break;

                            case GreSourceRouteEntryAddressFamily.IpSourceRoute:
                                GreSourceRouteEntryIp ipEntry = (GreSourceRouteEntryIp)entry;
                                MoreAssert.IsInRange(0, ipEntry.Addresses.Count, ipEntry.NextAddressIndex);
                                if (ipEntry.NextAddressIndex != ipEntry.Addresses.Count)
                                    Assert.AreEqual(ipEntry.Addresses[ipEntry.NextAddressIndex], ipEntry.NextAddress);
                                break;

                            default:
                                GreSourceRouteEntryUnknown unknownEntry = (GreSourceRouteEntryUnknown)entry;
                                MoreAssert.IsInRange(0, unknownEntry.Data.Length, unknownEntry.PayloadOffset);
                                break;

                        }
                    }
                }
                else
                {
                    Assert.IsNull(actualGre.ActiveSourceRouteEntry);
                }

                Assert.IsNotNull(actualGre.Payload);
                switch (actualGre.ProtocolType)
                {
                    case EthernetType.IpV4:
                        Assert.IsNotNull(actualGre.IpV4);
                        break;

                    case EthernetType.Arp:
                        Assert.IsNotNull(actualGre.Arp);
                        break;
                }
            }
        }

        [TestMethod]
        public void InvalidGreTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(IpV4Protocol.Gre);
            ipV4Layer.HeaderChecksum = null;

            GreLayer greLayer = random.NextGreLayer();
            greLayer.Checksum = null;
            greLayer.Routing = new List<GreSourceRouteEntry>
                               {
                                   new GreSourceRouteEntryAs(new List<ushort> {123}.AsReadOnly(), 0),
                                   new GreSourceRouteEntryIp(new List<IpV4Address> {random.NextIpV4Address()}.AsReadOnly(),
                                                             0)
                               }.AsReadOnly();

            PacketBuilder packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, greLayer);
            Packet packet = packetBuilder.Build(DateTime.Now);
            Assert.IsTrue(packet.IsValid);

            GreDatagram gre = packet.Ethernet.IpV4.Gre;

            // Remove a byte from routing
            Datagram newIpPayload = new Datagram(gre.Take(gre.Length - 1).ToArray());
            packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, new PayloadLayer {Data = newIpPayload});
            packet = packetBuilder.Build(DateTime.Now);
            Assert.IsFalse(packet.IsValid);

            // SreLength is too big
            byte[] buffer = gre.ToArray();
            buffer[buffer.Length - 1] = 200;
            newIpPayload = new Datagram(buffer);
            packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, new PayloadLayer { Data = newIpPayload });
            packet = packetBuilder.Build(DateTime.Now);
            Assert.IsFalse(packet.IsValid);

            // PayloadOffset is too big
            buffer = gre.ToArray();
            buffer[gre.Length - 10] = 100;
            newIpPayload = new Datagram(buffer);
            packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, new PayloadLayer { Data = newIpPayload });
            packet = packetBuilder.Build(DateTime.Now);
            Assert.IsFalse(packet.IsValid);

            // PayloadOffset isn't aligned to ip
            buffer = gre.ToArray();
            buffer[gre.Length - 10] = 3;
            newIpPayload = new Datagram(buffer);
            packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, new PayloadLayer { Data = newIpPayload });
            packet = packetBuilder.Build(DateTime.Now);
            Assert.IsFalse(packet.IsValid);

            // PayloadOffset isn't aligned to as
            buffer = gre.ToArray();
            buffer[gre.Length - 16] = 1;
            newIpPayload = new Datagram(buffer);
            packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, new PayloadLayer { Data = newIpPayload });
            packet = packetBuilder.Build(DateTime.Now);
            Assert.IsFalse(packet.IsValid);
        }
    }
}