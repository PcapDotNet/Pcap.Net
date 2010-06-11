using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Test
{
    /// <summary>
    /// Summary description for HttpTests
    /// </summary>
    [TestClass]
    public class HttpTests
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
        public void HttpParsingTest()
        {
            TestHttpRequest("", null, null, null);
            TestHttpRequest(" ", null, null, null);
            TestHttpRequest("GET", "GET", null, null);
            TestHttpRequest("GET /url", "GET", "/url", null);
            TestHttpRequest("GET /url HTTP/1.0", "GET", "/url", HttpVersion.Version10);
            TestHttpRequest("GET /url HTTP/1.1", "GET", "/url", HttpVersion.Version11);
            TestHttpRequest("GET /url  HTTP/1.1", "GET", "/url", null);
            TestHttpRequest("GET  HTTP/1.1", "GET", "", HttpVersion.Version11);

            TestHttpResponse("HTTP/", null, null, null);
            TestHttpResponse("HTTP/1", null, null, null);
            TestHttpResponse("HTTP/1.", null, null, null);
            TestHttpResponse("HTTP/1.0", HttpVersion.Version10, null, null);
            TestHttpResponse("HTTP/1.0 ", HttpVersion.Version10, null, null);
            TestHttpResponse("HTTP/1.0 A", HttpVersion.Version10, null, null);
            TestHttpResponse("HTTP/1.0 200", HttpVersion.Version10, 200, null);
            TestHttpResponse("HTTP/1.0 200 ", HttpVersion.Version10, 200, "");
            TestHttpResponse("HTTP/1.0 200 OK", HttpVersion.Version10, 200, "OK");
            TestHttpResponse("HTTP/1.1 200 OK", HttpVersion.Version11, 200, "OK");
            TestHttpResponse("HTTP/1.1  200 OK", HttpVersion.Version11, null, null);
            TestHttpResponse("HTTP/1.1 200  OK", HttpVersion.Version11, 200, " OK");
        }

        private static void TestHttpRequest(string httpString, string expectedMethod, string expectedUri, HttpVersion expectedVersion)
        {
            Packet packet = BuildPacket(httpString);

            // HTTP
            HttpDatagram http = packet.Ethernet.IpV4.Tcp.Http;
            Assert.IsTrue(http.IsRequest, "IsRequest " + httpString);
            Assert.IsFalse(http.IsResponse, "IsResponse " + httpString);
            Assert.AreEqual(expectedVersion, http.Version, "Version " + httpString);

            HttpRequestDatagram request = (HttpRequestDatagram)http;
            Assert.AreEqual(expectedMethod, request.Method, "Method " + httpString);
            Assert.AreEqual(expectedUri, request.Uri, "Uri " + httpString);

//            HttpHeader header = http.Header;
//            Assert.IsNotNull(header);
        }

        private static void TestHttpResponse(string httpString, HttpVersion expectedVersion, uint? expectedStatusCode, string expectedReasonPhrase)
        {
            Packet packet = BuildPacket(httpString);

            // HTTP
            HttpDatagram http = packet.Ethernet.IpV4.Tcp.Http;
            Assert.IsFalse(http.IsRequest, "IsRequest " + httpString);
            Assert.IsTrue(http.IsResponse, "IsResponse " + httpString);
            Assert.AreEqual(expectedVersion, http.Version, "Version " + httpString);

            HttpResponseDatagram response = (HttpResponseDatagram)http;
            Assert.AreEqual(expectedStatusCode, response.StatusCode, "StatusCode " + httpString);
            Assert.AreEqual(expectedReasonPhrase == null ? null : new Datagram(Encoding.ASCII.GetBytes(expectedReasonPhrase)), response.ReasonPhrase, "ReasonPhrase " + httpString);
        }

        private static Packet BuildPacket(string httpString)
        {
            MacAddress ethernetSource = new MacAddress("00:01:02:03:04:05");
            MacAddress ethernetDestination = new MacAddress("A0:A1:A2:A3:A4:A5");

            EthernetLayer ethernetLayer = new EthernetLayer
            {
                Source = ethernetSource,
                Destination = ethernetDestination
            };

            Random random = new Random();

            IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
            ipV4Layer.HeaderChecksum = null;
            TcpLayer tcpLayer = random.NextTcpLayer();

            PayloadLayer payloadLayer = new PayloadLayer
            {
                Data = new Datagram(Encoding.ASCII.GetBytes(httpString))
            };

            Packet packet = new PacketBuilder(ethernetLayer, ipV4Layer, tcpLayer, payloadLayer).Build(DateTime.Now);

            Assert.IsTrue(packet.IsValid);

            // Ethernet
            ethernetLayer.EtherType = EthernetType.IpV4;
            Assert.AreEqual(ethernetLayer, packet.Ethernet.ExtractLayer(), "Ethernet Layer");

            // IpV4
            ipV4Layer.Protocol = IpV4Protocol.Tcp;
            ipV4Layer.HeaderChecksum = ((IpV4Layer)packet.Ethernet.IpV4.ExtractLayer()).HeaderChecksum;
            Assert.AreEqual(ipV4Layer, packet.Ethernet.IpV4.ExtractLayer(), "IP Layer");
            ipV4Layer.HeaderChecksum = null;

            // TCP
            tcpLayer.Checksum = packet.Ethernet.IpV4.Tcp.Checksum;
            Assert.AreEqual(tcpLayer, packet.Ethernet.IpV4.Tcp.ExtractLayer(), "TCP Layer");

            return packet;
        }
    }
}