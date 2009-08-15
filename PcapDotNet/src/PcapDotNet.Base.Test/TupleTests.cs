using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for TupleTests
    /// </summary>
    [TestClass]
    public class TupleTests
    {
        public TupleTests()
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
        public void TupleTest()
        {
            Random random = new Random();

            for (int i = 0; i != 100; ++i)
            {
                Tuple<int, double> expected = new Tuple<int, double>(random.Next(), random.NextDouble());
                Tuple<int, double> equal = new Tuple<int, double>(expected.Value1, expected.Value2);
                Tuple<int, double> different = new Tuple<int, double>(expected.Value1 + 1, expected.Value2);

                Assert.AreEqual(expected, equal);
                Assert.AreEqual(expected.GetHashCode(), equal.GetHashCode());
                Assert.IsTrue(expected == equal);
                Assert.IsFalse(expected != equal);

                Assert.AreNotEqual(expected, different);
                Assert.AreNotEqual(expected.GetHashCode(), different.GetHashCode());
                Assert.IsFalse(expected == different);
                Assert.IsTrue(expected != different);
            }
        }
    }

    /// <summary>
    /// Summary description for DateTimeTests
    /// </summary>
    [TestClass]
    public class DateTimeTests
    {
        public DateTimeTests()
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
        public void DateTimeToLocalTimeTest()
        {
            const long ticks = 633737178954260865;
            Assert.AreEqual("Jerusalem Daylight Time", TimeZone.CurrentTimeZone.DaylightName);
            Assert.AreEqual("Israel Standard Time", TimeZoneInfo.Local.Id);

            DateTime dateTime = new DateTime(ticks, DateTimeKind.Local);
            Assert.IsTrue(TimeZoneInfo.Local.IsInvalidTime(dateTime));
        }
    }
}