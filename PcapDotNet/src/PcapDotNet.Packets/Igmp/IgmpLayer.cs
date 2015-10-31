using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// The base of all IGMP layers.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public abstract class IgmpLayer : SimpleLayer, IIpNextLayer
    {
        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public abstract IgmpMessageType MessageType { get; }

        /// <summary>
        /// The IGMP version of a Membership Query message.
        /// If the type is not a query, None will be returned.
        /// </summary>
        public virtual IgmpQueryVersion QueryVersion
        {
            get { return IgmpQueryVersion.None; }
        }


        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetGroupManagementProtocol; }
        }

        /// <summary>
        /// IGMP layers are equal if they have the same message type, query version, similar max response time and the same specific type fields.
        /// </summary>
        public bool Equals(IgmpLayer other)
        {
            return other != null &&
                   MessageType == other.MessageType &&
                   QueryVersion == other.QueryVersion &&
                   EqualsVersionSpecific(other);
        }

        /// <summary>
        /// IGMP layers are equal if they have the same message type, query version, similar max response time and the same specific type fields.
        /// </summary>
        public sealed override bool Equals(Layer other)
        {
            return Equals(other as IgmpLayer);
        }

        /// <summary>
        /// Xor of the hash codes of the layer length, datalink, message type and query version.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Sequence.GetHashCode(MessageType, QueryVersion);
        }

        /// <summary>
        /// true iff the fields that are not mutual to all IGMP layers are equal.
        /// </summary>
        protected abstract bool EqualsVersionSpecific(IgmpLayer other);
    }
}