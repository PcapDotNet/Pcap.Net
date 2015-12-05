using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal class WiresharkDatagramComparerIpV4 : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "IpV4"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IpV4Datagram ipV4Datagram = (IpV4Datagram)datagram;
            switch (field.Name())
            {
                case "ip.version":
                    field.AssertShowDecimal(ipV4Datagram.Version);
                    field.AssertNoFields();
                    break;

                case "ip.hdr_len":
                    field.AssertShowDecimal(ipV4Datagram.HeaderLength);
                    field.AssertNoFields();
                    break;

                case "ip.dsfield":
                    field.AssertShowDecimal((int)ipV4Datagram.TypeOfService);
                    // TODO: Parse TypeOfService to Differentiated Services and ECN.
                    break;

                case "ip.len":
                    field.AssertShowDecimal(ipV4Datagram.TotalLength);
                    field.AssertNoFields();
                    break;

                case "ip.id":
                    field.AssertShowDecimal(ipV4Datagram.Identification);
                    field.AssertNoFields();
                    break;

                case "ip.flags":
                    field.AssertShowDecimal(((ushort)ipV4Datagram.Fragmentation.Options >> 13));
                    foreach (XElement subfield in field.Fields())
                    {
                        subfield.AssertNoFields();
                        switch (subfield.Name())
                        {
                            case "ip.flags.rb":
                                break;

                            case "ip.flags.df":
                                subfield.AssertShowDecimal((ipV4Datagram.Fragmentation.Options & IpV4FragmentationOptions.DoNotFragment) ==
                                                           IpV4FragmentationOptions.DoNotFragment);
                                break;

                            case "ip.flags.mf":
                                subfield.AssertShowDecimal((ipV4Datagram.Fragmentation.Options & IpV4FragmentationOptions.MoreFragments) ==
                                                           IpV4FragmentationOptions.MoreFragments);
                                break;

                            default:
                                throw new InvalidOperationException(string.Format("Invalid ip flags subfield {0}", subfield.Name()));
                        }
                    }
                    break;

                case "ip.frag_offset":
                    field.AssertShowDecimal(ipV4Datagram.Fragmentation.Offset);
                    field.AssertNoFields();
                    break;

                case "ip.ttl":
                    field.AssertShowDecimal(ipV4Datagram.Ttl);
                    foreach (XElement subfield in field.Fields())
                    {
                        switch (subfield.Name())
                        {
                            case "_ws.expert":
                                break;

                            default:
                                subfield.AssertNoFields();
                                throw new InvalidOperationException(string.Format("Invalid ip subfield {0}", subfield.Name()));
                        }
                    }
                    break;

                case "ip.proto":
                    field.AssertShowDecimal((byte)ipV4Datagram.Protocol);
                    field.AssertNoFields();
                    break;

                case "ip.checksum":
                    field.AssertShowDecimal(ipV4Datagram.HeaderChecksum);
                    if (field.Showname().EndsWith(" [not all data available]"))
                    {
                        Assert.IsFalse(ipV4Datagram.IsValid);
                        break;
                    }
                    foreach (var checksumField in field.Fields())
                    {
                        switch (checksumField.Name())
                        {
                            case "ip.checksum_good":
                                checksumField.AssertNoFields();
                                // TODO: Remove this case when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10579 is fixed.
                                if (field.Showname().EndsWith(" [in ICMP error packet]"))
                                    break;
                                checksumField.AssertShowDecimal(ipV4Datagram.IsHeaderChecksumCorrect);
                                break;

                            case "ip.checksum_bad":
                                if (ipV4Datagram.Length < IpV4Datagram.HeaderMinimumLength ||
                                    ipV4Datagram.Length < ipV4Datagram.HeaderLength ||
                                    // TODO: Remove this case when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10579 is fixed.
                                    field.Showname().EndsWith(" [in ICMP error packet]"))
                                    break;

                                checksumField.AssertShowDecimal(!ipV4Datagram.IsHeaderChecksumCorrect);
                                break;
                        }
                    }
                    break;

                case "ip.src":
                case "ip.src_host":
                    field.AssertShow(ipV4Datagram.Source.ToString());
                    field.AssertNoFields();
                    break;

                case "ip.dst":
                case "ip.dst_host":
                    // TODO: Remove this condition when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10959 is fixed or (worst case) when MTU Reply and CommercialSecurity options are supported.
                    if (ipV4Datagram.Options == null || !ipV4Datagram.Options.Any(option => option.OptionType == IpV4OptionType.MaximumTransmissionUnitReply ||
                                                                                            option.OptionType == IpV4OptionType.CommercialSecurity))
                    {
                        field.AssertShow(ipV4Datagram.Destination.ToString());
                    }
                    field.AssertNoFields();
                    break;

                case "ip.addr":
                case "ip.host":
                    // TODO: Remove this condition when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10959 is fixed or (worst case) when MTU Reply and CommercialSecurity options are supported.
                    if (ipV4Datagram.Options == null || !ipV4Datagram.Options.Any(option => option.OptionType == IpV4OptionType.MaximumTransmissionUnitReply ||
                                                                                            option.OptionType == IpV4OptionType.CommercialSecurity))
                    {
                        Assert.IsTrue(field.Show() == ipV4Datagram.Source.ToString() ||
                                      field.Show() == ipV4Datagram.Destination.ToString());
                    }
                    field.AssertNoFields();
                    break;

                case "ip.cur_rt":
                case "ip.cur_rt_host":
                    field.AssertShow(ipV4Datagram.CurrentDestination.ToString());
                    break;

                case "":
                    CompareIpV4Options(field, ipV4Datagram, ipV4Datagram.Options);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid ip field {0}", field.Name()));
            }

            return true;
        }

        private static void CompareIpV4Options(XElement element, IpV4Datagram ipV4Datagram, IpV4Options options)
        {
            int currentOptionIndex = 0;
            foreach (var field in element.Fields())
            {
                if (currentOptionIndex >= options.Count)
                {
                    Assert.IsFalse(options.IsValid);
                    Assert.IsTrue(field.Show() == "Commercial IP security option" ||
                                  field.Show() == "Loose source route (length byte past end of options)" ||
                                  field.Show() == "Loose Source Route (5 bytes)" ||
                                  field.Show() == "Time stamp:" ||
                                  field.Show().StartsWith("Unknown") ||
                                  field.Show().StartsWith("Security") ||
                                  field.Show().StartsWith("Router Alert (with option length = ") ||
                                  field.Show().StartsWith("Stream ID (with option length = ") ||
                                  field.Show().Contains("with too") ||
                                  field.Show().Contains(" bytes says option goes past end of options") ||
                                  field.Show().Contains("(length byte past end of options)") ||
                                  XElementExtensions.Show(field.Fields().First()).StartsWith("Pointer: ") && XElementExtensions.Show(field.Fields().First()).EndsWith(" (points to middle of address)") ||
                                  field.Fields().Where(value => value.Show() == "(suboption would go past end of option)").Count() != 0, field.Show());
                    break;
                }
                IpV4Option option = options[currentOptionIndex++];

                switch (option.OptionType)
                {
                    case IpV4OptionType.NoOperation:
                    case IpV4OptionType.EndOfOptionList:
                        field.AssertShow(option.OptionType == IpV4OptionType.EndOfOptionList ? "End of Options List (EOL)" : "No Operation (NOP)");
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.BasicSecurity:
                        field.AssertShow("Security (" + option.Length + " bytes)");
                        var basicSecurity = (IpV4OptionBasicSecurity)option;
                        int basicSecurityFlagsIndex = 0;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                case "ip.opt.sec_cl":
                                    subfield.AssertNoFields();
                                    subfield.AssertShowDecimal((byte)basicSecurity.ClassificationLevel);
                                    break;

                                case "ip.opt.sec_prot_auth_flags":
                                    foreach (XElement flagField in subfield.Fields())
                                    {
                                        flagField.AssertNoFields();
                                        switch (flagField.Name())
                                        {
                                            case "ip.opt.sec_prot_auth_genser":
                                                flagField.AssertShowDecimal((basicSecurity.ProtectionAuthorities &
                                                                             IpV4OptionSecurityProtectionAuthorities.Genser) ==
                                                                            IpV4OptionSecurityProtectionAuthorities.Genser);
                                                break;

                                            case "ip.opt.sec_prot_auth_siop_esi":
                                                flagField.AssertShowDecimal((basicSecurity.ProtectionAuthorities &
                                                                             IpV4OptionSecurityProtectionAuthorities.
                                                                                 SingleIntegrationOptionalPlanExtremelySensitiveInformation) ==
                                                                            IpV4OptionSecurityProtectionAuthorities.
                                                                                SingleIntegrationOptionalPlanExtremelySensitiveInformation);
                                                break;

                                            case "ip.opt.sec_prot_auth_sci":
                                                flagField.AssertShowDecimal((basicSecurity.ProtectionAuthorities &
                                                                             IpV4OptionSecurityProtectionAuthorities.SensitiveCompartmentedInformation) ==
                                                                            IpV4OptionSecurityProtectionAuthorities.SensitiveCompartmentedInformation);
                                                break;

                                            case "ip.opt.sec_prot_auth_nsa":
                                                flagField.AssertShowDecimal((basicSecurity.ProtectionAuthorities & IpV4OptionSecurityProtectionAuthorities.Nsa) ==
                                                                            IpV4OptionSecurityProtectionAuthorities.Nsa);
                                                break;

                                            case "ip.opt.sec_prot_auth_doe":
                                                flagField.AssertShowDecimal((basicSecurity.ProtectionAuthorities &
                                                                             IpV4OptionSecurityProtectionAuthorities.DepartmentOfEnergy) ==
                                                                            IpV4OptionSecurityProtectionAuthorities.DepartmentOfEnergy);
                                                break;

                                            case "ip.opt.sec_prot_auth_unassigned":
                                                flagField.AssertShowDecimal(0);
                                                break;

                                            case "ip.opt.sec_prot_auth_fti":
                                                flagField.AssertShowDecimal(basicSecurity.Length - basicSecurityFlagsIndex > 4);
                                                break;

                                            default:
                                                throw new InvalidOperationException("Invalid flag field " + flagField.Name());
                                        }
                                    }
                                    ++basicSecurityFlagsIndex;
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.StreamIdentifier:
                        field.AssertShow("Stream ID (" + option.Length + " bytes): " + ((IpV4OptionStreamIdentifier)option).Identifier);
                        var streamIdentifier = (IpV4OptionStreamIdentifier)option;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                case "ip.opt.sid":
                                    subfield.AssertNoFields();
                                    subfield.AssertShowDecimal(streamIdentifier.Identifier);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;


                    case IpV4OptionType.LooseSourceRouting:
                        field.AssertShow("Loose Source Route (" + option.Length + " bytes)");

                        var looseSourceRouting = (IpV4OptionLooseSourceRouting)option;
                        int looseRouteIndex = 0;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                case "ip.opt.ptr":
                                    subfield.AssertShowDecimal(IpV4Address.SizeOf * (looseSourceRouting.PointedAddressIndex + 1));
                                    subfield.AssertNoFields();
                                    break;

                                case "ip.rec_rt":
                                case "ip.dst":
                                case "ip.addr":
                                case "ip.dst_host":
                                case "ip.src_rt":
                                    subfield.AssertShow(looseSourceRouting.Route[looseRouteIndex].ToString());
                                    subfield.AssertNoFields();
                                    break;

                                case "ip.rec_rt_host":
                                case "ip.host":
                                case "ip.src_rt_host":
                                    subfield.AssertShow(looseSourceRouting.Route[looseRouteIndex].ToString());
                                    subfield.AssertNoFields();
                                    ++looseRouteIndex;
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.RecordRoute:
                        field.AssertShow("Record Route (" + option.Length + " bytes)");

                        var recordRoute = (IpV4OptionRecordRoute)option;
                        int recordRouteIndex = 0;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                case "ip.opt.ptr":
                                    subfield.AssertShowDecimal(IpV4Address.SizeOf * (recordRoute.PointedAddressIndex + 1));
                                    subfield.AssertNoFields();
                                    break;

                                case "ip.rec_rt":
                                case "ip.empty_rt":
                                    subfield.AssertShow(recordRoute.Route[recordRouteIndex].ToString());
                                    subfield.AssertNoFields();
                                    break;

                                case "ip.rec_rt_host":
                                case "ip.empty_rt_host":
                                    subfield.AssertShow(recordRoute.Route[recordRouteIndex].ToString());
                                    subfield.AssertNoFields();
                                    ++recordRouteIndex;
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.StrictSourceRouting:
                        field.AssertShow("Strict Source Route (" + option.Length + " bytes)");

                        var strictSourceRouting = (IpV4OptionStrictSourceRouting)option;
                        int strictSourceRoutingIndex = 0;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                case "ip.opt.ptr":
                                    subfield.AssertShowDecimal(IpV4Address.SizeOf * (strictSourceRouting.PointedAddressIndex + 1));
                                    subfield.AssertNoFields();
                                    break;

                                case "ip.dst":
                                case "ip.addr":
                                case "ip.dst_host":
                                case "ip.rec_rt":
                                case "ip.src_rt":
                                    subfield.AssertShow(strictSourceRouting.Route[strictSourceRoutingIndex].ToString());
                                    subfield.AssertNoFields();
                                    break;

                                case "ip.host":
                                case "ip.rec_rt_host":
                                case "ip.src_rt_host":
                                    subfield.AssertShow(strictSourceRouting.Route[strictSourceRoutingIndex].ToString());
                                    subfield.AssertNoFields();
                                    ++strictSourceRoutingIndex;
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.RouterAlert:
                        var routerAlert = (IpV4OptionRouterAlert)option;
                        field.AssertShow("Router Alert (" + option.Length + " bytes): " +
                                         ((routerAlert.Value != 0) ? "Reserved (" + routerAlert.Value + ")" : "Router shall examine packet (0)"));
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            switch (subfield.Name())
                            {
                                case "ip.opt.ra":
                                    subfield.AssertNoFields();
                                    subfield.AssertShowDecimal(routerAlert.Value);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.TraceRoute:
                        field.AssertShow("Traceroute (" + option.Length + " bytes)");
                        var traceRoute = (IpV4OptionTraceRoute)option;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            subfield.AssertNoFields();
                            switch (subfield.Name())
                            {
                                case "ip.opt.id_number":
                                    subfield.AssertShowDecimal(traceRoute.Identification);
                                    break;
                                case "ip.opt.ohc":
                                    subfield.AssertShowDecimal(traceRoute.OutboundHopCount);
                                    break;
                                case "ip.opt.rhc":
                                    subfield.AssertShowDecimal(traceRoute.ReturnHopCount);
                                    break;
                                case "ip.opt.originator":
                                    subfield.AssertShow(traceRoute.OriginatorIpAddress.ToString());
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.InternetTimestamp:
                        field.AssertShow("Time Stamp (" + option.Length + " bytes)");
                        var timestamp = (IpV4OptionTimestamp)option;
                        int timestampIndex = 0;
                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            subfield.AssertNoFields();
                            switch (subfield.Name())
                            {
                                case "":
                                    var subfieldParts = subfield.Show().Split(new[] { ':', '=', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    string subfieldValue = subfieldParts[1].Trim();
                                    switch (subfieldParts[0].Trim())
                                    {
                                        case "Pointer":
                                            Assert.AreEqual(timestamp.PointedIndex, int.Parse(subfieldValue) / 4 - 1);
                                            break;

                                        case "Overflow":
                                            Assert.AreEqual(timestamp.Overflow.ToString(), subfieldValue);
                                            break;

                                        case "Flag":
                                            switch (timestamp.TimestampType)
                                            {
                                                case IpV4OptionTimestampType.AddressAndTimestamp:
                                                    Assert.AreEqual("Time stamp and address", subfieldValue);
                                                    break;

                                                case IpV4OptionTimestampType.TimestampOnly:
                                                    Assert.AreEqual("Time stamps only", subfieldValue);
                                                    break;

                                                case IpV4OptionTimestampType.AddressPrespecified:
                                                    Assert.AreEqual("Time stamps for prespecified addresses", subfieldValue);
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Invalid timestamp type: " + timestamp.TimestampType);
                                            }
                                            break;

                                        case "Time stamp":
                                            var timestampOnly = (IpV4OptionTimestampOnly)timestamp;
                                            Assert.AreEqual(timestampOnly.Timestamps[timestampIndex].MillisecondsSinceMidnightUniversalTime, uint.Parse(subfieldValue));
                                            ++timestampIndex;
                                            break;

                                        case "Address":
                                            Assert.AreEqual(4, subfieldParts.Length);
                                            var timestampAndAddress = (IpV4OptionTimestampAndAddress)timestamp;
                                            Assert.AreEqual(timestampAndAddress.TimedRoute[timestampIndex].Address.ToString(), subfieldParts[1].Trim());
                                            Assert.AreEqual("time stamp", subfieldParts[2].Trim());
                                            Assert.AreEqual(timestampAndAddress.TimedRoute[timestampIndex].TimeOfDay.MillisecondsSinceMidnightUniversalTime,
                                                            uint.Parse(subfieldParts[3]));
                                            ++timestampIndex;
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid subfield " + subfield.Show());
                                    }
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.QuickStart:
                        IpV4OptionQuickStart quickStart = (IpV4OptionQuickStart)option;
                        StringBuilder quickStartWireshark = new StringBuilder("Quick-Start (" + option.Length + " bytes): ");
                        quickStartWireshark.Append(quickStart.QuickStartFunction == IpV4OptionQuickStartFunction.RateRequest ? "Rate request" : "Rate report");
                        quickStartWireshark.Append(" (" + (byte)quickStart.QuickStartFunction + ")");
                        quickStartWireshark.Append(", ");
                        if (quickStart.RateKbps == 0)
                            quickStartWireshark.Append("0 bit/s");
                        else if (quickStart.RateKbps < 1024)
                            quickStartWireshark.Append(quickStart.RateKbps + " Kbit/s");
                        else if (quickStart.RateKbps < 1024 * 1024)
                            quickStartWireshark.Append(((double)quickStart.RateKbps / 1000).ToString(CultureInfo.InvariantCulture) + " Mbit/s");
                        else
                            quickStartWireshark.Append(((double)quickStart.RateKbps / 1000000).ToString(CultureInfo.InvariantCulture) + " Gbit/s");
                        if (quickStart.QuickStartFunction == IpV4OptionQuickStartFunction.RateRequest)
                            quickStartWireshark.Append(", QS TTL " + quickStart.Ttl + ", QS TTL diff " + (256 + ipV4Datagram.Ttl - quickStart.Ttl) % 256);
                        field.AssertShow(quickStartWireshark.ToString());

                        foreach (var subfield in field.Fields())
                        {
                            if (HandleCommonOptionSubfield(subfield, option))
                                continue;
                            subfield.AssertNoFields();
                            switch (subfield.Name())
                            {
                                case "ip.opt.qs_func":
                                    subfield.AssertShowDecimal((byte)quickStart.QuickStartFunction);
                                    break;
                                case "ip.opt.qs_rate":
                                    subfield.AssertShowDecimal(quickStart.Rate);
                                    break;
                                case "ip.opt.qs_ttl":
                                case "ip.opt.qs_unused":
                                    subfield.AssertShowDecimal(quickStart.Ttl);
                                    break;
                                case "ip.opt.qs_ttl_diff":
                                    subfield.AssertShowDecimal((256 + ipV4Datagram.Ttl - quickStart.Ttl) % 256);
                                    break;
                                case "ip.opt.qs_nonce":
                                    subfield.AssertShowDecimal(quickStart.Nonce);
                                    break;
                                case "ip.opt.qs_reserved":
                                    subfield.AssertShowDecimal(0);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid subfield " + subfield.Name());
                            }
                        }
                        break;

                    case IpV4OptionType.MaximumTransmissionUnitProbe:
                        // TODO: Support MTU Proble.
                        Assert.IsTrue(field.Show().StartsWith("MTU Probe (" + option.Length + " bytes): "));
                        break;

                    case (IpV4OptionType)12:
                        // TODO: Support 12.
                        if (option.Length != 4)
                            field.AssertShow("MTU Reply (with option length = " + option.Length + " bytes; should be 4)");
                        else
                            Assert.IsTrue(field.Show().StartsWith("MTU Reply (4 bytes): "));
                        break;

                    case (IpV4OptionType)133:
                        // TODO: Support 133.
                        if (option.Length >= 3)
                            field.AssertShow("Extended Security (" + option.Length + " bytes)");
                        else
                            field.AssertShow("Extended Security (with option length = " + option.Length + " bytes; should be >= 3)");
                        break;

                    case (IpV4OptionType)134:
                        // TODO: Support 134.
                        field.AssertShow("Commercial Security " +
                                         (option.Length >= 10 ? "(" + option.Length + " bytes)" : "(with option length = " + option.Length + " bytes; should be >= 10)"));
                        break;

                    case (IpV4OptionType)149:
                        // TODO: Support 149.
                        if (option.Length >= 6)
                            field.AssertShow("Selective Directed Broadcast (" + option.Length + " bytes)");
                        else
                            field.AssertShow("Selective Directed Broadcast (with option length = " + option.Length + " bytes; should be >= 6)");
                        break;

                    default:
                        field.AssertShow("Unknown (0x" + ((byte)option.OptionType).ToString("x2") + ") (" + option.Length + " bytes)");
                        field.AssertNoFields();
                        break;
                }
            }
        }

        private static bool HandleCommonOptionSubfield(XElement subfield, IpV4Option option)
        {
            switch (subfield.Name())
            {
                case "ip.opt.type":
                    subfield.AssertShowDecimal((byte)option.OptionType);
                    return true;

                case "ip.opt.len":
                    subfield.AssertShowDecimal(option.Length);
                    subfield.AssertNoFields();
                    return true;

                default:
                    return false;
            }
        }
    }
}