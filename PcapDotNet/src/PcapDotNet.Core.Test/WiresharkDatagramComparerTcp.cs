using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
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
                    field.AssertShowDecimal(tcpDatagram.Length - tcpDatagram.HeaderLength);
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
                    foreach (XElement subfield in field.Fields())
                    {
                        switch (subfield.Name())
                        {
                            case "_ws.expert":
                                break;

                            default:
                                subfield.AssertNoFields();
                                throw new InvalidOperationException("Invalid TCP subfield name " + subfield.Name());
                        }
                    }
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
                    field.AssertShowDecimal(flags);
                    foreach (var flagField in field.Fields())
                    {
                        switch (flagField.Name())
                        {
                            case "tcp.flags.res":
                                flagField.AssertNoFields();
                                break;

                            case "tcp.flags.ns":
                                flagField.AssertNoFields();
                                flagField.AssertShowDecimal(tcpDatagram.IsNonceSum);
                                break;

                            case "tcp.flags.cwr":
                                flagField.AssertNoFields();
                                flagField.AssertShowDecimal(tcpDatagram.IsCongestionWindowReduced);
                                break;

                            case "tcp.flags.ecn":
                                flagField.AssertNoFields();
                                flagField.AssertShowDecimal(tcpDatagram.IsExplicitCongestionNotificationEcho);
                                break;

                            case "tcp.flags.urg":
                                flagField.AssertNoFields();
                                flagField.AssertShowDecimal(tcpDatagram.IsUrgent);
                                break;

                            case "tcp.flags.ack":
                                flagField.AssertNoFields();
                                flagField.AssertShowDecimal(tcpDatagram.IsAcknowledgment);
                                break;

                            case "tcp.flags.push":
                                flagField.AssertNoFields();
                                flagField.AssertShowDecimal(tcpDatagram.IsPush);
                                break;

                            case "tcp.flags.reset":
                                flagField.AssertShowDecimal(tcpDatagram.IsReset);
                                foreach (XElement subfield in flagField.Fields())
                                {
                                    switch (subfield.Name())
                                    {
                                        case "_ws.expert":
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid TCP subfield name " + subfield.Name());
                                    }
                                }
                                break;

                            case "tcp.flags.syn":
                                flagField.AssertShowDecimal(tcpDatagram.IsSynchronize);
                                foreach (XElement subfield in flagField.Fields())
                                {
                                    switch (subfield.Name())
                                    {
                                        case "_ws.expert":
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid TCP subfield name " + subfield.Name());
                                    }
                                }
                                break;

                            case "tcp.flags.fin":
                                flagField.AssertShowDecimal(tcpDatagram.IsFin);
                                foreach (XElement subfield in flagField.Fields())
                                {
                                    switch (subfield.Name())
                                    {
                                        case "_ws.expert":
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid TCP subfield name " + subfield.Name());
                                    }
                                }
                                break;

                            default:
                                throw new InvalidOperationException("Invalid TCP flag field name " + flagField.Name());
                        }
                    }
                    break;

                case "tcp.window_size_value":
                    field.AssertShowDecimal(tcpDatagram.Window);
                    field.AssertNoFields();
                    break;

                case "tcp.checksum":
                    field.AssertShowDecimal(tcpDatagram.Checksum);
                    IpV4Datagram ipV4Datagram = ipDatagram as IpV4Datagram;
                    if (ipV4Datagram != null)
                    {
                        foreach (var checksumField in field.Fields())
                        {
                            // When TCP checksum is zero Wireshark assumes it's Checksum Offloading and puts false in both checksum_good and checksum_bad.
                            switch (checksumField.Name())
                            {
                                case "tcp.checksum_good":
                                    checksumField.AssertNoFields();
                                    checksumField.AssertShowDecimal(tcpDatagram.Checksum != 0 && ipDatagram.IsTransportChecksumCorrect);
                                    break;

                                case "tcp.checksum_bad":
                                    checksumField.AssertShowDecimal(!ipDatagram.IsTransportChecksumCorrect);
                                    if (checksumField.Fields().Any())
                                    {
                                        checksumField.AssertNumFields(1);
                                        checksumField.Fields().First().AssertName("_ws.expert");
                                    }
                                    break;

                                case "tcp.checksum_calculated":
                                    checksumField.AssertNoFields();
                                    if (ipDatagram.IsTransportChecksumCorrect)
                                        checksumField.AssertShowDecimal(tcpDatagram.Checksum);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid checksum field name " + checksumField.Name());
                            }
                        }
                    }
                    break;

                case "tcp.urgent_pointer":
                    field.AssertShowDecimal(tcpDatagram.UrgentPointer);
                    break;

                case "tcp.options":
                    CompareTcpOptions(field, tcpDatagram.Options);
                    break;

                case "tcp.stream":
                case "tcp.pdu.size":
                case "tcp.window_size":
                case "tcp.window_size_scalefactor":
                    field.AssertNoFields();
                    break;

                case "":
                    if (field.Show() == "Short segment. Segment/fragment does not contain a full TCP header (might be NMAP or someone else deliberately sending unusual packets)")
                    {
                        field.AssertNumFields(1);
                        field.Fields().First().AssertName("_ws.expert");
                    }
                    else
                    {
                        field.AssertNoFields();
                    }
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
                if (field.Name() == "_ws.expert")
                    continue;
                if (currentOptionIndex >= options.Count)
                {
                    Assert.IsFalse(options.IsValid, "Options IsValid");
                    Assert.IsTrue(
                        field.Show().StartsWith("Unknown (0x09) ") || // Unknown in Wireshark but known (and invalid) in Pcap.Net.
                        field.Show().StartsWith("Unknown (0x0a) ") || // Unknown in Wireshark but known (and invalid) in Pcap.Net.
                        field.Show().StartsWith("Unknown (0x19) ") || // Unknown in Wireshark but known (and invalid) in Pcap.Net.
                        field.Show().StartsWith("Unknown (0x2d) ") || // Unknown in Wireshark and unknown and invalid in Pcap.Net.
                        field.Show().StartsWith("Unknown (0x84) ") || // Unknown in Wireshark and unknown and invalid in Pcap.Net.
                        field.Show().StartsWith("Unknown (0xa9) ") || // Unknown in Wireshark and unknown and invalid in Pcap.Net.
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
                            case TcpOptionType.WindowScale:
                                TcpOptionWindowScale windowScale = (TcpOptionWindowScale)option;
                                byte scaleFactorLog = windowScale.ScaleFactorLog;
                                field.AssertShow(string.Format("Window scale: {0} (multiply by {1})", scaleFactorLog, (1L << (scaleFactorLog % 32))));
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    switch (subField.Name())
                                    {
                                        case "tcp.options.wscale.shift":
                                            subField.AssertShowDecimal(windowScale.ScaleFactorLog);
                                            break;

                                        case "tcp.options.wscale.multiplier":
                                            subField.AssertShowDecimal(1L << (windowScale.ScaleFactorLog % 32));
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.SelectiveAcknowledgment:
                                var selectiveAcknowledgmentOption = (TcpOptionSelectiveAcknowledgment)option;
                                IEnumerable<TcpOptionSelectiveAcknowledgmentBlock> blocks = selectiveAcknowledgmentOption.Blocks;
                                field.AssertShow("SACK:" + (blocks.Count() == 0
                                                                ? string.Empty
                                                                : ((TcpOptionSelectiveAcknowledgment)option).Blocks.SequenceToString(" ", " ")));
                                int blockIndex = 0;
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    subField.AssertNoFields();
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

                                        case "tcp.options.sack.count":
                                            subField.AssertShowDecimal(selectiveAcknowledgmentOption.Blocks.Count);
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.Timestamp:
                                var timestampOption = (TcpOptionTimestamp)option;
                                field.AssertShow("Timestamps: TSval " + timestampOption.TimestampValue + ", TSecr " + timestampOption.TimestampEchoReply);
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    subField.AssertNoFields();
                                    switch (subField.Name())
                                    {
                                        case "tcp.options.timestamp.tsval":
                                            subField.AssertShowDecimal(timestampOption.TimestampValue);
                                            break;

                                        case "tcp.options.timestamp.tsecr":
                                            subField.AssertShowDecimal(timestampOption.TimestampEchoReply);
                                            break;

                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.ConnectionCount:
                                field.AssertShow("CC: " + ((TcpOptionConnectionCount)option).ConnectionCount);
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    subField.AssertNoFields();
                                    switch (subField.Name())
                                    {
                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.Echo:
                                field.AssertShow("Echo: " + ((TcpOptionEcho)option).Info);
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    subField.AssertNoFields();
                                    switch (subField.Name())
                                    {
                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.ConnectionCountNew:
                                field.AssertShow("CC.NEW: " + ((TcpOptionConnectionCountNew)option).ConnectionCount);
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    subField.AssertNoFields();
                                    switch (subField.Name())
                                    {
                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.EndOfOptionList:
                                field.AssertShow("End of Option List (EOL)");
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    switch (subField.Name())
                                    {
                                        default:
                                            subField.AssertNoFields();
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.ConnectionCountEcho:
                                field.AssertShow("CC.ECHO: " + ((TcpOptionConnectionCountEcho)option).ConnectionCount);
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    subField.AssertNoFields();
                                    switch (subField.Name())
                                    {
                                        default:
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.Md5Signature:
                                field.AssertShow("TCP MD5 signature");
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    switch (subField.Name())
                                    {
                                        case "tcp.options.type":
                                            subField.AssertShowDecimal((byte)TcpOptionType.Md5Signature);
                                            break;

                                        default:
                                            subField.AssertNoFields();
                                            throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                    }
                                }
                                break;

                            case TcpOptionType.NoOperation:
                                field.AssertShow("No-Operation (NOP)");
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                }
                                break;

                            case TcpOptionType.EchoReply:
                                field.AssertShow("Echo reply: " + ((TcpOptionEchoReply)option).Info);
                                foreach (var subField in field.Fields())
                                {
                                    if (HandleOptionCommonFields(subField, option))
                                        continue;
                                    throw new InvalidOperationException("Invalid tcp option subfield " + subField.Name());
                                }
                                break;

                            case TcpOptionType.SelectiveAcknowledgmentPermitted:
                                field.AssertShow("SACK permitted");
                                field.AssertNoFields();
                                break;

                            case TcpOptionType.SelectiveNegativeAcknowledgements: // TODO: Support Selective Negative Acknowledgements.
                                Assert.IsTrue(field.Show().StartsWith("SACK permitted"));
                                field.AssertNoFields();
                                break;

                            case (TcpOptionType)20:
                                // TODO: Support 20.
                                field.AssertShow("SCPS capabilities" + (option.Length >= 4
                                                                            ? string.Empty
                                                                            : " (with option length = " + option.Length + " bytes; should be >= 4)"));
                                break;

                            case (TcpOptionType)22:
                                field.AssertShow("SCPS record boundary (with option length = " + option.Length + " bytes; should be 2)");
                                // TODO: Support 22.
                                break;

                            case (TcpOptionType)23:
                                field.AssertShow("SCPS corruption experienced (with option length = " + option.Length + " bytes; should be 2)");
                                // TODO: Support 23.
                                break;

                            case (TcpOptionType)30:
                                // TODO: Support 30.
                                Assert.IsTrue(field.Show().StartsWith("Multipath TCP"));
                                break;

                            case (TcpOptionType)78:
                                field.AssertShow("Riverbed Transparency (with option length = " + option.Length + " bytes; should be 16)");
                                // TODO: Support 78 - Support Riverbed.
                                break;

                            case TcpOptionType.PartialOrderConnectionPermitted: // 9.
                            case TcpOptionType.PartialOrderServiceProfile:      // 10.
                            case TcpOptionType.AlternateChecksumRequest:        // 14.
                            case TcpOptionType.AlternateChecksumData:           // 15.
                            case TcpOptionType.Mood:                            // 25.
                            case TcpOptionType.TcpAuthentication:               // 29.
                                field.AssertShow(string.Format("Unknown (0x{0}) ({1} bytes)", ((byte)option.OptionType).ToString("x2"), option.Length));
                                field.AssertNoFields();
                                break;

                            default:
                                field.AssertNoFields();
                                field.AssertShow("Unknown (0x" + ((byte)option.OptionType).ToString("x") + ") (" + option.Length + " bytes)");
                                break;
                        }
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.mss":
                        Assert.AreEqual(TcpOptionType.MaximumSegmentSize, option.OptionType);
                        var maximumSegmentSize = (TcpOptionMaximumSegmentSize)option;
                        field.AssertShowname("Maximum segment size: " + maximumSegmentSize.MaximumSegmentSize + " bytes");
                        foreach (var subField in field.Fields())
                        {
                            if (HandleOptionCommonFields(subField, option))
                                continue;
                            subField.AssertNoFields();
                            switch (subField.Name())
                            {
                                case "tcp.options.mss_val":
                                    subField.AssertShowDecimal(maximumSegmentSize.MaximumSegmentSize);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid tcp options subfield " + subField.Name());
                            }
                        }
                        ++currentOptionIndex;
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
                        // TODO: Support 20.
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
//                        if (field.Show() == "0")
//                            ++currentOptionIndex;
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.scps":
                        // TODO: Support 20.
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
                        foreach (var subField in field.Fields())
                        {
                            if (HandleOptionCommonFields(subField, option))
                                continue;
                            subField.AssertNoFields();
                            throw new InvalidOperationException("Invalid tcp options subfield " + subField.Name());
                        }
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

                    case "tcp.options.experimental":
                        Assert.IsTrue(new []{(TcpOptionType)253, (TcpOptionType)254}.Contains(option.OptionType));
                        // TODO: Support Experimental.
                        ++currentOptionIndex;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid tcp options field " + field.Name());
                }
            }
        }

        private static bool HandleOptionCommonFields(XElement subfield, TcpOption option)
        {
            switch (subfield.Name())
            {
                case "tcp.option_kind":
                    subfield.AssertNoFields();
                    subfield.AssertShowDecimal((byte)option.OptionType);
                    return true;

                case "tcp.options.type":
                    subfield.AssertShowDecimal((byte)option.OptionType);
                    return true;

                case "tcp.option_len":
                    subfield.AssertNoFields();
                    subfield.AssertShowDecimal(option.Length);
                    return true;

                default:
                    return false;
            }
        }
    }
}