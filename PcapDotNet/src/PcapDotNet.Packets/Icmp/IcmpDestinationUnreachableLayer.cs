namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792 and RFC 1191.
    /// <seealso cref="IcmpDestinationUnreachableDatagram"/>
    /// </summary>
    public sealed class IcmpDestinationUnreachableLayer : IcmpLayer
    {
        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public IcmpCodeDestinationUnreachable Code { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DestinationUnreachable; }
        }

        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public override byte CodeValue
        {
            get { return (byte)Code; }
        }

        /// <summary>
        /// The size in octets of the largest datagram that could be forwarded, 
        /// along the path of the original datagram, without being fragmented at this router.  
        /// The size includes the IP header and IP data, and does not include any lower-level headers.
        /// </summary>
        public ushort NextHopMaximumTransmissionUnit { get; set; }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override uint Variable
        {
            get { return NextHopMaximumTransmissionUnit; }
        }
    }
}