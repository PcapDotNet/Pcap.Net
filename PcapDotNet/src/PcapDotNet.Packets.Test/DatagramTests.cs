using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for DatagramTests
    /// </summary>
    [TestClass]
    public class DatagramTests
    {
        public DatagramTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void RandomDatagramTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                Datagram datagram = random.NextDatagram(random.Next(1024));

                Assert.AreEqual(datagram, new Datagram(new List<byte>(datagram).ToArray()));
                Assert.AreEqual(datagram.GetHashCode(), new Datagram(new List<byte>(datagram).ToArray()).GetHashCode());

                Assert.AreNotEqual(datagram, random.NextDatagram(random.Next(10 * 1024)));
                Assert.AreNotEqual(datagram.GetHashCode(), random.NextDatagram(random.Next(10 * 1024)).GetHashCode());

                if (datagram.Length != 0)
                {
                    Assert.AreNotEqual(datagram, Datagram.Empty);
                    Assert.AreNotEqual(datagram, random.NextDatagram(datagram.Length));
                    Assert.AreNotEqual(datagram.GetHashCode(), random.NextDatagram(datagram.Length).GetHashCode());
                }
                else
                    Assert.AreEqual(datagram, Datagram.Empty);

                // Check Enumerable
                IEnumerable enumerable = datagram;
                int offset = 0;
                foreach (byte b in enumerable)
                    Assert.AreEqual(datagram[offset++], b);
            }
        }

        [TestMethod]
        public void DatagramExtractLayerTest()
        {
            PayloadLayer payloadLayer = new PayloadLayer
                                            {
                                                Data = new Datagram(new byte[] {100, 101, 102})
                                            };

            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer
                                                                  {
                                                                      EtherType = EthernetType.IpV4
                                                                  }, payloadLayer);
            Assert.AreEqual(payloadLayer, packet.Ethernet.Payload.ExtractLayer());
        }

        [TestMethod]
        public void DatagramCalculateIsValidTest()
        {
            Datagram data = new Datagram(new byte[]{1,2,3});
            Assert.IsTrue(data.IsValid);
        }
    }
}