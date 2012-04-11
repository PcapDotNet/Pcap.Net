using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Http;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomHttpExtensions
    {
        public static HttpLayer NextHttpLayer(this Random random)
        {
            if (random.NextBool())
                return random.NextHttpRequestLayer();
            return random.NextHttpResponseLayer();
        }

        public static HttpRequestLayer NextHttpRequestLayer(this Random random)
        {
            HttpRequestLayer httpRequestLayer = new HttpRequestLayer();
            if (random.NextBool(10))
            {
                httpRequestLayer.Method = random.NextHttpRequestMethod();
                httpRequestLayer.Uri = httpRequestLayer.Method == null ? null : random.NextHttpUri();
                httpRequestLayer.Version = httpRequestLayer.Uri == null || random.NextBool(10)  ? null : random.NextHttpVersion();
                httpRequestLayer.Header = httpRequestLayer.Version == null ? null : random.NextHttpHeader();
                httpRequestLayer.Body = httpRequestLayer.Header == null ? null : random.NextHttpBody(true, null, httpRequestLayer.Header);
            }
            return httpRequestLayer;
        }

        public static HttpResponseLayer NextHttpResponseLayer(this Random random)
        {
            HttpResponseLayer httpResponseLayer = new HttpResponseLayer
            {
                Version = random.NextHttpVersion(),
                StatusCode = random.NextBool(10) ? null : (uint?)random.NextUInt(100, 999),
            };
            httpResponseLayer.ReasonPhrase = httpResponseLayer.StatusCode == null ? null : random.NextHttpReasonPhrase();
            httpResponseLayer.Header = httpResponseLayer.ReasonPhrase == null ? null : random.NextHttpHeader();
            httpResponseLayer.Body = httpResponseLayer.Header == null ? null : random.NextHttpBody(false, httpResponseLayer.StatusCode, httpResponseLayer.Header);

            return httpResponseLayer;
        }

        public static Datagram NextHttpReasonPhrase(this Random random)
        {
            int reasonPhraseLength = random.Next(100);
            StringBuilder reasonPhrase = new StringBuilder(reasonPhraseLength);
            for (int i = 0; i != reasonPhraseLength; ++i)
            {
                if (random.NextBool())
                    reasonPhrase.Append(random.NextHttpTextChar());
                else if (random.NextBool())
                    reasonPhrase.Append(' ');
                else
                    reasonPhrase.Append('\t');
            }
            return new Datagram(EncodingExtensions.Iso88591.GetBytes(reasonPhrase.ToString()));
        }

        public static HttpRequestMethod NextHttpRequestMethod(this Random random)
        {
            HttpRequestKnownMethod knownMethod = random.NextEnum<HttpRequestKnownMethod>();
            if (knownMethod == HttpRequestKnownMethod.Unknown)
                return new HttpRequestMethod(random.NextHttpToken());
            return new HttpRequestMethod(knownMethod);
        }

        public static string NextHttpToken(this Random random)
        {
            char[] httpToken = ((Func<char>)(() => random.NextHttpTokenChar())).GenerateArray(random.NextInt(1, 100));
            return new string(httpToken);
        }

        public static char NextHttpTokenChar(this Random random)
        {
            char result;
            do
            {
                result = random.NextChar((char)33, (char)127);
            } while (new[] {'(', ')', '<', '>', '@', ',', ';', ':', '\\', '"', '/', '[', ']', '?', '=', '{', '}'}.Contains(result));

            return result;
        }

        public static string NextHttpFieldValue(this Random random)
        {
            int valueLength = random.NextInt(1, 100);
            StringBuilder stringBuilder = new StringBuilder();
            while (stringBuilder.Length < valueLength)
            {
                switch (random.Next(3))
                {
                    case 0:
                        stringBuilder.Append(random.NextHttpTextChar());
                        break;

                    case 1:
                        stringBuilder.Append(random.NextHttpQuotedString());
                        break;

                    case 2:
                        if (stringBuilder.Length > 0 && stringBuilder.Length < valueLength - 3)
                            stringBuilder.Append(random.NextHttpLinearWhiteSpace());
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        public static string NextHttpLinearWhiteSpace(this Random random)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (random.NextBool())
                stringBuilder.Append("\r\n");
            stringBuilder.Append(random.NextBool() ? ' ' : '\t');
            return stringBuilder.ToString();
        }

        public static char NextHttpTextChar(this Random random)
        {
            char text = random.NextChar((char)33, (char)254);
            if (text == '"')
                return (char)254;
            if (text == 127)
                return (char)255;
            return text;
        }

        public static string NextHttpUri(this Random random)
        {
            int uriLength = random.Next(100);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i != uriLength; ++i)
                stringBuilder.Append(random.NextChar((char)33, (char)255));
            return stringBuilder.ToString();
        }

        public static HttpVersion NextHttpVersion(this Random random)
        {
            return new HttpVersion(random.NextUInt(10000000), random.NextUInt(10000000));
        }

        public static HttpHeader NextHttpHeader(this Random random)
        {
            int numFields = random.Next(100);
            List<HttpField> fields = new List<HttpField>(numFields);
            HashSet<string> fieldNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i != numFields; ++i)
            {
                fields.Add(random.NextHttpField(fieldNames));
                fieldNames.Add(fields.Last().Name);
            }

            return new HttpHeader(fields);
        }

        public static HttpField NextHttpField(this Random random, HashSet<string> fieldNames)
        {
            const string unknownField = "Unknown Name";
            List<string> allOptions = new List<string> { unknownField, HttpTransferEncodingField.FieldNameUpper, HttpContentLengthField.FieldNameUpper, HttpContentTypeField.FieldNameUpper };
            List<string> possibleOptions = new List<string>(allOptions.Count);
            foreach (string option in allOptions)
            {
                if (!fieldNames.Contains(option))
                    possibleOptions.Add(option);
            }

            string chosenOption = random.NextValue(possibleOptions);
            switch (chosenOption)
            {
                case unknownField:
                    string fieldName;
                    do
                    {
                        fieldName = random.NextHttpToken();
                    } while (fieldNames.Contains(fieldName));
                    return HttpField.CreateField(fieldName, random.NextHttpFieldValue());

                case HttpTransferEncodingField.FieldNameUpper:
                    string[] transferCodings = ((Func<string>)(() => random.NextHttpTransferCoding())).GenerateArray(random.NextInt(1, 10));
                    return new HttpTransferEncodingField(transferCodings);

                case HttpContentLengthField.FieldNameUpper:
                    return new HttpContentLengthField(random.NextUInt(1000));

                case HttpContentTypeField.FieldNameUpper:

                    return new HttpContentTypeField(random.NextHttpToken(), random.NextHttpToken(), random.NextHttpFieldParameters());

                default:
                    throw new InvalidOperationException("Invalid option " + chosenOption);
            }
        }

        public static HttpFieldParameters NextHttpFieldParameters(this Random random)
        {
            int numParameters = random.Next(10);
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>(numParameters);
            for (int i = 0; i != numParameters; ++i)
            {
                string parameterName = random.NextHttpToken();
                while (parameters.Any(pair => pair.Key == parameterName))
                    parameterName = random.NextHttpToken();
                parameters.Add(new KeyValuePair<string, string>(parameterName,
                                                                random.NextBool() ? random.NextHttpToken() : random.NextHttpQuotedString()));
            }
            return new HttpFieldParameters(parameters);
        }

        public static string NextHttpTransferCoding(this Random random)
        {
            if (random.NextBool())
                return "chunked";

            StringBuilder transferCoding = new StringBuilder(random.NextHttpToken());
            int numParameters = random.Next(5);
            for (int i = 0; i != numParameters; ++i)
            {
                transferCoding.Append(';');
                transferCoding.Append(random.NextHttpToken());
                transferCoding.Append('=');
                if (random.NextBool())
                    transferCoding.Append(random.NextHttpToken());
                else
                    transferCoding.Append(random.NextHttpQuotedString());
            }

            return transferCoding.ToString();
        }

        public static string NextHttpQuotedString(this Random random)
        {
            StringBuilder quotedString = new StringBuilder();
            quotedString.Append('"');
            int numQuotedValues = random.Next(100);
            for (int i = 0; i != numQuotedValues; ++i)
            {
                char quotedValue = random.NextHttpTextChar();
                if (quotedValue != '\\')
                    quotedString.Append(quotedValue);
                else
                {
                    quotedString.Append('\\');
                    quotedString.Append(random.NextChar((char)0, (char)128));
                }
            }
            quotedString.Append('"');

            return quotedString.ToString();
        }

        public static Datagram NextHttpBody(this Random random, bool isRequest, uint? statusCode, HttpHeader httpHeader)
        {
            if (isRequest && httpHeader.ContentLength == null ||
                !isRequest && statusCode >= 100 && statusCode <= 199 || statusCode == 204 || statusCode == 205 || statusCode == 304)
                return Datagram.Empty;

            if (httpHeader.TransferEncoding != null && 
                httpHeader.TransferEncoding.TransferCodings.Any(coding => coding != "identity"))
            {
                // chunked
                List<byte> chunkedBody = new List<byte>();
                int numChunks = random.Next(10);
                for (int i = 0; i != numChunks; ++i)
                {
                    int chunkSize = random.NextInt(1, 1000);
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes(chunkSize.ToString("x")));
                    var chunkExtension = random.NextHttpFieldParameters();
                    foreach (var parameter in chunkExtension)
                    {
                        chunkedBody.Add((byte)';');
                        chunkedBody.AddRange(Encoding.ASCII.GetBytes(parameter.Key));
                        chunkedBody.Add((byte)'=');
                        chunkedBody.AddRange(EncodingExtensions.Iso88591.GetBytes(parameter.Key));
                    }
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                    chunkedBody.AddRange(random.NextDatagram(chunkSize));
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                }
                int numZeros = random.NextInt(1, 10);
                for (int i = 0; i != numZeros; ++i)
                    chunkedBody.Add((byte)'0');
                var lastChunkExtension = random.NextHttpFieldParameters();
                foreach (var parameter in lastChunkExtension)
                {
                    chunkedBody.Add((byte)';');
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes(parameter.Key));
                    chunkedBody.Add((byte)'=');
                    chunkedBody.AddRange(EncodingExtensions.Iso88591.GetBytes(parameter.Key));
                }
                chunkedBody.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                var trailer = random.NextHttpHeader();
                byte[] trailerBuffer = new byte[trailer.BytesLength];
                trailer.Write(trailerBuffer, 0);
                chunkedBody.AddRange(trailerBuffer);

                return new Datagram(chunkedBody.ToArray());
            }

            if (httpHeader.ContentLength != null)
            {
                return random.NextDatagram(random.Next((int)(httpHeader.ContentLength.ContentLength + 1)));
            }

            if (httpHeader.ContentType != null &&
                httpHeader.ContentType.MediaType == "multipart" &&
                httpHeader.ContentType.MediaSubtype == "byteranges" &&
                httpHeader.ContentType.Parameters["boundary"] != null)
            {
                List<byte> boundedBody = new List<byte>(random.NextDatagram(random.Next(1000)));
                boundedBody.AddRange(EncodingExtensions.Iso88591.GetBytes("--" + httpHeader.ContentType.Parameters["boundary"] + "--"));
                return new Datagram(boundedBody.ToArray());
            }

            return random.NextDatagram(random.Next(1000));
        }
    }
}