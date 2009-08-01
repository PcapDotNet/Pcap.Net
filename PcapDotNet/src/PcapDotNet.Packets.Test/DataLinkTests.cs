using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for DataLinkTests
    /// </summary>
    [TestClass]
    public class DataLinkTests
    {
        public DataLinkTests()
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
        public void DataLinkTest()
        {
            Assert.AreEqual(DataLink.Ethernet, DataLink.Ethernet);
            Assert.AreEqual(DataLinkKind.Ethernet.ToString(), DataLink.Ethernet.ToString());
            Assert.AreEqual(DataLink.Ethernet.GetHashCode(), DataLink.Ethernet.GetHashCode());
            Assert.IsTrue(DataLink.Ethernet == DataLink.Ethernet);
        }
    }
}