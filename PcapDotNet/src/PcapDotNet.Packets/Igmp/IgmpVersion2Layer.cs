using System;

namespace PcapDotNet.Packets.Igmp
{
    public abstract class IgmpVersion2Layer : IgmpSimpleLayer
    {
        public TimeSpan MaxResponseTime { get; set; }
        public override TimeSpan MaxResponseTimeValue
        {
            get { return MaxResponseTime; }
        }
    }
}