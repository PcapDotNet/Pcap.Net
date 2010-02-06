using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    public class IgmpQueryVersion3Layer : IgmpLayer, IIgmpLayerWithGroupAddress
    {
        public IgmpQueryVersion3Layer()
        {
            SourceAddresses = new List<IpV4Address>();
        }

        public TimeSpan MaxResponseTime { get; set; }
        public IpV4Address GroupAddress { get; set; }
        public bool IsSuppressRouterSideProcessing { get; set; }

        public byte QueryRobustnessVariable{get; set ;}

        public TimeSpan QueryInterval{get; set ;}

        public IList<IpV4Address> SourceAddresses{get; set ;}

        public override int Length
        {
            get { return IgmpDatagram.GetQueryVersion3Length(SourceAddresses.Count); }
        }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteQueryVersion3(buffer, offset,
                                            MaxResponseTime, GroupAddress, IsSuppressRouterSideProcessing, QueryRobustnessVariable,
                                            QueryInterval, SourceAddresses);
        }

        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version3;
            }
        }

        public override TimeSpan MaxResponseTimeValue
        {
            get { return MaxResponseTime; }
        }

        public bool Equals(IgmpQueryVersion3Layer other)
        {
            return other != null &&
                   GroupAddress == other.GroupAddress &&
                   IsSuppressRouterSideProcessing == other.IsSuppressRouterSideProcessing &&
                   QueryRobustnessVariable == other.QueryRobustnessVariable &&
                   QueryInterval.Divide(2) <= other.QueryInterval && QueryInterval.Multiply(2) >= other.QueryInterval &&
                   SourceAddresses.SequenceEqual(other.SourceAddresses);
        }

        public override sealed bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpQueryVersion3Layer);
        }
    }
}