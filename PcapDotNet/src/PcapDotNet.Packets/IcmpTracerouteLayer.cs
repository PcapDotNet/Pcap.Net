using System;
using PcapDotNet.Packets.Icmp;

namespace PcapDotNet.Packets
{
    public class IcmpTracerouteLayer : IcmpLayer
    {
        public IcmpCodeTraceroute Code { get; set; }

        public ushort Identification{get;set;}
        
        public ushort OutboundHopCount{get;set;}

        public ushort ReturnHopCount{get;set;}

        public uint OutputLinkSpeed{get;set;}
        
        public uint OutputLinkMtu{get;set;}

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Traceroute; }
        }

        public override int Length
        {
            get
            {
                return base.Length + IcmpTracerouteDatagram.HeaderAdditionalLength;
            }
        }

        public override byte CodeValue
        {
            get
            {
                return (byte)Code;
            }
        }

        protected override uint Value
        {
            get
            {
                return (uint)(Identification << 16);
            }
        }

        protected override void WriteHeaderAdditional(byte[] buffer, int offset)
        {
            IcmpTracerouteDatagram.WriteHeaderAdditional(buffer, offset, OutboundHopCount, ReturnHopCount, OutputLinkSpeed, OutputLinkMtu);
        }
    }
}