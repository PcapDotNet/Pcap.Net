using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for DateTimeExtensionsTest
    /// </summary>
    [TestClass]
    public class DateTimeExtensionsTest
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
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void AddMicrosecondsValueTooBigTest()
        {
            DateTime dateTime = DateTime.Now;
            dateTime.AddMicroseconds((long.MaxValue / TimeSpanExtensions.TicksPerMicrosecond) * 2);
            Assert.IsNotNull(dateTime);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void AddMicrosecondsValueTooSmallTest()
        {
            DateTime dateTime = DateTime.Now;
            dateTime.AddMicroseconds((long.MinValue / TimeSpanExtensions.TicksPerMicrosecond) * 2);
            Assert.IsNotNull(dateTime);
            Assert.Fail();
        }
    }
}