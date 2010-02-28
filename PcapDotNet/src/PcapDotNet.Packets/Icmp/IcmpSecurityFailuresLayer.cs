namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 2521.
    /// Represents an ICMP Security Failures layer.
    /// <seealso cref="IcmpSecurityFailuresDatagram"/>
    /// </summary>
    public class IcmpSecurityFailuresLayer : IcmpLayer
    {
        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public IcmpCodeSecurityFailure Code { get; set; }

        /// <summary>
        /// An offset into the Original Internet Headers that locates the most significant octet of the offending SPI.  
        /// Will be zero when no SPI is present.
        /// </summary>
        public ushort Pointer { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.SecurityFailures; }
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