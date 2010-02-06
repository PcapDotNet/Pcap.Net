using System;

namespace PcapDotNet.Packets.Igmp
{
    public abstract class IgmpVersion1Layer : IgmpSimpleLayer
    {
        public override TimeSpan MaxResponseTimeValue
        {
            get { return TimeSpan.Zero; }
        }
    }
}