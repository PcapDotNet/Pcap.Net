using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    /// <summary>
    /// Summary description for PcapLibTests.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PcapLibTests
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
        public void VersionTest()
        {
            const string VersionNumberRegex = @"[0-9]+\.[0-9]+(?:\.| beta)[0-9]+(?:\.[0-9]+)?";
            const string LibpcapVersionRegex = @"(?:[0-9]+\.[0-9]+\.[0-9]+(?:\.[0-9]+)?)|(?:[0-9]\.[0-9] branch [0-9]_[0-9]_rel0b \([0-9]+\))";
            // WinPcap version 4.1.1 (packet.dll version 4.1.0.1753), based on libpcap version 1.0 branch 1_0_rel0b (20091008)
            // WinPcap version 4.1 beta5 (packet.dll version 4.1.0.1452), based on libpcap version 1.0.0
            const string VersionRegex = "^WinPcap version " + VersionNumberRegex + @" \(packet\.dll version " + VersionNumberRegex + @"\), based on libpcap version " + LibpcapVersionRegex + "$";
            string version = PcapLibrary.Version;
            MoreAssert.IsMatch(VersionRegex, version);
        }
    }
}
