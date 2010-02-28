using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 3376.
    /// Represents an IGMP Report version 3 layer.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public class IgmpReportVersion3Layer : IgmpLayer
    {
        /// <summary>
        /// Creates an instance with no group records.
        /// </summary>
        public IgmpReportVersion3Layer()
            :this(new List<IgmpGroupRecord>())
        {
        }

        /// <summary>
        /// Creates an instance with the given group records.
        /// </summary>
        /// <param name="groupRecords">
        /// Each Group Record is a block of fields containing information pertaining to the sender's membership in a single multicast group on the interface from which the Report is sent.
        /// </param>
        public IgmpReportVersion3Layer(IList<IgmpGroupRecord> groupRecords)
        {
            _groupRecords = groupRecords;
        }

        /// <summary>
        /// Each Group Record is a block of fields containing information pertaining to the sender's membership in a single multicast group on the interface from which the Report is sent.
        /// </summary>
        public IList<IgmpGroupRecord> GroupRecords
        {
            get { return _groupRecords; }
        }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get { return IgmpDatagram.GetReportVersion3Length(GroupRecords); }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// This method ignores the payload length, and the previous and next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteReportVersion3(buffer, offset, GroupRecords);
        }

        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion3; }
        }

        /// <summary>
        /// The actual time allowed, called the Max Resp Time.
        /// </summary>
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