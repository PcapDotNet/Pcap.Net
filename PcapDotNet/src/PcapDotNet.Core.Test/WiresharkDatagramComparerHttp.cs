using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.TestUtils;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerHttp : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Http"; }
        }

        protected override bool Ignore(Datagram datagram)
        {
            HttpDatagram httpDatagram = (HttpDatagram)datagram;
            return (httpDatagram.Header != null && httpDatagram.Header.ContentLength != null && httpDatagram.Header.TransferEncoding != null);
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            HttpDatagram httpDatagram = (HttpDatagram)datagram;

            if (field.Name() == "data" || field.Name() == "data.data")
            {
                if (field.Name() == "data")
                    field.AssertNoShow();

                MoreAssert.AreSequenceEqual(httpDatagram.Take(_data.Length / 2), HexEncoding.Instance.GetBytes(_data.ToString()));
                //                    string previousData = data.ToString();
                //                    for (int i = 0; i != previousData.Length / 2; ++i)
                //                    {
                //                        byte value = Convert.ToByte(previousData.Substring(i * 2, 2), 16);
                //                        Assert.AreEqual(httpDatagram[i], value);
                //                    }
                field.AssertValue(httpDatagram.Skip(_data.Length / 2));
                return false;
            }

            string fieldShow = field.Show();
            string httpFieldName;
            switch (field.Name())
            {
                case "http.request":
                    field.AssertShowDecimal(httpDatagram.IsRequest);
                    break;

                case "http.response":
                    field.AssertShowDecimal(httpDatagram.IsResponse);
                    break;

                case "":
                    if (fieldShow == "HTTP chunked response")
                    {
                        throw new InvalidOperationException("HTTP chunked response");
                    }

                    _data.Append(field.Value());
                    if (fieldShow == @"\r\n")
                        break;

                    if (_isFirstEmptyName)
                    {
                        CompareHttpFirstLine(field, httpDatagram);
                        _isFirstEmptyName = false;
                    }
                    else if (fieldShow.StartsWith("Content-encoded entity body"))
                    {
                        break;
                    }
                    else
                    {
                        fieldShow = EncodingExtensions.Iso88591.GetString(HexEncoding.Instance.GetBytes(field.Value()));
                        fieldShow = fieldShow.Substring(0, fieldShow.Length - 2);
                        int colonIndex = fieldShow.IndexOf(':');
                        MoreAssert.IsBiggerOrEqual(0, colonIndex, "Can't find colon in field with empty name");

                        if (httpDatagram.Header == null)
                        {
                            Assert.IsTrue(httpDatagram.IsRequest);
                            Assert.IsNull(httpDatagram.Version);
                            break;
                        }
                        httpFieldName = fieldShow.Substring(0, colonIndex);
                        string fieldValue = fieldShow.Substring(colonIndex + 1).SkipWhile(c => c == ' ').TakeWhile(c => c != '\\').SequenceToString();
                        string expectedFieldValue = httpDatagram.Header[httpFieldName].ValueString;
                        Assert.IsTrue(expectedFieldValue.Contains(fieldValue),
                                      string.Format("{0} <{1}> doesn't contain <{2}>", field.Name(), expectedFieldValue, fieldValue));
                    }
                    break;

                case "data.len":
                    field.AssertShowDecimal(httpDatagram.Length - _data.Length / 2);
                    break;

                case "http.host":
                case "http.user_agent":
                case "http.accept":
                case "http.accept_language":
                case "http.accept_encoding":
                case "http.connection":
                case "http.cookie":
                case "http.cache_control":
                case "http.content_encoding":
                case "http.date":
                case "http.referer":
                case "http.last_modified":
                case "http.server":
                case "http.set_cookie":
                case "http.location":
                    _data.Append(field.Value());
                    httpFieldName = field.Name().Substring(5).Replace('_', '-');
                    HttpField httpField = httpDatagram.Header[httpFieldName];
                    if (!field.Value().EndsWith("0d0a"))
                        Assert.IsNull(httpField);
                    else
                    {
                        string fieldValue = field.Show().Replace("\\\"", "\"");
                        string expectedFieldValue = httpField.ValueString;
                        Assert.IsTrue(expectedFieldValue.Contains(fieldValue),
                                      string.Format("{0} <{1}> doesn't contain <{2}>", field.Name(), expectedFieldValue, fieldValue));
                    }
                    break;

                case "http.content_length_header":
                    _data.Append(field.Value());
                    field.AssertShowDecimal(httpDatagram.Header.ContentLength.ContentLength.Value);
                    break;

                case "http.content_type":
                    _data.Append(field.Value());
                    string[] mediaType = fieldShow.Split(new[] {';', ' ', '/'}, StringSplitOptions.RemoveEmptyEntries);
                    Assert.AreEqual(httpDatagram.Header.ContentType.MediaType, mediaType[0]);
                    Assert.AreEqual(httpDatagram.Header.ContentType.MediaSubtype, mediaType[1]);
                    int fieldShowParametersStart = fieldShow.IndexOf(';');
                    if (fieldShowParametersStart == -1)
                        Assert.IsFalse(httpDatagram.Header.ContentType.Parameters.Any());
                    else
                        Assert.AreEqual(
                            httpDatagram.Header.ContentType.Parameters.Select(pair => pair.Key + '=' + pair.Value.ToWiresharkLiteral()).SequenceToString(';'),
                            fieldShow.Substring(fieldShowParametersStart + 1));
                    break;

                case "http.transfer_encoding":
                    _data.Append(field.Value());
                    Assert.AreEqual(fieldShow.ToWiresharkLowerLiteral(),
                                    httpDatagram.Header.TransferEncoding.TransferCodings.SequenceToString(',').ToWiresharkLiteral());
                    break;

                default:
                    throw new InvalidOperationException("Invalid HTTP field " + field.Name());
            }

            return true;
        }

        private static void CompareHttpFirstLine(XElement httpFirstLineElement, HttpDatagram httpDatagram)
        {
            foreach (var field in httpFirstLineElement.Fields())
            {
                switch (field.Name())
                {
                    case "http.request.method":
                        Assert.IsTrue(httpDatagram.IsRequest, field.Name() + " IsRequest");
                        field.AssertShow(((HttpRequestDatagram)httpDatagram).Method.Method);
                        break;

                    case "http.request.uri":
                        Assert.IsTrue(httpDatagram.IsRequest, field.Name() + " IsRequest");
                        field.AssertShow(((HttpRequestDatagram)httpDatagram).Uri.ToWiresharkLiteral());
                        break;

                    case "http.request.version":
                        if (httpDatagram.Version == null)
                            field.AssertShow(string.Empty);
                        else
                            field.AssertShow(httpDatagram.Version.ToString());
                        break;

                    case "http.response.code":
                        Assert.IsTrue(httpDatagram.IsResponse, field.Name() + " IsResponse");
                        field.AssertShowDecimal(((HttpResponseDatagram)httpDatagram).StatusCode.Value);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid HTTP first line field " + field.Name());
                }
            }
        }

        readonly StringBuilder _data = new StringBuilder();
        bool _isFirstEmptyName = true;
    }
}