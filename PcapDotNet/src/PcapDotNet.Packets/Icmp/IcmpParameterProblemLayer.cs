namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public sealed class IcmpParameterProblemLayer : IcmpLayer
    {
        /// <summary>
        /// The pointer identifies the octet of the original datagram's header where the error was detected (it may be in the middle of an option).  
        /// For example, 1 indicates something is wrong with the Type of Service, and (if there are options present) 20 indicates something is wrong with the type code of the first option.
        /// </summary>
        public byte Pointer { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ParameterProblem; }
        }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override uint Variable
        {
            get { return (uint)(Pointer << 24); }
        }
    }
}