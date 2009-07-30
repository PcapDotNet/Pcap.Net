using System;
using System.Collections;
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

        public Datagram Payload
        {
            get { return Tcp; }
        }

        public Datagram Tcp
        {
            get
            {
                if (_tcp == null && Length >= HeaderLength)
                    _tcp = new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);

                return _tcp;
            }
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
            buffer[offset + Offset.VersionAndHeaderLength] = (byte)((Version << 4) + headerLength / 4);
            buffer[offset + Offset.TypeOfService] = typeOfService;
            buffer.Write(offset + Offset.TotalLength, (ushort)(headerLength + payloadLength), Endianity.Big);
            buffer.Write(offset + Offset.Identification, identification, Endianity.Big);
            fragmentation.Write(buffer, offset + Offset.Fragmentation);
            buffer[offset + Offset.Ttl] = ttl;
            buffer[offset + Offset.Protocol] = (byte)protocol;

            buffer.Write(offset + Offset.Source, source.ToValue(), Endianity.Big);
            buffer.Write(offset + Offset.Destination, destination.ToValue(), Endianity.Big);
//            options.Write(offset + Offset.Options);

            buffer.Write(offset + Offset.HeaderChecksum, Sum16BitsToChecksum(Sum16Bits(buffer, offset, headerLength)), Endianity.Big);
        }

        private ushort CalculateHeaderChecksum()
        {
            uint sum = Sum16Bits(Buffer, StartOffset, Offset.HeaderChecksum) +
                       Sum16Bits(Buffer, StartOffset + Offset.HeaderChecksum + 2, HeaderLength - Offset.HeaderChecksum - 2);

            return Sum16BitsToChecksum(sum);
        }

        private static ushort Sum16BitsToChecksum(uint sum)
        {
            // take only 16 bits out of the 32 bit sum and add up the carrier
            sum = (sum & 0x0000FFFF) + (sum >> 16);

            // one's complement the result
            sum = ~sum;
            return (ushort)sum;
        }

        private static uint Sum16Bits(byte[] buffer, int offset, int length)
        {
            int endOffset = offset + length;
            uint sum = 0;
            while (offset < endOffset - 1)
                sum += buffer.ReadUShort(ref offset, Endianity.Big);
            return sum;
        }

        private bool? _isHeaderChecksumCorrect;
        private Datagram _tcp;
    }

    public class IpV4Options : IEquatable<IpV4Options>
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
            _length = length;

            int offsetEnd = offset + length;
            while (offset != offsetEnd)
            {
                IpV4Option option = IpV4Option.Read(buffer, ref offset, offsetEnd - offset);
                if (option == null)
                    return; // Invalid

                if (option.IsAppearsAtMostOnce)
                {
                    foreach (IpV4Option previousOption in _options)
                    {
                        if (option.GetType() == previousOption.GetType())
                            return; // Invalid
                    }
                }

                _options.Add(option);
                if (option is IpV4OptionEndOfOptionsList)
                    break; // Valid?
            }
        }

        public int Length
        {
            get { return _length; }
        }

        public bool Equals(IpV4Options other)
        {
            if (other == null)
                return false;

            if (Length != other.Length)
                return false;

            return _options.EqualCollection(other._options);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV4Options);
        }

        private List<IpV4Option> _options = new List<IpV4Option>();
        private int _length;
        private static IpV4Options _none = new IpV4Options();
    }

    public static class MoreIEnumerable
    {
        public static bool EqualCollection<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            IEnumerator<T> enumerator1 = collection1.GetEnumerator();
            IEnumerator<T> enumerator2 = collection1.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                if (!enumerator2.MoveNext())
                    return false;

                if (!enumerator1.Current.Equals(enumerator2.Current))
                    return false;
            }

            if (enumerator2.MoveNext())
                return false;

            return true;
        }
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
        public abstract bool IsAppearsAtMostOnce
        {
            get;
        }

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

                case IpV4OptionType.LooseSourceRouting:
                    return IpV4OptionLooseSourceRouting.Read(optionType, buffer, ref offset, offsetEnd - offset);
                case IpV4OptionType.StrictSourceRouting:
                    return IpV4OptionStrictSourceRouting.Read(optionType, buffer, ref offset, offsetEnd - offset);
                case IpV4OptionType.RecordRoute:
                    return IpV4OptionRecordRoute.Read(optionType, buffer, ref offset, offsetEnd - offset);
                case IpV4OptionType.StreamIdentifier:
                    return IpV4OptionStreamIdentifier.Read(optionType, buffer, ref offset, offsetEnd - offset);
                case IpV4OptionType.InternetTimestamp:
                    return IpV4OptionInternetTimestamp.Read(optionType, buffer, ref offset, offsetEnd - offset);
                default:
                    return null;
            }
        }

        private IpV4OptionType _type;
    }

    [Flags]
    public enum IpV4OptionInternetTimestampType : byte
    {
        TimestampOnly = 0,
        AddressAndTimestamp = 1,
        AddressPrespecified = 3
    }

    public class IpV4OptionInternetTimestamp : IpV4Option
    {
        private IpV4OptionInternetTimestamp(IpV4OptionType optionType, IpV4OptionInternetTimestampType timestampType, byte overflow, KeyValuePair<IpV4Address, DateTime>[] values, int pointedIndex)
            : base(optionType)
        {
            _timestampType = timestampType;
            _overflow = overflow;
            _values = values;
            _pointedIndex = pointedIndex;
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        internal static IpV4OptionInternetTimestamp Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 3)
                return null;

            byte optionLength = buffer[offset++];
            if (optionLength > length + 1 || optionLength % 4 != 0)
                return null;

            byte pointer = buffer[offset++];
            if (pointer % 4 != 1)
                return null;

            int pointedIndex = pointer / 4 - 1;
            if (pointedIndex < 0)
                return null;

            byte overflow = buffer[offset++];
            IpV4OptionInternetTimestampType type = (IpV4OptionInternetTimestampType)(overflow & 0x0F);
            overflow >>= 4;

            int numValues = optionLength / 4 - 1;

            if (type != IpV4OptionInternetTimestampType.TimestampOnly)
                numValues = numValues / 2 - numValues % 2;

            KeyValuePair<IpV4Address, DateTime>[] values = new KeyValuePair<IpV4Address, DateTime>[numValues];
            for (int i = 0; i < numValues; ++i)
            {
                switch (type)
                {
                    case IpV4OptionInternetTimestampType.TimestampOnly:
                        values[i] = new KeyValuePair<IpV4Address, DateTime>(IpV4Address.Zero, ReadTimeOfDay(buffer, ref offset));
                        break;
                    case IpV4OptionInternetTimestampType.AddressAndTimestamp:
                    case IpV4OptionInternetTimestampType.AddressPrespecified:
                        IpV4Address address = new IpV4Address(buffer.ReadUInt(ref offset, Endianity.Big));
                        values[i] = new KeyValuePair<IpV4Address, DateTime>(address, ReadTimeOfDay(buffer, ref offset));
                        break;
                }
            }

            return new IpV4OptionInternetTimestamp(optionType, type, overflow, values, pointedIndex);
        }

        private static DateTime ReadTimeOfDay(byte[] buffer, ref int offset)
        {
            return DateTime.MinValue.Date.AddMilliseconds(buffer.ReadUInt(ref offset, Endianity.Big));
        }

        private IpV4OptionInternetTimestampType _timestampType;
        private byte _overflow;
        private KeyValuePair<IpV4Address, DateTime>[] _values;
        private int _pointedIndex;
    }

    public class IpV4OptionStreamIdentifier : IpV4Option
    {
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        internal static IpV4OptionStreamIdentifier Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 3)
                return null;

            byte optionLength = buffer[offset++];
            if (optionLength != 4)
                return null;

            ushort identifier = buffer.ReadUShort(ref offset, Endianity.Big);
            return new IpV4OptionStreamIdentifier(optionType, identifier);
        }

        private IpV4OptionStreamIdentifier(IpV4OptionType optionType, ushort identifier)
            : base(optionType)
        {
            _identifier = identifier;
        }

        private ushort _identifier;
    }

    public class IpV4OptionRecordRoute : IpV4OptionRoute
    {
        internal static IpV4OptionRecordRoute Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            IpV4Address[] addresses;
            int pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, length, true))
                return null;
            return new IpV4OptionRecordRoute(optionType, addresses, pointedAddressIndex);
        }

        private IpV4OptionRecordRoute(IpV4OptionType optionType, IpV4Address[] addresses, int pointedAddressIndex)
            : base(optionType, addresses, pointedAddressIndex)
        {
        }
    }

    public abstract class IpV4OptionRoute : IpV4Option
    {
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        protected static bool TryRead(out IpV4Address[] addresses, out int pointedAddressIndex,
                                      byte[] buffer, ref int offset, int length, bool readUpToPointer)
        {
            addresses = null;
            pointedAddressIndex = 0;

            if (length < 2)
                return false;

            byte optionLength = buffer[offset++];
            if (optionLength > length + 1 || optionLength % 4 != 3)
                return false;

            byte pointer = buffer[offset++];
            if (pointer % 4 != 0)
                return false;

            pointedAddressIndex = pointer / 4 - 1;
            if (pointedAddressIndex < 0)
                return false;

            int numAddresses = readUpToPointer ? pointedAddressIndex : (optionLength - 3) / 4;
            addresses = new IpV4Address[numAddresses];
            for (int i = 0; i != numAddresses; ++i)
                addresses[i] = new IpV4Address(buffer.ReadUInt(ref offset, Endianity.Big));

            return true;
        }

        protected IpV4OptionRoute(IpV4OptionType optionType, IpV4Address[] addresses, int pointedAddressIndex)
            : base(optionType)
        {
            _addresses = addresses;
            _pointedAddressIndex = pointedAddressIndex;
        }

        private IpV4Address[] _addresses;
        private int _pointedAddressIndex;
    }

    public class IpV4OptionStrictSourceRouting : IpV4OptionRoute
    {
        internal static IpV4OptionStrictSourceRouting Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            IpV4Address[] addresses;
            int pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, length, false))
                return null;
            return new IpV4OptionStrictSourceRouting(optionType, addresses, pointedAddressIndex);
        }

        private IpV4OptionStrictSourceRouting(IpV4OptionType optionType, IpV4Address[] addresses, int pointedAddressIndex)
            : base(optionType, addresses, pointedAddressIndex)
        {
        }
    }

    public class IpV4OptionLooseSourceRouting : IpV4OptionRoute
    {
        internal static IpV4OptionLooseSourceRouting Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            IpV4Address[] addresses;
            int pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, length, false))
                return null;
            return new IpV4OptionLooseSourceRouting(optionType, addresses, pointedAddressIndex);
        }

        private IpV4OptionLooseSourceRouting(IpV4OptionType optionType, IpV4Address[] addresses, int pointedAddressIndex)
            : base(optionType, addresses, pointedAddressIndex)
        {
        }
    }

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
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        internal static IpV4OptionSecurity Read(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 10)
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
        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }

        internal IpV4OptionNoOperation(IpV4OptionType type)
            : base(type)
        {
        }
    }

    public class IpV4OptionEndOfOptionsList : IpV4Option
    {
        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }

        internal IpV4OptionEndOfOptionsList(IpV4OptionType type)
            : base(type)
        {
        }
    }
}
