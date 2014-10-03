using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerTcp : WiresharkDatagramComparer
    {
        protected override string PropertyName
        {
            get { return "Tcp"; }
        }

        protected override bool CompareField(XElement field, Datagram parentDatagram, Datagram datagram)
        {
            IpDatagram ipDatagram = (IpDatagram)parentDatagram;
            TcpDatagram tcpDatagram = (TcpDatagram)datagram;

            switch (field.Name())
            {
                case "tcp.len":
                    if (tcpDatagram.Payload == null)
                    {
                        // todo seems like a bug in tshark https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=5235
                        break;
//                            field.AssertShowDecimal(tcpDatagram.Length);
                    }
                    else
                        field.AssertShowDecimal(tcpDatagram.Payload.Length);
                    field.AssertNoFields();
                    break;

                case "tcp.srcport":
                    field.AssertShowDecimal(tcpDatagram.SourcePort);
                    field.AssertNoFields();
                    break;

                case "tcp.dstport":
                    field.AssertShowDecimal(tcpDatagram.DestinationPort);
                    field.AssertNoFields();
                    break;

                case "tcp.port":
                    Assert.IsTrue(ushort.Parse(field.Show()) == tcpDatagram.SourcePort ||
                                  ushort.Parse(field.Show()) == tcpDatagram.DestinationPort);
                    field.AssertNoFields();
                    break;


                case "tcp.seq":
                    field.AssertShowDecimal(tcpDatagram.SequenceNumber);
                    field.AssertNoFields();
                    break;

                case "tcp.nxtseq":
                    field.AssertShowDecimal(tcpDatagram.NextSequenceNumber);
                    field.AssertNoFields();
                    break;

                case "tcp.ack":
                    field.AssertShowDecimal(tcpDatagram.AcknowledgmentNumber);
                    field.AssertNoFields();
                    break;

                case "tcp.hdr_len":
                    field.AssertShowDecimal(tcpDatagram.HeaderLength);
                    field.AssertNoFields();
                    break;

                case "tcp.flags":
                    ushort flags =
                        (ushort)((tcpDatagram.Reserved << 9) |
                                 (((tcpDatagram.ControlBits & TcpControlBits.NonceSum) == TcpControlBits.NonceSum ? 1 : 0) << 8) |
                                 (byte)tcpDatagram.ControlBits);
                    field.AssertShow("0x" + flags.ToString("x" + 4 * sizeof(byte)));
                    foreach (var flagField in field.Fields())
                    {
                        switch (flagField.Name())
                        {
                            case "tcp.flags.cwr":
                                flagField.AssertShowDecimal(tcpDatagram.IsCongestionWindowReduced);
                                break;

                            case "tcp.flags.ecn":
                                flagField.AssertShowDecimal(tcpDatagram.IsExplicitCongestionNotificationEcho);
                                break;

                            case "tcp.flags.urg":
                                flagField.AssertShowDecimal(tcpDatagram.IsUrgent);
                                break;

                            case "tcp.flags.ack":
                                flagField.AssertShowDecimal(tcpDatagram.IsAcknowledgment);
                                break;

                            case "tcp.flags.push":
                                flagField.AssertShowDecimal(tcpDatagram.IsPush);
                                break;

                            case "tcp.flags.reset":
                                flagField.AssertShowDecimal(tcpDatagram.IsReset);
                                break;

                            case "tcp.flags.syn":
                                flagField.AssertShowDecimal(tcpDatagram.IsSynchronize);
                                break;

                            case "tcp.flags.fin":
                                flagField.AssertShowDecimal(tcpDatagram.IsFin);
                                break;
                        }
                        flagField.AssertNoFields();
                    }
                    break;

                case "tcp.window_size_value":
                    field.AssertShowDecimal(tcpDatagram.Window);
                    field.AssertNoFields();
                    break;

                case "tcp.checksum":
                    field.AssertShowHex(tcpDatagram.Checksum);
                    IpV4Datagram ipV4Datagram = ipDatagram as IpV4Datagram;
                    if (ipV4Datagram != null && !ipV4Datagram.Options.IsBadForWireshark())
                    {
                        foreach (var checksumField in field.Fields())
                        {
                            // When TCP checksum is zero Wireshark assumes it's Checksum Offloading and puts false in both checksum_good and checksum_bad.
                            switch (checksumField.Name())
                            {
                                case "tcp.checksum_good":
                                    checksumField.AssertShowDecimal(tcpDatagram.Checksum != 0 && ipDatagram.IsTransportChecksumCorrect);
                                    break;

                                case "tcp.checksum_bad":
                                    checksumField.AssertShowDecimal(tcpDatagram.Checksum != 0 && !ipDatagram.IsTransportChecksumCorrect);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid checksum field name " + checksumField.Name());
                            }
                            checksumField.AssertNoFields();
                        }
                    }
                    break;

                case "tcp.urgent_pointer":
                    field.AssertShowDecimal(tcpDatagram.UrgentPointer);
                    field.AssertNoFields();
                    break;

                case "tcp.options":
                    CompareTcpOptions(field, tcpDatagram.Options);
                    break;

                case "tcp.stream":
                case "tcp.pdu.size":
                case "tcp.window_size":
                case "tcp.window_size_scalefactor":
                case "":
                    field.AssertNoFields();
                    break;

                default:
                    throw new InvalidOperationException("Invalid tcp field " + field.Name());
            }

            return true;
        }

        private static void CompareTcpOptions(XElement element, TcpOptions options)
        {
            int currentOptionIndex = 0;
            foreach (var field in element.Fields())
            {
                if (currentOptionIndex >= options.Count)
                {
                    Assert.IsFalse(options.IsValid, "Options IsValid");
                    Assert.IsTrue(
                        field.Show().StartsWith("Unknown (0x0a) ") || // Unknown in Wireshark but known (and invalid) in Pcap.Net
                        field.Show().StartsWith("Echo reply (with option length = ") ||
                        field.Show().Contains("bytes says option goes past end of options") ||
                        field.Show().Contains(") (with too-short option length = ") ||
                        field.Show().EndsWith(" (length byte past end of options)"),
                        "Options show: " + field.Show());
                    Assert.AreEqual(options.Count, currentOptionIndex, "Options Count");
                    return;
                }

                TcpOption option = options[currentOptionIndex];
                switch (field.Name())
                {
                    case "":
                        switch (option.OptionType)
                        {
                            case TcpOptionType.SelectiveNegativeAcknowledgements: // TODO: Support Selective Negative Acknowledgements.
                                Assert.IsTrue(field.Show().StartsWith(option.GetWiresharkString()));
                                field.AssertNoFields();
                                break;
                            
                            case (TcpOptionType)78: // TODO: Support Riverbed.
                                break;

                            default:
                                field.AssertShow(option.GetWiresharkString());
                                break;
                        }

                        switch (option.OptionType)
                        {
                            case TcpOptionType.WindowScale:
                                TcpOptionWindowScale windowScale = (TcpOptionWindowScale)option;
                                foreach (var subField in field.Fields())
                                {
                                    switch (subField.Name())
                                    {
                                        case "tcp.option_kind":
                                            subField.AssertShowDecimal((byte)windowScale.OptionType);
                                            break;

                                        case "tcp.option_len":
                                            subField.AssertShowDecimal(windowScale.Length);
                                            break;

                                        case "tcp.options.wscale.shift":
                                            subField.AssertShowDecimal(windowScale.ScaleFactorLog);
                                            break;

                                        case "tcp.options.wscale.multiplier":
                                            subField.AssertShowDecimal(1L << (windowScale.ScaleFactorLog % 32));
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid tcp options subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.SelectiveAcknowledgment:
                                var selectiveAcknowledgmentOption = (TcpOptionSelectiveAcknowledgment)option;
                                int blockIndex = 0;
                                foreach (var subField in field.Fields())
                                {
                                    switch (subField.Name())
                                    {
                                        case "tcp.options.sack":
                                            subField.AssertShowDecimal(true);
                                            break;

                                        case "tcp.options.sack_le":
                                            subField.AssertShowDecimal(selectiveAcknowledgmentOption.Blocks[blockIndex].LeftEdge);
                                            break;

                                        case "tcp.options.sack_re":
                                            subField.AssertShowDecimal(selectiveAcknowledgmentOption.Blocks[blockIndex].RightEdge);
                                            ++blockIndex;
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid tcp options subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.Timestamp:
                                var timestampOption = (TcpOptionTimestamp)option;
                                foreach (var subField in field.Fields())
                                {
                                    switch (subField.Name())
                                    {
                                        case "tcp.option_kind":
                                            subField.AssertShowDecimal((byte)option.OptionType);
                                            break;

                                        case "tcp.option_len":
                                            subField.AssertShowDecimal(option.Length);
                                            break;

                                        case "tcp.options.timestamp.tsval":
                                            subField.AssertShowDecimal(timestampOption.TimestampValue);
                                            break;

                                        case "tcp.options.timestamp.tsecr":
                                            subField.AssertShowDecimal(timestampOption.TimestampEchoReply);
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid tcp options subfield " + subField.Name());
                                    }
                                }
                                break;

                            default:
                                field.AssertNoFields();
                                break;
                        }
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.mss":
                        Assert.AreEqual(TcpOptionType.MaximumSegmentSize, option.OptionType);
                        field.AssertShowDecimal(true);
                        field.AssertNoFields();
                        break;

                    case "tcp.options.mss_val":
                        field.AssertShowDecimal(((TcpOptionMaximumSegmentSize)option).MaximumSegmentSize);
                        field.AssertNoFields();
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.echo":
                        Assert.IsTrue(option is TcpOptionEchoReply || option is TcpOptionEcho);
                        field.AssertShowDecimal(1);
                        field.AssertNoFields();
                        break;

                    case "tcp.options.cc":
                        Assert.IsTrue(option is TcpOptionConnectionCountBase);
                        field.AssertShowDecimal(1);
                        field.AssertNoFields();
                        break;

                    case "tcp.options.scps.vector":
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
                        if (field.Show() == "0")
                            ++currentOptionIndex;
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.scps":
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
                        Assert.IsFalse(field.Fields().Any());
                        break;

                    case "tcp.options.snack": // TODO: Support Selective Negative Acknowledgements.
                    case "tcp.options.snack.offset":
                    case "tcp.options.snack.size":
                        Assert.AreEqual(TcpOptionType.SelectiveNegativeAcknowledgements, option.OptionType);
                        field.AssertNoFields();
                        break;

                    case "tcp.options.sack_perm":
                        Assert.AreEqual(TcpOptionType.SelectiveAcknowledgmentPermitted, option.OptionType);
                        field.AssertNoFields();
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.rvbd.probe":
                        Assert.AreEqual((TcpOptionType)76, option.OptionType);
                        // TODO: Support Riverbed.
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.rvbd.trpy":
                        Assert.AreEqual((TcpOptionType)78, option.OptionType);
                        // TODO: Support Riverbed.
                        ++currentOptionIndex;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid tcp options field " + field.Name());
                }
            }
        }
    }
}