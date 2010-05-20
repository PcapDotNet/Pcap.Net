using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for PayloadLayerTests
    /// </summary>
    [TestClass]
    public class PayloadLayerTests
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set;}

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
        public void PayloadLayerEqualsTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                PayloadLayer layer = random.NextPayloadLayer(random.Next(100));
                Assert.AreNotEqual(layer, null);
                Assert.AreEqual(layer, new PayloadLayer
                                           {
                                               Data = layer.Data
                                           });
                Assert.AreNotEqual(layer, new PayloadLayer
                                              {
                                                  Data = new Datagram(layer.Data.Concat<byte>(1).ToArray())
                                              });
                if (layer.Length > 1)
                {
                    Assert.AreNotEqual(layer, new PayloadLayer
                                                  {
                                                      Data = random.NextDatagram(layer.Length)
                                                  });
                }
            }
        }
    }
}