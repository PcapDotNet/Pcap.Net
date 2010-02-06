using System;

namespace PcapDotNet.Packets.Icmp
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
                return base.Length + IcmpTracerouteDatagram.PayloadLength;
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

        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpTracerouteDatagram.WriteHeaderAdditional(buffer, offset, OutboundHopCount, ReturnHopCount, OutputLinkSpeed, OutputLinkMtu);
        }

        public bool Equals(IcmpTracerouteLayer other)
        {
            return other != null &&
                   OutboundHopCount == other.OutboundHopCount &&
                   ReturnHopCount == other.ReturnHopCount &&
                   OutputLinkSpeed == other.OutputLinkSpeed &&
                   OutputLinkMtu == other.OutputLinkMtu;
        }

        public override sealed bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpTracerouteLayer);
        }
    }
}