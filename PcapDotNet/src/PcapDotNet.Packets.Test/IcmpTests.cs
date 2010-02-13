using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IcmpTests
    /// </summary>
    [TestClass]
    public class IcmpTests
    {
        public IcmpTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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
        public void RandomIcmpTest()
        {
            EthernetLayer ethernetLayer = new EthernetLayer
                                              {
                                                  Source = new MacAddress("00:01:02:03:04:05"),
                                                  Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
                                              };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;

            for (int i = 0; i != 2000; ++i)
            {
                IcmpLayer icmpLayer = random.NextIcmpLayer();
                icmpLayer.Checksum = null;
                if (icmpLayer.MessageType == IcmpMessageType.DestinationUnreachable && 
                    icmpLayer.MessageTypeAndCode != IcmpMessageTypeAndCode.DestinationUnreachableFragmentationNeededAndDontFragmentSet)
                {
                    ((IcmpDestinationUnreachableLayer)icmpLayer).NextHopMtu = 0;
                }

                IEnumerable<ILayer> icmpPayloadLayers = random.NextIcmpPayloadLayers(icmpLayer);

                int icmpPayloadLength = icmpPayloadLayers.Select(layer => layer.Length).Sum();

                switch (icmpLayer.MessageType)
                {
                    case IcmpMessageType.ParameterProblem:
                        ((IcmpParameterProblemLayer)icmpLayer).Pointer %= (byte)icmpPayloadLength;
                        break;

                    case IcmpMessageType.SecurityFailures:
                        ((IcmpSecurityFailuresLayer)icmpLayer).Pointer %= (ushort)icmpPayloadLength;
                        break;
                }

                PacketBuilder packetBuilder = new PacketBuilder(new ILayer[] { ethernetLayer, ipV4Layer, icmpLayer }.Concat(icmpPayloadLayers));

                Packet packet = packetBuilder.Build(DateTime.Now);
                Assert.IsTrue(packet.IsValid, "IsValid");

                byte[] buffer = (byte[])packet.Buffer.Clone();
                buffer.Write(ethernetLayer.Length + ipV4Layer.Length, random.NextDatagram(icmpLayer.Length));
                Packet illegalPacket = new Packet(buffer, DateTime.Now, packet.DataLink);
                Assert.IsFalse(illegalPacket.IsValid, "IsInvalid");
                if (illegalPacket.Ethernet.IpV4.Icmp is IcmpUnknownDatagram)
                {
                    byte[] icmpBuffer = new byte[illegalPacket.Ethernet.IpV4.Icmp.ExtractLayer().Length];
                    ILayer layer = illegalPacket.Ethernet.IpV4.Icmp.ExtractLayer();
                    layer.Write(icmpBuffer,0,icmpBuffer.Length, null,null);
                    layer.Finalize(icmpBuffer,0,icmpBuffer.Length,null);
                    MoreAssert.AreSequenceEqual(illegalPacket.Ethernet.IpV4.Icmp.ToArray(),
                                    icmpBuffer);

                    Assert.AreEqual(illegalPacket,
                                    new PacketBuilder(ethernetLayer, ipV4Layer, illegalPacket.Ethernet.IpV4.Icmp.ExtractLayer()).Build(DateTime.Now));
                }

                // Ethernet
                ethernetLayer.EtherType = EthernetType.IpV4;
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

                // IPv4
                ipV4Layer.Protocol = IpV4Protocol.InternetControlMessageProtocol;
                ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
                Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer());
                ipV4Layer.HeaderChecksum = null;
                Assert.AreEqual(ipV4Layer.Length, packet.Ethernet.IpV4.HeaderLength);
                Assert.IsTrue(packet.Ethernet.IpV4.IsHeaderChecksumCorrect);
                Assert.AreEqual(ipV4Layer.Length + icmpLayer.Length + icmpPayloadLength,
                                packet.Ethernet.IpV4.TotalLength);
                Assert.AreEqual(IpV4Datagram.DefaultVersion, packet.Ethernet.IpV4.Version);

                // ICMP
                IcmpDatagram actualIcmp = packet.Ethernet.IpV4.Icmp;
                IcmpLayer actualIcmpLayer = (IcmpLayer)actualIcmp.ExtractLayer();
                icmpLayer.Checksum = actualIcmpLayer.Checksum;
                Assert.AreEqual(icmpLayer, actualIcmpLayer);
                if (actualIcmpLayer.MessageType != IcmpMessageType.RouterSolicitation)
                    Assert.AreNotEqual(random.NextIcmpLayer(), actualIcmpLayer);
                Assert.IsTrue(actualIcmp.IsChecksumCorrect);
                Assert.AreEqual(icmpLayer.MessageType, actualIcmp.MessageType);
                Assert.AreEqual(icmpLayer.CodeValue, actualIcmp.Code);
                Assert.AreEqual(icmpLayer.MessageTypeAndCode, actualIcmp.MessageTypeAndCode);
                Assert.AreEqual(packet.Length - ethernetLayer.Length - ipV4Layer.Length - IcmpDatagram.HeaderLength, actualIcmp.Payload.Length);
                Assert.IsNotNull(icmpLayer.ToString());

                switch (packet.Ethernet.IpV4.Icmp.MessageType)
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
                    case IcmpMessageType.Traceroute:
                        Assert.AreEqual(((IcmpTracerouteLayer)icmpLayer).OutboundHopCount == 0xFFFF, ((IcmpTracerouteDatagram)actualIcmp).IsOutbound);
                        break;
                    case IcmpMessageType.DomainNameRequest:
                    case IcmpMessageType.SecurityFailures:
                        break;

                    case IcmpMessageType.DomainNameReply:
                    default:
                        throw new InvalidOperationException("Invalid icmpMessageType " + packet.Ethernet.IpV4.Icmp.MessageType);

                }
            }
        }
    }
}