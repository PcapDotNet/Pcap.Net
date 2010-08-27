using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
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
            TestHttpRequest("");
            TestHttpRequest(" ");
            TestHttpRequest("GET", "GET");
            TestHttpRequest("GET /url", "GET", "/url");
            TestHttpRequest("GET /url HTTP/1.0", "GET", "/url", HttpVersion.Version10);
            TestHttpRequest("GET /url HTTP/1.1", "GET", "/url", HttpVersion.Version11);
            TestHttpRequest("GET /url  HTTP/1.1", "GET", "/url");
            TestHttpRequest("GET  HTTP/1.1", "GET", "", HttpVersion.Version11);

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "Cache-Control: no-cache\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                new HttpField("Cache-Control", "no-cache")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B: C\r\n" +
                            "C: \r\n \r\n D\r\n" +
                            "D: E\r\n" +
                            "D: F\r\n" +
                            "E: G,H\r\n" +
                            "E: I,J\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                new HttpField("A", "B"),
                                new HttpField("B", "C"),
                                new HttpField("C", "D"),
                                new HttpField("D", "E,F"),
                                new HttpField("E", "G,H,I,J")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "\r\n" +
                            "A: B\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(), string.Empty);

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A: B\r\n" +
                            "\r\n" +
                            "B: C\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                new HttpField("A", "B")), string.Empty);

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A: B\r\n" +
                            "abc\r\n" +
                            "B: C\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                new HttpField("A", "B")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A: B\r\n" +
                            "abc:\r\n" +
                            "B: C\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                new HttpField("A", "B"),
                                new HttpField("abc", ""),
                                new HttpField("B", "C")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(),
                            string.Empty);


            TestHttpResponse("HTTP/");
            TestHttpResponse("HTTP/1");
            TestHttpResponse("HTTP/1.");
            TestHttpResponse("HTTP/1.0", HttpVersion.Version10);
            TestHttpResponse("HTTP/1.0 ", HttpVersion.Version10);
            TestHttpResponse("HTTP/1.0 A", HttpVersion.Version10);
            TestHttpResponse("HTTP/1.0 200", HttpVersion.Version10, 200);
            TestHttpResponse("HTTP/1.0 200 ", HttpVersion.Version10, 200, "");
            TestHttpResponse("HTTP/1.0 200 OK", HttpVersion.Version10, 200, "OK");
            TestHttpResponse("HTTP/1.1 200 OK", HttpVersion.Version11, 200, "OK");
            TestHttpResponse("HTTP/1.1  200 OK", HttpVersion.Version11);
            TestHttpResponse("HTTP/1.1 200  OK", HttpVersion.Version11, 200, " OK");

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "Cache-Control: no-cache\r\n",
                             HttpVersion.Version11, 200, "OK",
                             new HttpHeader(
                                 new HttpField("Cache-Control", "no-cache")));

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "Transfer-Encoding: chunked,a,   b   , c\r\n\t,d   , e;f=g;h=\"ijk lmn\"\r\n",
                             HttpVersion.Version11, 200, "OK",
                             new HttpHeader(
                                 new HttpTransferEncodingField("chunked", "a", "b", "c", "d", "e;f=g;h=\"ijk lmn\"")));

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "\r\n" +
                             "Body",
                             HttpVersion.Version11, 200, "OK", HttpHeader.Empty,
                             "Body");

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "Transfer-Encoding: chunked\r\n" +
                             "\r\n" +
                             "5\r\n" +
                             "This \r\n" +
                             "3;Extension\r\n" +
                             "is \r\n" +
                             "a;Extension=Value\r\n" +
                             "the 123456\r\n" +
                             "A;Extension=\"Quoted \\\" Value\"\r\n" +
                             "body 12345\r\n" +
                             "0\r\n",
                             HttpVersion.Version11, 200, "OK", new HttpHeader(new HttpTransferEncodingField("chunked")),
                             "5\r\n" +
                             "This \r\n" +
                             "3;Extension\r\n" +
                             "is \r\n" +
                             "a;Extension=Value\r\n" +
                             "the 123456\r\n" +
                             "A;Extension=\"Quoted \\\" Value\"\r\n" +
                             "body 12345\r\n" +
                             "0\r\n");

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "Content-Length: 16\r\n" +
                             "\r\n" +
                             "This is the body",
                             HttpVersion.Version11, 200, "OK", new HttpHeader(new HttpContentLengthField(16)),
                             "This is the body");

            TestHttpResponse("HTTP/1.1 206 Partial content\r\n" +
                             "Date: Wed, 15 Nov 1995 06:25:24 GMT\r\n" +
                             "Last-modified: Wed, 15 Nov 1995 04:58:08 GMT\r\n" +
                             "Content-type: multipart/byteranges; boundary=THIS_STRING_SEPARATES\r\n" +
                             "\r\n" +
                             "--THIS_STRING_SEPARATES\r\n" +
                             "Content-type: application/pdf\r\n" +
                             "Content-range: bytes 500-999/8000\r\n" +
                             "\r\n" +
                             "...the first range...\r\n" +
                             "--THIS_STRING_SEPARATES\r\n" +
                             "Content-type: application/pdf\r\n" +
                             "Content-range: bytes 7000-7999/8000\r\n" +
                             "\r\n" +
                             "...the second range\r\n" +
                             "--THIS_STRING_SEPARATES--",
                             HttpVersion.Version11, 206, "Partial content",
                             new HttpHeader(
                                 new HttpField("Date", "Wed, 15 Nov 1995 06:25:24 GMT"),
                                 new HttpField("Last-modified", "Wed, 15 Nov 1995 04:58:08 GMT"),
                                 new HttpContentTypeField("multipart", "byteranges",
                                                          new HttpFieldParameters(new KeyValuePair<string, string>("boundary", "THIS_STRING_SEPARATES")))),
                             "--THIS_STRING_SEPARATES\r\n" +
                             "Content-type: application/pdf\r\n" +
                             "Content-range: bytes 500-999/8000\r\n" +
                             "\r\n" +
                             "...the first range...\r\n" +
                             "--THIS_STRING_SEPARATES\r\n" +
                             "Content-type: application/pdf\r\n" +
                             "Content-range: bytes 7000-7999/8000\r\n" +
                             "\r\n" +
                             "...the second range\r\n" +
                             "--THIS_STRING_SEPARATES--");
        }

        private static void TestHttpRequest(string httpString, string expectedMethod = null, string expectedUri = null, HttpVersion expectedVersion = null, HttpHeader expectedHeader = null, string expectedBodyString = null)
        {
            Datagram expectedBody = expectedBodyString == null ? null : new Datagram(Encoding.ASCII.GetBytes(expectedBodyString));

            Packet packet = BuildPacket(httpString);

            // HTTP
            HttpDatagram http = packet.Ethernet.IpV4.Tcp.Http;
            Assert.IsTrue(http.IsRequest, "IsRequest " + httpString);
            Assert.IsFalse(http.IsResponse, "IsResponse " + httpString);
            Assert.AreEqual(expectedVersion, http.Version, "Version " + httpString);
            Assert.AreEqual(expectedHeader, http.Header, "Header " + httpString);
            if (expectedHeader != null)
                Assert.AreEqual(expectedHeader.ToString(), http.Header.ToString(), "Header " + httpString);

            HttpRequestDatagram request = (HttpRequestDatagram)http;
            Assert.AreEqual(expectedMethod, request.Method, "Method " + httpString);
            Assert.AreEqual(expectedUri, request.Uri, "Uri " + httpString);
            Assert.AreEqual(expectedBody, request.Body, "Body " + httpString);
        }

        private static void TestHttpResponse(string httpString, HttpVersion expectedVersion = null, uint? expectedStatusCode = null, string expectedReasonPhrase = null, HttpHeader expectedHeader = null, string expectedBodyString = null)
        {
            Datagram expectedBody = expectedBodyString == null ? null : new Datagram(Encoding.ASCII.GetBytes(expectedBodyString));

            Packet packet = BuildPacket(httpString);

            // HTTP
            HttpDatagram http = packet.Ethernet.IpV4.Tcp.Http;
            Assert.IsFalse(http.IsRequest, "IsRequest " + httpString);
            Assert.IsTrue(http.IsResponse, "IsResponse " + httpString);
            Assert.AreEqual(expectedVersion, http.Version, "Version " + httpString);
            Assert.AreEqual(expectedHeader, http.Header, "Header " + httpString);
            if (expectedHeader != null)
                Assert.IsNotNull(http.Header.ToString());

            HttpResponseDatagram response = (HttpResponseDatagram)http;
            Assert.AreEqual(expectedStatusCode, response.StatusCode, "StatusCode " + httpString);
            Assert.AreEqual(expectedReasonPhrase == null ? null : new Datagram(Encoding.ASCII.GetBytes(expectedReasonPhrase)), response.ReasonPhrase, "ReasonPhrase " + httpString);
            Assert.AreEqual(expectedBody, response.Body);
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