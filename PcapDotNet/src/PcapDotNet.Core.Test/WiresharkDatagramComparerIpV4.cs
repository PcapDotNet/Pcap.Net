using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
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
                    break;

                case "ip.hdr_len":
                    field.AssertShowDecimal(ipV4Datagram.HeaderLength);
                    break;

                case "ip.dsfield":
                    field.AssertShowDecimal((int)ipV4Datagram.TypeOfService);
                    break;

                case "ip.len":
                    field.AssertShowDecimal(ipV4Datagram.TotalLength);
                    break;

                case "ip.id":
                    field.AssertShowHex(ipV4Datagram.Identification);
                    break;

                case "ip.flags":
                    field.AssertShowHex((byte)((ushort)ipV4Datagram.Fragmentation.Options >> 13));
                    break;

                case "ip.frag_offset":
                    field.AssertShowDecimal(ipV4Datagram.Fragmentation.Offset);
                    break;

                case "ip.ttl":
                    field.AssertShowDecimal(ipV4Datagram.Ttl);
                    break;

                case "ip.proto":
                    field.AssertShowDecimal((byte)ipV4Datagram.Protocol);
                    break;

                case "ip.checksum":
                    field.AssertShowHex(ipV4Datagram.HeaderChecksum);
                    foreach (var checksumField in field.Fields())
                    {
                        switch (checksumField.Name())
                        {
                            case "ip.checksum_good":
                                checksumField.AssertShowDecimal(ipV4Datagram.IsHeaderChecksumCorrect);
                                break;

                            case "ip.checksum_bad":
                                if (ipV4Datagram.Length < IpV4Datagram.HeaderMinimumLength ||
                                    ipV4Datagram.Length < ipV4Datagram.HeaderLength)
                                    break;

                                checksumField.AssertShowDecimal(!ipV4Datagram.IsHeaderChecksumCorrect);
                                break;
                        }
                    }
                    break;

                case "ip.src":
                case "ip.src_host":
                    field.AssertShow(ipV4Datagram.Source.ToString());
                    break;

                case "ip.dst":
                case "ip.dst_host":
                    if (field.Show() != ipV4Datagram.Destination.ToString())
                    {
                        // TODO: Remove this fallback once https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=7043 is fixed.
                        field.AssertShow(ipV4Datagram.CurrentDestination.ToString());
                        Assert.IsTrue(ipV4Datagram.Options.IsBadForWireshark(),
                                      string.Format("Expected destination: {0}. Destination: {1}. Current destination: {2}.", field.Show(),
                                                    ipV4Datagram.Destination, ipV4Datagram.CurrentDestination));
                    }
                    break;

                case "ip.addr":
                case "ip.host":
                    Assert.IsTrue(field.Show() == ipV4Datagram.Source.ToString() ||
                                  field.Show() == ipV4Datagram.Destination.ToString() ||
                                  // TODO: Remove this fallback once https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=7043 is fixed.
                                  field.Show() == ipV4Datagram.CurrentDestination.ToString() && ipV4Datagram.Options.IsBadForWireshark(),
                                  string.Format("Expected ip: {0}. ", field.Show()) +
                                  (ipV4Datagram.IsValid
                                       ? string.Format("Source: {0}. Destination: {1}. Current destination: {2}.", ipV4Datagram.Source,
                                                       ipV4Datagram.Destination, ipV4Datagram.CurrentDestination)
                                       : ""));
                    break;

                case "":
                    CompareIpV4Options(field, ipV4Datagram.Options);
                    break;

                default:
                    throw new InvalidOperationException("Invalid ip field " + field.Name());
            }

            return true;
        }

        private static void CompareIpV4Options(XElement element, IpV4Options options)
        {
            int currentOptionIndex = 0;
            foreach (var field in element.Fields())
            {
                if (currentOptionIndex >= options.Count)
                {
                    Assert.IsFalse(options.IsValid);
                    Assert.IsTrue(field.Show() == "Commercial IP security option" ||
                                  field.Show() == "Loose source route (length byte past end of options)" ||
                                  field.Show() == "Time stamp:" ||
                                  field.Show().StartsWith("Unknown") ||
                                  field.Show().StartsWith("Security") ||
                                  field.Show().StartsWith("Router Alert (with option length = ") ||
                                  field.Show().StartsWith("Stream identifier (with option length = ") ||
                                  field.Show().Contains("with too") ||
                                  field.Show().Contains(" bytes says option goes past end of options") ||
                                  field.Show().Contains("(length byte past end of options)") ||
                                  XElementExtensions.Show(field.Fields().First()).StartsWith("Pointer: ") && XElementExtensions.Show(field.Fields().First()).EndsWith(" (points to middle of address)") ||
                                  field.Fields().Where(value => value.Show() == "(suboption would go past end of option)").Count() != 0, field.Show());
                    break;
                }
                IpV4Option option = options[currentOptionIndex++];
                if (option.OptionType == IpV4OptionType.BasicSecurity ||
                    option.OptionType == IpV4OptionType.TraceRoute)
                {
                    Assert.IsTrue(field.Show().StartsWith(option.GetWiresharkString()));
                    continue; // Wireshark doesn't support 
                }
                field.AssertShow(option.GetWiresharkString());

                if ((option is IpV4OptionUnknown))
                    continue;

                var optionShows = from f in field.Fields() select f.Show();
                MoreAssert.AreSequenceEqual(optionShows, option.GetWiresharkSubfieldStrings());
            }
        }
    }
}