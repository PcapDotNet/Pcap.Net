using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5142.
    /// <pre>
    /// +-----+----------------+-------------------------+
    /// | Bit | 0-7            | 8-15                    |
    /// +-----+----------------+-------------------------+
    /// | 0   | Next Header    | Header Extension Length |
    /// +-----+----------------+-------------------------+
    /// | 16  | MH Type        | Reserved                |
    /// +-----+----------------+-------------------------+
    /// | 32  | Checksum                                 |
    /// +-----+----------------+-------------------------+
    /// | 48  | # of Addresses | Reserved                |
    /// +-----+----------------+-------------------------+
    /// | 64  | Home Agent Addresses                     |
    /// | ... |                                          |
    /// +-----+------------------------------------------+
    /// |     | Mobility Options                         |
    /// | ... |                                          |
    /// +-----+------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage : IpV6ExtensionHeaderMobility
    {
        private static class MessageDataOffset
        {
            public const int NumberOfAddresses = 0;
            public const int HomeAgentAddresses = NumberOfAddresses + sizeof(byte) + sizeof(byte);
        }

        public const int MinimumMessageDataLength = MessageDataOffset.HomeAgentAddresses;

        public IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(IpV4Protocol nextHeader, ushort checksum, ReadOnlyCollection<IpV6Address> homeAgentAddresses,
                                                                 IpV6MobilityOptions options)
            : base(nextHeader, checksum, options, MessageDataOffset.HomeAgentAddresses + homeAgentAddresses.Count * IpV6Address.SizeOf)
        {
            HomeAgentAddresses = homeAgentAddresses;
        }

        public IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(IpV4Protocol nextHeader, ushort checksum, IList<IpV6Address> homeAgentAddresses, IpV6MobilityOptions options)
            : this(nextHeader, checksum, (ReadOnlyCollection<IpV6Address>)homeAgentAddresses.AsReadOnly(), options)
        {
        }

        /// <summary>
        /// Identifies the particular mobility message in question.
        /// An unrecognized MH Type field causes an error indication to be sent.
        /// </summary>
        public override IpV6MobilityHeaderType MobilityHeaderType
        {
            get { return IpV6MobilityHeaderType.HomeAgentSwitchMessage; }
        }

        /// <summary>
        /// A list of alternate home agent addresses for the mobile node.
        /// </summary>
        public ReadOnlyCollection<IpV6Address> HomeAgentAddresses { get; private set; }

        internal override int MessageDataLength
        {
            get { return MinimumMessageDataLength + HomeAgentAddresses.Count * IpV6Address.SizeOf + MobilityOptions.BytesLength; }
        }

        internal override bool EqualsMessageData(IpV6ExtensionHeaderMobility other)
        {
            return EqualsMessageData(other as IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage);
        }

        internal static IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage ParseMessageData(IpV4Protocol nextHeader, ushort checksum, DataSegment messageData)
        {
            if (messageData.Length < MinimumMessageDataLength)
                return null;

            byte numberOfAddresses = messageData[MessageDataOffset.NumberOfAddresses];
            int homeAgentAddressesSize = numberOfAddresses * IpV6Address.SizeOf;
            if (messageData.Length < MinimumMessageDataLength + homeAgentAddressesSize)
                return null;

            IpV6Address[] homeAgentAddresses = new IpV6Address[numberOfAddresses];
            for (int i = 0; i != numberOfAddresses; ++i)
                homeAgentAddresses[i] = messageData.ReadIpV6Address(MessageDataOffset.HomeAgentAddresses + i * IpV6Address.SizeOf, Endianity.Big);

            int optionsOffset = MessageDataOffset.HomeAgentAddresses + homeAgentAddressesSize;
            IpV6MobilityOptions options = new IpV6MobilityOptions(messageData.Subsegment(optionsOffset, messageData.Length - optionsOffset));
            return new IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(nextHeader, checksum, homeAgentAddresses, options);
        }

        internal override void WriteMessageData(byte[] buffer, int offset)
        {
            buffer.Write(offset + MessageDataOffset.NumberOfAddresses, (byte)HomeAgentAddresses.Count);
            for (int i = 0; i != HomeAgentAddresses.Count; ++i)
                buffer.Write(offset + MessageDataOffset.HomeAgentAddresses + i * IpV6Address.SizeOf, HomeAgentAddresses[i], Endianity.Big);
            MobilityOptions.Write(buffer, offset + MessageDataOffset.HomeAgentAddresses + HomeAgentAddresses.Count * IpV6Address.SizeOf);
        }

        private bool EqualsMessageData(IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage other)
        {
            return other != null &&
                   HomeAgentAddresses.SequenceEqual(other.HomeAgentAddresses);
        }
    }
}