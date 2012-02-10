using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    public abstract class DnsResourceData
    {
        internal const int StringMinimumLength = sizeof(byte); 

        public static Type GetDnsResourceDataType(DnsType dnsType)
        {
            DnsResourceData prototype = TryGetPrototype(dnsType);
            if (prototype == null)
                return null;
            return prototype.GetType();
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
            DnsResourceData prototype = TryGetPrototype(type);
            if (prototype != null)
                return prototype.CreateInstance(dns, offsetInDns, length);
            return new DnsResourceDataAnything(dns.Subsegment(offsetInDns, length));
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
            DataSegment str = data.Subsegment(ref offset, stringLength);

            return str;
        }

        internal static void WriteString(byte[] buffer, ref int offset, DataSegment str)
        {
            buffer.Write(ref offset, (byte)str.Length);
            str.Write(buffer, ref offset);
        }

        private static DnsResourceData TryGetPrototype(DnsType type)
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
}
