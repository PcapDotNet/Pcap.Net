using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for UInt48Tests
    /// </summary>
    [TestClass]
    public class UInt48Tests
    {
        public UInt48Tests()
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
        public void ParseTest()
        {
            Random random = new Random();
            for (int i = 0; i != 100; ++i)
            {
                UInt48 expected = (UInt48)random.NextLong(UInt48.MaxValue + 1);

                UInt48 actual = UInt48.Parse(expected.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture);
                Assert.AreEqual(expected, actual);

                actual = UInt48.Parse(expected.ToString(), NumberStyles.Integer);
                Assert.AreEqual(expected, actual);

                actual = UInt48.Parse(expected.ToString(), CultureInfo.InvariantCulture);
                Assert.AreEqual(expected, actual);

                actual = UInt48.Parse(expected.ToString());
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ParseTooBigTest()
        {
            UInt48 value = UInt48.Parse(ulong.MaxValue.ToString());
            Assert.IsNotNull(value);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ParseTooBigTestEvenForUInt64()
        {
            UInt48 value = UInt48.Parse(ulong.MaxValue + "0");
            Assert.IsNotNull(value);
        }

        //        [TestMethod]
        //        public void UInt48Test()
        //        {
        //            UInt48 value = (UInt48)0x010203040506;
        //            byte[] buffer = new byte[UInt48.SizeOf];
        //
        //            buffer.Write(0, value, Endianity.Big);
        //            Assert.AreEqual(value, buffer.ReadUInt48(0, Endianity.Big));
        //            Assert.AreEqual(0x01, buffer[0]);
        //            Assert.AreEqual(0x02, buffer[1]);
        //            Assert.AreEqual(0x03, buffer[2]);
        //            Assert.AreEqual(0x04, buffer[3]);
        //            Assert.AreEqual(0x05, buffer[4]);
        //            Assert.AreEqual(0x06, buffer[5]);
        //
        //            int offset = 0;
        //            buffer.Write(ref offset, value, Endianity.Big);
        //            Assert.AreEqual(value, buffer.ReadUInt48(0, Endianity.Big));
        //            Assert.AreEqual(6, offset);
        //
        //            buffer.Write(0, value, Endianity.Small);
        //            Assert.AreEqual(value, buffer.ReadUInt48(0, Endianity.Small));
        //            Assert.AreEqual(0x06, buffer[0]);
        //            Assert.AreEqual(0x05, buffer[1]);
        //            Assert.AreEqual(0x04, buffer[2]);
        //            Assert.AreEqual(0x03, buffer[3]);
        //            Assert.AreEqual(0x02, buffer[4]);
        //            Assert.AreEqual(0x01, buffer[5]);
        //
        //            offset = 0;
        //            buffer.Write(ref offset, value, Endianity.Small);
        //            Assert.AreEqual(value, buffer.ReadUInt48(0, Endianity.Small));
        //            Assert.AreEqual(6, offset);
        //        }

        [TestMethod]
        public void UInt48Test()
        {
            Random random = new Random();
            for (int i = 0; i != 1000; ++i)
            {
                UInt48 value = random.NextUInt48();

                Assert.AreEqual(value, value);
                Assert.AreNotEqual(value, "1");
                Assert.IsTrue(value == value);
                Assert.IsFalse(value != value);
                Assert.IsNotNull(value.GetHashCode());

                if (value < uint.MaxValue)
                    Assert.AreEqual(value, uint.Parse(value.ToString()));

                Assert.AreEqual((byte)value, (byte)(value % 256));
            }
        }
    }
}
