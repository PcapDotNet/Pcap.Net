using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;
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

                case TcpOptionType.TimeStamp:
                    TcpOptionTimeStamp timeStampOption = (TcpOptionTimeStamp)option;
                    return "Timestamps: TSval " + timeStampOption.TimeStampValue + ", TSecr " + timeStampOption.TimeStampEchoReply;

                case TcpOptionType.PartialOrderServiceProfile:
                    return "Unknown (0x0a) (3 bytes)";

                case TcpOptionType.PartialOrderConnectionPermitted:
                    return "Unknown (0x09) (2 bytes)";

                default:
                    throw new InvalidOperationException("Illegal option type " + option.OptionType);
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
                case TcpOptionType.TimeStamp:
                case TcpOptionType.PartialOrderServiceProfile:
                case TcpOptionType.PartialOrderConnectionPermitted:
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
                    throw new InvalidOperationException("Illegal option type " + option.OptionType);
            }
        }
    }
}