using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PcapLibTests
    /// </summary>
    [TestClass]
    public class PcapLibTests
    {
        public PcapLibTests()
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
        public void VersionTest()
        {
            const string VersionNumberRegex = @"[0-9]+\.[0-9]+\.[0-9]+(?:\.[0-9]+)?";
            const string VersionRegex = "^WinPcap version " + VersionNumberRegex + @" \(packet\.dll version " + VersionNumberRegex + @"\), based on libpcap version " + VersionNumberRegex + "$";
            string version = PcapLibrary.Version;
            Assert.IsTrue(Regex.IsMatch(version, VersionRegex), "Version " + version + " doesn't match regex " + VersionRegex);
        }
    }
}
