using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Core.Test
{
    internal static class MoreTcpOption
    {
        public static string GetWiresharkString(this TcpOption option)
        {
            switch (option.OptionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return "EOL";

                case TcpOptionType.NoOperation:
                    return "NOP";

                case TcpOptionType.SelectiveAcknowledgmentPermitted:
                    return "SACK permitted";

                case TcpOptionType.SelectiveAcknowledgment:
                    IEnumerable<TcpOptionSelectiveAcknowledgmentBlock> blocks = ((TcpOptionSelectiveAcknowledgment)option).Blocks;
                    return "SACK:" + (blocks.Count() == 0
                                          ? string.Empty
                                          : ((TcpOptionSelectiveAcknowledgment)option).Blocks.SequenceToString(" ", " "));

                case TcpOptionType.Echo:
                    return "Echo: " + ((TcpOptionEcho)option).Info;

                case TcpOptionType.EchoReply:
                    return "Echo reply: " + ((TcpOptionEchoReply)option).Info;

                case TcpOptionType.Timestamp:
                    TcpOptionTimestamp timestampOption = (TcpOptionTimestamp)option;
                    return "Timestamps: TSval " + timestampOption.TimestampValue + ", TSecr " + timestampOption.TimestampEchoReply;

                case TcpOptionType.PartialOrderServiceProfile:
                    return "Unknown (0x0a) (3 bytes)";

                case TcpOptionType.PartialOrderConnectionPermitted:
                    return "Unknown (0x09) (2 bytes)";

                case TcpOptionType.ConnectionCount:
                    return "CC: " + ((TcpOptionConnectionCount)option).ConnectionCount;

                case TcpOptionType.ConnectionCountNew:
                    return "CC.NEW: " + ((TcpOptionConnectionCountNew)option).ConnectionCount;

                case TcpOptionType.ConnectionCountEcho:
                    return "CC.ECHO: " + ((TcpOptionConnectionCountEcho)option).ConnectionCount;

                case TcpOptionType.AlternateChecksumRequest:
                    return "Unknown (0x0e) (3 bytes)";

                case TcpOptionType.AlternateChecksumData:
                    return "Unknown (0x0f) (" + option.Length + " bytes)";

                case TcpOptionType.Md5Signature:
                    return "TCP MD5 signature";

                case (TcpOptionType)20:
                    return "SCPS capabilities" + (option.Length >= 4
                                                         ? string.Empty
                                                         : " (with option length = " + option.Length + " bytes; should be >= 4)");

                case (TcpOptionType)21:
                    return "Selective Negative Acknowledgement" + (option.Length == 6
                                                         ? string.Empty
                                                         : " (with option length = " + option.Length + " bytes; should be 6)");
                    
                case (TcpOptionType)22:
                    return "SCPS record boundary" + (option.Length == 2
                                                         ? string.Empty
                                                         : " (with option length = " + option.Length + " bytes; should be 2)");

                case (TcpOptionType)23:
                    return "SCPS corruption experienced" + (option.Length == 2
                                                                ? string.Empty
                                                                : " (with option length = " + option.Length + " bytes; should be 2)");

                default:
                    if (typeof(TcpOptionType).GetEnumValues<TcpOptionType>().Contains(option.OptionType))
                        throw new InvalidOperationException("Invalid option type " + option.OptionType);
                    return "Unknown (0x" + ((byte)option.OptionType).ToString("x2") + ") (" + option.Length + " bytes)";
            }
        }

        public static IEnumerable<string> GetWiresharkSubfieldStrings(this TcpOption option)
        {
            switch (option.OptionType)
            {
                case TcpOptionType.EndOfOptionList:
                case TcpOptionType.NoOperation:
                case TcpOptionType.MaximumSegmentSize:
                case TcpOptionType.WindowScale:
                case TcpOptionType.SelectiveAcknowledgmentPermitted:
                case TcpOptionType.Echo:
                case TcpOptionType.EchoReply:
                case TcpOptionType.Timestamp:
                case TcpOptionType.PartialOrderServiceProfile:
                case TcpOptionType.PartialOrderConnectionPermitted:
                case TcpOptionType.ConnectionCount:
                case TcpOptionType.ConnectionCountNew:
                case TcpOptionType.ConnectionCountEcho:
                case TcpOptionType.AlternateChecksumRequest:
                case TcpOptionType.AlternateChecksumData:
                case TcpOptionType.Md5Signature:
                case TcpOptionType.Mood:
                    break;

                case TcpOptionType.SelectiveAcknowledgment:
                    var blocks = ((TcpOptionSelectiveAcknowledgment)option).Blocks;
                    if (blocks.Count() == 0)
                        break;
                    yield return "1";
                    foreach (TcpOptionSelectiveAcknowledgmentBlock block in blocks)
                    {
                        yield return block.LeftEdge.ToString();
                        yield return block.RightEdge.ToString();
                    }
                    break;

                default:
                    if (typeof(TcpOptionType).GetEnumValues<TcpOptionType>().Contains(option.OptionType))
                        throw new InvalidOperationException("Invalid option type " + option.OptionType);
                    break;
            }
        }
    }
}