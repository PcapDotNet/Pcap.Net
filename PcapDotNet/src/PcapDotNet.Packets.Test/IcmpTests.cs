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

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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

            for (int i = 0; i != 1000; ++i)
            {
                IcmpLayer icmpLayer = random.NextIcmpLayer();
                icmpLayer.Checksum = null;

                bool isIpV4Payload;
                switch (icmpLayer.MessageType)
                {
                    case IcmpMessageType.DestinationUnreachable:
                    case IcmpMessageType.TimeExceeded:
                    case IcmpMessageType.ParameterProblem:
                    case IcmpMessageType.SourceQuench:
                    case IcmpMessageType.Redirect:
                    case IcmpMessageType.ConversionFailed:
                        isIpV4Payload = true;
                        break;

                    case IcmpMessageType.Echo:
                    case IcmpMessageType.EchoReply:
                    case IcmpMessageType.Timestamp:
                    case IcmpMessageType.TimestampReply:
                    case IcmpMessageType.InformationRequest:
                    case IcmpMessageType.InformationReply:
                    case IcmpMessageType.RouterAdvertisement:
                    case IcmpMessageType.RouterSolicitation:
                    case IcmpMessageType.AddressMaskRequest:
                    case IcmpMessageType.AddressMaskReply:
                    case IcmpMessageType.Traceroute:
                    case IcmpMessageType.DomainNameRequest:
                    case IcmpMessageType.SecurityFailures:
                        isIpV4Payload = false;
                        break;

                    case IcmpMessageType.DomainNameReply:
                    default:
                        throw new InvalidOperationException("Invalid icmpMessageType " + icmpLayer.MessageType);
                }
                PacketBuilder packetBuilder;
                IpV4Layer icmpIpV4Layer = null;
                PayloadLayer icmpIpV4PayloadLayer = null; 
                if (isIpV4Payload)
                {
                    icmpIpV4Layer = random.NextIpV4Layer();

                    icmpIpV4PayloadLayer = random.NextPayloadLayer(random.Next(200));

                    packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer, icmpIpV4Layer, icmpIpV4PayloadLayer);
                }
                else
                    packetBuilder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer);

                Packet packet = packetBuilder.Build(DateTime.Now);

                Assert.IsTrue(packet.IsValid, "IsValid");

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
                Assert.AreEqual(ipV4Layer.Length + icmpLayer.Length + (isIpV4Payload ? icmpIpV4Layer.Length + icmpIpV4PayloadLayer.Length : 0),
                                packet.Ethernet.IpV4.TotalLength);
                Assert.AreEqual(IpV4Datagram.DefaultVersion, packet.Ethernet.IpV4.Version);
                
                // ICMP
                IcmpLayer actualIcmpLayer = (IcmpLayer)packet.Ethernet.IpV4.Icmp.ExtractLayer();
                icmpLayer.Checksum = actualIcmpLayer.Checksum;
                Assert.AreEqual(icmpLayer, actualIcmpLayer);
                Assert.IsTrue(packet.Ethernet.IpV4.Icmp.IsChecksumCorrect);

                switch (packet.Ethernet.IpV4.Icmp.MessageType)
                {
                    case IcmpMessageType.DestinationUnreachable:
//                        Assert.AreEqual(icmpIpV4Layer, packet.Ethernet.IpV4.Icmp.DestinationUncreachable.IpV4.ExtractLayer());
                        
                    case IcmpMessageType.TimeExceeded:
                    case IcmpMessageType.ParameterProblem:
                    case IcmpMessageType.SourceQuench:
                    case IcmpMessageType.Redirect:
                    case IcmpMessageType.ConversionFailed:
                        isIpV4Payload = true;
                        break;

                    case IcmpMessageType.Echo:
                    case IcmpMessageType.EchoReply:
                    case IcmpMessageType.Timestamp:
                    case IcmpMessageType.TimestampReply:
                    case IcmpMessageType.InformationRequest:
                    case IcmpMessageType.InformationReply:
                    case IcmpMessageType.RouterAdvertisement:
                    case IcmpMessageType.RouterSolicitation:
//                        packet.Ethernet.IpV4.Icmp.RouterSolicitation
                    case IcmpMessageType.AddressMaskRequest:
                    case IcmpMessageType.AddressMaskReply:
                    case IcmpMessageType.Traceroute:
                    case IcmpMessageType.DomainNameRequest:
                    case IcmpMessageType.SecurityFailures:
                        isIpV4Payload = false;
                        break;

                    case IcmpMessageType.DomainNameReply:

                     default:
                        throw new InvalidOperationException("Invalid icmpMessageType " + packet.Ethernet.IpV4.Icmp.MessageType);
                   
                }
            }
        }
    }
}