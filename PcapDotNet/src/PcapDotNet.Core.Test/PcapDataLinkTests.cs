using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PcapDataLinkTests
    /// </summary>
    [TestClass]
    public class PcapDataLinkTests
    {
        public PcapDataLinkTests()
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
        public void ConstructorsTest()
        {
            PcapDataLink dataLink = new PcapDataLink();
            Assert.AreEqual(new PcapDataLink("NULL"), dataLink);
            for (int i = 0; i != 1000; ++i)
            {
                dataLink = new PcapDataLink(i);
                string dataLinkName;
                try
                {
                    dataLinkName = dataLink.Name;
                }
                catch (InvalidOperationException)
                {
                    // Ignore invalid values
                    continue;
                }

                Assert.AreEqual(new PcapDataLink(dataLinkName), dataLink);
                Assert.IsNotNull(dataLink.Description);
                Assert.AreEqual(i, dataLink.Value);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void UnsupportedKindErrorTest()
        {
            PcapDataLink dataLink = new PcapDataLink();
            Assert.IsNotNull(dataLink.Kind);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoDescriptionErrorTest()
        {
            PcapDataLink dataLink = GetInvalidDataLink();
            Assert.IsNotNull(dataLink.Description);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidNameErrorTest()
        {
            PcapDataLink dataLink = new PcapDataLink("Invalid Name");
            Assert.IsNotNull(dataLink);
            Assert.Fail();
        }

        private static PcapDataLink GetInvalidDataLink()
        {
            for (int i = 0; i != 1000; ++i)
            {
                PcapDataLink dataLink = new PcapDataLink(i);
                string dataLinkName;
                try
                {
                    dataLinkName = dataLink.Name;
                    Assert.IsNotNull(dataLinkName);
                }
                catch (ArgumentException)
                {
                    return dataLink;
                }
            }
            Assert.Fail();
            return new PcapDataLink();
        }
    }
}