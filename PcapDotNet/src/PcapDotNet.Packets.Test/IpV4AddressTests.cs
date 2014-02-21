using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV4AddressTests
    /// </summary>
    [TestClass]
    public class IpV4AddressTests
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
        public void IpV4AddressRandomTest()
        {
            Assert.AreEqual(IpV4Address.AllHostsGroupAddress, new IpV4Address("224.0.0.1"));

            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                IpV4Address address = random.NextIpV4Address();

                Assert.AreEqual(address, new IpV4Address(address.ToString()));
                Assert.IsTrue(address == new IpV4Address(address.ToString()));
                Assert.IsFalse(address != new IpV4Address(address.ToString()));
                Assert.AreEqual(address.GetHashCode(), new IpV4Address(address.ToString()).GetHashCode());
                Assert.AreEqual(address, new IpV4Address(address.ToValue()));
                
                Assert.AreNotEqual(address, random.NextIpV4Address());
                Assert.IsFalse(address == random.NextIpV4Address());
                Assert.IsTrue(address != random.NextIpV4Address());
                Assert.AreNotEqual(address.GetHashCode(), random.NextIpV4Address().GetHashCode());

                Assert.AreNotEqual(2, address);
                Assert.IsFalse(address.Equals(null));
            }
        }

        [TestMethod]
        public void IpV4AddressOrderTest()
        {
            Assert.AreEqual("0.0.0.0", new IpV4Address(0).ToString());
            Assert.AreEqual("0.0.0.0", IpV4Address.Zero.ToString());
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

        [TestMethod]
        public void IpV4AddressWithBufferTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                IpV4Address address = random.NextIpV4Address();

                byte[] buffer = new byte[IpV4Address.SizeOf];

                buffer.Write(0, address, Endianity.Big);
                Assert.AreEqual(address, buffer.ReadIpV4Address(0, Endianity.Big));
                Assert.AreNotEqual(address, buffer.ReadIpV4Address(0, Endianity.Small));

                buffer.Write(0, address, Endianity.Small);
                Assert.AreEqual(address, buffer.ReadIpV4Address(0, Endianity.Small));
                Assert.AreNotEqual(address, buffer.ReadIpV4Address(0, Endianity.Big));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void IpV4AddressConstructorNullTest()
        {
            Assert.IsNotNull(new IpV4Address(null));
            Assert.Fail();
        }

        [TestMethod]
        public void TryParseTest()
        {
            IpV4Address actual;
            Assert.IsTrue(IpV4Address.TryParse("1.2.3.4", out actual));
            Assert.AreEqual(new IpV4Address("1.2.3.4"), actual);

            Assert.IsFalse(IpV4Address.TryParse(null, out actual));
            Assert.IsFalse(IpV4Address.TryParse("1", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2.", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2.3", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2.3.", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2.3.a", out actual));
            Assert.IsFalse(IpV4Address.TryParse("a.2.3.4", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.a.3.4", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2.a.4", out actual));
            Assert.IsFalse(IpV4Address.TryParse("1.2.3.a", out actual));
            Assert.IsFalse(IpV4Address.TryParse("256.2.3.4", out actual));
        }
    }
}
