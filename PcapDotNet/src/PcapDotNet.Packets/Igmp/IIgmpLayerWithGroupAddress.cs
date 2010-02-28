using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// Represents an IGMP layer that contains a Group Address.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public interface IIgmpLayerWithGroupAddress
    {
        /// <summary>
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </summary>
        IpV4Address GroupAddress { get; set; }
    }
}