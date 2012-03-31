using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for BitSequenceTest
    /// </summary>
    [TestClass]
    public class BitSequenceTest
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
        public void Merge8BoolRandomTest()
        {
            Random random = new Random();
            for (int i = 0; i != 10; ++i)
            {
                byte expectedResult = 0;
                bool[] input = new bool[8];
                for (int bit = 0; bit != 8; ++bit)
                {
                    bool bitValue = random.NextBool();
                    input[bit] = bitValue;
                    expectedResult <<= 1;
                    if (bitValue)
                        ++expectedResult;
                }

                Assert.AreEqual(expectedResult, BitSequence.Merge(input[0], input[1], input[2], input[3], input[4], input[5], input[6], input[7]));
            }
        }
    }
}