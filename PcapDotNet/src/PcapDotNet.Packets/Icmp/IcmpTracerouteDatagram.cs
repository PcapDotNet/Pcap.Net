using System;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1393.
    /// <pre>
    /// +-----+--------------------+------------------+
    /// | Bit | 0-15               | 16-31            |
    /// +-----+--------------------+------------------+
    /// | 0   | ID Number          | unused           |
    /// +-----+--------------------+------------------+
    /// | 32  | Outbound Hop Count | Return Hop Count |
    /// +-----+--------------------+------------------+
    /// | 64  | Output Link Speed                     |
    /// +-----+---------------------------------------+
    /// | 96  | Output Link MTU                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpTracerouteDatagram : IcmpTypedDatagram
    {
        public const int HeaderAdditionalLength = 12;

        private class Offset
        {
            public const int Identifier = 0;
            public const int OutboundHopCount = 4;
            public const int ReturnHopCount = 6;
            public const int OutputLinkSpeed = 8;
            public const int OutputLinkMtu = 12;
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
        public uint OutputLinkMtu
        {
            get { return ReadUInt(Offset.OutputLinkMtu, Endianity.Big); }
        }

        internal IcmpTracerouteDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, ushort outboundHopCount, ushort returnHopCount, uint outputLinkSpeed, uint outputLinkMtu)
        {
            buffer.Write(ref offset, outboundHopCount, Endianity.Big);
            buffer.Write(ref offset, returnHopCount, Endianity.Big);
            buffer.Write(ref offset, outputLinkSpeed, Endianity.Big);
            buffer.Write(offset, outputLinkMtu, Endianity.Big);
        }
    }
}