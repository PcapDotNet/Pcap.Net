using System;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1393.
    /// <pre>
    /// +-----+------+-------------+------------------+
    /// | Bit | 0-7  | 8-15        | 16-31            |
    /// +-----+------+-------------+------------------+
    /// | 0   | Type | Code        | Checksum         |
    /// +-----+------+-------------+------------------+
    /// | 32  | ID Number          | unused           |
    /// +-----+--------------------+------------------+
    /// | 64  | Outbound Hop Count | Return Hop Count |
    /// +-----+--------------------+------------------+
    /// | 96  | Output Link Speed                     |
    /// +-----+---------------------------------------+
    /// | 128 | Output Link MTU                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.TraceRoute)]
    public class IcmpTraceRouteDatagram : IcmpDatagram
    {
        /// <summary>
        /// The number of bytes this datagram should take.
        /// </summary>
        public const int DatagramLength = HeaderLength + PayloadLength;

        /// <summary>
        /// The number of bytes this ICMP payload should take.
        /// </summary>
        public const int PayloadLength = 12;

        /// <summary>
        /// The value the Return Hop Count should be for an outbound ICMP packet.
        /// </summary>
        public const ushort OutboundReturnHopCountValue = 0xFFFF;

        private static class Offset
        {
            public const int Identifier = 4;
            public const int OutboundHopCount = 8;
            public const int ReturnHopCount = 10;
            public const int OutputLinkSpeed = 12;
            public const int OutputLinkMtu = 16;
        }

        /// <summary>
        /// The ID Number as copied from the IP Traceroute option of the packet which caused this Traceroute message to be sent.  
        /// This is NOT related to the ID number in the IP header.
        /// </summary>
        public ushort Identification
        {
            get { return ReadUShort(Offset.Identifier, Endianity.Big); }
        }

        /// <summary>
        /// The Outbound Hop Count as copied from the IP Traceroute option of the packet which caused this Traceroute message to be sent.
        /// </summary>
        public ushort OutboundHopCount
        {
            get { return ReadUShort(Offset.OutboundHopCount, Endianity.Big); }
        }

        /// <summary>
        /// The Return Hop Count as copied from the IP Traceroute option of the packet which caused this Traceroute message to be sent.
        /// </summary>
        public ushort ReturnHopCount
        {
            get { return ReadUShort(Offset.ReturnHopCount, Endianity.Big); }
        }

        /// <summary>
        /// The speed, in OCTETS per second, of the link over which the Outbound/Return Packet will be sent.  
        /// Since it will not be long before network speeds exceed 4.3Gb/s, and since some machines deal poorly with fields longer than 32 bits, octets per second was chosen over bits per second.  
        /// If this value cannot be determined, the field should be set to zero.
        /// </summary>
        public uint OutputLinkSpeed
        {
            get { return ReadUInt(Offset.OutputLinkSpeed, Endianity.Big); }
        }

        /// <summary>
        /// The MTU, in bytes, of the link over which the Outbound/Return Packet will be sent.  
        /// MTU refers to the data portion (includes IP header; excludes datalink header/trailer) of the packet.  
        /// If this value cannot be determined, the field should be set to zero.
        /// </summary>
        public uint OutputLinkMaximumTransmissionUnit
        {
            get { return ReadUInt(Offset.OutputLinkMtu, Endianity.Big); }
        }

        /// <summary>
        /// Is the packet an Outbound packet.
        /// This is indicated by a value of  0xFFFF in the ReturnHopCount field.
        /// </summary>
        public bool IsOutbound
        {
            get { return ReturnHopCount == OutboundReturnHopCountValue; }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpTraceRouteLayer
            {
                Code = (IcmpCodeTraceRoute)Code,
                Checksum = Checksum,
                Identification = Identification,
                OutboundHopCount = OutboundHopCount,
                ReturnHopCount = ReturnHopCount,
                OutputLinkSpeed = OutputLinkSpeed,
                OutputLinkMaximumTransmissionUnit = OutputLinkMaximumTransmissionUnit
            };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct and the code is in the expected range.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Length == DatagramLength;
        }

        /// <summary>
        /// The minimum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected override byte MinCodeValue
        {
            get { return _minCode; }
        }

        /// <summary>
        /// The maximum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected override byte MaxCodeValue
        {
            get { return _maxCode; }
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpTraceRouteDatagram(buffer, offset, length);
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, ushort outboundHopCount, ushort returnHopCount, uint outputLinkSpeed, uint outputLinkMtu)
        {
            buffer.Write(ref offset, outboundHopCount, Endianity.Big);
            buffer.Write(ref offset, returnHopCount, Endianity.Big);
            buffer.Write(ref offset, outputLinkSpeed, Endianity.Big);
            buffer.Write(offset, outputLinkMtu, Endianity.Big);
        }

        private IcmpTraceRouteDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeTraceRoute).GetEnumValues<IcmpCodeTraceRoute>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeTraceRoute).GetEnumValues<IcmpCodeTraceRoute>().Max();
    }
}