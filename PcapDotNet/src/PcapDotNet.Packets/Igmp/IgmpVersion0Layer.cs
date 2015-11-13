using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// </summary>
    public abstract class IgmpVersion0Layer : IgmpLayer
    {
        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override sealed int Length
        {
            get { return IgmpDatagram.Version0HeaderLength; }
        }

        /// <summary>
        /// In a Confirm Group Request message, the identifier field contains zero.
        /// In all other Request messages, the identifier field contains a value to distinguish the request from other requests by the same host.
        /// In a Reply message, the identifier field contains the same value as in the corresponding Request message.
        /// </summary>
        public abstract uint IdentifierValue { get; }

        /// <summary>
        /// In a Create Group Request message, the access key field contains zero.
        /// In all other Request messages, the access key field contains the access key assigned to the host group identified in the Group Address field
        /// (zero for public groups).
        /// In a Create Group Reply message, the access key field contains either a non-zero 64-bit number (if the request for a private group is granted)
        /// or zero.
        /// In all other Reply messages, the access key field contains the same access key as in the corresponding Request.
        /// </summary>
        public abstract ulong AccessKeyValue { get; }

        /// <summary>
        /// Xor of the hash codes of the layer length, datalink, message type, query version, code, identifier, group address and access key.
        /// </summary>
        public override int GetHashCode()
        {
            return new[]
                   {
                       base.GetHashCode(),
                       CodeValue.GetHashCode(),
                       IdentifierValue.GetHashCode(),
                       GroupAddressValue.GetHashCode(),
                       AccessKeyValue.GetHashCode()
                   }.Xor();
        }

        /// <summary>
        /// true iff the fields that are not mutual to all IGMP layers are equal.
        /// </summary>
        protected override sealed bool EqualsVersionSpecific(IgmpLayer other)
        {
            return EqualsVersionSpecific(other as IgmpVersion0Layer);
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// This method ignores the payload length, and the previous and next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteVersion0Header(buffer, offset, MessageTypeValue, CodeValue, IdentifierValue, GroupAddressValue, AccessKeyValue);
        }

        internal abstract byte CodeValue { get; }

        internal abstract IpV4Address GroupAddressValue { get; }

        private bool EqualsVersionSpecific(IgmpVersion0Layer other)
        {
            return other != null &&
                   CodeValue == other.CodeValue &&
                   IdentifierValue == other.IdentifierValue &&
                   GroupAddressValue.Equals(other.GroupAddressValue) &&
                   AccessKeyValue == other.AccessKeyValue;
        }
    }
}