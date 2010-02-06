using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpRedirectLayer : IcmpLayer
    {
        public IcmpCodeRedirect Code { get; set; }
        public IpV4Address GatewayInternetAddress{get;set;}
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Redirect; }
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
                return GatewayInternetAddress.ToValue();
            }
        }
    }
}