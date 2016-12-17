using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used by DHCP clients to specify their unique
    /// identifier.DHCP servers use this value to index their database of
    /// address bindings.  This value is expected to be unique for all
    /// clients in an administrative domain.
    /// <pre>
    ///  Code   Len   Type  Client-Identifier
    /// +-----+-----+-----+-----+-----+---
    /// |  61 |  n  |  t1 |  i1 |  i2 | ...
    /// +-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpClientIdentifierOption : DhcpOption
    {
        /// <summary>
        /// create new DhcpClientIdentifierOption
        /// </summary>
        /// <param name="type">Type </param>
        /// <param name="clientIdentifier">Client-Identifier</param>
        public DhcpClientIdentifierOption(byte type, DataSegment clientIdentifier) : base(DhcpOptionCode.ClientIdentifier)
        {
            ClientIdentifierType = type;
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
            buffer.Write(ref offset, ClientIdentifierType);
            buffer.Write(ref offset, ClientIdentifier);
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)(sizeof(byte) + ClientIdentifier.Length);
            }
        }

        /// <summary>
        /// RFC 2132.
        /// Type
        /// </summary>
        public byte ClientIdentifierType
        {
            get;
            set;
        }

        /// <summary>
        /// RFC 2132.
        /// Client-Identifier
        /// </summary>
        public DataSegment ClientIdentifier
        {
            get { return _clientIdentifier; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "ClientIdentifier.Length has to be greater than 0");
                }
                if (value.Length >= byte.MaxValue - 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "ClientIdentifier.Length has to be less than 254");
                }
                _clientIdentifier = value;
            }
        }

        private DataSegment _clientIdentifier;
    }
}