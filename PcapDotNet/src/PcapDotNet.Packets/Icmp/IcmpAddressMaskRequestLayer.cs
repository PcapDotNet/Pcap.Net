using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 950.
    /// Represents an ICMP Trace Route message layer.
    /// <seealso cref="IcmpAddressMaskRequestDatagram"/>
    /// </summary>
    public class IcmpAddressMaskRequestLayer : IcmpIdentifiedLayer
    {
        /// <summary>
        /// A 32-bit mask.
        /// </summary>
        public IpV4Address AddressMask { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.AddressMaskRequest; }
        }

        /// <summary>
        /// The number of bytes the ICMP payload takes.
        /// </summary>
        protected override int PayloadLength
        {
            get { return IcmpAddressMaskRequestDatagram.PayloadLength; }
        }

        /// <summary>
        /// Writes the ICMP payload to the buffer.
        /// Doesn't include payload in the next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the ICMP payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing the payload at.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpAddressMaskRequestDatagram.WriteHeaderAdditional(buffer, offset, AddressMask);
        }

        /// <summary>
        /// True iff the address mask is equal to the other address mask.
        /// </summary>
        protected override bool EqualPayload(IcmpLayer other)
        {
            return EqualPayload(other as IcmpAddressMaskRequestLayer);
        }

        /// <summary>
        /// True iff the address mask is equal to the other address mask.
        /// </summary>
        private bool EqualPayload(IcmpAddressMaskRequestLayer other)
        {
            return other != null && AddressMask.Equals(other.AddressMask);
        }
    }
}