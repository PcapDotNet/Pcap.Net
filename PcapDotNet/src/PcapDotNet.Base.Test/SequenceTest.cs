using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for SequenceTest
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SequenceTest
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
        public void GetHashCodeNullValue1Test()
        {
            Assert.AreEqual(1.GetHashCode(), Sequence.GetHashCode(null, 1));
        }

        [TestMethod]
        public void GetHashCodeNullValue2Test()
        {
            Assert.AreEqual(1.GetHashCode(), Sequence.GetHashCode(1, null));
        }

        [TestMethod]
        public void GetHashCodeNullValue3Test()
        {
            Assert.AreEqual(Sequence.GetHashCode(1, 2), Sequence.GetHashCode(1, 2, null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void GetHashCodeNullValuesTest()
        {
            Assert.IsNotNull(Sequence.GetHashCode(null));
            Assert.Fail();
        }
    }
}