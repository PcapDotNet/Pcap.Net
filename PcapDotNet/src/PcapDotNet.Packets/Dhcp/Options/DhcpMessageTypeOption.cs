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
    /// This option is used to convey the type of the DHCP message.
    /// <pre>
    ///  Code   Len  Type
    /// +-----+-----+-----+
    /// |  53 |  1  | 1-7 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMessageTypeOption : DhcpOption
    {
        /// <summary>
        /// create new DhcpMessageTypeOption.
        /// </summary>
        /// <param name="type">Type</param>
        public DhcpMessageTypeOption(MessageType type) : base(DhcpOptionCode.MessageType)
        {
            Type = type;
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.MessageType)]
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

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public override byte Length
        {
            get
            {
                return sizeof(MessageType);
            }
        }

        /// <summary>
        /// RFC 2132.
        /// Message Type
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public MessageType Type
        {
            get { return _type; }
            set
            {
                if (!Enum.IsDefined(typeof(MessageType), value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Not a valid MessageType");
                _type = value;
            }
        }

        private MessageType _type;

        /// <summary>
        /// RFC 2132.
        /// Supported Message-Types.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
        public enum MessageType : byte
        {
            /// <summary>
            /// RFC 2132.
            /// DHCP Discover message
            /// </summary>
            Discover = 1,

            /// <summary>
            /// RFC 2132.
            /// DHCP Offer message
            /// </summary>
            Offer = 2,

            /// <summary>
            /// RFC 2132.
            /// DHCP Request message
            /// </summary>
            Request = 3,

            /// <summary>
            /// RFC 2132.
            /// DHCP Decline message
            /// </summary>
            Decline = 4,

            /// <summary>
            /// RFC 2132.
            /// DHCP Acknowledgment message
            /// </summary>
            Ack = 5,

            /// <summary>
            /// RFC 2132.
            /// DHCP Negative Acknowledgment message
            /// </summary>
            Nak = 6,

            /// <summary>
            /// RFC 2132.
            /// DHCP Release message
            /// </summary>
            Release = 7
        }
    }
}