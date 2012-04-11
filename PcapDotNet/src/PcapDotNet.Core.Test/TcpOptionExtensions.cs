using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Core.Test
{
    internal static class TcpOptionExtensions
    {
        public static string GetWiresharkString(this TcpOption option)
        {
            switch (option.OptionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return "End of Option List (EOL)";

                case TcpOptionType.NoOperation:
                    return "No-Operation (NOP)";

                case TcpOptionType.WindowScale:
                    byte scaleFactorLog = ((TcpOptionWindowScale)option).ScaleFactorLog;
                    return string.Format("Window scale: {0} (multiply by {1})", scaleFactorLog, (1L << (scaleFactorLog % 32)));

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

                case TcpOptionType.ConnectionCount:
                    return "CC: " + ((TcpOptionConnectionCount)option).ConnectionCount;

                case TcpOptionType.ConnectionCountNew:
                    return "CC.NEW: " + ((TcpOptionConnectionCountNew)option).ConnectionCount;

                case TcpOptionType.ConnectionCountEcho:
                    return "CC.ECHO: " + ((TcpOptionConnectionCountEcho)option).ConnectionCount;

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

                case TcpOptionType.PartialOrderConnectionPermitted: // 9.
                case TcpOptionType.PartialOrderServiceProfile:      // 10.
                case TcpOptionType.AlternateChecksumRequest:        // 14.
                case TcpOptionType.AlternateChecksumData:           // 15.
                case TcpOptionType.Mood:                            // 25.
                    return string.Format("Unknown (0x{0}) ({1} bytes)", ((byte)option.OptionType).ToString("x2"), option.Length);

                default:
                    if (typeof(TcpOptionType).GetEnumValues<TcpOptionType>().Contains(option.OptionType))
                        throw new InvalidOperationException("Invalid option type " + option.OptionType);
                    return string.Format("Unknown (0x{0}) ({1} bytes)", ((byte)option.OptionType).ToString("x2"), option.Length);
            }
        }
    }
}