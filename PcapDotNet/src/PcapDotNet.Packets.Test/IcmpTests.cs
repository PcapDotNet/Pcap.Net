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

            IpV4Layer ipV4Layer = new IpV4Layer
                                      {
                                          TypeOfService = random.NextByte(),
                                          Identification = random.NextUShort(),
                                          Ttl = random.NextByte(),
                                          Fragmentation = random.NextIpV4Fragmentation(),
                                          Source = random.NextIpV4Address(),
                                          Destination = random.NextIpV4Address(),
                                          Options = random.NextIpV4Options()
                                      };


            for (int i = 0; i != 1000; ++i)
            {
                IpV4Layer icmpIpV4Layer = new IpV4Layer
                                              {
                                                  TypeOfService = random.NextByte(),
                                                  Identification = random.NextUShort(),
                                                  Fragmentation = random.NextIpV4Fragmentation(),
                                                  Ttl = random.NextByte(),
                                                  Protocol = random.NextEnum<IpV4Protocol>(),
                                                  Source = random.NextIpV4Address(),
                                                  Destination = random.NextIpV4Address(),
                                                  Options = random.NextIpV4Options(),
                                              };

                PayloadLayer icmpIpV4PayloadLayer = new PayloadLayer
                                                    {
                                                        Data = random.NextDatagram(random.Next(200))
                                                    };

                IcmpLayer icmpLayer = random.NextIcmpLayer();

                Packet packet = new PacketBuilder2(ethernetLayer, ipV4Layer, icmpLayer, icmpIpV4Layer, icmpIpV4PayloadLayer).Build(DateTime.Now);

                Assert.IsTrue(packet.IsValid, "IsValid");
            }
        }
    }
}