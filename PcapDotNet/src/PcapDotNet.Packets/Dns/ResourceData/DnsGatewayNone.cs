using System;

namespace PcapDotNet.Packets.Dns
{
    public class DnsGatewayNone : DnsGateway, IEquatable<DnsGatewayNone>
    {
        public override DnsGatewayType GatewayType
        {
            get { return DnsGatewayType.None; }
        }

        public override int Length
        {
            get { return 0; }
        }

        public bool Equals(DnsGatewayNone other)
        {
            return other != null;
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayNone);
        }

        internal override int DataGetHashCode()
        {
            return 0;
        }

        internal DnsGatewayNone()
        {
        }

        internal override void Write(byte[] buffer, int offset)
        {
        }
    }
}