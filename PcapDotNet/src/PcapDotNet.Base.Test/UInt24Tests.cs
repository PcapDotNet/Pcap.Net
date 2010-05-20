using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for UInt24Tests
    /// </summary>
    [TestClass]
    public class UInt24Tests
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
        public void UInt24Test()
        {
            Random random = new Random();
            for (int i = 0; i != 1000; ++i)
            {
                UInt24 value = random.NextUInt24();

                Assert.AreEqual(value, value);
                // ReSharper disable EqualExpressionComparison
                Assert.IsTrue(value == value);
                Assert.IsFalse(value != value);
                // ReSharper restore EqualExpressionComparison
                Assert.AreNotEqual(value, "string");
                Assert.AreNotEqual(value, (UInt24)(((value & 0x00FFFF) + 1) | value & 0xFF0000));
                Assert.AreNotEqual(value, (UInt24)((value & 0x00FFFF) | ((value & 0xFF0000) + 0x010000)));
                Assert.IsNotNull(value.GetHashCode());
                Assert.AreEqual(((int)value).ToString(), value.ToString());
            }
        }
    }
}