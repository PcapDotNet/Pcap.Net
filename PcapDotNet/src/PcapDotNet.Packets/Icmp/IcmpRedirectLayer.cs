using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// Represents an ICMP Redirect message layer.
    /// <seealso cref="IcmpRedirectDatagram"/>
    /// </summary>
    public sealed class IcmpRedirectLayer : IcmpLayer
    {
        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public IcmpCodeRedirect Code { get; set; }

        /// <summary>
        /// Address of the gateway to which traffic for the network specified in the internet destination network field of the original datagram's data should be sent.
        /// </summary>
        public IpV4Address GatewayInternetAddress { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Redirect; }
        }

        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public override byte CodeValue
        {
            get { return (byte)Code; }
        }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override uint Variable
        {
            get { return GatewayInternetAddress.ToValue(); }
        }
    }
}