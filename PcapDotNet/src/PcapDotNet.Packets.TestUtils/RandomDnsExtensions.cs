using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomDnsExtensions
    {
        public static DnsLayer NextDnsLayer(this Random random)
        {
            DnsLayer dnsLayer = new DnsLayer();
            dnsLayer.Id = random.NextUShort();
            dnsLayer.IsQuery = random.NextBool();
            dnsLayer.Opcode = random.NextEnum<DnsOpcode>();
            dnsLayer.IsAuthoritiveAnswer = random.NextBool();
            dnsLayer.IsTruncated = random.NextBool();
            dnsLayer.IsRecusionDesired = random.NextBool();
            dnsLayer.IsRecusionAvailable = random.NextBool();
            dnsLayer.FutureUse = random.NextByte(DnsDatagram.MaxFutureUse + 1);
            dnsLayer.ResponseCode = random.NextEnum<DnsResponseCode>();
            dnsLayer.DomainNameCompressionMode = random.NextEnum<DnsDomainNameCompressionMode>();
            int numQueries = random.Next(10);
            List<DnsQueryResourceRecord> queries = new List<DnsQueryResourceRecord>();
            for (int i = 0; i != numQueries; ++i)
                queries.Add(random.NextDnsQueryResourceRecord());
            dnsLayer.Queries = queries;
            int numAnswers = random.Next(10);
            List<DnsDataResourceRecord> answers = new List<DnsDataResourceRecord>();
            for (int i = 0; i != numAnswers; ++i)
                answers.Add(random.NextDnsDataResourceRecord());
            dnsLayer.Answers = answers;
            int numAuthorities = random.Next(10);
            List<DnsDataResourceRecord> authorities = new List<DnsDataResourceRecord>();
            for (int i = 0; i != numAuthorities; ++i)
                authorities.Add(random.NextDnsDataResourceRecord());
            dnsLayer.Authorities = authorities;
            int numAdditionals = random.Next(10);
            List<DnsDataResourceRecord> additionals = new List<DnsDataResourceRecord>();
            for (int i = 0; i != numAdditionals; ++i)
                additionals.Add(random.NextDnsDataResourceRecord());
            dnsLayer.Additionals = additionals;
            return dnsLayer;
        }

        public static DnsQueryResourceRecord NextDnsQueryResourceRecord(this Random random)
        {
            DnsQueryResourceRecord record = new DnsQueryResourceRecord(random.NextDnsDomainName(), random.NextEnum<DnsType>(), random.NextEnum<DnsClass>());
            return record;
        }

        public static DnsDataResourceRecord NextDnsDataResourceRecord(this Random random)
        {
            DnsType type = random.NextEnum<DnsType>();
            DnsDataResourceRecord record = new DnsDataResourceRecord(random.NextDnsDomainName(), type, random.NextEnum<DnsClass>(), random.Next(), random.NextDnsResourceData(type));
            return record;
        }

        public static DnsDomainName NextDnsDomainName(this Random random)
        {
            List<string> labels = new List<string>();
            int numLabels = random.Next(10);
            for (int i = 0; i != numLabels; ++i)
            {
                int labelLength = random.Next(10);
                StringBuilder label = new StringBuilder();
                for (int j = 0; j != labelLength; ++j)
                    label.Append(random.NextChar('a', 'z'));
                labels.Add(label.ToString());
            }
            return new DnsDomainName(string.Join(".", labels));
        }

        public static DnsResourceData NextDnsResourceData(this Random random, DnsType type)
        {
            switch (type)
            {
                case DnsType.A:
                    return new DnsResourceDataIpV4(random.NextIpV4Address());
                case DnsType.Ns:
                case DnsType.Md:
                case DnsType.Mf:
                case DnsType.CName:
                case DnsType.Mb:
                case DnsType.Mg:
                case DnsType.Mr:
                case DnsType.Ptr:
                case DnsType.NsapPtr:
                    return new DnsResourceDataDomainName(random.NextDnsDomainName());
                case DnsType.Soa:
                    return new DnsResourceDataStartOfAuthority(random.NextDnsDomainName(), random.NextDnsDomainName(),
                                                               random.NextUInt(), random.NextUInt(), random.NextUInt(), random.NextUInt(), random.NextUInt());
                case DnsType.Null:
                    return new DnsResourceDataAnything(random.NextDataSegment(random.Next(65536)));

                case DnsType.Wks:
                    return new DnsResourceDataWellKnownService(random.NextIpV4Address(), random.NextEnum<IpV4Protocol>(),
                                                               random.NextDataSegment(random.Next(10)));

                case DnsType.HInfo:
                    return new DnsResourceDataHostInformation(random.NextDataSegment(random.Next(10)), random.NextDataSegment(random.Next(10)));

                case DnsType.MInfo:
                    return new DnsResourceDataMailingListInfo(random.NextDnsDomainName(), random.NextDnsDomainName());

                case DnsType.Mx:
                    return new DnsResourceDataMailExchange(random.NextUShort(), random.NextDnsDomainName());

                case DnsType.Txt:
                    return new DnsResourceDataText(((Func<DataSegment>)(() => random.NextDataSegment(random.Next(10)))).GenerateArray(10).AsReadOnly());

                case DnsType.Rp:
                    return new DnsResourceDataResponsiblePerson(random.NextDnsDomainName(), random.NextDnsDomainName());

                case DnsType.AfsDb:
                    return new DnsResourceDataAfsDb(random.NextUShort(), random.NextDnsDomainName());

                case DnsType.X25:
                    return new DnsResourceDataString(random.NextDataSegment(random.Next(10)));

                case DnsType.Isdn:
                    return random.NextBool()
                               ? new DnsResourceDataIsdn(random.NextDataSegment(random.Next(10)))
                               : new DnsResourceDataIsdn(random.NextDataSegment(random.Next(10)), random.NextDataSegment(random.Next(10)));

                case DnsType.Rt:
                    return new DnsResourceDataRouteThrough(random.NextUShort(), random.NextDnsDomainName());

                case DnsType.Nsap:
                    return new DnsResourceDataNetworkServiceAccessPoint(random.NextDataSegment(1 + random.Next(10)), random.NextUInt48(), random.NextByte());

                case DnsType.Sig:
                    return new DnsResourceDataSig(random.NextEnum<DnsType>(), random.NextEnum<DnsAlgorithm>(), random.NextByte(), random.NextUInt(),
                                                  random.NextUInt(), random.NextUInt(), random.NextUShort(), random.NextDnsDomainName(),
                                                  random.NextDataSegment(random.Next(100)));

                case DnsType.Key:
                    return new DnsResourceDataKey(random.NextBool(), random.NextBool(), random.NextEnum<DnsKeyNameType>(), random.NextFlags<DnsKeySignatory>(),
                                                  random.NextEnum<DnsKeyProtocol>(), random.NextEnum<DnsAlgorithm>(),
                                                  random.NextBool() ? (ushort?)random.NextUShort() : null, random.NextDataSegment(random.Next(100)));

                case DnsType.Px:
                    return new DnsResourceDataX400Pointer(random.NextUShort(), random.NextDnsDomainName(), random.NextDnsDomainName());

                case DnsType.GPos:
                    return new DnsResourceDataGeographicalPosition((random.Next(-90, 90) * random.NextDouble()).ToString("0.##########"),
                                                                   (random.Next(-180, 180) * random.NextDouble()).ToString("0.##########"),
                                                                   (random.Next(-500, 50000) * random.NextDouble()).ToString("0.##########"));

                default:
                    return new DnsResourceDataAnything(random.NextDataSegment(random.Next(100)));
            }
        }
    }
}
