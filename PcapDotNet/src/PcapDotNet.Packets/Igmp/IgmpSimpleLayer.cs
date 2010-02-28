using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// Represents a Simple IGMP layer.
    /// A simple layer only has the 8 bytes header without additional fields.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public abstract class IgmpSimpleLayer : IgmpLayer, IIgmpLayerWithGroupAddress
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
        public override int Length
        {
            get { return IgmpDatagram.HeaderLength; }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// This method ignores the payload length, and the previous and next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteHeader(buffer, offset,
                                     MessageType, MaxResponseTimeValue, GroupAddress);
        }

        public bool Equals(IgmpSimpleLayer other)
        {
            return other != null &&
                   GroupAddress == other.GroupAddress;
        }

        public override sealed bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpSimpleLayer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   GroupAddress.GetHashCode();
        }
    }
}