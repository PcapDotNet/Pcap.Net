using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpRouterAdvertisementLayer : IcmpLayer
    {
        public IcmpRouterAdvertisementLayer(IList<IcmpRouterAdvertisementEntry> entries)
        {
            _entries = entries;
        }

        public TimeSpan Lifetime { get; set; }
        public IList<IcmpRouterAdvertisementEntry> Entries { get { return _entries;} }
        
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterAdvertisement; }
        }

        protected override uint Value
        {
            get
            {
                return (uint)(((byte)Entries.Count << 24) |
                              (IcmpRouterAdvertisementDatagram.DefaultAddressEntrySize << 16) |
                              ((ushort)Lifetime.TotalSeconds));
            }
        }

        protected override int PayloadLength
        {
            get
            {
                return IcmpRouterAdvertisementDatagram.GetPayloadLength(Entries.Count);
            }
        }

        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpRouterAdvertisementDatagram.WriteHeaderAdditional(buffer, offset, Entries);
        }

        public bool Equals(IcmpRouterAdvertisementLayer other)
        {
            return other != null &&
                   Entries.SequenceEqual(other.Entries);
        }

        public sealed override bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpRouterAdvertisementLayer);
        }

        private readonly IList<IcmpRouterAdvertisementEntry> _entries;
    }
}