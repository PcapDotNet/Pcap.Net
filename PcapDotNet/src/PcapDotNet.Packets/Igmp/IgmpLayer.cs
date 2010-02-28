using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// The base of all IGMP layers.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public abstract class IgmpLayer : SimpleLayer, IIpV4NextLayer
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
        /// The actual time allowed, called the Max Resp Time.
        /// </summary>
        public abstract TimeSpan MaxResponseTimeValue { get; }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetGroupManagementProtocol; }
        }

        public virtual bool Equals(IgmpLayer other)
        {
            return other != null &&
                   MessageType == other.MessageType &&
                   QueryVersion == other.QueryVersion &&
                   MaxResponseTimeValue.Divide(2) <= other.MaxResponseTimeValue && MaxResponseTimeValue.Multiply(2) >= other.MaxResponseTimeValue;
        }

        public sealed override bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as IgmpLayer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   MessageType.GetHashCode() ^ QueryVersion.GetHashCode();
        }
    }
}