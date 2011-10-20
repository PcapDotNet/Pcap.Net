﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dns
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class DnsTypeRegistrationAttribute : Attribute
    {
        public DnsType Type { get; set; }
    }

    public abstract class DnsResourceData : IEquatable<DnsResourceData>
    {
        public abstract bool Equals(DnsResourceData other);

        public sealed override bool Equals(object obj)
        {
 	        return Equals(obj as DnsResourceData);
        }

        internal abstract int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns);

        internal int Write(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int length = WriteData(buffer, dnsOffset, offsetInDns + sizeof(ushort), compressionData);
            buffer.Write(dnsOffset + offsetInDns, (ushort)length, Endianity.Big);
            length += sizeof(ushort);
            return length;
        }

        internal abstract int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData);

        internal static DnsResourceData Read(DnsDatagram dns, DnsType type, DnsClass dnsClass, int offsetInDns, int length)
        {
            DnsResourceData prototype = TryGetPrototype(type, dnsClass);
            if (prototype != null)
                return prototype.CreateInstance(dns, offsetInDns, length);
            return new DnsResourceDataAnything(dns.SubSegment(offsetInDns, length));
        }

        internal abstract DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length);

        internal static int GetStringLength(DataSegment str)
        {
            return sizeof(byte) + str.Length;
        }

        internal static DataSegment ReadString(DataSegment data, ref int offset)
        {
            if (data.Length <= offset)
                return null;
            int stringLength = data[offset++];
            if (data.Length < offset + stringLength)
                return null;
            DataSegment str = data.SubSegment(ref offset, stringLength);

            return str;
        }

        internal static void WriteString(byte[] buffer, ref int offset, DataSegment str)
        {
            buffer.Write(ref offset, (byte)str.Length);
            str.Write(buffer, ref offset);
        }
        
        private static DnsResourceData TryGetPrototype(DnsType type, DnsClass dnsClass)
        {
            DnsResourceData prototype;
            if (!_prototypes.TryGetValue(type, out prototype))
                return null;
            return prototype;
        }

        private static Dictionary<DnsType, DnsResourceData> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                from attribute in type.GetCustomAttributes<DnsTypeRegistrationAttribute>(false)
                where typeof(DnsResourceData).IsAssignableFrom(type)
                select new
                       {
                           attribute.Type,
                           Prototype = (DnsResourceData)Activator.CreateInstance(type),
                       };

            return prototypes.ToDictionary(prototype => prototype.Type, prototype => prototype.Prototype);
        }


        private static readonly Dictionary<DnsType, DnsResourceData> _prototypes = InitializePrototypes();
    }

    public abstract class DnsResourceDataNoCompression : DnsResourceData
    {
        internal sealed override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return GetLength();
        }

        internal sealed override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            return WriteData(buffer, dnsOffset + offsetInDns);
        }

        internal abstract int GetLength();
        internal abstract int WriteData(byte[] buffer, int offset);
    }

    public abstract class DnsResourceDataSimple : DnsResourceDataNoCompression
    {
        internal sealed override int WriteData(byte[] buffer, int offset)
        {
            WriteDataSimple(buffer, offset);
            return GetLength();
        }

        internal abstract void WriteDataSimple(byte[] buffer, int offset);

        internal sealed override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            return CreateInstance(dns.SubSegment(offsetInDns, length));
        }

        internal abstract DnsResourceData CreateInstance(DataSegment data);
    }

    [DnsTypeRegistration(Type = DnsType.A)]
    public sealed class DnsResourceDataIpV4 : DnsResourceDataSimple, IEquatable<DnsResourceDataIpV4>
    {
        public DnsResourceDataIpV4()
            : this(IpV4Address.Zero)
        {
        }

        public DnsResourceDataIpV4(IpV4Address data)
        {
            Data = data;
        }

        public IpV4Address Data { get; private set; }

        public bool Equals(DnsResourceDataIpV4 other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataIpV4);
        }

        internal override int GetLength()
        {
 	        return IpV4Address.SizeOf;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data, Endianity.Big);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length != IpV4Address.SizeOf)
                return null;
            return new DnsResourceDataIpV4(data.ReadIpV4Address(0, Endianity.Big));
        }
    }

    [DnsTypeRegistration(Type = DnsType.Ns)]
    [DnsTypeRegistration(Type = DnsType.Md)]
    [DnsTypeRegistration(Type = DnsType.Mf)]
    [DnsTypeRegistration(Type = DnsType.CName)]
    [DnsTypeRegistration(Type = DnsType.Mb)]
    [DnsTypeRegistration(Type = DnsType.Mg)]
    [DnsTypeRegistration(Type = DnsType.Mr)]
    [DnsTypeRegistration(Type = DnsType.Ptr)]
    [DnsTypeRegistration(Type = DnsType.NsapPtr)]
    public sealed class DnsResourceDataDomainName : DnsResourceData, IEquatable<DnsResourceDataDomainName>
    {
        public DnsResourceDataDomainName()
            : this(DnsDomainName.Root)
        {
        }

        public DnsResourceDataDomainName(DnsDomainName data)
        {
            Data = data;
        }

        public DnsDomainName Data { get; private set; }

        public bool Equals(DnsResourceDataDomainName other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataDomainName);
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Data.GetLength(compressionData, offsetInDns);
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            return Data.Write(buffer, dnsOffset, compressionData, offsetInDns);
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            int numBytesRead;
            DnsDomainName domainName;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out domainName, out numBytesRead))
                return null;
            length -= numBytesRead;

            if (length != 0)
                return null;

            return new DnsResourceDataDomainName(domainName);
        }
    }

    /// <summary>
    /// <pre>
    /// +-------+---------+
    /// | bit   | 0-31    |
    /// +-------+---------+
    /// | 0     | MNAME   |
    /// +-------+---------+
    /// | X     | RNAME   |
    /// +-------+---------+
    /// | Y     | SERIAL  |
    /// +-------+---------+
    /// | Y+32  | REFRESH |
    /// +-------+---------+
    /// | Y+64  | RETRY   |
    /// +-------+---------+
    /// | Y+96  | EXPIRE  |
    /// +-------+---------+
    /// | Y+128 | MINIMUM |
    /// +-------+---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Soa)]
    public sealed class DnsResourceDataStartOfAuthority : DnsResourceData, IEquatable<DnsResourceDataStartOfAuthority>
    {
        private static class Offset
        {
            public const int Serial = 0;
            public const int Refresh = Serial + sizeof(uint);
            public const int Retry = Refresh + sizeof(uint);
            public const int Expire = Retry + sizeof(uint);
            public const int MinimumTtl = Expire + sizeof(uint);
        }

        private const int ConstantPartLength = Offset.MinimumTtl + sizeof(uint);

        public DnsResourceDataStartOfAuthority()
            : this(DnsDomainName.Root, DnsDomainName.Root, 0, 0, 0, 0, 0)
        {
        }

        public DnsResourceDataStartOfAuthority(DnsDomainName mainNameServer, DnsDomainName responsibleMailBox,
                                               SerialNumber32 serial, uint refresh, uint retry, uint expire, uint minimumTtl)
        {
            MainNameServer = mainNameServer;
            ResponsibleMailBox = responsibleMailBox;
            Serial = serial;
            Refresh = refresh;
            Retry = retry;
            Expire = expire;
            MinimumTtl = minimumTtl;
        }

        /// <summary>
        /// The domain-name of the name server that was the original or primary source of data for this zone.
        /// </summary>
        public DnsDomainName MainNameServer { get; private set; }

        /// <summary>
        /// A domain-name which specifies the mailbox of the person responsible for this zone.
        /// </summary>
        public DnsDomainName ResponsibleMailBox { get; private set; }

        /// <summary>
        /// The unsigned 32 bit version number of the original copy of the zone.
        /// Zone transfers preserve this value.
        /// This value wraps and should be compared using sequence space arithmetic.
        /// </summary>
        public SerialNumber32 Serial { get; private set; }

        /// <summary>
        /// A 32 bit time interval before the zone should be refreshed.
        /// </summary>
        public uint Refresh { get; private set; }

        /// <summary>
        /// A 32 bit time interval that should elapse before a failed refresh should be retried.
        /// </summary>
        public uint Retry { get; private set; }

        /// <summary>
        /// A 32 bit time value that specifies the upper limit on the time interval that can elapse before the zone is no longer authoritative.
        /// </summary>
        public uint Expire { get; private set; }

        /// <summary>
        /// The unsigned 32 bit minimum TTL field that should be exported with any RR from this zone.
        /// </summary>
        public uint MinimumTtl { get; private set; }

        public bool Equals(DnsResourceDataStartOfAuthority other)
        {
            return other != null &&
                   MainNameServer.Equals(other.MainNameServer) &&
                   ResponsibleMailBox.Equals(other.ResponsibleMailBox) &&
                   Serial.Equals(other.Serial) &&
                   Refresh.Equals(other.Refresh) &&
                   Retry.Equals(other.Retry) &&
                   Expire.Equals(other.Expire) &&
                   MinimumTtl.Equals(other.MinimumTtl);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataStartOfAuthority);
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return MainNameServer.GetLength(compressionData, offsetInDns) +
                   ResponsibleMailBox.GetLength(compressionData, offsetInDns) +
                   ConstantPartLength;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = MainNameServer.Write(buffer, dnsOffset, compressionData, offsetInDns);
            numBytesWritten += ResponsibleMailBox.Write(buffer, dnsOffset, compressionData, offsetInDns + numBytesWritten);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Serial, Serial.Value, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Refresh, Refresh, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Retry, Retry, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Expire, Expire, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.MinimumTtl, MinimumTtl, Endianity.Big);

            return numBytesWritten + ConstantPartLength;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName mainNameServer;
            int domainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out mainNameServer, out domainNameLength))
                return null;
            offsetInDns += domainNameLength;
            length -= domainNameLength;

            DnsDomainName responsibleMailBox;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out responsibleMailBox, out domainNameLength))
                return null;
            offsetInDns += domainNameLength;
            length -= domainNameLength;

            if (length != ConstantPartLength)
                return null;

            uint serial = dns.ReadUInt(offsetInDns + Offset.Serial, Endianity.Big); ;
            uint refresh = dns.ReadUInt(offsetInDns + Offset.Refresh, Endianity.Big); ;
            uint retry = dns.ReadUInt(offsetInDns + Offset.Retry, Endianity.Big); ;
            uint expire = dns.ReadUInt(offsetInDns + Offset.Expire, Endianity.Big); ;
            uint minimumTtl = dns.ReadUInt(offsetInDns + Offset.MinimumTtl, Endianity.Big); ;

            return new DnsResourceDataStartOfAuthority(mainNameServer, responsibleMailBox, serial, refresh, retry, expire, minimumTtl);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+----------+---------+
    /// | bit | 0-7      | 8-31    |
    /// +-----+----------+---------+
    /// | 0   | Address            |
    /// +-----+----------+---------+
    /// | 32  | Protocol | Bit Map | (Bit Map is variable multiple of 8 bits length)
    /// +-----+----------+---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Wks)]
    public sealed class DnsResourceDataWellKnownService : DnsResourceDataSimple, IEquatable<DnsResourceDataWellKnownService>
    {
        private static class Offset
        {
            public const int Address = 0;
            public const int Protocol = Address + IpV4Address.SizeOf;
            public const int BitMap = Protocol + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.BitMap;

        public DnsResourceDataWellKnownService()
            : this(IpV4Address.Zero, IpV4Protocol.Ip, DataSegment.Empty)
        {
        }

        public DnsResourceDataWellKnownService(IpV4Address address, IpV4Protocol protocol, DataSegment bitMap)
        {
            Address = address;
            Protocol = protocol;
            BitMap = bitMap;
        }

        /// <summary>
        /// The service address.
        /// </summary>
        public IpV4Address Address { get; private set; }

        /// <summary>
        /// Specifies an IP protocol number.
        /// </summary>
        public IpV4Protocol Protocol { get; private set; }

        /// <summary>
        /// Has one bit per port of the specified protocol.
        /// </summary>
        public DataSegment BitMap { get; private set; }

        public bool Equals(DnsResourceDataWellKnownService other)
        {
            return other != null &&
                   Address.Equals(other.Address) &&
                   Protocol.Equals(other.Protocol) &&
                   BitMap.Equals(other.BitMap);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataWellKnownService);
        }

        internal override int GetLength()
        {
            return ConstantPartLength + BitMap.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            buffer.Write(offset + Offset.Protocol, (byte)Protocol);
            BitMap.Write(buffer, offset + Offset.BitMap);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            IpV4Address address = data.ReadIpV4Address(Offset.Address, Endianity.Big);
            IpV4Protocol protocol = (IpV4Protocol)data[Offset.Protocol];
            DataSegment bitMap = data.SubSegment(Offset.BitMap, data.Length - Offset.BitMap);

            return new DnsResourceDataWellKnownService(address, protocol, bitMap);
        }
    }

    public abstract class DnsResourceDataStrings : DnsResourceDataSimple, IEquatable<DnsResourceDataStrings>
    {
        public bool Equals(DnsResourceDataStrings other)
        {
            return other != null &&
                   GetType() == other.GetType() &&
                   Strings.SequenceEqual(other.Strings);
        }

        public sealed override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataStrings);
        }

        internal DnsResourceDataStrings(ReadOnlyCollection<DataSegment> strings)
        {
            Strings = strings;
        }

        internal DnsResourceDataStrings(params DataSegment[] strings)
            : this(strings.AsReadOnly())
        {
        }

        internal ReadOnlyCollection<DataSegment> Strings { get; private set; }

        internal sealed override int GetLength()
        {
            return Strings.Sum(str => sizeof(byte) + str.Length);
        }

        internal sealed override void WriteDataSimple(byte[] buffer, int offset)
        {
            foreach (DataSegment str in Strings)
            {
                buffer.Write(ref offset, (byte)str.Length);
                str.Write(buffer, ref offset);
            }
        }

        internal static List<DataSegment> ReadStrings(DataSegment data, int numExpected = 0)
        {
            List<DataSegment> strings = new List<DataSegment>(numExpected);
            int offset = 0;
            while (offset != data.Length)
            {
                DataSegment str = ReadString(data, ref offset);
                if (str == null)
                    return null;
                strings.Add(str);
            }

            return strings;
        }
    }

    [DnsTypeRegistration(Type = DnsType.X25)]
    public sealed class DnsResourceDataString : DnsResourceDataSimple, IEquatable<DnsResourceDataString>
    {
        public DnsResourceDataString()
            : this(DataSegment.Empty)
        {
        }

        public DnsResourceDataString(DataSegment str)
        {
            String = str;
        }

        public DataSegment String { get; private set; }

        public bool Equals(DnsResourceDataString other)
        {
            return other != null &&
                   String.Equals(other.String);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataString);
        }

        internal override int GetLength()
        {
            return GetStringLength(String);
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            WriteString(buffer, ref offset, String);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            int offset = 0;
            DataSegment str = ReadString(data, ref offset);
            if (str == null)
                return null;
            return new DnsResourceDataString(str);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+
    /// | CPU |
    /// +-----+ 
    /// | OS  |
    /// +-----+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.HInfo)]
    public sealed class DnsResourceDataHostInformation : DnsResourceDataStrings
    {
        private const int NumStrings = 2;

        public DnsResourceDataHostInformation()
            : this(DataSegment.Empty, DataSegment.Empty)
        {
        }

        public DnsResourceDataHostInformation(DataSegment cpu, DataSegment os)
            : base(cpu, os)
        {
        }

        public DataSegment Cpu { get { return Strings[0]; } }

        public DataSegment Os { get { return Strings[1]; } }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data, NumStrings);
            if (strings == null || strings.Count != NumStrings)
                return null;

            return new DnsResourceDataHostInformation(strings[0], strings[1]);
        }
    }

    public abstract class DnsResourceDataDomainNames : DnsResourceData, IEquatable<DnsResourceDataDomainNames>
    {
        public bool Equals(DnsResourceDataDomainNames other)
        {
            return other != null &&
                   GetType() == other.GetType() &&
                   DomainNames.SequenceEqual(other.DomainNames);
        }

        public sealed override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataDomainNames);
        }

        internal DnsResourceDataDomainNames(ReadOnlyCollection<DnsDomainName> domainNames)
        {
            DomainNames = domainNames;
        }

        internal DnsResourceDataDomainNames(params DnsDomainName[] domainNames)
            : this(domainNames.AsReadOnly())
        {
        }

        internal ReadOnlyCollection<DnsDomainName> DomainNames { get; private set; }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int totalLength = 0;
            foreach (DnsDomainName domainName in DomainNames)
            {
                int length = domainName.GetLength(compressionData, offsetInDns);
                offsetInDns += length;
                totalLength += length;
            }

            return totalLength;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = 0;
            foreach (DnsDomainName domainName in DomainNames)
                numBytesWritten += domainName.Write(buffer, dnsOffset, compressionData, offsetInDns + numBytesWritten);
            return numBytesWritten;
        }

        internal static List<DnsDomainName> ReadDomainNames(DnsDatagram dns, int offsetInDns, int length, int numExpected = 0)
        {
            List<DnsDomainName> domainNames = new List<DnsDomainName>(numExpected);
            while (length != 0)
            {
                DnsDomainName domainName;
                int domainNameLength;
                if (!DnsDomainName.TryParse(dns, offsetInDns, length, out domainName, out domainNameLength))
                    return null;
                offsetInDns += domainNameLength;
                length -= domainNameLength;
                domainNames.Add(domainName);
            }

            return domainNames;
        }
    }

    public abstract class DnsResourceData2DomainNames : DnsResourceDataDomainNames
    {
        private const int NumDomains = 2;

        public DnsResourceData2DomainNames(DnsDomainName first, DnsDomainName second)
            : base(first, second)
        {
        }

        internal DnsDomainName First { get { return DomainNames[0]; } }

        internal DnsDomainName Second { get { return DomainNames[1]; } }

        internal static bool TryRead(out DnsDomainName first, out DnsDomainName second,
                                     DnsDatagram dns, int offsetInDns, int length)
        {
            List<DnsDomainName> domainNames = ReadDomainNames(dns, offsetInDns, length, NumDomains);
            if (domainNames == null || domainNames.Count != NumDomains)
            {
                first = null;
                second = null;
                return false;
            }

            first = domainNames[0];
            second = domainNames[1];
            return true;
        }
    }

    /// <summary>
    /// <pre>
    /// +---------+
    /// | RMAILBX |
    /// +---------+
    /// | EMAILBX |
    /// +---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.MInfo)]
    public sealed class DnsResourceDataMailingListInfo : DnsResourceData2DomainNames
    {
        public DnsResourceDataMailingListInfo()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        public DnsResourceDataMailingListInfo(DnsDomainName mailingList, DnsDomainName errorMailBox)
            : base(mailingList, errorMailBox)
        {
        }

        /// <summary>
        /// Specifies a mailbox which is responsible for the mailing list or mailbox.
        /// If this domain name names the root, the owner of the MINFO RR is responsible for itself.
        /// Note that many existing mailing lists use a mailbox X-request for the RMAILBX field of mailing list X, e.g., Msgroup-request for Msgroup.
        /// This field provides a more general mechanism.
        /// </summary>
        public DnsDomainName MailingList { get { return First; } }

        /// <summary>
        /// Specifies a mailbox which is to receive error messages related to the mailing list or mailbox specified by the owner of the MINFO RR
        /// (similar to the ERRORS-TO: field which has been proposed).
        /// If this domain name names the root, errors should be returned to the sender of the message.
        /// </summary>
        public DnsDomainName ErrorMailBox { get { return Second; } }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName mailingList;
            DnsDomainName errorMailBox;
            if (!TryRead(out mailingList, out errorMailBox, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataMailingListInfo(mailingList, errorMailBox);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+--------+
    /// | bit | 0-15   |
    /// +-----+--------+
    /// | 0   | Value  |
    /// +-----+--------+
    /// | 16  | Domain |
    /// +-----+--------+
    /// </pre>
    /// </summary>
    public abstract class DnsResourceDataUShortDomainName : DnsResourceData, IEquatable<DnsResourceDataUShortDomainName>
    {
        private static class Offset
        {
            public const int Value = 0;
            public const int DomainName = Value + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.DomainName;

        public bool Equals(DnsResourceDataUShortDomainName other)
        {
            return other != null &&
                   GetType().Equals(other.GetType()) &&
                   Value.Equals(other.Value) &&
                   DomainName.Equals(other.DomainName);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataUShortDomainName);
        }

        internal DnsResourceDataUShortDomainName(ushort value, DnsDomainName domainName)
        {
            Value = value;
            DomainName = domainName;
        }

        internal ushort Value { get; private set; }

        internal DnsDomainName DomainName { get; private set; }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return ConstantPartLength +
                   DomainName.GetLength(compressionData, offsetInDns);
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            buffer.Write(dnsOffset + offsetInDns + Offset.Value, Value, Endianity.Big);
            int numBytesWritten = DomainName.Write(buffer, dnsOffset, compressionData, offsetInDns + Offset.DomainName);

            return ConstantPartLength + numBytesWritten;
        }

        internal static bool TryRead(out ushort value, out DnsDomainName domainName, 
                                     DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
            {
                value = 0;
                domainName = null;
                return false;
            }
            value = dns.ReadUShort(offsetInDns + Offset.Value, Endianity.Big);
            length -= ConstantPartLength;

            int domainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns + Offset.DomainName, length, out domainName, out domainNameLength))
                return false;
            length -= domainNameLength;

            if (length != 0)
                return false;

            return true;
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+------------+
    /// | bit | 0-15       |
    /// +-----+------------+
    /// | 0   | PREFERENCE |
    /// +-----+------------+
    /// | 16  | EXCHANGE   |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Mx)]
    public sealed class DnsResourceDataMailExchange : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataMailExchange()
            : this(0, DnsDomainName.Root)
        {
        }

        public DnsResourceDataMailExchange(ushort preference, DnsDomainName mailExchangeHost)
            : base(preference, mailExchangeHost)
        {
        }

        public ushort Preference { get { return Value; } }

        public DnsDomainName MailExchangeHost { get { return DomainName; } }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort preference;
            DnsDomainName mailExchangeHost;
            if (!TryRead(out preference, out mailExchangeHost, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataMailExchange(preference, mailExchangeHost);
        }
    }

    /// <summary>
    /// <pre>
    /// +---------+
    /// | Strings |
    /// +---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Txt)]
    public sealed class DnsResourceDataText : DnsResourceDataStrings
    {
        public DnsResourceDataText()
        {
        }

        public DnsResourceDataText(ReadOnlyCollection<DataSegment> strings)
            : base(strings)
        {
        }

        public ReadOnlyCollection<DataSegment> Text { get { return Strings; } }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data);
            return new DnsResourceDataText(strings.AsReadOnly());
        }
    }

    /// <summary>
    /// <pre>
    /// +------------+
    /// | mbox-dname |
    /// +------------+
    /// | txt-dname  |
    /// +------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Rp)]
    public sealed class DnsResourceDataResponsiblePerson : DnsResourceData2DomainNames
    {
        public DnsResourceDataResponsiblePerson()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        public DnsResourceDataResponsiblePerson(DnsDomainName mailBox, DnsDomainName textDomain)
            : base(mailBox, textDomain)
        {
        }

        /// <summary>
        /// A domain name that specifies the mailbox for the responsible person.
        /// Its format in master files uses the DNS convention for mailbox encoding, identical to that used for the RNAME mailbox field in the SOA RR.
        /// The root domain name (just ".") may be specified for MailBox to indicate that no mailbox is available.
        /// </summary>
        public DnsDomainName MailBox { get { return First; } }

        /// <summary>
        /// A domain name for which TXT RR's exist. 
        /// A subsequent query can be performed to retrieve the associated TXT resource records at TextDomain.
        /// This provides a level of indirection so that the entity can be referred to from multiple places in the DNS.
        /// The root domain name (just ".") may be specified for TextDomain to indicate that the TXT_DNAME is absent, and no associated TXT RR exists.
        /// </summary>
        public DnsDomainName TextDomain { get { return Second; } }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName mailBox;
            DnsDomainName textDomain;
            if (!TryRead(out mailBox, out textDomain, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataResponsiblePerson(mailBox, textDomain);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+----------+
    /// | bit | 0-15     |
    /// +-----+----------+
    /// | 0   | subtype  |
    /// +-----+----------+
    /// | 16  | hostname |
    /// +-----+----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.AfsDb)]
    public sealed class DnsResourceDataAfsDb : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataAfsDb()
            : this(0, DnsDomainName.Root)
        {
        }

        public DnsResourceDataAfsDb(ushort subType, DnsDomainName hostname)
            : base(subType, hostname)
        {
        }

        public ushort SubType { get { return Value; } }

        public DnsDomainName Hostname { get { return DomainName; } }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort subType;
            DnsDomainName hostName;
            if (!TryRead(out subType, out hostName, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataAfsDb(subType, hostName);
        }
    }

    /// <summary>
    /// <pre>
    /// +---------------+
    /// | ISDN-address  |
    /// +---------------+
    /// | sa (optional) |
    /// +---------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Isdn)]
    public sealed class DnsResourceDataIsdn : DnsResourceDataStrings
    {
        private const int MinNumStrings = 1;
        private const int MaxNumStrings = 2;

        public DnsResourceDataIsdn()
            : this(DataSegment.Empty)
        {
        }
        
        public DnsResourceDataIsdn(DataSegment isdnAddress)
            : base(isdnAddress)
        {
            
        }
            
        public DnsResourceDataIsdn(DataSegment isdnAddress, DataSegment subAddress)
            : base(isdnAddress, subAddress)
        {
        }

        /// <summary>
        /// Identifies the ISDN number of the owner and DDI (Direct Dial In) if any, as defined by E.164 and E.163, 
        /// the ISDN and PSTN (Public Switched Telephone Network) numbering plan.
        /// E.163 defines the country codes, and E.164 the form of the addresses.
        /// </summary>
        public DataSegment IsdnAddress { get { return Strings[0]; } }

        /// <summary>
        /// Specifies the subaddress (SA).
        /// </summary>
        public DataSegment SubAddress { get { return Strings.Count == MaxNumStrings ? Strings[1] : null; } }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data, MaxNumStrings);
            if (strings == null)
                return null;
            if (strings.Count == MinNumStrings)
                return new DnsResourceDataIsdn(strings[0]);
            if (strings.Count == MaxNumStrings)
                return new DnsResourceDataIsdn(strings[0], strings[1]);
            return null;
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+-------------------+
    /// | bit | 0-15              |
    /// +-----+-------------------+
    /// | 0   | preference        |
    /// +-----+-------------------+
    /// | 16  | intermediate-host |
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Rt)]
    public sealed class DnsResourceDataRouteThrough : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataRouteThrough()
            : this(0, DnsDomainName.Root)
        {
        }

        public DnsResourceDataRouteThrough(ushort preference, DnsDomainName intermediateHost)
            : base(preference, intermediateHost)
        {
        }

        /// <summary>
        /// Representing the preference of the route.
        /// Smaller numbers indicate more preferred routes.
        /// </summary>
        public ushort Preference { get { return Value; } }

        /// <summary>
        /// The domain name of a host which will serve as an intermediate in reaching the host specified by the owner.
        /// The DNS RRs associated with IntermediateHost are expected to include at least one A, X25, or ISDN record.
        /// </summary>
        public DnsDomainName IntermediateHost { get { return DomainName; } }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort preference;
            DnsDomainName intermediateHost;
            if (!TryRead(out preference, out intermediateHost, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataRouteThrough(preference, intermediateHost);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+-----+----------------------+----------+-----------+
    /// | bit | 0-7 | 8-7+X                | 8+X-55+X | 56+X-63+X |
    /// +-----+-----+----------------------+----------+-----------+
    /// | 0   | AFI | Domain Specific Area | ID       | Sel       |
    /// +-----+-----+-----+----------------+----------+-----------+
    /// | 0   | AFI | IDI | HO-DSP         | ID       | Sel       |
    /// +-----+-----+-----+----------------+----------+-----------+
    /// | 0   | Area Address               | ID       | Sel       |
    /// +-----+-----------+----------------+----------+-----------+
    /// | 0   | IDP       | DSP                                   |
    /// +-----+-----------+---------------------------------------+
    /// </pre>
    /// IDP is Initial Domain Part.
    /// DSP is Domain Specific Part.
    /// HO-DSP may use any format as defined by the authority identified by IDP.
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Nsap)]
    public sealed class DnsResourceDataNetworkServiceAccessPoint : DnsResourceDataSimple, IEquatable<DnsResourceDataNetworkServiceAccessPoint>
    {
        private static class Offset
        {
            public const int AreaAddress = 0;
        }

        private static class OffsetAfterArea
        {
            public const int SystemIdentifier = 0;
            public const int Selector = SystemIdentifier + UInt48.SizeOf;
        }

        private const int MinAreaAddressLength = sizeof(byte);

        private const int ConstantPartLength = MinAreaAddressLength + OffsetAfterArea.Selector + sizeof(byte);

        public DnsResourceDataNetworkServiceAccessPoint()
            : this(new DataSegment(new byte[MinAreaAddressLength]), 0, 0)
        {
        }

        public DnsResourceDataNetworkServiceAccessPoint(DataSegment areaAddress, UInt48 systemIdentifier, byte selector)
        {
            if (areaAddress.Length < MinAreaAddressLength)
                throw new ArgumentOutOfRangeException("areaAddress", areaAddress.Length,
                                                      string.Format("Area Address length must be at least {0}.", MinAreaAddressLength));
            AreaAddress = areaAddress;
            SystemIdentifier = systemIdentifier;
            Selector = selector;
        }

        /// <summary>
        /// Authority and Format Identifier.
        /// </summary>
        public byte AuthorityAndFormatIdentifier { get { return AreaAddress[0]; } }

        /// <summary>
        /// The combination of [IDP, HO-DSP] identify both the routing domain and the area within the routing domain.
        /// Hence the combination [IDP, HO-DSP] is called the "Area Address".
        /// All nodes within the area must have same Area address.
        /// </summary>
        public DataSegment AreaAddress { get; private set; }

        /// <summary>
        /// System Identifier.
        /// </summary>
        public UInt48 SystemIdentifier { get; private set; }

        /// <summary>
        /// NSAP Selector
        /// </summary>
        public byte Selector { get; private set; }

        public bool Equals(DnsResourceDataNetworkServiceAccessPoint other)
        {
            return other != null &&
                   AreaAddress.Equals(other.AreaAddress) &&
                   SystemIdentifier.Equals(other.SystemIdentifier) &&
                   Selector.Equals(other.Selector);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNetworkServiceAccessPoint);
        }

        internal override int GetLength()
        {
            return ConstantPartLength + AreaAddress.Length - MinAreaAddressLength;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            AreaAddress.Write(buffer, offset + Offset.AreaAddress);

            int afterAreaOffset = offset + AreaAddress.Length;
            buffer.Write(afterAreaOffset + OffsetAfterArea.SystemIdentifier, SystemIdentifier, Endianity.Big);
            buffer.Write(afterAreaOffset + OffsetAfterArea.Selector, Selector);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DataSegment areaAddress = data.SubSegment(Offset.AreaAddress, MinAreaAddressLength + data.Length - ConstantPartLength);

            int afterAreaOffset = areaAddress.Length;
            UInt48 systemIdentifier = data.ReadUInt48(afterAreaOffset + OffsetAfterArea.SystemIdentifier, Endianity.Big);
            byte selector = data[afterAreaOffset + OffsetAfterArea.Selector];

            return new DnsResourceDataNetworkServiceAccessPoint(areaAddress, systemIdentifier, selector);
        }
    }

    /// <summary>
    /// RFC 2535.
    /// The key algorithm.
    /// </summary>
    public enum DnsAlgorithm
    {
        /// <summary>
        /// RFC 4034.
        /// Field is not used.
        /// </summary>
        None = 0,

        /// <summary>
        /// RFCs 2537, 4034.
        /// RSA/MD5.
        /// Deprecated.
        /// </summary>
        RsaMd5 = 1,

        /// <summary>
        /// RFC 2539.
        /// Diffie-Hellman.
        /// Implementation is optional, key only.
        /// </summary>
        DiffieHellman = 2,

        /// <summary>
        /// RFCs 2536, 3755.
        /// DSA.
        /// Implementation is mandatory.
        /// </summary>
        Dsa = 3,

        /// <summary>
        /// Reserved for elliptic curve crypto.
        /// </summary>
        Ecc = 4,

        /// <summary>
        /// RFCs 3110, 3755.
        /// RSA/SHA-1.
        /// </summary>
        RsaSha1 = 5,

        /// <summary>
        /// RFC 5155.
        /// DSA-NSEC3-SHA1.
        /// </summary>
        DsaNsec3Sha1 = 6,

        /// <summary>
        /// RFC 5155.
        /// RSASHA1-NSEC3-SHA1.
        /// </summary>
        RsaSha1Nsec3Sha1 = 7,

        /// <summary>
        /// RFC 5702.
        /// RSA/SHA-256.
        /// </summary>
        RsaSha256 = 8,

        /// <summary>
        /// RFC 5702.
        /// RSA/SHA-512.
        /// </summary>
        RsaSha512 = 10,

        /// <summary>
        /// RFC 5933.
        /// GOST R 34.10-2001.
        /// </summary>
        EccGost = 12,

        /// <summary>
        /// RFC 4034.
        /// Reserved for Indirect Keys.
        /// </summary>
        Indirect = 252,

        /// <summary>
        /// RFCs 2535, 3755.
        /// Private algorithms - domain name.
        /// </summary>
        PrivateDns = 253,

        /// <summary>
        /// RFCs 2535, 3755.
        /// Private algorithms - OID.
        /// </summary>
        PrivateOid = 254,
    }

    /// <summary>
    /// <pre>
    /// +-----+--------------+-----------+--------+
    /// | bit | 0-15         | 16-23     | 24-31  |
    /// +-----+--------------+-----------+--------+
    /// | 0   | type covered | algorithm | labels |
    /// +-----+--------------+-----------+--------+
    /// | 32  | original TTL                      |
    /// +-----+-----------------------------------+
    /// | 64  | signature expiration              |
    /// +-----+-----------------------------------+
    /// | 96  | signature inception               |
    /// +-----+--------------+--------------------+
    /// | 128 | key tag      |                    |
    /// +-----+--------------+ signer's name      |
    /// |     |                                   |
    /// +-----+-----------------------------------+
    /// |     | signature                         |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Sig)]
    public sealed class DnsResourceDataSig : DnsResourceData
    {
        private static class Offset
        {
            public const int TypeCovered = 0;
            public const int Algorithm = TypeCovered + sizeof(ushort);
            public const int Labels = Algorithm + sizeof(byte);
            public const int OriginalTtl = Labels + sizeof(byte);
            public const int SignatureExpiration = OriginalTtl + sizeof(uint);
            public const int SignatureInception= SignatureExpiration + sizeof(uint);
            public const int KeyTag = SignatureInception + sizeof(uint);
            public const int SignersName = KeyTag + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.SignersName;

        public DnsResourceDataSig()
            : this(DnsType.A, DnsAlgorithm.None, 0, 0, 0, 0, 0, DnsDomainName.Root, DataSegment.Empty)
        {
        }

        public DnsResourceDataSig(DnsType typeCovered, DnsAlgorithm algorithm, byte labels, uint originalTtl, SerialNumber32 signatureExpiration,
                                  SerialNumber32 signatureInception, ushort keyTag, DnsDomainName signersName, DataSegment signature)
        {
            TypeCovered = typeCovered;
            Algorithm = algorithm;
            Labels = labels;
            OriginalTtl = originalTtl;
            SignatureExpiration = signatureExpiration;
            SignatureInception = signatureInception;
            KeyTag = keyTag;
            SignersName = signersName;
            Signature = signature;
        }

        /// <summary>
        /// The type of the other RRs covered by this SIG.
        /// </summary>
        public DnsType TypeCovered { get; private set; }

        /// <summary>
        /// The key algorithm.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// An unsigned count of how many labels there are in the original SIG RR owner name not counting the null label for root and not counting any initial "*" for a wildcard.  
        /// If a secured retrieval is the result of wild card substitution, it is necessary for the resolver to use the original form of the name in verifying the digital signature.
        /// This field makes it easy to determine the original form.
        /// 
        /// If, on retrieval, the RR appears to have a longer name than indicated by "labels", the resolver can tell it is the result of wildcard substitution.
        /// If the RR owner name appears to be shorter than the labels count, the SIG RR must be considered corrupt and ignored.
        /// The maximum number of labels allowed in the current DNS is 127 but the entire octet is reserved and would be required should DNS names ever be expanded to 255 labels.
        /// </summary>
        public byte Labels { get; private set; }

        /// <summary>
        /// The "original TTL" field is included in the RDATA portion to avoid
        /// (1) authentication problems that caching servers would otherwise cause by decrementing the real TTL field and
        /// (2) security problems that unscrupulous servers could otherwise cause by manipulating the real TTL field.
        /// This original TTL is protected by the signature while the current TTL field is not.
        /// 
        /// NOTE:  The "original TTL" must be restored into the covered RRs when the signature is verified.
        /// This generaly implies that all RRs for a particular type, name, and class, that is, all the RRs in any particular RRset, must have the same TTL to start with.
        ///  </summary>
        public uint OriginalTtl { get; private set; }

        /// <summary>
        /// The last time the signature is valid.
        /// Numbers of seconds since the start of 1 January 1970, GMT, ignoring leap seconds.
        /// Ring arithmetic is used.
        /// This time can never be more than about 68 years after the inception.
        /// </summary>
        public SerialNumber32 SignatureExpiration { get; private set; }

        /// <summary>
        /// The first time the signature is valid.
        /// Numbers of seconds since the start of 1 January 1970, GMT, ignoring leap seconds.
        /// Ring arithmetic is used.
        /// This time can never be more than about 68 years before the expiration.
        /// </summary>
        public SerialNumber32 SignatureInception { get; private set; }

        /// <summary>
        /// Used to efficiently select between multiple keys which may be applicable and thus check that a public key about to be used for the computationally expensive effort to check the signature is possibly valid.  
        /// For algorithm 1 (MD5/RSA) as defined in RFC 2537, it is the next to the bottom two octets of the public key modulus needed to decode the signature field.
        /// That is to say, the most significant 16 of the least significant 24 bits of the modulus in network (big endian) order. 
        /// For all other algorithms, including private algorithms, it is calculated as a simple checksum of the KEY RR.
        /// </summary>
        public ushort KeyTag { get; private set; }

        /// <summary>
        /// The domain name of the signer generating the SIG RR.
        /// This is the owner name of the public KEY RR that can be used to verify the signature.  
        /// It is frequently the zone which contained the RRset being authenticated.
        /// Which signers should be authorized to sign what is a significant resolver policy question.
        /// The signer's name may be compressed with standard DNS name compression when being transmitted over the network.
        /// </summary>
        public DnsDomainName SignersName { get; private set; }

        /// <summary>
        /// The actual signature portion of the SIG RR binds the other RDATA fields to the RRset of the "type covered" RRs with that owner name and class.
        /// This covered RRset is thereby authenticated. 
        /// To accomplish this, a data sequence is constructed as follows: 
        /// 
        /// data = RDATA | RR(s)...
        /// 
        /// where "|" is concatenation,
        /// 
        /// RDATA is the wire format of all the RDATA fields in the SIG RR itself (including the canonical form of the signer's name) before but not including the signature,
        /// and RR(s) is the RRset of the RR(s) of the type covered with the same owner name and class as the SIG RR in canonical form and order.
        /// 
        /// How this data sequence is processed into the signature is algorithm dependent.
        /// </summary>
        public DataSegment Signature { get; private set; }

        public bool Equals(DnsResourceDataSig other)
        {
            return other != null &&
                   TypeCovered.Equals(other.TypeCovered) &&
                   Algorithm.Equals(other.Algorithm) &&
                   Labels.Equals(other.Labels) &&
                   OriginalTtl.Equals(other.OriginalTtl) &&
                   SignatureExpiration.Equals(other.SignatureExpiration) &&
                   SignatureInception.Equals(other.SignatureInception) &&
                   KeyTag.Equals(other.KeyTag) &&
                   SignersName.Equals(other.SignersName) &&
                   Signature.Equals(other.Signature);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataSig);
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return ConstantPartLength + SignersName.GetLength(compressionData, offsetInDns) + Signature.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            buffer.Write(dnsOffset + offsetInDns + Offset.TypeCovered, (ushort)TypeCovered, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.Algorithm, (byte)Algorithm);
            buffer.Write(dnsOffset + offsetInDns + Offset.Labels, Labels);
            buffer.Write(dnsOffset + offsetInDns + Offset.OriginalTtl, OriginalTtl, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.SignatureExpiration, SignatureExpiration.Value, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.SignatureInception, SignatureInception.Value, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.KeyTag, KeyTag, Endianity.Big);

            int numBytesWritten = ConstantPartLength;
            numBytesWritten += SignersName.Write(buffer, dnsOffset, compressionData, offsetInDns + numBytesWritten);

            Signature.Write(buffer, dnsOffset + offsetInDns + numBytesWritten);
            return numBytesWritten + Signature.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            DnsType typeCovered = (DnsType)dns.ReadUShort(offsetInDns + Offset.TypeCovered, Endianity.Big);
            DnsAlgorithm algorithm = (DnsAlgorithm)dns[offsetInDns + Offset.Algorithm];
            byte labels = dns[offsetInDns + Offset.Labels];
            uint originalTtl = dns.ReadUInt(offsetInDns + Offset.OriginalTtl, Endianity.Big);
            uint signatureExpiration = dns.ReadUInt(offsetInDns + Offset.SignatureExpiration, Endianity.Big);
            uint signatureInception = dns.ReadUInt(offsetInDns + Offset.SignatureInception, Endianity.Big);
            ushort keyTag = dns.ReadUShort(offsetInDns + Offset.KeyTag, Endianity.Big);

            offsetInDns += ConstantPartLength;
            length -= ConstantPartLength;

            DnsDomainName signersName;
            int signersNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out signersName, out signersNameLength))
                return null;
            offsetInDns += signersNameLength;
            length -= signersNameLength;

            DataSegment signature = dns.SubSegment(offsetInDns, length);

            return new DnsResourceDataSig(typeCovered, algorithm, labels, originalTtl, signatureExpiration, signatureInception, keyTag, signersName, signature);
        }
    }
}
