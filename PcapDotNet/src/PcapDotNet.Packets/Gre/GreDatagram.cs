using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// RFC 1701, RFC 1702, RFC 2637, RFC 2784.
    /// <pre>
    /// +-----+---+---+---+---+---+-------+-------+---------+-------------------+
    /// | Bit | 0 | 1 | 2 | 3 | 4 | 5-7   | 8-12  | 13-15   | 16-31             |
    /// +-----+---+-----------+---+-------+-------+---------+-------------------+
    /// | 0   | C | R | K | S | s | Recur | Flags | Version | Protocol Type     |
    /// +-----+---+-----------+---+-------+-------+---------+-------------------+
    /// | 32  | Checksum (optional)                         | Offset (optional) |
    /// +-----+---------------------------------------------+-------------------+
    /// | 32  | Key (optional)                                                  |
    /// +-----+-----------------------------------------------------------------+
    /// | 32  | Sequence Number (optional)                                      |
    /// +-----+-----------------------------------------------------------------+
    /// | 32  | Routing (optional)                                              |
    /// +-----+-----------------------------------------------------------------+
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
            public const byte Version = 0x07;

            //        private const ushort Reserved0 = 0x7FF8;
        }

        public const int HeaderMinimumLength = 4;
        public const int HeaderMaximumLength = 8;

        public int HeaderLength
        {
            get
            {
                if (ChecksumPresent)
                    return HeaderMaximumLength;
                return HeaderMinimumLength;
            }
        }

        private int OffsetKey
        {
            get{return HeaderMinimumLength + ((ChecksumPresent || RoutingPresent) ? 4 : 0);}
        }

        private int OffsetSequenceNumber
        {
            get{return OffsetKey + (KeyPresent ? 4 : 0);}
        }

        private int OffsetRouting
        {
            get{return OffsetSequenceNumber + (SequenceNumberPresent ? 4 : 0);}
        }

        /// <summary>
        /// If the Checksum Present bit is set to 1, then the Checksum field is present and contains valid information.
        /// If either the Checksum Present bit or the Routing Present bit are set, BOTH the Checksum and Offset fields are present in the GRE packet.
        /// </summary>
        public bool ChecksumPresent
        {
            get { return (this[Offset.ChecksumPresent] & Mask.ChecksumPresent) != 0; }
        }

        /// <summary>
        /// If the Routing Present bit is set to 1, then it indicates that the Offset and Routing fields are present and contain valid information.
        /// If either the Checksum Present bit or the Routing Present bit are set, BOTH the Checksum and Offset fields are present in the GRE packet.
        /// </summary>
        public bool RoutingPresent
        {
            get { return (this[Offset.RoutingPresent] & Mask.RoutingPresent) != 0; }
        }

        /// <summary>
        /// If the Key Present bit is set to 1, then it indicates that the Key field is present in the GRE header.  
        /// Otherwise, the Key field is not present in the GRE header.
        /// </summary>
        public bool KeyPresent
        {
            get { return (this[Offset.KeyPresent] & Mask.KeyPresent) != 0; }
        }

        /// <summary>
        /// If the Sequence Number Present bit is set to 1, then it indicates that the Sequence Number field is present.  
        /// Otherwise, the Sequence Number field is not present in the GRE header.
        /// </summary>
        public bool SequenceNumberPresent
        {
            get { return (this[Offset.SequenceNumberPresent] & Mask.SequenceNumberPresent) != 0; }
        }

        /// <summary>
        /// If the source route is incomplete, then the Strict Source Route bit is checked.  
        /// If the source route is a strict source route and the next IP destination or autonomous system is NOT an adjacent system, the packet MUST be dropped.
        /// </summary>
        public bool StrictSourceRoute
        {
            get { return (this[Offset.StrictSourceRoute] & Mask.StrictSourceRoute) != 0; }
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

        /// <summary>
        /// The offset field indicates the octet offset from the start of the Routing field to the first octet of the active Source Route Entry to be examined.  
        /// This field is present if the Routing Present or the Checksum Present bit is set to 1, and contains valid information only if the Routing Present bit is set to 1.
        /// </summary>
        public ushort RoutingOffset
        {
            get { return ReadUShort(Offset.RoutingOffset, Endianity.Big); }
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

        /// <summary>
        /// The Sequence Number field contains an unsigned 32 bit integer which is inserted by the encapsulator.  
        /// It may be used by the receiver to establish the order in which packets have been transmitted from the encapsulator to the receiver.  
        /// </summary>
        public uint SequenceNumber
        {
            get { return ReadUInt(OffsetSequenceNumber, Endianity.Big); }
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
                if (_routing == null)
                {
                    List<GreSourceRouteEntry> entries = new List<GreSourceRouteEntry>();
                    int entryOffset = StartOffset + OffsetRouting;

                    while (Length >= entryOffset)
                    {
                        GreSourceRouteEntry entry;
                        if (!GreSourceRouteEntry.TryReadEntry(Buffer, ref entryOffset, Length - entryOffset, out entry))
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

                return _routing;
            }
        }

        public IpV4Datagram IpV4
        {
            get
            {
                if (_ipV4 == null && Length >= HeaderLength)
                    _ipV4 = new IpV4Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);
                return _ipV4;
            }
        }

        public override ILayer ExtractLayer()
        {
            throw new NotImplementedException();
        }

        protected override bool CalculateIsValid()
        {
            throw new NotImplementedException();
//            return (Length >= HeaderMinimumLength && HeaderLength >= HeaderLength &&
//                    Reserved0 == 0 && Reserved1 == 0);
        }

        internal GreDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private IpV4Datagram _ipV4;
        private ReadOnlyCollection<GreSourceRouteEntry> _routing;
        private bool _isValidRouting = true;
    }

    public abstract class GreSourceRouteEntry
    {
        public const int HeaderLength = 4;

        private static class Offset
        {
            public const int SreOffset = 2;
            public const int SreLength = 3;
        }

        internal static bool TryReadEntry(byte[] buffer, ref int offset, int length, out GreSourceRouteEntry entry)
        {
            entry = null;
            if (length < HeaderLength)
                return false;

            // Address Family
            GreSourceRouteEntryAddressFamily addressFamily = (GreSourceRouteEntryAddressFamily)buffer.ReadUShort(ref offset, Endianity.Big);

            // SRE Length
            byte sreLength = buffer[offset + Offset.SreLength];
            if (sreLength == 0)
                return addressFamily == GreSourceRouteEntryAddressFamily.None;
            
            if (HeaderLength + sreLength > length)
                return false;

            // SRE Offset
            byte sreOffset = buffer[offset + Offset.SreOffset];
            if (sreOffset > sreLength)
                return false;

            // Entry
            if (!TryReadEntry(buffer, offset + HeaderLength, sreLength, addressFamily, sreOffset, out entry))
                return false;

            // Change offset
            offset += sreLength;
            return true;
        }

        private static bool TryReadEntry(byte[] buffer, int payloadOffset, int payloadLength, GreSourceRouteEntryAddressFamily addressFamily, int offsetInPayload, out GreSourceRouteEntry entry)
        {
            entry = null;
            switch (addressFamily)
            {
                case GreSourceRouteEntryAddressFamily.IpSourceRoute:
                    if (offsetInPayload % IpV4Address.SizeOf != 0 || payloadLength % IpV4Address.SizeOf != 0)
                        return false;

                    int numAddresses = payloadLength / IpV4Address.SizeOf;
                    IpV4Address[] addresses = new IpV4Address[numAddresses];
                    for (int i = 0; i != numAddresses; ++i)
                        addresses[i] = buffer.ReadIpV4Address(payloadOffset + i * IpV4Address.SizeOf, Endianity.Big);

                    entry = new GreSourceRouteEntryIp(addresses, offsetInPayload / IpV4Address.SizeOf);
                    return true;

                case GreSourceRouteEntryAddressFamily.AsSourceRoute:
                    if (offsetInPayload % sizeof(ushort) != 0 || payloadLength % sizeof(ushort) != 0)
                        return false;

                    int numAsNumbers = payloadLength / sizeof(ushort);
                    ushort[] asNumbers = new ushort[numAsNumbers];
                    for (int i = 0; i != numAsNumbers; ++i)
                        asNumbers[i] = buffer.ReadUShort(payloadOffset + i * sizeof(ushort), Endianity.Big);

                    entry = new GreSourceRouteEntryAs(asNumbers, offsetInPayload / sizeof(ushort));
                    return true;

                default:
                    Datagram data = new Datagram(buffer, payloadOffset, payloadLength);
                    entry = new GreSourceRouteEntryUnknown(data, offsetInPayload);
                    return true;
            }
        }
    }

    public class GreSourceRouteEntryIp : GreSourceRouteEntry
    {
        public ReadOnlyCollection<IpV4Address> Addresses
        {
            get { return _addresses; }
        }

        public int NextAddressIndex
        {
            get { return _nextAddressIndex; }
        }

        public IpV4Address NextAddress
        {
            get { return Addresses[NextAddressIndex]; }
        }

        internal GreSourceRouteEntryIp(IpV4Address[] addresses, int nextAddressIndex)
        {
            _addresses = new ReadOnlyCollection<IpV4Address>(addresses);
            _nextAddressIndex = nextAddressIndex;
        }

        private readonly ReadOnlyCollection<IpV4Address> _addresses;
        private readonly int _nextAddressIndex;
    }

    public class GreSourceRouteEntryAs : GreSourceRouteEntry
    {
        public ReadOnlyCollection<ushort> AsNumbers
        {
            get { return _asNumbers; }
        }

        public int NextAsNumberIndex
        {
            get { return _nextAsNumberIndex; }
        }

        public ushort NextAsNumber
        {
            get { return AsNumbers[NextAsNumberIndex]; }
        }

        internal GreSourceRouteEntryAs(ushort[] asNumbers, int nextAsNumberIndex)
        {
            _asNumbers = new ReadOnlyCollection<ushort>(asNumbers);
            _nextAsNumberIndex = nextAsNumberIndex;
        }

        private readonly ReadOnlyCollection<ushort> _asNumbers;
        private readonly int _nextAsNumberIndex;
    }

    public class GreSourceRouteEntryUnknown : GreSourceRouteEntry
    {
        public Datagram Data
        {
            get { return _data; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        internal GreSourceRouteEntryUnknown(Datagram data, int offset)
        {
            _data = data;
            _offset = offset;
        }

        private readonly Datagram _data;
        private readonly int _offset;
    }
    public enum GreSourceRouteEntryAddressFamily : ushort
    {
        None = 0x0000,
        IpSourceRoute = 0x0800,
        AsSourceRoute = 0xfffe,
    }

    public enum GreVersion : byte
    {
        /// <summary>
        /// RFC 2784
        /// </summary>
        Gre = 0x00,

        /// <summary>
        /// RFC 2637
        /// </summary>
        EnhancedGre = 0x01
    }
}