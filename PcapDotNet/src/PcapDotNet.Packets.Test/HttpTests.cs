using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

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
        public void RandomHttpTest()
        {
           Random random = new Random();

            for (int i = 0; i != 200; ++i)
            {
                EthernetLayer ethernetLayer = random.NextEthernetLayer(EthernetType.None);
                IpV4Layer ipV4Layer = random.NextIpV4Layer(null);
                ipV4Layer.HeaderChecksum = null;
                TcpLayer tcpLayer = random.NextTcpLayer();
                tcpLayer.Checksum = null;
                HttpLayer httpLayer = random.NextHttpLayer();

                Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, tcpLayer, httpLayer);
                Assert.IsTrue(packet.IsValid, "IsValid");

                HttpDatagram httpDatagram = packet.Ethernet.IpV4.Tcp.Http;
                Assert.AreEqual(httpLayer.Version, httpDatagram.Version);
                if (httpLayer.Version != null)
                    Assert.AreEqual(httpLayer.Version.GetHashCode(), httpDatagram.Version.GetHashCode());
                if (httpLayer is HttpRequestLayer)
                {
                    Assert.IsTrue(httpDatagram.IsRequest);
                    Assert.IsTrue(httpLayer.IsRequest);
                    Assert.IsFalse(httpDatagram.IsResponse);
                    Assert.IsFalse(httpLayer.IsResponse);
                    
                    HttpRequestLayer httpRequestLayer = (HttpRequestLayer)httpLayer;
                    HttpRequestDatagram httpRequestDatagram = (HttpRequestDatagram)httpDatagram;
                    Assert.AreEqual(httpRequestLayer.Method, httpRequestDatagram.Method);
                    if (httpRequestLayer.Method != null)
                    {
                        Assert.AreEqual(httpRequestLayer.Method.GetHashCode(), httpRequestDatagram.Method.GetHashCode());
                        Assert.AreEqual(httpRequestLayer.Method.KnownMethod, httpRequestDatagram.Method.KnownMethod);
                    }
                    Assert.AreEqual(httpRequestLayer.Uri, httpRequestDatagram.Uri);
                }
                else
                {
                    Assert.IsFalse(httpDatagram.IsRequest);
                    Assert.IsFalse(httpLayer.IsRequest);
                    Assert.IsTrue(httpDatagram.IsResponse);
                    Assert.IsTrue(httpLayer.IsResponse);

                    HttpResponseLayer httpResponseLayer = (HttpResponseLayer)httpLayer;
                    HttpResponseDatagram httpResponseDatagram = (HttpResponseDatagram)httpDatagram;
                    Assert.AreEqual(httpResponseLayer.StatusCode, httpResponseDatagram.StatusCode);
                    Assert.AreEqual(httpResponseLayer.ReasonPhrase, httpResponseDatagram.ReasonPhrase);
                }
                Assert.AreEqual(httpLayer.Header, httpDatagram.Header);
                if (httpLayer.Header != null)
                {
                    Assert.AreEqual(httpLayer.Header.GetHashCode(), httpDatagram.Header.GetHashCode());

                    foreach (var field in httpLayer.Header)
                        Assert.IsFalse(field.Equals("abc"));

                    MoreAssert.AreSequenceEqual(httpLayer.Header.Select(field => field.GetHashCode()), httpDatagram.Header.Select(field => field.GetHashCode()));

                    if (httpLayer.Header.ContentType != null)
                    {
                        var parameters = httpLayer.Header.ContentType.Parameters;
                        Assert.IsNotNull(((IEnumerable)parameters).GetEnumerator());
                        Assert.AreEqual<object>(parameters, httpDatagram.Header.ContentType.Parameters);
                        Assert.AreEqual(parameters.GetHashCode(), httpDatagram.Header.ContentType.Parameters.GetHashCode());
                        Assert.AreEqual(parameters.Count, httpDatagram.Header.ContentType.Parameters.Count);
                        int maxParameterNameLength = parameters.Any() ? parameters.Max(pair => pair.Key.Length) : 0;
                        Assert.IsNull(parameters[new string('a', maxParameterNameLength + 1)]);
                    }
                }
                Assert.AreEqual(httpLayer.Body, httpDatagram.Body);
                Assert.AreEqual(httpLayer, httpDatagram.ExtractLayer(), "HTTP Layer");
                Assert.AreEqual(httpLayer.Length, httpDatagram.Length);
            }
        }

        [TestMethod]
        public void HttpParsingTest()
        {
            // Request First Line
            TestHttpRequest("");
            TestHttpRequest(" ");
            TestHttpRequest("G", "G");
            TestHttpRequest("GET", "GET");
            TestHttpRequest("GET ", "GET", "");
            TestHttpRequest("GET /url", "GET", "/url");
            TestHttpRequest("GET /url ", "GET", "/url");
            TestHttpRequest("GET /url H", "GET", "/url");
            TestHttpRequest("GET /url HTTP/", "GET", "/url");
            TestHttpRequest("GET /url HTTP/1.0", "GET", "/url", HttpVersion.Version10);
            TestHttpRequest("GET /url HTTP/1.1", "GET", "/url", HttpVersion.Version11);
            TestHttpRequest("GET /url HTTP/1.1A", "GET", "/url", HttpVersion.Version11);
            TestHttpRequest("GET /url  HTTP/1.1", "GET", "/url");
            TestHttpRequest("GET  HTTP/1.1", "GET", "", HttpVersion.Version11);
            TestHttpRequest("GET /url HTTP/1.1\r\n", "GET", "/url", HttpVersion.Version11, HttpHeader.Empty);

            // Request Header
            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "Cache-Control: no-cache\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                HttpField.CreateField("Cache-Control", "no-cache")));

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
                                HttpField.CreateField("A", "B"),
                                HttpField.CreateField("B", "C"),
                                HttpField.CreateField("C", "D"),
                                HttpField.CreateField("D", "E,F"),
                                HttpField.CreateField("E", "G,H,I,J")));

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
                                HttpField.CreateField("A", "B")), string.Empty);

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A: B\r\n" +
                            "abc\r\n" +
                            "B: C\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                HttpField.CreateField("A", "B")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A: B\r\n" +
                            "abc:\r\n" +
                            "B: C\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(
                                HttpField.CreateField("A", "B"),
                                HttpField.CreateField("abc", ""),
                                HttpField.CreateField("B", "C")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(HttpField.CreateField("A", "B")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(HttpField.CreateField("A", "B")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B\r\n" +
                            "C:D\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(HttpField.CreateField("A", "B")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B:",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(HttpField.CreateField("A", "B")));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B:\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(HttpField.CreateField("A", "B"),
                                           HttpField.CreateField("B", string.Empty)));

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "A:B\r\n" +
                            "B:\r\n" +
                            "C:D\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(HttpField.CreateField("A", "B"),
                                           HttpField.CreateField("B", string.Empty),
                                           HttpField.CreateField("C", "D")));

            // Request Body
            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                            "\r\n",
                            "GET", "/url", HttpVersion.Version11,
                            HttpHeader.Empty,
                            string.Empty);

            TestHttpRequest("GET /url HTTP/1.1\r\n" +
                "Content-Length: 10\r\n" +
                            "\r\n" +
                            "1234567890",
                            "GET", "/url", HttpVersion.Version11,
                            new HttpHeader(new HttpContentLengthField(10)),
                            "1234567890");

            // Response

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
            TestHttpResponse("HTTP/1.1 200  OK", HttpVersion.Version11, 200, "OK");

            // Response Header

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "Cache-Control: no-cache\r\n",
                             HttpVersion.Version11, 200, "OK",
                             new HttpHeader(
                                 HttpField.CreateField("Cache-Control", "no-cache")));

            TestHttpResponse("HTTP/1.1 200 OK\r\n" +
                             "Transfer-Encoding: chunked,a,   b   , c\r\n\t,d   , e;f=g;h=\"ijk lmn\"\r\n",
                             HttpVersion.Version11, 200, "OK",
                             new HttpHeader(
                                 new HttpTransferEncodingField("chunked", "a", "b", "c", "d", "e;f=g;h=\"ijk lmn\"")));

            // Respone Body

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
                             "Transfer-Encoding: chunked\r\n" +
                             "\r\n" +
                             "g\r\n" +
                             "12345678901234567890\r\n" +
                             "0\r\n",
                             HttpVersion.Version11, 200, "OK", new HttpHeader(new HttpTransferEncodingField("chunked")),
                             string.Empty);

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
                                 HttpField.CreateField("Date", "Wed, 15 Nov 1995 06:25:24 GMT"),
                                 HttpField.CreateField("Last-modified", "Wed, 15 Nov 1995 04:58:08 GMT"),
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

        [TestMethod]
        public void HttpParsingMultipeRequestsTest()
        {
            const string HttpString = "GET /url1 HTTP/1.1\r\n" +
                                      "A: B\r\n" +
                                      "B: C\r\n" +
                                      "\r\n" +
                                      "GET /url2 HTTP/1.1\r\n" +
                                      "C: D\r\n" +
                                      "D: E\r\n" +
                                      "\r\n";

            Packet packet = BuildPacket(HttpString);
            var https = packet.Ethernet.IpV4.Tcp.HttpCollection;
            Assert.AreEqual(2, https.Count);
            foreach (HttpDatagram http in https)
            {
                Assert.IsTrue(http.IsRequest);
                Assert.IsFalse(http.IsResponse);
                Assert.AreEqual(HttpVersion.Version11, http.Version);
                Assert.AreEqual(Datagram.Empty, http.Body);
            }
            HttpRequestDatagram request = (HttpRequestDatagram)https[0];
            Assert.AreEqual(new HttpRequestMethod(HttpRequestKnownMethod.Get), request.Method);
            Assert.AreEqual("/url1", request.Uri);
            Assert.AreEqual(new HttpHeader(HttpField.CreateField("A", "B"), HttpField.CreateField("B", "C")), request.Header);

            request = (HttpRequestDatagram)https[1];
            Assert.AreEqual(new HttpRequestMethod(HttpRequestKnownMethod.Get), request.Method);
            Assert.AreEqual("/url2", request.Uri);
            Assert.AreEqual(new HttpHeader(HttpField.CreateField("C", "D"), HttpField.CreateField("D", "E")), request.Header);
        }

        [TestMethod]
        public void HttpParsingMultipeResponsesTest()
        {
            const string HttpString = "HTTP/1.1 200 OK\r\n" +
                                      "Transfer-Encoding: chunked\r\n" +
                                      "\r\n" +
                                      "b\r\n" +
                                      "12345678901\r\n" +
                                      "0\r\n" +
                                      "HTTP/1.1 404 Not Found\r\n" +
                                      "Content-Length: 6\r\n" +
                                      "\r\n" +
                                      "123456" +
                                      "HTTP/1.1 302 Moved\r\n" +
                                      "Transfer-Encoding: chunked\r\n" +
                                      "\r\n" +
                                      "8\r\n" +
                                      "12345678\r\n" +
                                      "0\r\n";

            Packet packet = BuildPacket(HttpString);
            var https = packet.Ethernet.IpV4.Tcp.HttpCollection;
            Assert.AreEqual(3, https.Count);
            foreach (HttpDatagram http in https)
            {
                Assert.IsFalse(http.IsRequest);
                Assert.IsTrue(http.IsResponse);
                Assert.AreEqual(HttpVersion.Version11, http.Version);
            }
            HttpResponseDatagram response = (HttpResponseDatagram)https[0];
            Assert.AreEqual(new Datagram(Encoding.ASCII.GetBytes("b\r\n12345678901\r\n0\r\n")), response.Body);
            Assert.AreEqual(new HttpHeader(new HttpTransferEncodingField("chunked")), response.Header);
            Assert.AreEqual(new Datagram(Encoding.ASCII.GetBytes("OK")), response.ReasonPhrase);
            Assert.AreEqual<uint>(200, response.StatusCode.Value);

            response = (HttpResponseDatagram)https[1];
            Assert.AreEqual(new Datagram(Encoding.ASCII.GetBytes("123456")), response.Body);
            Assert.AreEqual(new HttpHeader(new HttpContentLengthField(6)), response.Header);
            Assert.AreEqual(new Datagram(Encoding.ASCII.GetBytes("Not Found")), response.ReasonPhrase);
            Assert.AreEqual<uint>(404, response.StatusCode.Value);

            response = (HttpResponseDatagram)https[2];
            Assert.AreEqual(new Datagram(Encoding.ASCII.GetBytes("8\r\n12345678\r\n0\r\n")), response.Body);
            Assert.AreEqual(new HttpHeader(new HttpTransferEncodingField("chunked")), response.Header);
            Assert.AreEqual(new Datagram(Encoding.ASCII.GetBytes("Moved")), response.ReasonPhrase);
            Assert.AreEqual<uint>(302, response.StatusCode.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
        public void HttpRequestMethodBadKnownTest()
        {
            Assert.IsNotNull(new HttpRequestMethod(HttpRequestKnownMethod.Unknown));
        }

        private static void TestHttpRequest(string httpString, string expectedMethodString = null, string expectedUri = null, HttpVersion expectedVersion = null, HttpHeader expectedHeader = null, string expectedBodyString = null)
        {
            Datagram expectedBody = expectedBodyString == null ? null : new Datagram(Encoding.ASCII.GetBytes(expectedBodyString));
            HttpRequestMethod expectedMethod = expectedMethodString == null ? null : new HttpRequestMethod(expectedMethodString);

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

        [TestMethod]
        public void HttpResponseWithoutVersionStatusCodeOrReasonPhraseTest()
        {
            HttpResponseLayer httpLayer = new HttpResponseLayer();
            PacketBuilder builder = new PacketBuilder(new EthernetLayer(),
                                                      new IpV4Layer(),
                                                      new TcpLayer(),
                                                      httpLayer);

            // null version
            Packet packet = builder.Build(DateTime.Now);
            Assert.IsNull(packet.Ethernet.IpV4.Tcp.Http.Version);

            // null status code
            httpLayer.Version = HttpVersion.Version11;
            packet = builder.Build(DateTime.Now);
            Assert.IsNotNull(packet.Ethernet.IpV4.Tcp.Http.Version);
            Assert.IsNull(((HttpResponseDatagram)packet.Ethernet.IpV4.Tcp.Http).StatusCode);

            // null reason phrase
            httpLayer.StatusCode = 200;
            packet = builder.Build(DateTime.Now);
            Assert.IsNotNull(packet.Ethernet.IpV4.Tcp.Http.Version);
            Assert.IsNotNull(((HttpResponseDatagram)packet.Ethernet.IpV4.Tcp.Http).StatusCode);
            Assert.AreEqual(Datagram.Empty, ((HttpResponseDatagram)packet.Ethernet.IpV4.Tcp.Http).ReasonPhrase, "ReasonPhrase");
        }

        [TestMethod]
        public void HttpRequestWithoutUriTest()
        {
           PacketBuilder builder = new PacketBuilder(new EthernetLayer(),
                                                      new IpV4Layer(),
                                                      new TcpLayer(),
                                                      new HttpRequestLayer
                                                      {
                                                          Method = new HttpRequestMethod("UnknownMethod")
                                                      });

            Packet packet = builder.Build(DateTime.Now);
            Assert.IsNotNull(((HttpRequestDatagram)packet.Ethernet.IpV4.Tcp.Http).Method);
            Assert.AreEqual(HttpRequestKnownMethod.Unknown, ((HttpRequestDatagram)packet.Ethernet.IpV4.Tcp.Http).Method.KnownMethod);
            Assert.AreEqual(string.Empty, ((HttpRequestDatagram)packet.Ethernet.IpV4.Tcp.Http).Uri, "Uri");
        }

        [TestMethod]
        public void HttpBadTransferCodingsRegexTest()
        {
            Packet packet = BuildPacket("GET /url HTTP/1.1\r\n" +
                                        "Transfer-Encoding: ///\r\n" +
                                        "\r\n");
            Assert.IsNull(packet.Ethernet.IpV4.Tcp.Http.Header.TransferEncoding.TransferCodings);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void HttpCreateFieldNullEncodingTest()
        {
            HttpField field = HttpField.CreateField("abc", "cde", null);
            Assert.IsNull(field);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void HttpCreateFieldNullNameTest()
        {
            HttpField field = HttpField.CreateField(null, "cde");
            Assert.IsNull(field);
            Assert.Fail();
        }

        [TestMethod]
        public void HttpNonAsciiUriTest()
        {
            Packet packet = Packet.FromHexadecimalString(
                "0013f7a44dc0000c2973b9bb0800450001642736400080060000c0a80126c0002b0a12710050caa94b1f450b454950180100ae2f0000474554202f442543332542437273742" +
                "f3fefbca120485454502f312e310d0a4163636570743a20746578742f68746d6c2c206170706c69636174696f6e2f7868746d6c2b786d6c2c202a2f2a0d0a52656665726572" +
                "3a20687474703a2f2f6c6f6f6b6f75742e6e65742f746573742f6972692f6d6978656e632e7068700d0a4163636570742d4c616e67756167653a20656e2d55530d0a5573657" +
                "22d4167656e743a204d6f7a696c6c612f352e302028636f6d70617469626c653b204d53494520392e303b2057696e646f7773204e5420362e313b20574f5736343b20547269" +
                "64656e742f352e30290d0a4163636570742d456e636f64696e673a20677a69702c206465666c6174650d0a486f73743a207777772e6578616d706c652e636f6d0d0a436f6e6" +
                "e656374696f6e3a204b6565702d416c6976650d0a0d0a",
                DateTime.Now, DataLinkKind.Ethernet);

            Assert.AreEqual("/D%C3%BCrst/?ï¼¡", ((HttpRequestDatagram)packet.Ethernet.IpV4.Tcp.Http).Uri);
        }

        [TestMethod]
        public void MultipleHttpLayersTest()
        {
            var httpLayer1 = new HttpResponseLayer
                             {
                                 Version = HttpVersion.Version11,
                                 StatusCode = 200,
                                 ReasonPhrase = new DataSegment(Encoding.ASCII.GetBytes("OK")),
                                 Header = new HttpHeader(new HttpContentLengthField(10)),
                                 Body = new Datagram(new byte[10])
                             };
            var httpLayer2 = new HttpResponseLayer
                             {
                                 Version = HttpVersion.Version11,
                                 StatusCode = 201,
                                 ReasonPhrase = new DataSegment(Encoding.ASCII.GetBytes("MOVED")),
                                 Header = new HttpHeader(new HttpContentLengthField(20)),
                                 Body = new Datagram(new byte[20])
                             };
            Packet packet = PacketBuilder.Build(DateTime.Now, new EthernetLayer(), new IpV4Layer(), new TcpLayer(), httpLayer1, httpLayer2);
            Assert.AreEqual(2, packet.Ethernet.IpV4.Tcp.HttpCollection.Count);
            Assert.AreEqual(httpLayer1, packet.Ethernet.IpV4.Tcp.Http.ExtractLayer());
            Assert.AreEqual(httpLayer2, packet.Ethernet.IpV4.Tcp.HttpCollection[1].ExtractLayer());
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
            Assert.AreEqual(expectedBody, response.Body, "Body " + httpString);
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

            Packet packet = PacketBuilder.Build(DateTime.Now, ethernetLayer, ipV4Layer, tcpLayer, payloadLayer);

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