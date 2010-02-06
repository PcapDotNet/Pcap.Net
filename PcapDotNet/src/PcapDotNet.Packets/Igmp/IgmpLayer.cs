using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    public abstract class IgmpLayer : SimpleLayer, IIpV4NextLayer
    {
        public abstract IgmpMessageType MessageType { get; }
        public virtual IgmpQueryVersion QueryVersion
        {
            get { return IgmpQueryVersion.None; }
        }
        public abstract TimeSpan MaxResponseTimeValue { get; }

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
    }
}