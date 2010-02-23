using System;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTraceRouteLayer : IcmpLayer
    {
        public IcmpCodeTraceRoute Code { get; set; }

        public ushort Identification{get;set;}
        
        public ushort OutboundHopCount{get;set;}

        public ushort ReturnHopCount{get;set;}

        public uint OutputLinkSpeed{get;set;}
        
        public uint OutputLinkMaximumTransmissionUnit{get;set;}

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.TraceRoute; }
        }

        protected override int PayloadLength
        {
            get
            {
                return IcmpTraceRouteDatagram.PayloadLength;
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
            IcmpTraceRouteDatagram.WriteHeaderAdditional(buffer, offset, OutboundHopCount, ReturnHopCount, OutputLinkSpeed, OutputLinkMaximumTransmissionUnit);
        }

        public bool Equals(IcmpTraceRouteLayer other)
        {
            return other != null &&
                   OutboundHopCount == other.OutboundHopCount &&
                   ReturnHopCount == other.ReturnHopCount &&
                   OutputLinkSpeed == other.OutputLinkSpeed &&
                   OutputLinkMaximumTransmissionUnit == other.OutputLinkMaximumTransmissionUnit;
        }

        public override sealed bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpTraceRouteLayer);
        }
    }
}