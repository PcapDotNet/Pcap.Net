using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for DatagramTests
    /// </summary>
    [TestClass]
    public class DatagramTests
    {
        public DatagramTests()
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
        public void RandomDatagramTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                Datagram datagram = random.NextDatagram(random.Next(1024));

                Assert.AreEqual(datagram, new Datagram(new List<byte>(datagram).ToArray()));
                Assert.AreEqual(datagram.GetHashCode(), new Datagram(new List<byte>(datagram).ToArray()).GetHashCode());

                Assert.AreNotEqual(datagram, random.NextDatagram(random.Next(10 * 1024)));
                Assert.AreNotEqual(datagram.GetHashCode(), random.NextDatagram(random.Next(10 * 1024)).GetHashCode());

                if (datagram.Length != 0)
                {
                    Assert.AreNotEqual(datagram, Datagram.Empty);
                    Assert.AreNotEqual(datagram, random.NextDatagram(datagram.Length));
                    Assert.AreNotEqual(datagram.GetHashCode(), random.NextDatagram(datagram.Length).GetHashCode());
                }
                else
                    Assert.AreEqual(datagram, Datagram.Empty);
            }
        }
    }
}