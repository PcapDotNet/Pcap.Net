using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Igmp
{
    public class IgmpReportVersion3Layer : IgmpLayer
    {
        public IList<IgmpGroupRecord> GroupRecords{get; set ;}

        public override int Length
        {
            get { return IgmpDatagram.GetReportVersion3Length(GroupRecords); }
        }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteReportVersion3(buffer, offset, GroupRecords);
        }

        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion3; }
        }

        public override TimeSpan MaxResponseTimeValue
        {
            get { return TimeSpan.Zero; }
        }

        public bool Equals(IgmpReportVersion3Layer other)
        {
            return other != null &&
                   GroupRecords.SequenceEqual(other.GroupRecords);
        }

        public sealed override bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpReportVersion3Layer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   GroupRecords.SequenceGetHashCode();
        }
    }
}