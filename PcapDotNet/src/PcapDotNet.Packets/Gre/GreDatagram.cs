using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// RFC 1701, RFC 1702, RFC 2637, RFC 2784.
    /// <pre>
    /// +-----+---+---+---+---+---+-------+---+-------+---------+-------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4 | 5-7   | 8 | 9-12  | 13-15   | 16-31             |
    /// +-----+---+-----------+---+-------+---+-------+---------+-------------------+
    /// | 0   | C | R | K | S | s | Recur | A | Flags | Version | Protocol Type     |
    /// +-----+---+-----------+---+-------+---+-------+---------+-------------------+
    /// | 32  | Checksum (optional)                             | Offset (optional) |
    /// +-----+-------------------------------------------------+-------------------+
    /// | 32  | Key (optional)                                                      |
    /// +-----+---------------------------------------------------------------------+
    /// | 32  | Sequence Number (optional)                                          |
    /// +-----+---------------------------------------------------------------------+
    /// | 32  | Acknowledgment Number (optional)                                    |
    /// +-----+---------------------------------------------------------------------+
    /// | 32  | Routing (optional)                                                  |
    /// +-----+---------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public class GreDatagram : Datagram
    {
        private static class Offset
        {
            public const int ChecksumPresent = 0;
            public const int RoutingPresent = 0;
            public const int KeyPresent = 0;
            public const int SequenceNumberPresent = 0;
            public const int StrictSourceRoute = 0;
            public const int RecursionControl = 0;
            public const int AcknowledgmentSequenceNumberPresent = 1;
            public const int Flags = 1;
            public const int Version = 1;
            public const int ProtocolType = 2;
            public const int Checksum = 4;
            public const int RoutingOffset = 6;
        }

        private static class Mask
        {
            public const byte ChecksumPresent = 0x80;
            public const byte RoutingPresent = 0x40;
            public const byte KeyPresent = 0x20;
            public const byte SequenceNumberPresent = 0x10;
            public const byte StrictSourceRoute = 0x08;
            public const byte RecursionControl = 0x07;
            public const byte AcknowledgmentSequenceNumberPresent = 0x80;
            public const byte Flags = 0x78;
            public const byte Version = 0x07;
        }

        private static class Shift
        {
            public const int Flags = 3;
        }

        public const int HeaderMinimumLength = 4;

        public int HeaderLength
        {
            get
            {
                return GetHeaderLength(ChecksumPresent, KeyPresent, SequenceNumberPresent, AcknowledgmentSequenceNumberPresent, Routing);
            }
        }

        /// <summary>
        /// If the Checksum Present bit is set to 1, then the Checksum field is present and contains valid information.
        /// If either the Checksum Present bit or the Routing Present bit are set, BOTH the Checksum and Offset fields are present in the GRE packet.
        /// </summary>
        public bool ChecksumPresent
        {
            get { return (this[Offset.ChecksumPresent] & Mask.ChecksumPresent) == Mask.ChecksumPresent; }
        }

        /// <summary>
        /// If the Routing Present bit is set to 1, then it indicates that the Offset and Routing fields are present and contain valid information.
        /// If either the Checksum Present bit or the Routing Present bit are set, BOTH the Checksum and Offset fields are present in the GRE packet.
        /// </summary>
        public bool RoutingPresent
        {
            get { return (this[Offset.RoutingPresent] & Mask.RoutingPresent) == Mask.RoutingPresent; }
        }

        /// <summary>
        /// If the Key Present bit is set to 1, then it indicates that the Key field is present in the GRE header.  
        /// Otherwise, the Key field is not present in the GRE header.
        /// </summary>
        public bool KeyPresent
        {
            get { return (this[Offset.KeyPresent] & Mask.KeyPresent) == Mask.KeyPresent; }
        }

        /// <summary>
        /// If the Sequence Number Present bit is set to 1, then it indicates that the Sequence Number field is present.  
        /// Otherwise, the Sequence Number field is not present in the GRE header.
        /// </summary>
        public bool SequenceNumberPresent
        {
            get { return (this[Offset.SequenceNumberPresent] & Mask.SequenceNumberPresent) == Mask.SequenceNumberPresent; }
        }

        /// <summary>
        /// If the source route is incomplete, then the Strict Source Route bit is checked.  
        /// If the source route is a strict source route and the next IP destination or autonomous system is NOT an adjacent system, the packet MUST be dropped.
        /// </summary>
        public bool StrictSourceRoute
        {
            get { return (this[Offset.StrictSourceRoute] & Mask.StrictSourceRoute) == Mask.StrictSourceRoute; }
        }

        /// <summary>
        /// Recursion control contains a three bit unsigned integer which contains the number of additional encapsulations which are permissible.  
        /// This SHOULD default to zero.
        /// </summary>
        public byte RecursionControl
        {
            get { return (byte)(this[Offset.RecursionControl] & Mask.RecursionControl); }
        }

        /// <summary>
        /// Set to one (1) if packet contains Acknowledgment Number to be used for acknowledging previously transmitted data.
        /// </summary>
        public bool AcknowledgmentSequenceNumberPresent
        {
            get { return (this[Offset.AcknowledgmentSequenceNumberPresent] & Mask.AcknowledgmentSequenceNumberPresent) == Mask.AcknowledgmentSequenceNumberPresent; }
        }

        /// <summary>
        /// Must be set to zero (0).
        /// </summary>
        public byte Flags
        {
            get { return (byte)((this[Offset.Flags] & Mask.Flags) >> Shift.Flags); }
        }

        /// <summary>
        /// The Version Number field MUST contain the value zero.
        /// ?
        /// </summary>
        public GreVersion Version
        {
            get { return (GreVersion)(this[Offset.Version] & Mask.Version); }
        }

        /// <summary>
        /// The Protocol Type field contains the protocol type of the payload packet. 
        /// These Protocol Types are defined in [RFC1700] as "ETHER TYPES" and in [ETYPES]. 
        /// An implementation receiving a packet containing a Protocol Type which is not listed in [RFC1700] or [ETYPES] SHOULD discard the packet.
        /// </summary>
        public EthernetType ProtocolType
        {
            get { return (EthernetType)ReadUShort(Offset.ProtocolType, Endianity.Big); }
        }

        /// <summary>
        /// The Checksum field contains the IP (one's complement) checksum sum of the all the 16 bit words in the GRE header and the payload packet.
        /// For purposes of computing the checksum, the value of the checksum field is zero. 
        /// This field is present only if the Checksum Present bit is set to one.
        /// </summary>
        public ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        public bool IsChecksumCorrect
        {
            get
            {
                if (_isChecksumCorrect == null)
                    _isChecksumCorrect = (CalculateChecksum() == Checksum);
                return _isChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// The offset field indicates the octet offset from the start of the Routing field to the first octet of the active Source Route Entry to be examined.  
        /// This field is present if the Routing Present or the Checksum Present bit is set to 1, and contains valid information only if the Routing Present bit is set to 1.
        /// </summary>
        public ushort RoutingOffset
        {
            get { return ReadUShort(Offset.RoutingOffset, Endianity.Big); }
        }

        public int? ActiveSourceRouteEntryIndex
        {
            get
            {
                TryParseRouting();
                return _activeSourceRouteEntryIndex;
            }
        }

        public GreSourceRouteEntry ActiveSourceRouteEntry
        {
            get
            {
                int? activeSourceRouteEntryIndex = ActiveSourceRouteEntryIndex;
                if (activeSourceRouteEntryIndex == null || activeSourceRouteEntryIndex == Routing.Count)
                    return null;

                return Routing[activeSourceRouteEntryIndex.Value];
            }
        }

        /// <summary>
        /// The Key field contains a four octet number which was inserted by the encapsulator.  
        /// It may be used by the receiver to authenticate the source of the packet.  
        /// The Key field is only present if the Key Present field is set to 1.
        /// </summary>
        public uint Key
        {
            get { return ReadUInt(OffsetKey, Endianity.Big); }
        }

        public ushort KeyPayloadLength
        {
            get { return ReadUShort(OffsetKeyPayloadLength, Endianity.Big); }
        }

        public ushort KeyCallId
        {
            get { return ReadUShort(OffsetKeyCallId, Endianity.Big); }
        }

        /// <summary>
        /// The Sequence Number field contains an unsigned 32 bit integer which is inserted by the encapsulator.  
        /// It may be used by the receiver to establish the order in which packets have been transmitted from the encapsulator to the receiver.  
        /// </summary>
        public uint SequenceNumber
        {
            get { return ReadUInt(OffsetSequenceNumber, Endianity.Big); }
        }

        /// <summary>
        /// Contains the sequence number of the highest numbered GRE packet received by the sending peer for this user session.
        /// Present if A bit (Bit 8) is one (1).
        /// </summary>
        public uint  AcknowledgmentSequenceNumber
        {
            get { return ReadUInt(OffsetAcknowledgmentSequenceNumber, Endianity.Big); }
        }

        /// <summary>
        /// The Routing field is optional and is present only if the Routing Present bit is set to 1.
        /// The Routing field is a list of Source Route Entries (SREs). 
        /// Each SRE has the form:
        /// <pre>
        /// +-----+----------------+------------+------------+
        /// | Bit | 0-15           | 16-23      | 24-31      |
        /// +-----+----------------+------------+------------+
        /// | 0   | Address Family | SRE Offset | SRE Length |
        /// +-----+----------------+------------+------------+
        /// | 32  | Routing Information ...                  |
        /// +-----+------------------------------------------+
        /// </pre>
        /// The routing field is terminated with a "NULL" SRE containing an address family of type 0x0000 and a length of 0.
        /// </summary>
        public ReadOnlyCollection<GreSourceRouteEntry> Routing
        {
            get
            {
                TryParseRouting();
                return _routing;
            }
        }

        public override ILayer ExtractLayer()
        {
            return new GreLayer
                   {
                       Version = Version,
                       ProtocolType = ProtocolType,
                       RecursionControl = RecursionControl,
                       Flags = Flags,
                       ChecksumPresent = ChecksumPresent,
                       Checksum = ChecksumPresent ? (ushort?)Checksum : null,
                       Key = KeyPresent ? (uint?)Key : null,
                       SequenceNumber = SequenceNumberPresent ? (uint?)SequenceNumber : null,
                       AcknowledgmentSequenceNumber = AcknowledgmentSequenceNumberPresent ? (uint?)AcknowledgmentSequenceNumber : null,
                       Routing = RoutingPresent ? Routing : null,
                       RoutingOffset = RoutingPresent ? (ushort?)RoutingOffset : null,
                       StrictSourceRoute = StrictSourceRoute,
                   };
        }

        /// <summary>
        /// The Ethernet payload.
        /// </summary>
        public Datagram Payload
        {
            get { return PayloadDatagrams.Payload; }
        }

        /// <summary>
        /// The Ethernet payload as an IPv4 datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get { return PayloadDatagrams.IpV4; }
        }

        /// <summary>
        /// The Ethernet payload as an ARP datagram.
        /// </summary>
        public ArpDatagram Arp
        {
            get { return PayloadDatagrams.Arp; }
        }


        protected override bool CalculateIsValid()
        {
            return (Length >= HeaderMinimumLength && Length >= HeaderLength &&
                    IsValidRouting && Flags == 0 &&
                    (Version == GreVersion.EnhancedGre || Version == GreVersion.Gre && !AcknowledgmentSequenceNumberPresent) &&
                    (!ChecksumPresent || IsChecksumCorrect) &&
                    PayloadDatagrams.Get(ProtocolType).IsValid);
        }

        internal GreDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static int GetHeaderLength(bool isChecksumPresent, bool isKeyPresent, bool isSequenceNumberPresent, bool isAcknowledgmentSequenceNumberPresent, IEnumerable<GreSourceRouteEntry> routing)
        {
            return HeaderMinimumLength +
                   (isChecksumPresent || routing != null ? sizeof(ushort) + sizeof(ushort) : 0) +
                   (isKeyPresent ? sizeof(uint) : 0) +
                   (isSequenceNumberPresent ? sizeof(uint) : 0) +
                   (isAcknowledgmentSequenceNumberPresent ? sizeof(uint) : 0) +
                   (routing != null ? routing.Sum(entry => entry.Length) + GreSourceRouteEntry.HeaderLength : 0);
        }

        internal static void WriteHeader(byte[] buffer, int offset,
            byte recursionControl, byte flags, GreVersion version, EthernetType protocolType,
            bool checksumPresent, uint? key, uint? sequenceNumber, uint? acknowledgmentSequenceNumber,
            ReadOnlyCollection<GreSourceRouteEntry> routing, ushort? routingOffset, bool strictSourceRoute)
        {
            buffer.Write(offset + Offset.ChecksumPresent,
                         (byte)((checksumPresent ? Mask.ChecksumPresent : (byte)0) |
                                (routing != null ? Mask.RoutingPresent : (byte)0) |
                                (key != null ? Mask.KeyPresent : (byte)0) |
                                (sequenceNumber != null ? Mask.SequenceNumberPresent : (byte)0) |
                                (strictSourceRoute ? Mask.StrictSourceRoute : (byte)0) |
                                (recursionControl & Mask.RecursionControl)));

            buffer.Write(offset + Offset.Flags, (byte)((acknowledgmentSequenceNumber != null ? Mask.AcknowledgmentSequenceNumberPresent : (byte)0) |
                                                       ((flags << Shift.Flags) & Mask.Flags) |
                                                       ((byte)version & Mask.Version)));

            buffer.Write(offset + Offset.ProtocolType, (ushort)protocolType, Endianity.Big);

            offset += Offset.Checksum;
            if (checksumPresent || routing != null)
            {
                offset += sizeof(ushort);
                if (routingOffset != null)
                    buffer.Write(offset, routingOffset.Value, Endianity.Big);
                offset += sizeof(ushort);
            }

            if (key != null)
                buffer.Write(ref offset, key.Value, Endianity.Big);

            if (sequenceNumber != null)
                buffer.Write(ref offset, sequenceNumber.Value, Endianity.Big);

            if (acknowledgmentSequenceNumber != null)
                buffer.Write(ref offset, acknowledgmentSequenceNumber.Value, Endianity.Big);

            if (routing != null)
            {
                foreach (GreSourceRouteEntry entry in routing)
                    entry.Write(buffer, ref offset);

                buffer.Write(ref offset, (uint)0, Endianity.Big);
            }
        }

        internal static void WriteChecksum(byte[] buffer, int offset, int length, ushort? checksum)
        {
            ushort checksumValue = checksum == null
                                       ? CalculateChecksum(buffer, offset, length)
                                       : checksum.Value;
            buffer.Write(offset + Offset.Checksum, checksumValue, Endianity.Big);
        }

        private int OffsetKey
        {
            get { return HeaderMinimumLength + ((ChecksumPresent || RoutingPresent) ? sizeof(ushort) + sizeof(ushort) : 0); }
        }

        private int OffsetKeyPayloadLength
        {
            get { return OffsetKey; }
        }

        private int OffsetKeyCallId
        {
            get { return OffsetKey + sizeof(ushort); }
        }

        private int OffsetSequenceNumber
        {
            get { return OffsetKey + (KeyPresent ? sizeof(uint) : 0); }
        }

        private int OffsetAcknowledgmentSequenceNumber
        {
            get { return OffsetSequenceNumber + (SequenceNumberPresent ? sizeof(uint) : 0); }
        }

        private int OffsetRouting
        {
            get { return OffsetAcknowledgmentSequenceNumber + (AcknowledgmentSequenceNumberPresent ? sizeof(uint) : 0); }
        }

        private void TryParseRouting()
        {
            if (_routing != null || !RoutingPresent)
                return;

            List<GreSourceRouteEntry> entries = new List<GreSourceRouteEntry>();
            int routingStartOffset = StartOffset + OffsetRouting;
            int entryOffset = routingStartOffset;

            int totalLength = StartOffset + Length;
            while (totalLength >= entryOffset)
            {
                GreSourceRouteEntry entry;
                if (entryOffset == routingStartOffset + RoutingOffset)
                    _activeSourceRouteEntryIndex = entries.Count;
                if (!GreSourceRouteEntry.TryReadEntry(Buffer, ref entryOffset, totalLength - entryOffset, out entry))
                {
                    _isValidRouting = false;
                    break;
                }

                if (entry == null)
                    break;

                entries.Add(entry);
            }

            _routing = new ReadOnlyCollection<GreSourceRouteEntry>(entries);
        }

        private ushort CalculateChecksum()
        {
            return CalculateChecksum(Buffer, StartOffset, Length);
        }

        private static ushort CalculateChecksum(byte[] buffer, int offset, int length)
        {
            uint sum = Sum16Bits(buffer, offset, Math.Min(Offset.Checksum, length)) +
                       Sum16Bits(buffer, offset + Offset.Checksum + sizeof(ushort), length - Offset.Checksum - sizeof(ushort));

            return Sum16BitsToChecksum(sum);
        }

        private bool IsValidRouting
        {
            get
            {
                TryParseRouting();

                return _isValidRouting;
            }
        }

        private EthernetPayloadDatagrams PayloadDatagrams
        {
            get
            {
                if (_payloadDatagrams == null)
                {
                    _payloadDatagrams = new EthernetPayloadDatagrams(Length >= HeaderLength
                                                                         ? new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength)
                                                                         : null);
                }
                return _payloadDatagrams;
            }
        }
        
        private EthernetPayloadDatagrams _payloadDatagrams;
        private ReadOnlyCollection<GreSourceRouteEntry> _routing;
        private bool _isValidRouting = true;
        private bool? _isChecksumCorrect;
        private int? _activeSourceRouteEntryIndex;
    }
}