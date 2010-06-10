using System.Collections.Generic;

namespace PcapDotNet.Packets.Http
{
    public class HttpHeader
    {
        internal HttpHeader(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> fields)
        {
//            int totalLength = offset + length;
//            Datagram data = new Datagram(buffer, offset, totalLength - offset);

            // Parse field-name = token
//            string fieldName;
//            if (!TryParseToken(buffer, offset, totalLength - offset, out fieldName))
//                return;
//            offset += fieldName.Length;
//            data = new Datagram(buffer, offset, totalLength - offset);

            // Parse ":"
//            if (data.FirstOrDefault() != AsciiBytes.Colon)
//                return;
//
//            ++offset;
            
            // Parse field-value
//            if (!TryParseFieldValue(buffer, offset, totalLength - offset))
//                return;

//            data = new Datagram(buffer, offset, totalLength - offset);

            // Parse field-value = *( field-content | LWS )
//            IEnumerable<byte> fieldValue = new byte[0];
//
//            int lwsCount = data.CountLinearWhiteSpaces();
//            if (lwsCount != 0)
//            {
//                offset += lwsCount;
//                data = new Datagram(buffer, offset, totalLength - offset);
//            }
//
//            int fieldContentCount;
//            IEnumerable<byte> fieldContent = data.TakeFieldContent(out fieldContentCount);
//
//            if (fieldContentCount != 0)
//            {
//                if (fieldValue.Any())
//                    fieldValue = fieldValue.Concat(AsciiBytes.Space);
//                fieldValue = fieldValue.Concat(fieldContent);
//                offset += fieldContentCount;
//            }
//            data.SkipWhile
//            buffer..Take(count).TakeWhile(value => value.IsToken()).Count))
//            Encoding.ASCII.GetString()
        }

//        private HttpHeaderPart _generalHeader;
//        private HttpHeaderPart _requestHeader;
//        private HttpHeaderPart _responseHeader;
//        private HttpHeaderPart _entityHeader;
//        private Dictionary<string, >
    }
}