using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for PacketBuilderTests
    /// </summary>
    [TestClass]
    public class PacketBuilderTests
    {
        public PacketBuilderTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        [ExpectedException(typeof(ArgumentException))]
        public void NoLayersTest()
        {
            PacketBuilder packetBuilder = new PacketBuilder();
            Assert.IsNull(packetBuilder);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BadFirstLayerTest()
        {
            PacketBuilder packetBuilder = new PacketBuilder(new TcpLayer());
            Assert.IsNull(packetBuilder);
        }
    }
}