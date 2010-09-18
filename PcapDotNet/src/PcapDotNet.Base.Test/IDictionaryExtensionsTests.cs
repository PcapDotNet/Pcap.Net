using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// Summary description for DictionaryExtensionsTests
    /// </summary>
    [TestClass]
    // ReSharper disable InconsistentNaming
    public class IDictionaryExtensionsTests
    // ReSharper restore InconsistentNaming
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
        public void DictionaryEqualsTest()
        {
            // Both null
            Dictionary<int, int> dic1 = null;
            Dictionary<int, int> dic2 = null;
            Assert.IsTrue(dic1.DictionaryEquals(dic2));
            Assert.IsTrue(dic2.DictionaryEquals(dic1));
      
            // One null
            dic1 = new Dictionary<int, int>();
            Assert.IsFalse(dic1.DictionaryEquals(dic2));
            Assert.IsFalse(dic2.DictionaryEquals(dic1));

            // Both empty
            dic2 = new Dictionary<int, int>();
            Assert.IsTrue(dic1.DictionaryEquals(dic2));
            Assert.IsTrue(dic2.DictionaryEquals(dic1));

            // Different count
            dic1.Add(1,1);
            Assert.IsFalse(dic1.DictionaryEquals(dic2));
            Assert.IsFalse(dic2.DictionaryEquals(dic1));

            // Different key
            dic2.Add(2, 1);
            Assert.IsFalse(dic1.DictionaryEquals(dic2));
            Assert.IsFalse(dic2.DictionaryEquals(dic1));

            // Different value
            dic1.Add(2, 1);
            dic2.Add(1, 2);
            Assert.IsFalse(dic1.DictionaryEquals(dic2));
            Assert.IsFalse(dic2.DictionaryEquals(dic1));
        }
    }
}