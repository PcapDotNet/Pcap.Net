namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1475.
    /// Represents a Conversion Failed ICMP layer.
    /// <seealso cref="IcmpConversionFailedDatagram"/>
    /// </summary>
    public sealed class IcmpConversionFailedLayer : IcmpLayer
    {
        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public IcmpCodeConversionFailed Code { get; set; }

        /// <summary>
        /// An offset from the start of the original datagram to the beginning of the offending field.
        /// </summary>
        public uint Pointer { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ConversionFailed; }
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
            get { return Pointer; }
        }
    }
}