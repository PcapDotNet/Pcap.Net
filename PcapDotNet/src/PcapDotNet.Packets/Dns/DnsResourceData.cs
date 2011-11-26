using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

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
                           Prototype = (DnsResourceData)Activator.CreateInstance(type, true),
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

    /// <summary>
    /// <pre>
    /// +-----+------+
    /// | bit | 0-31 |
    /// +-----+------+
    /// | 0   | IP   |
    /// +-----+------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.A)]
    public sealed class DnsResourceDataIpV4 : DnsResourceDataSimple, IEquatable<DnsResourceDataIpV4>
    {
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

        internal DnsResourceDataIpV4()
            : this(IpV4Address.Zero)
        {
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

    /// <summary>
    /// <pre>
    /// +------+
    /// | NAME |
    /// +------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Ns)]
    [DnsTypeRegistration(Type = DnsType.Md)]
    [DnsTypeRegistration(Type = DnsType.Mf)]
    [DnsTypeRegistration(Type = DnsType.CName)]
    [DnsTypeRegistration(Type = DnsType.Mb)]
    [DnsTypeRegistration(Type = DnsType.Mg)]
    [DnsTypeRegistration(Type = DnsType.Mr)]
    [DnsTypeRegistration(Type = DnsType.Ptr)]
    [DnsTypeRegistration(Type = DnsType.NsapPtr)]
    [DnsTypeRegistration(Type = DnsType.DName)]
    public sealed class DnsResourceDataDomainName : DnsResourceData, IEquatable<DnsResourceDataDomainName>
    {
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

        internal DnsResourceDataDomainName()
            : this(DnsDomainName.Root)
        {
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
    /// | ...   |         |
    /// +-------+---------+
    /// | X     | RNAME   |
    /// | ...   |         |
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
            public const int Refresh = Serial + SerialNumber32.SizeOf;
            public const int Retry = Refresh + sizeof(uint);
            public const int Expire = Retry + sizeof(uint);
            public const int MinimumTtl = Expire + sizeof(uint);
        }

        private const int ConstantPartLength = Offset.MinimumTtl + sizeof(uint);

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

        internal DnsResourceDataStartOfAuthority()
            : this(DnsDomainName.Root, DnsDomainName.Root, 0, 0, 0, 0, 0)
        {
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

        internal DnsResourceDataWellKnownService()
            : this(IpV4Address.Zero, IpV4Protocol.Ip, DataSegment.Empty)
        {
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

        internal DnsResourceDataString()
            : this(DataSegment.Empty)
        {
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

        public DnsResourceDataHostInformation(DataSegment cpu, DataSegment os)
            : base(cpu, os)
        {
        }

        public DataSegment Cpu { get { return Strings[0]; } }

        public DataSegment Os { get { return Strings[1]; } }

        internal DnsResourceDataHostInformation()
            : this(DataSegment.Empty, DataSegment.Empty)
        {
        }

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

        internal DnsResourceData2DomainNames(DnsDomainName first, DnsDomainName second)
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

        internal DnsResourceDataMailingListInfo()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

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
    /// | ... |        |
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
    /// | ... |            |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Mx)]
    public sealed class DnsResourceDataMailExchange : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataMailExchange(ushort preference, DnsDomainName mailExchangeHost)
            : base(preference, mailExchangeHost)
        {
        }

        /// <summary>
        /// Specifies the preference given to this RR among others at the same owner.
        /// Lower values are preferred.
        /// </summary>
        public ushort Preference { get { return Value; } }

        /// <summary>
        /// Specifies a host willing to act as a mail exchange for the owner name.
        /// </summary>
        public DnsDomainName MailExchangeHost { get { return DomainName; } }

        internal DnsResourceDataMailExchange()
            : this(0, DnsDomainName.Root)
        {
        }

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
    [DnsTypeRegistration(Type = DnsType.Spf)]
    public sealed class DnsResourceDataText : DnsResourceDataStrings
    {
        public DnsResourceDataText(ReadOnlyCollection<DataSegment> strings)
            : base(strings)
        {
        }

        public ReadOnlyCollection<DataSegment> Text { get { return Strings; } }

        internal DnsResourceDataText()
        {
        }

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

        internal DnsResourceDataResponsiblePerson()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

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
    /// | ... |          |
    /// +-----+----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.AfsDb)]
    public sealed class DnsResourceDataAfsDb : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataAfsDb(ushort subType, DnsDomainName hostname)
            : base(subType, hostname)
        {
        }

        public ushort SubType { get { return Value; } }

        public DnsDomainName Hostname { get { return DomainName; } }

        internal DnsResourceDataAfsDb()
            : this(0, DnsDomainName.Root)
        {
        }

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

        internal DnsResourceDataIsdn()
            : this(DataSegment.Empty)
        {
        }

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
    /// | ... |                   |
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Rt)]
    public sealed class DnsResourceDataRouteThrough : DnsResourceDataUShortDomainName
    {
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

        internal DnsResourceDataRouteThrough()
            : this(0, DnsDomainName.Root)
        {
        }

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

        internal DnsResourceDataNetworkServiceAccessPoint()
            : this(new DataSegment(new byte[MinAreaAddressLength]), 0, 0)
        {
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
    /// RFCs 2535, 2536, 2537, 2539, 3110, 3755, 4034, 5155, 5702, 5933.
    /// The key algorithm.
    /// </summary>
    public enum DnsAlgorithm
    {
        /// <summary>
        /// RFC 4034.
        /// Field is not used or indicates that the algorithm is unknown to a secure DNS, 
        /// which may simply be the result of the algorithm not having been standardized for DNSSEC.
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
        /// DSA - Digital Signature Algorithm.
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
    /// RFCs 2535, 4034.
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
    /// | ... |                                   |
    /// +-----+-----------------------------------+
    /// |     | signature                         |
    /// | ... |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Sig)]
    [DnsTypeRegistration(Type = DnsType.RrSig)]
    public sealed class DnsResourceDataSig : DnsResourceData, IEquatable<DnsResourceDataSig>
    {
        private static class Offset
        {
            public const int TypeCovered = 0;
            public const int Algorithm = TypeCovered + sizeof(ushort);
            public const int Labels = Algorithm + sizeof(byte);
            public const int OriginalTtl = Labels + sizeof(byte);
            public const int SignatureExpiration = OriginalTtl + sizeof(uint);
            public const int SignatureInception = SignatureExpiration + SerialNumber32.SizeOf;
            public const int KeyTag = SignatureInception + SerialNumber32.SizeOf;
            public const int SignersName = KeyTag + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.SignersName;

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

        internal DnsResourceDataSig()
            : this(DnsType.A, DnsAlgorithm.None, 0, 0, 0, 0, 0, DnsDomainName.Root, DataSegment.Empty)
        {
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

    /// <summary>
    /// RFC 2535.
    /// </summary>
    public enum DnsKeyNameType : byte
    {
        /// <summary>
        /// Indicates that this is a key associated with a "user" or "account" at an end entity, usually a host.
        /// The coding of the owner name is that used for the responsible individual mailbox in the SOA and RP RRs:
        /// The owner name is the user name as the name of a node under the entity name.
        /// For example, "j_random_user" on host.subdomain.example could have a public key associated through a KEY RR with name j_random_user.host.subdomain.example.
        /// It could be used in a security protocol where authentication of a user was desired.
        /// This key might be useful in IP or other security for a user level service such a telnet, ftp, rlogin, etc.
        /// </summary>
        UserOrAccountAtEndEntity = 0,

        /// <summary>
        /// Indicates that this is a zone key for the zone whose name is the KEY RR owner name.
        /// This is the public key used for the primary DNS security feature of data origin authentication.
        /// Zone KEY RRs occur only at delegation points.
        /// </summary>
        ZoneKey = 1,

        /// <summary>
        /// Indicates that this is a key associated with the non-zone "entity" whose name is the RR owner name.
        /// This will commonly be a host but could, in some parts of the DNS tree, be some other type of entity such as a telephone number [RFC 1530] or numeric IP address.
        /// This is the public key used in connection with DNS request and transaction authentication services.
        /// It could also be used in an IP-security protocol where authentication at the host, rather than user, level was desired, such as routing, NTP, etc.
        /// </summary>
        NonZoneEntity = 2,
    }

    /// <summary>
    /// RFC 2137.
    /// </summary>
    [Flags]
    public enum DnsKeySignatory : byte
    {
        /// <summary>
        /// The general update signatory field bit has no special meaning.
        /// If the other three bits are all zero, it must be one so that the field is non-zero to designate that the key is an update key.
        /// The meaning of all values of the signatory field with the general bit and one or more other signatory field bits on is reserved.
        /// </summary>
        General = 1,

        /// <summary>
        /// If non-zero, this key is authorized to add and update RRs for only a single owner name.
        /// If there already exist RRs with one or more names signed by this key, they may be updated but no new name created until the number of existing names is reduced to zero.
        /// This bit is meaningful only for mode A dynamic zones and is ignored in mode B dynamic zones. 
        /// This bit is meaningful only if the owner name is a wildcard.
        /// (Any dynamic update KEY with a non-wildcard name is, in effect, a unique name update key.)
        /// </summary>
        Unique = 2,

        /// <summary>
        /// If non-zero, this key is authorized to add and delete RRs even if there are other RRs with the same owner name and class that are authenticated by a SIG signed with a different dynamic update KEY.
        /// If zero, the key can only authorize updates where any existing RRs of the same owner and class are authenticated by a SIG using the same key.
        /// This bit is meaningful only for type A dynamic zones and is ignored in type B dynamic zones.
        /// 
        /// Keeping this bit zero on multiple KEY RRs with the same or nested wild card owner names permits multiple entities to exist that can create and delete names but can not effect RRs with different owner names from any they created.
        /// In effect, this creates two levels of dynamic update key, strong and weak, where weak keys are limited in interfering with each other but a strong key can interfere with any weak keys or other strong keys.
        /// </summary>
        Strong = 4,

        /// <summary>
        /// If nonzero, this key is authorized to attach, detach, and move zones by creating and deleting NS, glue A, and zone KEY RR(s).
        /// If zero, the key can not authorize any update that would effect such RRs.
        /// This bit is meaningful for both type A and type B dynamic secure zones.
        /// NOTE:  do not confuse the "zone" signatory field bit with the "zone" key type bit.
        /// </summary>
        Zone = 8,
    }

    /// <summary>
    /// RFC 2535.
    /// </summary>
    public enum DnsKeyProtocol : byte
    {
        /// <summary>
        /// Connection with TLS.
        /// </summary>
        Tls = 1,

        /// <summary>
        /// Connection with email.
        /// </summary>
        Email = 2,

        /// <summary>
        /// DNS security.
        /// The protocol field should be set to this value for zone keys and other keys used in DNS security.
        /// Implementations that can determine that a key is a DNS security key by the fact that flags label it a zone key or the signatory flag field is non-zero are not required to check the protocol field.
        /// </summary>
        DnsSec = 3,

        /// <summary>
        /// Oakley/IPSEC [RFC 2401] protocol.
        /// Indicates that this key is valid for use in conjunction with that security standard.
        /// This key could be used in connection with secured communication on behalf of an end entity or user whose name is the owner name of the KEY RR if the entity or user flag bits are set.
        /// The presence of a KEY resource with this protocol value is an assertion that the host speaks Oakley/IPSEC.
        /// </summary>
        IpSec = 4,

        /// <summary>
        /// The key can be used in connection with any protocol for which KEY RR protocol octet values have been defined.
        /// The use of this value is discouraged and the use of different keys for different protocols is encouraged.
        /// </summary>
        All = 255,
    }

    /// <summary>
    /// <pre>
    /// +-----+---+---+----------+----+----------+--------+----------+-------+
    /// | bit | 0 | 1 | 2        | 3  | 4-5      | 6-7    | 8-11     | 12-15 |
    /// +-----+---+---+----------+----+----------+--------+----------+-------+
    /// | 0   | A | C | Reserved | XT | Reserved | NAMTYP | Reserved | SIG   |
    /// +-----+---+---+----------+----+----------+--------+----------+-------+
    /// | 16  | protocol                                  | algorithm        |
    /// +-----+-------------------------------------------+------------------+
    /// | 32  | Flags extension (optional)                                   |
    /// +-----+--------------------------------------------------------------+
    /// | 32  | public key                                                   |
    /// | or  |                                                              |
    /// | 48  |                                                              |
    /// | ... |                                                              |
    /// +-----+--------------------------------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Key)]
    public sealed class DnsResourceDataKey : DnsResourceDataSimple, IEquatable<DnsResourceDataKey>
    {
        private static class Offset
        {
            public const int AuthenticationProhibited = 0;
            public const int ConfidentialityProhibited = 0;
            public const int IsFlagsExtension = 0;
            public const int NameType = 0;
            public const int Signatory = 1;
            public const int Protocol = sizeof(ushort);
            public const int Algorithm = Protocol + sizeof(byte);
            public const int FlagsExtension = Algorithm + sizeof(byte);
        }

        private static class Mask
        {
            public const byte AuthenticationProhibited = 0x80;
            public const byte ConfidentialityProhibited = 0x40;
            public const byte IsFlagsExtension = 0x10;
            public const byte NameType = 0x03;
            public const byte Signatory = 0x0F;
        }

        private const int ConstantPartLength = Offset.FlagsExtension;

        public DnsResourceDataKey(bool authenticationProhibited, bool confidentialityProhibited, DnsKeyNameType nameType, DnsKeySignatory signatory,
                                  DnsKeyProtocol protocol, DnsAlgorithm algorithm, ushort? flagsExtension, DataSegment publicKey)
        {
            AuthenticationProhibited = authenticationProhibited;
            ConfidentialityProhibited = confidentialityProhibited;
            FlagsExtension = flagsExtension;
            NameType = nameType;
            Signatory = signatory;
            Protocol = protocol;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// Use of the key is prohibited for authentication.
        /// </summary>
        public bool AuthenticationProhibited { get; private set; }

        /// <summary>
        /// Use of the key is prohibited for confidentiality.
        /// </summary>
        public bool ConfidentialityProhibited { get; private set; }

        /// <summary>
        /// The name type.
        /// </summary>
        public DnsKeyNameType NameType { get; private set; }

        /// <summary>
        /// If non-zero, indicates that the key can validly sign things as specified in DNS dynamic update.
        /// Note that zone keys always have authority to sign any RRs in the zone regardless of the value of the signatory field.
        /// </summary>
        public DnsKeySignatory Signatory { get; private set; }

        /// <summary>
        /// It is anticipated that keys stored in DNS will be used in conjunction with a variety of Internet protocols.
        /// It is intended that the protocol octet and possibly some of the currently unused (must be zero) bits in the KEY RR flags as specified in the future will be used to indicate a key's validity for different protocols.
        /// </summary>
        public DnsKeyProtocol Protocol { get; private set; }

        /// <summary>
        /// The key algorithm parallel to the same field for the SIG resource.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// Optional second 16 bit flag field after the algorithm octet and before the key data.
        /// Must not be non-null unless one or more such additional bits have been defined and are non-zero.
        /// </summary>
        public ushort? FlagsExtension { get; private set; }

        /// <summary>
        /// The public key value.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        public bool Equals(DnsResourceDataKey other)
        {
            return other != null &&
                   AuthenticationProhibited.Equals(other.AuthenticationProhibited) &&
                   ConfidentialityProhibited.Equals(other.ConfidentialityProhibited) &&
                   NameType.Equals(other.NameType) &&
                   Signatory.Equals(other.Signatory) &&
                   Protocol.Equals(other.Protocol) &&
                   Algorithm.Equals(other.Algorithm) &&
                   (FlagsExtension.HasValue
                        ? other.FlagsExtension.HasValue && FlagsExtension.Value.Equals(other.FlagsExtension.Value)
                        : !other.FlagsExtension.HasValue) &&
                   PublicKey.Equals(other.PublicKey);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataKey);
        }

        internal DnsResourceDataKey()
            : this(false, false, DnsKeyNameType.ZoneKey, DnsKeySignatory.Zone, DnsKeyProtocol.All, DnsAlgorithm.None, null, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + (FlagsExtension != null ? sizeof(ushort) : 0) + PublicKey.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            byte flagsByte0 = 0;
            if (AuthenticationProhibited)
                flagsByte0 |= Mask.AuthenticationProhibited;
            if (ConfidentialityProhibited)
                flagsByte0 |= Mask.ConfidentialityProhibited;
            if (FlagsExtension.HasValue)
                flagsByte0 |= Mask.IsFlagsExtension;
            flagsByte0 |= (byte)((byte)NameType & Mask.NameType);
            buffer.Write(offset + Offset.AuthenticationProhibited, flagsByte0);

            byte flagsByte1 = 0;
            flagsByte1 |= (byte)((byte)Signatory & Mask.Signatory);
            buffer.Write(offset + Offset.Signatory, flagsByte1);

            buffer.Write(offset + Offset.Protocol, (byte)Protocol);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);

            if (FlagsExtension.HasValue)
                buffer.Write(offset + Offset.FlagsExtension, FlagsExtension.Value, Endianity.Big);

            PublicKey.Write(buffer, offset + Offset.FlagsExtension + (FlagsExtension.HasValue ? sizeof(ushort) : 0));
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            bool authenticationProhibited = data.ReadBool(Offset.AuthenticationProhibited, Mask.AuthenticationProhibited);
            bool confidentialityProhibited = data.ReadBool(Offset.ConfidentialityProhibited, Mask.ConfidentialityProhibited);
            bool isFlagsExtension = data.ReadBool(Offset.IsFlagsExtension, Mask.IsFlagsExtension);
            DnsKeyNameType nameType = (DnsKeyNameType)(data[Offset.NameType] & Mask.NameType);
            DnsKeySignatory signatory = (DnsKeySignatory)(data[Offset.Signatory] & Mask.Signatory);
            DnsKeyProtocol protocol = (DnsKeyProtocol)data[Offset.Protocol];
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            ushort? flagsExtension = (isFlagsExtension ? ((ushort?)data.ReadUShort(Offset.FlagsExtension, Endianity.Big)) : null);
            int publicKeyOffset = Offset.FlagsExtension + (isFlagsExtension ? sizeof(ushort) : 0);
            DataSegment publicKey = data.SubSegment(publicKeyOffset, data.Length - publicKeyOffset);

            return new DnsResourceDataKey(authenticationProhibited, confidentialityProhibited, nameType, signatory, protocol, algorithm, flagsExtension,
                                          publicKey);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+------------+
    /// | bit | 0-15       |
    /// +-----+------------+
    /// | 0   | Preference |
    /// +-----+------------+
    /// | 16  | MAP822     |
    /// | ... |            |
    /// +-----+------------+
    /// |     | MAPX400    |
    /// | ... |            |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Px)]
    public sealed class DnsResourceDataX400Pointer : DnsResourceData, IEquatable<DnsResourceDataX400Pointer>
    {
        private static class Offset
        {
            public const int Preference = 0;
            public const int Map822 = Preference + IpV4Address.SizeOf;
        }

        private const int ConstantPartLength = Offset.Map822;

        public DnsResourceDataX400Pointer(ushort preference, DnsDomainName map822, DnsDomainName mapX400)
        {
            Preference = preference;
            Map822 = map822;
            MapX400 = mapX400;
        }

        /// <summary>
        /// The preference given to this RR among others at the same owner.
        /// Lower values are preferred.
        /// </summary>
        public ushort Preference { get; private set; }

        /// <summary>
        /// RFC 822 domain.
        /// The RFC 822 part of the MCGAM.
        /// </summary>
        public DnsDomainName Map822 { get; private set; }

        /// <summary>
        /// The value of x400-in-domain-syntax derived from the X.400 part of the MCGAM.
        /// </summary>
        public DnsDomainName MapX400 { get; private set; }

        public bool Equals(DnsResourceDataX400Pointer other)
        {
            return other != null &&
                   Preference.Equals(other.Preference) &&
                   Map822.Equals(other.Map822) &&
                   MapX400.Equals(other.MapX400);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataX400Pointer);
        }

        internal DnsResourceDataX400Pointer()
            : this(0, DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return ConstantPartLength + Map822.GetLength(compressionData, offsetInDns) + MapX400.GetLength(compressionData, offsetInDns);
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            buffer.Write(dnsOffset + offsetInDns + Offset.Preference, Preference, Endianity.Big);
            int numBytesWritten = Map822.Write(buffer, dnsOffset, compressionData, offsetInDns + Offset.Map822);
            numBytesWritten += MapX400.Write(buffer, dnsOffset, compressionData, offsetInDns + ConstantPartLength + numBytesWritten);

            return ConstantPartLength + numBytesWritten;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            ushort preference = dns.ReadUShort(offsetInDns + Offset.Preference, Endianity.Big);

            offsetInDns += ConstantPartLength;
            length -= ConstantPartLength;

            DnsDomainName map822;
            int map822Length;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out map822, out map822Length))
                return null;
            offsetInDns += map822Length;
            length -= map822Length;

            DnsDomainName mapX400;
            int mapX400Length;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out mapX400, out mapX400Length))
                return null;
            length -= mapX400Length;

            if (length != 0)
                return null;

            return new DnsResourceDataX400Pointer(preference, map822, mapX400);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----------+
    /// | LONGITUDE |
    /// +-----------+
    /// | LATITUDE  |
    /// +-----------+
    /// | ALTITUDE  |
    /// +-----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.GPos)]
    public sealed class DnsResourceDataGeographicalPosition : DnsResourceDataSimple, IEquatable<DnsResourceDataGeographicalPosition>
    {
        public DnsResourceDataGeographicalPosition(string longitude, string latitude, string altitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
        }

        /// <summary>
        /// The real number describing the longitude encoded as a printable string.
        /// The precision is limited by 256 charcters within the range -90..90 degrees.
        /// Positive numbers indicate locations north of the equator.
        /// </summary>
        public string Longitude { get; private set; }

        /// <summary>
        /// The real number describing the latitude encoded as a printable string.
        /// The precision is limited by 256 charcters within the range -180..180 degrees.
        /// Positive numbers indicate locations east of the prime meridian.
        /// </summary>
        public string Latitude { get; private set; }

        /// <summary>
        /// The real number describing the altitude (in meters) from mean sea-level encoded as a printable string.
        /// The precision is limited by 256 charcters.
        /// Positive numbers indicate locations above mean sea-level.
        /// </summary>
        public string Altitude { get; private set; }

        public bool Equals(DnsResourceDataGeographicalPosition other)
        {
            return other != null &&
                   Longitude.Equals(other.Longitude) &&
                   Latitude.Equals(other.Latitude) &&
                   Altitude.Equals(other.Altitude);

        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataGeographicalPosition);
        }

        internal DnsResourceDataGeographicalPosition()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        internal override int GetLength()
        {
            return Encoding.ASCII.GetByteCount(Longitude) + 1 +
                   Encoding.ASCII.GetByteCount(Latitude) + 1 +
                   Encoding.ASCII.GetByteCount(Altitude) + 1;
        }


        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            byte[] longtitudeBytes = Encoding.ASCII.GetBytes(Longitude);
            byte[] latitudeBytes = Encoding.ASCII.GetBytes(Latitude);
            byte[] altitudeBytes = Encoding.ASCII.GetBytes(Altitude);

            buffer.Write(ref offset, longtitudeBytes);
            ++offset;
            buffer.Write(ref offset, latitudeBytes);
            ++offset;
            buffer.Write(ref offset, altitudeBytes);
            ++offset;
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            int longtitudeNumBytes = data.TakeWhile(value => value != 0).Count();
            if (longtitudeNumBytes == data.Length)
                return null;
            string longtitude = data.SubSegment(0, longtitudeNumBytes).ToString(Encoding.ASCII);
            data = data.SubSegment(longtitudeNumBytes + 1, data.Length - longtitudeNumBytes - 1);

            int latitudeNumBytes = data.TakeWhile(value => value != 0).Count();
            if (latitudeNumBytes == data.Length)
                return null;
            string latitude = data.SubSegment(0, latitudeNumBytes).ToString(Encoding.ASCII);
            data = data.SubSegment(latitudeNumBytes + 1, data.Length - latitudeNumBytes - 1);

            int altitudeNumBytes = data.TakeWhile(value => value != 0).Count();
            if (altitudeNumBytes == data.Length)
                return null;
            string altitude = data.SubSegment(0, altitudeNumBytes).ToString(Encoding.ASCII);

            return new DnsResourceDataGeographicalPosition(longtitude, latitude, altitude);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+-------+
    /// | bit | 0-127 |
    /// +-----+-------+
    /// | 0   | IP    |
    /// +-----+-------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Aaaa)]
    public sealed class DnsResourceDataIpV6 : DnsResourceDataSimple, IEquatable<DnsResourceDataIpV6>
    {
        public DnsResourceDataIpV6(IpV6Address data)
        {
            Data = data;
        }

        public IpV6Address Data { get; private set; }

        public bool Equals(DnsResourceDataIpV6 other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataIpV6);
        }

        internal DnsResourceDataIpV6()
            : this(IpV6Address.Zero)
        {
        }

        internal override int GetLength()
        {
            return IpV6Address.SizeOf;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data, Endianity.Big);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length != IpV6Address.SizeOf)
                return null;
            return new DnsResourceDataIpV6(data.ReadIpV6Address(0, Endianity.Big));
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+---------+------+-----------+----------+
    /// | bit | 0-7     | 8-15 | 16-23     | 24-31    |
    /// +-----+---------+------+-----------+----------+
    /// | 0   | VERSION | SIZE | HORIZ PRE | VERT PRE |
    /// +-----+---------+------+-----------+----------+
    /// | 32  | LATITUDE                              |
    /// +-----+---------------------------------------+
    /// | 64  | LONGITUDE                             |
    /// +-----+---------------------------------------+
    /// | 96  | ALTITUDE                              |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Loc)]
    public sealed class DnsResourceDataLocationInformation : DnsResourceDataSimple, IEquatable<DnsResourceDataLocationInformation>
    {
        public const ulong MaxSizeValue = 9 * 1000000000L;

        private static class Offset
        {
            public const int Version = 0;
            public const int Size = Version + sizeof(byte);
            public const int HorizontalPrecision = Size + sizeof(byte);
            public const int VerticalPrecision = HorizontalPrecision + sizeof(byte);
            public const int Latitude = VerticalPrecision + sizeof(byte);
            public const int Longitude = Latitude + sizeof(uint);
            public const int Altitude = Longitude + sizeof(uint);
        }

        public const int Length = Offset.Altitude + sizeof(uint);

        public DnsResourceDataLocationInformation(byte version, ulong size, ulong horizontalPrecision, ulong verticalPrecision, uint latitude, uint longitude,
                                                  uint altitude)
        {
            if (!IsValidSize(size))
                throw new ArgumentOutOfRangeException("size", size, "Must be in the form <digit> * 10^<digit>.");
            if (!IsValidSize(horizontalPrecision))
                throw new ArgumentOutOfRangeException("horizontalPrecision", horizontalPrecision, "Must be in the form <digit> * 10^<digit>.");
            if (!IsValidSize(verticalPrecision))
                throw new ArgumentOutOfRangeException("verticalPrecision", verticalPrecision, "Must be in the form <digit> * 10^<digit>.");

            Version = version;
            Size = size;
            HorizontalPrecision = horizontalPrecision;
            VerticalPrecision = verticalPrecision;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        /// <summary>
        /// Version number of the representation.
        /// This must be zero.
        /// Implementations are required to check this field and make no assumptions about the format of unrecognized versions.
        /// </summary>
        public byte Version { get; private set; }

        /// <summary>
        /// The diameter of a sphere enclosing the described entity, in centimeters.
        /// Only numbers of the form decimal digit times 10 in the power of a decimal digit are allowed since it is expressed as a pair of four-bit unsigned integers, 
        /// each ranging from zero to nine, with the most significant four bits representing the base and the second number representing the power of ten by which to multiply the base.
        /// This allows sizes from 0e0 (&lt;1cm) to 9e9(90,000km) to be expressed.
        /// This representation was chosen such that the hexadecimal representation can be read by eye; 0x15 = 1e5.
        /// Four-bit values greater than 9 are undefined, as are values with a base of zero and a non-zero exponent.
        /// 
        /// Since 20000000m (represented by the value 0x29) is greater than the equatorial diameter of the WGS 84 ellipsoid (12756274m),
        /// it is therefore suitable for use as a "worldwide" size.
        /// </summary>
        public ulong Size { get; private set; }

        /// <summary>
        /// The horizontal precision of the data, in centimeters, expressed using the same representation as Size.
        /// This is the diameter of the horizontal "circle of error", rather than a "plus or minus" value.
        /// (This was chosen to match the interpretation of Size; to get a "plus or minus" value, divide by 2.)
        /// </summary>
        public ulong HorizontalPrecision { get; private set; }

        /// <summary>
        /// The vertical precision of the data, in centimeters, expressed using the sane representation as for Size.
        /// This is the total potential vertical error, rather than a "plus or minus" value.
        /// (This was chosen to match the interpretation of SIize; to get a "plus or minus" value, divide by 2.)
        /// Note that if altitude above or below sea level is used as an approximation for altitude relative to the ellipsoid, the precision value should be adjusted.
        /// </summary>
        public ulong VerticalPrecision { get; private set; }

        /// <summary>
        /// The latitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in thousandths of a second of arc.
        /// 2^31 represents the equator; numbers above that are north latitude.
        /// </summary>
        public uint Latitude { get; private set; }

        /// <summary>
        /// The longitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in thousandths of a second of arc, rounded away from the prime meridian.
        /// 2^31 represents the prime meridian; numbers above that are east longitude.
        /// </summary>
        public uint Longitude { get; private set; }

        /// <summary>
        /// The altitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in centimeters,
        /// from a base of 100,000m below the reference spheroid used by GPS (semimajor axis a=6378137.0, reciprocal flattening rf=298.257223563).
        /// Altitude above (or below) sea level may be used as an approximation of altitude relative to the the spheroid,
        /// though due to the Earth's surface not being a perfect spheroid, there will be differences.
        /// (For example, the geoid (which sea level approximates) for the continental US ranges from 10 meters to 50 meters below the spheroid.
        /// Adjustments to Altitude and/or VerticalPrecision will be necessary in most cases.
        /// The Defense Mapping Agency publishes geoid height values relative to the ellipsoid.
        /// </summary>
        public uint Altitude { get; private set; }

        public bool Equals(DnsResourceDataLocationInformation other)
        {
            return other != null &&
                   Version.Equals(other.Version) &&
                   Size.Equals(other.Size) &&
                   HorizontalPrecision.Equals(other.HorizontalPrecision) &&
                   VerticalPrecision.Equals(other.VerticalPrecision) &&
                   Latitude.Equals(other.Latitude) &&
                   Longitude.Equals(other.Longitude) &&
                   Altitude.Equals(other.Altitude);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataLocationInformation);
        }

        internal DnsResourceDataLocationInformation()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

        internal override int GetLength()
        {
            return Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Version, Version);
            WriteSize(buffer, offset + Offset.Size, Size);
            WriteSize(buffer, offset + Offset.HorizontalPrecision, HorizontalPrecision);
            WriteSize(buffer, offset + Offset.VerticalPrecision, VerticalPrecision);
            buffer.Write(offset + Offset.Latitude, Latitude, Endianity.Big);
            buffer.Write(offset + Offset.Longitude, Longitude, Endianity.Big);
            buffer.Write(offset + Offset.Altitude, Altitude, Endianity.Big);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length != Length)
                return null;

            byte version = data[Offset.Version];
            ulong size = ReadSize(data[Offset.Size]);
            if (size > MaxSizeValue)
                return null;
            ulong horizontalPrecision = ReadSize(data[Offset.HorizontalPrecision]);
            if (horizontalPrecision > MaxSizeValue)
                return null;
            ulong verticalPrecision = ReadSize(data[Offset.VerticalPrecision]);
            if (verticalPrecision > MaxSizeValue)
                return null;
            uint latitude = data.ReadUInt(Offset.Latitude, Endianity.Big);
            uint longitude = data.ReadUInt(Offset.Longitude, Endianity.Big);
            uint altitude = data.ReadUInt(Offset.Altitude, Endianity.Big);

            return new DnsResourceDataLocationInformation(version, size, horizontalPrecision, verticalPrecision, latitude, longitude, altitude);
        }

        private static bool IsValidSize(ulong size)
        {
            if (size == 0)
                return true;
            if (size > MaxSizeValue)
                return false;
            while (size % 10 == 0)
                size /= 10;

            return size <= 9;
        }

        private static void WriteSize(byte[] buffer, int offset, ulong size)
        {
            byte baseValue;
            byte exponent;
            if (size == 0)
            {
                baseValue = 0;
                exponent = 0;
            }
            else
            {
                exponent = (byte)Math.Log10(size);
                baseValue = (byte)(size / Math.Pow(10, exponent));
            }

            byte value = (byte)((baseValue << 4) | exponent);
            buffer.Write(offset, value);
        }

        private static ulong ReadSize(byte value)
        {
            return (ulong)((value >> 4) * Math.Pow(10, value & 0x0F));
        }
    }

    /// <summary>
    /// RFC 2535.
    /// <pre>
    /// +------------------+
    /// | next domain name |
    /// |                  |
    /// +------------------+
    /// | type bit map     |
    /// |                  |
    /// +------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Nxt)]
    public sealed class DnsResourceDataNextDomain : DnsResourceData, IEquatable<DnsResourceDataNextDomain>
    {
        public const int MaxTypeBitMapLength = 16;
        public const DnsType MaxTypeBitMapDnsType = (DnsType)(8 * MaxTypeBitMapLength);

        public DnsResourceDataNextDomain(DnsDomainName nextDomainName, DataSegment typeBitMap)
        {
            if (typeBitMap.Length > MaxTypeBitMapLength)
                throw new ArgumentOutOfRangeException("typeBitMap", typeBitMap.Length, string.Format("Cannot be longer than {0} bytes.", MaxTypeBitMapLength));
            if (typeBitMap.Length > 0 && typeBitMap.Last == 0)
                throw new ArgumentOutOfRangeException("typeBitMap", typeBitMap, "Last byte cannot be 0x00");

            NextDomainName = nextDomainName;
            TypeBitMap = typeBitMap;
        }

        /// <summary>
        /// The next domain name according to the canonical DNS name order.
        /// </summary>
        public DnsDomainName NextDomainName { get; private set; }

        /// <summary>
        /// One bit per RR type present for the owner name.
        /// A one bit indicates that at least one RR of that type is present for the owner name.
        /// A zero indicates that no such RR is present.
        /// All bits not specified because they are beyond the end of the bit map are assumed to be zero.
        /// Note that bit 30, for NXT, will always be on so the minimum bit map length is actually four octets.
        /// Trailing zero octets are prohibited in this format.
        /// The first bit represents RR type zero (an illegal type which can not be present) and so will be zero in this format.
        /// This format is not used if there exists an RR with a type number greater than 127.
        /// If the zero bit of the type bit map is a one, it indicates that a different format is being used which will always be the case if a type number greater than 127 is present.
        /// </summary>
        public DataSegment TypeBitMap { get; private set; }

        public bool IsTypePresentForOwner(DnsType dnsType)
        {
            if (dnsType >= MaxTypeBitMapDnsType)
                throw new ArgumentOutOfRangeException("dnsType", dnsType, string.Format("Cannot be bigger than {0}.", MaxTypeBitMapDnsType));

            int byteOffset;
            byte mask;
            DnsTypeToByteOffsetAndMask(out byteOffset, out mask, dnsType);
            if (byteOffset > TypeBitMap.Length)
                return false;

            return TypeBitMap.ReadBool(byteOffset, mask);
        }

        public static DataSegment CreateTypeBitMap(IEnumerable<DnsType> typesPresentForOwner)
        {
            DnsType maxDnsType = typesPresentForOwner.Max();
            int length = (ushort)(maxDnsType + 7) / 8;
            if (length == 0)
                return DataSegment.Empty;

            byte[] typeBitMapBuffer = new byte[length];
            foreach (DnsType dnsType in typesPresentForOwner)
            {
                int byteOffset;
                byte mask;
                DnsTypeToByteOffsetAndMask(out byteOffset, out mask, dnsType);

                typeBitMapBuffer[byteOffset] |= mask;
            }

            return new DataSegment(typeBitMapBuffer);
        }

        public bool Equals(DnsResourceDataNextDomain other)
        {
            return other != null &&
                   NextDomainName.Equals(other.NextDomainName) &&
                   TypeBitMap.Equals(other.TypeBitMap);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNextDomain);
        }

        internal DnsResourceDataNextDomain()
            : this(DnsDomainName.Root, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return NextDomainName.GetLength(compressionData, offsetInDns) + TypeBitMap.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = NextDomainName.Write(buffer, dnsOffset, compressionData, offsetInDns);
            TypeBitMap.Write(buffer, dnsOffset + offsetInDns + numBytesWritten);

            return numBytesWritten + TypeBitMap.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName nextDomainName;
            int nextDomainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out nextDomainName, out nextDomainNameLength))
                return null;
            offsetInDns += nextDomainNameLength;
            length -= nextDomainNameLength;

            if (length > MaxTypeBitMapLength)
                return null;

            DataSegment typeBitMap = dns.SubSegment(offsetInDns, length);
            if (length != 0 && typeBitMap.Last == 0)
                return null;

            return new DnsResourceDataNextDomain(nextDomainName, typeBitMap);
        }

        private static void DnsTypeToByteOffsetAndMask(out int byteOffset, out byte mask, DnsType dnsType)
        {
            byteOffset = (ushort)dnsType / 8;
            mask = (byte)(1 << ((ushort)dnsType % 8));
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+----------+
    /// | bit | 0-15     |
    /// +-----+----------+
    /// | 0   | Priority |
    /// +-----+----------+
    /// | 16  | Weight   |
    /// +-----+----------+
    /// | 32  | Port     |
    /// +-----+----------+
    /// | 48  | Target   |
    /// | ... |          |
    /// +-----+----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Srv)]
    public sealed class DnsResourceDataServerSelection : DnsResourceDataNoCompression, IEquatable<DnsResourceDataServerSelection>
    {
        private static class Offset
        {
            public const int Priority = 0;
            public const int Weight = Priority + sizeof(ushort);
            public const int Port = Weight + sizeof(ushort);
            public const int Target = Port + sizeof(ushort);
        }

        public const int ConstantPartLength = Offset.Target;

        public DnsResourceDataServerSelection(ushort priority, ushort weight, ushort port, DnsDomainName target)
        {
            Priority = priority;
            Weight = weight;
            Port = port;
            Target = target;
        }

        /// <summary>
        /// The priority of this target host.
        /// A client must attempt to contact the target host with the lowest-numbered priority it can reach; 
        /// target hosts with the same priority should be tried in an order defined by the weight field.
        /// </summary>
        public ushort Priority { get; private set; }

        /// <summary>
        /// A server selection mechanism.
        /// The weight field specifies a relative weight for entries with the same priority.
        /// Larger weights should be given a proportionately higher probability of being selected.
        /// Domain administrators should use Weight 0 when there isn't any server selection to do, to make the RR easier to read for humans (less noisy).
        /// In the presence of records containing weights greater than 0, records with weight 0 should have a very small chance of being selected.
        /// 
        /// In the absence of a protocol whose specification calls for the use of other weighting information, a client arranges the SRV RRs of the same Priority in the order in which target hosts,
        /// specified by the SRV RRs, will be contacted. 
        /// The following algorithm SHOULD be used to order the SRV RRs of the same priority:
        /// To select a target to be contacted next, arrange all SRV RRs (that have not been ordered yet) in any order, except that all those with weight 0 are placed at the beginning of the list.
        /// Compute the sum of the weights of those RRs, and with each RR associate the running sum in the selected order.
        /// Then choose a uniform random number between 0 and the sum computed (inclusive), and select the RR whose running sum value is the first in the selected order which is greater than or equal to the random number selected.
        /// The target host specified in the selected SRV RR is the next one to be contacted by the client.
        /// Remove this SRV RR from the set of the unordered SRV RRs and apply the described algorithm to the unordered SRV RRs to select the next target host.
        /// Continue the ordering process until there are no unordered SRV RRs.
        /// This process is repeated for each Priority.
        /// </summary>
        public ushort Weight { get; private set; }

        /// <summary>
        /// The port on this target host of this service. 
        /// This is often as specified in Assigned Numbers but need not be.
        /// </summary>
        public ushort Port { get; private set; }

        /// <summary>
        /// The domain name of the target host.
        /// There must be one or more address records for this name, the name must not be an alias (in the sense of RFC 1034 or RFC 2181).
        /// Implementors are urged, but not required, to return the address record(s) in the Additional Data section.
        /// Unless and until permitted by future standards action, name compression is not to be used for this field.
        /// 
        /// A Target of "." means that the service is decidedly not available at this domain.
        /// </summary>
        public DnsDomainName Target { get; private set; }

        public bool Equals(DnsResourceDataServerSelection other)
        {
            return other != null &&
                   Priority.Equals(other.Priority) &&
                   Weight.Equals(other.Weight) &&
                   Port.Equals(other.Port) &&
                   Target.Equals(other.Target);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataServerSelection);
        }

        internal DnsResourceDataServerSelection()
            : this(0, 0, 0, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Target.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
 	    {
            buffer.Write(offset + Offset.Priority, Priority, Endianity.Big);
            buffer.Write(offset + Offset.Weight, Weight, Endianity.Big);
            buffer.Write(offset + Offset.Port, Port, Endianity.Big);
            Target.WriteUncompressed(buffer, offset + Offset.Target);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            ushort priortiy = dns.ReadUShort(offsetInDns + Offset.Priority, Endianity.Big);
            ushort weight = dns.ReadUShort(offsetInDns + Offset.Weight, Endianity.Big);
            ushort port = dns.ReadUShort(offsetInDns + Offset.Port, Endianity.Big);

            DnsDomainName target;
            int targetLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns + Offset.Target, length - ConstantPartLength, out target, out targetLength))
                return null;

            if (ConstantPartLength + targetLength != length)
                return null;

            return new DnsResourceDataServerSelection(priortiy, weight, port, target);
        }
    }

    public enum DnsAtmAddressFormat : byte
    {
        /// <summary>
        /// ATM  End  System Address (AESA) format.
        /// </summary>
        AtmEndSystemAddress = 0,

        /// <summary>
        /// E.164 format.
        /// </summary>
        E164 = 1,
    }

    /// <summary>
    /// <pre>
    /// +-----+---------+
    /// | bit | 0-7     |
    /// +-----+---------+
    /// | 0   | FORMAT  |
    /// +-----+---------+
    /// | 8   | ADDRESS |
    /// | ... |         |
    /// +-----+---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.AtmA)]
    public sealed class DnsResourceDataAtmAddress : DnsResourceDataSimple, IEquatable<DnsResourceDataAtmAddress>
    {
        private static class Offset
        {
            public const int Format = 0;
            public const int Address = Format + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.Address;

        public DnsResourceDataAtmAddress(DnsAtmAddressFormat format, DataSegment address)
        {
            Format = format;
            Address = address;
        }

        /// <summary>
        /// The format of Address.
        /// </summary>
        public DnsAtmAddressFormat Format { get; private set; }

        /// <summary>
        /// Variable length string of octets containing the ATM address of the node to which this RR pertains.
        /// When the format is AESA, the address is coded as described in ISO 8348/AD 2 using the preferred binary encoding of the ISO NSAP format.
        /// When the format value is E.164, the Address/Number Digits appear in the order in which they would be entered on a numeric keypad.
        /// Digits are coded in IA5 characters with the leftmost bit of each digit set to 0.
        /// This ATM address appears in ATM End System Address Octets field (AESA format) or the Address/Number Digits field (E.164 format) of the Called party number information element [ATMUNI3.1].
        /// Subaddress information is intentionally not included because E.164 subaddress information is used for routing.
        /// </summary>
        public DataSegment Address { get; private set; }

        public bool Equals(DnsResourceDataAtmAddress other)
        {
            return other != null &&
                   Format.Equals(other.Format) &&
                   Address.Equals(other.Address);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataAtmAddress);
        }

        internal DnsResourceDataAtmAddress()
            : this(DnsAtmAddressFormat.AtmEndSystemAddress, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Address.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Format, (byte)Format);
            Address.Write(buffer, offset + Offset.Address);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsAtmAddressFormat format = (DnsAtmAddressFormat)data[Offset.Format];
            DataSegment address = data.SubSegment(Offset.Address, data.Length - ConstantPartLength);

            return new DnsResourceDataAtmAddress(format, address);
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+-------+------------+
    /// | bit | 0-15  | 16-31      |
    /// +-----+-------+------------+
    /// | 0   | Order | Preference |
    /// +-----+-------+------------+
    /// | 32  | FLAGS              |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | SERVICES           |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | REGEXP             |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | REPLACEMENT        |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NaPtr)]
    public sealed class DnsResourceDataNamingAuthorityPointer : DnsResourceDataNoCompression, IEquatable<DnsResourceDataNamingAuthorityPointer>
    {
        private static class Offset
        {
            public const int Order = 0;
            public const int Preference = Order + sizeof(ushort);
            public const int Flags = Preference + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.Flags;

        public DnsResourceDataNamingAuthorityPointer(ushort order, ushort preference, DataSegment flags, DataSegment services, DataSegment regexp, DnsDomainName replacement)
        {
            if (!IsLegalFlags(flags))
            {
                throw new ArgumentException(
                    string.Format("Flags ({0}) contain a non [a-zA-Z0-9] character.",
                                  Encoding.ASCII.GetString(flags.Buffer, flags.StartOffset, flags.Length)),
                    "flags");
            }

            Order = order;
            Preference = preference;
            Flags = flags.All(flag => flag < 'a' || flag > 'z') && flags.IsStrictOrdered()
                        ? flags
                        : new DataSegment(flags.Select(flag => flag >= 'a' && flag <= 'z' ? (byte)(flag + 'A' - 'a') : flag)
                                              .Distinct().OrderBy(flag => flag).ToArray());
            Services = services;
            Regexp = regexp;
            Replacement = replacement;
        }

        /// <summary>
        /// A 16-bit unsigned integer specifying the order in which the NAPTR records MUST be processed in order to accurately represent the ordered list of Rules.
        /// The ordering is from lowest to highest.
        /// If two records have the same order value then they are considered to be the same rule and should be selected based on the combination of the Preference values and Services offered.
        /// </summary>
        public ushort Order { get; private set; }

        /// <summary>
        /// Although it is called "preference" in deference to DNS terminology, this field is equivalent to the Priority value in the DDDS Algorithm.
        /// It specifies the order in which NAPTR records with equal Order values should be processed, low numbers being processed before high numbers.
        /// This is similar to the preference field in an MX record, and is used so domain administrators can direct clients towards more capable hosts or lighter weight protocols.
        /// A client may look at records with higher preference values if it has a good reason to do so such as not supporting some protocol or service very well.
        /// The important difference between Order and Preference is that once a match is found the client must not consider records with a different Order but they may process records with the same Order but different Preferences.
        /// The only exception to this is noted in the second important Note in the DDDS algorithm specification concerning allowing clients to use more complex Service determination between steps 3 and 4 in the algorithm.
        /// Preference is used to give communicate a higher quality of service to rules that are considered the same from an authority standpoint but not from a simple load balancing standpoint.
        /// 
        /// It is important to note that DNS contains several load balancing mechanisms and if load balancing among otherwise equal services should be needed then methods such as SRV records or multiple A records should be utilized to accomplish load balancing.
        /// </summary>
        public ushort Preference { get; private set; }

        /// <summary>
        /// Flags to control aspects of the rewriting and interpretation of the fields in the record.
        /// Flags are single characters from the set A-Z and 0-9.
        /// The case of the alphabetic characters is not significant.
        /// The field can be empty.
        /// 
        /// It is up to the Application specifying how it is using this Database to define the Flags in this field.
        /// It must define which ones are terminal and which ones are not.
        /// </summary>
        public DataSegment Flags { get; private set; }

        /// <summary>
        /// Specifies the Service Parameters applicable to this this delegation path.
        /// It is up to the Application Specification to specify the values found in this field.
        /// </summary>
        public DataSegment Services { get; private set; }

        /// <summary>
        /// A substitution expression that is applied to the original string held by the client in order to construct the next domain name to lookup.
        /// See the DDDS Algorithm specification for the syntax of this field.
        /// 
        /// As stated in the DDDS algorithm, The regular expressions must not be used in a cumulative fashion, that is, they should only be applied to the original string held by the client, never to the domain name produced by a previous NAPTR rewrite.
        /// The latter is tempting in some applications but experience has shown such use to be extremely fault sensitive, very error prone, and extremely difficult to debug.
        /// </summary>
        public DataSegment Regexp { get; private set; }

        /// <summary>
        /// The next domain-name to query for depending on the potential values found in the flags field.
        /// This field is used when the regular expression is a simple replacement operation.
        /// Any value in this field must be a fully qualified domain-name.
        /// Name compression is not to be used for this field.
        /// 
        /// This field and the Regexp field together make up the Substitution Expression in the DDDS Algorithm.
        /// It is simply a historical optimization specifically for DNS compression that this field exists.
        /// The fields are also mutually exclusive.  
        /// If a record is returned that has values for both fields then it is considered to be in error and SHOULD be either ignored or an error returned.
        /// </summary>
        public DnsDomainName Replacement { get; private set; }

        public bool Equals(DnsResourceDataNamingAuthorityPointer other)
        {
            return other != null &&
                   Order.Equals(other.Order) &&
                   Preference.Equals(other.Preference) &&
                   Flags.Equals(other.Flags) &&
                   Services.Equals(other.Services) &&
                   Regexp.Equals(other.Regexp) &&
                   Replacement.Equals(other.Replacement);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNamingAuthorityPointer);
        }

        internal DnsResourceDataNamingAuthorityPointer()
            : this(0, 0, DataSegment.Empty, DataSegment.Empty, DataSegment.Empty, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + GetStringLength(Flags) + GetStringLength(Services) + GetStringLength(Regexp) + Replacement.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Order, Order, Endianity.Big);
            buffer.Write(offset + Offset.Preference, Preference, Endianity.Big);
            offset += Offset.Flags;
            WriteString(buffer, ref offset, Flags);
            WriteString(buffer, ref offset, Services);
            WriteString(buffer, ref offset, Regexp);
            Replacement.WriteUncompressed(buffer, offset);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            ushort order = dns.ReadUShort(offsetInDns + Offset.Order, Endianity.Big);
            ushort preference = dns.ReadUShort(offsetInDns + Offset.Preference, Endianity.Big);

            DataSegment data = dns.SubSegment(offsetInDns + ConstantPartLength, length - ConstantPartLength);
            int dataOffset = 0;

            DataSegment flags = ReadString(data, ref dataOffset);
            if (flags == null || !IsLegalFlags(flags))
                return null;

            DataSegment services = ReadString(data, ref dataOffset);
            if (services == null)
                return null;

            DataSegment regexp = ReadString(data, ref dataOffset);
            if (regexp == null)
                return null;

            DnsDomainName replacement;
            int replacementLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns + ConstantPartLength + dataOffset, length - ConstantPartLength - dataOffset,
                                        out replacement, out replacementLength))
            {
                return null;
            }

            if (ConstantPartLength + dataOffset + replacementLength != length)
                return null;

            return new DnsResourceDataNamingAuthorityPointer(order, preference, flags, services, regexp, replacement);
        }

        private static bool IsLegalFlags(DataSegment flags)
        {
            return flags.All(flag => (flag >= '0' && flag <= '9' || flag >= 'A' && flag <= 'Z' || flag >= 'a' && flag <= 'z'));
        }
    }

    /// <summary>
    /// <pre>
    /// +-----+-------------------+
    /// | bit | 0-15              |
    /// +-----+-------------------+
    /// | 0   | PREFERENCE        |
    /// +-----+-------------------+
    /// | 16  | EXCHANGER         |
    /// | ... |                   |
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Kx)]
    public sealed class DnsResourceDataKeyExchanger : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataKeyExchanger(ushort preference, DnsDomainName keyExchanger)
            : base(preference, keyExchanger)
        {
        }

        /// <summary>
        /// Specifies the preference given to this RR among other KX records at the same owner.
        /// Lower values are preferred.
        /// </summary>
        public ushort Preference { get { return Value; } }

        /// <summary>
        /// Specifies a host willing to act as a key exchange for the owner name.
        /// </summary>
        public DnsDomainName KeyExchangeHost { get { return DomainName; } }

        internal DnsResourceDataKeyExchanger()
            : this(0, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort preference;
            DnsDomainName keyExchangeHost;
            if (!TryRead(out preference, out keyExchangeHost, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataKeyExchanger(preference, keyExchangeHost);
        }
    }

    public enum DnsCertificateType : ushort
    {
        /// <summary>
        /// RFC 4398.
        /// Indicates an X.509 certificate conforming to the profile defined by the IETF PKIX working group. 
        /// The certificate section will start with a one-octet unsigned OID length and then an X.500 OID indicating the nature of the remainder of the certificate section.
        /// Note: X.509 certificates do not include their X.500 directory-type-designating OID as a prefix.
        /// </summary>
        Pkix = 1,

        /// <summary>
        /// RFC 4398.
        /// SPKI certificate.
        /// </summary>
        Spki = 2,

        /// <summary>
        /// RFC 4398.
        /// Indicates an OpenPGP packet.
        /// This is used to transfer public key material and revocation signatures.
        /// The data is binary and must not be encoded into an ASCII armor.
        /// An implementation should process transferable public keys, but it may handle additional OpenPGP packets.
        /// </summary>
        Pgp = 3,

        /// <summary>
        /// RFC 4398.
        /// The URL of an X.509 data object.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        IPkix = 4,

        /// <summary>
        /// RFC 4398.
        /// The URL of an SPKI certificate.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        ISpki = 5,

        /// <summary>
        /// RFC 4398.
        /// Contains both an OpenPGP fingerprint for the key in question, as well as a URL.
        /// The certificate portion of the IPgp CERT RR is defined as a one-octet fingerprint length, followed by the OpenPGP fingerprint, followed by the URL.
        /// The OpenPGP fingerprint is calculated as defined in RFC 2440.
        /// A zero-length fingerprint or a zero-length URL are legal, and indicate URL-only IPGP data or fingerprint-only IPGP data, respectively.
        /// A zero-length fingerprint and a zero-length URL are meaningless and invalid.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        Ipgp = 6,

        /// <summary>
        /// RFC 4398.
        /// Attribute Certificate.
        /// </summary>
        AcPkix = 7,

        /// <summary>
        /// RFC 4398.
        /// The URL of an Attribute Certificate.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        IAcPkix = 8,

        /// <summary>
        /// RFC 4398.
        /// Indicates a certificate format defined by an absolute URI.
        /// The certificate portion of the CERT RR must begin with a null-terminated URI, and the data after the null is the private format certificate itself.
        /// The URI should be such that a retrieval from it will lead to documentation on the format of the certificate.
        /// Recognition of private certificate types need not be based on URI equality but can use various forms of pattern matching so that, for example, subtype or version information can also be encoded into the URI.
        /// </summary>
        Uri = 253,

        /// <summary>
        /// RFC 4398.
        /// Indicates a private format certificate specified by an ISO OID prefix.
        /// The certificate section will start with a one-octet unsigned OID length and then a BER-encoded OID indicating the nature of the remainder of the certificate section.
        /// This can be an X.509 certificate format or some other format.
        /// X.509 certificates that conform to the IETF PKIX profile should be indicated by the PKIX type, not the OID private type.
        /// Recognition of private certificate types need not be based on OID equality but can use various forms of pattern matching such as OID prefix.
        /// </summary>
        Oid = 254,
    }

    /// <summary>
    /// RFC 4398.
    /// <pre>
    /// +-----+-----------+------+------------+
    /// | bit | 0-7       | 8-15 | 16-31      |
    /// +-----+-----------+------+------------+
    /// | 0   | type             | key tag    |
    /// +-----+-----------+------+------------+
    /// | 32  | algorithm | certificate or CRL|
    /// +-----+-----------+                   |
    /// |     |                               |
    /// | ... |                               |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Cert)]
    public sealed class DnsResourceDataCertificate : DnsResourceDataSimple, IEquatable<DnsResourceDataCertificate>
    {
        private static class Offset
        {
            public const int Type = 0;
            public const int KeyTag = Type + sizeof(ushort);
            public const int Algorithm = KeyTag + sizeof(ushort);
            public const int Certificate = Algorithm + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.Certificate;

        public DnsResourceDataCertificate(DnsCertificateType certificateType, ushort keyTag, DnsAlgorithm algorithm, DataSegment certificate)
        {
            CertificateType = certificateType;
            KeyTag = keyTag;
            Algorithm = algorithm;
            Certificate = certificate;
        }

        /// <summary>
        /// The certificate type.
        /// </summary>
        public DnsCertificateType CertificateType { get; private set; }

        /// <summary>
        /// Value computed for the key embedded in the certificate, using the RRSIG Key Tag algorithm.
        /// This field is used as an efficiency measure to pick which CERT RRs may be applicable to a particular key.
        /// The key tag can be calculated for the key in question, and then only CERT RRs with the same key tag need to be examined.
        /// Note that two different keys can have the same key tag.
        /// However, the key must be transformed to the format it would have as the public key portion of a DNSKEY RR before the key tag is computed.
        /// This is only possible if the key is applicable to an algorithm and complies to limits (such as key size) defined for DNS security.
        /// If it is not, the algorithm field must be zero and the tag field is meaningless and should be zero.
        /// </summary>
        public ushort KeyTag { get; private set; }

        /// <summary>
        /// Has the same meaning as the algorithm field in DNSKEY and RRSIG RRs,
        /// except that a zero algorithm field indicates that the algorithm is unknown to a secure DNS, 
        /// which may simply be the result of the algorithm not having been standardized for DNSSEC.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// The certificate data according to the type.
        /// </summary>
        public DataSegment Certificate { get; private set; }

        public bool Equals(DnsResourceDataCertificate other)
        {
            return other != null &&
                   CertificateType.Equals(other.CertificateType) &&
                   KeyTag.Equals(other.KeyTag) &&
                   Algorithm.Equals(other.Algorithm) &&
                   Certificate.Equals(other.Certificate);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataCertificate);
        }

        internal DnsResourceDataCertificate()
            : this(DnsCertificateType.Pkix, 0, DnsAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Certificate.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Type, (ushort)CertificateType, Endianity.Big);
            buffer.Write(offset + Offset.KeyTag, KeyTag, Endianity.Big);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            Certificate.Write(buffer, offset + Offset.Certificate);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsCertificateType type = (DnsCertificateType)data.ReadUShort(Offset.Type, Endianity.Big);
            ushort keyTag = data.ReadUShort(Offset.KeyTag, Endianity.Big);
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DataSegment certificate = data.SubSegment(Offset.Certificate, data.Length - ConstantPartLength);

            return new DnsResourceDataCertificate(type, keyTag, algorithm, certificate);
        }
    }

    /// <summary>
    /// RFC 2874.
    /// <pre>
    /// +-------------+----------------+-----------------+
    /// | Prefix len. | Address suffix | Prefix name     |
    /// | (1 octet)   | (0..16 octets) | (0..255 octets) |
    /// +-------------+----------------+-----------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.A6)]
    public sealed class DnsResourceDataA6 : DnsResourceDataNoCompression, IEquatable<DnsResourceDataA6>
    {
        public const byte MaxPrefixLength = 8 * IpV6Address.SizeOf;

        private static class Offset
        {
            public const int PrefixLength = 0;
            public const int AddressSuffix = PrefixLength + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.AddressSuffix;

        public DnsResourceDataA6(byte prefixLength, IpV6Address addressSuffix, DnsDomainName prefixName)
        {
            PrefixLength = prefixLength;
            AddressSuffix = addressSuffix;
            PrefixName = prefixName;
        }

        /// <summary>
        /// Encoded as an eight-bit unsigned integer with value between 0 and 128 inclusive.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// An IPv6 address suffix, encoded in network order (high-order octet first).
        /// There must be exactly enough octets in this field to contain a number of bits equal to 128 minus prefix length, with 0 to 7 leading pad bits to make this field an integral number of octets.
        /// Pad bits, if present, must be set to zero when loading a zone file and ignored (other than for SIG verification) on reception.
        /// </summary>
        public IpV6Address AddressSuffix { get; private set; }

        /// <summary>
        /// The number of bytes the address suffix takes.
        /// </summary>
        public int AddressSuffixLength { get { return CalculateAddressSuffixLength(PrefixLength); } }

        /// <summary>
        /// The name of the prefix, encoded as a domain name.
        /// This name must not be compressed.
        /// </summary>
        public DnsDomainName PrefixName { get; private set; }

        public bool Equals(DnsResourceDataA6 other)
        {
            return other != null &&
                   PrefixLength.Equals(other.PrefixLength) &&
                   AddressSuffix.Equals(other.AddressSuffix) &&
                   PrefixName.Equals(other.PrefixName);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataA6);
        }

        internal DnsResourceDataA6()
            : this(0, IpV6Address.Zero, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + AddressSuffixLength + PrefixName.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.WriteUnsigned(offset + Offset.AddressSuffix, AddressSuffix.ToValue(), AddressSuffixLength, Endianity.Big);
            PrefixName.WriteUncompressed(buffer, offset + ConstantPartLength + AddressSuffixLength);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            byte prefixLength = dns[offsetInDns + Offset.PrefixLength];
            if (prefixLength > MaxPrefixLength)
                return null;
            offsetInDns += ConstantPartLength;
            length -= ConstantPartLength;

            int addressSuffixLength = CalculateAddressSuffixLength(prefixLength);
            if (length < addressSuffixLength)
                return null;
            IpV6Address addressSuffix = new IpV6Address((UInt128)dns.ReadUnsignedBigInteger(offsetInDns, addressSuffixLength, Endianity.Big));
            offsetInDns += addressSuffixLength;
            length -= addressSuffixLength;

            DnsDomainName prefixName;
            int prefixNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out prefixName, out prefixNameLength))
                return null;
            if (prefixNameLength != length)
                return null;

            return new DnsResourceDataA6(prefixLength, addressSuffix, prefixName);
        }

        private static int CalculateAddressSuffixLength(byte prefixLength)
        {
            return (MaxPrefixLength - prefixLength + 7) / 8;
        }
    }

    /// <summary>
    /// Eastlake.
    /// </summary>
    public enum DnsSinkCodingSubcoding : ushort
    {
        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Basic Encoding Rules.
        /// </summary>
        Asn1SnmpBer = 0x0101,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Distinguished Encoding Rules.
        /// </summary>
        Asn1SnmpDer = 0x0102,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Packed Encoding Rules Aligned.
        /// </summary>
        Asn1SnmpPer = 0x0103,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        Asn1SnmpPerUnaligned = 0x0104,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Canonical Encoding Rules.
        /// </summary>
        Asn1SnmpCer = 0x0105,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// An OID preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private encoding.
        /// </summary>
        Asn1SnmpPrivate = 0x01FE,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Basic Encoding Rules.
        /// </summary>
        Asn1Osi1990Ber = 0x0201,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Distinguished Encoding Rules.
        /// </summary>
        Asn1Osi1990Der = 0x0202,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Packed Encoding Rules Aligned.
        /// </summary>
        Asn1Osi1990Per = 0x0203,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        Asn1Osi1990PerUnaligned = 0x0204,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Canonical Encoding Rules.
        /// </summary>
        Asn1Osi1990Cer = 0x0205,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// An OID preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private encoding.
        /// </summary>
        Asn1Osi1990Private = 0x02FE,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Basic Encoding Rules.
        /// </summary>
        Asn1Osi1994Ber = 0x0301,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Distinguished Encoding Rules.
        /// </summary>
        Asn1Osi1994Der = 0x0302,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Packed Encoding Rules Aligned.
        /// </summary>
        Asn1Osi1994Per = 0x0303,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        Asn1Osi1994PerUnaligned = 0x0304,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Canonical Encoding Rules.
        /// </summary>
        Asn1Osi1994Cer = 0x0305,

        /// <summary>
        /// OSI ASN.1 1994.
        /// An OID preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private encoding.
        /// </summary>
        Asn1Osi1994Private = 0x03FE,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Basic Encoding Rules.
        /// </summary>
        AsnPrivateBer = 0x3F01,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Distinguished Encoding Rules.
        /// </summary>
        AsnPrivateDer = 0x3F02,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Packed Encoding Rules Aligned.
        /// </summary>
        AsnPrivatePer = 0x3F03,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        AsnPrivatePerUnaligned = 0x3F04,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Canonical Encoding Rules.
        /// </summary>
        AsnPrivateCer = 0x3F05,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// An OID preceded by a one byte unsigned length appears in the data area just after the coding OID.
        /// </summary>
        AsnPrivatePrivate = 0x3FFE,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// 7 bit.
        /// </summary>
        Mime7Bit = 0x4101,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// 8 bit.
        /// </summary>
        Mime8Bit = 0x4102,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// Binary.
        /// </summary>
        MimeBinary = 0x4103,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// Quoted-printable.
        /// </summary>
        MimeQuotedPrintable = 0x4104,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// Base 64.
        /// </summary>
        MimeBase64 = 0x4105,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// The data portion must start with an "x-" token denoting the private content-transfer-encoding immediately followed by one null (zero) octet 
        /// followed by the remainder of the MIME object.
        /// </summary>
        MimePrivate = 0x41FE,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// ASCII.
        /// </summary>
        TextTaggedDataAscii = 0x4201,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// UTF-7 [RFC 1642].
        /// </summary>
        TextTaggedDataUtf7 = 0x4202,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// UTF-8 [RFC 2044].
        /// </summary>
        TextTaggedDataUtf8 = 0x4203,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// ASCII with MIME header escapes [RFC 2047].
        /// </summary>
        TextTaggedDataAsciiMimeHeaderEscapes = 0x4204,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// Each text item must start with a domain name [RFC 1034] denoting the private text encoding immediately followed by one null (zero) octet
        /// followed by the remainder of the text item.
        /// </summary>
        TextTaggedDataPrivate = 0x42FE,
    }
    
    /// <summary>
    /// Eastlake.
    /// </summary>
    public enum DnsSinkCoding : byte
    {
        /// <summary>
        /// The SNMP subset of ASN.1.
        /// </summary>
        Asn1Snmp = 0x01,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// </summary>
        Asn1Osi1990 = 0x02,

        /// <summary>
        /// OSI ASN.1 1994.
        /// </summary>
        Asn1Osi1994 = 0x03,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// </summary>
        AsnPrivate = 0x3F,

        /// <summary>
        /// DNS RRs.
        /// The data portion consists of DNS resource records as they would be transmitted in a DNS response section.
        /// The subcoding octet is the number of RRs in the data area as an unsigned integer.
        /// Domain names may be compressed via pointers as in DNS replies.
        /// The origin for the pointers is the beginning of the RDATA section of the SINK RR.
        /// Thus the SINK RR is safe to cache since only code that knows how to parse the data portion need know of and can expand these compressions.
        /// </summary>
        DnsResourceRecords = 0x40,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// The top level Content-Transfer-Encoding may be encoded into the subcoding octet.
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// </summary>
        Mime = 0x41,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// </summary>
        TextTaggedData = 0x42,

        /// <summary>
        /// Private formats indicated by a URL.
        /// The format of the data portion is indicated by an initial URL [RFC 1738] which is terminated by a zero valued octet
        /// followed by the data with that format.
        /// The subcoding octet is available for whatever use the private formating wishes to make of it.
        /// The manner in which the URL specifies the format is not defined but presumably the retriever will recognize the URL or the data it points to.
        /// </summary>
        PrivateByUrl = 0xFE
    }

    /// <summary>
    /// Eastlake.
    /// <pre>
    /// +-----+--------+-----------+
    /// | bit | 0-7    | 8-15      |
    /// +-----+--------+-----------+
    /// | 0   | coding | subcoding |
    /// +-----+--------+-----------+
    /// | 16  | data               |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Sink)]
    public sealed class DnsResourceDataSink: DnsResourceDataSimple, IEquatable<DnsResourceDataSink>
    {
        private static class Offset
        {
            public const int Coding = 0;
            public const int Subcoding = Coding + sizeof(byte);
            public const int Data = Subcoding + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.Data;

        public DnsResourceDataSink(DnsSinkCodingSubcoding codingSubcoding, DataSegment data)
            : this((DnsSinkCoding)((ushort)codingSubcoding >> 8), (byte)((ushort)codingSubcoding & 0x00FF), data)
        {
        }

        public DnsResourceDataSink(DnsSinkCoding coding, byte subcoding, DataSegment data)
        {
            Coding = coding;
            Subcoding = subcoding;
            Data = data;
        }

        /// <summary>
        /// Gives the general structure of the data.
        /// </summary>
        public DnsSinkCoding Coding { get; private set; }

        /// <summary>
        /// Provides additional information depending on the value of the coding.
        /// </summary>
        public byte Subcoding { get; private set; }

        /// <summary>
        /// Returns a combination of coding and subcoding.
        /// Has a valid enum value if the subcoding is defined specifically for the coding.
        /// </summary>
        public DnsSinkCodingSubcoding CodingSubcoding
        {
            get
            {
                ushort codingSubcoding = (ushort)(((ushort)Coding << 8) | Subcoding);
                return (DnsSinkCodingSubcoding)codingSubcoding;
            }
        }

        /// <summary>
        /// Variable length and could be null in some cases.
        /// </summary>
        public DataSegment Data { get; private set; }

        public bool Equals(DnsResourceDataSink other)
        {
            return other != null &&
                   Coding.Equals(other.Coding) &&
                   Subcoding.Equals(other.Subcoding) &&
                   Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataSink);
        }

        internal DnsResourceDataSink()
            : this(DnsSinkCodingSubcoding.Asn1SnmpBer, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Data.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Coding, (byte)Coding);
            buffer.Write(offset + Offset.Subcoding, Subcoding);
            Data.Write(buffer, offset + Offset.Data);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsSinkCoding coding = (DnsSinkCoding)data[Offset.Coding];
            byte subcoding = data[Offset.Subcoding];
            DataSegment dataValue = data.SubSegment(Offset.Data, data.Length - ConstantPartLength);

            return new DnsResourceDataSink(coding, subcoding, dataValue);
        }
    }

    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
    /// </summary>
    public enum DnsLongLivedQueryOpcode : ushort
    {
        Setup = 1,
        Refresh = 2,
        Event = 3,
    }

    public enum DnsLongLivedQueryErrorCode : ushort
    {
        /// <summary>
        /// The LLQ Setup Request was successful.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// The server cannot grant the LLQ request because it is overloaded,
        /// or the request exceeds the server's rate limit (see Section 8 "Security Considerations").
        /// Upon returning this error, the server MUST include in the LEASE-LIFE field a time interval, in seconds,
        /// after which the client may re-try the LLQ Setup.
        /// </summary>
        ServerFull = 1,

        /// <summary>
        /// The data for this name and type is not expected to change frequently, and the server therefore does not support the requested LLQ.
        /// The client must not poll for this name and type, nor should it re-try the LLQ Setup, and should instead honor the normal resource record TTLs returned.
        /// To reduce server load, an administrator MAY return this error for all records with types other than PTR and TXT as a matter of course.
        /// </summary>
        Static = 2,

        /// <summary>
        /// The LLQ was improperly formatted.
        /// Note that if the rest of the DNS message is properly formatted, the DNS header error code must not include a format error code,
        ///  as this would cause confusion between a server that does not understand the LLQ format, and a client that sends malformed LLQs.
        /// </summary>
        FormatError = 3,

        /// <summary>
        /// The client attempts to refresh an expired or non-existent LLQ (as determined by the LLQ-ID in the request).
        /// </summary>
        NoSuchLlq =  4,

        /// <summary>
        /// The protocol version specified in the client's request is not supported by the server.
        /// </summary>
        BadVersion = 5,

        /// <summary>
        /// The LLQ was not granted for an unknown reason.
        /// </summary>
        UnknownError = 6,
    }

    public enum DnsOptionCode : ushort
    {
        /// <summary>
        /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
        /// LLQ.
        /// </summary>
        LongLivedQuery = 1,

        /// <summary>
        /// http://files.dns-sd.org/draft-sekar-dns-ul.txt.
        /// UL.
        /// </summary>
        UpdateLease = 2,

        /// <summary>
        /// RFC 5001.
        /// NSID.
        /// </summary>
        NameServerIdentifier = 3,
    }

    public abstract class DnsOption : IEquatable<DnsOption>
    {
        public const int MinimumLength = sizeof(ushort) + sizeof(ushort);

        public DnsOptionCode Code { get; private set; }
        public int Length { get { return MinimumLength + DataLength; } }
        public abstract int DataLength { get; }

        public bool Equals(DnsOption other)
        {
            return other != null &&
                   Code.Equals(other.Code) && 
                   GetType().Equals(other.GetType()) &&
                   EqualsData(other);
        }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as DnsOption);
        }

        internal DnsOption(DnsOptionCode code)
        {
            Code = code;
        }

        internal abstract bool EqualsData(DnsOption other);

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (ushort)Code, Endianity.Big);
            buffer.Write(ref offset, (ushort)DataLength, Endianity.Big);
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);

        internal static DnsOption CreateInstance(DnsOptionCode code, DataSegment data)
        {
            switch (code)
            {
                case DnsOptionCode.LongLivedQuery:
                    if (data.Length < DnsOptionLongLivedQuery.MinimumDataLength)
                        return null;
                    return DnsOptionLongLivedQuery.Read(data);

                case DnsOptionCode.UpdateLease:
                    if (data.Length < DnsOptionUpdateLease.MinimumDataLength)
                        return null;
                    return DnsOptionUpdateLease.Read(data);

                case DnsOptionCode.NameServerIdentifier:
                default:
                    return new DnsOptionAnything(code, data);
            }
        }
    }

    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
    /// <pre>
    /// +-----+------------+
    /// | bit | 0-15       |
    /// +-----+------------+
    /// | 0   | VERSION    |
    /// +-----+------------+
    /// | 16  | LLQ-OPCODE |
    /// +-----+------------+
    /// | 32  | ERROR-CODE |
    /// +-----+------------+
    /// | 48  | LLQ-ID     |
    /// |     |            |
    /// |     |            |
    /// |     |            |
    /// +-----+------------+
    /// | 112 | LEASE-LIFE |
    /// |     |            |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    public class DnsOptionLongLivedQuery : DnsOption
    {
        private static class Offset
        {
            public const int Version = 0;
            public const int Opcode = Version + sizeof(ushort);
            public const int ErrorCode = Opcode + sizeof(ushort);
            public const int Id = ErrorCode + sizeof(ushort);
            public const int LeaseLife = Id + sizeof(ulong);
        }

        public const int MinimumDataLength = Offset.LeaseLife + sizeof(uint);

        public DnsOptionLongLivedQuery(ushort version, DnsLongLivedQueryOpcode opcode, DnsLongLivedQueryErrorCode errorCode, ulong id, uint leaseLife)
            : base(DnsOptionCode.LongLivedQuery)
        {
            Version = version;
            Opcode = opcode;
            ErrorCode = errorCode;
            Id = id;
            LeaseLife = leaseLife;
        }

        public ushort Version { get; private set; }
        public DnsLongLivedQueryOpcode Opcode { get; private set; }
        public DnsLongLivedQueryErrorCode ErrorCode { get; private set; }
        public ulong Id { get; private set; }
        public uint LeaseLife { get; private set; }

        public override int DataLength
        {
            get { return MinimumDataLength; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            DnsOptionLongLivedQuery castedOther = (DnsOptionLongLivedQuery)other;
            return Version.Equals(castedOther.Version) &&
                   Opcode.Equals(castedOther.Opcode) &&
                   ErrorCode.Equals(castedOther.ErrorCode) &&
                   Id.Equals(castedOther.Id) &&
                   LeaseLife.Equals(castedOther.LeaseLife);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Version, Version, Endianity.Big);
            buffer.Write(offset + Offset.Opcode, (ushort)Opcode, Endianity.Big);
            buffer.Write(offset + Offset.ErrorCode, (ushort)ErrorCode, Endianity.Big);
            buffer.Write(offset + Offset.Id, Id, Endianity.Big);
            buffer.Write(offset + Offset.LeaseLife, LeaseLife, Endianity.Big);
            offset += DataLength;
        }

        internal static DnsOptionLongLivedQuery Read(DataSegment data)
        {
            if (data.Length < MinimumDataLength)
                return null;
            ushort version = data.ReadUShort(Offset.Version, Endianity.Big);
            DnsLongLivedQueryOpcode opcode = (DnsLongLivedQueryOpcode)data.ReadUShort(Offset.Opcode, Endianity.Big);
            DnsLongLivedQueryErrorCode errorCode = (DnsLongLivedQueryErrorCode)data.ReadUShort(Offset.ErrorCode, Endianity.Big);
            ulong id = data.ReadULong(Offset.Id, Endianity.Big);
            uint leaseLife = data.ReadUInt(Offset.LeaseLife, Endianity.Big);

            return new DnsOptionLongLivedQuery(version, opcode, errorCode, id, leaseLife);
        }
    }

    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-ul.txt.
    /// <pre>
    /// +-----+-------+
    /// | bit | 0-31  |
    /// +-----+-------+
    /// | 0   | LEASE |
    /// +-----+-------+
    /// </pre>
    /// </summary>
    public class DnsOptionUpdateLease : DnsOption
    {
        public const int MinimumDataLength = sizeof(int);

        public DnsOptionUpdateLease(int lease)
            : base(DnsOptionCode.UpdateLease)
        {
            Lease = lease;
        }

        /// <summary>
        /// Indicating the lease life, in seconds, desired by the client.
        /// In Update Responses, this field contains the actual lease granted by the server.
        /// Note that the lease granted by the server may be less than, greater than, or equal to the value requested by the client.
        /// To reduce network and server load, a minimum lease of 30 minutes (1800 seconds) is recommended.
        /// Note that leases are expected to be sufficiently long as to make timer discrepancies (due to transmission latency, etc.)
        /// between a client and server negligible.
        /// Clients that expect the updated records to be relatively static may request appropriately longer leases.
        /// Servers may grant relatively longer or shorter leases to reduce network traffic due to refreshes, or reduce stale data, respectively.
        /// </summary>
        public int Lease { get; private set; }

        public override int DataLength
        {
            get { return MinimumDataLength; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            return Lease.Equals(((DnsOptionUpdateLease)other).Lease);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Lease, Endianity.Big);
        }

        internal static DnsOptionUpdateLease Read(DataSegment data)
        {
            if (data.Length < MinimumDataLength)
                return null;

            int lease = data.ReadInt(0, Endianity.Big);

            return new DnsOptionUpdateLease(lease);
        }
    }

    public sealed class DnsOptions : IEquatable<DnsOptions>
    {
        public static DnsOptions None { get { return _none; } }

        public DnsOptions(IList<DnsOption> options)
        {
            Options = options.AsReadOnly();
            NumBytes = options.Sum(option => option.Length);
        }

        public DnsOptions(params DnsOption[] options)
            : this((IList<DnsOption>)options)
        {
        }

        public ReadOnlyCollection<DnsOption> Options { get; private set; }

        public int NumBytes { get; private set; }

        public bool Equals(DnsOptions other)
        {
            return other != null &&
                   Options.SequenceEqual(other.Options);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsOptions);
        }

        private static readonly DnsOptions _none = new DnsOptions();

        internal void Write(byte[] buffer, int offset)
        {
            foreach (DnsOption option in Options)
                option.Write(buffer, ref offset);
        }

        public static DnsOptions Read(DataSegment data)
        {
            List<DnsOption> options = new List<DnsOption>();
            while (data.Length != 0)
            {
                if (data.Length < DnsOption.MinimumLength)
                    return null;
                DnsOptionCode code = (DnsOptionCode)data.ReadUShort(0, Endianity.Big);
                ushort optionDataLength = data.ReadUShort(sizeof(ushort), Endianity.Big);
                
                int optionLength = DnsOption.MinimumLength + optionDataLength;
                if (data.Length < optionLength)
                    return null;
                DnsOption option = DnsOption.CreateInstance(code, data.SubSegment(DnsOption.MinimumLength, optionDataLength));
                if (option == null)
                    return null;
                options.Add(option);

                data = data.SubSegment(optionLength, data.Length - optionLength);
            }

            return new DnsOptions(options);
        }
    }

    /// <summary>
    /// RFC 2671.
    /// <pre>
    /// 0 Or more of:
    /// +-----+---------------+
    /// | bit | 0-15          |
    /// +-----+---------------+
    /// | 0   | OPTION-CODE   |
    /// +-----+---------------+
    /// | 16  | OPTION-LENGTH |
    /// +-----+---------------+
    /// | 32  | OPTION-DATA   |
    /// | ... |               |
    /// +-----+---------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Opt)]
    public sealed class DnsResourceDataOptions : DnsResourceDataSimple, IEquatable<DnsResourceDataOptions>
    {
        public DnsResourceDataOptions(DnsOptions options)
        {
            Options = options;
        }

        public DnsOptions Options { get; private set; }

        public bool Equals(DnsResourceDataOptions other)
        {
            return other != null &&
                   Options.Equals(other.Options);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataOptions);
        }

        internal DnsResourceDataOptions()
            : this(DnsOptions.None)
        {
        }

        internal override int GetLength()
        {
            return Options.NumBytes;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            Options.Write(buffer, offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsOptions options = DnsOptions.Read(data);
            if (options == null)
                return null;
            return new DnsResourceDataOptions(options);
        }
    }
    
    /// <summary>
    ///  RFCs 2453, 2858.
    /// </summary>
    public enum AddressFamily : ushort
    {
        /// <summary>
        /// IP (IP version 4).
        /// </summary>
        IpV4 = 1,

        /// <summary>
        /// IP6 (IP version 6).
        /// </summary>
        IpV6 = 2,

        /// <summary>
        /// Network Service Access Point.
        /// </summary>
        Nsap = 3,

        /// <summary>
        /// High-Level Data Link (8-bit multidrop).
        /// </summary>
        Hdlc = 4,

        /// <summary>
        /// BBN Report 1822.
        /// </summary>
        Bbn1822 = 5,

        /// <summary>
        /// 802 (includes all 802 media plus Ethernet "canonical format").
        /// </summary>
        Media802 = 6,

        /// <summary>
        /// E.163.
        /// </summary>
        E163 = 7,

        /// <summary>
        /// E.164 (SMDS, Frame Relay, ATM).
        /// </summary>
        E164 = 8,

        /// <summary>
        /// F.69 (Telex).
        /// </summary>
        F69 = 9,

        /// <summary>
        /// X.121 (X.25, Frame Relay).
        /// </summary>
        X121 = 10,

        /// <summary>
        /// IPX.
        /// </summary>
        Ipx = 11,

        /// <summary>
        /// Appletalk.
        /// </summary>
        AppleTalk = 12,

        /// <summary>
        /// Decnet IV.
        /// </summary>
        DecnetIv = 13,	

        /// <summary>
        /// Banyan Vines.
        /// </summary>
        BanyanVines	= 14,

        /// <summary>
        /// E.164 with NSAP format subaddress.
        /// ATM Forum UNI 3.1. October 1995.
        /// Andy Malis.
        /// </summary>
        E164WithNsapFormatSubaddresses = 15,

        /// <summary>
        /// DNS (Domain Name System).
        /// </summary>
        Dns = 16,

        /// <summary>
        /// Distinguished Name.
        /// Charles Lynn.
        /// </summary>
        DistinguishedName = 17,

        /// <summary>
        /// AS Number.
        /// Charles Lynn.
        /// </summary>
        AsNumber = 18,

        /// <summary>
        /// XTP over IP version 4.
        /// Mike Saul.
        /// </summary>
        XtpOverIpV4 = 19,

        /// <summary>
        /// XTP over IP version 6.
        /// Mike Saul.
        /// </summary>
        XtpOverIpV6 = 20,

        /// <summary>
        /// XTP native mode XTP.
        /// Mike Saul.
        /// </summary>
        XtpNativeModeXtp = 21,

        /// <summary>
        /// Fibre Channel World-Wide Port Name.
        /// Mark Bakke.
        /// </summary>
        FibreChannelWorldWidePortName = 22,

        /// <summary>
        /// Fibre Channel World-Wide Node Name.
        /// Mark Bakke.
        /// </summary>
        FibreChannelWorldWideNodeName = 23,

        /// <summary>
        /// GWID.
        /// Subra Hegde.
        /// </summary>
        Gwis = 24,

        /// <summary>
        /// RFCs 4761, 6074.
        /// AFI for L2VPN information.
        /// </summary>
        AfiForL2VpnInformation = 25,

        /// <summary>
        /// EIGRP Common Service Family.
        /// Donnie Savage.
        /// </summary>
	    EigrpCommonServiceFamily = 16384,

        /// <summary>
        /// EIGRP IPv4 Service Family.
        /// Donnie Savage.
        /// </summary>
        EigrpIpV4ServiceFamily = 16385,

        /// <summary>
        /// EIGRP IPv6 Service Family.
        /// Donnie Savage.
        /// </summary>
	    EigrpIpV6ServiceFamily = 16386,

        /// <summary>
        /// LISP Canonical Address Format (LCAF).
        /// David Meyer.
        /// </summary>
        LispCanonicalAddressFormat = 16387,
    }

    /// <summary>
    /// RFC 3123.
    /// <pre>
    /// +-----+--------+---+-----------+
    /// | bit | 0-7    | 8 | 9-15      |
    /// +-----+--------+---+-----------+
    /// | 0   | ADDRESSFAMILY          |
    /// +-----+--------+---+-----------+
    /// | 16  | PREFIX | N | AFDLENGTH |
    /// +-----+--------+---+-----------+
    /// | 32  | AFDPART                |
    /// | ... |                        |
    /// +-----+------------------------+
    /// </pre>
    /// </summary>
    public class DnsAddressPrefix : IEquatable<DnsAddressPrefix>
    {
        private static class Offset
        {
            public const int AddressFamily = 0;
            public const int PrefixLength = AddressFamily + sizeof(ushort);
            public const int Negation = PrefixLength + sizeof(byte);
            public const int AddressFamilyDependentPartLength = Negation;
            public const int AddressFamilyDependentPart = AddressFamilyDependentPartLength + sizeof(byte);
        }

        public const int MinimumLength = Offset.AddressFamilyDependentPart;

        private static class Mask
        {
            public const byte Negation = 0x80;
            public const byte AddressFamilyDependentPartLength = 0x7F;
        }

        public const int AddressFamilyDependentPartMaxLength = (1 << 7) - 1;


        public DnsAddressPrefix(AddressFamily addressFamily, byte prefixLength, bool negation, DataSegment addressFamilyDependentPart)
        {
            if (addressFamilyDependentPart.Length > AddressFamilyDependentPartMaxLength)
                throw new ArgumentOutOfRangeException("addressFamilyDependentPart", addressFamilyDependentPart, "Cannot be longer than " + AddressFamilyDependentPartMaxLength);

            AddressFamily = addressFamily;
            PrefixLength = prefixLength;
            Negation = negation;
            AddressFamilyDependentPart = addressFamilyDependentPart;
        }

        public AddressFamily AddressFamily { get; private set; }

        /// <summary>
        /// Prefix length.
        /// Upper and lower bounds and interpretation of this value are address family specific.
        /// 
        /// For IPv4, specifies the number of bits of the IPv4 address starting at the most significant bit.
        /// Legal values range from 0 to 32.
        /// 
        /// For IPv6, specifies the number of bits of the IPv6 address starting at the most significant bit.
        /// Legal values range from 0 to 128.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Negation flag, indicates the presence of the "!" character in the textual format.
        /// </summary>
        public bool Negation { get; private set; }

        /// <summary>
        /// For IPv4, the encoding follows the encoding specified for the A RR by RFC 1035.
        /// Trailing zero octets do not bear any information (e.g., there is no semantic difference between 10.0.0.0/16 and 10/16) in an address prefix,
        /// so the shortest possible AddressFamilyDependentPart can be used to encode it.
        /// However, for DNSSEC (RFC 2535) a single wire encoding must be used by all.
        /// Therefore the sender must not include trailing zero octets in the AddressFamilyDependentPart regardless of the value of PrefixLength.
        /// This includes cases in which AddressFamilyDependentPart length times 8 results in a value less than PrefixLength.
        /// The AddressFamilyDependentPart is padded with zero bits to match a full octet boundary.
        /// An IPv4 AddressFamilyDependentPart has a variable length of 0 to 4 octets.
        /// 
        /// For IPv6, the 128 bit IPv6 address is encoded in network byte order (high-order byte first).
        /// The sender must not include trailing zero octets in the AddressFamilyDependentPart regardless of the value of PrefixLength.
        /// This includes cases in which AddressFamilyDependentPart length times 8 results in a value less than PrefixLength.
        /// The AddressFamilyDependentPart is padded with zero bits to match a full octet boundary.
        /// An IPv6 AddressFamilyDependentPart has a variable length of 0 to 16 octets.
        /// </summary>
        public DataSegment AddressFamilyDependentPart { get; private set; }

        public int Length
        {
            get { return MinimumLength + AddressFamilyDependentPart.Length; }
        }

        public bool Equals(DnsAddressPrefix other)
        {
            return other != null &&
                   AddressFamily.Equals(other.AddressFamily) &&
                   PrefixLength.Equals(other.PrefixLength) &&
                   Negation.Equals(other.Negation) &&
                   AddressFamilyDependentPart.Equals(other.AddressFamilyDependentPart);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsAddressPrefix);
        }

        public static DnsAddressPrefix Read(DataSegment data)
        {
            if (data.Length < MinimumLength)
                return null;
            AddressFamily addressFamily = (AddressFamily)data.ReadUShort(Offset.AddressFamily, Endianity.Big);
            byte prefixLength = data[Offset.PrefixLength];
            bool negation = data.ReadBool(Offset.Negation, Mask.Negation);
            byte addressFamilyDependentPartLength = (byte)(data[Offset.AddressFamilyDependentPartLength] & Mask.AddressFamilyDependentPartLength);
            
            if (data.Length < MinimumLength + addressFamilyDependentPartLength)
                return null;
            DataSegment addressFamilyDependentPart = data.SubSegment(Offset.AddressFamilyDependentPart, addressFamilyDependentPartLength);

            return new DnsAddressPrefix(addressFamily, prefixLength, negation, addressFamilyDependentPart);
        }

        public void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.AddressFamily, (ushort)AddressFamily, Endianity.Big);
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.Negation, (byte)((Negation ? Mask.Negation : 0) | AddressFamilyDependentPart.Length));
            AddressFamilyDependentPart.Write(buffer, offset + Offset.AddressFamilyDependentPart);

            offset += MinimumLength + AddressFamilyDependentPart.Length;
        }
    }

    /// <summary>
    /// RFC 3123.
    /// <pre>
    /// 0 Or more of:
    /// +-----+--------+---+-----------+
    /// | bit | 0-7    | 8 | 9-15      |
    /// +-----+--------+---+-----------+
    /// | 0   | ADDRESSFAMILY          |
    /// +-----+--------+---+-----------+
    /// | 16  | PREFIX | N | AFDLENGTH |
    /// +-----+--------+---+-----------+
    /// | 32  | AFDPART                |
    /// | ... |                        |
    /// +-----+------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Apl)]
    public sealed class DnsResourceDataAddressPrefixList: DnsResourceDataSimple, IEquatable<DnsResourceDataAddressPrefixList>
    {
        public DnsResourceDataAddressPrefixList(IList<DnsAddressPrefix> items)
        {
            Items = items.AsReadOnly();
        }

        public DnsResourceDataAddressPrefixList(params DnsAddressPrefix[] items)
            : this((IList<DnsAddressPrefix>)items)
        {
            Length = items.Sum(item => item.Length);
        }

        public ReadOnlyCollection<DnsAddressPrefix> Items { get; private set; }
        public int Length { get; private set; }

        public bool Equals(DnsResourceDataAddressPrefixList other)
        {
            return other != null &&
                   Items.SequenceEqual(other.Items);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataAddressPrefixList);
        }

        internal DnsResourceDataAddressPrefixList()
            : this(new DnsAddressPrefix[0])
        {
        }

        internal override int GetLength()
        {
            return Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            foreach (DnsAddressPrefix item in Items)
                item.Write(buffer, ref offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DnsAddressPrefix> items = new List<DnsAddressPrefix>();
            while (data.Length != 0)
            {
                DnsAddressPrefix item = DnsAddressPrefix.Read(data);
                if (item == null)
                    return null;
                items.Add(item);
                data = data.SubSegment(item.Length, data.Length - item.Length);
            }

            return new DnsResourceDataAddressPrefixList(items);
        }
    }

    public enum DnsDigestType : byte
    {
        /// <summary>
        /// RFC 3658.
        /// SHA-1.
        /// </summary>
        Sha1 = 1,

        /// <summary>
        /// RFC 4509.
        /// SHA-256.
        /// </summary>
        Sha256 = 2,

        /// <summary>
        /// RFC 5933.
        /// GOST R 34.11-94.
        /// </summary>
        GostR341194 = 3,
    }

    /// <summary>
    /// RFC 3658.
    /// <pre>
    /// 0 Or more of:
    /// +-----+---------+-----------+-------------+
    /// | bit | 0-15    | 16-23     | 24-31       |
    /// +-----+---------+-----------+-------------+
    /// | 0   | key tag | algorithm | Digest type |
    /// +-----+---------+-----------+-------------+
    /// | 32  | digest                            |
    /// | ... |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Ds)]
    [DnsTypeRegistration(Type = DnsType.Cds)]
    public sealed class DnsResourceDataDelegationSigner : DnsResourceDataSimple, IEquatable<DnsResourceDataDelegationSigner>
    {
        public static class Offset
        {
            public const int KeyTag = 0;
            public const int Algorithm = KeyTag + sizeof(ushort);
            public const int DigestType = Algorithm + sizeof(byte);
            public const int Digest = DigestType + sizeof(byte);
        }

        public const int ConstPartLength = Offset.Digest;

        public DnsResourceDataDelegationSigner(ushort keyTag, DnsAlgorithm algorithm, DnsDigestType digestType, DataSegment digest)
        {
            KeyTag = keyTag;
            Algorithm = algorithm;
            DigestType = digestType;
            Digest = digest;
        }

        /// <summary>
        /// Lists the key tag of the DNSKEY RR referred to by the DS record.
        /// The Key Tag used by the DS RR is identical to the Key Tag used by RRSIG RRs.
        /// Calculated as specified in RFC 2535.
        /// </summary>
        public ushort KeyTag { get; private set; }

        /// <summary>
        /// Algorithm must be allowed to sign DNS data.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// An identifier for the digest algorithm used.
        /// </summary>
        public DnsDigestType DigestType { get; private set; }

        /// <summary>
        /// Calculated over the canonical name of the delegated domain name followed by the whole RDATA of the KEY record (all four fields).
        /// digest = hash(canonical FQDN on KEY RR | KEY_RR_rdata)
        /// KEY_RR_rdata = Flags | Protocol | Algorithm | Public Key
        /// </summary>
        public DataSegment Digest { get; private set; }


        public bool Equals(DnsResourceDataDelegationSigner other)
        {
            return other != null &&
                   KeyTag.Equals(other.KeyTag) &&
                   Algorithm.Equals(other.Algorithm) &&
                   DigestType.Equals(other.DigestType) &&
                   Digest.Equals(other.Digest);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataDelegationSigner);
        }

        internal DnsResourceDataDelegationSigner()
            : this(0, DnsAlgorithm.None, DnsDigestType.Sha1, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstPartLength + Digest.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.KeyTag, KeyTag, Endianity.Big);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            buffer.Write(offset + Offset.DigestType, (byte)DigestType);
            Digest.Write(buffer, offset + Offset.Digest);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            ushort keyTag = data.ReadUShort(Offset.KeyTag, Endianity.Big);
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DnsDigestType digestType = (DnsDigestType)data[Offset.DigestType];
            DataSegment digest = data.SubSegment(Offset.Digest, data.Length - ConstPartLength);

            return new DnsResourceDataDelegationSigner(keyTag, algorithm, digestType, digest);
        }
    }

    /// <summary>
    /// RFC 4255.
    /// </summary>
    public enum DnsFingerprintType : byte
    {
        /// <summary>
        /// RFC 4255.
        /// </summary>
        Sha1 = 1,
    }

    /// <summary>
    /// RFC 4255.
    /// Describes the algorithm of the public key.
    /// </summary>
    public enum DnsFingerprintPublicKeyAlgorithm
    {
        Rsa = 1,
        Dss = 2,
    }

    /// <summary>
    /// RFC 4255.
    /// <pre>
    /// +-----+-----------+-----------+
    /// | bit | 0-7       | 8-15      |
    /// +-----+-----------+-----------+
    /// | 0   | algorithm | fp type   |
    /// +-----+-----------+-----------+
    /// | 16  | fingerprint           |
    /// | ... |                       |
    /// +-----+-----------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.SshFp)]
    public sealed class DnsResourceDataSshFingerprint : DnsResourceDataSimple, IEquatable<DnsResourceDataSshFingerprint>
    {
        public static class Offset
        {
            public const int Algorithm = 0;
            public const int FingerprintType = Algorithm + sizeof(byte);
            public const int Fingerprint = FingerprintType + sizeof(byte);
        }

        public const int ConstPartLength = Offset.Fingerprint;

        public DnsResourceDataSshFingerprint(DnsFingerprintPublicKeyAlgorithm algorithm, DnsFingerprintType fingerprintType, DataSegment fingerprint)
        {
            Algorithm = algorithm;
            FingerprintType = fingerprintType;
            Fingerprint = fingerprint;
        }

        /// <summary>
        /// Describes the algorithm of the public key.
        /// </summary>
        public DnsFingerprintPublicKeyAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// Describes the message-digest algorithm used to calculate the fingerprint of the public key.
        /// </summary>
        public DnsFingerprintType FingerprintType { get; private set; }

        /// <summary>
        /// The fingerprint is calculated over the public key blob.
        /// The message-digest algorithm is presumed to produce an opaque octet string output, which is placed as-is in the RDATA fingerprint field.
        /// </summary>
        public DataSegment Fingerprint { get; private set; }

        public bool Equals(DnsResourceDataSshFingerprint other)
        {
            return other != null &&
                   Algorithm.Equals(other.Algorithm) &&
                   FingerprintType.Equals(other.FingerprintType) &&
                   Fingerprint.Equals(other.Fingerprint);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataSshFingerprint);
        }

        internal DnsResourceDataSshFingerprint()
            : this(DnsFingerprintPublicKeyAlgorithm.Rsa, DnsFingerprintType.Sha1, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstPartLength + Fingerprint.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            buffer.Write(offset + Offset.FingerprintType, (byte)FingerprintType);
            Fingerprint.Write(buffer, offset + Offset.Fingerprint);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsFingerprintPublicKeyAlgorithm algorithm = (DnsFingerprintPublicKeyAlgorithm)data[Offset.Algorithm];
            DnsFingerprintType fingerprintType = (DnsFingerprintType)data[Offset.FingerprintType];
            DataSegment fingerprint = data.SubSegment(Offset.Fingerprint, data.Length - ConstPartLength);

            return new DnsResourceDataSshFingerprint(algorithm, fingerprintType, fingerprint);
        }
    }

    public abstract class DnsGateway : IEquatable<DnsGateway>
    {
        public static DnsGatewayNone None { get { return _none; } }

        public abstract DnsGatewayType Type { get; }

        public abstract int Length { get; }

        public abstract bool Equals(DnsGateway other);

        public override bool Equals(object obj)
        {
 	         return Equals(obj as DnsGateway);
        }

        internal abstract void Write(byte[] buffer, int offset);

        internal static DnsGateway CreateInstance(DnsGatewayType gatewayType, DnsDatagram dns, int offsetInDns, int length)
        {
            switch (gatewayType)
            {
                case DnsGatewayType.None:
                    return None;

                case DnsGatewayType.IpV4:
                    if (length < IpV4Address.SizeOf)
                        return null;
                    return new DnsGatewayIpV4(dns.ReadIpV4Address(offsetInDns, Endianity.Big));

                case DnsGatewayType.IpV6:
                    if (length < IpV6Address.SizeOf)
                        return null;
                    return new DnsGatewayIpV6(dns.ReadIpV6Address(offsetInDns, Endianity.Big));

                case DnsGatewayType.DomainName:
                    DnsDomainName domainName;
                    int numBytesRead;
                    if (!DnsDomainName.TryParse(dns, offsetInDns, length, out domainName, out numBytesRead))
                        return null;
                    return new DnsGatewayDomainName(domainName);

                default:
                    return null;
            }
        }

        private static DnsGatewayNone _none = new DnsGatewayNone();
    }

    public class DnsGatewayNone : DnsGateway, IEquatable<DnsGatewayNone>
    {
        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.None; }
        }

        public override int Length
        {
            get { return 0; }
        }

        public bool Equals(DnsGatewayNone other)
        {
            return other != null;
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayNone);
        }

        internal DnsGatewayNone()
        {
        }

        internal override void Write(byte[] buffer, int offset)
        {
        }
    }

    public class DnsGatewayIpV4 : DnsGateway, IEquatable<DnsGatewayIpV4>
    {
        public DnsGatewayIpV4(IpV4Address value)
        {
            Value = value;
        }

        public IpV4Address Value { get; private set; }

        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.IpV4; }
        }

        public override int Length
        {
            get { return IpV4Address.SizeOf; }
        }

        public bool Equals(DnsGatewayIpV4 other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayIpV4);
        }

        internal override void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, Value, Endianity.Big);
        }
    }

    public class DnsGatewayIpV6 : DnsGateway, IEquatable<DnsGatewayIpV6>
    {
        public DnsGatewayIpV6(IpV6Address value)
        {
            Value = value;
        }

        public IpV6Address Value { get; private set; }

        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.IpV6; }
        }

        public override int Length
        {
            get { return IpV6Address.SizeOf; }
        }

        public bool Equals(DnsGatewayIpV6 other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayIpV6);
        }

        internal override void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, Value, Endianity.Big);
        }
    }

    public class DnsGatewayDomainName : DnsGateway, IEquatable<DnsGatewayDomainName>
    {
        public DnsGatewayDomainName(DnsDomainName value)
        {
            Value = value;
        }

        public DnsDomainName Value { get; private set; }

        public override DnsGatewayType Type
        {
            get { return DnsGatewayType.DomainName; }
        }

        public override int Length
        {
            get { return Value.NonCompressedLength; }
        }

        public bool Equals(DnsGatewayDomainName other)
        {
            return other != null &&
                   Value.Equals(other.Value);
        }

        public override bool Equals(DnsGateway other)
        {
            return Equals(other as DnsGatewayDomainName);
        }

        internal override void Write(byte[] buffer, int offset)
        {
            Value.WriteUncompressed(buffer, offset);
        }
    }

    /// <summary>
    /// Indicates the format of the information that is stored in the gateway field.
    /// </summary>
    public enum DnsGatewayType : byte
    {
        /// <summary>
        /// No gateway is present.
        /// </summary>
        None = 0,

        /// <summary>
        /// A 4-byte IPv4 address is present.
        /// </summary>
        IpV4 = 1,

        /// <summary>
        /// A 16-byte IPv6 address is present.
        /// </summary>
        IpV6 = 2,
   
        /// <summary>
        /// A wire-encoded domain name is present.
        /// The wire-encoded format is self-describing, so the length is implicit.
        /// The domain name must not be compressed.
        /// </summary>
        DomainName = 3,
    }

    /// <summary>
    /// Identifies the public key's cryptographic algorithm and determines the format of the public key field.
    /// </summary>
    public enum DnsPublicKeyAlgorithm : byte
    {
        /// <summary>
        /// Indicates that no key is present.
        /// </summary>
        None = 0,

        /// <summary>
        /// A DSA key is present, in the format defined in RFC 2536.
        /// </summary>
        Dsa = 1,

        /// <summary>
        /// A RSA key is present, in the format defined in RFC 3110 with the following changes:
        /// The earlier definition of RSA/MD5 in RFC 2065 limited the exponent and modulus to 2552 bits in length.
        /// RFC 3110 extended that limit to 4096 bits for RSA/SHA1 keys
        /// The IPSECKEY RR imposes no length limit on RSA public keys, other than the 65535 octet limit imposed by the two-octet length encoding.
        /// This length extension is applicable only to IPSECKEY; it is not applicable to KEY RRs. 
        /// </summary>
        Rsa = 2,
    }

    /// <summary>
    /// RFC 4025.
    /// <pre>
    /// +-----+--------------+
    /// | bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | precedence   |
    /// +-----+--------------+
    /// | 8   | gateway type |
    /// +-----+--------------+
    /// | 16  | algorithm    |
    /// +-----+--------------+
    /// | 24  | gateway      |
    /// | ... |              |
    /// +-----+--------------+
    /// |     | public key   |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.IpSecKey)]
    public sealed class DnsResourceDataIpSecKey : DnsResourceDataNoCompression, IEquatable<DnsResourceDataIpSecKey>
    {
        public static class Offset
        {
            public const int Precedence = 0;
            public const int GatewayType = Precedence + sizeof(byte);
            public const int Algorithm = GatewayType + sizeof(byte);
            public const int Gateway = Algorithm + sizeof(byte);
        }

        public const int ConstPartLength = Offset.Gateway;

        public DnsResourceDataIpSecKey(byte precedence, DnsGateway gateway, DnsPublicKeyAlgorithm algorithm, DataSegment publicKey)
        {
            Precedence = precedence;
            Gateway = gateway;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// Precedence for this record.
        /// Gateways listed in IPSECKEY records with lower precedence are to be attempted first.
        /// Where there is a tie in precedence, the order should be non-deterministic.
        /// </summary>
        public byte Precedence { get; private set; }

        /// <summary>
        /// Indicates the format of the information that is stored in the gateway field.
        /// </summary>
        public DnsGatewayType GatewayType { get { return Gateway.Type; } }

        /// <summary>
        /// Indicates a gateway to which an IPsec tunnel may be created in order to reach the entity named by this resource record.
        /// </summary>
        public DnsGateway Gateway { get; private set;}

        /// <summary>
        /// Identifies the public key's cryptographic algorithm and determines the format of the public key field.
        /// </summary>
        public DnsPublicKeyAlgorithm Algorithm { get; private set;} 

        /// <summary>
        /// Contains the algorithm-specific portion of the KEY RR RDATA.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        public bool Equals(DnsResourceDataIpSecKey other)
        {
            return other != null &&
                   Precedence.Equals(other.Precedence) &&
                   Gateway.Equals(other.Gateway) &&
                   Algorithm.Equals(other.Algorithm) &&
                   PublicKey.Equals(other.PublicKey);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataIpSecKey);
        }

        internal DnsResourceDataIpSecKey()
            : this(0, DnsGateway.None, DnsPublicKeyAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstPartLength + Gateway.Length + PublicKey.Length;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Precedence, Precedence);
            buffer.Write(offset + Offset.GatewayType, (byte)GatewayType);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            Gateway.Write(buffer, offset + Offset.Gateway);
            PublicKey.Write(buffer, offset + ConstPartLength + Gateway.Length);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstPartLength)
                return null;

            byte precedence = dns[offsetInDns + Offset.Precedence];
            DnsGatewayType gatewayType = (DnsGatewayType)dns[offsetInDns + Offset.GatewayType];
            DnsPublicKeyAlgorithm algorithm = (DnsPublicKeyAlgorithm)dns[offsetInDns + Offset.Algorithm];
            DnsGateway gateway = DnsGateway.CreateInstance(gatewayType, dns, offsetInDns + Offset.Gateway, length - ConstPartLength);
            if (gateway == null)
                return null;
            DataSegment publicKey = dns.SubSegment(offsetInDns + ConstPartLength + gateway.Length, length - ConstPartLength - gateway.Length);

            return new DnsResourceDataIpSecKey(precedence, gateway, algorithm, publicKey);
        }
    }

    /// <summary>
    /// RFCs 4034, 5155.
    /// </summary>
    internal class DnsTypeBitmaps : IEquatable<DnsTypeBitmaps>
    {
        private const int MaxTypeBitmapsLength = 256 * (2 + 32);

        public DnsTypeBitmaps(IEnumerable<DnsType> typesExist)
        {
            TypesExist = typesExist.Distinct().ToList();
            TypesExist.Sort();
        }

        public bool Contains(DnsType dnsType)
        {
            return TypesExist.BinarySearch(dnsType) >= 0;
        }

        public List<DnsType> TypesExist { get; private set;}

        public bool Equals(DnsTypeBitmaps other)
        {
            return other != null &&
                   TypesExist.SequenceEqual(other.TypesExist);
        }

        public override bool  Equals(object obj)
        {
            return Equals(obj as DnsTypeBitmaps);
        }

        public int GetLength()
        {
            int length = 0;
            int previousWindow = -1;
            int maxBit = -1;
            foreach (DnsType dnsType in TypesExist)
            {
                byte window = (byte)(((ushort)dnsType) >> 8);
                if (window > previousWindow)
                {
                    if (maxBit != -1)
                        length += 2 + maxBit / 8 + 1;
                    previousWindow = window;
                    maxBit = -1;
                }

                byte bit = (byte)dnsType;
                maxBit = Math.Max(bit, maxBit);
            }

            if (maxBit != -1)
                length += 2 + maxBit / 8 + 1;

            return length;
        }

        public int Write(byte[] buffer, int offset)
        {
            int originalOffset = offset;
            int previousWindow = -1;
            int maxBit = -1;
            byte[] windowBitmap = null;
            foreach (DnsType dnsType in TypesExist)
            {
                byte window = (byte)(((ushort)dnsType) >> 8);
                if (window > previousWindow)
                {
                    if (maxBit != -1)
                        WriteBitmap(buffer, ref offset, (byte)previousWindow, maxBit, windowBitmap);
                    previousWindow = window;
                    windowBitmap = new byte[32];
                    maxBit = -1;
                }

                byte bit = (byte)dnsType;
                maxBit = Math.Max(bit, maxBit);
                windowBitmap[bit / 8] |= (byte)(1 << (7 - bit % 8));
            }

            if (maxBit != -1)
                WriteBitmap(buffer, ref offset, (byte)previousWindow, maxBit, windowBitmap);

            return offset - originalOffset;
        }

        public static DnsTypeBitmaps CreateInstance(byte[] buffer, int offset, int length)
        {
            if (length > MaxTypeBitmapsLength)
                return null;

            List<DnsType> typesExist = new List<DnsType>();
            while (length != 0)
            {
                if (length < 3)
                    return null;
                byte window = buffer[offset++];
                byte bitmapLength = buffer[offset++];
                length -= 2;

                if (bitmapLength < 1 || bitmapLength > 32 || length < bitmapLength)
                    return null;
                for (int i = 0; i != bitmapLength; ++i)
                {
                    byte bits = buffer[offset++];
                    int bitIndex = 0;
                    while (bits != 0)
                    {
                        if ((byte)(bits & 0x80) == 0x80)
                        {
                            typesExist.Add((DnsType)((window << 8) + 8 * i + bitIndex));
                        }
                        bits <<= 1;
                        ++bitIndex;
                    }
                }
                length -= bitmapLength;
            }

            return new DnsTypeBitmaps(typesExist);
        }

        private static void WriteBitmap(byte[] buffer, ref int offset, byte window, int maxBit, byte[] windowBitmap)
        {
            buffer.Write(ref offset, window);
            byte numBytes = (byte)(maxBit / 8 + 1);
            buffer.Write(ref offset, numBytes);
            DataSegment data = new DataSegment(windowBitmap, 0, numBytes);
            data.Write(buffer, ref offset);
        }
    }

    /// <summary>
    /// RFC 4034.
    /// <pre>
    /// +------------------+
    /// | next domain name |
    /// |                  |
    /// +------------------+
    /// | type bit map     |
    /// |                  |
    /// +------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NSec)]
    public sealed class DnsResourceDataNextDomainSecure : DnsResourceDataNoCompression, IEquatable<DnsResourceDataNextDomainSecure>
    {
        public DnsResourceDataNextDomainSecure(DnsDomainName nextDomainName, IEnumerable<DnsType> typesExist)
            : this(nextDomainName, new DnsTypeBitmaps(typesExist))
        {
        }

        /// <summary>
        /// Contains the next owner name (in the canonical ordering of the zone) that has authoritative data or contains a delegation point NS RRset;
        /// The value of the Next Domain Name field in the last NSEC record in the zone is the name of the zone apex (the owner name of the zone's SOA RR).
        /// This indicates that the owner name of the NSEC RR is the last name in the canonical ordering of the zone.
        ///
        /// Owner names of RRsets for which the given zone is not authoritative (such as glue records) must not be listed in the Next Domain Name
        /// unless at least one authoritative RRset exists at the same owner name.
        /// </summary>
        public DnsDomainName NextDomainName { get; private set; }

        /// <summary>
        /// Identifies the RRset types that exist at the NSEC RR's owner name.
        /// Ordered by the DnsType value.
        /// </summary>
        public ReadOnlyCollection<DnsType> TypesExist { get { return _typeBitmaps.TypesExist.AsReadOnly(); } }

        public bool IsTypePresentForOwner(DnsType dnsType)
        {
            return _typeBitmaps.Contains(dnsType);
        }

        public bool Equals(DnsResourceDataNextDomainSecure other)
        {
            return other != null &&
                   NextDomainName.Equals(other.NextDomainName) &&
                   _typeBitmaps.Equals(other._typeBitmaps);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNextDomainSecure);
        }

        internal DnsResourceDataNextDomainSecure()
            : this(DnsDomainName.Root, new DnsType[0])
        {
        }

        internal override int GetLength()
        {
            return NextDomainName.NonCompressedLength + _typeBitmaps.GetLength();
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            NextDomainName.WriteUncompressed(buffer, offset);
            int nextDomainNameLength = NextDomainName.NonCompressedLength;
            return nextDomainNameLength + _typeBitmaps.Write(buffer, offset + nextDomainNameLength);
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName nextDomainName;
            int nextDomainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out nextDomainName, out nextDomainNameLength))
                return null;
            offsetInDns += nextDomainNameLength;
            length -= nextDomainNameLength;

            DnsTypeBitmaps typeBitmaps = DnsTypeBitmaps.CreateInstance(dns.Buffer, dns.StartOffset + offsetInDns, length);
            if (typeBitmaps == null)
                return null;

            return new DnsResourceDataNextDomainSecure(nextDomainName, typeBitmaps);
        }

        private DnsResourceDataNextDomainSecure(DnsDomainName nextDomainName, DnsTypeBitmaps typeBitmaps)
        {
            NextDomainName = nextDomainName;
            _typeBitmaps = typeBitmaps;
        }

        private readonly DnsTypeBitmaps _typeBitmaps;
    }

    /// <summary>
    /// RFC 4034.
    /// <pre>
    /// +-----+----------+----------+----------+--------------------+
    /// | bit | 0-6      | 7        | 8-14     | 15                 |
    /// +-----+----------+----------+----------+--------------------+
    /// | 0   | Reserved | Zone Key | Reserved | Secure Entry Point |
    /// +-----+----------+----------+----------+--------------------+
    /// | 16  | Protocol            | Algorithm                     |
    /// +-----+---------------------+-------------------------------+
    /// | 32  | Public Key                                          |
    /// | ... |                                                     |
    /// +-----+---------------------+-------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.DnsKey)]
    public sealed class DnsResourceDataDnsKey : DnsResourceDataSimple, IEquatable<DnsResourceDataDnsKey>
    {
        public const byte ProtocolValue = 3;

        private static class Offset
        {
            public const int ZoneKey = 0;
            public const int SecureEntryPoint = 1;
            public const int Protocol = sizeof(ushort);
            public const int Algorithm = Protocol + sizeof(byte);
            public const int PublicKey = Algorithm + sizeof(byte);
        }

        private static class Mask
        {
            public const byte ZoneKey = 0x01;
            public const byte SecureEntryPoint = 0x01;
        }

        private const int ConstantPartLength = Offset.PublicKey;

        public DnsResourceDataDnsKey(bool zoneKey, bool secureEntryPoint, byte protocol,  DnsAlgorithm algorithm, DataSegment publicKey)
        {
            ZoneKey = zoneKey;
            SecureEntryPoint = secureEntryPoint;
            Protocol = protocol;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// If true, the DNSKEY record holds a DNS zone key, and the DNSKEY RR's owner name must be the name of a zone.
        /// If false, then the DNSKEY record holds some other type of DNS public key and must not be used to verify RRSIGs that cover RRsets.
        /// </summary>
        public bool ZoneKey { get; private set; }

        /// <summary>
        /// RFC 3757.
        /// If true, then the DNSKEY record holds a key intended for use as a secure entry point.
        /// This flag is only intended to be a hint to zone signing or debugging software as to the intended use of this DNSKEY record;
        /// validators must not alter their behavior during the signature validation process in any way based on the setting of this bit.
        /// This also means that a DNSKEY RR with the SEP bit set would also need the Zone Key flag set in order to be able to generate signatures legally.
        /// A DNSKEY RR with the SEP set and the Zone Key flag not set MUST NOT be used to verify RRSIGs that cover RRsets.
        /// </summary>
        public bool SecureEntryPoint { get; private set; }

        /// <summary>
        ///  Musthave value 3, and the DNSKEY RR MUST be treated as invalid during signature verification if it is found to be some value other than 3.
        /// </summary>
        public byte Protocol { get; private set; }

        /// <summary>
        /// Identifies the public key's cryptographic algorithm and determines the format of the Public Key field.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// The public key material.
        /// The format depends on the algorithm of the key being stored.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        public bool Equals(DnsResourceDataDnsKey other)
        {
            return other != null &&
                   ZoneKey.Equals(other.ZoneKey) &&
                   SecureEntryPoint.Equals(other.SecureEntryPoint) &&
                   Protocol.Equals(other.Protocol) &&
                   Algorithm.Equals(other.Algorithm) &&
                   PublicKey.Equals(other.PublicKey);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataDnsKey);
        }

        internal DnsResourceDataDnsKey()
            : this(false, false, ProtocolValue, DnsAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + PublicKey.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            byte flagsByte0 = 0;
            if (ZoneKey)
                flagsByte0 |= Mask.ZoneKey;
            buffer.Write(offset + Offset.ZoneKey, flagsByte0);

            byte flagsByte1 = 0;
            if (SecureEntryPoint)
                flagsByte1 |= Mask.SecureEntryPoint;
            buffer.Write(offset + Offset.SecureEntryPoint, flagsByte1);

            buffer.Write(offset + Offset.Protocol, Protocol);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            PublicKey.Write(buffer, offset + Offset.PublicKey);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            bool zoneKey = data.ReadBool(Offset.ZoneKey, Mask.ZoneKey);
            bool secureEntryPoint = data.ReadBool(Offset.SecureEntryPoint, Mask.SecureEntryPoint);
            byte protocol = data[Offset.Protocol];
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DataSegment publicKey = data.SubSegment(Offset.PublicKey, data.Length - ConstantPartLength);

            return new DnsResourceDataDnsKey(zoneKey, secureEntryPoint, protocol, algorithm, publicKey);
        }
    }

    /// <summary>
    /// RFC 5155.
    /// </summary>
    [Flags]
    public enum DnsSecNSec3Flags : byte
    {
        None = 0x00,

        /// <summary>
        /// RFC 5155.
        /// Indicates whether this NSEC3 RR may cover unsigned delegations.
        /// If set, the NSEC3 record covers zero or more unsigned delegations.
        /// If clear, the NSEC3 record covers zero unsigned delegations.
        /// </summary>
        OptOut = 0x01,
    }

    /// <summary>
    /// RFC 5155.
    /// </summary>
    public enum DnsSecNSec3HashAlgorithm : byte
    {
        /// <summary>
        /// RFC 5155.
        /// </summary>
        Sha1 = 0x01,
    }

    /// <summary>
    /// RFC 5155.
    /// <pre>
    /// +-----+-------------+----------+--------+------------+
    /// | bit | 0-7         | 8-14     | 15     | 16-31      |
    /// +-----+-------------+----------+--------+------------+
    /// | 0   | Hash Alg    | Reserved | OptOut | Iterations |
    /// +-----+-------------+----------+--------+------------+
    /// | 32  | Salt Length | Salt                           |
    /// +-----+-------------+                                |
    /// | ... |                                              |
    /// +-----+----------------------------------------------+
    /// | ... | ...                                          |
    /// +-----+----------------------------------------------+
    public abstract class DnsResourceDataNextDomainSecure3Base : DnsResourceDataSimple
    {
        private static class Offset
        {
            public const int HashAlgorithm = 0;
            public const int Flags = HashAlgorithm + sizeof(byte);
            public const int Iterations = Flags + sizeof(byte);
            public const int SaltLength = Iterations + sizeof(ushort);
            public const int Salt = SaltLength + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.Salt;

        /// <summary>
        /// Identifies the cryptographic hash algorithm used to construct the hash-value.
        /// </summary>
        public DnsSecNSec3HashAlgorithm HashAlgorithm { get; private set; }

        /// <summary>
        /// Can be used to indicate different processing.
        /// All undefined flags must be zero.
        /// </summary>
        public DnsSecNSec3Flags Flags { get; private set; }

        /// <summary>
        /// Defines the number of additional times the hash function has been performed.
        /// More iterations result in greater resiliency of the hash value against dictionary attacks, but at a higher computational cost for both the server and resolver.
        /// </summary>
        public ushort Iterations { get; private set; }

        /// <summary>
        /// Appended to the original owner name before hashing in order to defend against pre-calculated dictionary attacks.
        /// </summary>
        public DataSegment Salt { get; private set; }

        internal bool EqualsParameters(DnsResourceDataNextDomainSecure3Base other)
        {
            return other != null &&
                   HashAlgorithm.Equals(other.HashAlgorithm) &&
                   Flags.Equals(other.Flags) &&
                   Iterations.Equals(other.Iterations) &&
                   Salt.Equals(other.Salt);
        }

        internal DnsResourceDataNextDomainSecure3Base(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt)
        {
            if (salt.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("salt", salt.Length, string.Format("Cannot bigger than {0}.", byte.MaxValue));

            HashAlgorithm = hashAlgorithm;
            Flags = flags;
            Iterations = iterations;
            Salt = salt;
        }

        internal int ParametersLength { get { return GetParametersLength(Salt.Length); } }

        internal static int GetParametersLength(int saltLength)
        {
            return ConstantPartLength + saltLength;
        }

        internal void WriteParameters(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.HashAlgorithm, (byte)HashAlgorithm);
            buffer.Write(offset + Offset.Flags, (byte)Flags);
            buffer.Write(offset + Offset.Iterations, Iterations, Endianity.Big);
            buffer.Write(offset + Offset.SaltLength, (byte)Salt.Length);
            Salt.Write(buffer, offset + Offset.Salt);
        }

        internal static bool TryReadParameters(DataSegment data, out DnsSecNSec3HashAlgorithm hashAlgorithm, out DnsSecNSec3Flags flags, out ushort iterations, out DataSegment salt)
        {
            if (data.Length < ConstantPartLength)
            {
                hashAlgorithm = DnsSecNSec3HashAlgorithm.Sha1;
                flags = DnsSecNSec3Flags.None;
                iterations = 0;
                salt = null;
                return false;
            }

            hashAlgorithm = (DnsSecNSec3HashAlgorithm)data[Offset.HashAlgorithm];
            flags = (DnsSecNSec3Flags)data[Offset.Flags];
            iterations = data.ReadUShort(Offset.Iterations, Endianity.Big);
            
            int saltLength = data[Offset.SaltLength];
            if (data.Length - Offset.Salt < saltLength)
            {
                salt = null;
                return false;
            }
            salt = data.SubSegment(Offset.Salt, saltLength);
            return true;
        }
    }

    /// <summary>
    /// RFC 5155.
    /// <pre>
    /// +-----+-------------+----------+--------+------------+
    /// | bit | 0-7         | 8-14     | 15     | 16-31      |
    /// +-----+-------------+----------+--------+------------+
    /// | 0   | Hash Alg    | Reserved | OptOut | Iterations |
    /// +-----+-------------+----------+--------+------------+
    /// | 32  | Salt Length | Salt                           |
    /// +-----+-------------+                                |
    /// | ... |                                              |
    /// +-----+-------------+--------------------------------+
    /// |     | Hash Length | Next Hashed Owner Name         |
    /// +-----+-------------+                                |
    /// | ... |                                              |
    /// +-----+----------------------------------------------+
    /// |     | Type Bit Maps                                |
    /// | ... |                                              |
    /// +-----+----------------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NSec3)]
    public sealed class DnsResourceDataNextDomainSecure3 : DnsResourceDataNextDomainSecure3Base, IEquatable<DnsResourceDataNextDomainSecure3>
    {
        public DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt,
                                                DataSegment nextHashedOwnerName, IEnumerable<DnsType> existTypes)
            : this(hashAlgorithm, flags, iterations, salt, nextHashedOwnerName, new DnsTypeBitmaps(existTypes))
        {
        }

        /// <summary>
        /// Contains the next hashed owner name in hash order.
        /// This value is in binary format.
        /// Given the ordered set of all hashed owner names, the Next Hashed Owner Name field contains the hash of an owner name that immediately follows the owner name of the given NSEC3 RR.
        /// The value of the Next Hashed Owner Name field in the last NSEC3 RR in the zone is the same as the hashed owner name of the first NSEC3 RR in the zone in hash order.
        /// Note that, unlike the owner name of the NSEC3 RR, the value of this field does not contain the appended zone name.
        /// </summary>
        public DataSegment NextHashedOwnerName { get; private set; }

        /// <summary>
        /// Identifies the RRSet types that exist at the original owner name of the NSEC3 RR.
        /// </summary>
        public ReadOnlyCollection<DnsType> TypesExist { get { return _typeBitmaps.TypesExist.AsReadOnly(); } }

        public bool Equals(DnsResourceDataNextDomainSecure3 other)
        {
            return EqualsParameters(other) &&
                   NextHashedOwnerName.Equals(other.NextHashedOwnerName) &&
                   _typeBitmaps.Equals(other._typeBitmaps);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNextDomainSecure3);
        }

        internal DnsResourceDataNextDomainSecure3()
            : this(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0, DataSegment.Empty, DataSegment.Empty, new DnsType[0])
        {
        }

        internal override int GetLength()
        {
            return ParametersLength + sizeof(byte) + NextHashedOwnerName.Length + _typeBitmaps.GetLength();
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            WriteParameters(buffer, offset);
            buffer.Write(offset + NextHashedOwnerNameLengthOffset, (byte)NextHashedOwnerName.Length);
            NextHashedOwnerName.Write(buffer, offset + NextHashedOwnerNameOffset);
            _typeBitmaps.Write(buffer, offset + TypeBitmapsOffset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsSecNSec3HashAlgorithm hashAlgorithm;
            DnsSecNSec3Flags flags;
            ushort iterations;
            DataSegment salt;
            if (!TryReadParameters(data, out hashAlgorithm, out flags, out iterations, out salt))
                return null;

            int nextHashedOwnerNameLengthOffset = GetParametersLength(salt.Length);
            if (data.Length - nextHashedOwnerNameLengthOffset < sizeof(byte))
                return null;
            int nextHashedOwnerNameOffset = nextHashedOwnerNameLengthOffset + sizeof(byte);
            int nextHashedOwnerNameLength = data[nextHashedOwnerNameLengthOffset];
            if (data.Length - nextHashedOwnerNameOffset < nextHashedOwnerNameLength)
                return null;
            DataSegment nextHashedOwnerName = data.SubSegment(nextHashedOwnerNameOffset, nextHashedOwnerNameLength);

            int typeBitmapsOffset = nextHashedOwnerNameOffset + nextHashedOwnerNameLength;
            DnsTypeBitmaps typeBitmaps = DnsTypeBitmaps.CreateInstance(data.Buffer, data.StartOffset + typeBitmapsOffset, data.Length - typeBitmapsOffset);
            if (typeBitmaps == null)
                return null;

            return new DnsResourceDataNextDomainSecure3(hashAlgorithm, flags, iterations, salt, nextHashedOwnerName, typeBitmaps);
        }

        private DnsResourceDataNextDomainSecure3(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt,
                                                 DataSegment nextHashedOwnerName, DnsTypeBitmaps typeBitmaps)
            : base(hashAlgorithm, flags, iterations, salt)
        {
            if (nextHashedOwnerName.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("nextHashedOwnerName", nextHashedOwnerName.Length, string.Format("Cannot bigger than {0}.", byte.MaxValue));

            NextHashedOwnerName = nextHashedOwnerName;
            _typeBitmaps = typeBitmaps;
        }

        private int NextHashedOwnerNameLengthOffset { get { return ParametersLength; } }

        private int NextHashedOwnerNameOffset { get { return NextHashedOwnerNameLengthOffset + sizeof(byte); } }

        private int TypeBitmapsOffset { get { return NextHashedOwnerNameOffset + NextHashedOwnerName.Length; } }

        private readonly DnsTypeBitmaps _typeBitmaps;
    }

    /// <summary>
    /// RFC 5155.
    /// <pre>
    /// +-----+-------------+----------+--------+------------+
    /// | bit | 0-7         | 8-14     | 15     | 16-31      |
    /// +-----+-------------+----------+--------+------------+
    /// | 0   | Hash Alg    | Reserved | OptOut | Iterations |
    /// +-----+-------------+----------+--------+------------+
    /// | 32  | Salt Length | Salt                           |
    /// +-----+-------------+                                |
    /// | ... |                                              |
    /// +-----+----------------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NSec3Param)]
    public sealed class DnsResourceDataNextDomainSecure3Parameters : DnsResourceDataNextDomainSecure3Base, IEquatable<DnsResourceDataNextDomainSecure3Parameters>
    {
        public DnsResourceDataNextDomainSecure3Parameters(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt)
            : base(hashAlgorithm, flags, iterations, salt)
        {
        }

        public bool Equals(DnsResourceDataNextDomainSecure3Parameters other)
        {
            return EqualsParameters(other);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNextDomainSecure3Parameters);
        }

        internal DnsResourceDataNextDomainSecure3Parameters()
            : this(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ParametersLength;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            WriteParameters(buffer, offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsSecNSec3HashAlgorithm hashAlgorithm;
            DnsSecNSec3Flags flags;
            ushort iterations;
            DataSegment salt;
            if (!TryReadParameters(data, out hashAlgorithm, out flags, out iterations, out salt))
                return null;

            if (data.Length != GetParametersLength(salt.Length))
                return null;

            return new DnsResourceDataNextDomainSecure3Parameters(hashAlgorithm, flags, iterations, salt);
        }
    }

    /// <summary>
    /// RFC 5205.
    /// <pre>
    /// +-----+------------+--------------+-----------+
    /// | bit | 0-7        | 8-15         | 16-31     |
    /// +-----+------------+--------------+-----------+
    /// | 0   | HIT Length | PK Algorithm | PK Length |
    /// +-----+------------+--------------+-----------+
    /// | 32  | HIT                                   |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// |     | Public Key                            |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// |     | Rendezvous Servers                    |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Hip)]
    public sealed class DnsResourceDataHostIdentityProtocol: DnsResourceDataNoCompression, IEquatable<DnsResourceDataHostIdentityProtocol>
    {
        private static class Offset
        {
            public const int HostIdentityTagLength = 0;
            public const int PublicKeyAlgorithm = HostIdentityTagLength + sizeof(byte);
            public const int PublicKeyLength = PublicKeyAlgorithm + sizeof(byte);
            public const int HostIdentityTag = PublicKeyLength + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.HostIdentityTag;

        public DnsResourceDataHostIdentityProtocol(DataSegment hostIdentityTag, DnsPublicKeyAlgorithm publicKeyAlgorithm, DataSegment publicKey,
                                                   IEnumerable<DnsDomainName> rendezvousServers)
        {
            if (hostIdentityTag.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("hostIdentityTag", hostIdentityTag.Length, string.Format("Cannot be bigger than {0}.", byte.MaxValue));
            if (hostIdentityTag.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("publicKey", publicKey.Length, string.Format("Cannot be bigger than {0}.", ushort.MaxValue));
            HostIdentityTag = hostIdentityTag;
            PublicKeyAlgorithm = publicKeyAlgorithm;
            PublicKey = publicKey;
            RendezvousServers = rendezvousServers.ToArray().AsReadOnly();
        }

        /// <summary>
        /// Stored as a binary value in network byte order.
        /// </summary>
        public DataSegment HostIdentityTag { get; private set; }

        /// <summary>
        /// Identifies the public key's cryptographic algorithm and determines the format of the public key field.
        /// </summary>
        public DnsPublicKeyAlgorithm PublicKeyAlgorithm { get; private set; }

        /// <summary>
        /// Contains the algorithm-specific portion of the KEY RR RDATA.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        /// <summary>
        /// Indicates one or more domain names of rendezvous server(s).
        /// Must not be compressed.
        /// The rendezvous server(s) are listed in order of preference (i.e., first rendezvous server(s) are preferred),
        /// defining an implicit order amongst rendezvous servers of a single RR.
        /// When multiple HIP RRs are present at the same owner name,
        /// this implicit order of rendezvous servers within an RR must not be used to infer a preference order between rendezvous servers stored in different RRs.
        /// </summary>
        public ReadOnlyCollection<DnsDomainName> RendezvousServers { get; private set; }

        public bool Equals(DnsResourceDataHostIdentityProtocol other)
        {
            return other != null &&
                   HostIdentityTag.Equals(other.HostIdentityTag) &&
                   PublicKeyAlgorithm.Equals(other.PublicKeyAlgorithm) &&
                   PublicKey.Equals(other.PublicKey) &&
                   RendezvousServers.SequenceEqual(RendezvousServers);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataHostIdentityProtocol);
        }

        internal DnsResourceDataHostIdentityProtocol()
            : this(DataSegment.Empty, DnsPublicKeyAlgorithm.None, DataSegment.Empty, new DnsDomainName[0])
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + HostIdentityTag.Length + PublicKey.Length + RendezvousServers.Sum(rendezvousServer => rendezvousServer.NonCompressedLength);
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.HostIdentityTagLength, (byte)HostIdentityTag.Length);
            buffer.Write(offset + Offset.PublicKeyAlgorithm, (byte)PublicKeyAlgorithm);
            buffer.Write(offset + Offset.PublicKeyLength, (ushort)PublicKey.Length, Endianity.Big);
            HostIdentityTag.Write(buffer, offset + Offset.HostIdentityTag);
            int numBytesWritten = ConstantPartLength + HostIdentityTag.Length;
            PublicKey.Write(buffer, offset + numBytesWritten);
            numBytesWritten += PublicKey.Length;
            foreach (DnsDomainName rendezvousServer in RendezvousServers)
            {
                rendezvousServer.WriteUncompressed(buffer, offset + numBytesWritten);
                numBytesWritten += rendezvousServer.NonCompressedLength;
            }

            return numBytesWritten;
        }
        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            int hostIdentityTagLength = dns[offsetInDns + Offset.HostIdentityTagLength];
            DnsPublicKeyAlgorithm publicKeyAlgorithm = (DnsPublicKeyAlgorithm)dns[offsetInDns + Offset.PublicKeyAlgorithm];
            int publicKeyLength = dns.ReadUShort(offsetInDns + Offset.PublicKeyLength, Endianity.Big);
            
            if (length < ConstantPartLength + hostIdentityTagLength + publicKeyLength)
                return null;
            DataSegment hostIdentityTag = dns.SubSegment(offsetInDns + Offset.HostIdentityTag, hostIdentityTagLength);
            int publicKeyOffset = offsetInDns + ConstantPartLength + hostIdentityTagLength;
            DataSegment publicKey = dns.SubSegment(publicKeyOffset, publicKeyLength);

            offsetInDns += ConstantPartLength + hostIdentityTagLength + publicKeyLength;
            length -= ConstantPartLength + hostIdentityTagLength + publicKeyLength;

            List<DnsDomainName> rendezvousServers = new List<DnsDomainName>();
            while (length != 0)
            {
                DnsDomainName rendezvousServer;
                int rendezvousServerLength;
                if (!DnsDomainName.TryParse(dns, offsetInDns, length, out rendezvousServer, out rendezvousServerLength))
                    return null;
                rendezvousServers.Add(rendezvousServer);
                offsetInDns += rendezvousServerLength;
                length -= rendezvousServerLength;

            }

            return new DnsResourceDataHostIdentityProtocol(hostIdentityTag, publicKeyAlgorithm, publicKey, rendezvousServers);
        }
    }

    /// <summary>
    /// <pre>
    /// +---------------------+
    /// | One ore more strings|
    /// +---------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NInfo)]
    public sealed class DnsResourceDataNInfo : DnsResourceDataStrings
    {
        private const int MinNumStrings = 1;

        public DnsResourceDataNInfo(ReadOnlyCollection<DataSegment> strings)
            : base(strings)
        {
            if (strings.Count < MinNumStrings)
                throw new ArgumentOutOfRangeException("strings", strings.Count, "There must be at least one string.");
        }

        public DnsResourceDataNInfo(params DataSegment[] strings)
            : this(strings.AsReadOnly())
        {
        }

        internal DnsResourceDataNInfo()
            : this(DataSegment.Empty)
        {
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data, MinNumStrings);
            if (strings == null || strings.Count < 1)
                return null;

            return new DnsResourceDataNInfo(strings.AsReadOnly());
        }
    }

    /// <summary>
    /// Reid.
    /// <pre>
    /// +-----+-------+----------+-----------+
    /// | bit | 0-15  | 16-23    | 24-31     |
    /// +-----+-------+----------+-----------+
    /// | 0   | flags | protocol | algorithm |
    /// +-----+-------+----------+-----------+
    /// | 32  | public key                   |
    /// | ... |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.RKey)]
    public sealed class DnsResourceDataRKey : DnsResourceDataSimple, IEquatable<DnsResourceDataRKey>
    {
        private static class Offset
        {
            public const int Flags = 0;
            public const int Protocol = Flags + sizeof(ushort);
            public const int Algorithm = Protocol + sizeof(byte);
            public const int PublicKey = Algorithm + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.PublicKey;

        public DnsResourceDataRKey(ushort flags, byte protocol, DnsAlgorithm algorithm, DataSegment publicKey)
        {
            Flags = flags;
            Protocol = protocol;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// Reserved and must be zero.
        /// </summary>
        public ushort Flags { get; private set; }

        /// <summary>
        /// Must be set to 1.
        /// </summary>
        public byte Protocol { get; private set; }

        /// <summary>
        /// The key algorithm parallel to the same field for the SIG resource.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// The public key value.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        public bool Equals(DnsResourceDataRKey other)
        {
            return other != null &&
                   Flags.Equals(other.Flags) &&
                   Protocol.Equals(other.Protocol) &&
                   Algorithm.Equals(other.Algorithm) &&
                   PublicKey.Equals(other.PublicKey);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataRKey);
        }

        internal DnsResourceDataRKey()
            : this(0, 1, DnsAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + PublicKey.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Flags, Flags, Endianity.Big);
            buffer.Write(offset + Offset.Protocol, Protocol);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            PublicKey.Write(buffer, offset + Offset.PublicKey);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            ushort flags = data.ReadUShort(Offset.Flags, Endianity.Big);
            byte protocol = data[Offset.Protocol];
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DataSegment publicKey = data.SubSegment(Offset.PublicKey, data.Length - ConstantPartLength);

            return new DnsResourceDataRKey(flags, protocol, algorithm, publicKey);
        }
    }

    /// <summary>
    /// Wijngaards.
    /// <pre>
    /// +----------------------+
    /// | Previous Domain Name |
    /// +----------------------+
    /// | Next Domain Name     |
    /// +----------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.TaLink)]
    public sealed class DnsResourceDataTrustAnchorLink : DnsResourceDataNoCompression, IEquatable<DnsResourceDataTrustAnchorLink>
    {
        private const int MinimumLength = 2 * DnsDomainName.RootLength;

        public DnsResourceDataTrustAnchorLink(DnsDomainName previous, DnsDomainName next)
        {
            Previous = previous;
            Next = next;
        }

        /// <summary>
        /// The start, or previous name.
        /// </summary>
        public DnsDomainName Previous { get; private set; }

        /// <summary>
        /// End or next name in the list.
        /// </summary>
        public DnsDomainName Next { get; private set; }

        public bool Equals(DnsResourceDataTrustAnchorLink other)
        {
            return other != null &&
                   Previous.Equals(other.Previous) &&
                   Next.Equals(other.Next);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataTrustAnchorLink);
        }

        internal DnsResourceDataTrustAnchorLink()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return Previous.NonCompressedLength + Next.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            Previous.WriteUncompressed(buffer, offset);
            int previousLength = Previous.NonCompressedLength;
            Next.WriteUncompressed(buffer, offset + previousLength);

            return previousLength + Next.NonCompressedLength;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < MinimumLength)
                return null;

            DnsDomainName previous;
            int previousLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length - DnsDomainName.RootLength, out previous, out previousLength))
                return null;
            offsetInDns += previousLength;
            length -= previousLength;

            DnsDomainName next;
            int nextLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out next, out nextLength))
                return null;

            return new DnsResourceDataTrustAnchorLink(previous, next);
        }
    }

    /// <summary>
    /// RFC 2930.
    /// </summary>
    public enum DnsTransactionKeyMode : ushort
    {
        /// <summary>
        /// RFC 2930.
        /// </summary>
        ServerAssignment = 1,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        DiffieHellmanExchange = 2,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        GssApiNegotiation = 3,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        ResolverAssignment = 4,

        /// <summary>
        /// RFC 2930.
        /// </summary>
        KeyDeletion = 5,
    }

    /// <summary>
    /// RFC 2930.
    /// <pre>
    /// +------+------------+------------+
    /// | bit  | 0-15       | 16-31      |
    /// +------+------------+------------+
    /// | 0    | Algorithm               |
    /// | ...  |                         |
    /// +------+-------------------------+
    /// | X    | Inception               |
    /// +------+-------------------------+
    /// | X+32 | Expiration              |
    /// +------+------------+------------+
    /// | X+64 | Mode       | Error      |
    /// +------+------------+------------+
    /// | X+96 | Key Size   |            |
    /// +------+------------+ Key Data   |
    /// | ...  |                         |
    /// +------+------------+------------+
    /// |      | Other Size |            |
    /// +------+------------+ Other Data |
    /// | ...  |                         |
    /// +------+-------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.TKey)]
    public sealed class DnsResourceDataTransactionKey : DnsResourceData, IEquatable<DnsResourceDataTransactionKey>
    {
        private static class OffsetAfterAlgorithm
        {
            public const int Inception = 0;
            public const int Expiration = Inception + SerialNumber32.SizeOf;
            public const int Mode = Expiration + SerialNumber32.SizeOf;
            public const int Error = Mode + sizeof(ushort);
            public const int KeySize = Error + sizeof(ushort);
            public const int KeyData = KeySize + sizeof(ushort);
        }

        private const int ConstantPartLength = OffsetAfterAlgorithm.KeyData + sizeof(ushort);

        public DnsResourceDataTransactionKey(DnsDomainName algorithm, SerialNumber32 inception, SerialNumber32 expiration, DnsTransactionKeyMode mode,
                                             DnsResponseCode error, DataSegment key, DataSegment other)
        {
            if (key.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("key", key.Length, string.Format("Cannot be longer than {0}", ushort.MaxValue));
            if (other.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("other", other.Length, string.Format("Cannot be longer than {0}", ushort.MaxValue));

            Algorithm = algorithm;
            Inception = inception;
            Expiration = expiration;
            Mode = mode;
            Error = error;
            Key = key;
            Other = other;
        }

        /// <summary>
        /// Name of the algorithm in domain name syntax.
        /// The algorithm determines how the secret keying material agreed to using the TKEY RR is actually used to derive the algorithm specific key.
        /// </summary>
        public DnsDomainName Algorithm { get; private set; }

        /// <summary>
        /// Number of seconds since the beginning of 1 January 1970 GMT ignoring leap seconds treated as modulo 2**32 using ring arithmetic.
        /// In messages between a DNS resolver and a DNS server where this field is meaningful,
        /// it is either the requested validity interval start for the keying material asked for or
        /// specify the validity interval start of keying material provided.
        /// 
        /// To avoid different interpretations of the inception time in TKEY RRs,
        /// resolvers and servers exchanging them must have the same idea of what time it is.
        /// One way of doing this is with the NTP protocol [RFC 2030] but that or any other time synchronization used for this purpose must be done securely.
        /// </summary>
        public SerialNumber32 Inception { get; private set; }

        /// <summary>
        /// Number of seconds since the beginning of 1 January 1970 GMT ignoring leap seconds treated as modulo 2**32 using ring arithmetic.
        /// In messages between a DNS resolver and a DNS server where this field is meaningful,
        /// it is either the requested validity interval end for the keying material asked for or
        /// specify the validity interval end of keying material provided.
        /// 
        /// To avoid different interpretations of the expiration time in TKEY RRs,
        /// resolvers and servers exchanging them must have the same idea of what time it is.
        /// One way of doing this is with the NTP protocol [RFC 2030] but that or any other time synchronization used for this purpose must be done securely.
        /// </summary>
        public SerialNumber32 Expiration { get; private set; }

        /// <summary>
        /// Specifies the general scheme for key agreement or the purpose of the TKEY DNS message.
        /// Servers and resolvers supporting this specification must implement the Diffie-Hellman key agreement mode and the key deletion mode for queries.
        /// All other modes are optional.
        /// A server supporting TKEY that receives a TKEY request with a mode it does not support returns the BADMODE error.
        /// </summary>
        public DnsTransactionKeyMode Mode { get; private set; }

        /// <summary>
        /// When the TKEY Error Field is non-zero in a response to a TKEY query, the DNS header RCODE field indicates no error.
        /// However, it is possible if a TKEY is spontaneously included in a response the TKEY RR and DNS header error field could have unrelated non-zero error codes.
        /// </summary>
        public DnsResponseCode Error { get; private set; }

        /// <summary>
        /// The key exchange data.
        /// The meaning of this data depends on the mode.
        /// </summary>
        public DataSegment Key { get; private set; }

        /// <summary>
        /// May be used in future extensions.
        /// </summary>
        public DataSegment Other { get; private set; }

        public bool Equals(DnsResourceDataTransactionKey other)
        {
            return other != null &&
                   Algorithm.Equals(other.Algorithm) &&
                   Inception.Equals(other.Inception) &&
                   Expiration.Equals(other.Expiration) &&
                   Mode.Equals(other.Mode) &&
                   Error.Equals(other.Error) &&
                   Key.Equals(other.Key) &&
                   Other.Equals(other.Other);
        }

        public override bool  Equals(DnsResourceData other)
        {
 	        return Equals(other as DnsResourceDataTransactionKey);
        }

        internal DnsResourceDataTransactionKey()
            : this(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.DiffieHellmanExchange, DnsResponseCode.NoError, DataSegment.Empty, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Algorithm.GetLength(compressionData, offsetInDns) + ConstantPartLength + Key.Length + Other.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int algorithmLength = Algorithm.Write(buffer, dnsOffset, compressionData, offsetInDns);
            int offset = dnsOffset + offsetInDns + algorithmLength;
            buffer.Write(offset + OffsetAfterAlgorithm.Inception, Inception.Value, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Expiration, Expiration.Value, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Mode, (ushort)Mode, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Error, (ushort)Error, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.KeySize, (ushort)Key.Length, Endianity.Big);
            Key.Write(buffer, offset + OffsetAfterAlgorithm.KeyData);

            int otherSizeOffset = offset + OffsetAfterAlgorithm.KeyData + Key.Length;
            buffer.Write(otherSizeOffset, (ushort)Other.Length, Endianity.Big);
            Other.Write(buffer, otherSizeOffset + sizeof(ushort));

            return algorithmLength + ConstantPartLength + Key.Length + Other.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength + DnsDomainName.RootLength)
                return null;

            DnsDomainName algorithm;
            int algorithmLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length - ConstantPartLength, out algorithm, out algorithmLength))
                return null;
            offsetInDns += algorithmLength;
            length -= algorithmLength;

            if (length < ConstantPartLength)
                return null;

            uint inception = dns.ReadUInt(offsetInDns + OffsetAfterAlgorithm.Inception, Endianity.Big);
            uint expiration = dns.ReadUInt(offsetInDns + OffsetAfterAlgorithm.Expiration, Endianity.Big);
            DnsTransactionKeyMode mode = (DnsTransactionKeyMode)dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.Mode, Endianity.Big);
            DnsResponseCode error = (DnsResponseCode)dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.Error, Endianity.Big);

            int keySize = dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.KeySize, Endianity.Big);
            if (length < ConstantPartLength + keySize)
                return null;
            DataSegment key = dns.SubSegment(offsetInDns + OffsetAfterAlgorithm.KeyData, keySize);

            int totalReadAfterAlgorithm = OffsetAfterAlgorithm.KeyData + keySize;
            offsetInDns += totalReadAfterAlgorithm;
            length -= totalReadAfterAlgorithm;
            int otherSize = dns.ReadUShort(offsetInDns, Endianity.Big);
            if (length != sizeof(ushort) + otherSize)
                return null;
            DataSegment other = dns.SubSegment(offsetInDns + sizeof(ushort), otherSize);

            return new DnsResourceDataTransactionKey(algorithm, inception, expiration, mode, error, key, other);
        }
    }

    /// <summary>
    /// RFC 2845.
    /// <pre>
    /// +------+-------------+----------+-----------+
    /// | bit  | 0-15        | 16-31    | 32-47     |
    /// +------+-------------+----------+-----------+
    /// | 0    | Algorithm Name                     |
    /// | ...  |                                    |
    /// +------+------------------------------------+
    /// | X    | Time Signed                        |
    /// +------+-------------+----------+-----------+
    /// | X+48 | Fudge       | MAC Size | MAC       |
    /// +------+-------------+----------+           |
    /// | ...  |                                    |
    /// +------+-------------+----------+-----------+
    /// | Y    | Original ID | Error    | Other Len |
    /// +------+-------------+----------+-----------+
    /// | Y+48 | Other Data                         |
    /// | ...  |                                    |
    /// +------+------------------------------------+
    /// </pre>
    /// // 
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.TSig)]
    public sealed class DnsResourceDataTransactionSignature : DnsResourceData, IEquatable<DnsResourceDataTransactionSignature>
    {
        private static class OffsetAfterAlgorithm
        {
            public const int TimeSigned = 0;
            public const int Fudge = TimeSigned + UInt48.SizeOf;
            public const int MessageAuthenticationCodeSize = Fudge + sizeof(ushort);
            public const int MessageAuthenticationCode = MessageAuthenticationCodeSize + sizeof(ushort);
        }

        private static class OffsetAfterMessageAuthenticationCode
        {
            public const int OriginalId = 0;
            public const int Error = OriginalId + sizeof(ushort);
            public const int OtherLength = Error + sizeof(ushort);
            public const int OtherData = OtherLength + sizeof(ushort);
        }

        private const int ConstantPartLength = OffsetAfterAlgorithm.MessageAuthenticationCode + OffsetAfterMessageAuthenticationCode.OtherData;

        public DnsResourceDataTransactionSignature(DnsDomainName algorithm, UInt48 timeSigned, ushort fudge, DataSegment messageAuthenticationCode,
                                                   ushort originalId, DnsResponseCode error, DataSegment other)
        {
            if (messageAuthenticationCode.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("messageAuthenticationCode", messageAuthenticationCode.Length, string.Format("Cannot be longer than {0}", ushort.MaxValue));
            if (other.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("other", other.Length, string.Format("Cannot be longer than {0}", ushort.MaxValue));

            Algorithm = algorithm;
            TimeSigned = timeSigned;
            Fudge = fudge;
            MessageAuthenticationCode = messageAuthenticationCode;
            OriginalId = originalId;
            Error = error;
            Other = other;
        }

        /// <summary>
        /// Name of the algorithm in domain name syntax.
        /// </summary>
        public DnsDomainName Algorithm { get; private set; }

        /// <summary>
        /// Seconds since 1-Jan-70 UTC.
        /// </summary>
        public UInt48 TimeSigned { get; private set; }

        /// <summary>
        /// Seconds of error permitted in Time Signed.
        /// </summary>
        public ushort Fudge { get; private set; }

        /// <summary>
        /// Defined by Algorithm Name.
        /// </summary>
        public DataSegment MessageAuthenticationCode { get; private set; }

        /// <summary>
        /// Original message ID.
        /// </summary>
        public ushort OriginalId { get; private set; }

        /// <summary>
        /// RCODE covering TSIG processing.
        /// </summary>
        public DnsResponseCode Error { get; private set; }

        /// <summary>
        /// Empty unless Error == BADTIME.
        /// </summary>
        public DataSegment Other { get; private set; }

        public bool Equals(DnsResourceDataTransactionSignature other)
        {
            return other != null &&
                   Algorithm.Equals(other.Algorithm) &&
                   TimeSigned.Equals(other.TimeSigned) &&
                   Fudge.Equals(other.Fudge) &&
                   MessageAuthenticationCode.Equals(other.MessageAuthenticationCode) &&
                   OriginalId.Equals(other.OriginalId) &&
                   Error.Equals(other.Error) &&
                   Other.Equals(other.Other);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataTransactionSignature);
        }

        internal DnsResourceDataTransactionSignature()
            : this(DnsDomainName.Root, 0, 0, DataSegment.Empty, 0, DnsResponseCode.NoError, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Algorithm.GetLength(compressionData, offsetInDns) + ConstantPartLength + MessageAuthenticationCode.Length + Other.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int algorithmLength = Algorithm.Write(buffer, dnsOffset, compressionData, offsetInDns);
            int offset = dnsOffset + offsetInDns + algorithmLength;
            buffer.Write(offset + OffsetAfterAlgorithm.TimeSigned, TimeSigned, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Fudge, Fudge, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.MessageAuthenticationCodeSize, (ushort)MessageAuthenticationCode.Length, Endianity.Big);
            MessageAuthenticationCode.Write(buffer, offset + OffsetAfterAlgorithm.MessageAuthenticationCode);

            offset += OffsetAfterAlgorithm.MessageAuthenticationCode + MessageAuthenticationCode.Length;
            buffer.Write(offset + OffsetAfterMessageAuthenticationCode.OriginalId, OriginalId, Endianity.Big);
            buffer.Write(offset + OffsetAfterMessageAuthenticationCode.Error, (ushort)Error, Endianity.Big);
            buffer.Write(offset + OffsetAfterMessageAuthenticationCode.OtherLength, (ushort)Other.Length, Endianity.Big);
            Other.Write(buffer, offset + OffsetAfterMessageAuthenticationCode.OtherData);

            return algorithmLength + ConstantPartLength + MessageAuthenticationCode.Length + Other.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength + DnsDomainName.RootLength)
                return null;

            DnsDomainName algorithm;
            int algorithmLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length - ConstantPartLength, out algorithm, out algorithmLength))
                return null;
            offsetInDns += algorithmLength;
            length -= algorithmLength;

            if (length < ConstantPartLength)
                return null;

            UInt48 timeSigned = dns.ReadUInt48(offsetInDns + OffsetAfterAlgorithm.TimeSigned, Endianity.Big);
            ushort fudge  = dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.Fudge, Endianity.Big);
            int messageAuthenticationCodeLength = dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.MessageAuthenticationCodeSize, Endianity.Big);
            if (length < ConstantPartLength + messageAuthenticationCodeLength)
                return null;
            DataSegment messageAuthenticationCode = dns.SubSegment(offsetInDns + OffsetAfterAlgorithm.MessageAuthenticationCode, messageAuthenticationCodeLength);
            int totalReadAfterAlgorithm = OffsetAfterAlgorithm.MessageAuthenticationCode + messageAuthenticationCodeLength;
            offsetInDns += totalReadAfterAlgorithm;
            length -= totalReadAfterAlgorithm;

            ushort originalId = dns.ReadUShort(offsetInDns + OffsetAfterMessageAuthenticationCode.OriginalId, Endianity.Big);
            DnsResponseCode error = (DnsResponseCode)dns.ReadUShort(offsetInDns + OffsetAfterMessageAuthenticationCode.Error, Endianity.Big);
            int otherLength = dns.ReadUShort(offsetInDns + OffsetAfterMessageAuthenticationCode.OtherLength, Endianity.Big);
            if (length != OffsetAfterMessageAuthenticationCode.OtherData + otherLength)
                return null;
            DataSegment other = dns.SubSegment(offsetInDns + OffsetAfterMessageAuthenticationCode.OtherData, otherLength);

            return new DnsResourceDataTransactionSignature(algorithm, timeSigned, fudge, messageAuthenticationCode, originalId, error, other);
        }
    }
}
