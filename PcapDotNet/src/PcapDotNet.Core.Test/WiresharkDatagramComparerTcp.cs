using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
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
            IpV4Datagram ipV4Datagram = (IpV4Datagram)parentDatagram;
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
                    break;

                case "tcp.srcport":
                    field.AssertShowDecimal(tcpDatagram.SourcePort);
                    break;

                case "tcp.dstport":
                    field.AssertShowDecimal(tcpDatagram.DestinationPort);
                    break;

                case "tcp.port":
                    Assert.IsTrue(ushort.Parse(field.Show()) == tcpDatagram.SourcePort ||
                                  ushort.Parse(field.Show()) == tcpDatagram.DestinationPort);
                    break;


                case "tcp.seq":
                    field.AssertShowDecimal(tcpDatagram.SequenceNumber);
                    break;

                case "tcp.nxtseq":
                    field.AssertShowDecimal(tcpDatagram.NextSequenceNumber);
                    break;

                case "tcp.ack":
                    field.AssertShowDecimal(tcpDatagram.AcknowledgmentNumber);
                    break;

                case "tcp.hdr_len":
                    field.AssertShowDecimal(tcpDatagram.HeaderLength);
                    break;

                case "tcp.flags":
                    ushort flags =
                        (ushort)((tcpDatagram.Reserved << 9) |
                                 (((tcpDatagram.ControlBits & TcpControlBits.NonceSum) == TcpControlBits.NonceSum ? 1 : 0) << 8) |
                                 (byte)tcpDatagram.ControlBits);
                    field.AssertShow("0x" + flags.ToString("x" + 2 * sizeof(byte)));
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
                    }
                    break;

                case "tcp.window_size":
                    field.AssertShowDecimal(tcpDatagram.Window);
                    break;

                case "tcp.checksum":
                    field.AssertShowHex(tcpDatagram.Checksum);
                    foreach (var checksumField in field.Fields())
                    {
                        switch (checksumField.Name())
                        {
                            case "tcp.checksum_good":
                                checksumField.AssertShowDecimal(ipV4Datagram.IsTransportChecksumCorrect);
                                break;

                            case "tcp.checksum_bad":
                                checksumField.AssertShowDecimal(!ipV4Datagram.IsTransportChecksumCorrect);
                                break;
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
                case "":
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
                        field.Show().Contains("bytes says option goes past end of options"), "Options show: " + field.Show());
                    Assert.AreEqual(options.Count, currentOptionIndex, "Options Count");
                    return;
                }

                TcpOption option = options[currentOptionIndex];
                switch (field.Name())
                {
                    case "":
                        if (option.OptionType == (TcpOptionType)21)
                            Assert.IsTrue(field.Show().StartsWith(option.GetWiresharkString()));
                        else
                            field.AssertShow(option.GetWiresharkString());
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.mss":
                        field.AssertShowDecimal(option is TcpOptionMaximumSegmentSize);
                        break;

                    case "tcp.options.mss_val":
                        field.AssertShowDecimal(((TcpOptionMaximumSegmentSize)option).MaximumSegmentSize);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.wscale":
                        field.AssertShowDecimal(option is TcpOptionWindowScale);
                        break;

                    case "tcp.options.wscale_val":
                        field.AssertShowDecimal(((TcpOptionWindowScale)option).ScaleFactorLog);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.echo":
                        Assert.IsTrue(option is TcpOptionEchoReply || option is TcpOptionEcho);
                        field.AssertShowDecimal(1);
                        break;

                    case "tcp.options.time_stamp":
                        Assert.IsTrue(option is TcpOptionTimestamp);
                        field.AssertShowDecimal(1);
                        break;

                    case "tcp.options.cc":
                        Assert.IsTrue(option is TcpOptionConnectionCountBase);
                        field.AssertShowDecimal(1);
                        break;

                    case "tcp.options.scps.vector":
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
                        if (field.Show() == "0")
                            ++currentOptionIndex;
                        break;

                    case "tcp.options.scps":
                        Assert.AreEqual((TcpOptionType)20, option.OptionType);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.snack":
                    case "tcp.options.snack.offset":
                    case "tcp.options.snack.size":
                        Assert.AreEqual((TcpOptionType)21, option.OptionType);
                        break;

                    case "tcp.options.sack_perm":
                        Assert.AreEqual(TcpOptionType.SelectiveAcknowledgmentPermitted, option.OptionType);
                        ++currentOptionIndex;
                        break;

                    case "tcp.options.mood":
                        Assert.AreEqual(TcpOptionType.Mood, option.OptionType);
                        field.AssertValue(Encoding.ASCII.GetBytes(((TcpOptionMood)option).EmotionString));
                        break;

                    case "tcp.options.mood_val":
                        Assert.AreEqual(TcpOptionType.Mood, option.OptionType);
                        field.AssertShow(((TcpOptionMood)option).EmotionString);
                        ++currentOptionIndex;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid tcp options field " + field.Name());
                }

                if ((option is TcpOptionUnknown))
                    continue;
                
                var optionShows = from f in field.Fields() select f.Show();
                MoreAssert.AreSequenceEqual(optionShows, option.GetWiresharkSubfieldStrings());
            }
        }
    }
}