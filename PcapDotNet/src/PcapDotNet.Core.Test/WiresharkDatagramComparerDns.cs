using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerDns : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Dns"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            DnsDatagram dnsDatagram = (DnsDatagram)datagram;

            switch (field.Name())
            {
                case "dns.id":
                    field.AssertShowHex(dnsDatagram.Id);
                    break;
                    
                case "dns.flags":
                    field.AssertShowHex(dnsDatagram.Subsegment(2, 2).ToArray().ReadUShort(0, Endianity.Big));
                    foreach (var flagField in field.Fields())
                    {
                        switch (flagField.Name())
                        {
                            case "dns.flags.response":
                                flagField.AssertShowDecimal(dnsDatagram.IsResponse);
                                break;

                            case "dns.flags.opcode":
                                flagField.AssertShowDecimal((byte)dnsDatagram.OpCode);
                                break;

                            case "dns.flags.authoritative":
                                flagField.AssertShowDecimal(dnsDatagram.IsAuthoritativeAnswer);
                                break;

                            case "dns.flags.truncated":
                                flagField.AssertShowDecimal(dnsDatagram.IsTruncated);
                                break;

                            case "dns.flags.recdesired":
                                flagField.AssertShowDecimal(dnsDatagram.IsRecursionDesired);
                                break;

                            case "dns.flags.recavail":
                                flagField.AssertShowDecimal(dnsDatagram.IsRecursionAvailable);
                                break;

                            case "dns.flags.z":
                                flagField.AssertShowDecimal(dnsDatagram.FutureUse);
                                break;

                            case "dns.flags.authenticated":
                                flagField.AssertShowDecimal(dnsDatagram.IsAuthenticData);
                                break;

                            case "dns.flags.checkdisable":
                                flagField.AssertShowDecimal(dnsDatagram.IsCheckingDisabled);
                                break;

                            case "dns.flags.rcode":
                                flagField.AssertShowDecimal((ushort)dnsDatagram.ResponseCode);
                                break;

                            default:
                                throw new InvalidOperationException("Invalid DNS flag field " + flagField.Name());
                        }
                    }
                    break;

                case "dns.count.queries":
                case "dns.count.zones":
                    field.AssertShowDecimal(dnsDatagram.QueryCount);
                    break;

                case "dns.count.answers":
                case "dns.count.prerequisites":
                    field.AssertShowDecimal(dnsDatagram.AnswerCount);
                    break;

                case "dns.count.auth_rr":
                case "dns.count.updates":
                    field.AssertShowDecimal(dnsDatagram.AuthorityCount);
                    break;

                case "dns.count.add_rr":
                    field.AssertShowDecimal(dnsDatagram.AdditionalCount);
                    break;

                case "":
                    var resourceRecordsFields = field.Fields();
                    switch (field.Show())
                    {
                        case "Queries":
                        case "Zone":
                            CompareResourceRecords(resourceRecordsFields, dnsDatagram.Queries);
                            break;

                        case "Answers":
                        case "Prerequisites":
                            CompareResourceRecords(resourceRecordsFields, dnsDatagram.Answers);
                            break;

                        case "Authoritative nameservers":
                        case "Updates":
                            CompareResourceRecords(resourceRecordsFields, dnsDatagram.Authorities);
                            break;

                        case "Additional records":
                            CompareResourceRecords(resourceRecordsFields, dnsDatagram.Additionals);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS resource records field " + field.Show());
                    }
                    break;

                default:
                    throw new InvalidOperationException("Invalid DNS field " + field.Name());
            }

            return true;
        }

        private void CompareResourceRecords(IEnumerable<XElement> resourceRecordFields, IEnumerable<DnsResourceRecord> resourceRecords)
        {
            XElement[] resourceRecordFieldsArray= resourceRecordFields.ToArray();
            DnsResourceRecord[] resourceRecordsArray = resourceRecords.ToArray();
            Assert.AreEqual(resourceRecordFieldsArray.Length, resourceRecordsArray.Length);
            for (int i = 0; i != resourceRecordFieldsArray.Length; ++i)
            {
                ResetRecordFields();
                var resourceRecordField = resourceRecordFieldsArray[i];
                var resourceRecord = resourceRecordsArray[i];
                foreach (var resourceRecordAttributeField in resourceRecordField.Fields())
                {
                    switch (resourceRecordAttributeField.Name())
                    {
                        case "dns.qry.name":
                        case "dns.resp.name":
                            resourceRecordAttributeField.AssertShow(GetWiresharkDomainName(resourceRecord.DomainName));
                            break;

                        case "dns.qry.type":
                        case "dns.resp.type":
                            resourceRecordAttributeField.AssertShowHex((ushort)resourceRecord.DnsType);
                            break;

                        case "dns.qry.class":
                        case "dns.resp.class":
                            resourceRecordAttributeField.AssertShowHex((ushort)resourceRecord.DnsClass);
                            break;

                        case "dns.resp.ttl":
                            resourceRecordAttributeField.AssertShowDecimal(resourceRecord.Ttl);
                            break;

                        case "dns.resp.len":
                            break;

                        default:
                            CompareResourceRecordData(resourceRecordAttributeField, resourceRecord);
                            break;
                    }
                }
            }
        }

        private void ResetRecordFields()
        {
            _hipRendezvousServersIndex = 0;
            _wksBitmapIndex = 0;
            _nxtTypeIndex = 0;
            _nSecTypeIndex = 0;
            _nSec3TypeIndex = 0;
            _txtIndex = 0;
            _aplItemIndex = 0;
        }

        private void CompareResourceRecordData(XElement dataField, DnsResourceRecord resourceRecord)
        {
            var data = resourceRecord.Data;
            string dataFieldName = dataField.Name();
            string dataFieldShow = dataField.Show();
            string dataFieldShowUntilColon = dataFieldShow.Split(':')[0];
            switch (resourceRecord.DnsType)
            {
                case DnsType.A:
                    switch (dataFieldName)
                    {
                        case "dns.resp.addr":
                            dataField.AssertShow(((DnsResourceDataIpV4)data).Data.ToString());
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Ns:
                    dataField.AssertName("");
                    dataField.AssertShow("Name server: " + GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;
                case DnsType.Md:         // 3.
                case DnsType.MailForwarder:         // 4.
                case DnsType.Mailbox:         // 7.
                case DnsType.MailGroup:         // 8.
                case DnsType.MailRename: // 9.
                    dataField.AssertName("");
                    dataField.AssertShow("Host: " + GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;
                case DnsType.CName:
                    dataField.AssertName("");
                    dataField.AssertShow("Primary name: " + GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;
                case DnsType.StartOfAuthority:
                    dataField.AssertName("");
                    var soaData = (DnsResourceDataStartOfAuthority)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Primary name server":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(soaData.MainNameServer));
                            break;

                        case "Responsible authority's mailbox":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(soaData.ResponsibleMailbox));
                            break;

                        case "Serial number":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + soaData.Serial);
                            break;

                        case "Refresh interval":
                            dataField.AssertValue(soaData.Refresh);
                            break;

                        case "Retry interval":
                            dataField.AssertValue(soaData.Retry);
                            break;

                        case "Expiration limit":
                            dataField.AssertValue(soaData.Expire);
                            break;

                        case "Minimum TTL":
                            dataField.AssertValue(soaData.MinimumTtl);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                    
                case DnsType.Wks:
                    dataField.AssertName("");
                    var wksData = (DnsResourceDataWellKnownService)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Addr":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + wksData.Address);
                            break;

                        case "Protocol":
                            dataField.AssertValue((byte)wksData.Protocol);
                            break;

                        case "Bits":
                            while (wksData.Bitmap[_wksBitmapIndex] == 0x00)
                                ++_wksBitmapIndex;
                            dataField.AssertValue(wksData.Bitmap[_wksBitmapIndex++]);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.Ptr:
                    dataField.AssertName("");
                    dataField.AssertShow("Domain name: " + GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;
                case DnsType.HInfo:
                    dataField.AssertName("");
                    var hInfoData = (DnsResourceDataHostInformation)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "CPU":
                            dataField.AssertValue(new[] {(byte)hInfoData.Cpu.Length}.Concat(hInfoData.Cpu));
                            break;

                        case "OS":
                            dataField.AssertValue(new[] {(byte)hInfoData.Os.Length}.Concat(hInfoData.Os));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                    
                case DnsType.MInfo:
                    dataField.AssertName("");
                    var mInfoData = (DnsResourceDataMailingListInfo)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Responsible Mailbox":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(mInfoData.MailingList));
                            break;

                        case "Error Mailbox":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(mInfoData.ErrorMailbox));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.MailExchange:
                    var mxData = (DnsResourceDataMailExchange)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Preference":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + mxData.Preference);
                            break;

                        case "Mail exchange":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(mxData.MailExchangeHost));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.Txt: // 16.
                case DnsType.Spf: // 99.
                    var txtData = (DnsResourceDataText)data;
                    dataField.AssertShow("Text: " + txtData.Text[_txtIndex++].ToString(EncodingExtensions.Iso88591).ToWiresharkLiteral(false));
                    break;

                case DnsType.ResponsiblePerson:
                    dataField.AssertName("");
                    var rpData = (DnsResourceDataResponsiblePerson)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Mailbox":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(rpData.Mailbox));
                            break;

                        case "TXT RR":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(rpData.TextDomain));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.AfsDatabase:
                    dataField.AssertName("");
                    var afsDbData = (DnsResourceDataAfsDatabase)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Subtype":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + afsDbData.Subtype);
                            break;

                        case "Hostname":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(afsDbData.HostName));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);

                    }
                    break;
                case DnsType.X25:
                    dataField.AssertName("");
                    dataField.AssertShow("PSDN-Address: " + ((DnsResourceDataString)data).String.ToString(EncodingExtensions.Iso88591).ToWiresharkLiteral(false));
                    break;
                case DnsType.Isdn:
                    dataField.AssertName("");
                    var isdnData = (DnsResourceDataIsdn)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "ISDN Address":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " +
                                                 isdnData.IsdnAddress.ToString(EncodingExtensions.Iso88591).ToWiresharkLiteral(false));
                            break;

                        case "Subaddress":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " +
                                                 isdnData.Subaddress.ToString(EncodingExtensions.Iso88591).ToWiresharkLiteral(false));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    
                    break;
                case DnsType.RouteThrough:
                    dataField.AssertName("");
                    var rtData = (DnsResourceDataRouteThrough)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Preference":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + rtData.Preference);
                            break;

                        case "Intermediate-Host":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(rtData.IntermediateHost));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.NetworkServiceAccessPoint:
                    var nsapData = (DnsResourceDataNetworkServiceAccessPoint)data;

                    switch (dataFieldName)
                    {
                        case "dns.nsap.rdata":
                            byte[] nsapId = new byte[6];
                            nsapId.Write(0, nsapData.SystemIdentifier, Endianity.Big);
                            dataField.AssertValue(nsapData.AreaAddress.Concat(nsapId).Concat(nsapData.Selector));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.NetworkServiceAccessPointPointer:
                    dataField.AssertName("");
                    dataField.AssertShow("Owner: " + GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;

                case DnsType.Key:
                    dataField.AssertName("");
                    var keyData = (DnsResourceDataKey)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Flags":
                            foreach (var flagField in dataField.Fields())
                            {
                                int flagCount = GetFlagCount(flagField);
                                switch (flagCount)
                                {
                                    case 0:
                                        flagField.AssertShow(keyData.AuthenticationProhibited
                                                                 ? "1... .... .... .... = Key prohibited for authentication"
                                                                 : "0... .... .... .... = Key allowed for authentication");
                                        break;

                                    case 1:
                                        flagField.AssertShow(keyData.ConfidentialityProhibited
                                                                 ? ".1.. .... .... .... = Key prohibited for confidentiality"
                                                                 : ".0.. .... .... .... = Key allowed for confidentiality");
                                        break;

                                    case 2:
                                        flagField.AssertShow(keyData.Experimental
                                                                 ? "..1. .... .... .... = Key is experimental or optional"
                                                                 : "..0. .... .... .... = Key is required");
                                        break;

                                    case 5:
                                        flagField.AssertShow(keyData.UserAssociated
                                                                 ? ".... .1.. .... .... = Key is associated with a user"
                                                                 : ".... .0.. .... .... = Key is not associated with a user");
                                        break;

                                    case 6:
                                        flagField.AssertShow(keyData.NameType == DnsKeyNameType.NonZoneEntity
                                                                 ? ".... ..1. .... .... = Key is associated with the named entity"
                                                                 : ".... ..0. .... .... = Key is not associated with the named entity");
                                        break;

                                    case 8:
                                        flagField.AssertShow(keyData.IpSec
                                                                 ? ".... .... 1... .... = Key is valid for use with IPSEC"
                                                                 : ".... .... 0... .... = Key is not valid for use with IPSEC");
                                        break;

                                    case 9:
                                        flagField.AssertShow(keyData.Email
                                                                 ? ".... .... .1.. .... = Key is valid for use with MIME security multiparts"
                                                                 : ".... .... .0.. .... = Key is not valid for use with MIME security multiparts");
                                        break;

                                    case 12:
                                        Assert.AreEqual(flagField.Show().Substring(19), " = Signatory = " + (byte)keyData.Signatory);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid flag count " + flagCount);
                                }
                            }
                            break;

                        case "Protocol":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + (byte)keyData.Protocol);
                            break;

                        case "Algorithm":
                            dataField.AssertValue((byte)keyData.Algorithm);
                            break;

                        case "Key id":
                            // TODO: Remove this once wireshark bug is fixed.
                            // https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=6704
                            break;

                        case "Public key":
                            byte[] flagsExtension;
                            if (keyData.FlagsExtension == null)
                            {
                                flagsExtension = new byte[0];
                            } 
                            else
                            {
                                flagsExtension = new byte[2];
                                flagsExtension.Write(0, keyData.FlagsExtension.Value, Endianity.Big);
                            }
                            dataField.AssertValue(flagsExtension.Concat(keyData.PublicKey));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.Signature:   // 24.
                case DnsType.ResourceRecordSignature: // 46.
                    dataField.AssertName("");
                    var sigData = (DnsResourceDataSignature)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Type covered":
                            dataField.AssertValue((ushort)sigData.TypeCovered);
                            break;

                        case "Algorithm":
                            dataField.AssertValue((byte)sigData.Algorithm);
                            break;

                        case "Labels":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + sigData.Labels);
                            break;

                        case "Original TTL":
                            dataField.AssertValue(sigData.OriginalTtl);
                            break;

                        case "Signature expiration":
                            dataField.AssertValue(sigData.SignatureExpiration);
                            break;

                        case "Time signed":
                            dataField.AssertValue(sigData.SignatureInception);
                            break;

                        case "Id of signing key(footprint)":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + sigData.KeyTag);
                            break;

                        case "Signer's name":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(sigData.SignersName));
                            break;

                        case "Signature":
                            dataField.AssertValue(sigData.Signature);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.PointerX400:
                    dataField.AssertName("");
                    var pxData = (DnsResourceDataX400Pointer)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Preference":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + pxData.Preference);
                            break;

                        case "MAP822":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(pxData.Map822));
                            break;

                        case "MAPX400":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(pxData.MapX400));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.GPos:
                    dataField.AssertName("");
                    var gposData = (DnsResourceDataGeographicalPosition)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Longitude":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + gposData.Longitude);
                            break;

                        case "Latitude":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + gposData.Latitude);
                            break;

                        case "Altitude":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + gposData.Altitude);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.Aaaa:
                    dataField.AssertName("");
                    dataField.AssertShow("Addr: " + GetWiresharkIpV6(((DnsResourceDataIpV6)data).Data));
                    break;

                case DnsType.Loc:
                    dataField.AssertName("");
                    var locData = (DnsResourceDataLocationInformation)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Version":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + locData.Version);
                            break;

                        case "Data":
                            dataField.AssertShow("Data");
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.NextDomain:
                    dataField.AssertName("");
                    var nxtData = (DnsResourceDataNextDomain)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Next domain name":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(nxtData.NextDomainName));
                            break;

                        case "RR type in bit map":
                            DnsType actualType = nxtData.TypesExist.Skip(_nxtTypeIndex++).First();
                            DnsType expectedType;
                            if (!TryGetDnsType(dataFieldShow, out expectedType))
                                throw new InvalidOperationException(string.Format("Can't parse DNS field {0} : {1}", dataFieldShow, actualType));
                            else
                                Assert.AreEqual(expectedType, actualType);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.ServerSelection:
                    dataField.AssertName("");
                    var srvData = (DnsResourceDataServerSelection)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Priority":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + srvData.Priority);
                            break;

                        case "Weight":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + srvData.Weight);
                            break;

                        case "Port":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + srvData.Port);
                            break;

                        case "Target":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(srvData.Target));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.NaPtr:
                    dataField.AssertName("");
                    var naPtrData = (DnsResourceDataNamingAuthorityPointer)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Order":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + naPtrData.Order);
                            break;

                        case "Preference":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + naPtrData.Preference);
                            break;

                        case "Flags length":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + naPtrData.Flags.Length);
                            break;

                        case "Flags":
                            dataField.AssertValue(naPtrData.Flags);
                            break;

                        case "Service length":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + naPtrData.Services.Length);
                            break;

                        case "Service":
                            dataField.AssertValue(naPtrData.Services);
                            break;

                        case "Regex length":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + naPtrData.RegularExpression.Length);
                            break;

                        case "Regex":
                            dataField.AssertValue(naPtrData.RegularExpression);
                            break;

                        case "Replacement length":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + naPtrData.Replacement.NonCompressedLength);
                            break;

                        case "Replacement":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(naPtrData.Replacement));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.KeyExchanger:
                    dataField.AssertName("");
                    var kxData = (DnsResourceDataKeyExchanger)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Preference":
                            dataField.AssertShow(dataFieldShowUntilColon + ": 0");
                            dataField.AssertValue(kxData.Preference);
                            break;

                        case "Key exchange":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(kxData.KeyExchangeHost));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.Cert:
                    dataField.AssertName("");
                    var certData = (DnsResourceDataCertificate)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Type":
                            dataField.AssertValue((ushort)certData.CertificateType);
                            break;

                        case "Key footprint":
                            dataField.AssertValue(certData.KeyTag);
                            break;

                        case "Algorithm":
                            dataField.AssertValue((byte)certData.Algorithm);
                            break;

                        case "Public key":
                            dataField.AssertValue(certData.Certificate);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                    
                case DnsType.A6:
                    var a6Data = (DnsResourceDataA6)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Prefix len":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + a6Data.PrefixLength);
                            break;

                        case "Address suffix":
                            Assert.AreEqual(new IpV6Address(dataFieldShow.Substring(dataFieldShowUntilColon.Length + 2)), a6Data.AddressSuffix);
                            break;

                        case "Prefix name":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(a6Data.PrefixName));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }

                    break;
                
                case DnsType.DName:
                    dataField.AssertName("");
                    dataField.AssertShow("Target name: " + GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;

                case DnsType.Opt:
                    var optResourceRecord = (DnsOptResourceRecord)resourceRecord;
                    var optData = (DnsResourceDataOptions)data;
                    switch (dataFieldName)
                    {
                        case "":
                            switch (dataFieldShowUntilColon)
                            {
                                case "UDP payload size":
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + optResourceRecord.SendersUdpPayloadSize);
                                    break;

                                case "Higher bits in extended RCODE":
                                    dataField.AssertValue(optResourceRecord.ExtendedReturnCode);
                                    break;

                                case "EDNS0 version":
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + (byte)optResourceRecord.Version);
                                    break;

                                case "Z":
                                    dataField.AssertValue((ushort)optResourceRecord.Flags);
                                    break;

                                case "Data":
                                    Assert.AreEqual(dataField.Value().Length, 2 * optData.Options.Options.Sum(option => option.Length));
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                            }
                            break;

                        case "dns.resp.len":
                            dataField.AssertShow("Data length: " + optData.Options.Options.Sum(option => option.DataLength));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Apl:
                    var aplData = (DnsResourceDataAddressPrefixList)data;
                    switch (dataFieldName)
                    {
                        case "":
                            dataField.AssertValue((ushort)aplData.Items[_aplItemIndex++].AddressFamily);
                            break;

                        case "hf.dns.apl.coded.prefix":
                            dataField.AssertShowDecimal(aplData.Items[_aplItemIndex - 1].PrefixLength);
                            break;

                        case "dns.apl.negation":
                            dataField.AssertShowDecimal(aplData.Items[_aplItemIndex - 1].Negation);
                            break;

                        case "dns.apl.afdlength":
                            dataField.AssertShowDecimal(aplData.Items[_aplItemIndex - 1].AddressFamilyDependentPart.Length);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.DelegationSigner:  // 43.
                case DnsType.DnsSecLookAsideValidation: // 32769.
                    dataField.AssertName("");
                    var dsData = (DnsResourceDataDelegationSigner)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Key id":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + dsData.KeyTag.ToString("0000"));
                            break;

                        case "Algorithm":
                            dataField.AssertValue((byte)dsData.Algorithm);
                            break;

                        case "Digest type":
                            dataField.AssertValue((byte)dsData.DigestType);
                            break;

                        case "Public key":
                            dataField.AssertValue(dsData.Digest);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                case DnsType.SshFingerprint:
                    var sshFpData = (DnsResourceDataSshFingerprint)data;
                    switch (dataFieldName)
                    {
                        case "":
                            switch (dataFieldShowUntilColon)
                            {
                                case "Algorithm":
                                    dataField.AssertValue((byte)sshFpData.Algorithm);
                                    break;

                                case "Fingerprint type":
                                    dataField.AssertValue((byte)sshFpData.FingerprintType);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                            }
                            break;

                        case "dns.sshfp.fingerprint":
                            dataField.AssertValue(sshFpData.Fingerprint);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.IpSecKey:
                    dataField.AssertName("");
                    var ipSecKeyData = (DnsResourceDataIpSecKey)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Gateway precedence":
                            dataField.AssertValue(ipSecKeyData.Precedence);
                            break;

                        case "Algorithm":
                            dataField.AssertValue((byte)ipSecKeyData.Algorithm);
                            break;

                        case "Gateway":
                            switch (ipSecKeyData.GatewayType)
                            {
                                case DnsGatewayType.None:
                                    dataField.AssertShow(dataFieldShowUntilColon + ": no gateway");
                                    break;

                                case DnsGatewayType.IpV4:
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + ((DnsGatewayIpV4)ipSecKeyData.Gateway).Value);
                                    break;

                                case DnsGatewayType.IpV6:
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkIpV6(((DnsGatewayIpV6)ipSecKeyData.Gateway).Value));
                                    break;

                                case DnsGatewayType.DomainName:
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(((DnsGatewayDomainName)ipSecKeyData.Gateway).Value));
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid Gateway Type " + ipSecKeyData.GatewayType);
                            }
                            break;

                        case "Public key":
                            dataField.AssertValue(ipSecKeyData.PublicKey);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.NSec:
                    dataField.AssertName("");
                    var nSecData = (DnsResourceDataNextDomainSecure)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Next domain name":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(nSecData.NextDomainName));
                            break;

                        case "RR type in bit map":
                            DnsType actualType = nSecData.TypesExist[_nSecTypeIndex++];
                            DnsType expectedType;
                            if (!TryGetDnsType(dataFieldShow, out expectedType))
                                throw new InvalidOperationException(string.Format("Failed parsing type from {0} : {1}", dataFieldShow, actualType));

                            Assert.AreEqual(expectedType, actualType);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;
                    
                case DnsType.DnsKey:
                    dataField.AssertName("");
                    var dnsKeyData = (DnsResourceDataDnsKey)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Flags":
                            foreach (var flagField in dataField.Fields())
                            {
                                int flagCount = GetFlagCount(flagField);
                                switch (flagCount)
                                {
                                    case 7:
                                        flagField.AssertShow(dnsKeyData.ZoneKey
                                                                 ? ".... ...1 .... .... = This is the zone key for the specified zone"
                                                                 : ".... ...0 .... .... = This is not a zone key");
                                        break;

                                    case 8:
                                        flagField.AssertShow(dnsKeyData.Revoke
                                                                 ? ".... .... 1... .... = Key is revoked"
                                                                 : ".... .... 0... .... = Key is not revoked");
                                        break;

                                    case 15:
                                        flagField.AssertShow(dnsKeyData.SecureEntryPoint
                                                                 ? ".... .... .... ...1 = Key is a Key Signing Key"
                                                                 : ".... .... .... ...0 = Key is a Zone Signing Key");
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid DNS data flag field " + flagField.Show());
                                }
                            }
                            break;

                        case "Protocol":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + dnsKeyData.Protocol);
                            break;

                        case "Algorithm":
                            dataField.AssertValue((byte)dnsKeyData.Algorithm);
                            break;

                        case "Key id":
                            // TODO: Remove this once wireshark bug is fixed.
                            // https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=6704
                            break;

                        case "Public key":
                            dataField.AssertValue(dnsKeyData.PublicKey);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.DynamicHostConfigurationId:
                    switch (dataFieldName)
                    {
                        case "dns.dhcid.rdata":
                            dataField.AssertValue(((DnsResourceDataAnything)data).Data);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS resource data field name " + dataFieldName);
                    }
                    break;

                case DnsType.NSec3:
                    var nSec3Data = (DnsResourceDataNextDomainSecure3)data;
                    switch (dataFieldName)
                    {
                        case "dns.nsec3.algo":
                            dataField.AssertShowDecimal((byte)nSec3Data.HashAlgorithm);
                            break;

                        case "dns.nsec3.flags":
                            dataField.AssertShowDecimal((byte)nSec3Data.Flags);
                            foreach (var flagField in dataField.Fields())
                            {
                                string flagFieldName = flagField.Name();
                                switch (flagFieldName)
                                {
                                    case "dns.nsec3.flags.opt_out":
                                        dataField.AssertShowDecimal((nSec3Data.Flags & DnsSecNSec3Flags.OptOut) == DnsSecNSec3Flags.OptOut);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid DNS resource data flag field name " + flagFieldName);
                                }
                            }
                            break;

                        case "dns.nsec3.iterations":
                            dataField.AssertShowDecimal(nSec3Data.Iterations);
                            break;

                        case "dns.nsec3.salt_length":
                            dataField.AssertShowDecimal(nSec3Data.Salt.Length);
                            break;

                        case "dns.nsec3.salt_value":
                            dataField.AssertValue(nSec3Data.Salt);
                            break;

                        case "dns.nsec3.hash_length":
                            dataField.AssertShowDecimal(nSec3Data.NextHashedOwnerName.Length);
                            break;

                        case "dns.nsec3.hash_value":
                            dataField.AssertValue(nSec3Data.NextHashedOwnerName);
                            break;

                        case "":
                            DnsType expectedType = nSec3Data.TypesExist[_nSec3TypeIndex++];
                            Assert.IsTrue(dataField.Show().StartsWith("RR type in bit map: "));
                            if (dataField.Show().EndsWith(string.Format("({0})", (ushort)expectedType)))
                                dataField.AssertShow(string.Format("RR type in bit map: Unknown ({0})", (ushort)expectedType));
                            else
                                Assert.IsTrue(
                                    dataFieldShow.Replace("-", "").StartsWith("RR type in bit map: " + GetWiresharkDnsType(expectedType).Replace("-", "")),
                                    string.Format("{0} : {1}", dataFieldShow, GetWiresharkDnsType(expectedType)));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS resource data field name " + dataFieldName);
                    }
                    break;

                case DnsType.NSec3Parameters:
                    var nSec3ParamData = (DnsResourceDataNextDomainSecure3Parameters)data;
                    switch (dataFieldName)
                    {
                        case "dns.nsec3.algo":
                            dataField.AssertShowDecimal((byte)nSec3ParamData.HashAlgorithm);
                            break;

                        case "dns.nsec3.flags":
                            dataField.AssertShowDecimal((byte)nSec3ParamData.Flags);
                            break;

                        case "dns.nsec3.iterations":
                            dataField.AssertShowDecimal(nSec3ParamData.Iterations);
                            break;

                        case "dns.nsec3.salt_length":
                            dataField.AssertShowDecimal(nSec3ParamData.Salt.Length);
                            break;

                        case "dns.nsec3.salt_value":
                            dataField.AssertShow(nSec3ParamData.Salt);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS resource data field name " + dataFieldName);
                    }
                    break;
                case DnsType.Hip:
                    var hipData = (DnsResourceDataHostIdentityProtocol)data;
                    switch (dataFieldName)
                    {
                        case "":
                            switch (dataFieldShowUntilColon)
                            {
                                case "HIT length":
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + hipData.HostIdentityTag.Length);
                                    break;

                                case "PK algorithm":
                                    dataField.AssertValue((byte)hipData.PublicKeyAlgorithm);
                                    break;

                                case "PK length":
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " + hipData.PublicKey.Length);
                                    break;

                                case "Rendezvous Server":
                                    dataField.AssertShow(dataFieldShowUntilColon + ": " +
                                                         GetWiresharkDomainName(hipData.RendezvousServers[_hipRendezvousServersIndex++]));
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                            }
                            break;

                        case "dns.hip.hit":
                            dataField.AssertShow(hipData.HostIdentityTag);
                            break;

                        case "dns.hip.pk":
                            dataField.AssertShow(hipData.PublicKey);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.TKey:
                    dataField.AssertName("");
                    var tKeyData = (DnsResourceDataTransactionKey)data;
                    switch (dataFieldShowUntilColon)
                    {
                        case "Algorithm name":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + GetWiresharkDomainName(tKeyData.Algorithm));
                            break;

                        case "Signature inception":
                            dataField.AssertValue(tKeyData.Inception);
                            break;

                        case "Signature expiration":
                            dataField.AssertValue(tKeyData.Expiration);
                            break;

                        case "Mode":
                            dataField.AssertValue((ushort)tKeyData.Mode);
                            break;

                        case "Error":
                            dataField.AssertValue((ushort)tKeyData.Error);
                            break;

                        case "Key Size":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + tKeyData.Key.Length);
                            break;

                        case "Key Data":
                            dataField.AssertValue(tKeyData.Key);
                            break;

                        case "Other Size":
                            dataField.AssertShow(dataFieldShowUntilColon + ": " + tKeyData.Other.Length);
                            break;

                        case "Other Data":
                            dataField.AssertValue(tKeyData.Other);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                    }
                    break;

                case DnsType.TransactionSignature:
                    var tSigData = (DnsResourceDataTransactionSignature)data;
                    switch (dataFieldName)
                    {
                        case "dns.tsig.algorithm_name":
                            dataField.AssertShow(GetWiresharkDomainName(tSigData.Algorithm));
                            break;

                        case "":
                            switch (dataFieldShowUntilColon)
                            {
                                case "Time signed":
                                    dataField.AssertValue(tSigData.TimeSigned);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid DNS data field " + dataFieldShow);
                            }
                            break;

                        case "dns.tsig.fudge":
                            dataField.AssertShowDecimal(tSigData.Fudge);
                            break;

                        case "dns.tsig.mac_size":
                            dataField.AssertShowDecimal(tSigData.MessageAuthenticationCode.Length);
                            break;

                        case "dns.tsig.mac":
                            dataField.AssertShow("");
                            Assert.AreEqual(1, dataField.Fields().Count());
                            var tsigSubfield = dataField.Fields().First();
                                tsigSubfield.AssertShow("No dissector for algorithm:" + GetWiresharkDomainName(tSigData.Algorithm));
                            tsigSubfield.AssertValue(tSigData.MessageAuthenticationCode);
                            break;

                        case "dns.tsig.original_id":
                            dataField.AssertShowDecimal(tSigData.OriginalId);
                            break;

                        case "dns.tsig.error":
                            dataField.AssertShowDecimal((ushort)tSigData.Error);
                            break;

                        case "dns.tsig.other_len":
                            dataField.AssertShowDecimal(tSigData.Other.Length);
                            break;

                        case "dns.tsig.other_data":
                            dataField.AssertValue(tSigData.Other);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;
                case DnsType.Null:   // 10.
                case DnsType.EId:    // 31.
                case DnsType.NimrodLocator: // 32.
                case DnsType.AtmA:   // 34.
                case DnsType.Sink:   // 40.
                case DnsType.NInfo:  // 56.
                case DnsType.RKey:   // 57.
                case DnsType.TrustAnchorLink: // 58.
                case DnsType.Cds:    // 59.
                case DnsType.UInfo:  // 100.
                case DnsType.Uid:    // 101.
                case DnsType.Gid:    // 102.
                case DnsType.Unspecified: // 103.
                case DnsType.Ixfr:   // 251.
                case DnsType.Axfr:   // 252.
                case DnsType.MailB:  // 253.
                case DnsType.MailA:  // 254.
                case DnsType.Any:    // 255.
                case DnsType.Uri:    // 256.
                case DnsType.CertificationAuthorityAuthorization:    // 257.
                case DnsType.TrustAnchor:     // 32768.
                default:
                    dataField.AssertName("");
                    dataField.AssertShow("Data");
                    break;
            }
        }

        private static string GetWiresharkDomainName(DnsDomainName domainName)
        {
            if (domainName.IsRoot)
                return "<Root>";
            return domainName.ToString().TrimEnd('.');
        }

        private static string GetWiresharkIpV6(IpV6Address data)
        {
            return data.ToString().ToLowerInvariant().TrimStart('0').Replace(":0", ":").Replace(":0", ":").Replace(":0", ":");
        }

        private static readonly Dictionary<string, DnsType> _wiresharkDnsTypeToDnsType =
            new Dictionary<string, DnsType>
            {
                {"Unused", DnsType.None},                               // 0
                {"MF", DnsType.MailForwarder},                          // 4
                {"SOA", DnsType.StartOfAuthority},                      // 6
                {"MB", DnsType.Mailbox},                                // 7
                {"MG", DnsType.MailGroup},                              // 8
                {"MR", DnsType.MailRename},                             // 9
                {"MX", DnsType.MailExchange},                           // 15
                {"RP", DnsType.ResponsiblePerson},                      // 17
                {"AFSDB", DnsType.AfsDatabase},                         // 18
                {"RT", DnsType.RouteThrough},                           // 21
                {"NSAP", DnsType.NetworkServiceAccessPoint},            // 22
                {"NSAP-PTR", DnsType.NetworkServiceAccessPointPointer}, // 23
                {"SIG", DnsType.Signature},                             // 24
                {"PX", DnsType.PointerX400},                            // 26
                {"NXT", DnsType.NextDomain},                            // 30
                {"NIMLOC", DnsType.NimrodLocator},                      // 32
                {"SRV", DnsType.ServerSelection},                       // 33
                {"KX", DnsType.KeyExchanger},                           // 36
                {"DS", DnsType.DelegationSigner},                       // 43
                {"SSHFP", DnsType.SshFingerprint},                      // 44
                {"RRSIG", DnsType.ResourceRecordSignature},             // 46
                {"DHCID", DnsType.DynamicHostConfigurationId},          // 49
                {"NSEC3PARAM", DnsType.NSec3Parameters},                // 51
                {"UNSPEC", DnsType.Unspecified},                        // 103
                {"TSIG", DnsType.TransactionSignature},                 // 250
                {"DLV", DnsType.DnsSecLookAsideValidation},             // 32769
            };

        private static readonly Dictionary<DnsType, string> _dnsTypeToWiresharkDnsType =
            _wiresharkDnsTypeToDnsType.ToDictionary(pair => pair.Value, pair => pair.Key);

        private static string GetWiresharkDnsType(DnsType type)
        {
            string wiresharkString;
            if (_dnsTypeToWiresharkDnsType.TryGetValue(type, out wiresharkString))
                return wiresharkString;

            return type.ToString().ToUpperInvariant();
        }

        private static bool TryGetDnsType(string dataFieldShow, out DnsType type)
        {
            if (Enum.TryParse(dataFieldShow.Split(':')[1].Split(' ')[1].Replace("-", ""), true, out type))
                return true;

            ushort typeValue;
            if (dataFieldShow.Contains("(") &&
                ushort.TryParse(dataFieldShow.Split('(', ')')[1], out typeValue))
            {
                type = (DnsType)typeValue;
                return true;
            }

            return _wiresharkDnsTypeToDnsType.TryGetValue(dataFieldShow.Split(new[] { ": " }, StringSplitOptions.None)[1].Split(' ')[0], out type);
        }

        private static int GetFlagCount(XElement flagField)
        {
            return flagField.Show().Replace(" ", "").TakeWhile(c => c == '.').Count();
        }

        private int _hipRendezvousServersIndex;
        private int _wksBitmapIndex;
        private int _nxtTypeIndex;
        private int _nSecTypeIndex;
        private int _nSec3TypeIndex;
        private int _txtIndex;
        private int _aplItemIndex;
    }
}
