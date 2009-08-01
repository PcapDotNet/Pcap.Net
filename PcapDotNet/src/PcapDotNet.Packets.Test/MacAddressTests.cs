using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for MacAddressTests
    /// </summary>
    [TestClass]
    public class MacAddressTests
    {
        public MacAddressTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void MacAddressTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                MacAddress macAddress = random.NextMacAddress();

                Assert.IsNotNull(macAddress.ToString());
                Assert.AreEqual(macAddress, new MacAddress(macAddress.ToString()));
                Assert.AreNotEqual(macAddress, random.NextMacAddress());
                Assert.IsTrue(macAddress == new MacAddress(macAddress.ToString()));
                Assert.AreEqual(macAddress.GetHashCode(), new MacAddress(macAddress.ToString()).GetHashCode());
                Assert.IsTrue(macAddress != random.NextMacAddress());
                Assert.AreNotEqual(macAddress.GetHashCode(), random.NextMacAddress().GetHashCode());
            }
        }

        [TestMethod]
        public void MacAddressWithBufferTest()
        {
            Random random = new Random();
            MacAddress address = random.NextMacAddress();

            byte[] buffer = new byte[MacAddress.SizeOf];

            buffer.Write(0, address, Endianity.Big);
            Assert.AreEqual(address, buffer.ReadMacAddress(0, Endianity.Big));
            Assert.AreNotEqual(address, buffer.ReadMacAddress(0, Endianity.Small));

            int offset = 0;
            Assert.AreEqual(address, buffer.ReadMacAddress(ref offset, Endianity.Big));
            Assert.AreEqual(MacAddress.SizeOf, offset);

            buffer.Write(0, address, Endianity.Small);
            Assert.AreEqual(address, buffer.ReadMacAddress(0, Endianity.Small));
            Assert.AreNotEqual(address, buffer.ReadMacAddress(0, Endianity.Big));

            offset = 0;
            Assert.AreEqual(address, buffer.ReadMacAddress(ref offset, Endianity.Small));
            Assert.AreEqual(MacAddress.SizeOf, offset);
        }
    }
}