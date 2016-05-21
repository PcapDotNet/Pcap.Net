using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IcmpTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class IcmpTests
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
        public void RandomIcmpTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            int seed = new Random().Next();
            Console.WriteLine("Seed: " + seed);
            Random random = new Random(seed);

            for (int i = 0; i != 2000; ++i)
            {
                IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
                ipV4Layer.HeaderChecksum = null;
                Layer ipLayer = random.NextBool() ? (Layer)ipV4Layer : random.NextIpV6Layer(IpV4Protocol.InternetControlMessageProtocol, false);

                IcmpLayer icmpLayer = random.NextIcmpLayer();
                icmpLayer.Checksum = null;
                if (icmpLayer.MessageType == IcmpMessageType.DestinationUnreachable && 
                    icmpLayer.MessageTypeAndCode != IcmpMessageTypeAndCode.DestinationUnreachableFragmentationNeededAndDoNotFragmentSet)
                {
                    ((IcmpDestinationUnreachableLayer)icmpLayer).NextHopMaximumTransmissionUnit = 0;
                }

                IEnumerable<ILayer> icmpPayloadLayers = random.NextIcmpPayloadLayers(icmpLayer);

                int icmpPayloadLength = icmpPayloadLayers.Select(layer => layer.Length).Sum();

                switch (icmpLayer.MessageType)
                {
                    case IcmpMessageType.ParameterProblem:
                        if (icmpPayloadLength % 4 != 0)
                            icmpPayloadLayers = icmpPayloadLayers.Concat(new[] {new PayloadLayer {Data = random.NextDatagram(4 - icmpPayloadLength % 4)}});
                        icmpPayloadLength = icmpPayloadLayers.Select(layer => layer.Length).Sum();
                        IcmpParameterProblemLayer icmpParameterProblemLayer = (IcmpParameterProblemLayer)icmpLayer;
                        icmpParameterProblemLayer.Pointer = (byte)(icmpParameterProblemLayer.Pointer % icmpPayloadLength);
                        icmpParameterProblemLayer.OriginalDatagramLength = icmpPayloadLength - icmpPayloadLayers.First().Length;
                        break;

                    case IcmpMessageType.SecurityFailures:
                        ((IcmpSecurityFailuresLayer)icmpLayer).Pointer %= (ushort)icmpPayloadLength;
                        break;
                }

                PacketBuilder packetBuilder = new PacketBuilder(new ILayer[] { ethernetLayer, ipLayer, icmpLayer }.Concat(icmpPayloadLayers));

                Packet packet = packetBuilder.Build(DateTime.Now);
                Assert.IsTrue(packet.IsValid, "IsValid");

                byte[] buffer = (byte[])packet.Buffer.Clone();
                buffer.Write(ethernetLayer.Length + ipLayer.Length, random.NextDatagram(icmpLayer.Length));
                Packet illegalPacket = new Packet(buffer, DateTime.Now, packet.DataLink);
                Assert.IsFalse(illegalPacket.IsValid, "IsInvalid");
                if (illegalPacket.Ethernet.Ip.Icmp is IcmpUnknownDatagram)
                {
                    byte[] icmpBuffer = new byte[illegalPacket.Ethernet.Ip.Icmp.ExtractLayer().Length];
                    ILayer layer = illegalPacket.Ethernet.Ip.Icmp.ExtractLayer();
                    layer.Write(icmpBuffer,0,icmpBuffer.Length, null,null);
                    layer.Finalize(icmpBuffer,0,icmpBuffer.Length,null);
                    MoreAssert.AreSequenceEqual(illegalPacket.Ethernet.Ip.Icmp.ToArray(),
                                    icmpBuffer);

                    Assert.AreEqual(illegalPacket,
                                    PacketBuilder.Build(DateTime.Now, ethernetLayer, ipLayer, illegalPacket.Ethernet.Ip.Icmp.ExtractLayer()));
                }

                // Ethernet
                ethernetLayer.EtherType = ipLayer == ipV4Layer ? EthernetType.IpV4 : EthernetType.IpV6;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");
                ethernetLayer.EtherType = EthernetType.None;

                // IP.
                if (ipLayer == ipV4Layer)
                {
                    // IPv4.
                    ipV4Layer.Protocol = IpV4Protocol.InternetControlMessageProtocol;
                    ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                    Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer());
                    ipV4Layer.HeaderChecksum = null;
                    Assert.AreEqual(ipV4Layer.Length, packet.Ethernet.IpV4.HeaderLength);
                    Assert.IsTrue(packet.Ethernet.IpV4.IsHeaderChecksumCorrect);
                    Assert.AreEqual(ipV4Layer.Length + icmpLayer.Length + icmpPayloadLength,
                                    packet.Ethernet.IpV4.TotalLength);
                    Assert.AreEqual(IpV4Datagram.DefaultVersion, packet.Ethernet.IpV4.Version);
                } 
                else
                {
                    // IPv6.
                    Assert.AreEqual(ipLayer, packet.Ethernet.IpV6.ExtractLayer());
                }

                // ICMP
                IcmpDatagram actualIcmp = packet.Ethernet.Ip.Icmp;
                IcmpLayer actualIcmpLayer = (IcmpLayer)actualIcmp.ExtractLayer();
                icmpLayer.Checksum = actualIcmpLayer.Checksum;
                Assert.AreEqual(icmpLayer, actualIcmpLayer);
                Assert.AreEqual(icmpLayer.GetHashCode(), actualIcmpLayer.GetHashCode());
                if (actualIcmpLayer.MessageType != IcmpMessageType.RouterSolicitation)
                {
                    Assert.AreNotEqual(random.NextIcmpLayer(), actualIcmpLayer);
                    IcmpLayer otherIcmpLayer = random.NextIcmpLayer();
                    Assert.AreNotEqual(otherIcmpLayer.GetHashCode(), actualIcmpLayer.GetHashCode());
                }
                Assert.IsTrue(actualIcmp.IsChecksumCorrect);
                Assert.AreEqual(icmpLayer.MessageType, actualIcmp.MessageType);
                Assert.AreEqual(icmpLayer.CodeValue, actualIcmp.Code);
                Assert.AreEqual(icmpLayer.MessageTypeAndCode, actualIcmp.MessageTypeAndCode);
                Assert.AreEqual(packet.Length - ethernetLayer.Length - ipLayer.Length - IcmpDatagram.HeaderLength, actualIcmp.Payload.Length);
                Assert.IsNotNull(icmpLayer.ToString());

                switch (packet.Ethernet.Ip.Icmp.MessageType)
                {
                    case IcmpMessageType.RouterSolicitation:
                    case IcmpMessageType.SourceQuench:
                    case IcmpMessageType.TimeExceeded:
                        Assert.AreEqual<uint>(0, actualIcmp.Variable);
                        break;

                    case IcmpMessageType.DestinationUnreachable:
                    case IcmpMessageType.ParameterProblem:
                    case IcmpMessageType.Redirect:
                    case IcmpMessageType.ConversionFailed:
                    case IcmpMessageType.Echo:
                    case IcmpMessageType.EchoReply:
                    case IcmpMessageType.Timestamp:
                    case IcmpMessageType.TimestampReply:
                    case IcmpMessageType.InformationRequest:
                    case IcmpMessageType.InformationReply:
                    case IcmpMessageType.RouterAdvertisement:
                    case IcmpMessageType.AddressMaskRequest:
                    case IcmpMessageType.AddressMaskReply:
                        break;
                    case IcmpMessageType.TraceRoute:
                        Assert.AreEqual(((IcmpTraceRouteLayer)icmpLayer).ReturnHopCount == 0xFFFF, ((IcmpTraceRouteDatagram)actualIcmp).IsOutbound);
                        break;
                    case IcmpMessageType.DomainNameRequest:
                    case IcmpMessageType.SecurityFailures:
                        break;

                    case IcmpMessageType.DomainNameReply:
                    default:
                        throw new InvalidOperationException("Invalid icmpMessageType " + packet.Ethernet.Ip.Icmp.MessageType);

                }
            }
        }

        [TestMethod]
        public void IcmpRouterAdvertisementEntryTest()
        {
            Random random = new Random();
            IcmpRouterAdvertisementEntry entry1 = new IcmpRouterAdvertisementEntry(random.NextIpV4Address(), random.Next());
            IcmpRouterAdvertisementEntry entry2 = new IcmpRouterAdvertisementEntry(random.NextIpV4Address(), random.Next());

            Assert.AreEqual(entry1, entry1);
            Assert.AreEqual(entry1.GetHashCode(), entry1.GetHashCode());
            Assert.AreNotEqual(entry1, entry2);
            Assert.AreNotEqual(entry1.GetHashCode(), entry2.GetHashCode());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void IcmpDatagramCreateDatagramNullBufferTest()
        {
            Assert.IsNotNull(IcmpDatagram.CreateDatagram(null, 0, 0));
            Assert.Fail();
        }

        [TestMethod]
        public void IcmpDatagramCreateDatagramBadOffsetTest()
        {
            Assert.IsInstanceOfType(IcmpDatagram.CreateDatagram(new byte[0], -1, 0), typeof(IcmpUnknownDatagram));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IcmpParameterProblemLayerOriginalDatagramLengthNotRound()
        {
            Layer layer = new IcmpParameterProblemLayer {OriginalDatagramLength = 6};
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void IcmpParameterProblemLayerOriginalDatagramLengthTooBig()
        {
            Layer layer = new IcmpParameterProblemLayer { OriginalDatagramLength = 2000 };
            Assert.IsNotNull(layer);
        }
    }
}