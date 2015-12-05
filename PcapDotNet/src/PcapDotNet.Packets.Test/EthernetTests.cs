using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for EthernetTests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EthernetTests
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
        public void RandomEthernetTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                EthernetLayer ethernetLayer = random.NextEthernetLayer();
                int ethernetPayloadLength = random.Next(1500);
                PayloadLayer payloadLayer = new PayloadLayer
                                                {
                                                    Data = random.NextDatagram(ethernetPayloadLength),
                                                };
                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, payloadLayer);

                // Ethernet
                Assert.IsTrue(new[] {EthernetType.IpV4, EthernetType.IpV6, EthernetType.Arp, EthernetType.VLanTaggedFrame}.Contains(packet.Ethernet.EtherType) ||
                              packet.IsValid, "IsValid - EtherType = " + packet.Ethernet.EtherType);
                Assert.AreEqual(packet.Length - EthernetDatagram.HeaderLengthValue, packet.Ethernet.PayloadLength, "PayloadLength");
                Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");
                Assert.AreEqual(ethernetLayer.GetHashCode(), packet.Ethernet.ExtractLayer().GetHashCode(), "Ethernet Layer Hash Code");
                Assert.AreNotEqual(random.NextEthernetLayer().GetHashCode(), packet.Ethernet.ExtractLayer().GetHashCode(), "Ethernet Layer Hash Code");
                Assert.AreEqual(ethernetLayer.ToString(), packet.Ethernet.ExtractLayer().ToString(), "Ethernet Layer ToString()");
                Assert.AreNotEqual(random.NextEthernetLayer().ToString(), packet.Ethernet.ExtractLayer().ToString(), "Ethernet Layer ToString()");
                Assert.AreNotEqual(2, packet.Ethernet.Source, "Ethernet Source");

                if (packet.Ethernet.EtherType == EthernetType.IpV4)
                    Assert.IsInstanceOfType(packet.Ethernet.Ip, typeof(IpV4Datagram));
                else if (packet.Ethernet.EtherType == EthernetType.IpV6)
                    Assert.IsInstanceOfType(packet.Ethernet.Ip, typeof(IpV6Datagram));
                else
                    Assert.IsNull(packet.Ethernet.Ip);

                Assert.AreEqual(payloadLayer.Data, packet.Ethernet.Payload);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticEthernetTypeNoNextLayer()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer());
            Assert.IsTrue(packet.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void AutomaticEthernetTypeBadNextLayer()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new TcpLayer());
            Assert.IsTrue(packet.IsValid);
        }

        [TestMethod]
        public void NoPayloadByEtherType()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer
                                                    {
                                                        EtherType = EthernetType.AppleTalk
                                                    },
                                                new PayloadLayer
                                                    {
                                                        Data = new Datagram(new byte[100])
                                                    });
            Assert.IsTrue(packet.IsValid);
            Assert.IsNull(packet.Ethernet.Padding);
            Assert.IsNull(packet.Ethernet.Trailer);
            Assert.IsNull(packet.Ethernet.FrameCheckSequence);
            Assert.IsNull(packet.Ethernet.ExtraData);
        }

        [TestMethod]
        public void EmptyPadding()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer
                                                {
                                                    EtherType = EthernetType.AppleTalk
                                                },
                                                new PayloadLayer
                                                {
                                                    Data = new Datagram(new byte[10])
                                                });
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(DataSegment.Empty, packet.Ethernet.Padding);
        }

        [TestMethod]
        public void PayloadTooBigForPadding()
        {
            Packet packet = PacketBuilder.Build(DateTime.Now,
                                                new EthernetLayer(),
                                                new ArpLayer
                                                {
                                                    ProtocolType = EthernetType.IpV4,
                                                    Operation = ArpOperation.DynamicReverseError,
                                                    SenderHardwareAddress = new byte[12].AsReadOnly(),
                                                    SenderProtocolAddress = new byte[22].AsReadOnly(),
                                                    TargetHardwareAddress = new byte[12].AsReadOnly(),
                                                    TargetProtocolAddress = new byte[22].AsReadOnly(),
                                                });
            Assert.IsTrue(packet.IsValid);
            Assert.AreEqual(DataSegment.Empty, packet.Ethernet.Padding);
        }
    }
}