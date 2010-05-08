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
        /// Creates an instance of an IGMP Report Version 3 Layer with a default of no Group Records.
        /// </summary>
        public IgmpReportVersion3Layer()
        {
            GroupRecords = new IgmpGroupRecord[0].AsReadOnly();
        }
        /// <summary>
        /// Each Group Record is a block of fields containing information pertaining to the sender's membership in a single multicast group on the interface from which the Report is sent.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ReadOnlyCollection<IgmpGroupRecord> GroupRecords { get; set;}

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

        /// <summary>
        /// Xor of the hash codes of the layer length, datalink, message type, query version and the group records.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   GroupRecords.SequenceGetHashCode();
        }

        /// <summary>
        /// true iff the group records are equal.
        /// </summary>
        protected override bool EqualFields(IgmpLayer other)
        {
            return EqualFields(other as IgmpReportVersion3Layer);
        }

        /// <summary>
        /// true iff the group records are equal.
        /// </summary>
        private bool EqualFields(IgmpReportVersion3Layer other)
        {
            return other != null &&
                   GroupRecords.SequenceEqual(other.GroupRecords);
        }
    }
}