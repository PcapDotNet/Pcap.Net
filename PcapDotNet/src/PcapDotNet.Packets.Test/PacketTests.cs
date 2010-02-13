using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.TestUtils;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for PacketTests
    /// </summary>
    [TestClass]
    public class PacketTests
    {
        public PacketTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext{ get; set;}

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
        public void RandomPacketTest()
        {
            Random random = new Random();

            for (int i = 0; i != 1000; ++i)
            {
                Packet packet = random.NextPacket(random.Next(10 * 1024));

                // Check Equals
                Assert.AreEqual(packet, new Packet(packet.Buffer, packet.Timestamp.AddHours(1), packet.DataLink));
                Assert.AreNotEqual(packet, random.NextPacket(random.Next(10 * 1024)));
                if (packet.Length != 0)
                    Assert.AreNotEqual(packet, random.NextPacket(packet.Length));

                // Check GetHashCode
                Assert.AreEqual(packet.GetHashCode(), new Packet(packet.Buffer, packet.Timestamp.AddHours(1), packet.DataLink).GetHashCode());
                Assert.AreNotEqual(packet.GetHashCode(), random.NextPacket(random.Next(10 * 1024)).GetHashCode());
                if (packet.Length != 0)
                    Assert.AreNotEqual(packet.GetHashCode(), random.NextPacket(packet.Length).GetHashCode());

                // Check ToString
                Assert.IsNotNull(packet.ToString());

                Assert.IsFalse(new Packet(packet.Buffer, DateTime.Now, (DataLinkKind)((int)DataLinkKind.Ethernet + 1)).IsValid);

                // Check Enumerable
                IEnumerable enumerable = packet;
                int offset = 0;
                foreach (byte b in enumerable)
                    Assert.AreEqual(packet[offset++], b);

            }
        }

        [TestMethod]
        public void PacketIListTest()
        {
            byte[] buffer = new byte[]{1,2,3,4,5};
            IList<byte> packet = new Packet(buffer, DateTime.Now, DataLinkKind.Ethernet);

            Assert.IsTrue(packet.Contains(1));

            buffer = new byte[buffer.Length];
            packet.CopyTo(buffer, 0);
            packet.SequenceEqual(buffer);

            Assert.AreEqual(1, packet.IndexOf(2));
            Assert.AreEqual(buffer.Length, packet.Count);
            Assert.AreEqual(buffer[2], packet[2]);
            Assert.IsTrue(packet.IsReadOnly);
        }

        [TestMethod]
        public void MutationMethodsTest()
        {
            string[] methodNames = new[] {"Add", "Clear", "Insert", "Remove", "RemoveAt", "set_Item"};

            Packet packet = new Random().NextPacket(100);
            var methods = from method in typeof(Packet).GetMethods()
                          where (methodNames.Contains(method.Name))
                          select method;

            Assert.AreEqual(methodNames.Length, methods.Count());

            foreach (var method in methods)
            {
                var parameters = from parameter in method.GetParameters()
                                 select Activator.CreateInstance(parameter.ParameterType);
                try
                {
                    method.Invoke(packet, parameters.ToArray());
                }
                catch (TargetInvocationException e)
                {
                    Assert.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException));
                    continue;
                }

                Assert.Fail();
            }
        }
    }
}