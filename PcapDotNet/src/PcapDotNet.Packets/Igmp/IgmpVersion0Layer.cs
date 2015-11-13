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

        public abstract uint IdentifierValue { get; }

        public abstract ulong AccessKeyValue { get; }

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

        protected override sealed bool EqualsVersionSpecific(IgmpLayer other)
        {
            return EqualsVersionSpecific(other as IgmpVersion0Layer);
        }

        protected abstract byte CodeValue { get; }
        protected abstract IpV4Address GroupAddressValue { get; }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteVersion0Header(buffer, offset, MessageType, CodeValue, IdentifierValue, GroupAddressValue, AccessKeyValue);
        }

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