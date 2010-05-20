using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for MemberInfoExtensionsTests
    /// </summary>
    [TestClass]
    public class MemberInfoExtensionsTests
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
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void GetCustomAttributesNullTest()
        {
            Assert.IsNotNull(MemberInfoExtensions.GetCustomAttributes<Attribute>(null, true));
            Assert.Fail();
        }
    }
}