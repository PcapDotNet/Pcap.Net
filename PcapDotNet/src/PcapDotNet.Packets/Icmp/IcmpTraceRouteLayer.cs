using System;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1393.
    /// Represents an ICMP Trace Route message layer.
    /// <seealso cref="IcmpTraceRouteDatagram"/>
    /// </summary>
    public class IcmpTraceRouteLayer : IcmpLayer
    {
        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public IcmpCodeTraceRoute Code { get; set; }

        /// <summary>
        /// The ID Number as copied from the IP Traceroute option of the packet which caused this Traceroute message to be sent.  
        /// This is NOT related to the ID number in the IP header.
        /// </summary>
        public ushort Identification { get; set; }

        /// <summary>
        /// The Outbound Hop Count as copied from the IP Traceroute option of the packet which caused this Traceroute message to be sent.
        /// </summary>
        public ushort OutboundHopCount { get; set; }

        /// <summary>
        /// The Return Hop Count as copied from the IP Traceroute option of the packet which caused this Traceroute message to be sent.
        /// </summary>
        public ushort ReturnHopCount { get; set; }

        /// <summary>
        /// The speed, in OCTETS per second, of the link over which the Outbound/Return Packet will be sent.  
        /// Since it will not be long before network speeds exceed 4.3Gb/s, and since some machines deal poorly with fields longer than 32 bits, octets per second was chosen over bits per second.  
        /// If this value cannot be determined, the field should be set to zero.
        /// </summary>
        public uint OutputLinkSpeed { get; set; }

        /// <summary>
        /// The MTU, in bytes, of the link over which the Outbound/Return Packet will be sent.  
        /// MTU refers to the data portion (includes IP header; excludes datalink header/trailer) of the packet.  
        /// If this value cannot be determined, the field should be set to zero.
        /// </summary>
        public uint OutputLinkMaximumTransmissionUnit { get; set; }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.TraceRoute; }
        }

        /// <summary>
        /// The number of bytes the ICMP payload takes.
        /// </summary>
        protected override int PayloadLength
        {
            get { return IcmpTraceRouteDatagram.PayloadLength; }
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
            get { return (uint)(Identification << 16); }
        }

        /// <summary>
        /// Writes the ICMP payload to the buffer.
        /// Doesn't include payload in the next layers.
        /// </summary>
        /// <param name="buffer">The buffer to write the ICMP payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing the payload at.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            IcmpTraceRouteDatagram.WriteHeaderAdditional(buffer, offset, OutboundHopCount, ReturnHopCount, OutputLinkSpeed, OutputLinkMaximumTransmissionUnit);
        }

        /// <summary>
        /// True iff the OutboundHopCount, ReturnHopCount, OutputLinkSpeed and OutputLinkMaximumTransmissionUnit fields are equal to the other layer fields.
        /// </summary>
        protected override bool EqualPayload(IcmpLayer other)
        {
            return EqualPayload(other as IcmpTraceRouteLayer);
        }

        /// <summary>
        /// True iff the OutboundHopCount, ReturnHopCount, OutputLinkSpeed and OutputLinkMaximumTransmissionUnit fields are equal to the other layer fields.
        /// </summary>
        private bool EqualPayload(IcmpTraceRouteLayer other)
        {
            return other != null &&
                   OutboundHopCount == other.OutboundHopCount &&
                   ReturnHopCount == other.ReturnHopCount &&
                   OutputLinkSpeed == other.OutputLinkSpeed &&
                   OutputLinkMaximumTransmissionUnit == other.OutputLinkMaximumTransmissionUnit;
        }
    }
}