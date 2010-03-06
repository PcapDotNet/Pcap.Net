using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for IpV6AddressTests
    /// </summary>
    [TestClass]
    public class IpV6AddressTests
    {
        public IpV6AddressTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
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
        public void IpV6AddressRandomTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                IpV6Address address = random.NextIpV6Address();

                Assert.AreEqual(address, new IpV6Address(address.ToString()));
                Assert.IsTrue(address == new IpV6Address(address.ToString()));
                Assert.IsFalse(address != new IpV6Address(address.ToString()));
                Assert.AreEqual(address.GetHashCode(), new IpV6Address(address.ToString()).GetHashCode());
                Assert.AreEqual(address, new IpV6Address(address.ToValue()));

                Assert.AreNotEqual(address, random.NextIpV6Address());
                Assert.IsFalse(address == random.NextIpV6Address());
                Assert.IsTrue(address != random.NextIpV6Address());
                Assert.AreNotEqual(address.GetHashCode(), random.NextIpV6Address().GetHashCode());

                Assert.AreNotEqual(2, address);
                Assert.IsFalse(address.Equals(null));
            }
        }

        [TestMethod]
        public void IpV6AddressOrderTest()
        {
//            Assert.AreEqual("0.0.0.0", new IpV4Address(0).ToString());
//            Assert.AreEqual("0.0.0.0", IpV4Address.Zero.ToString());
//            Assert.AreEqual("0.0.0.1", new IpV4Address(1).ToString());
//            Assert.AreEqual("0.0.0.255", new IpV4Address(255).ToString());
//            Assert.AreEqual("0.0.1.0", new IpV4Address(256).ToString());
//            Assert.AreEqual("0.0.255.0", new IpV4Address(255 * 256).ToString());
//            Assert.AreEqual("0.1.0.0", new IpV4Address(256 * 256).ToString());
//            Assert.AreEqual("0.255.0.0", new IpV4Address(255 * 256 * 256).ToString());
//            Assert.AreEqual("1.0.0.0", new IpV4Address(256 * 256 * 256).ToString());
//            Assert.AreEqual("255.0.0.0", new IpV4Address((uint)255 * 256 * 256 * 256).ToString());
//            Assert.AreEqual("255.254.253.252", new IpV4Address((uint)255 * 256 * 256 * 256 + 254 * 256 * 256 + 253 * 256 + 252).ToString());
        }

        [TestMethod]
        public void IpV6AddressWithBufferTest()
        {
//            Random random = new Random();

//            for (int i = 0; i != 1000; ++i)
//            {
//                IpV4Address address = random.NextIpV4Address();

//                byte[] buffer = new byte[IpV4Address.SizeOf];

//                buffer.Write(0, address, Endianity.Big);
//                Assert.AreEqual(address, buffer.ReadIpV4Address(0, Endianity.Big));
//                Assert.AreNotEqual(address, buffer.ReadIpV4Address(0, Endianity.Small));

//                buffer.Write(0, address, Endianity.Small);
//                Assert.AreEqual(address, buffer.ReadIpV4Address(0, Endianity.Small));
//                Assert.AreNotEqual(address, buffer.ReadIpV4Address(0, Endianity.Big));
//            }
        }

        [TestMethod]
        public void IpV6AddressParsingTest()
        {
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000:0000:0000:0000:0000:0000:0000"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000:0000:0000:0000:0000:0.0.0.0"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000:0000::0000:0000:0.0.0.0"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000:0000::0000:0.0.0.0"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000::0000:0.0.0.0"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000::0000"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000:0000::"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("0000::"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("::"));
            Assert.AreEqual(IpV6Address.Zero, new IpV6Address("::0.0.0.0"));

            Assert.AreEqual(new IpV6Address("1:2:3:4:5:6:7:8"), new IpV6Address("0001:0002:0003:0004:0005:0006:0007:0008"));
            Assert.AreEqual(new IpV6Address("1:2:3:4:5:6:7:8"), new IpV6Address("0001:0002:0003:0004:0005:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("1:0:3:4:5:6:7:8"), new IpV6Address("0001:0000:0003:0004:0005:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("1:0:3:4:5:6:7:8"), new IpV6Address("0001::0003:0004:0005:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:4:5:6:7:8"), new IpV6Address("0:0:0003:0004:0005:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:4:5:6:7:8"), new IpV6Address(":0:0003:0004:0005:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:4:5:6:7:8"), new IpV6Address("::0003:0004:0005:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address("0:0:0003:0000:0000:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address("0:0:0003:0:0:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address("0:0:0003::0:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address("0:0:0003::0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address("0::0003:0:0:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address("::0003:0:0:0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address(":0:0003::0006:0.7.0.8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:8"), new IpV6Address(":0:0003:0:0:0006:7:8"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:7:0"), new IpV6Address(":0:0003:0:0:0006:7:"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:0:0"), new IpV6Address(":0:0003:0:0:0006::"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:0:0"), new IpV6Address(":0:0003:0:0:0006::0"));
            Assert.AreEqual(new IpV6Address("0:0:3:0:0:6:0:0"), new IpV6Address(":0:0003:0:0:0006::0"));
        }

        [TestMethod]
        public void IpV6AddressToStringTest()
        {
            Assert.AreEqual("0000:0000:0000:0000:0000:0000:0000:0000", IpV6Address.Zero.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV6AddressNoColonTest()
        {
            IpV6Address ipV6Address = new IpV6Address("123");
            Assert.AreEqual(ipV6Address,ipV6Address);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IpV6AddressDoubleColonsWithoutMissingColonsTest()
        {
            IpV6Address ipV6Address = new IpV6Address("1::2:3:4:5:6:7:8");
            Assert.AreEqual(ipV6Address, ipV6Address);
        }
    }
}