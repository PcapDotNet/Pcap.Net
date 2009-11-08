using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for UInt128Tests
    /// </summary>
    [TestClass]
    public class UInt128Tests
    {
        public UInt128Tests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        public void ParseTest()
        {
//            Random random = new Random();
//            for (int i = 0; i != 100; ++i)
//            {
//                UInt48 expected = (UInt48)random.NextLong(UInt48.MaxValue + 1);
//                UInt48 actual = UInt48.Parse(expected.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture);
//
//                Assert.AreEqual(expected, actual);
//            }
        }

        [TestMethod]
        public void UInt128Test()
        {
//            Random random = new Random();
//            for (int i = 0; i != 1000; ++i)
//            {
//                UInt48 value = random.NextUInt48();
//
//                Assert.AreEqual(value, value);
//                Assert.IsTrue(value == value);
//                Assert.IsFalse(value != value);
//                Assert.IsNotNull(value.GetHashCode());
//
//                if (value < uint.MaxValue)
//                    Assert.AreEqual(value, uint.Parse(value.ToString()));
//
//                Assert.AreEqual((byte)value, (byte)(value % 256));
//            }
        }

        [TestMethod]
        public void ShiftRightTest()
        {
            const string valueString = "0123456789ABCDEFFEDCBA9876543210";
            UInt128 value = UInt128.Parse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(UInt128.Parse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture), value);

            for (int i = 0; i <= 124; i += 4)
            {
                string expectedValueString = new string('0', i / 4) + valueString.Substring(0, valueString.Length - i / 4);
                UInt128 expectedValue = UInt128.Parse(expectedValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                Assert.AreEqual(expectedValue, value >> i, i.ToString());
                Assert.AreEqual(expectedValue, value >> (i / 2) >> (i / 2), i.ToString());
                Assert.AreEqual(expectedValue, value >> (i / 4) >> (i / 4) >> (i / 4) >> (i / 4), i.ToString());
            }
        }

        [TestMethod]
        public void BitwiseAndTest()
        {
            const string valueString = "0123456789ABCDEFFEDCBA9876543210";
            UInt128 value = UInt128.Parse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(UInt128.Parse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture), value);

            for (int i = 0; i <= 32; ++i)
            {
                string andValueString = new string('0', i) + new string('F', valueString.Length - i);
                UInt128 andValue = UInt128.Parse(andValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                string expectedValueString = new string('0', i) + valueString.Substring(i, valueString.Length - i);
                UInt128 expectedValue = UInt128.Parse(expectedValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                UInt128 actualValue = value & andValue;
                Assert.AreEqual(expectedValue, actualValue, i.ToString());
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            const string valueString = "0123456789ABCDEFFEDCBA9876543210";
            UInt128 value = UInt128.Parse(valueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(valueString, value.ToString("X32"));
        }
    }
}