using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Base.Test
{
    /// <summary>
    /// Summary description for IEnumerableExtensionsTests
    /// </summary>
    [TestClass]
// ReSharper disable InconsistentNaming
    public class IEnumerableExtensionsTests
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
        public void SequenceToStringTest()
        {
            int[] sequence = new[]{1,2,3,4,5};

            Assert.AreEqual("12345",sequence.SequenceToString());
        }

        [TestMethod]
        public void SequenceGetHashCodeTest()
        {
            int[] sequence = new[]{1,2,3,4,5};

            Assert.AreEqual(1.GetHashCode() ^ 2.GetHashCode() ^ 3.GetHashCode() ^ 4.GetHashCode() ^ 5.GetHashCode(), sequence.SequenceGetHashCode());
        }

        [TestMethod]
        public void BytesSequenceGetHashCodeTest()
        {
            byte[] sequence = new byte[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual((int)BitSequence.Merge(4, 3, 2, 1) ^ 5, sequence.BytesSequenceGetHashCode());
        }

        [TestMethod]
        public void ConcatTest()
        {
            int[] sequence = new[] {1, 2, 3, 4, 5};

            Assert.IsTrue(sequence.SequenceEqual(new[] {1,2,3}.Concat(4, 5)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void SequenceToStringNullTest()
        {
            Assert.IsNotNull(IEnumerableExtensions.SequenceToString<int>(null));
            Assert.Fail();
        }

        [TestMethod]
        public void XorTest()
        {
            int[] intValues = new[] {0x0001, 0x0020, 0x0300, 0x4000};
            Assert.AreEqual(0x4321, intValues.Xor());

            long[] longValues = new[]
                                {
                                    0x00000001L, 0x00000020L, 0x00000300L, 0x00004000L,
                                    0x00050000L, 0x00600000L, 0x07000000L, 0x80000000L
                                };
            Assert.AreEqual(0x87654321L, longValues.Xor());
        }
    }
}