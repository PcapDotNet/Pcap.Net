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
            get 
            { 
                if (_isHeaderChecksumCorrect == null)
                    _isHeaderChecksumCorrect = (CalculateHeaderChecksum() == HeaderChecksum);
                return _isHeaderChecksumCorrect.Value;
            }
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
            get { return new IpV4Options(Buffer, StartOffset + Offset.Options, HeaderLength - HeaderMinimumLength); }
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

        private ushort CalculateHeaderChecksum()
        {
            // and 16 bits
            uint sum = 0;
            for (int offset = 0; offset < Offset.HeaderChecksum; offset += 2)
                sum += ReadUShort(offset, Endianity.Big);
            for (int offset = Offset.HeaderChecksum + 2; offset < HeaderLength; offset += 2)
                sum += ReadUShort(offset, Endianity.Big);

            // take only 16 bits out of the 32 bit sum and add up the carrier
            sum = (sum & 0x0000FFFF) + (sum >> 16);

            // one's complement the result
            sum = ~sum;
            return (ushort)sum;
        }

        private bool? _isHeaderChecksumCorrect;
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

        internal IpV4Options(byte[] buffer, int offset, int length)
        {
            int offsetEnd = offset + length;
            while (offset != offsetEnd)
            {
                IpV4Option option = IpV4Option.Read(buffer, ref offset, offsetEnd - offset);
                if (option == null)
                    break;
                
                _options.Add(option);
                if (option is IpV4OptionEndOfOptionsList)
                    break;
            }
        }

        public int Length
        {
            get { throw new NotImplementedException(); }
        }

        private List<IpV4Option> _options = new List<IpV4Option>();
        private static IpV4Options _none = new IpV4Options();
    }

    public enum IpV4OptionType : byte
    {
        EndOfOptionList = 0,
        NoOperation = 1,
        Security = 130,
        LooseSourceRouting = 131,
        StrictSourceRouting = 137,
        RecordRoute = 7,
        StreamIdentifier = 136,
        InternetTimestamp = 68
    }

    public abstract class IpV4Option
    {
        protected IpV4Option(IpV4OptionType type)
        {
            _type = type;
        }

        internal static IpV4Option Read(byte[] buffer, ref int offset, int length)
        {
            int offsetEnd = offset + length;
            if (offset == offsetEnd)
                return null;

            IpV4OptionType optionType = (IpV4OptionType)buffer[offset++];
            switch (optionType)
            {
                case IpV4OptionType.EndOfOptionList:
                    return new IpV4OptionEndOfOptionsList(optionType);
                case IpV4OptionType.NoOperation:
                    return new IpV4OptionNoOperation(optionType);
                case IpV4OptionType.Security:
                    return IpV4OptionSecurity.Read(optionType, buffer, ref offset, offsetEnd - offset);

                    // Todo support more option types
//                case IpV4OptionType.LooseSourceRouting:
//                    return IpV4OptionLooseSourceRouting.Read(optionType, buffer, ref offset, offsetEnd - offset);
//                case IpV4OptionType.StrictSourceRouting:
//                    return IpV4OptionStrictSourceRouting.Read(optionType, buffer, ref offset, offsetEnd - offset);
//                case IpV4OptionType.RecordRoute:
//                    return IpV4OptionRecordRoute.Read(optionType, buffer, ref offset, offsetEnd - offset);
//                case IpV4OptionType.StreamIdentifier:
//                    return IpV4OptionStreamIdentifier.Read(optionType, buffer, ref offset, offsetEnd - offset);
//                case IpV4OptionType.InternetTimestamp:
//                    return IpV4OptionInternetTimestamp.Read(optionType, buffer, ref offset, offsetEnd - offset);
                default:
                    return null;
            }
        }

        private IpV4OptionType _type;
    }

//    public class IpV4OptionInternetTimestamp : IpV4Option
//    {
//        internal static IpV4OptionInternetTimestamp Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
//        {
//        }
//    }
//
//    public class IpV4OptionStreamIdentifier : IpV4Option
//    {
//        internal static IpV4OptionStreamIdentifier Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
//        {
//        }
//    }
//
//    public class IpV4OptionRecordRoute : IpV4Option
//    {
//        internal static IpV4OptionRecordRoute Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
//        {
//        }
//    }
//
//    public class IpV4OptionStrictSourceRouting : IpV4Option
//    {
//        internal static IpV4OptionStrictSourceRouting Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
//        {
//        }
//    }
//
//    public class IpV4OptionLooseSourceRouting : IpV4Option
//    {
//        internal static IpV4OptionLooseSourceRouting Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
//        {
//        }
//    }

    public enum IpV4OptionSecurityLevel : ushort
    {
        Unclassified = 0x0000,
        Confidential = 0xF135,
        EFTO = 0x789A,
        MMMM = 0xBC4D,
        PROG = 0x5E26,
        Restricted = 0xAF13,
        Secret = 0xD788,
        TopSecret = 0x6BC5
    }

    public class IpV4OptionSecurity : IpV4Option
    {
        internal static IpV4OptionSecurity Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 12)
                return null;
            byte optionLength = buffer[offset++];
            if (optionLength != 11)
                return null;

            IpV4OptionSecurityLevel level = (IpV4OptionSecurityLevel)buffer.ReadUShort(ref offset, Endianity.Big);
            ushort compartments = buffer.ReadUShort(ref offset, Endianity.Big);
            ushort handlingRestrictions = buffer.ReadUShort(ref offset, Endianity.Big);
            uint transmissionControlCode = (uint)((buffer.ReadUShort(ref offset, Endianity.Big) << 8) +
                                                  buffer[offset++]);

            return new IpV4OptionSecurity(optionType, level, compartments, handlingRestrictions, transmissionControlCode);
        }

        private IpV4OptionSecurity(IpV4OptionType optionType,
                                   IpV4OptionSecurityLevel level, ushort compartments,
                                   ushort handlingRestrictions, uint transmissionControlCode)
            : base(optionType)
        {
            _level = level;
            _compartments = compartments;
            _handlingRestrictions = handlingRestrictions;
            _transmissionControlCode = transmissionControlCode;
        }

        private IpV4OptionSecurityLevel _level;
        private ushort _compartments;
        private ushort _handlingRestrictions;
        private uint _transmissionControlCode; // this could actually be 24 bits
    }

    public class IpV4OptionNoOperation : IpV4Option
    {
        public IpV4OptionNoOperation(IpV4OptionType type)
            : base(type)
        {
        }
    }

    public class IpV4OptionEndOfOptionsList : IpV4Option
    {
        public IpV4OptionEndOfOptionsList(IpV4OptionType type)
            : base(type)
        {
        }
    }
}
