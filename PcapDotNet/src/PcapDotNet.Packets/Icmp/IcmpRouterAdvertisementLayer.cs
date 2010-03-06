using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// An ICMP Router Advertisement layer.
    /// <seealso cref="IcmpRouterAdvertisementDatagram"/>
    /// </summary>
    public class IcmpRouterAdvertisementLayer : IcmpLayer
    {
        /// <summary>
        /// Creates an instance with the given router advertisement entries.
        /// </summary>
        /// <param name="entries">
        /// The pairs of sending router's IP address(es) on the interface from which this message is sent
        /// and the preferability of each Router Address[i] as a default router address, relative to other router addresses on the same subnet.
        /// A signed, twos-complement value; higher values mean more preferable.
        /// </param>
        public IcmpRouterAdvertisementLayer(IList<IcmpRouterAdvertisementEntry> entries)
        {
            _entries = entries;
        }

        /// <summary>
        /// The maximum time that the router addresses may be considered valid.
        /// </summary>
        public TimeSpan Lifetime { get; set; }

        /// <summary>
        /// The pairs of sending router's IP address(es) on the interface from which this message is sent
        /// and the preferability of each Router Address[i] as a default router address, relative to other router addresses on the same subnet.
        /// A signed, twos-complement value; higher values mean more preferable.
        /// </summary>
        public IList<IcmpRouterAdvertisementEntry> Entries { get { return _entries; } }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterAdvertisement; }
        }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override uint Variable
        {
            get
            {
                return (uint)(((byte)Entries.Count << 24) |
                              (IcmpRouterAdvertisementDatagram.DefaultAddressEntrySize << 16) |
                              ((ushort)Lifetime.TotalSeconds));
            }
        }

        /// <summary>
        /// The number of bytes the ICMP payload takes.
        /// </summary>
        protected override int PayloadLength
        {
            get { return IcmpRouterAdvertisementDatagram.GetPayloadLength(Entries.Count); }
        }

        /// <summary>
        /// Writes the ICMP payload to the buffer.
        /// Doesn't include payload in the next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the ICMP payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing the payload at.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpRouterAdvertisementDatagram.WriteHeaderAdditional(buffer, offset, Entries);
        }

        /// <summary>
        /// True iff the Entries are equal to the other ICMP entries.
        /// </summary>
        protected override bool EqualPayload(IcmpLayer other)
        {
            return EqualPayload(other as IcmpRouterAdvertisementLayer);
        }

        /// <summary>
        /// True iff the Entries are equal to the other ICMP entries.
        /// </summary>
        private bool EqualPayload(IcmpRouterAdvertisementLayer other)
        {
            return other != null &&
                   Entries.SequenceEqual(other.Entries);
        }

        private readonly IList<IcmpRouterAdvertisementEntry> _entries;
    }
}