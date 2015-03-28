using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for DataLinkTests
    /// </summary>
    [TestClass]
    public class DataLinkTests
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
        public void DataLinkTest()
        {
            Assert.AreEqual(DataLinkKind.Ethernet.ToString(), DataLink.Ethernet.ToString());
            Assert.AreEqual(DataLinkKind.PointToPointProtocolWithDirection.ToString(), new DataLink(DataLinkKind.PointToPointProtocolWithDirection).ToString());
            foreach (DataLink dataLink in new[] { DataLink.Ethernet, DataLink.IpV4 })
            {
                Assert.AreEqual(dataLink, dataLink);
                Assert.AreNotEqual(dataLink, 2);
                Assert.AreEqual(dataLink.GetHashCode(), dataLink.GetHashCode());
                // ReSharper disable EqualExpressionComparison
                Assert.IsTrue(dataLink == dataLink);
                Assert.IsFalse(dataLink != dataLink);
                // ReSharper restore EqualExpressionComparison
            }
        }
    }
}