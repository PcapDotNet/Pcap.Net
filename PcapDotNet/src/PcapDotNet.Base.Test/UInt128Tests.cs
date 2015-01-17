using System;
using System.Globalization;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for UInt128Tests
    /// </summary>
    [TestClass]
    public class UInt128Tests
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
        public void UInt128Test()
        {
            Random random = new Random();
            for (int i = 0; i != 1000; ++i)
            {
                UInt128 value = random.NextUInt128();

                // Test comparisons.
                Assert.AreEqual(value, value);
                Assert.AreNotEqual(value, string.Empty);
                Assert.AreNotEqual(value, UInt128.MaxValue);
                Assert.AreNotEqual(value, UInt128.Zero);
                // ReSharper disable EqualExpressionComparison
                Assert.IsTrue(value == value);
                Assert.IsFalse(value != value);
                Assert.IsTrue(value <= value);
                Assert.IsTrue(value >= value);
                // ReSharper restore EqualExpressionComparison
                if (value != UInt128.MaxValue)
                {
                    Assert.IsTrue(value < value + 1);
                    Assert.IsTrue(value <= value + 1);
                    Assert.IsTrue(value + 1 > value);
                    Assert.IsTrue(value + 1 >= value);
                }

                // Test GetHashCode
                Assert.IsNotNull(value.GetHashCode());

                // Test Parse()
                Assert.AreEqual(value, UInt128.Parse(value.ToString()));
                Assert.AreEqual(value, UInt128.Parse(value.ToString(), CultureInfo.InvariantCulture));
                Assert.AreEqual(value, UInt128.Parse(value.ToString(), NumberStyles.Integer));

                // Test TryParse()
                UInt128 actualValue;
                Assert.IsTrue(UInt128.TryParse(value.ToString(), out actualValue));
                Assert.AreEqual(value, actualValue);
                Assert.IsTrue(UInt128.TryParse(value.ToString(CultureInfo.InvariantCulture), out actualValue));
                Assert.AreEqual(value, actualValue);

                // Cast to UInt64
                ulong smallValue = random.NextULong();
                Assert.AreEqual(smallValue, (ulong)((UInt128)smallValue));
            }
        }

        [TestMethod]
        public void CastToULongOverflow()
        {
            Random random = new Random();
            UInt128 value;
            ulong overflow = random.NextULong(ulong.MaxValue);
            try
            {
                value = (UInt128)(((BigInteger)ulong.MaxValue) + overflow + 1);
            }
            catch (Exception)
            {
                Assert.Fail();
                return;
            }
            Assert.AreEqual(overflow, (ulong)value); 
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException), AllowDerivedTypes = false)]
        public void ParseOverflow()
        {
            Assert.AreEqual(0, UInt128.Parse("-1"));
            Assert.Fail();
        }

        [TestMethod]
        public void TryParseOverflow()
        {
            UInt128 actual;
            Assert.IsFalse(UInt128.TryParse("-1", out actual));
            Assert.AreEqual(UInt128.Zero, actual);
            Assert.IsFalse(UInt128.TryParse((UInt128.MaxValue + BigInteger.One).ToString(), out actual));
            Assert.AreEqual(UInt128.Zero, actual);
        }

        [TestMethod]
        public void ShiftRightTest()
        {
            const string ValueString = "0123456789ABCDEFFEDCBA9876543210";
            UInt128 value = UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture), value);

            for (int i = 0; i <= 124; i += 4)
            {
                string expectedValueString = new string('0', i / 4) + ValueString.Substring(0, ValueString.Length - i / 4);
                UInt128 expectedValue = UInt128.Parse(expectedValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                Assert.AreEqual(expectedValue, value >> i, i.ToString());
                Assert.AreEqual(expectedValue, value >> (i / 2) >> (i / 2), i.ToString());
                Assert.AreEqual(expectedValue, value >> (i / 4) >> (i / 4) >> (i / 4) >> (i / 4), i.ToString());
            }

            Assert.AreEqual<UInt128>(value >> 128, 0);
        }

        [TestMethod]
        public void SumTest()
        {
            UInt128 value1 = 0;
            UInt128 value2 = 0;
            Assert.AreEqual<UInt128>(0, value1 + value2);

            value1 = 1;
            Assert.AreEqual<UInt128>(1, value1 + value2);
            
            value2 = 1;
            Assert.AreEqual<UInt128>(2, value1 + value2);

            value1 = 100;
            Assert.AreEqual<UInt128>(101, value1 + value2);

            value2 = 1000;
            Assert.AreEqual<UInt128>(1100, value1 + value2);

            value1 = ulong.MaxValue;
            value2 = 0;
            Assert.AreEqual(ulong.MaxValue, value1 + value2);

            value2 = 1;
            Assert.AreEqual(new UInt128(1,0), value1 + value2);

            value2 = 2;
            Assert.AreEqual(new UInt128(1, 1), value1 + value2);

            value2 = ulong.MaxValue;
            Assert.AreEqual(new UInt128(1, ulong.MaxValue - 1), value1 + value2);

            value1 = 2;
            value2 = new UInt128(1000, ulong.MaxValue);
            Assert.AreEqual(new UInt128(1001, 1), value1 + value2);

            value1 = new UInt128(100, ulong.MaxValue / 2 + 1);
            value2 = new UInt128(1000, ulong.MaxValue / 2 + 2);
            Assert.AreEqual(new UInt128(1101, 1), value1 + value2);

            value1 = new UInt128(ulong.MaxValue / 2, ulong.MaxValue / 2 + 1);
            value2 = new UInt128(ulong.MaxValue / 2, ulong.MaxValue / 2 + 2);
            Assert.AreEqual(new UInt128(ulong.MaxValue, 1), value1 + value2);

            value1 = new UInt128(ulong.MaxValue / 2 + 1, ulong.MaxValue / 2 + 1);
            value2 = new UInt128(ulong.MaxValue / 2, ulong.MaxValue / 2 + 2);
            Assert.AreEqual(new UInt128(0, 1), value1 + value2);
        }

        [TestMethod]
        public void Substract()
        {
            UInt128 value1 = 0;
            UInt128 value2 = 0;
            Assert.AreEqual<UInt128>(0, value1 - value2);

            value1 = 1;
            Assert.AreEqual<UInt128>(1, value1 - value2);

            value2 = 1;
            Assert.AreEqual<UInt128>(0, value1 - value2);

            value1 = 100;
            Assert.AreEqual<UInt128>(99, value1 - value2);

            value1 = new UInt128(1, 0);
            value2 = 0;
            Assert.AreEqual<UInt128>(value1, value1 - value2);

            value2 = 1;
            Assert.AreEqual<UInt128>(ulong.MaxValue, value1 - value2);

            value2 = 2;
            Assert.AreEqual<UInt128>(ulong.MaxValue - 1, value1 - value2);

            value1 = new UInt128(100, 1);
            Assert.AreEqual<UInt128>(new UInt128(99, ulong.MaxValue), value1 - value2);

            value1 = 1;
            Assert.AreEqual<UInt128>(UInt128.MaxValue, value1 - value2);
        }

        [TestMethod]
        public void BitwiseAndTest()
        {
            const string ValueString = "0123456789ABCDEFFEDCBA9876543210";
            UInt128 value = UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture), value);

            for (int i = 0; i <= 32; ++i)
            {
                string andValueString = new string('0', i) + new string('F', ValueString.Length - i);
                UInt128 andValue = UInt128.Parse("0" + andValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                string expectedValueString = new string('0', i) + ValueString.Substring(i, ValueString.Length - i);
                UInt128 expectedValue = UInt128.Parse(expectedValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                UInt128 actualValue = value & andValue;
                Assert.AreEqual(expectedValue, actualValue, i.ToString());
            }
        }

        [TestMethod]
        public void BitwiseOrTest()
        {
            const string ValueString = "0123456789ABCDEFFEDCBA9876543210";
            UInt128 value = UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture), value);

            for (int i = 0; i <= 32; ++i)
            {
                string orValueString = new string('0', i) + new string('F', ValueString.Length - i);
                UInt128 orValue = UInt128.Parse("0" + orValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                string expectedValueString = ValueString.Substring(0, i) + new string('F', ValueString.Length - i);
                UInt128 expectedValue = UInt128.Parse(expectedValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                UInt128 actualValue = value | orValue;
                Assert.AreEqual(expectedValue, actualValue, i.ToString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void ParseNullTest()
        {
            Assert.IsNotNull(UInt128.Parse(null, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            Assert.Fail();
        }

        [TestMethod]
        public void ToStringTest()
        {
            const string ValueString = "1234567890abcdeffedcba0987654321";
            UInt128 value = UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(ValueString, value.ToString("x32"));
        }

        [TestMethod]
        public void ToStringTestFirstBitIsOne()
        {
            const string ValueString = "fedcba9876543210fedcba9876543210";
            UInt128 value = UInt128.Parse(ValueString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Assert.AreEqual(ValueString, value.ToString("x32"));
        }
    }
}