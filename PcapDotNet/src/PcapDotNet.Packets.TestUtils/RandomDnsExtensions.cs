using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
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
            if (type == DnsType.Opt)
            {
                return new DnsOptResourceRecord(random.NextDnsDomainName(), random.NextUShort(), random.NextByte(), (DnsOptVersion)random.NextByte(),
                                                random.NextFlags<DnsOptFlags>(), (DnsResourceDataOptions)random.NextDnsResourceData(type));
            }
            return new DnsDataResourceRecord(random.NextDnsDomainName(), type, random.NextEnum<DnsClass>(), random.Next(), random.NextDnsResourceData(type));
        }

        public static DnsOptions NextDnsOptions(this Random random)
        {
            return new DnsOptions(((Func<DnsOption>)(() => random.NextDnsOption())).GenerateArray(random.Next(10)));
        }

        public static DnsOption NextDnsOption(this Random random)
        {
            if (random.NextBool())
            {
                return new DnsOptionAnything((DnsOptionCode)random.NextUShort(4, ushort.MaxValue + 1), random.NextDataSegment(random.Next(20)));
            }

            switch (random.NextEnum<DnsOptionCode>())
            {
                case DnsOptionCode.LongLivedQuery:
                    return new DnsOptionLongLivedQuery(random.NextUShort(), random.NextEnum<DnsLongLivedQueryOpcode>(),
                                                       random.NextEnum<DnsLongLivedQueryErrorCode>(), random.NextULong(), random.NextUInt());

                case DnsOptionCode.UpdateLease:
                    return new DnsOptionUpdateLease(random.NextInt());

                case DnsOptionCode.NameServerIdentifier:
                    return new DnsOptionAnything(DnsOptionCode.NameServerIdentifier, random.NextDataSegment(random.NextInt(0, 100)));

                default:
                    throw new InvalidOperationException("Invalid value");
            }
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
                case DnsType.DName:
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
                case DnsType.RrSig:
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
                    return new DnsResourceDataGeographicalPosition((random.NextInt(-90, 90) * random.NextDouble()).ToString("0.##########"),
                                                                   (random.NextInt(-180, 180) * random.NextDouble()).ToString("0.##########"),
                                                                   (random.NextInt(-500, 50000) * random.NextDouble()).ToString("0.##########"));

                case DnsType.Aaaa:
                    return new DnsResourceDataIpV6(random.NextIpV6Address());

                case DnsType.Loc:
                    return new DnsResourceDataLocationInformation(random.NextByte(),
                                                                  (ulong)(random.NextInt(0, 10) * Math.Pow(10, random.NextInt(0, 10))),
                                                                  (ulong)(random.NextInt(0, 10) * Math.Pow(10, random.NextInt(0, 10))),
                                                                  (ulong)(random.NextInt(0, 10) * Math.Pow(10, random.NextInt(0, 10))),
                                                                  random.NextUInt(), random.NextUInt(), random.NextUInt());

                case DnsType.Nxt:
                    byte[] typeBitMap = random.NextBytes(random.Next(DnsResourceDataNextDomain.MaxTypeBitMapLength + 1));
                    if (typeBitMap.Length > 0 && typeBitMap[typeBitMap.Length - 1] == 0)
                        typeBitMap[typeBitMap.Length - 1] = random.NextByte(1, 256);
                    return new DnsResourceDataNextDomain(random.NextDnsDomainName(), new DataSegment(typeBitMap));

                case DnsType.EId:
                case DnsType.NimLoc:
                case DnsType.Dhcid:
                    return new DnsResourceDataAnything(random.NextDataSegment(random.Next(32)));

                case DnsType.Srv:
                    return new DnsResourceDataServerSelection(random.NextUShort(), random.NextUShort(), random.NextUShort(), random.NextDnsDomainName());

                case DnsType.AtmA:
                    return new DnsResourceDataAtmAddress(random.NextEnum<DnsAtmAddressFormat>(), random.NextDataSegment(random.Next(100)));

                case DnsType.NaPtr:
                    IEnumerable<byte> possibleFlags =
                        Enumerable.Range('0', '9' - '0' + 1).Concat(Enumerable.Range('a', 'z' - 'a' + 1)).Concat(Enumerable.Range('A', 'Z' - 'A' + 1)).Select(value => (byte)value);
                    return new DnsResourceDataNamingAuthorityPointer(
                        random.NextUShort(), random.NextUShort(),
                        new DataSegment(FuncExtensions.GenerateArray(() => random.NextValue(possibleFlags.ToArray()), 10)),
                        random.NextDataSegment(random.Next(100)), random.NextDataSegment(random.Next(100)),
                        random.NextDnsDomainName());

                case DnsType.Kx:
                    return new DnsResourceDataKeyExchanger(random.NextUShort(), random.NextDnsDomainName());

                case DnsType.Cert:
                    return new DnsResourceDataCertificate(random.NextEnum<DnsCertificateType>(), random.NextUShort(), random.NextEnum<DnsAlgorithm>(),
                                                          random.NextDataSegment(random.Next(100)));

                case DnsType.A6:
                    byte prefixLength = random.NextByte(DnsResourceDataA6.MaxPrefixLength + 1);
                    UInt128 addressSuffixValue = prefixLength == 0
                                                     ? random.NextUInt128()
                                                     : random.NextUInt128(((UInt128)1) << (128 - prefixLength));
                    return new DnsResourceDataA6(prefixLength,
                                                 new IpV6Address(addressSuffixValue),
                                                 random.NextDnsDomainName());

                case DnsType.Sink:
                    return new DnsResourceDataSink(random.NextEnum<DnsSinkCoding>(), random.NextByte(), random.NextDataSegment(random.Next(100)));

                case DnsType.Opt:
                    return new DnsResourceDataOptions(random.NextDnsOptions());

                case DnsType.Apl:
                    return new DnsResourceDataAddressPrefixList(
                        ((Func<DnsAddressPrefix>)
                         (() =>
                          new DnsAddressPrefix(random.NextEnum<AddressFamily>(), random.NextByte(), random.NextBool(),
                                               random.NextDataSegment(random.Next(0, 128))))).GenerateArray(random.Next(10)));

                case DnsType.Ds:
                    return new DnsResourceDataDelegationSigner(random.NextUShort(), random.NextEnum<DnsAlgorithm>(), random.NextEnum<DnsDigestType>(),
                                                               random.NextDataSegment(random.Next(50)));

                case DnsType.SshFp:
                    return new DnsResourceDataSshFingerprint(random.NextEnum<DnsFingerprintPublicKeyAlgorithm>(), random.NextEnum<DnsFingerprintType>(),
                                                             random.NextDataSegment(random.Next(20)));

                case DnsType.IpSecKey:
                    return new DnsResourceDataIpSecKey(random.NextByte(), random.NextDnsGateway(), random.NextEnum<DnsPublicKeyAlgorithm>(),
                                                       random.NextDataSegment(random.Next(100)));

                case DnsType.NSec:
                    return new DnsResourceDataNextDomainSecure(random.NextDnsDomainName(), random.NextDnsTypeArray(random.Next(100)));

                case DnsType.DnsKey:
                    return new DnsResourceDataDnsKey(random.NextBool(), random.NextBool(), random.NextByte(), random.NextEnum<DnsAlgorithm>(), random.NextDataSegment(random.Next(100)));

                case DnsType.NSec3:
                    return new DnsResourceDataNextDomainSecure3(random.NextEnum<DnsSecNSec3HashAlgorithm>(), random.NextFlags<DnsSecNSec3Flags>(),
                                                                random.NextUShort(), random.NextDataSegment(random.Next(10)), random.NextDataSegment(10),
                                                                random.NextDnsTypeArray(random.Next(100)));

                case DnsType.NSec3Param:
                    return new DnsResourceDataNextDomainSecure3Parameters(random.NextEnum<DnsSecNSec3HashAlgorithm>(), random.NextFlags<DnsSecNSec3Flags>(),
                                                                          random.NextUShort(), random.NextDataSegment(random.Next(10)));

                case DnsType.Hip:
                    return new DnsResourceDataHostIdentityProtocol(random.NextDataSegment(random.NextInt(0, 100)), random.NextEnum<DnsPublicKeyAlgorithm>(),
                                                                   random.NextDataSegment(random.NextInt(0, 100)),
                                                                   ((Func<DnsDomainName>)(() => random.NextDnsDomainName())).GenerateArray(random.NextInt(0, 10)));

                case DnsType.NInfo:
                    return new DnsResourceDataNInfo(((Func<DataSegment>)(() => random.NextDataSegment(random.NextInt(1, 10)))).GenerateArray(10).AsReadOnly());

                case DnsType.RKey:
                    return new DnsResourceDataRKey(random.NextUShort(), random.NextByte(), random.NextEnum<DnsAlgorithm>(), random.NextDataSegment(random.NextInt(0, 100)));

                default:
                    return new DnsResourceDataAnything(random.NextDataSegment(random.Next(100)));
            }
        }

        public static DnsType[] NextDnsTypeArray(this Random random, int length)
        {
            return ((Func<DnsType>)(() => random.NextEnum<DnsType>())).GenerateArray(length);
        }

        public static DnsGateway NextDnsGateway(this Random random)
        {
            DnsGatewayType gatewayType = random.NextEnum<DnsGatewayType>();
            switch (gatewayType)
            {
                case DnsGatewayType.None:
                    return DnsGateway.None;

                case DnsGatewayType.IpV4:
                    return new DnsGatewayIpV4(random.NextIpV4Address());

                case DnsGatewayType.IpV6:
                    return new DnsGatewayIpV6(random.NextIpV6Address());

                case DnsGatewayType.DomainName:
                    return new DnsGatewayDomainName(random.NextDnsDomainName());

                default:
                    throw new InvalidOperationException(string.Format("Invalid gateway type: {0}", gatewayType));
            }
                    
        }
    }
}
