using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for ByteArrayExtensionsTests
    /// </summary>
    [TestClass]
    public class ByteArrayExtensionsTests
    {
        public ByteArrayExtensionsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void WriteByteNullBufferTest()
        {
            ByteArrayExtensions.Write(null, 0, 1);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void WriteEnumerableNullBufferTest()
        {
            int offset = 0;
            ByteArrayExtensions.Write(null, ref offset, new byte[0]);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void WriteEnumerableNullEnumerableTest()
        {
            int offset = 0;
            new byte[0].Write(ref offset, (IEnumerable<byte>)null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void WriteDatagramNullBufferTest()
        {
            new byte[0].Write(0, null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void WriteRefDatagramNullBufferTest()
        {
            int offset = 0;
            new byte[0].Write(ref offset, null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void ReadByteNullBufferTest()
        {
            Assert.IsNotNull(ByteArrayExtensions.ReadByte(null, 0));
            Assert.Fail();
        }
    }
}