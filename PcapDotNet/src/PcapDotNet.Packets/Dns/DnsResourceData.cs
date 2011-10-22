using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// |       |         |
    /// +-------+---------+
    /// | X     | RNAME   |
    /// |       |         |
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
    /// |     |        |
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
    /// |     |            |
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
    /// |     |          |
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
    /// |     |                   |
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
    /// |     |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Sig)]
    public sealed class DnsResourceDataSig : DnsResourceData, IEquatable<DnsResourceDataSig>
    {
        private static class Offset
        {
            public const int TypeCovered = 0;
            public const int Algorithm = TypeCovered + sizeof(ushort);
            public const int Labels = Algorithm + sizeof(byte);
            public const int OriginalTtl = Labels + sizeof(byte);
            public const int SignatureExpiration = OriginalTtl + sizeof(uint);
            public const int SignatureInception = SignatureExpiration + sizeof(uint);
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

        public DnsResourceDataKey()
            : this(false, false, DnsKeyNameType.ZoneKey, DnsKeySignatory.Zone, DnsKeyProtocol.All, DnsAlgorithm.None, null, DataSegment.Empty)
        {
        }

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
    /// |     |            |
    /// +-----+------------+
    /// |     | MAPX400    |
    /// |     |            |
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

        public DnsResourceDataX400Pointer()
            : this(0, DnsDomainName.Root, DnsDomainName.Root)
        {
        }

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
        public DnsResourceDataGeographicalPosition()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

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
        public DnsResourceDataIpV6()
            : this(IpV6Address.Zero)
        {
        }

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

        public DnsResourceDataLocationInformation()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

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
    /// <pre>
    /// +------------------+
    /// | next domain name |
    /// |                  |
    /// +------------------+
    /// | type bit map     |
    /// |                  |
    /// +------------------+
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Nxt)]
    public sealed class DnsResourceDataNextDomain : DnsResourceData, IEquatable<DnsResourceDataNextDomain>
    {
        public const int MaxTypeBitMapLength = 16;
        public const DnsType MaxTypeBitMapDnsType = (DnsType)(8 * MaxTypeBitMapLength);

        public DnsResourceDataNextDomain()
            : this(DnsDomainName.Root, DataSegment.Empty)
        {
        }

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
    /// |     |          |
    /// +-----+----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Srv)]
    public sealed class DnsResourceDataServerSelection : DnsResourceData, IEquatable<DnsResourceDataServerSelection>
    {
        private static class Offset
        {
            public const int Priority = 0;
            public const int Weight = Priority + sizeof(ushort);
            public const int Port = Weight + sizeof(ushort);
            public const int Target = Port + sizeof(ushort);
        }

        public const int ConstantPartLength = Offset.Target;

        public DnsResourceDataServerSelection()
            : this(0, 0, 0, DnsDomainName.Root)
        {
        }

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

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return GetLength();
        }

        private int GetLength()
        {
            return ConstantPartLength + Target.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int offset = dnsOffset + offsetInDns;
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
    /// |     |         |
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

        public DnsResourceDataAtmAddress()
            : this(DnsAtmAddressFormat.AtmEndSystemAddress, DataSegment.Empty)
        {
        }

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
    /// |     |                    |
    /// +-----+--------------------+
    /// |     | SERVICES           |
    /// |     |                    |
    /// +-----+--------------------+
    /// |     | REGEXP             |
    /// |     |                    |
    /// +-----+--------------------+
    /// |     | REPLACEMENT        |
    /// |     |                    |
    /// +-----+--------------------+
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NaPtr)]
    public sealed class DnsResourceDataNamingAuthorityPointer : DnsResourceData, IEquatable<DnsResourceDataNamingAuthorityPointer>
    {
        private static class Offset
        {
            public const int Order = 0;
            public const int Preference = Order + sizeof(ushort);
            public const int Flags = Preference + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.Flags;

        public DnsResourceDataNamingAuthorityPointer()
            : this(0, 0, DataSegment.Empty, DataSegment.Empty, DataSegment.Empty, DnsDomainName.Root)
        {
        }

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

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return GetLength();
        }

        private int GetLength()
        {
            return ConstantPartLength + GetStringLength(Flags) + GetStringLength(Services) + GetStringLength(Regexp) + Replacement.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int offset = dnsOffset + offsetInDns;
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
}
