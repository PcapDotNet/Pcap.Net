using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Igmp
{
    public class IgmpReportVersion3Layer : IgmpLayer
    {
        public IgmpReportVersion3Layer()
            :this(new List<IgmpGroupRecord>())
        {
        }

        public IgmpReportVersion3Layer(IList<IgmpGroupRecord> groupRecords)
        {
            _groupRecords = groupRecords;
        }

        public IList<IgmpGroupRecord> GroupRecords
        {
            get { return _groupRecords; }
        }

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
 
        private readonly IList<IgmpGroupRecord> _groupRecords;
    }
}