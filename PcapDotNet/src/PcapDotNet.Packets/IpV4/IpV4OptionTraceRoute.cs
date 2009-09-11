using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The presence of this option in an ICMP Echo (or any other) packet, hereinafter referred to as the Outbound Packet, 
    /// will cause a router to send the newly defined ICMP Traceroute message to the originator of the Outbound Packet.  
    /// In this way, the path of the Outbound Packet will be logged by the originator with only n+1 (instead of 2n) packets.  
    /// This algorithm does not suffer from a changing path and allows the response to the Outbound Packet, 
    /// hereinafter refered to as the Return Packet, to be traced 
    /// (provided the Outbound Packet's destination preserves the IP Traceroute option in the Return Packet).
    /// 
    /// <para>
    /// IP Traceroute option format
    /// <pre>
    ///  0               8              16              24
    /// +-+---+---------+---------------+-------------------------------+
    /// |F| C |  Number |    Length     |          ID Number            |
    /// +-+---+-------------------------+-------------------------------+
    /// |      Outbound Hop Count       |       Return Hop Count        |
    /// +-------------------------------+-------------------------------+
    /// |                     Originator IP Address                     |
    /// +---------------------------------------------------------------+
    /// </pre>
    /// </para>
    /// 
    /// <para>
    /// F (copy to fragments): 0 (do not copy to fragments)
    /// C (class): 2 (Debugging and Measurement)
    /// Number: 18 (F+C+Number = 82)
    /// </para>
    /// </summary>
    [OptionTypeRegistration(typeof(IpV4OptionType), IpV4OptionType.TraceRoute)]
    public class IpV4OptionTraceRoute : IpV4OptionComplex, IOptionComplexFactory, IEquatable<IpV4OptionTraceRoute>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 12;

        /// <summary>
        /// The number of bytes this option's value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Create the trace route option from the trace route option values.
        /// </summary>
        /// <param name="identification">
        /// An arbitrary number used by the originator of the Outbound Packet to identify the ICMP Traceroute messages.  
        /// It is NOT related to the ID number in the IP header.
        /// </param>
        /// <param name="outboundHopCount">
        /// Outbound Hop Count (OHC)
        /// The number of routers through which the Outbound Packet has passed.  
        /// This field is not incremented by the Outbound Packet's destination.
        /// </param>
        /// <param name="returnHopCount"></param>
        /// Return Hop Count (RHC)
        /// The number of routers through which the Return Packet has passed.
        /// This field is not incremented by the Return Packet's destination.       
        /// <param name="originatorIpAddress">
        /// The IP address of the originator of the Outbound Packet.  
        /// This isneeded so the routers know where to send the ICMP Traceroute message for Return Packets.  
        /// It is also needed for Outbound Packets which have a Source Route option.
        /// </param>
        public IpV4OptionTraceRoute(ushort identification, ushort outboundHopCount, ushort returnHopCount, IpV4Address originatorIpAddress)
            : base(IpV4OptionType.TraceRoute)
        {
            _identification = identification;
            _outboundHopCount = outboundHopCount;
            _returnHopCount = returnHopCount;
            _originatorIpAddress = originatorIpAddress;
        }

        /// <summary>
        /// Creates empty trace route option.
        /// </summary>
        public IpV4OptionTraceRoute()
            : this(0, 0, 0, IpV4Address.Zero)
        {
        }

        /// <summary>
        /// An arbitrary number used by the originator of the Outbound Packet to identify the ICMP Traceroute messages.  
        /// It is NOT related to the ID number in the IP header.
        /// </summary>
        public ushort Identification
        {
            get { return _identification; }
        }

    
        /// <summary>
        /// The IP address of the originator of the Outbound Packet.  
        /// This isneeded so the routers know where to send the ICMP Traceroute message for Return Packets.  
        /// It is also needed for Outbound Packets which have a Source Route option.
        /// </summary>
        public IpV4Address OriginatorIpAddress
        {
            get { return _originatorIpAddress; }
        }

        /// <summary>
        /// Outbound Hop Count (OHC)
        /// The number of routers through which the Outbound Packet has passed.  
        /// This field is not incremented by the Outbound Packet's destination.
        /// </summary>
        public ushort OutboundHopCount
        {
            get { return _outboundHopCount; }
        }

        /// <summary>
        /// Return Hop Count (RHC)
        /// The number of routers through which the Return Packet has passed.
        /// This field is not incremented by the Return Packet's destination.       
        /// /// </summary>
        public ushort ReturnHopCount
        {
            get { return _returnHopCount; }
        }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two trace route options are equal iff they have the exact same field values.
        /// </summary>
        public bool Equals(IpV4OptionTraceRoute other)
        {
            if (other == null)
                return false;

            return Identification == other.Identification &&
                   OutboundHopCount == other.OutboundHopCount &&
                   ReturnHopCount == other.ReturnHopCount &&
                   OriginatorIpAddress == other.OriginatorIpAddress;
        }

        /// <summary>
        /// Two trace route options are equal iff they have the exact same field values.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionTraceRoute);
        }

        /// <summary>
        /// The hash code is the xor of the base class hash code with the following values hash code:
        /// The identification, the combination of the outbound and return hop count, the originator address.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Identification.GetHashCode() ^
                   ((OutboundHopCount << 16) | (ReturnHopCount << 16)).GetHashCode() ^
                   OriginatorIpAddress.GetHashCode();

        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        public Option CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionValueLength)
                return null;

            ushort identification = buffer.ReadUShort(ref offset, Endianity.Big);
            ushort outboundHopCount = buffer.ReadUShort(ref offset, Endianity.Big);
            ushort returnHopCount = buffer.ReadUShort(ref offset, Endianity.Big);
            IpV4Address originatorIpAddress = buffer.ReadIpV4Address(ref offset, Endianity.Big);

            return new IpV4OptionTraceRoute(identification, outboundHopCount, returnHopCount, originatorIpAddress);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);

            buffer.Write(ref offset, Identification, Endianity.Big);
            buffer.Write(ref offset, OutboundHopCount, Endianity.Big);
            buffer.Write(ref offset, ReturnHopCount, Endianity.Big);
            buffer.Write(ref offset, OriginatorIpAddress, Endianity.Big);
        }

        private readonly ushort _identification;
        private readonly ushort _outboundHopCount;
        private readonly ushort _returnHopCount;
        private readonly IpV4Address _originatorIpAddress;
    }
}