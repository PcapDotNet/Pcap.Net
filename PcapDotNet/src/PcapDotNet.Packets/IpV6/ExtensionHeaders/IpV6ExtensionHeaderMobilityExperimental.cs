using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5096.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | MH Type     | Reserved                |
    /// +-----+-------------+-------------------------+
    /// | 32  | Checksum                              |
    /// +-----+-------------+-------------------------+
    /// | 48  | Message Data                          |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityExperimental : IpV6ExtensionHeaderMobility
    {
        public IpV6ExtensionHeaderMobilityExperimental(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
            : base(nextHeader, checksum, IpV6MobilityOptions.None, null)
        {
            if (messageData.Length % 8 != 2)
                throw new ArgumentException("Message data size must be an integral product of 8 bytes plus 2 bytes", "messageData");
            MessageData = messageData;
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.Experimental; }
        }

        /// <summary>
        /// Carries the data specific to the experimental protocol extension.
        /// </summary>
        public DataSegment MessageData { get; private set; }

        internal override int MessageDataLength
        {
            get { return MessageData.Length; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityExperimental);
        }

        internal override int GetMessageDataHashCode()
        {
            return MessageData.GetHashCode();
        }

        internal static IpV6ExtensionHeaderMobilityExperimental ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            return new IpV6ExtensionHeaderMobilityExperimental(nextHeader, checksum, messageData);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            MessageData.Write(buffer, offset);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityExperimental other)
        {
            return other != null &&
                   MessageData.Equals(other.MessageData);
        }
    }
}