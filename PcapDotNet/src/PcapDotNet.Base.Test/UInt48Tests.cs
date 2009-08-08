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
        public void ParseTest()
        {
            Random random = new Random();
            for (int i = 0; i != 100; ++i)
            {
                UInt48 expected = (UInt48)random.NextLong(UInt48.MaxValue + 1);
                UInt48 actual = UInt48.Parse(expected.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void UInt48Test()
        {
            Random random = new Random();
            for (int i = 0; i != 1000; ++i)
            {
                UInt48 value = random.NextUInt48();

                Assert.AreEqual(value, value);
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
