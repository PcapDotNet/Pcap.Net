using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packets
{
    /// <summary>
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | Bit | 0-3     | 4-7 | 8-15            | 16-18 | 19-31           |
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | 0   | Version | IHL | Type of Service | Total Length            |
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | 32  | Identification                  | Flags | Fragment Offset |
    /// +-----+---------------+-----------------+-------+-----------------+
    /// | 64  | Time to Live  | Protocol        | Header Checksum         |
    /// +-----+---------------+-----------------+-------------------------+
    /// | 96  | Source Address                                            |
    /// +-----+-----------------------------------------------------------+
    /// | 128 | Destination Address                                       |
    /// +-----+-----------------------------------------------------------+
    /// | 160 | Options with padding                                      |
    /// +-----+-----------------------------------------------------------+
    /// </summary>
    public class IpV4Datagram : Datagram
    {
        public const int HeaderMinimumLength = 20;

        private static class Offset
        {
            public const int VersionAndHeaderLength = 0;
            public const int TypeOfService = 1;
            public const int TotalLength = 2;
            public const int Identification = 4;
            public const int Fragmentation = 6;
            public const int Ttl = 8;
            public const int Protocol = 9;
            public const int HeaderChecksum = 10;
            public const int Source = 12;
            public const int Destination = 16;
            public const int Options = 20;
        }

        public const int Version = 0x4;

        public int HeaderLength
        {
            get { return (this[Offset.VersionAndHeaderLength] & 0x0F) * 4; }
        }

        public byte TypeOfService
        {
            get { return this[Offset.TypeOfService];}
        }

        public ushort TotalLength
        {
            get { return ReadUShort(Offset.TotalLength, Endianity.Big); }
        }

        public ushort Identification
        {
            get { return ReadUShort(Offset.Identification, Endianity.Big); }
        }

        public IpV4Fragmentation Fragmentation
        {
            get { return new IpV4Fragmentation(ReadUShort(Offset.Fragmentation, Endianity.Big)); }
        }

        public byte Ttl
        {
            get { return this[Offset.Ttl]; }
        }

        public IpV4Protocol Protocol
        {
            get { return (IpV4Protocol)this[Offset.Protocol]; }
        }

        public ushort HeaderChecksum
        {
            get { return ReadUShort(Offset.HeaderChecksum, Endianity.Big); }
        }

        public bool IsHeaderChecksumCorrect
        {
            get { throw new NotImplementedException(); }
        }

        public IpV4Address Source
        {
            get { return new IpV4Address(ReadUInt(Offset.Source, Endianity.Big)); }
        }

        public IpV4Address Destination
        {
            get { return new IpV4Address(ReadUInt(Offset.Destination, Endianity.Big)); }
        }

        public IpV4Options Options
        {
            get { return new IpV4Options(Buffer, StartOffset + Offset.Options); }
        }

        internal IpV4Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         byte typeOfService, ushort identification,
                                         IpV4Fragmentation fragmentation,
                                         byte ttl, IpV4Protocol protocol,
                                         IpV4Address source, IpV4Address destination,
                                         IpV4Options options, int payloadLength)
        {
            int headerLength = HeaderMinimumLength + options.Length;
            buffer[offset + Offset.VersionAndHeaderLength] = (byte)(Version << 4 + headerLength / 4);
            buffer[offset + Offset.TypeOfService] = typeOfService;
            buffer.Write(offset + Offset.TotalLength, (ushort)(headerLength + payloadLength), Endianity.Big);
            buffer.Write(offset + Offset.Identification, identification, Endianity.Big);
            fragmentation.Write(buffer, offset + Offset.Fragmentation);
            buffer[offset + Offset.Ttl] = ttl;
            buffer[offset + Offset.Protocol] = (byte)protocol;
            // Todo Checksum
//            buffer.Write(offset + Offset.Source, source, Endianity.Big);
//            buffer.Write(offset + Offset.Destination, destination, Endianity.Big);
//            options.Write(offset + Offset.Options);
        }
    }

    [Flags]
    public enum IpV4FragmentationFlags : byte
    {
        Reserved =      0x1,
        DontFragment =  0x2,
        MoreFragments = 0x4
    }

    public struct IpV4Fragmentation : IEquatable<IpV4Fragmentation>
    {
        public IpV4Fragmentation(IpV4FragmentationFlags flags, ushort offset)
            : this((ushort)((byte)flags << 13 | offset))
        {
        }

        public bool Equals(IpV4Fragmentation other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return (obj is IpV4Fragmentation &&
                    Equals((IpV4Fragmentation)obj));
        }

        internal IpV4Fragmentation(ushort value)
        {
            _value = value;
        }

        internal void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, _value, Endianity.Big);
        }

        private ushort _value;
    }

    public enum IpV4Protocol : byte
    {
        Tcp = 0x06
    }

    public class IpV4Options
    {
        public static IpV4Options None
        {
            get { return _none; }
        }

        internal IpV4Options()
        {
        }

        internal IpV4Options(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        public int Length
        {
            get { throw new NotImplementedException(); }
        }

        private static IpV4Options _none = new IpV4Options();
    }
}
