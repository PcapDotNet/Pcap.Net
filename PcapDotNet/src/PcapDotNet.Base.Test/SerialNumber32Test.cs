using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for SerialNumber32Test
    /// </summary>
    [TestClass]
    public class SerialNumber32Test
    {
        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
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
        public void SimpleTest()
        {
            Assert.AreEqual<SerialNumber32>(1, 1);
            Assert.AreNotEqual<SerialNumber32>(1, 2);
            MoreAssert.IsBigger(1, 2);
            MoreAssert.IsSmaller(2, 1);

            SerialNumber32 serialNumber = 1;
            serialNumber = serialNumber.Add(10);
            Assert.AreEqual<SerialNumber32>(11, serialNumber);

            serialNumber = serialNumber.Add(((uint)1 << 31) - 1);
            Assert.AreEqual<SerialNumber32>(2147483658, serialNumber);
            MoreAssert.IsSmaller<SerialNumber32>(1, serialNumber);
            MoreAssert.IsBigger<SerialNumber32>(20, serialNumber);

            serialNumber = serialNumber.Add(((uint)1 << 31) - 1);
            Assert.AreEqual<SerialNumber32>(9, serialNumber);

            Assert.IsTrue(new SerialNumber32(1) < new SerialNumber32(2));
            Assert.IsTrue(new SerialNumber32(2) > new SerialNumber32(1));
            // ReSharper disable EqualExpressionComparison
            Assert.IsFalse(new SerialNumber32(1) < new SerialNumber32(1));
            Assert.IsFalse(new SerialNumber32(1) > new SerialNumber32(1));
            // ReSharper restore EqualExpressionComparison
            Assert.IsTrue(new SerialNumber32(2) != new SerialNumber32(1));
            Assert.IsFalse(new SerialNumber32(1) != new SerialNumber32(0).Add(1));
            Assert.IsTrue(new SerialNumber32(2) == new SerialNumber32(1).Add(1));
            Assert.IsFalse(new SerialNumber32(1).Equals(1.0));

            Assert.AreEqual("1", new SerialNumber32(1).ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = false)]
        public void OverflowAddTest()
        {
            SerialNumber32 serialNumber = 1;
            serialNumber = serialNumber.Add((uint)1 << 31);
            Assert.Fail(serialNumber.ToString());
        }

    }
}