using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code   Len  Type
    /// +-----+-----+-----+
    /// |  53 |  1  | 1-7 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMessageTypeOption : DhcpOption
    {
        public DhcpMessageTypeOption(MessageType type) : base(DhcpOptionCode.MessageType)
        {
            Type = type;
        }

        internal static DhcpMessageTypeOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP MessageTypeOption has to be 1");
            DhcpMessageTypeOption option = new DhcpMessageTypeOption((MessageType)data[offset]);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, (byte)Type);
        }

        public override byte Length
        {
            get
            {
                return 1;
            }
        }

        public MessageType Type
        {
            get { return _type; }
            set
            {
                if (!Enum.IsDefined(typeof(MessageType), value))
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "Not a valid MessageType");
                _type = value;
            }
        }

        private MessageType _type;

        public enum MessageType : byte
        {
            Discover = 1,
            Offer = 2,
            Request = 3,
            Decline = 4,
            Ack = 5,
            Nak = 6,
            Release = 7
        }
    }
}