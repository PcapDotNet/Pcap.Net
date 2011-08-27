using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;


namespace PcapDotNet.Packets.Dns
{
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

        internal static DnsResourceData Read(DnsType type, DnsClass dnsClass, DataSegment data)
        {
            DnsResourceData prototype = TryGetPrototype(type, dnsClass);
            if (prototype != null)
                return prototype.CreateInstance(data);
            return new DnsResourceDataUnknown(data);
        }

        internal abstract DnsResourceData CreateInstance(DataSegment data);

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
                where typeof(DnsResourceData).IsAssignableFrom(type) &&
                      type.GetCustomAttributes<DnsTypeRegistrationAttribute>(false).Any()
                select new
                {
                    type.GetCustomAttributes<DnsTypeRegistrationAttribute>(false).First().Type,
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
    }

    [DnsTypeRegistration(Type = DnsType.A)]
    public class DnsResourceDataIpV4 : DnsResourceDataSimple, IEquatable<DnsResourceDataIpV4>
    {
        public DnsResourceDataIpV4()
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

        public sealed override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataIpV4);
        }

        internal sealed override int GetLength()
        {
 	        return IpV4Address.SizeOf;
        }

        internal sealed override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data, Endianity.Big);
        }

        internal sealed override DnsResourceData CreateInstance(DataSegment data)
        {
            return new DnsResourceDataIpV4(data.ReadIpV4Address(0, Endianity.Big));
        }

    }
}