using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpRouterAdvertisementLayer : IcmpLayer
    {
        public TimeSpan Lifetime { get; set; }
        public List<IcmpRouterAdvertisementEntry> Entries { get; set; }
        
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

        public override int Length
        {
            get
            {
                return base.Length + IcmpRouterAdvertisementDatagram.GetHeaderAdditionalLength(Entries.Count);
            }
        }

        protected override void WriteHeaderAdditional(byte[] buffer, int offset)
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
    }
}