using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code   Len   Type  Client-Identifier
    /// +-----+-----+-----+-----+-----+---
    /// |  61 |  n  |  t1 |  i1 |  i2 | ...
    /// +-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpClientIdentifierOption : DhcpOption
    {
        public DhcpClientIdentifierOption(byte type, DataSegment clientIdentifier) : base(DhcpOptionCode.ClientIdentifier)
        {
            Type = type;
            ClientIdentifier = clientIdentifier;
        }

        internal static DhcpClientIdentifierOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            DhcpClientIdentifierOption option = new DhcpClientIdentifierOption(data[offset], data.Subsegment(offset + 1, length - 1));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Type);
            buffer.Write(ref offset, ClientIdentifier);
        }

        public override byte Length
        {
            get
            {
                return (byte)(sizeof(byte) + ClientIdentifier.Length);
            }
        }

        public byte Type
        {
            get;
            set;
        }

        public DataSegment ClientIdentifier
        {
            get { return _clientIdentifier; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(ClientIdentifier));
                }
                if (value.Length < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(ClientIdentifier), value.Length, "ClientIdentifier.Length has to be greater than 0");
                }
                if (value.Length > byte.MaxValue - 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(ClientIdentifier), value.Length, "ClientIdentifier.Length has to be less than 255");
                }
                _clientIdentifier = value;
            }
        }

        private DataSegment _clientIdentifier;
    }
}