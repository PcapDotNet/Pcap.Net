using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV4AddressTests
    /// </summary>
    [TestClass]
    public class IpV4AddressTests
    {
        public IpV4AddressTests()
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
        public void IpV4AddressTest()
        {
            Assert.AreEqual("0.0.0.0", new IpV4Address(0).ToString());
            Assert.AreEqual("0.0.0.1", new IpV4Address(1).ToString());
            Assert.AreEqual("0.0.0.255", new IpV4Address(255).ToString());
            Assert.AreEqual("0.0.1.0", new IpV4Address(256).ToString());
            Assert.AreEqual("0.0.255.0", new IpV4Address(255 * 256).ToString());
            Assert.AreEqual("0.1.0.0", new IpV4Address(256 * 256).ToString());
            Assert.AreEqual("0.255.0.0", new IpV4Address(255 * 256 * 256).ToString());
            Assert.AreEqual("1.0.0.0", new IpV4Address(256 * 256 * 256).ToString());
            Assert.AreEqual("255.0.0.0", new IpV4Address((uint)255 * 256 * 256 * 256).ToString());
            Assert.AreEqual("255.254.253.252", new IpV4Address((uint)255 * 256 * 256 * 256 + 254 * 256 * 256 + 253 * 256 + 252).ToString());
        }
    }
}
