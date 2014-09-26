using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for PppFrameCheckSequenceCalculatorTests
    /// </summary>
    [TestClass]
    public class PppFrameCheckSequenceCalculatorTests
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
        public void RandomFcs16Test()
        {
            Random random = new Random();
            for (int i = 0; i != 100; ++i)
            {
                DataSegment data = random.NextDataSegment(random.Next(1000));
                ushort fcs = PppFrameCheckSequenceCalculator.CalculateFrameCheckSequence16(data);
            }
        }

        [TestMethod]
        public void GoodFcs16Test()
        {
            const ushort GoodFcs16 = 0xf0b8;

            for (int fcs16Value = 0; fcs16Value <= ushort.MaxValue; ++fcs16Value)
            {
                ushort extraValue = (ushort)(fcs16Value ^ 0xffff); // Complement.
                Assert.AreEqual(GoodFcs16, PppFrameCheckSequenceCalculator.CalculateFrameCheckSequence16((ushort)fcs16Value, new[] { (byte)extraValue, (byte)(extraValue >> 8) }));
            }
        }
    }
}