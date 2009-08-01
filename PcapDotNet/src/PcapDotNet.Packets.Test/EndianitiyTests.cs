using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for EndianitiyTests
    /// </summary>
    [TestClass]
    public class EndianitiyTests
    {
        public EndianitiyTests()
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
        public void UIntTest()
        {
            const uint value = 0x01020304;
            byte[] buffer = new byte[sizeof(uint)];

            buffer.Write(0, value, Endianity.Big);
            Assert.AreEqual(value, buffer.ReadUInt(0, Endianity.Big));
            Assert.AreEqual(0x01, buffer[0]);
            Assert.AreEqual(0x02, buffer[1]);
            Assert.AreEqual(0x03, buffer[2]);
            Assert.AreEqual(0x04, buffer[3]);

            buffer.Write(0, value, Endianity.Small);
            Assert.AreEqual(value, buffer.ReadUInt(0, Endianity.Small));
            Assert.AreEqual(0x04, buffer[0]);
            Assert.AreEqual(0x03, buffer[1]);
            Assert.AreEqual(0x02, buffer[2]);
            Assert.AreEqual(0x01, buffer[3]);
        }

        [TestMethod]
        public void UInt24Test()
        {
            UInt24 value = (UInt24)0x010203;
            byte[] buffer = new byte[UInt24.SizeOf];

            buffer.Write(0, value, Endianity.Big);
            Assert.AreEqual(value, buffer.ReadUInt24(0, Endianity.Big));
            Assert.AreEqual(0x01, buffer[0]);
            Assert.AreEqual(0x02, buffer[1]);
            Assert.AreEqual(0x03, buffer[2]);

            buffer.Write(0, value, Endianity.Small);
            Assert.AreEqual(value, buffer.ReadUInt24(0, Endianity.Small));
            Assert.AreEqual(0x03, buffer[0]);
            Assert.AreEqual(0x02, buffer[1]);
            Assert.AreEqual(0x01, buffer[2]);
        }

        [TestMethod]
        public void UInt48Test()
        {
            UInt48 value = (UInt48)0x010203040506;
            byte[] buffer = new byte[UInt48.SizeOf];

            buffer.Write(0, value, Endianity.Big);
            Assert.AreEqual(value, buffer.ReadUInt48(0, Endianity.Big));
            Assert.AreEqual(0x01, buffer[0]);
            Assert.AreEqual(0x02, buffer[1]);
            Assert.AreEqual(0x03, buffer[2]);
            Assert.AreEqual(0x04, buffer[3]);
            Assert.AreEqual(0x05, buffer[4]);
            Assert.AreEqual(0x06, buffer[5]);

            buffer.Write(0, value, Endianity.Small);
            Assert.AreEqual(value, buffer.ReadUInt48(0, Endianity.Small));
            Assert.AreEqual(0x06, buffer[0]);
            Assert.AreEqual(0x05, buffer[1]);
            Assert.AreEqual(0x04, buffer[2]);
            Assert.AreEqual(0x03, buffer[3]);
            Assert.AreEqual(0x02, buffer[4]);
            Assert.AreEqual(0x01, buffer[5]);
        }
    }
}