﻿using System;
using System.Collections.Generic;

namespace PcapDotNet.Packets.Http
{
    public class HttpResponseDatagram : HttpDatagram
    {
        internal HttpResponseDatagram(byte[] buffer, int offset, int length) 
            : base(buffer, offset, length)
        {
        }

        public override bool IsRequest
        {
            get { return false; }
        }

        public uint? StatusCode
        {
            get
            {
                ParseFirstLine();
                return _statusCode;
            }
        }

        public Datagram ReasonPhrase
        {
            get
            {
                ParseFirstLine();
                return _reasonPhrase;
            }
        }

        protected override bool IsBodyPossible
        {
            get
            {
                uint statusCodeValue = StatusCode.Value;
                if (statusCodeValue >= 100 && statusCodeValue <= 199 || statusCodeValue == 204 || statusCodeValue == 205 || statusCodeValue == 304)
                    return false;
                // if (IsResponseToHeadRequest)
                //     return false;
                return true;
            }
        }

        internal override void ParseSpecificFirstLine(out HttpVersion version, out int? headerOffset)
        {
            HttpParser parser = new HttpParser(Buffer, StartOffset, Length);
            parser.Version(out version).Space().DecimalNumber(3, out _statusCode).Space().ReasonPhrase(out _reasonPhrase).CarriageReturnLineFeed();
            headerOffset = parser.Success ? (int?)(parser.Offset - StartOffset) : null;
        }

        private uint? _statusCode;
        private Datagram _reasonPhrase;
    }
}