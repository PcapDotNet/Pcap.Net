using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// Represents a Simple IGMP layer.
    /// A simple layer only has the 8 bytes header without additional fields.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public abstract class IgmpVersion1PlusSimpleLayer : IgmpVersion1PlusLayer, IIgmpLayerWithGroupAddress
    {
        /// <summary>
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </summary>
        public IpV4Address GroupAddress { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public sealed override int Length
        {
            get { return IgmpDatagram.HeaderLength; }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// This method ignores the payload length, and the previous and next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected sealed override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteVersion1PlusSimpleHeader(buffer, offset,
                                                       MessageTypeValue, MaxResponseTimeValue, GroupAddress);
        }

        /// <summary>
        /// Xor of the hash codes of the layer length, datalink, message type, query version and group address.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return base.GetHashCode() ^
                   GroupAddress.GetHashCode();
        }

        /// <summary>
        /// True iff the fields that are not mutual to all IGMP version 1+ layers are equal.
        /// </summary>
        protected override bool EqualFields(IgmpVersion1PlusLayer other)
        {
            return EqualFields(other as IgmpVersion1PlusSimpleLayer);
        }

        /// <summary>
        /// True iff the the group address is equal.
        /// </summary>
        private bool EqualFields(IgmpVersion1PlusSimpleLayer other)
        {
            return other != null &&
                   GroupAddress == other.GroupAddress;
        }
    }
}