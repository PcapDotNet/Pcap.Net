using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
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
                    field.AssertShowDecimal(dnsDatagram.Id);
                    break;

                case "dns.flags":
                    field.AssertShowDecimal(dnsDatagram.Subsegment(2, 2).ToArray().ReadUShort(0, Endianity.Big));
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

                            case "dns.flags.conflict": // TODO: Support LLMNR.
                            case "dns.flags.authoritative":
                                flagField.AssertShowDecimal(dnsDatagram.IsAuthoritativeAnswer);
                                break;

                            case "dns.flags.truncated":
                                flagField.AssertShowDecimal(dnsDatagram.IsTruncated);
                                break;

                            case "dns.flags.tentative": // TODO: Support LLMNR.
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

                case "dns.response_to":
                case "dns.time":
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
            if (resourceRecordFieldsArray.Length > resourceRecordsArray.Length)
            {
                var queryNameField = resourceRecordFieldsArray[resourceRecordsArray.Length].Fields().First();
                if (queryNameField.Name() == "dns.qry.name")
                    Assert.AreEqual("<Unknown extended label>", queryNameField.Show());
                else
                    Assert.AreEqual("dns.resp.name", queryNameField.Name());
            }
            else if (resourceRecordFieldsArray.Length < resourceRecordsArray.Length)
            {
                // TODO: This case should never happen when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10615 and https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10988 are fixed.
                XElement lastDnsTypeField = resourceRecordFieldsArray.Last().Fields().Skip(1).First();
                lastDnsTypeField.AssertName("dns.resp.type");
                DnsType lastDnsType = (DnsType)ushort.Parse(lastDnsTypeField.Show());
                Assert.IsTrue(new[] {DnsType.NextDomain, DnsType.Opt}.Contains(lastDnsType));
            }
            for (int i = 0; i != resourceRecordsArray.Length; ++i)
            {
                ResetRecordFields();
                var resourceRecordField = resourceRecordFieldsArray[i];
                var resourceRecord = resourceRecordsArray[i];
                int dnsResponseTypeIndex = 0;
                foreach (var resourceRecordAttributeField in resourceRecordField.Fields())
                {
                    switch (resourceRecordAttributeField.Name())
                    {
                        case "dns.qry.name":
                        case "dns.resp.name":
                            resourceRecordAttributeField.AssertShow(GetWiresharkDomainName(resourceRecord.DomainName));
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.qry.name.len":
                            resourceRecordAttributeField.AssertShowDecimal(resourceRecord.DomainName.IsRoot ? 0 : resourceRecord.DomainName.NonCompressedLength - 2);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.count.labels":
                            resourceRecordAttributeField.AssertShowDecimal(resourceRecord.DomainName.LabelsCount);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.qry.type":
                            resourceRecordAttributeField.AssertShowDecimal((ushort)resourceRecord.DnsType);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.resp.type":
                            DnsType expectedDnsType;
                            if (dnsResponseTypeIndex == 0)
                            {
                                expectedDnsType = resourceRecord.DnsType;
                            }
                            else
                            {
                                expectedDnsType = ((DnsResourceDataNextDomain)resourceRecord.Data).TypesExist.Skip(dnsResponseTypeIndex - 1).First();
                            }
                            resourceRecordAttributeField.AssertShowDecimal((ushort)expectedDnsType);
                            resourceRecordAttributeField.AssertNoFields();
                            ++dnsResponseTypeIndex;
                            break;

                        case "dns.qry.class":
                        case "dns.resp.class":
                            resourceRecordAttributeField.AssertShowDecimal((ushort)resourceRecord.DnsClass);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.resp.ttl":
                            resourceRecordAttributeField.AssertShowDecimal(resourceRecord.Ttl);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.resp.len":
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.resp.cache_flush": // TODO: Support MDNS.
                            resourceRecordAttributeField.AssertShowDecimal((ushort)resourceRecord.DnsClass >> 15);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.srv.service":
                            Assert.AreEqual(resourceRecord.DnsType, DnsType.ServerSelection);
                            resourceRecordAttributeField.AssertShow(resourceRecord.DomainName.IsRoot ? "<Root>" : resourceRecord.DomainName.ToString().Split('.')[0]);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.srv.proto":
                            Assert.AreEqual(resourceRecord.DnsType, DnsType.ServerSelection);
                            resourceRecordAttributeField.AssertShow(resourceRecord.DomainName.ToString().Split('.')[1]);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.srv.name":
                            Assert.AreEqual(resourceRecord.DnsType, DnsType.ServerSelection);
                            resourceRecordAttributeField.AssertShow(
                                resourceRecord.DomainName.ToString().Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries).Skip(2).SequenceToString("."));
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        case "dns.qry.qu":
                            // TODO: Support MDNS.
                            resourceRecordAttributeField.AssertShowDecimal(((ushort)resourceRecord.DnsClass >> 15) == 1);
                            resourceRecordAttributeField.AssertNoFields();
                            break;

                        default:
                            if (!CompareResourceRecordData(resourceRecordAttributeField, resourceRecord))
                                return;
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
            _spfTypeIndex = 0;
            _txtTypeIndex = 0;
            _nSecTypeIndex = 0;
            _nSec3TypeIndex = 0;
            _aplItemIndex = 0;
            _optOptionIndex = 0;
        }

        private bool CompareResourceRecordData(XElement dataField, DnsResourceRecord resourceRecord)
        {
            var data = resourceRecord.Data;
            string dataFieldName = dataField.Name();
            string dataFieldShow = dataField.Show();
            string dataFieldShowUntilColon = dataFieldShow.Split(':')[0];
            switch (resourceRecord.DnsType)
            {
                case DnsType.A:
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.a":
                            dataField.AssertShow(((DnsResourceDataIpV4)data).Data.ToString());
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Ns:
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.ns":
                            dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Mailbox:
                    dataField.AssertName("dns.mb");
                    dataField.AssertNoFields();
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;

                case DnsType.Md:
                    dataField.AssertName("dns.md");
                    dataField.AssertNoFields();
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;

                case DnsType.MailForwarder: 
                    dataField.AssertName("dns.mf");
                    dataField.AssertNoFields();
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    break;

                case DnsType.MailGroup:     // 8.
                    dataField.AssertName("dns.mg");
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.MailRename:    // 9.
                    dataField.AssertName("dns.mr");
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.CName:
                    dataField.AssertName("dns.cname");
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.StartOfAuthority:
                    var startOfAuthority = (DnsResourceDataStartOfAuthority)data;
                    dataField.AssertNoFields();
                    switch (dataField.Name())
                    {
                        case "dns.soa.mname":
                            dataField.AssertShow(GetWiresharkDomainName(startOfAuthority.MainNameServer));
                            break;

                        case "dns.soa.rname":
                            dataField.AssertShow(GetWiresharkDomainName(startOfAuthority.ResponsibleMailbox));
                            break;

                        case "dns.soa.serial_number":
                            dataField.AssertShowDecimal(startOfAuthority.Serial.Value);
                            break;

                        case "dns.soa.refresh_interval":
                            dataField.AssertShowDecimal(startOfAuthority.Refresh);
                            break;

                        case "dns.soa.retry_interval":
                            dataField.AssertShowDecimal(startOfAuthority.Retry);
                            break;

                        case "dns.soa.expire_limit":
                            dataField.AssertShowDecimal(startOfAuthority.Expire);
                            break;

                        case "dns.soa.mininum_ttl":
                            dataField.AssertShowDecimal(startOfAuthority.MinimumTtl);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    break;

                case DnsType.WellKnownService:
                    var wksData = (DnsResourceDataWellKnownService)data;
                    dataField.AssertNoFields();
                    switch (dataField.Name())
                    {
                        case "dns.wks.address":
                            dataField.AssertShow(wksData.Address.ToString());
                            break;

                        case "dns.wks.protocol":
                            dataField.AssertShowDecimal((byte)wksData.Protocol);
                            break;

                        case "dns.wks.bits":
                            while (wksData.Bitmap[_wksBitmapIndex] == 0x00)
                                ++_wksBitmapIndex;
                            dataField.AssertShowDecimal(wksData.Bitmap[_wksBitmapIndex++]);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    break;

                case DnsType.Ptr:
                    dataField.AssertName("dns.ptr.domain_name");
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.HInfo:
                    dataField.AssertNoFields();
                    var hInfoData = (DnsResourceDataHostInformation)data;
                    switch (dataFieldName)
                    {
                        case "dns.hinfo.cpu_length":
                            dataField.AssertShowDecimal(hInfoData.Cpu.Length);
                            break;

                        case "dns.hinfo.cpu":
                            dataField.AssertValue(hInfoData.Cpu);
                            break;

                        case "dns.hinfo.os_length":
                            dataField.AssertShowDecimal(hInfoData.OperatingSystem.Length);
                            break;

                        case "dns.hinfo.os":
                            dataField.AssertValue(hInfoData.OperatingSystem);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.MInfo:
                    dataField.AssertNoFields();
                    var mInfoData = (DnsResourceDataMailingListInfo)data;
                    switch (dataFieldName)
                    {
                        case "dns.minfo.r":
                            dataField.AssertShow(GetWiresharkDomainName(mInfoData.MailingList));
                            break;

                        case "dns.minfo.e":
                            dataField.AssertShow(GetWiresharkDomainName(mInfoData.ErrorMailbox));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.MailExchange:
                    var mxData = (DnsResourceDataMailExchange)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.mx.preference":
                            dataField.AssertShowDecimal(mxData.Preference);
                            break;

                        case "dns.mx.mail_exchange":
                            dataField.AssertShow(GetWiresharkDomainName(mxData.MailExchangeHost));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Txt: // 16.
                    var txtData = (DnsResourceDataText)data;
                    dataField.AssertNoFields();
                    switch (dataField.Name())
                    {
                        case "dns.txt.length":
                            dataField.AssertShowDecimal(txtData.Text[_txtTypeIndex].Length);
                            break;

                        case "dns.txt":
                            dataField.AssertValue(txtData.Text[_txtTypeIndex]);
                            ++_txtTypeIndex;
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    break;

                case DnsType.SenderPolicyFramework: // 99.
                    var spfData = (DnsResourceDataText)data;
                    dataField.AssertNoFields();
                    switch (dataField.Name())
                    {
                        case "dns.spf.length":
                            dataField.AssertShowDecimal(spfData.Text[_spfTypeIndex].Length);
                            break;

                        case "dns.spf":
                            dataField.AssertValue(spfData.Text[_spfTypeIndex]);
                            ++_spfTypeIndex;
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    break;

                case DnsType.ResponsiblePerson:
                    var rpData = (DnsResourceDataResponsiblePerson)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.rp.mailbox":
                            dataField.AssertShow(GetWiresharkDomainName(rpData.Mailbox));
                            break;

                        case "dns.rp.txt_rr":
                            dataField.AssertShow(GetWiresharkDomainName(rpData.TextDomain));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.AfsDatabase:
                    var afsDbData = (DnsResourceDataAfsDatabase)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.afsdb.subtype":
                            dataField.AssertShowDecimal((ushort)afsDbData.Subtype);
                            break;

                        case "dns.afsdb.hostname":
                            dataField.AssertShow(GetWiresharkDomainName(afsDbData.HostName));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);

                    }
                    break;

                case DnsType.X25:
                    var x25 = (DnsResourceDataString)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.x25.length":
                            dataField.AssertShowDecimal(x25.String.Length);
                            break;

                        case "dns.x25.psdn_address":
                            dataField.AssertValue(x25.String);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.Isdn:
                    var isdnData = (DnsResourceDataIsdn)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.idsn.length":
                            dataField.AssertShowDecimal(isdnData.IsdnAddress.Length);
                            break;

                        case "dns.idsn.address":
                            dataField.AssertValue(isdnData.IsdnAddress);
                            break;

                        case "dns.idsn.sa.length":
                            dataField.AssertShowDecimal(isdnData.Subaddress.Length);
                            break;

                        case "dns.idsn.sa.address":
                            dataField.AssertValue(isdnData.Subaddress);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.RouteThrough:
                    dataField.AssertNoFields();
                    var rtData = (DnsResourceDataRouteThrough)data;
                    switch (dataFieldName)
                    {
                        case "dns.rt.subtype":
                            dataField.AssertShowDecimal(rtData.Preference);
                            break;

                        case "dns.rt.intermediate_host":
                            dataField.AssertShow(GetWiresharkDomainName(rtData.IntermediateHost));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
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
                    dataField.AssertNoFields();
                    break;

                case DnsType.NetworkServiceAccessPointPointer:
                    dataField.AssertName("dns.nsap_ptr.owner");
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.Key:
                    var keyData = (DnsResourceDataKey)data;
                    switch (dataFieldName)
                    {
                        case "dns.key.flags":
                            foreach (var flagField in dataField.Fields())
                            {
                                flagField.AssertNoFields();
                                int flagCount = GetFlagCount(flagField);
                                switch (flagCount)
                                {
                                    case 0:
                                        flagField.AssertShowDecimal(keyData.AuthenticationProhibited);
                                        break;

                                    case 1:
                                        flagField.AssertShowDecimal(keyData.ConfidentialityProhibited);
                                        break;

                                    case 2:
                                        flagField.AssertShowDecimal(keyData.Experimental);
                                        break;

                                    case 5:
                                        flagField.AssertShowDecimal(keyData.UserAssociated);
                                        break;

                                    case 6:
                                        flagField.AssertShowDecimal(keyData.NameType == DnsKeyNameType.NonZoneEntity);
                                        break;

                                    case 8:
                                        flagField.AssertShowDecimal(keyData.IpSec);
                                        break;

                                    case 9:
                                        flagField.AssertShowDecimal(keyData.Email);
                                        break;

                                    case 12:
                                        flagField.AssertShowDecimal((byte)keyData.Signatory);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid flag count " + flagCount);
                                }
                            }
                            break;

                        case "dns.key.protocol":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal((byte)keyData.Protocol);
                            break;

                        case "dns.key.algorithm":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal((byte)keyData.Algorithm);
                            break;

                        case "dns.key.key_id":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal(keyData.KeyTag);
                            break;

                        case "dns.key.public_key":
                            dataField.AssertNoFields();
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
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Signature:   // 24.
                case DnsType.ResourceRecordSignature: // 46.
                    dataField.AssertNoFields();
                    var sigData = (DnsResourceDataSignature)data;
                    switch (dataFieldName)
                    {
                        case "dns.rrsig.type_covered":
                            dataField.AssertShowDecimal((ushort)sigData.TypeCovered);
                            break;

                        case "dns.rrsig.algorithm":
                            dataField.AssertShowDecimal((byte)sigData.Algorithm);
                            break;

                        case "dns.rrsig.labels":
                            dataField.AssertShowDecimal(sigData.Labels);
                            break;

                        case "dns.rrsig.original_ttl":
                            dataField.AssertShowDecimal(sigData.OriginalTtl);
                            break;

                        case "dns.rrsig.signature_expiration":
                            dataField.AssertValue(sigData.SignatureExpiration);
                            break;

                        case "dns.rrsig.signature_inception":
                            dataField.AssertValue(sigData.SignatureInception);
                            break;

                        case "dns.rrsig.key_tag":
                            dataField.AssertShowDecimal(sigData.KeyTag);
                            break;

                        case "dns.rrsig.signers_name":
                            dataField.AssertShow(GetWiresharkDomainName(sigData.SignersName));
                            break;

                        case "dns.rrsig.signature":
                            dataField.AssertValue(sigData.Signature);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.PointerX400:
                    var pxData = (DnsResourceDataX400Pointer)data;
                    switch (dataField.Name())
                    {
                        case "dns.px.preference":
                            dataField.AssertShowDecimal(pxData.Preference);
                            break;

                        case "dns.px.map822":
                            dataField.AssertShow(GetWiresharkDomainName(pxData.Map822));
                            break;

                        case "dns.px.map400":
                            dataField.AssertShow(GetWiresharkDomainName(pxData.MapX400));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.GeographicalPosition:
                    dataField.AssertNoFields();
                    var gposData = (DnsResourceDataGeographicalPosition)data;
                    switch (dataFieldName)
                    {
                        case "dns.gpos.longitude_length":
                            dataField.AssertShowDecimal(gposData.Longitude.Length);
                            break;

                        case "dns.gpos.longitude":
                            dataField.AssertShow(gposData.Longitude);
                            break;

                        case "dns.gpos.latitude_length":
                            dataField.AssertShowDecimal(gposData.Latitude.Length);
                            break;

                        case "dns.gpos.latitude":
                            dataField.AssertShow(gposData.Latitude);
                            break;

                        case "dns.gpos.altitude_length":
                            dataField.AssertShowDecimal(gposData.Altitude.Length);
                            break;

                        case "dns.gpos.altitude":
                            dataField.AssertShow(gposData.Altitude);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    break;

                case DnsType.Aaaa:
                    dataField.AssertName("dns.aaaa");
                    dataField.AssertShow(GetWiresharkIpV6(((DnsResourceDataIpV6)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.Location:
                    var locData = (DnsResourceDataLocationInformation)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.loc.version":
                            dataField.AssertShowDecimal(locData.Version);
                            break;

                        case "dns.loc.unknown_data":
                            Assert.AreNotEqual(0, locData.Version);
                            break;

                        case "dns.loc.size":
                            Assert.AreEqual(0, locData.Version);
                            string sizeValue = dataField.Showname().Split('(', ')')[1];
                            Assert.AreEqual(GetPrecisionValueString(locData.Size), sizeValue);
                            break;

                        case "dns.loc.horizontal_precision":
                            Assert.AreEqual(0, locData.Version);
                            string horizontalPrecisionValue = dataField.Showname().Split('(', ')')[1];
                            Assert.AreEqual(GetPrecisionValueString(locData.HorizontalPrecision), horizontalPrecisionValue);
                            break;

                        case "dns.loc.vertial_precision":
                            Assert.AreEqual(0, locData.Version);
                            string verticalPrecisionValue = dataField.Showname().Split('(', ')')[1];
                            Assert.AreEqual(GetPrecisionValueString(locData.VerticalPrecision), verticalPrecisionValue);
                            break;

                        case "dns.loc.latitude":
                            Assert.AreEqual(0, locData.Version);
                            dataField.AssertShowDecimal(locData.Latitude);
                            break;

                        case "dns.loc.longitude":
                            Assert.AreEqual(0, locData.Version);
                            dataField.AssertShowDecimal(locData.Longitude);
                            break;

                        case "dns.loc.altitude":
                            Assert.AreEqual(0, locData.Version);
                            dataField.AssertShowDecimal(locData.Altitude);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.NextDomain:
                    var nxtData = (DnsResourceDataNextDomain)data;
                    switch (dataField.Name())
                    {
                        case "dns.nxt.next_domain_name":
                            dataField.AssertShow(GetWiresharkDomainName(nxtData.NextDomainName));
                            break;

                        case "":
                            DnsType actualType = nxtData.TypesExist.Skip(_nxtTypeIndex++).First();
                            DnsType expectedType;
                            if (!TryGetDnsType(dataFieldShow, out expectedType))
                                throw new InvalidOperationException(string.Format("Can't parse DNS field {0} : {1}", dataFieldShow, actualType));
                            Assert.AreEqual(expectedType, actualType);
                            return false;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.ServerSelection:
                    dataField.AssertNoFields();
                    var srvData = (DnsResourceDataServerSelection)data;
                    switch (dataFieldName)
                    {
                        case "dns.srv.priority":
                            dataField.AssertShowDecimal(srvData.Priority);
                            break;

                        case "dns.srv.weight":
                            dataField.AssertShowDecimal(srvData.Weight);
                            break;

                        case "dns.srv.port":
                            dataField.AssertShowDecimal(srvData.Port);
                            break;

                        case "dns.srv.target":
                            dataField.AssertShow(GetWiresharkDomainName(srvData.Target));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.NaPtr:
                    var naPtrData = (DnsResourceDataNamingAuthorityPointer)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.naptr.order":
                            dataField.AssertShowDecimal(naPtrData.Order);
                            break;

                        case "dns.naptr.preference":
                            dataField.AssertShowDecimal(naPtrData.Preference);
                            break;

                        case "dns.naptr.flags_length":
                            dataField.AssertShowDecimal(naPtrData.Flags.Length);
                            break;

                        case "dns.naptr.flags":
                            dataField.AssertValue(naPtrData.Flags);
                            break;

                        case "dns.naptr.service_length":
                            dataField.AssertShowDecimal(naPtrData.Services.Length);
                            break;

                        case "dns.naptr.service":
                            dataField.AssertValue(naPtrData.Services);
                            break;

                        case "dns.naptr.regex_length":
                            dataField.AssertShowDecimal(naPtrData.RegularExpression.Length);
                            break;

                        case "dns.naptr.regex":
                            dataField.AssertValue(naPtrData.RegularExpression);
                            break;

                        case "dns.naptr.replacement_length":
                            dataField.AssertShowDecimal(naPtrData.Replacement.NonCompressedLength);
                            break;

                        case "dns.naptr.replacement":
                            dataField.AssertShow(GetWiresharkDomainName(naPtrData.Replacement));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.KeyExchanger:
                    dataField.AssertNoFields();
                    var kxData = (DnsResourceDataKeyExchanger)data;
                    switch (dataFieldName)
                    {
                        case "dns.kx.preference":
                            dataField.AssertShowDecimal(kxData.Preference);
                            break;

                        case "dns.kx.key_exchange":
                            dataField.AssertShow(GetWiresharkDomainName(kxData.KeyExchangeHost));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.Cert:
                    dataField.AssertNoFields();
                    var certData = (DnsResourceDataCertificate)data;
                    switch (dataFieldName)
                    {
                        case "dns.cert.type":
                            dataField.AssertShowDecimal((ushort)certData.CertificateType);
                            break;

                        case "dns.cert.key_tag":
                            dataField.AssertShowDecimal(certData.KeyTag);
                            break;

                        case "dns.cert.algorithm":
                            dataField.AssertShowDecimal((byte)certData.Algorithm);
                            break;

                        case "dns.cert.certificate":
                            dataField.AssertValue(certData.Certificate);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.A6:
                    var a6Data = (DnsResourceDataA6)data;
                    switch (dataFieldName)
                    {
                        case "dns.a6.prefix_len":
                            dataField.AssertShowDecimal(a6Data.PrefixLength);
                            break;

                        case "dns.a6.address_suffix":
                            Assert.AreEqual(new IpV6Address(dataFieldShow), a6Data.AddressSuffix);
                            break;

                        case "dns.a6.prefix_name":
                            dataField.AssertShow(GetWiresharkDomainName(a6Data.PrefixName));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.DName:
                    dataField.AssertName("dns.dname");
                    dataField.AssertShow(GetWiresharkDomainName(((DnsResourceDataDomainName)data).Data));
                    dataField.AssertNoFields();
                    break;

                case DnsType.Opt:
                    var optResourceRecord = (DnsOptResourceRecord)resourceRecord;
                    var optData = (DnsResourceDataOptions)data;
                    switch (dataFieldName)
                    {
                        case "dns.rr.udp_payload_size":
                            _optOptionIndex = 0;
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal(optResourceRecord.SendersUdpPayloadSize);
                            break;

                        case "dns.resp.ext_rcode":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal(optResourceRecord.ExtendedReturnCode);
                            break;

                        case "dns.resp.edns0_version":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal((byte)optResourceRecord.Version);
                            break;

                        case "dns.resp.z":
                            DnsOptFlags flags = optResourceRecord.Flags;
                            dataField.AssertShowDecimal((ushort)flags);
                            foreach (XElement subfield in dataField.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "dns.resp.z.do":
                                        subfield.AssertShowDecimal((flags & DnsOptFlags.DnsSecOk) == DnsOptFlags.DnsSecOk);
                                        break;

                                    case "dns.resp.z.reserved":
                                        subfield.AssertShowDecimal(0);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid DNS data subfield name " + subfield.Name());
                                }
                            }
                            break;

                        case "dns.opt":
                            foreach (XElement subfield in dataField.Fields())
                            {
                                DnsOption dnsOption = optData.Options.Options[_optOptionIndex];
                                var clientSubnet = dnsOption as DnsOptionClientSubnet;
                                switch (subfield.Name())
                                {
                                    case "dns.opt.code":
                                        subfield.AssertNoFields();
                                        subfield.AssertShowDecimal((ushort)dnsOption.Code);
                                        break;

                                    case "dns.opt.len":
                                        subfield.AssertShowDecimal(dnsOption.DataLength);
                                        if (subfield.Fields().Any())
                                        {
                                            Assert.AreEqual(1, subfield.Fields().Count());
                                            subfield.Fields().First().AssertName("_ws.expert");
                                        }
                                        break;

                                    case "dns.opt.data":
                                        subfield.AssertNoFields();
                                        switch (dnsOption.Code)
                                        {
                                            case DnsOptionCode.UpdateLease:
                                                subfield.AssertValue((uint)((DnsOptionUpdateLease)dnsOption).Lease);
                                                break;

                                            case DnsOptionCode.LongLivedQuery:
                                                byte[] expectedLongLivedQueryValue = new byte[dnsOption.DataLength];
                                                var longLivedQuery = (DnsOptionLongLivedQuery)dnsOption;
                                                int longLivedQueryOffset = 0;
                                                expectedLongLivedQueryValue.Write(ref longLivedQueryOffset, longLivedQuery.Version, Endianity.Big);
                                                expectedLongLivedQueryValue.Write(ref longLivedQueryOffset, (ushort)longLivedQuery.OpCode, Endianity.Big);
                                                expectedLongLivedQueryValue.Write(ref longLivedQueryOffset, (ushort)longLivedQuery.ErrorCode, Endianity.Big);
                                                expectedLongLivedQueryValue.Write(ref longLivedQueryOffset, longLivedQuery.Id, Endianity.Big);
                                                expectedLongLivedQueryValue.Write(ref longLivedQueryOffset, longLivedQuery.LeaseLife, Endianity.Big);
                                                subfield.AssertValue(expectedLongLivedQueryValue);
                                                break;

                                            case DnsOptionCode.ClientSubnet:
                                                byte[] expectedClientSubnetValue = new byte[dnsOption.DataLength];
                                                int clientSubnetOffset = 0;
                                                expectedClientSubnetValue.Write(ref clientSubnetOffset, (ushort)clientSubnet.Family, Endianity.Big);
                                                expectedClientSubnetValue.Write(ref clientSubnetOffset, clientSubnet.SourceNetmask);
                                                expectedClientSubnetValue.Write(ref clientSubnetOffset, clientSubnet.ScopeNetmask);
                                                expectedClientSubnetValue.Write(ref clientSubnetOffset, clientSubnet.Address);
                                                subfield.AssertValue(expectedClientSubnetValue);
                                                break;

                                            default:
                                                subfield.AssertValue(((DnsOptionAnything)dnsOption).Data);
                                                break;
                                        }
                                        if (dnsOption.Code != DnsOptionCode.ClientSubnet)
                                            ++_optOptionIndex;
                                        break;

                                    case "dns.opt.client.family":
                                        subfield.AssertNoFields();
                                        subfield.AssertShowDecimal((ushort)clientSubnet.Family);
                                        break;

                                    case "dns.opt.client.netmask":
                                        subfield.AssertNoFields();
                                        subfield.AssertShowDecimal(clientSubnet.SourceNetmask);
                                        break;

                                    case "dns.opt.client.scope":
                                        subfield.AssertNoFields();
                                        subfield.AssertShowDecimal(clientSubnet.ScopeNetmask);
                                        break;

                                    case "dns.opt.client.addr":
                                    case "dns.opt.client.addr4":
                                    case "dns.opt.client.addr6":
                                        subfield.AssertNoFields();
                                        if (clientSubnet.Address.Length <= 16)
                                        {
                                            subfield.AssertValue(clientSubnet.Address);
                                        }
                                        else
                                        {
                                            subfield.AssertValue(clientSubnet.Address.Take(16));
                                            // TODO: Remove this return when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10988 is fixed.
                                            return false;
                                        }
                                        ++_optOptionIndex;
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid DNS data subfield name " + subfield.Name());
                                }
                            }
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.AddressPrefixList:
                    var aplData = (DnsResourceDataAddressPrefixList)data;
                    switch (dataFieldName)
                    {
                        case "dns.apl.address_family":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal((ushort)aplData.Items[_aplItemIndex++].AddressFamily);

                            break;

                        case "dns.apl.coded_prefix":
                            dataField.AssertShowDecimal(aplData.Items[_aplItemIndex - 1].PrefixLength);
                            break;

                        case "dns.apl.negation":
                            dataField.AssertShowDecimal(aplData.Items[_aplItemIndex - 1].Negation);
                            break;

                        case "dns.apl.afdlength":
                            dataField.AssertShowDecimal(aplData.Items[_aplItemIndex - 1].AddressFamilyDependentPart.Length);
                            break;

                        case "dns.apl.afdpart.data":
                        case "dns.apl.afdpart.ipv4":
                        case "dns.apl.afdpart.ipv6":
                            if (dataFieldName != "dns.apl.afdpart.data")
                            {
                                Assert.AreEqual(dataFieldName == "dns.apl.afdpart.ipv4" ? AddressFamily.IpV4 : AddressFamily.IpV6,
                                                aplData.Items[_aplItemIndex - 1].AddressFamily);
                            }
                            dataField.AssertValue(aplData.Items[_aplItemIndex - 1].AddressFamilyDependentPart);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.DelegationSigner:  // 43.
                case DnsType.DnsSecLookAsideValidation: // 32769.
                    dataField.AssertNoFields();
                    var dsData = (DnsResourceDataDelegationSigner)data;
                    switch (dataFieldName)
                    {
                        case "dns.ds.key_id":
                            dataField.AssertShowDecimal(dsData.KeyTag);
                            break;

                        case "dns.ds.algorithm":
                            dataField.AssertShowDecimal((byte)dsData.Algorithm);
                            break;

                        case "dns.ds.digest_type":
                            dataField.AssertShowDecimal((byte)dsData.DigestType);
                            break;

                        case "dns.ds.digest":
                            dataField.AssertValue(dsData.Digest);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.SshFingerprint:
                    var sshFpData = (DnsResourceDataSshFingerprint)data;
                    dataField.AssertNoFields();
                    switch (dataFieldName)
                    {
                        case "dns.sshfp.algorithm":
                            dataField.AssertShowDecimal((byte)sshFpData.Algorithm);
                            break;

                        case "dns.sshfp.fingerprint.type":
                            dataField.AssertShowDecimal((byte)sshFpData.FingerprintType);
                            break;

                        case "dns.sshfp.fingerprint":
                            dataField.AssertValue(sshFpData.Fingerprint);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.IpSecKey:
                    dataField.AssertNoFields();
                    var ipSecKeyData = (DnsResourceDataIpSecKey)data;
                    switch (dataField.Name())
                    {
                        case "dns.ipseckey.gateway_precedence":
                            dataField.AssertShowDecimal(ipSecKeyData.Precedence);
                            break;

                        case "dns.ipseckey.gateway_type":
                            dataField.AssertShowDecimal((byte)ipSecKeyData.GatewayType);
                            break;

                        case "dns.ipseckey.gateway_algorithm":
                            dataField.AssertShowDecimal((byte)ipSecKeyData.Algorithm);
                            break;

                        case "dns.ipseckey.gateway_ipv4":
                            dataField.AssertShow(((DnsGatewayIpV4)ipSecKeyData.Gateway).Value.ToString());
                            break;

                        case "dns.ipseckey.gateway_ipv6":
                            dataField.AssertValue(((DnsGatewayIpV6)ipSecKeyData.Gateway).Value.ToValue());
                            break;

                        case "dns.ipseckey.gateway_dns":
                            dataField.AssertShow(GetWiresharkDomainName(((DnsGatewayDomainName)ipSecKeyData.Gateway).Value));
                            break;

                        case "dns.ipseckey.public_key":
                            dataField.AssertValue(ipSecKeyData.PublicKey);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    break;

                case DnsType.NSec:
                    var nSecData = (DnsResourceDataNextDomainSecure)data;
                    switch (dataField.Name())
                    {
                        case "dns.nsec.next_domain_name":
                            dataField.AssertShow(GetWiresharkDomainName(nSecData.NextDomainName));
                            break;

                        case "":
                            DnsType actualType = nSecData.TypesExist[_nSecTypeIndex++];
                            DnsType expectedType;
                            if (!TryGetDnsType(dataFieldShow, out expectedType))
                                throw new InvalidOperationException(string.Format("Failed parsing type from {0} : {1}", dataFieldShow, actualType));

                            Assert.AreEqual(expectedType, actualType);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataField.Name());
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.DnsKey:
                    var dnsKeyData = (DnsResourceDataDnsKey)data;
                    switch (dataFieldName)
                    {
                        case "dns.dnskey.flags":
                            foreach (XElement subfield in dataField.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "dns.dnskey.flags.zone_key":
                                        subfield.AssertShowDecimal(dnsKeyData.ZoneKey);
                                        break;

                                    case "dns.dnskey.flags.key_revoked":
                                        subfield.AssertShowDecimal(dnsKeyData.Revoke);
                                        break;

                                    case "dns.dnskey.flags.secure_entry_point":
                                        subfield.AssertShowDecimal(dnsKeyData.SecureEntryPoint);
                                        break;

                                    case "dns.dnskey.flags.reserved":
                                        subfield.AssertShowDecimal(0);
                                        break;

                                    case "dns.dnskey.protocol":
                                        subfield.AssertShowDecimal(dnsKeyData.Protocol);
                                        break;

                                    case "dns.dnskey.algorithm":
                                        subfield.AssertShowDecimal((byte)dnsKeyData.Algorithm);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid DNS flags subfield name " + subfield.Name());
                                }
                            }
                            break;

                        case "dns.dnskey.key_id":
                            dataField.AssertNoFields();
                            dataField.AssertShowDecimal(dnsKeyData.KeyTag);
                            break;

                        case "dns.dnskey.public_key":
                            dataField.AssertNoFields();
                            dataField.AssertValue(dnsKeyData.PublicKey);
                            break;
                            
                        default:
                            throw new InvalidOperationException("Invalid DNS resource data field name " + dataFieldName);
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
                    dataField.AssertNoFields();
                    break;

                case DnsType.NSec3:
                    var nSec3Data = (DnsResourceDataNextDomainSecure3)data;
                    switch (dataFieldName)
                    {
                        case "dns.nsec3.algo":
                            dataField.AssertShowDecimal((byte)nSec3Data.HashAlgorithm);
                            dataField.AssertNoFields();
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
                            dataField.AssertNoFields();
                            break;

                        case "dns.nsec3.salt_length":
                            dataField.AssertShowDecimal(nSec3Data.Salt.Length);
                            dataField.AssertNoFields();
                            break;

                        case "dns.nsec3.salt_value":
                            dataField.AssertValue(nSec3Data.Salt);
                            dataField.AssertNoFields();
                            break;

                        case "dns.nsec3.hash_length":
                            dataField.AssertShowDecimal(nSec3Data.NextHashedOwnerName.Length);
                            dataField.AssertNoFields();
                            break;

                        case "dns.nsec3.hash_value":
                            dataField.AssertValue(nSec3Data.NextHashedOwnerName);
                            dataField.AssertNoFields();
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
                            dataField.AssertNoFields();
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
                    dataField.AssertNoFields();
                    break;

                case DnsType.Hip:
                    var hipData = (DnsResourceDataHostIdentityProtocol)data;
                    switch (dataFieldName)
                    {
                        case "dns.hip.hit":
                            dataField.AssertShow(hipData.HostIdentityTag);
                            break;

                        case "dns.hip.pk":
                            dataField.AssertShow(hipData.PublicKey);
                            break;

                        case "dns.hip.hit.length":
                            dataField.AssertShowDecimal(hipData.HostIdentityTag.Length);
                            break;

                        case "dns.hip.hit.pk.algo":
                            dataField.AssertShowDecimal((byte)hipData.PublicKeyAlgorithm);
                            break;

                        case "dns.hip.pk.length":
                            dataField.AssertShowDecimal(hipData.PublicKey.Length);
                            break;

                        case "dns.hip.rendezvous_server":
                            dataField.AssertShow(GetWiresharkDomainName(hipData.RendezvousServers[_hipRendezvousServersIndex++]));
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    dataField.AssertNoFields();
                    break;

                case DnsType.TKey:
                    dataField.AssertNoFields();
                    var tKeyData = (DnsResourceDataTransactionKey)data;
                    switch (dataFieldName)
                    {
                        case "dns.tkey.algo_name":
                            dataField.AssertShow(GetWiresharkDomainName(tKeyData.Algorithm));
                            break;

                        case "dns.tkey.signature_inception":
                            dataField.AssertValue(tKeyData.Inception);
                            break;

                        case "dns.tkey.signature_expiration":
                            dataField.AssertValue(tKeyData.Expiration);
                            break;

                        case "dns.tkey.mode":
                            dataField.AssertShowDecimal((ushort)tKeyData.Mode);
                            break;

                        case "dns.tkey.error":
                            dataField.AssertShowDecimal((ushort)tKeyData.Error);
                            break;

                        case "dns.tkey.key_size":
                            dataField.AssertShowDecimal(tKeyData.Key.Length);
                            break;

                        case "dns.tkey.key_data":
                            dataField.AssertValue(tKeyData.Key);
                            break;

                        case "dns.tkey.other_size":
                            dataField.AssertShowDecimal(tKeyData.Other.Length);
                            break;

                        case "dns.tkey.other_data":
                            dataField.AssertValue(tKeyData.Other);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field " + dataFieldName);
                    }
                    break;

                case DnsType.TransactionSignature:
                    var tSigData = (DnsResourceDataTransactionSignature)data;
                    switch (dataFieldName)
                    {
                        case "dns.tsig.algorithm_name":
                            dataField.AssertShow(GetWiresharkDomainName(tSigData.Algorithm));
                            dataField.AssertNoFields();
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
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.fudge":
                            dataField.AssertShowDecimal(tSigData.Fudge);
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.mac_size":
                            dataField.AssertShowDecimal(tSigData.MessageAuthenticationCode.Length);
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.mac":
                            dataField.AssertShow("");
                            Assert.AreEqual(1, dataField.Fields().Count());
                            var tsigSubfield = dataField.Fields().First();
                            tsigSubfield.AssertName("_ws.expert");
                            break;

                        case "dns.tsig.original_id":
                            dataField.AssertShowDecimal(tSigData.OriginalId);
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.error":
                            dataField.AssertShowDecimal((ushort)tSigData.Error);
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.other_len":
                            dataField.AssertShowDecimal(tSigData.Other.Length);
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.other_data":
                            dataField.AssertValue(tSigData.Other);
                            dataField.AssertNoFields();
                            break;

                        case "dns.tsig.time_signed":
                            dataField.AssertValue(tSigData.TimeSigned);
                            dataField.AssertNoFields();
                            break;

                        case "_ws.expert":
                            break;

                        default:
                            throw new InvalidOperationException("Invalid DNS data field name " + dataFieldName);
                    }
                    break;

                case DnsType.Null:
                    dataField.AssertNoFields();
                    dataField.AssertName("dns.null");
                    dataField.AssertValue(((DnsResourceDataAnything)data).Data);
                    break;

                case DnsType.CertificationAuthorityAuthorization:
                    var certificationAuthorityAuthorization = (DnsResourceDataCertificationAuthorityAuthorization)data;
                    switch (dataField.Name())
                    {
                        case "dns.caa.flags":
                            dataField.AssertShowDecimal((byte)certificationAuthorityAuthorization.Flags);
                            foreach (XElement subfield in dataField.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "dns.caa.flags.issuer_critical":
                                        subfield.AssertShowDecimal((certificationAuthorityAuthorization.Flags &
                                                                    DnsCertificationAuthorityAuthorizationFlags.Critical) ==
                                                                   DnsCertificationAuthorityAuthorizationFlags.Critical);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                                }

                            }
                            break;

                        case "dns.caa.unknown":
                        case "dns.caa.issue":
                            foreach (XElement subfield in dataField.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "dns.caa.tag_length":
                                        subfield.AssertShowDecimal(certificationAuthorityAuthorization.Tag.Length);
                                        break;

                                    case "dns.caa.tag":
                                        subfield.AssertValue(certificationAuthorityAuthorization.Tag);
                                        break;

                                    case "dns.caa.value":
                                        subfield.AssertValue(certificationAuthorityAuthorization.Value);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                                }

                            }
                            break;

                        default:
                            throw new InvalidOperationException("Invalid field " + dataField.Name());
                    }
                    break;

                case DnsType.EId:                                 // 31.
                case DnsType.NimrodLocator:                       // 32.
                case DnsType.AsynchronousTransferModeAddress:     // 34.
                case DnsType.Sink:                                // 40.
                case DnsType.NInfo:                               // 56.
                case DnsType.RKey:                                // 57.
                case DnsType.TrustAnchorLink:                     // 58.
                case DnsType.ChildDelegationSigner:               // 59.
                case DnsType.UInfo:                               // 100.
                case DnsType.Uid:                                 // 101.
                case DnsType.Gid:                                 // 102.
                case DnsType.Unspecified:                         // 103.
                case DnsType.Ixfr:                                // 251.
                case DnsType.Axfr:                                // 252.
                case DnsType.MailB:                               // 253.
                case DnsType.MailA:                               // 254.
                case DnsType.Any:                                 // 255.
                case DnsType.Uri:                                 // 256.
                case DnsType.TrustAnchor:                         // 32768.
                default:
                    if (dataField.Name() == "_ws.expert")
                    {
                        dataField.AssertShowname("Expert Info (Note/Undecoded): Dissector for DNS Type (" + (ushort)resourceRecord.DnsType +
                                                 ") code not implemented, Contact Wireshark developers if you want this supported");
                    }
                    else
                    {
                        dataField.AssertName("dns.data");
                    }
                    break;
            }
            return true;
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
                {"WKS", DnsType.WellKnownService},                      // 11
                {"MX", DnsType.MailExchange},                           // 15
                {"RP", DnsType.ResponsiblePerson},                      // 17
                {"AFSDB", DnsType.AfsDatabase},                         // 18
                {"RT", DnsType.RouteThrough},                           // 21
                {"NSAP", DnsType.NetworkServiceAccessPoint},            // 22
                {"NSAP-PTR", DnsType.NetworkServiceAccessPointPointer}, // 23
                {"SIG", DnsType.Signature},                             // 24
                {"PX", DnsType.PointerX400},                            // 26
                {"GPOS", DnsType.GeographicalPosition},                 // 27
                {"LOC", DnsType.Location},                              // 29
                {"NXT", DnsType.NextDomain},                            // 30
                {"NIMLOC", DnsType.NimrodLocator},                      // 32
                {"SRV", DnsType.ServerSelection},                       // 33
                {"ATMA", DnsType.AsynchronousTransferModeAddress},      // 34
                {"KX", DnsType.KeyExchanger},                           // 36
                {"APL", DnsType.AddressPrefixList},                     // 42
                {"DS", DnsType.DelegationSigner},                       // 43
                {"SSHFP", DnsType.SshFingerprint},                      // 44
                {"RRSIG", DnsType.ResourceRecordSignature},             // 46
                {"DHCID", DnsType.DynamicHostConfigurationId},          // 49
                {"NSEC3PARAM", DnsType.NSec3Parameters},                // 51
                {"TALINK", DnsType.TrustAnchorLink},                    // 58
                {"CDS", DnsType.ChildDelegationSigner},                 // 59
                {"SPF", DnsType.SenderPolicyFramework},                 // 99
                {"UNSPEC", DnsType.Unspecified},                        // 103
                {"TSIG", DnsType.TransactionSignature},                 // 250
                {"*", DnsType.Any},                                     // 255
                {"CAA", DnsType.CertificationAuthorityAuthorization},   // 257
                {"TA", DnsType.TrustAnchor},                            // 32768
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

            string wiresharkDnsType = dataFieldShow.Split(new[] {": "}, StringSplitOptions.None)[1].Split(' ', '(')[0];
            return _wiresharkDnsTypeToDnsType.TryGetValue(wiresharkDnsType, out type);
        }

        private static int GetFlagCount(XElement flagField)
        {
            return flagField.Showname().Replace(" ", "").TakeWhile(c => c == '.').Count();
        }

        private static string GetPrecisionValueString(ulong value)
        {
            double resultValue = value / 100.0;

            if (resultValue < 1000000)
                return resultValue + " m";

            int log = (int)Math.Log10(resultValue);
            resultValue /= Math.Pow(10, log);
            return resultValue + "e+00" + log;
        }

        private int _hipRendezvousServersIndex;
        private int _wksBitmapIndex;
        private int _nxtTypeIndex;
        private int _spfTypeIndex;
        private int _txtTypeIndex;
        private int _nSecTypeIndex;
        private int _nSec3TypeIndex;
        private int _aplItemIndex;
        private int _optOptionIndex;
    }
}
