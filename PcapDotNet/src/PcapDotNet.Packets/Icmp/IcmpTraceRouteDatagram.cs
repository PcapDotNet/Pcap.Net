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
    public class IcmpTraceRouteDatagram : IcmpDatagram
    {
        public const int DatagramLength = HeaderLength + PayloadLength;
        public const int PayloadLength = 12;
        public const ushort OutboundReturnHopCountValue = 0xFFFF;

        private static class Offset
        {
            public const int Identifier = 4;
            public const int OutboundHopCount = 8;
            public const int ReturnHopCount = 10;
            public const int OutputLinkSpeed = 12;
            public const int OutputLinkMtu = 16;
        }

        internal IcmpTraceRouteDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
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

        public bool IsOutbound
        {
            get { return ReturnHopCount == OutboundReturnHopCountValue; }
        }

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

        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Length == DatagramLength;
        }

        protected override byte MinCodeValue
        {
            get { return _minCode; }
        }

        protected override byte MaxCodeValue
        {
            get { return _maxCode; }
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, ushort outboundHopCount, ushort returnHopCount, uint outputLinkSpeed, uint outputLinkMtu)
        {
            buffer.Write(ref offset, outboundHopCount, Endianity.Big);
            buffer.Write(ref offset, returnHopCount, Endianity.Big);
            buffer.Write(ref offset, outputLinkSpeed, Endianity.Big);
            buffer.Write(offset, outputLinkMtu, Endianity.Big);
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeTraceRoute).GetEnumValues<IcmpCodeTraceRoute>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeTraceRoute).GetEnumValues<IcmpCodeTraceRoute>().Max();
    }
}